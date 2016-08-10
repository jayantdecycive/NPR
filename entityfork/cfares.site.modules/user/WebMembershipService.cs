using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.site.controllers.shared;
using System.Web.Security;
using System.Web;
using cfacore.shared.domain.user;
using cfares.domain.user;
using System.Security.Principal;
using cfacore.shared.domain._base;

namespace cfacore.shared.modules.user
{
    public class UserMembershipService : UserService
    {

        private HttpContextBase context;
        private ResUser userCache=null;

        private IPrincipal User { get { return context.User; } }



        public UserMembershipService(HttpContextBase context)
        {
            this.context = context;
            if (context.Request.IsAuthenticated)
                CheckSession();
            this.Saved+=new site.controllers._base.SaveEventHandler(UserMembershipService_Saved);
        }

        private void UserMembershipService_Saved(object sender, DomainServiceEventArgs e)
        {
            ResUser target = e.target as ResUser;
            if (target == null)
                return;
            switch(target.OperationRole)
            {
                case UserOperationRole.Guide:
                    if (Roles.IsUserInRole(target.Username, "Admin"))
                    {                        
                        if (!Roles.IsUserInRole("Admin"))
                            throw new System.Security.SecurityException("You cannot downgrade a user with more privileges than yourself.");
                        Roles.RemoveUserFromRole(target.Username, "Admin");
                    }
                    if (!Roles.IsUserInRole(target.Username, "cfa") && (Roles.IsUserInRole("Admin")|| Roles.IsUserInRole("cfa")))
                        Roles.AddUserToRole(target.Username, "cfa");
                    
                    break;
                case UserOperationRole.Admin:
                    if (!Roles.IsUserInRole(target.Username, "Admin") && Roles.IsUserInRole("Admin"))
                        Roles.AddUserToRole(target.Username, "Admin");                    
                    break;
                case UserOperationRole.None:
                default:
                    if (Roles.IsUserInRole(target.Username, "cfa"))
                        Roles.AddUserToRole(target.Username, "cfa");
                    if (Roles.IsUserInRole(target.Username, "Admin"))
                        Roles.RemoveUserFromRole(target.Username, "Admin");
                    break;
            }
            
        }

        public UserMembershipService()
        {
            // TODO: Complete member initialization
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
            if (context.Session["UserId"] != null)
                return;
            SyncSession(GetUser());            
        }

        private void CheckSession(bool force,ResUser user){
            if (user==null||(!force&&context.Session["UserId"]!=null))
                return;
            SyncSession(user);
        }


        /// <summary>
        /// Syncs the session. Edit this method to add convenience user information.
        /// </summary>
        /// <param name="resUser">The res user.</param>
        public void SyncSession(ResUser resUser) {
            if (resUser == null)
                return;
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
            MembershipUser User = GetAccount(user.Username);
            return ResetPassword(User,Password);
        }
        
        public bool ValidatePassword(string username, string password)
        {
            return Membership.ValidateUser(username,password);
        }

        public ResUser Authenticate(string UserName, bool RememberMe)
        {
            //Can initialize session here
            
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
        public ResUser Authenticate(MembershipUser Membership) 
        {
            return Authenticate(Membership.UserName,false);
        }
        public ResUser Authenticate(MembershipUser Membership, bool remember)
        {
            return Authenticate(Membership.UserName, remember);
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

            string authorityUid = memWrapped.Membership.ProviderUserKey.ToString();

            ResUser newUser = new ResUser();
            newUser.Authority = memWrapped.Membership.ProviderName;
            newUser.AuthorityUID = authorityUid;
            //newUser.Creation = DateTime.Now; mdrake: made this automatic
            newUser.Email = UserNameEmail;
            newUser.Username = UserNameEmail;
            newUser.Address = new Address();
            //newUser.Name = new Name("");

            this.Save(newUser);
            memWrapped.ApplicationUser = newUser;
            return memWrapped;
        }

        public UserAccount<ResUser> CreateUser(ResUser newUser, string pass, out MembershipCreateStatus createStatus)
        {
            UserAccount<ResUser> memWrapped = new UserAccount<ResUser>();            
            memWrapped.Membership = Membership.CreateUser(newUser.Username, pass, newUser.Email, null, null, true, null, out createStatus);

            if (createStatus != MembershipCreateStatus.Success)
            {
                return memWrapped;
            }

	        newUser.Authority = memWrapped.Membership.ProviderName;
            newUser.AuthorityUID = memWrapped.Membership.ProviderUserKey.ToString();

            //Some user objects may have bound addresses!
            //newUser.Address = new Address();            

            this.Save(newUser);
            memWrapped.ApplicationUser = newUser;
            return memWrapped;
        }

        public UserAccount<ResUser> CreateUser(ResUser newUser, out MembershipCreateStatus createStatus, out string pass)
        {
            UserAccount<ResUser> memWrapped = new UserAccount<ResUser>();
            pass = Membership.GeneratePassword(7,0);
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
            
            this.Save(newUser);
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


        public ResUser LoadOrCreateByDatasource(MembershipUser memUser)
        {
            if (memUser == null)
                return null;
            ResUser user = SqlDao.LoadByDatasource(memUser.ProviderName, memUser.ProviderUserKey.ToString());
            if (user!=null&&user.IsBound())
                return user;

            
            user = new ResUser();            
            user.Email = memUser.Email;
            user.Authority = memUser.ProviderName;
            user.AuthorityUID = memUser.ProviderUserKey.ToString();
            //user.Creation = DateTime.Now; automatic
            user.Username = memUser.UserName;
            user.AccountStatus=(cfacore.domain.user.UserAccountStatus.Normal);

            SqlDao.Save(user);
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
            if (context.Request.IsAuthenticated)            
                return GetUser(this.User.Identity.Name);
            return userCache;
        }

        public ResUser GetUser(string UserName)
        {
            return GetUser(UserName, false);
        }

        public ResUser GetUser(string UserNameEmail, bool isOnline)
        {
            ResUser user = null;
            
                user = LoadByUsername(UserNameEmail);
                if (user == null)
                {
                    MembershipUser memUser = GetAccount(UserNameEmail);
                    user = LoadOrCreateByDatasource(memUser);
                }
            
            return user;
        }

        public ResUser GetUserWithAddress()
        {
            if (context.Request.IsAuthenticated)
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

            user = LoadByUsernameWithAddress(UserNameEmail);
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

        public ResUserCollection GetUsersInRole(string roleName)
        {
            return this.LoadByUsernames(Roles.GetUsersInRole(roleName));
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
            return Delete(user) && Membership.DeleteUser(user.Username);

        }

        public bool DeleteUserAndAccount(string Username)
        {
            return Delete(Username) && Membership.DeleteUser(Username);
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











        
    }

    
}
