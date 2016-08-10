using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Data.Common;
using jlib.db;

namespace PerformanceInsights.Deployment.Tasks
{
	public class SqlChangeScripts : Microsoft.Build.Utilities.Task
	{
		#region ***** PROPERTIES *****
		[Required]
		public string ConnectionString { get; set; }

		[Required]
		public ITaskItem[] Files { get; set; }

		public bool CheckScriptStatus { get; set; }
		public string BatchSeparator { get; set; }

		public bool ExecuteInTransaction { get; set; }

		[Output]
		public int[] Results { get { return this._Results.ToArray(); } }
		private List<int> _Results = new List<int>();

		private const RegexOptions REGEX_OPTIONS = RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.Singleline;
		#endregion

		#region ***** CONSTRUCTORS *****
		public SqlChangeScripts()
		{
			this.BatchSeparator = "GO";
			this.ExecuteInTransaction = true;
		}
		#endregion

		#region ***** PUBLIC METHODS *****
		public override bool Execute()
		{
			try
			{
				base.Log.LogMessage("Starting SqlChangeScript task...");
				base.Log.LogMessage("Using {0}", this.ConnectionString);

				//If Check Sql Script Status Enabled, verify tracker tables exist
				if (this.CheckScriptStatus)
				{
					base.Log.LogMessage("Verifing that the SqlChangeScript table exists");
					this.VerifySqlScriptTrackerTables();
				}

				//Process Each File
				foreach (ITaskItem item in this.Files)
				{
					string scriptPath = item.ItemSpec;

					//Read Contents
					string sqlScript = File.ReadAllText(scriptPath);

					//Get Script File Name
					string scriptName = Path.GetFileName(scriptPath);

					//Check to see if already executed
					bool runScript = true;
					if (this.CheckScriptStatus)
					{
						if (this.ScriptHasAlreadyRun(scriptName))
						{
							base.Log.LogMessage("\tScript ('{0}') has already been run on this database.", scriptName);
							runScript = false;
						}
					}

					//Execute
					bool scriptStatus = false;
					if (runScript)
					{
						scriptStatus = this.ExecuteSql(scriptName, sqlScript);

						//Track Script Status
						if(this.CheckScriptStatus) this.LogSqlScriptTracker(scriptName, scriptStatus);

						//Status
						if (scriptStatus) base.Log.LogMessage("\tScript ('{0}') succeeded!", scriptName);
						else
						{
							base.Log.LogError("\tScript ('{0}') failed!", scriptName);
							return false;
						}
					}

					
				}

				//Return Successful
				return true;

			}
			catch (Exception e)
			{
				base.Log.LogErrorFromException(e);
				return false;
			}
			
		}

		#endregion

		#region ***** HELPER METHODS *****
		private SqlHelper GetSqlHelper()
		{
			return new SqlHelper(this.ConnectionString);
		}

		private void VerifySqlScriptTrackerTables()
		{
			this.GetSqlHelper().ExecuteSql(Resources.ResourceHelper.SqlScriptTracker);
		}


		private bool ScriptHasAlreadyRun(string ScriptName)
		{
			DataTable dtResults = this.GetSqlHelper().ExecuteSql("SELECT * FROM [SqlScriptTracker] WHERE ScriptName=@ScriptName AND IsSuccessful=1", "@ScriptName", ScriptName);

			//If row found, return true
			return dtResults.Rows.Count > 0;
		}

		private bool ExecuteSql(string ScriptName, string SqlScript)
		{
			try
			{
				//Append Trigger Disable Context to script
				if(this.ExecuteInTransaction && !SqlScript.Trim().StartsWith("ALTER VIEW", StringComparison.CurrentCultureIgnoreCase)) SqlScript = "SET Context_Info 0x55555;\n\n" + SqlScript + "";

				//Strip out perm line
				SqlScript = Regex.Replace(SqlScript, @"select\s*Has_Perms_By_Name.*?as\sContr_Per", "\n", SqlChangeScripts.REGEX_OPTIONS);
				
				//If contains go
				if(Regex.IsMatch(SqlScript, @"^\s*GO\s*$", SqlChangeScripts.REGEX_OPTIONS))
				{
					base.Log.LogMessage("\tScript ('{0}') has multiple parts:", ScriptName);

					//Get Parts
					string[] scriptParts = Regex.Split(SqlScript, @"^\s*GO\s*$", SqlChangeScripts.REGEX_OPTIONS);

					//Create a connection with a transaction
					SqlHelper sqlHelper = this.GetSqlHelper();
					DbConnection sqlConnection = sqlHelper.Connect(this.ExecuteInTransaction);

					//Process Each Part
					int partIndex = 0;
					foreach (string scriptPart in scriptParts)
					{
						partIndex++;

						//If script doesn't contain execution content
						if (String.IsNullOrWhiteSpace(scriptPart)) continue;

						try
						{
							sqlHelper.ExecuteSql(sqlConnection, scriptPart);
						}

						//Throw new exception about specific part
						catch (Exception e)
						{
							throw new Exception(String.Format("ExecuteSql failed on multi-statement file '{0}' on part {1} because: {2}\n\nSQL Command\n{3}", ScriptName, partIndex, e.Message, scriptPart), e);
						}

					}

					//close connection, commit
					sqlHelper.Disconnect();
				}

				//else, just run script
				else
					this.GetSqlHelper().ExecuteSql(SqlScript);

				//return successful
				return true;
			}
			catch(Exception e)
			{
				base.Log.LogErrorFromException(e);
				return false;
			}

		}

		private void LogSqlScriptTracker(string ScriptName, bool IsSuccessful)
		{
			
			this.GetSqlHelper().ExecuteSql("INSERT INTO [SqlScriptTracker] (ScriptName, IsSuccessful, Details) VALUES (@ScriptName, @IsSuccessful, @Details)",
				"@ScriptName", ScriptName,
				"@IsSuccessful", IsSuccessful,
				"@Details", "RepairEngine.Deployment.SqlChangeScripts Automatic Execution"
				);
		}

		#endregion

		
	}


}
