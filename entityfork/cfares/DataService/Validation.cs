using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Services;
using System.ComponentModel.DataAnnotations;
using System.Collections;
using System.Web.Security;
using System.ServiceModel.Channels;
using System.ServiceModel;
using cfacore.shared.modules.user;
using cfares.domain.user;

namespace cfares.DataService
{
    public abstract class Validation
    {
        public static void FireDataServiceValidation(object param, UpdateOperations operation)
        {
            // Only validates on inserts and updates
            if (operation != UpdateOperations.Add &&
                operation != UpdateOperations.Change)
                return;
            // Validation
            var validationContext = new ValidationContext(param, null, null);
            var result = new List<ValidationResult>();
            Validator.TryValidateObject(param, validationContext, result, true);

            if (result.Any())
                throw new DataServiceException(
                    result
                    .Select(r => r.ErrorMessage)
                    .Aggregate((m1, m2) => String.Concat(m1, Environment.NewLine, m2)));
        }
        public static string[] AdminRoles = new string[] { "Admin", "cfa" };

        public static bool IsFormsAuthorized() {
            return IsFormsAuthorized(Validation.AdminRoles);
        }

        public static string GetFormsUserName(){
            FormsAuthenticationTicket ticket = GetFormsTicket();
            return ticket.Name;
            //    roles = ticket.UserData;
        }

        public static FormsAuthenticationTicket GetFormsTicket() {
            var messageProperty = (HttpRequestMessageProperty)
                    OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name];
            string cookie = messageProperty.Headers.Get("Set-Cookie");
            if (cookie == null) // Check for another Message Header - SL applications
            {
                cookie = messageProperty.Headers.Get("Cookie");
            }
            if (cookie == null)
                cookie = string.Empty;

            Hashtable cookieTable = new Hashtable();
            string[] cookieValuePairs = cookie.Split(';');
            foreach (string pair in cookieValuePairs)
            {
                string[] splitted = pair.Split(',', '=');
                string key = splitted[0].Trim();

                if (splitted.Length >= 2)
                    if (!cookieTable.ContainsKey(key))
                        cookieTable.Add(key, splitted[1]);
            }

            string encryptedTicket = string.Empty;

            // Set User Name from cookie
            if (cookieTable.ContainsKey(FormsAuthentication.FormsCookieName))
                encryptedTicket = cookieTable[FormsAuthentication.FormsCookieName].ToString();

            FormsAuthenticationTicket ticket = null;
            string userName = string.Empty;
            string roles = string.Empty;

            
            // Decrypt
            if (!string.IsNullOrEmpty(encryptedTicket))
            {
                ticket = FormsAuthentication.Decrypt(encryptedTicket);
                return ticket;
                
            }
            return null;
        }

        public static bool IsFormsAuthorized(string[] checkRoles) {
            FormsAuthenticationTicket ticket = GetFormsTicket();

            if (string.IsNullOrEmpty(ticket.Name))
                return false;
            foreach (string role in checkRoles)
            {
                if (Roles.IsUserInRole(ticket.Name, role))
                    return true;
            }
            return false;            
        }

        public static long ResUserId()
        {
            FormsAuthenticationTicket ticket = GetFormsTicket();
            UserMembershipService userService = new UserMembershipService();
            ResUser user = userService.LoadByUsername(ticket.Name);
            if (user == null || !user.IsBound())
                return 0;
            return long.Parse(user.Id());
        }
    }
}