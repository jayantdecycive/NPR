using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Ninject;
using cfares.domain.user;
using cfares.entity.dbcontext.res_event;

namespace cfaresv2.App_Start
{
    public class ModelConfig
    {
        public static void Register()
        {
            var roleNames = Enum.GetNames(typeof(UserOperationRole));
            foreach (var role in roleNames)
            {
                if(!Roles.RoleExists(role))
                    Roles.CreateRole(role);
            }

            IKernel kernel = new StandardKernel();
            kernel.Bind<IResContext>().To<CfaResContext>();
        }
    }
}