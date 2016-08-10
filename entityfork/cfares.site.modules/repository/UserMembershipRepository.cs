using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Omu.ValueInjecter;
using cfacore.domain.user;
using System.Web.Security;
using System.Web;
using cfacore.shared.domain.user;
using cfacore.shared.modules.helpers;
using cfares.domain.user;
using System.Security.Principal;
using cfares.entity.dbcontext.res_event;

using cfares.repository.user;
using cfares.repository._base;
using cfares.site.modules.com.application;
using cfares.site.modules.mail;

namespace cfares.site.modules.user
{
    public class UserMembershipRepository : UserRepository
    {

        private HttpContextBase context;
        private ResUser userCache=null;

        private IPrincipal _User=null;
        private IPrincipal User { 
            get {
                if (_User == null && context!=null && context.Request.IsAuthenticated)
                {   
                    return context.User;
                }else if (_User == null)
                {
                    return null;
                }
                return _User;
            }
            set {
                _User = value;
            }
        }


        public UserMembershipRepository(IPrincipal User, IResContext dbcontext)
            : base(dbcontext)
        {
            if(User.Identity.IsAuthenticated)
            this.User = User;         
        }

        public UserMembershipRepository(IResContext dbcontext) 
			: this( new HttpContextWrapper( HttpContext.Current ), dbcontext ) {}

        public UserMembershipRepository(HttpContextBase context, IResContext dbcontext):base(dbcontext)
        {
            this.context = context;
            if (context.Request.IsAuthenticated)
                CheckSession();         
        }

        private ResUser _current = null;

        public ResUser Current {
            get {
                if (_current==null)
                    _current=GetUser();
                return _current;
            }
        }

        public bool IsAdmin {
            get {
                return IsAccountInRole("Admin");
            }
        }

        public override void OnSave(object sender, SavedEventArgs<ResUser> args)
        {
            ResUser target = args.Instance;
            if (target == null)
                return;
            var roleNames = Enum.GetNames(typeof (UserOperationRole));
            var roles = new Dictionary<string, int>();
            for (int i = 0; i < roleNames.Length; i++)
            {
                roles.Add(roleNames[i],i);
            }

            var targetRole = (int) target.OperationRole;
            string[] currentUserRoles = Roles.GetRolesForUser();
          /*  foreach (var role in currentUserRoles)
            {
                if (roles[role] >= targetRole)
                    break;
                throw new System.Security.SecurityException("You cannot downgrade a user with more privileges than yourself.");
            }*/
            //var rolesReversed = roles.ToDictionary(x=>x.Value,x=>x.Key);
            foreach (var key in roles)
            {
                if (key.Value <= targetRole)
                {
                    if (key.Value > 0 && !Roles.IsUserInRole(target.Username, key.Key))
                    {
                        Roles.AddUserToRole(target.Username,key.Key);
                    }
                }
                else
                {
                    if (key.Value>0 && Roles.IsUserInRole(target.Username, key.Key))
                    {
                        Roles.RemoveUserFromRole(target.Username, key.Key);
                    }
                }
            }

        }

        public string GetClientId()
        {
            if (context == null)
                return null;
            if (context.Session["cid"]==null)
            {
                context.Session["cid"] = Guid.NewGuid().ToString();
            }
            return context.Session["cid"].ToString();
        }

        


        private void CheckSession(ResUser user) {
            CheckSession(true,user);
        }

        private void CheckSession(bool force){
            CheckSession(force, GetUser());
        }

        private void CheckSession()
        {
            if (context == null || context.Session == null || context.Session["UserId"] != null)
                return;
           
            SyncSession(GetUser());            
        }

        private void CheckSession(bool force,ResUser user){
            if (context == null || context.Session == null)
                return;
            if (user==null||(!force&&context.Session["UserId"]!=null))
                return;
            SyncSession(user);
        }


        /// <summary>
        /// Syncs the session. Edit this method to add convenience user information.
        /// </summary>
        /// <param name="resUser">The res user.</param>
        public void SyncSession( ResUser resUser ) 
		{
			if( resUser == null || context==null || context.Session == null ) return;
            userCache = resUser;
            context.Session["FullName"] = resUser.Name.Full;
            context.Session["Email"] = resUser.Email;            
            context.Session["UserId"] = resUser.UserId;
        }

        public string QuickName{
            get {
                CheckSession();
                return context.Session["FullName"] as string;
            }
        }

        public string QuickId
        {
            get
            {
                CheckSession();
                return context.Session["UserId"] as string;
            }
        }

        public string QuickEmail
        {
            get
            {
                CheckSession();
                return context.Session["Email"] as string;
            }
        }

        public ResUser QuickUser
        {
            get
            {
                if(userCache!=null)
                    return userCache;
                return null;
            }
        }

        public string ResetPassword(MembershipUser User, string Password) {
            string pass = User.ResetPassword();
            User.ChangePassword(pass,Password);
            return Password;
        }

        public string ResetPassword(string Password)
        {
            return ResetPassword(GetAccount(),Password);
        }

        public string ResetPassword()
        {
            MembershipUser User = GetAccount();
            return User.ResetPassword();
        }

        public string ResetPassword(ResUser user, string Password)
        {
            MembershipCreateStatus stat;
            string pass;
            MembershipUser User = GetOrCreateAccount(user,out stat,out pass);
            return ResetPassword(User,Password);
        }
        
        public bool ValidatePassword(string username, string password)
        {
            return Membership.ValidateUser(username,password);
        }

		public void ChangePassword( string oldPassword, string newPassword )
		{
            MembershipUser u = GetAccount();
			u.ChangePassword( oldPassword, newPassword );
		}

        public void ChangePasswordForUser(string user, string newPassword)
        {
            MembershipUser u = GetAccount(user);
            var pass = u.ResetPassword();
            if (!u.ChangePassword(pass, newPassword))
            {
                throw new Exception("Password Reset Failed");
            }
        }

        public void ChangePassword(string newPassword)
        {
            MembershipUser u = GetAccount();
            var pass = u.ResetPassword();
            u.ChangePassword(pass, newPassword);
        }


        public ResUser Authenticate(string UserName, string Password, bool RememberMe)
        {
            //Can initialize session here
            //WebSecurity.Login(UserName, Password, RememberMe);
            FormsAuthentication.SetAuthCookie(UserName, RememberMe);
            ResUser user = GetUser(UserName);
            SyncSession(user);
            return user;
        }

        public ResUser Authenticate(ResUser resUser)
        {
            return Authenticate(resUser,false);
        }
        public ResUser Authenticate(ResUser user, bool RememberMe)
        {
            FormsAuthentication.SetAuthCookie(user.Username, RememberMe);            
            SyncSession(user);
            return user;
        }
        
        public ResUser Authenticate(MembershipUser Membership, string Password)
        {
            return Authenticate(Membership.UserName, Password,false);
        }

        public void LogOff()
        {
            //Can destroy session here too
            FormsAuthentication.SignOut();
        }

        public UserAccount<ResUser> CreateUser(string UserNameEmail, string Password, out MembershipCreateStatus createStatus)
        {
            UserAccount<ResUser> memWrapped = new UserAccount<ResUser>();

            memWrapped.Membership = Membership.CreateUser(UserNameEmail, Password, UserNameEmail, null, null, true, null, out createStatus);

            if(createStatus!=MembershipCreateStatus.Success){
                return memWrapped;
            }

            ResUser newUser = LoadOrCreateByDatasource(memWrapped.Membership);
            

            
            memWrapped.ApplicationUser = newUser;
            return memWrapped;
        }

        public UserAccount<ResUser> CreateUser(ResUser newUser, string pass, out MembershipCreateStatus createStatus)
        {
			return CreateUser( newUser, pass, false, out createStatus );
        }

        public UserAccount<ResUser> CreateUser(IUser iUser, string pass, bool createUserInMembershipOnly,
                                               out MembershipCreateStatus createStatus,UserOperationRole role)
        {
            var resUser = new ResUser();
            resUser.OperationRole = role;
            IValueInjecter injecter = new ValueInjecter();
            injecter.Inject(resUser, iUser);           
            
            return CreateUser(resUser, pass, createUserInMembershipOnly, out createStatus);
        }

        public UserAccount<ResUser> CreateUser(ResUser newUser, string pass, bool createUserInMembershipOnly, out MembershipCreateStatus createStatus)
        {
			if( newUser.BirthDay.Year == 1 ) newUser.BirthDay = new DateTime( 2000, 1, 1 );

            UserAccount<ResUser> memWrapped = new UserAccount<ResUser>();            
            memWrapped.Membership = Membership.CreateUser(newUser.Username, pass, newUser.Email, null, null, true, null, out createStatus);

            if (createStatus != MembershipCreateStatus.Success)
                return memWrapped;

            newUser.Authority = memWrapped.Membership.ProviderName;
            newUser.AuthorityUID = memWrapped.Membership.ProviderUserKey.ToString();
            if (! createUserInMembershipOnly)
            {
                Add(newUser);
            }
            else
            {
                Edit(newUser);
            }
            memWrapped.ApplicationUser = newUser;
            Commit();

            return memWrapped;
        }

        public UserAccount<ResUser> CreateUser(ResUser newUser, out MembershipCreateStatus createStatus, out string pass, bool membershipOnly=false)
        {
            UserAccount<ResUser> memWrapped = new UserAccount<ResUser>();
            //pass = Membership.GeneratePassword(7,0);
            pass = Regex.Replace(Guid.NewGuid().ToString(), @"[^A-Za-z0-9]", "").Substring(0, 7);
            memWrapped.Membership = Membership.CreateUser(newUser.Username, pass, newUser.Email, null, null, true, null, out createStatus);

            if (createStatus != MembershipCreateStatus.Success)
            {

                if (createStatus == MembershipCreateStatus.DuplicateEmail){
                    memWrapped.Membership = Membership.GetUser(newUser.Username);
                }else if(createStatus == MembershipCreateStatus.DuplicateUserName) {
                    memWrapped.Membership = Membership.GetUser(Membership.GetUserNameByEmail(newUser.Email));
                }
                if (memWrapped.Membership == null)
                    return memWrapped;
                else
                {
                    createStatus = MembershipCreateStatus.Success;
                    pass = string.Empty;
                }
            }

	        newUser.Authority = memWrapped.Membership.ProviderName;
            newUser.AuthorityUID = memWrapped.Membership.ProviderUserKey.ToString();
            
            //Some user objects may have bound addresses!
            //newUser.Address = new Address();            
            if (!membershipOnly)
                this.Add(newUser);
            this.Commit();
            memWrapped.ApplicationUser = newUser;
            return memWrapped;
        }


        public UserAccount<ResUser> GetUserAndAccount()
        {
            return GetUserAndAccount(this.User.Identity.Name, false);
        }

        public UserAccount<ResUser> GetUserAndAccount(string UserName)
        {
            return GetUserAndAccount(UserName,false);
        }

        public UserAccount<ResUser> GetUserAndAccount(string UserName, bool isOnline)
        {
            UserAccount<ResUser> memWrapped = new UserAccount<ResUser>();
            memWrapped.Membership = Membership.GetUser(UserName, isOnline);
            
            
            memWrapped.ApplicationUser = LoadOrCreateByDatasource(memWrapped.Membership);


            return memWrapped;
        }

        public void EnsureUserStoreRelationship(ResUser user, string[] locationIds)
        {
			// SH - Not sure yet how or why this could be null, but testing is indicating that null values are coming in
			// .. Add guard for now due to go-live need and follow up - MD seemed to guard against it below previously / incompletely
			if( locationIds == null ) return;

            ResUser searchResUser = LoadByUID( user.UID );
			if( searchResUser == null ) return;

		    // Get old subscriptions and downgrade them
		    List<LocationSubscription> oldSubscriptions = searchResUser.UserPreferredLocationSubscriptions
			    .Where( o => o.StoreId.IsValid() )
			    .Where( x => ! locationIds.Contains( x.StoreId ) ).ToList();
		    foreach (var locationSubscription in oldSubscriptions)
			    locationSubscription.Role = UserOperationRole.None;   

		    // Get current subscriptions and upgrade them
		    var currentSubscriptions = searchResUser.UserPreferredLocationSubscriptions
			    .Where( o => o.StoreId.IsValid() )
			    .Where( x => locationIds.Contains( x.StoreId ) ).ToList();
		    foreach (var locationSubscription in currentSubscriptions)
			    locationSubscription.Role = user.OperationRole;

		    // Remove current subscriptions from list
		    locationIds = locationIds.Where( 
			    x => ! currentSubscriptions.Where( o => o.StoreId.IsValid() ).Select( s => s.StoreId ).Contains( x ) 
			            && ! oldSubscriptions.Where( o => o.StoreId.IsValid() ).Select( s => s.StoreId ).Contains( x )
			    ).ToArray();

	        // Add new stores with operation role
            foreach (var locationId in locationIds)
	            searchResUser.UserPreferredLocationSubscriptions.Add(
		            new LocationSubscription
			            {
				            StoreId = locationId,
				            ReceiveEmails = false,
				            Role=user.OperationRole
			            });
        }

		public bool IsUserInSystem( string username )
		{
			MembershipUser searchMembershipUser = Membership.GetUser( username );
            ResUser searchResUser = GetUser( username );

			return searchMembershipUser != null || searchResUser != null;
		}

        public ResUser EnsureUserExists( ResUser user )
        {
			// Check to make sure user exists in the membership provider .. if not in either create
			MembershipUser searchMembershipUser = Membership.GetUser( user.Username );

			// Check to make sure user exists in the core DB .. if not in either create
			//ResUser searchResUser = GetUser( user.Username, false, true );
            //mdrake: need to search by authority uid
            ResUser searchResUser = LoadOrCreateByDatasource(searchMembershipUser);

            if (searchResUser == null && !string.IsNullOrEmpty(user.UID))
            {
                searchResUser = LoadByUID(user.UID);
                if (searchResUser!=null)
					searchMembershipUser = Membership.GetUser(searchResUser.Username);
            }

            // If user exists in both systems, we're nominal
			if( searchMembershipUser != null && searchResUser != null )
			{
				EnsureUserInRole( user, searchResUser ); // Always check role access
				return searchResUser;
			}

			// .. Condition where user doesn't exist in DB but does in membership is not supposed to exist
			if( searchMembershipUser != null && searchResUser == null ) 
				throw new ApplicationException( string.Format( 
					"Membership user located without corresponding database user [ Username = {0} ]", user.Username ) );

            // Condition where user doesn't exist in membership but does in DB .. results in us creating the user in the DB
			// .. for ex. if we import a large set of users, swap out the provider, etc.
			// .. continue processing below ( special create user flag skipping DB save )
	        bool createUserInMembershipOnly = searchMembershipUser == null && searchResUser != null;

            if (createUserInMembershipOnly)
            {
                if(!string.IsNullOrEmpty(user.Username))
                    searchResUser.Username = user.Username;
                if (!string.IsNullOrEmpty(user.Email))
                    searchResUser.Email = user.Email;
                if (!string.IsNullOrEmpty(user.NameString))
                    searchResUser.NameString = user.NameString;
            }

			// Nether membership user or DB user exist at this point .. create in both systems
	        MembershipCreateStatus createStatus = default( MembershipCreateStatus );
			string privateInternalGeneratedPassword = Guid.NewGuid().ToString();
            if(createUserInMembershipOnly)
                CreateUser(searchResUser??user, privateInternalGeneratedPassword, createUserInMembershipOnly, out createStatus);
            else
                CreateUser(searchResUser ?? user, privateInternalGeneratedPassword, createUserInMembershipOnly, out createStatus);

            if (createStatus != MembershipCreateStatus.Success)
				throw new ApplicationException( string.Format( 
					"Unable to create new membership user [ Username = {0}, CreateStatus = {1}, Info ={2} ]", 
					(searchResUser??user).Username,createStatus.ToString(),(searchResUser??user).ToChecksum() ) );

			searchMembershipUser = Membership.GetUser( user.Username );
            searchResUser = LoadOrCreateByDatasource(searchMembershipUser);

            if (searchResUser != null)
                EnsureUserInRole(user, searchResUser);

            return searchResUser??user;
        }

        private ResUser LoadByUID(string uid)
        {
            return Find(x=>x.UID==uid);
        }

        public ResUser LoadByWanye()
        {
            return Find(3140);
        }

        public void EnsureUserInRole( ResUser user, ResUser searchResUser )
        {
			if( ! IsUserInRole( user.Username, user.OperationRole.ToString() ) )
				AddUserToRole( user.Username, user.OperationRole.ToString() );

			// Due to non-normalization, roles are currently stored in multiple locations
			// .. Do our best to keep it in sync
            if (!string.IsNullOrEmpty(user.Username))
                searchResUser.Username = user.Username;
            if (!string.IsNullOrEmpty(user.Email))
                searchResUser.Email = user.Email;
            if (!string.IsNullOrEmpty(user.NameString))
                searchResUser.NameString = user.NameString;

			if( searchResUser.OperationRole != user.OperationRole && user.OperationRole != UserOperationRole.Guide )
				searchResUser.OperationRole = user.OperationRole;

			if( searchResUser.OperationRole == UserOperationRole.Guide )
				searchResUser.OperationRole = UserOperationRole.Operator; // Per Gabby / Billy

            Edit(searchResUser);
            Commit();
        }

		public ResUser LoadOrCreateByDatasource(MembershipUser memUser)
        {
            if (memUser == null) return null;
        
            ResUser user = LoadByDatasource(memUser.ProviderName, memUser.ProviderUserKey.ToString());
            if( user != null) return user;

		    user = FindBySlug(memUser.UserName) ?? FindByEmail( memUser.Email );

			//this really shouldn't happen: mdrake
		    if (user != null)
		    {
		        user.Authority = memUser.ProviderName;
		        user.AuthorityUID = memUser.ProviderUserKey.ToString();
                Commit(true);
		        return user;
		    }

		    string[] name = Regex.Replace(memUser.Email, @"@.*$","").Split('.');
		    string firstName = name[0];
		    string lastName = name.Length > 1 ? name[1] : "Account";
            user = new ResUser
	        {
		        Email = memUser.Email,
		        Authority = memUser.ProviderName,
		        AuthorityUID = memUser.ProviderUserKey.ToString(),
		        //CreatedDate = DateTime.Now, mdrake: made this automatic
		        Username = memUser.UserName,
				OperationRole = 0,
				AccountStatus = cfacore.domain.user.UserAccountStatus.Normal,
                FirstName = firstName,
                LastName=lastName,
                BirthDay = DateTime.Now
	        };
        
            Add(user);
            Commit( true );
            return user;
        }

        public MembershipUser GetAccount()
        {
            return GetAccount(this.User.Identity.Name, false);
        }

        public MembershipUser GetAccount(string UserName)
        {
            return GetAccount(UserName, false);
        }

        public MembershipUser GetAccount(string UserName, bool isOnline)
        {            
            return Membership.GetUser(UserName, isOnline);
        }

        public MembershipUser GetAccount(string Authority, string AuthorityUID, bool isOnline)
        {
            return Membership.Providers[Authority].GetUser(AuthorityUID, isOnline);
        }

        public ResUser GetUser()
        {
            if (this.User!=null)            
                return GetUser(this.User.Identity.Name);
            return userCache;
        }

        public ResUser GetUser(string UserName)
        {
            return GetUser(UserName, false);
        }

        public ResUser GetUser(string authority, string authorityUID)
        {
            return Find(x=>x.Authority==authority&&x.AuthorityUID==authorityUID);
        }

        public ResUser GetUser(string userNameEmail, bool isOnline)
        {
			return GetUser( userNameEmail, isOnline, false );
        }
            
        public ResUser GetUser(string userNameEmail, bool isOnline, bool bypassCreateByDatasource )
        {
	        ResUser user = FindBySlug(userNameEmail);
            if( user == null && ! bypassCreateByDatasource )
                {
                MembershipUser memUser = GetAccount(userNameEmail);
                    user = LoadOrCreateByDatasource(memUser);
                }
            
            return user;
        }

        public ResUser GetUserWithAddress()
        {
            if (this.User!=null)
                return GetUserWithAddress(this.User.Identity.Name);
            return userCache;
        }

        public ResUser GetUserWithAddress(string UserName)
        {
            return GetUserWithAddress(UserName, false);
        }

        public ResUser GetUserWithAddress(string UserNameEmail, bool isOnline)
        {
            ResUser user = null;

            user = Find(x=>x.Username==UserNameEmail,"Address");
            if (user == null)
            {
                MembershipUser memUser = GetAccount(UserNameEmail);
                user = LoadOrCreateByDatasource(memUser);
            }

            return user;
        }

        

        public void AddUserToRole(string role) {
            AddUserToRole(this.User.Identity.ToString(),role);
        }

        public void AddUserToRole(string username, string role)
        {
            Roles.AddUserToRole(username,role);
        }

        public void RemoveUserFromRole(string role)
        {
            RemoveUserFromRole(this.User.Identity.ToString(), role);
        }

        public void RemoveUserFromRole(string username, string role)
        {
            Roles.RemoveUserFromRole(username, role);            
        }
                

        
        public bool IsUserInRole(string username, string role)
        {
            return Roles.IsUserInRole(username, role);
        }

        public void AddUsersToRole(UserCollection users, string roleName)
        {
            Roles.AddUsersToRole(users.Select(user => user.Username).ToArray(),roleName);
        }

        public void AddUsersToRoles(UserCollection users, string[] roleName)
        {
            Roles.AddUsersToRoles(users.Select(user => user.Username).ToArray(), roleName);
        }

        public string[] GetAccountsInRole(string roleName)
        {
            return Roles.GetUsersInRole(roleName);
        }

        public IQueryable<ResUser> LoadByUsernames(string[] usernames) {
            return Get(x => usernames.Contains(x.Username));
        }

        public ResUserCollection GetUsersInRole(string roleName)
        {
            return new ResUserCollection(this.LoadByUsernames(Roles.GetUsersInRole(roleName)).ToList());
        }


        public void AddAccountToRole(string role)
        {
            AddAccountToRole(this.User.Identity.ToString(), role);
        }

        public void AddAccountToRole(string username, string role)
        {
            Roles.AddUserToRole(username, role);
        }

        public void RemoveAccountFromRole(string role)
        {
            RemoveAccountFromRole(this.User.Identity.ToString(), role);
        }

        public void RemoveAccountFromRole(string username, string role)
        {
            Roles.RemoveUserFromRole(username, role);
        }

        public bool IsAccountInRole(string role)
        {
            return Roles.IsUserInRole(role);
        }

        public bool IsAccountInRole(string username, string role)
        {
            return Roles.IsUserInRole(username, role);
        }

        public bool DeleteUserAndAccount(ResUser user) {
            Delete(user);
            Commit();
            return Membership.DeleteUser(user.Username);

        }

        public bool DeleteUserAndAccount(string Username)
        {
			try
			{
				bool deletedAccount = DeleteAccount(Username);
				var user = Find(x => x.Username == Username);
				if (user != null)
				{
					Delete(user);
					Commit();
				}
				return deletedAccount;

			}
			catch (Exception ex)
			{
				throw new ApplicationException( string.Format( "Error deleting user account [ {0} ]", Username ), ex );
			}
		}

        public bool Save(MembershipUser obj) {
            try
            {
                Membership.UpdateUser(obj);
                return true;
            }catch(Exception ex){
                return false;
            }
        }
        
        #region StoreStatus Codes
        public static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion


        public bool IsAuthenticated {
            get
            {
				if( User == null ) return false;
	            return User.Identity.IsAuthenticated;
            }
        }

        public bool DeleteAccount(string p)
        {
            return Membership.DeleteUser(p);
        }






        public MembershipUser GetOrCreateAccount(ResUser user, out MembershipCreateStatus stat, out string password)
        {
            var mem = Membership.GetUser(user.Username);
            if (mem == null)
            {
                mem = CreateUser(user, out stat, out password,true).Membership;
            }
            else
            {
                stat = MembershipCreateStatus.Success;
                password = null;
            }
            return mem;
        }

        public ResUser CreateUser(MembershipUser gabbysAccount)
        {
            return LoadOrCreateByDatasource(gabbysAccount);
        }

	    public void EnsureEmailDomainAppliedToUsername( ResUser resUser )
	    {
			if( resUser.Username.IndexOf( "@" + AppContext.Current.Configuration.SSO.UsernameEmailDomain, StringComparison.OrdinalIgnoreCase ) < 0 )
				resUser.Username += "@" + AppContext.Current.Configuration.SSO.UsernameEmailDomain;
	    }
    }

    public static class ResUserExtensions
    {
        public static bool SendMessage(this ResUser user, string subject, string body)
        {
            return new ResEmailService().Message(user.Email, subject, body);
        }
    }
}
