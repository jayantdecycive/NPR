using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RazorEngine;
using cfacore.shared.modules.com.admin;
using cfares.entity.dbcontext.res_event;
using cfares.repository.ticket;
using cfares.site.modules.com.application;
using cfares.site.modules.mail;
using cfares.site.modules.user;
using cfaresv2.Areas.MyAccount.Controllers._base;

namespace cfaresv2.Areas.MyAccount.Controllers
{
    [AreaAuthorize(Area = "MyAccount", Roles = "Admin,Operator,Customer", Controller = "LogOn", Action = "Index")]
    public class EmailController : MyAccountController
    {
        //
        // GET: /MyAccount/Email/

        public ActionResult Ticket(int? id)
        {
            IResContext serv = ReservationConfig.GetContext();
            var trepo = new TicketRepository(serv);
            var memberRepo = new UserMembershipRepository(HttpContext,serv);
            var t = trepo.Find(x=>x.TicketId==id.Value, new[] { "Slot.Occurrence.ResEvent.ReservationType", "Slot.Occurrence.ResEvent.Template" });
            var viewLocation = "NewReservationConfirmation";
            var context = new ResEmailContextModel()
                {
                    Ticket = t,
                    Context = AppContext.Current,
                    User = memberRepo.GetAccount(),
                    IsSite=true,
                    ThemeImagesUrl = System.Web.HttpContext.Current.Request.RequestContext.ThemeImageUrlBase(t.Slot.Occurrence.ResEvent.TemplateId)
                };
            string filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Views/Shared/Email/" + viewLocation + ".cshtml");
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings["mvc-template-root"]) + viewLocation + ".cshtml";
                if (string.IsNullOrEmpty(filePath))
                    throw new Exception("MVC Templates could not be found.");
            }
            var cshtmlText = System.IO.File.ReadAllText(filePath);
            var content =  Razor.Parse( cshtmlText, context );
            
            return Content(content);
        }

    }
}
