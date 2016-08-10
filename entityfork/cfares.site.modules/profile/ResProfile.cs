using cfares.site.modules.user;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Profile;
using System.Web.Security;

namespace cfares.site.modules.profile
{
    public class ResProfile: ProfileBase
    {
        static public ResProfile CurrentUser{
            get{
                return (ResProfile)
                       (ProfileBase.Create(Membership.GetUser().UserName));
            }
        }



        public UserMembershipRepository UserInfo
        {
            get { return ((UserMembershipRepository)(base["UserInfo"])); }
            set { base["UserInfo"] = value; Save(); }
        }
    }
}
