using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Security.Permissions;
using System.Web;
using cfares.site.modules.mail;
using cfacore.site.controllers.shared;
using System.Web.Security;

namespace cfares.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "User" in code, svc and config file together.
    public class User : IUser
    {
        public void DoWork()
        {
        }

        //[PrincipalPermission(SecurityAction.Demand, Role = "Admin")]
        public void EmailResetPassword(string UserId) {
            
            //ResEmailService serv = new ResEmailService(null);
            //UserService userv = new UserService();
            //serv.ResetPasswordEmail(userv.Load(UserId));
        }
    }
}
