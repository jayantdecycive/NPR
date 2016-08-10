using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;
using System.Web.Security;
using System.Diagnostics;
using System.Threading;
using System.DirectoryServices;
using cfacore.site.modules.log;
using System.Collections;

namespace cfacore.site.modules.auth
{
    public class LDAPProvider : MembershipProvider
    {
        private string _Name;
        private string _connectionStringName;
        private System.Configuration.ConnectionStringSettings _connectionString;

        private string _connectionUsername;
        private string _connectionPassword;
        private string _applicationName;
        private AuthenticationTypes _authenticationType = AuthenticationTypes.Secure;
        private string _attributeMapUsername;
        private string _domain;
        private int _passwordlength;


        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            _Name = name;



            try
            {
                if (config["connectionStringName"] != null)
                    _connectionStringName = config["connectionStringName"];
                if (config["authenticationType"] != null)
                    _authenticationType = (AuthenticationTypes)Enum.Parse(typeof(AuthenticationTypes), config["authenticationType"]);
                if (config["applicationName"] != null)
                    _applicationName = config["applicationName"];
                if (config["connectionUsername"] != null)
                    _connectionUsername = config["connectionUsername"];
                if (config["connectionPassword"] != null)
                    _connectionPassword = config["connectionPassword"];
                if (config["attributeMapUsername"] != null)
                    _attributeMapUsername = config["attributeMapUsername"];
                if (config["domain"] != null)
                    _domain = config["domain"];

            }
            catch (Exception Ex)
            {
                throw new System.Configuration.ConfigurationErrorsException(
                    "There was an error reading the membership configuration settings", Ex);
            }

            if (!string.IsNullOrEmpty(_connectionStringName))
            {

                try
                {

                    _connectionString = ConfigurationManager.ConnectionStrings[_connectionStringName];

                }
                catch (Exception Ex)
                {

                    throw new System.Configuration.ConfigurationErrorsException(
                    "The connection string " + _connectionStringName + " could not be found", Ex);

                }

            }
            base.Initialize(name, config);
            //this.ApplicationName = _applicationName;




        }

        public override bool ValidateUser(string username, string password)
        {

            //Find the person in the directory to determine their distinct name

            try
            {

                DirectoryEntry root = new DirectoryEntry(_connectionString.ConnectionString, _connectionUsername, _connectionPassword, _authenticationType);

                DirectorySearcher searcher = new DirectorySearcher(root);
                searcher.SearchRoot = root;


                searcher.Filter = "(&(objectCategory=user)(cn=" + username + "))";


                SearchResult findResult = searcher.FindOne();

                string distinctName = "cn=" + username;

                // Inverse the ou order found in LDAP to build distinct name



                for (int i = findResult.Properties["ou"].Count - 1; i >= 0; i--)
                {

                    distinctName += ",ou=" + findResult.Properties["ou"][i];

                }

                distinctName += ",CN=Users,DC=" + _domain.Replace(".", ",DC=");

                // Find the person as Employee

                DirectoryEntry root2 = new DirectoryEntry(_connectionString.ConnectionString,

                distinctName, password, AuthenticationTypes.ServerBind);

                DirectorySearcher searcher2 = new DirectorySearcher(root2);

                searcher2.SearchScope = SearchScope.Subtree;

                searcher2.Filter = "cn=" + username;

                try
                {

                    SearchResult resultEmployee = searcher2.FindOne();

                    if (resultEmployee.Properties["cn"].Count == 1) { return true; } else { return false; }

                }

                catch (Exception ex)
                {

                    LDAPAuthLog eventLog = new LDAPAuthLog();

                    ThreadStart starter = delegate { eventLog.WriteEntry(this.ToString(), String.Format("ValidateUser : {0} : uid {1} Found; Credentials failed", ex.Source, username), System.Diagnostics.EventLogEntryType.Warning); };

                    new Thread(starter).Start();

                    return false;

                }

            }

            catch (Exception ex)
            {

                if (ex.Message == "Object reference not set to an instance of an object.")
                {

                    LDAPAuthLog eventLog = new LDAPAuthLog();

                    ThreadStart starter = delegate { eventLog.WriteEntry(this.ToString(), String.Format("ValidateUser : {0} : cn {1} NOT found", ex.Source, username), System.Diagnostics.EventLogEntryType.Warning); };

                    new Thread(starter).Start();

                }

                else
                {

                    LDAPAuthLog eventLog = new LDAPAuthLog();

                    ThreadStart starter = delegate { eventLog.WriteEntry(this.ToString(), String.Format("ValidateUser : {0} : {1}", ex.Source, ex.Message), System.Diagnostics.EventLogEntryType.Error); };

                    new Thread(starter).Start();

                }

                return false;

            }

        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {

            try
            {
                DirectoryEntry root = new DirectoryEntry(_connectionString.ConnectionString, _connectionUsername, _connectionPassword, _authenticationType);
                DirectoryEntries entries = root.Children;
                DirectoryEntry newEntry = entries.Add(username, "User");
                newEntry.Properties["sAMAccountName"].Add(username);
                newEntry.Properties["cn"].Add(username);

                newEntry.Properties["mail"].Add(email);

                newEntry.Invoke("SetPassword", password);
                newEntry.CommitChanges();

                DirectoryEntry grp = root.Children.Find("Guests", "group");
                if (grp != null) { grp.Invoke("Add", new object[] { newEntry.Path.ToString() }); }
                status = MembershipCreateStatus.Success;
                return GetUser(username, true);

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                status = MembershipCreateStatus.ProviderError;
                return null;
            }
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { throw new NotImplementedException(); }            
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }
    }
}
