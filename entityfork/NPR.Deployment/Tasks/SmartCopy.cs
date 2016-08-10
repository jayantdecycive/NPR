using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.IO;
using System.Threading;

namespace PerformanceInsights.Deployment.Tasks
{
	public class SmartCopy : Microsoft.Build.Utilities.Task
	{
		#region ***** DELEGATES *****
		internal delegate bool CopyFileWithState(FileState source, FileState destination);
		#endregion

		#region ***** CONSTANTS / ENUMS *****
		private const int RETRY_DELAY_MILLISECONDS_DEFAULT = 0x3e8;
		private const string SMART_COPY_TIMESTAMP_FILE = "smartcopy.timestamp";
		#endregion

		#region ***** CONSTRUCTORS *****
		public SmartCopy()
		{
			this.RetryDelayMilliseconds = 0x3e8;
		}
		#endregion

		#region ***** PROPERTIES *****
		public ITaskItem LocalRootFolder { get; set; }
		public ITaskItem DestinationRootFolder { get; set; }
		public bool OverwriteReadOnlyFiles { get; set; }
		public int Retries { get; set; }
		public int RetryDelayMilliseconds { get; set; }
		public bool SkipUnchangedFiles { get; set; }
		private DateTime _OperationStartDate = DateTime.Now;

		[Required]
		public ITaskItem[] SourceFiles { get; set; }

		[Output]
		public ITaskItem[] CopiedFiles { get; set; }

		[Output]
		public ITaskItem[] DestinationFiles { get; set; }

		private DateTime? _LastCopiedDate = null;
		public DateTime LastCopiedDate
		{
			get
			{
				if (this._LastCopiedDate == null)
				{
					//if copy timestamp file exists at destination, Get Date
					string timestampFilePath = Path.Combine(this.DestinationRootFolder.ItemSpec, SmartCopy.SMART_COPY_TIMESTAMP_FILE);
					if(File.Exists(timestampFilePath))
					{
						//Get Value
						string timeStamp = File.ReadAllText(timestampFilePath);
	
						//Try to convert
						try
						{
							this._LastCopiedDate = DateTime.Parse(timeStamp);
						}
						catch (Exception e)
						{
							base.Log.LogError("Error reading Last Copied Date from Timestamp File.  FileValue='{0}', Error={1}", timeStamp, e);
							this._LastCopiedDate = DateTime.MinValue;
						}
								

						//If no match found
						if (this._LastCopiedDate == null)
							this._LastCopiedDate = DateTime.MinValue;

					}

					//else, return DateTime.Min
					else
						this._LastCopiedDate = DateTime.MinValue;

					base.Log.LogMessage("Last Copied Date is '{0}'", this._LastCopiedDate);
				}

				return this._LastCopiedDate.Value;
			}
		}
		#endregion
		
		#region ***** EXECUTE METHOD *****
		public override bool Execute()
		{
			return this.Execute(new CopyFileWithState(this.CopyFileWithLogging));
		}

		internal bool Execute(CopyFileWithState copyFile)
		{
			if ((this.SourceFiles == null) || (this.SourceFiles.Length == 0))
			{
				this.DestinationFiles = new TaskItem[0];
				this.CopiedFiles = new TaskItem[0];
				return true;
			}
			if (!this.ValidateInputs() || !this.InitializeDestinationFiles())
			{
				return false;
			}
			bool flag = true;
			List<ITaskItem> list = new List<ITaskItem>();
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			for (int i = 0; i < this.SourceFiles.Length; i++)
			{
				string str;
				bool flag2 = false;
				if (dictionary.TryGetValue(this.DestinationFiles[i].ItemSpec, out str) && string.Equals(str, this.SourceFiles[i].ItemSpec, StringComparison.OrdinalIgnoreCase))
				{
					flag2 = true;
				}
				if (!flag2)
				{
					if (this.DoCopyIfNecessary(new FileState(this.SourceFiles[i].ItemSpec), new FileState(this.DestinationFiles[i].ItemSpec), copyFile))
					{
						dictionary[this.DestinationFiles[i].ItemSpec] = this.SourceFiles[i].ItemSpec;
						flag2 = true;
					}
					else
					{
						flag = false;
					}
				}
				if (flag2)
				{
					this.SourceFiles[i].CopyMetadataTo(this.DestinationFiles[i]);
					list.Add(this.DestinationFiles[i]);
				}
			}
			this.CopiedFiles = list.ToArray();

			//If Smart Copy, then write out timestamp file
			if (this.SkipUnchangedFiles) this.SaveSmartCopyTimestampFile();

			return flag;
		}
		#endregion
		
		#region ***** INTERNAL METHODS *****
		private bool CopyFileWithLogging(FileState sourceFileState, FileState destinationFileState)
		{
			base.Log.LogMessage(MessageImportance.Normal, "Copy {0} -> {1}", new object[] { sourceFileState.Name, destinationFileState.Name });

			bool exists = false;
			if (Directory.Exists(destinationFileState.Name))
			{
				base.Log.LogError("Copy.DestinationIsDirectory", new object[] { sourceFileState.Name, destinationFileState.Name });
				return false;
			}
			if (Directory.Exists(sourceFileState.Name))
			{
				base.Log.LogError("Copy.SourceIsDirectory", new object[] { sourceFileState.Name });
				return false;
			}
			string directoryName = Path.GetDirectoryName(destinationFileState.Name);
			if (((directoryName != null) && (directoryName.Length > 0)) && !Directory.Exists(directoryName))
			{
				base.Log.LogMessage(MessageImportance.Normal, "Copy.CreatesDirectory {0}", new object[] { directoryName });
				Directory.CreateDirectory(directoryName);
			}
			if (this.OverwriteReadOnlyFiles)
			{
				this.MakeFileWriteable(destinationFileState, true);
				exists = destinationFileState.Exists;
			}
			bool flag2 = false;
			
			if (!flag2)
			{
				File.Copy(sourceFileState.Name, destinationFileState.Name, true);
			}
			destinationFileState.Reset();
			this.MakeFileWriteable(destinationFileState, false);
			return true;
		}
		private bool ValidateInputs()
		{
			//base.Log.LogMessage("Validating Inputs");
			if (this.Retries < 0)
			{
				base.Log.LogError("Copy.InvalidRetryCount", new object[] { this.Retries });
				return false;
			}
			if (this.RetryDelayMilliseconds < 0)
			{
				base.Log.LogError("Copy.InvalidRetryDelay", new object[] { this.RetryDelayMilliseconds });
				return false;
			}
			if ((this.DestinationFiles == null) && (this.DestinationRootFolder == null))
			{
				base.Log.LogError("Copy.NeedsDestination", new object[] { "DestinationFiles", "DestinationRootFolder" });
				return false;
			}
			if ((this.DestinationFiles != null) && (this.DestinationRootFolder != null))
			{
				base.Log.LogError("Copy.ExactlyOneTypeOfDestination", new object[] { "DestinationFiles", "DestinationRootFolder" });
				return false;
			}
			if ((this.DestinationFiles != null) && (this.DestinationFiles.Length != this.SourceFiles.Length))
			{
				base.Log.LogError("General.TwoVectorsMustHaveSameLength", new object[] { this.DestinationFiles.Length, this.SourceFiles.Length, "DestinationFiles", "SourceFiles" });
				return false;
			}
			return true;
		}
		private bool PathsAreIdentical(string source, string destination)
		{
			string fullPath = Path.GetFullPath(source);
			string strB = Path.GetFullPath(destination);
			return (0 == string.Compare(fullPath, strB, StringComparison.OrdinalIgnoreCase));
		}
		
		private bool DoCopyIfNecessary(FileState sourceFileState, FileState destinationFileState, CopyFileWithState copyFile)
		{
			try
			{
				if (this.SkipUnchangedFiles && !IsFileNewerThanLastCopyOperation(sourceFileState, destinationFileState))
				{
					//base.Log.LogMessage("Copy.DidNotCopyBecauseOfFileMatch '{0}'", new object[] { sourceFileState.Name, destinationFileState.Name, "SkipUnchangedFiles", "true" });
					return true;
				}
				if (string.Compare(sourceFileState.Name, destinationFileState.Name, StringComparison.OrdinalIgnoreCase) != 0)
				{
					return this.DoCopyWithRetries(sourceFileState, destinationFileState, copyFile);
				}
			}
			catch (PathTooLongException exception)
			{
				base.Log.LogError("Copy.Error {0} -> {1} because {2}", new object[] { sourceFileState.Name, destinationFileState.Name, exception.Message });
				return false;
			}
			catch (IOException exception2)
			{
				if (this.PathsAreIdentical(sourceFileState.Name, destinationFileState.Name)) return true;
				
				base.Log.LogError("Copy.Error {0} -> {1} because {2}", new object[] { sourceFileState.Name, destinationFileState.Name, exception2.Message });
				return false;
			}
			catch (Exception exception3)
			{
				base.Log.LogError("Copy.Error {0} -> {1} because {2}", new object[] { sourceFileState.Name, destinationFileState.Name, exception3.Message });
				return false;
			}
			return true;
		}

		private bool DoCopyWithRetries(FileState sourceFileState, FileState destinationFileState, CopyFileWithState copyFile)
		{
			bool success = false;
			int tries = 0;

			while (!success && (tries == 0 || tries < this.Retries))
			{
				try
				{
					success = copyFile(sourceFileState, destinationFileState);
				}
				catch (Exception exception)
				{
					if (tries >= this.Retries)
					{
						if (this.Retries > 0)
						{
							base.Log.LogError("Copy.ExceededRetries", new object[] { sourceFileState.Name, destinationFileState.Name, this.Retries });
						}
						else
							base.Log.LogErrorFromException(exception);

						//return false
						return false;
						
					}
				}

				if (tries < this.Retries)
				{
					tries++;
					base.Log.LogWarning("Copy.Retrying {0} -> {1}", new object[] { sourceFileState.Name, destinationFileState.Name, tries, this.RetryDelayMilliseconds, string.Empty });
					Thread.Sleep(this.RetryDelayMilliseconds);
				}

			}

			//If successful
			if (success) return true;

			//return false
			if (this.Retries > 0 && tries >= this.Retries) base.Log.LogError("Copy.ExceededRetries: {0} -> {1}", new object[] { sourceFileState.Name, destinationFileState.Name, this.Retries });

			//return false
			return false;
		}

		

		private bool InitializeDestinationFiles()
		{
			//base.Log.LogMessage("Initialize Destination Files");

			if (this.DestinationFiles == null)
			{
				this.DestinationFiles = new ITaskItem[this.SourceFiles.Length];
				for (int i = 0; i < this.SourceFiles.Length; i++)
				{
					string str;
					try
					{
						//Get Local File Path
						string localFilePath = this.SourceFiles[i].ItemSpec;

						//Remove Local Root Folder
						string relativeFilePath = localFilePath.Replace(this.LocalRootFolder.ItemSpec, "");

						//Combine with Destination Folder
						str = Path.Combine(this.DestinationRootFolder.ItemSpec, relativeFilePath);

						//base.Log.LogMessage("LocalFilePath: {0}, LocalRootFolder: {1}, RelativeFilePath: {2}, DestinationFilePath: {3}", localFilePath, this.LocalRootFolder.ItemSpec, relativeFilePath, str);

					}
					catch (ArgumentException exception)
					{
						base.Log.LogError("Copy.Error {0} -> {1} because {2}", new object[] { this.SourceFiles[i].ItemSpec, this.DestinationRootFolder.ItemSpec, exception.Message });
						this.DestinationFiles = new ITaskItem[0];
						return false;
					}
					this.DestinationFiles[i] = new TaskItem(str);
					this.SourceFiles[i].CopyMetadataTo(this.DestinationFiles[i]);
				}
			}
			return true;
		}
		
		private bool IsFileNewerThanLastCopyOperation(FileState sourceFile, FileState destinationFile)
		{
			if (sourceFile.LastWriteTime > this.LastCopiedDate && !this.IsMatchingSizeAndTimeStamp(sourceFile, destinationFile)) 
				return true;

			else 
				return false;
		}

		private bool IsMatchingSizeAndTimeStamp(FileState sourceFile, FileState destinationFile)
		{
			base.Log.LogMessage("Checking file Size & Timestamp {0}", sourceFile.Name);
			if (!destinationFile.Exists)
			{
				return false;
			}
			if (sourceFile.Length != destinationFile.Length)
			{
				return false;
			}
			if (sourceFile.LastWriteTime != destinationFile.LastWriteTime)
			{
				//if not web.config file
				if (!sourceFile.Name.EndsWith(".config", StringComparison.CurrentCultureIgnoreCase)) return false;

				//if web.config
				else return (File.ReadAllText(sourceFile.Name) == File.ReadAllText(destinationFile.Name));
			}

			return true;
		}

		private void MakeFileWriteable(FileState file, bool logActivity)
		{
			if (file.Exists && file.IsReadOnly)
			{
				if (logActivity)
				{
					base.Log.LogMessage(MessageImportance.Low, "Copy.RemovingReadOnlyAttribute", new object[] { file.Name });
				}
				File.SetAttributes(file.Name, FileAttributes.Normal);
				file.Reset();
			}
		}

		private void SaveSmartCopyTimestampFile()
		{
			
			//Get File Path
			string timestampFilePath = Path.Combine(this.DestinationRootFolder.ItemSpec, SmartCopy.SMART_COPY_TIMESTAMP_FILE);

			//Add Timestamp
			string output = string.Format("{0}", this._OperationStartDate);
			
			//Write out file
			File.WriteAllText(timestampFilePath, output);

		}
		#endregion
		

		#region ***** INTERNAL CLASS: FileState *****
		internal class FileState
		{
			//Constructor
			internal FileState(string Filename)
			{
				this._Filename = Filename;
			}

			// Fields & Properties
			private FileInfo _FileInfo = null;
			private FileInfo FileInfo
			{
				get
				{
					if (this._FileInfo == null)
					{
						this._FileInfo = new FileInfo(this._Filename);
					}
					return this._FileInfo;
				}
			}

			public string _Filename = null;
			
			internal FileAttributes Attributes
			{
				get
				{
					return this.FileInfo.Attributes;
				}
			}

			internal bool Exists
			{
				get
				{
					return this.FileInfo.Exists;
				}
			}

			internal bool IsReadOnly
			{
				get
				{
					return ((this.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly);
				}
			}

			internal DateTime LastWriteTime
			{
				get
				{
					return this.FileInfo.LastWriteTime;
				}
			}

			internal long Length
			{
				get
				{
					return this.FileInfo.Length;
				}
			}

			internal string Name
			{
				get
				{
					return this._Filename;
				}
			}

			// Methods
			internal void Reset()
			{
				this._FileInfo = null;
			}

		}


		#endregion


	}


}
