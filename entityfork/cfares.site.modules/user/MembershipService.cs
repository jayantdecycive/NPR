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

namespace cfacore.shared.modules.user
{
    public class UserMembershipService : UserService
    {

        private HttpContextBase context;

        private IPrincipal User { get { return context.User; } }



        public UserMembershipService(HttpContextBase context)
        {
            this.context = context;
        }

        public UserMembershipService()
        {
            // TODO: Complete member initialization
        }

        protected void awesomizeSession()
        {

        }
        
        public bool ValidatePassword(string username, string password)
        {
            return Membership.ValidateUser(username,password);
        }

        public void Authorize(string UserName, bool RememberMe)
        {
            //Can initialize session here
            ResUser user = GetUser(UserName);
            context.Session["FullName"] = user.Name.Full;
            context.Session["UserId"] = user.UserId;
            FormsAuthentication.SetAuthCookie(UserName, RememberMe);
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

            string authority = AuthorityFromProviderName(memWrapped.Membership.ProviderName);
            string authorityUid = memWrapped.Membership.ProviderUserKey.ToString();

            ResUser newUser = new ResUser();
            newUser.Authority = authority;
            newUser.AuthorityUID = authorityUid;
            newUser.Creation = DateTime.Now;
            newUser.Email = UserNameEmail;
            newUser.Username = UserNameEmail;
            //newUser.Name = new Name("");

            this.Save(newUser);

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
            
            string authority = AuthorityFromProviderName(memWrapped.Membership.ProviderName);
            string authorityUid = memWrapped.Membership.ProviderUserKey.ToString();

            memWrapped.ApplicationUser = this.LoadByDatasource(authority,authorityUid);


            return memWrapped;
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

        public ResUser GetUser()
        {
            return GetUser(this.User.Identity.Name, false);
        }

        public ResUser GetUser(string UserName)
        {
            return GetUser(UserName, false);
        }

        public ResUser GetUser(string UserNameEmail, bool isOnline)
        {
            return this.LoadByUsername(UserNameEmail);
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

        public static string AuthorityFromProviderName(string providerName){
        
            switch(providerName){
            
                case "CfaApplicationMySqlMembershipProvider":
                default:
                    return "CFAApplicationMySql";
            }
        }

       
    }
}
