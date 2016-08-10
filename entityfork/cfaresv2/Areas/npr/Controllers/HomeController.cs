#region include

using System.Linq;
using cfares.domain._event;
using cfares.repository._event;
using cfaresv2.Controllers;
using npr.domain._event.ticket;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Globalization;
using System;
using System.Configuration;
using cfares.domain._event.resevent;
using System.Data.SqlClient;
using System.Data;
using cfares.site.modules.repository.ticket;
using cfares.domain.user;


#endregion

namespace cfaresv2.Areas.npr.Controllers
{
    public class HomeController : ReservationControllerBase
    {
        ResEventRepository<ResEvent> eventRepo;
       

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            eventRepo = new ResEventRepository<ResEvent>(ResContext);

            if (getSubDomain(Request.Url).ToLower() == "events" || Request.Url.ToString().ToLower().Contains("events"))
                ViewBag.Subdomain = "events"; //getSubDomain(Request.Url).ToLower();
            // eventRepo.Context.SiteUrls.Contains
            string url = Convert.ToString(Request.Url);
           
        }

        public ActionResult Index(int? id)
        {
          
            if (ViewBag.Subdomain == "events")
            {
                return View("Events");
            }
            else
            {
                if (id != null)
                {
                    ViewBag.Event = id.Value;
                }
                return View("Tours", new NPRTicket());
            }
        }

        public ActionResult Tours(int? id)
        {


            if (ViewBag.Subdomain == "events")
            {
                return RedirectToAction("Index");
            }
            else
            {
                if (id != null)
                {
                    ViewBag.Event = id.Value;
                }
                return View(new NPRTicket());
            }
        }

        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            string size = collection["GroupSize"];
            string id = collection["slotId"];
            return RedirectToAction("Register", "Reservation", new { groupSize = size, slotId = id });
        }

        [HttpPost]
        public ActionResult Tours(FormCollection collection)
        {
            string size = collection["GroupSize"];
            string id = collection["slotId"];
            return RedirectToAction("Register", "Reservation", new { groupSize = size, slotId = id });
        }

        public ActionResult Events()
        {
            return View();
        }

        public ActionResult EventList(string filter, int? month)
        {
            List<IResEvent> events = eventRepo.GetAll().Where(x =>
                !string.IsNullOrEmpty(x.SubHeading)
                && x.Occurrences.Any(y => y.SlotsList.Count() > 0)
                && x.Status == ResEventStatus.Live
                ).ToList();
            return View(events);
           
        }

        public ActionResult EventDetails(int? id)
        {
            if (id == null) { return RedirectToAction("Index"); }
            IResEvent e = eventRepo.Find(id.Value);
            if (e == null) { return RedirectToAction("Index"); }
          
            if (e.IsEventEnded)
                return RedirectToAction("EventEnded");
          
            return View(e as ResEvent);
        }

        private ITicket GetTicket(int? id)
        {
            if (id == null)
                return new NPRTicket();
            var ticketRepo = new ResTicketRepository<NPRTicket>(ResContext);
            ITicket ticket = ticketRepo.Find(id.Value);
            if (ticket.OwnerId != null)
                ticket.Owner = UserMembershipRepo.Find(ticket.OwnerId.Value) as ResUser;
            return ticket;
        }

        [HttpPost]
        public ActionResult EventDetails(FormCollection collection)
        {
            string slot = collection["slotId"];
            return RedirectToAction("Register", "Reservation", new { groupSize = collection["groupSize"], slotId = slot });
        }

        public ActionResult Faq()
        {
            return View();
        }

        public ActionResult Plan()
        {
            return View();
        }

        public ActionResult Message()
        {
            return View();
        }

        private string getSubDomain(Uri url)
        {
            string host = Request.Url.Host;
            if (host.Split('.').Length > 1)
            {
                int index = host.IndexOf(".");
                return host.Substring(0, index);
            }

            if (host.IndexOf("localhost", StringComparison.CurrentCultureIgnoreCase) >= 0)
                return "tours";

            return null;
            
           // return "tours"; // This code is only for Staging, uncomment above code and comment this line before uploading to Production.

        }

        public ActionResult UrlCheck(string abc)
        {
            return Redirect(abc);
        }

        public ActionResult GetBySlug(string permalink)
        {
            string url = Request.Url.AbsoluteUri;

            var eventrecord = eventRepo.Context.SiteUrls.FirstOrDefault(s => s.Url == permalink);

            if (eventrecord == null)
            {
                return RedirectToAction("Index", "Home");
           }
            //return RedirectToAction("EventDetails", new { id = article.ResEventId });
            return Redirect("~/npr/Home/EventDetails/" + eventrecord.ResEventId);
        }

        public ActionResult Migrate()
        {
            int id;
            SqlConnection con = new SqlConnection();
            SqlCommand cmd = new SqlCommand();
            DataTable dt = new DataTable();
            con = new SqlConnection("Server=tcp:cw74x70b5k.database.windows.net,1433;Database=npr;User Id=res_admin@cw74x70b5k;Password=Beard7Beard7;Trusted_Connection=False;Encrypt=True;");
            //con = new SqlConnection("Server=sql.decycivefarm.com;Database=npr;User Id=res_admin;Password=Beard7Beard7;Trusted_Connection=False;Encrypt=True;TrustServerCertificate=True");
            cmd = new SqlCommand("select TicketId,GuestListString from Tickets", con);
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    id = Convert.ToInt32(row["TicketId"]);
                    {
                        string GuestList = Convert.ToString(row["GuestListString"]);
                        if (!string.IsNullOrEmpty(GuestList))
                        {
                            string[] arr = GuestList.Split(',');
                            foreach (var item in arr)
                            {
                                SqlCommand cmd1 = new SqlCommand();
                                //cmd1 = new SqlCommand("insert into TicketGuests(GuestName,TicketId)values('" + item + "'," + id + ")", con);
                                cmd1 = new SqlCommand("insert into TicketGuests(GuestName,TicketId) values(@item, @id)", con);
                                cmd1.Parameters.AddWithValue("@item", item);
                                cmd1.Parameters.AddWithValue("@id", id);
                                if (con.State == ConnectionState.Closed)
                                {
                                    con.Open();
                                }
                                cmd1.ExecuteNonQuery();
                                if (con.State == ConnectionState.Open)
                                {
                                    con.Close();
                                }
                            }
                        }
                    }
                }
            }
            return Content("Migration Success");
        }

    }
}