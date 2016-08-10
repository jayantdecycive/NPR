using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cfacore.site.controllers._event;
using cfares.domain._event;
using cfacore.shared.domain._base;
using cfares.domain._event.resevent;
using cfacore.shared.modules.com.admin;

namespace cfares.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,cfa")]
    public class EventController : Controller
    {
        ResEventService serv = new ResEventService();
        //
        // GET: /Admin/Event/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Admin/Event/Past/

        public ActionResult Past()
        {
            return View();
        }

        //
        // GET: /Admin/Event/Upcoming/

        public ActionResult Upcoming()
        {
            return View();
        }


        //
        // GET: /Admin/Event/Verify/

        public ActionResult Verify()
        {
            return View();
        }

        //
        // GET: /Admin/Event/Confirm/

        public ActionResult Confirm()
        {
            return View();
        }


        //
        // GET: /Admin/Event/Details/5

        public ActionResult Details(int id)
        {
            ResEvent resEvent = serv.Load(id.ToString());

            return View(resEvent);
        }

        //
        // GET: /Admin/Event/Summary/5

        public ActionResult Summary(int id)
        {
            
            ResEvent resEvent = serv.Load(id.ToString());
            Wizard wiz = initializeWizard(resEvent, 2, Session, this.ControllerContext);
            ViewBag.Wizard = wiz;

            return View(resEvent);
        }


        //
        // GET: /Admin/Event/Create
        [ActionName("Create-Dash")]
        public ActionResult CreateDash(string id)
        {


            Wizard wiz;
            ResEvent evnt = null;
            if (string.IsNullOrEmpty(Request.QueryString["event"]))
            {
                wiz = initializeWizard(null, 0, Session, this.ControllerContext,true);
            }
            else
            {
                string eid = Request.QueryString["event"];
                evnt = serv.Load(eid);
                wiz = initializeWizard(evnt, 0, Session, this.ControllerContext);
            }
            ViewBag.Wizard = wiz;

            if(string.IsNullOrEmpty(id))
                return View(evnt);
            ViewBag.Suggested = true;

            //create suggested values for quick submission
            
            ResEventThemeService themeServ = new ResEventThemeService();
            ResEventTheme theme = themeServ.Load(id);
            wiz.Name=theme.Name;
            string nextEventMonth = serv.GetLatestYearMonthForEventType(id);
            DateTime nextAvailableYearMonth = DateTime.Now;
            if (!string.IsNullOrEmpty(nextEventMonth))
            {
                string year = nextEventMonth.Substring(0, 4);
                string month = nextEventMonth.Substring(4);

                nextAvailableYearMonth = DateTime.Parse(string.Format("{0}/01/{1}", month, year)).AddMonths(1);
                if (nextAvailableYearMonth < DateTime.Now)
                    nextAvailableYearMonth = DateTime.Now;
            }
            return View(SuggestedMonthlyEventFromTheme(theme,nextAvailableYearMonth.Month,nextAvailableYearMonth.Year));
        }

        private ResEvent SuggestedQuarterlyEventFromTheme(ResEventTheme theme)
        {
            ResEvent resEvent = new ResEvent();

            resEvent.MobileTemplate = new ResTemplate(theme.MobileTemplateId);
            resEvent.TabletTemplate = new ResTemplate(theme.TabletTemplateId);
            resEvent.Template = new ResTemplate(theme.TemplateId);

            int quarter = (int)Math.Floor((DateTime.Now.AddMonths(3).Month) / 4.0)+1;
            DateTime janFirstThisYear = DateTime.Parse(DateTime.Today.ToString("01/01/yyyy"));

            DateTime quarterStart = janFirstThisYear.AddMonths(-3 + ((quarter-1) * 3));
            DateTime quarterEnd = janFirstThisYear.AddMonths(0 + ((quarter - 1) * 3)).AddDays(-1);

            resEvent.Name = "Q" + quarter + string.Format(" Home Office {0} Tours", theme.Name);

            resEvent.Description = theme.Name + string.Format(" Tours for Q{0} of ",quarter) + janFirstThisYear.Year;
            resEvent.UrlName = string.Format("{0}tours-q{1}-{2}", theme.ResEventThemeId, quarter, janFirstThisYear.Year);

            resEvent.RegistrationAvailability = new DateRange(quarterStart, quarterEnd);
            resEvent.SiteAvailability = new DateRange(quarterStart.AddMonths(-1), quarterEnd);

            resEvent.ReservationType = new ReservationType(theme.ReservationTypeId);

            resEvent.Status = ResEventStatus.Draft;
            return resEvent;
        }

        private ResEvent SuggestedMonthlyEventFromTheme(ResEventTheme theme, int month, int twodigityear)
        {
            ResEvent resEvent = new ResEvent();

            resEvent.MobileTemplate = new ResTemplate(theme.MobileTemplateId);
            resEvent.TabletTemplate = new ResTemplate(theme.TabletTemplateId);
            resEvent.Template = new ResTemplate(theme.TemplateId);

            DateTime monthTime = DateTime.Parse(string.Format("{0}/01/{1}",month,twodigityear));
            
            
            DateTime monthStart = monthTime;
            DateTime monthEnd = monthTime.AddMonths(1).AddSeconds(-1);
            string strId = monthTime.ToString("MMMM yyyy");
            resEvent.Name = strId + string.Format(" Home Office {0} Tours", theme.Name);

            resEvent.Description = theme.Name + string.Format(" Tours for {0} ", strId);
            resEvent.UrlName = string.Format("{0}tours-{1}", theme.ResEventThemeId, strId.Replace(" ","-")).ToLower();

            
            resEvent.RegistrationAvailability = (new DateRange(monthStart, monthEnd));
            resEvent.SiteAvailability = new DateRange(monthStart.AddMonths(-1), monthEnd);

            resEvent.ReservationType = new ReservationType(theme.ReservationTypeId);

            resEvent.Status = ResEventStatus.Draft;
            return resEvent;
        }

        //
        // POST: /Admin/Event/Create-Dash

        [HttpPost]
        [ActionName("Create-Dash")]
        public ActionResult CreateDash(string id,ResEvent resEvent, FormCollection collection)
        {
            
            
            try
            {
                bool infered = resEvent == null;

                // TODO: Add create logic here
                if (!string.IsNullOrEmpty(collection["ResEventId"]))
                {
                    resEvent = serv.Load(collection["ResEventId"]);
                }
                if (infered)
                {
                    if(resEvent==null)
                        resEvent = new ResEvent();
                    resEvent.Name = collection["Name"];
                    resEvent.Description = collection["Description"];
                    resEvent.UrlName = collection["UrlName"];
                    resEvent.Status = (ResEventStatus)Enum.Parse(typeof(ResEventStatus), collection["Status"]);
                }

                OccurrenceService oServ = new OccurrenceService();
                
                resEvent.SiteAvailability = oServ.ConvertToTimeZoneContext(new DateRange(collection["SiteAvailability"]));
                resEvent.RegistrationAvailability = oServ.ConvertToTimeZoneContext(new DateRange(collection["RegistrationAvailability"]));


                resEvent.MobileTemplate = new ResTemplate(collection["MobileTemplate"]);
                resEvent.TabletTemplate = new ResTemplate(collection["TabletTemplate"]);
                resEvent.Template = new ResTemplate(collection["Template"]);
                resEvent.ReservationType = new ReservationType(collection["ReservationType"]);

                serv.Save(resEvent);

                Wizard wiz = initializeWizard(resEvent,0,Session, this.ControllerContext);            
                wiz.CurrentStep.Complete = true;
                string authority = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);
                wiz.CurrentStep.Uri = new Uri(authority + "/Admin/Event/Create-Dash?event=" + resEvent.Id());
                
                setWizard(wiz, Session);    
                return RedirectToAction("Edit-Dash", "Occurrence", new  { id =  resEvent.ProtoOccurrence.Id()} );
            }
            catch(Exception ex)
            {
                if (ex.Message.IndexOf("UrlName") > -1) {
                    ModelState.AddModelError("UrlName","That Marketable name is already in use.");
                }
                Wizard wiz = initializeWizard(resEvent, 0, Session, this.ControllerContext);
                ViewBag.Wizard = wiz;
                Console.Write(ex.Message);
                return View(resEvent);
            }
        }
        
        //
        // GET: /Admin/Event/Create

        public ActionResult Create()
        {
            return View();
        }


        //
        // POST: /Admin/Event/Create

        [HttpPost]
        public ActionResult Create(ResEvent resEvent, FormCollection collection)
        {
            try
            {
                // TODO: Add create logic here
                OccurrenceService oServ = new OccurrenceService();

                resEvent.SiteAvailability = oServ.ConvertToTimeZoneContext(new DateRange(collection["SiteAvailability"]));
                resEvent.RegistrationAvailability = oServ.ConvertToTimeZoneContext(new DateRange(collection["RegistrationAvailability"]));


                resEvent.MobileTemplate = new ResTemplate(collection["MobileTemplate"]);
                resEvent.TabletTemplate = new ResTemplate(collection["TabletTemplate"]);
                resEvent.Template = new ResTemplate(collection["Template"]);
                resEvent.ReservationType = new ReservationType(collection["ReservationType"]);

                serv.Save(resEvent);
                return RedirectToAction("Details", new {id=resEvent.Id() });
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Admin/Event/Edit/5

        public ActionResult Edit(int id)
        {
            ResEvent resEvent = serv.Load(id.ToString());

            return View(resEvent);
        }

        //
        // GET: /Admin/Event/Edit/5

        public ActionResult Activate(int id)
        {
            ResEvent resEvent = serv.Load(id.ToString());
            resEvent.Status = ResEventStatus.Live;
            serv.Save(resEvent);
            return RedirectToAction("Details", new { id=id,message="Event has been activated"});
        }


        //
        // POST: /Admin/Event/Edit/5

        [HttpPost]
        public ActionResult Edit(int id,ResEvent resEvent, FormCollection collection)
        {
            

            try
            {
                // TODO: Add update logic here
                OccurrenceService oServ = new OccurrenceService();

                resEvent.SiteAvailability = oServ.ConvertToTimeZoneContext(new DateRange(collection["SiteAvailability"]));
                resEvent.RegistrationAvailability = oServ.ConvertToTimeZoneContext(new DateRange(collection["RegistrationAvailability"]));


                resEvent.MobileTemplate = new ResTemplate(collection["MobileTemplate"]);
                resEvent.TabletTemplate = new ResTemplate(collection["TabletTemplate"]);
                resEvent.Template = new ResTemplate(collection["Template"]);
                resEvent.ReservationType = new ReservationType(collection["ReservationType"]);

                serv.Save(resEvent);

                

                return RedirectToAction("Details", new { id=resEvent.Id()});
            }
            catch
            {
                return View(resEvent);
            }
        }

        //
        // GET: /Admin/Event/Delete/5

        public ActionResult Delete(int id)
        {
            ResEvent resEvent = serv.Load(id.ToString());

            return View(resEvent);
        }

        //
        // POST: /Admin/Event/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            ResEvent resEvent = serv.Load(id.ToString());

            try
            {
                // TODO: Add delete logic here
                serv.Delete(resEvent);
                return RedirectToAction("Index", new { message="Event has been deleted"});
            }
            catch
            {
                return View(resEvent);
            }
        }

        //
        // GET: /Admin/Occurrences/Edit-Dash
        [ActionName("Edit-Dash")]
        public ActionResult EditDash(int id)
        {
            

            try
            {
                // TODO: Add delete logic here
                ResEvent resEvent = serv.Load(id.ToString());
                return RedirectToAction("Edit-Dash","Occurrence",new{id=resEvent.ProtoOccurrence.Id()});
            }
            catch
            {                
                return RedirectToAction("Index");
            }
        }

        static public Wizard<T> getWizard<T>(HttpSessionStateBase Session,int index)
        {
            string wizardId = "homeofficeidadminwizard";
            

            cfacore.shared.modules.com.admin.Wizard wiz = null;

            if (wiz!=null&&Session[wiz.SessionId] != null)
            {
                wiz = (cfacore.shared.modules.com.admin.Wizard)Session[wizardId];
            }
            if (wiz == null)
            {

                wiz = new cfacore.shared.modules.com.admin.Wizard(wizardId, 3);
                wiz.Description = "Creating a New Tour Event:";
                //TODO - add wizard steps

                wiz.Steps.Add(new WizardStep { Index = 0, Complete = false, Name = "Event Details", Uri = new Uri("http://tours.chick-fil-a.com"), Summary = "" });
                wiz.Steps.Add(new WizardStep { Index = 1, Complete = false, Name = "Slot Dashboard", Uri = new Uri("http://tours.chick-fil-a.com"), Summary = "" });
                wiz.Steps.Add(new WizardStep { Index = 2, Complete = false, Name = "Summary", Uri = new Uri("http://tours.chick-fil-a.com"), Summary = "" });
                
                

                //uncomment this when the above is complete
                //Session[wizardId] = wiz;
            }
            wiz.Index = index;
            return (wiz);

        }

        static public Wizard initializeWizard(ResEvent resEvent, int index, HttpSessionStateBase Session, ControllerContext context) {
            return initializeWizard(resEvent, index, Session, context, false);
        }

        static public Wizard initializeWizard(ResEvent resEvent, int index, HttpSessionStateBase Session, ControllerContext context,bool clear)
        {
            Wizard wiz = getWizard(Session, index);
            if (clear)
                Session[wiz.SessionId] = null;


            if (Session[wiz.SessionId] == null && resEvent != null && resEvent.IsBound())
            {
                wiz.Name = resEvent.Name;
                wiz.Description = "Creating a New Tour Event:";
                string authority = context.HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);
                wiz.Steps[0].Uri = new Uri(authority + "/Admin/Event/Create-Dash?event=" + resEvent.Id());
                wiz.Steps[0].Complete = true;
                wiz.Steps[1].Uri = new Uri(authority + "/Admin/Occurrence/Edit-Dash/" + resEvent.ProtoOccurrence.Id());
                wiz.Steps[1].Complete = true;
                wiz.Steps[2].Uri = new Uri(authority + "/Admin/Event/Summary/" + resEvent.Id());
                wiz.Steps[2].Complete = true;

                setWizard(wiz, Session);
            }
            return wiz;
        }

        static public void setWizard(Wizard wiz, HttpSessionStateBase Session)
        {
            Session[wiz.SessionId] = wiz;
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            
            if (ViewBag.Wizard != null && ViewBag.Wizard is Wizard)
            {
                (ViewBag.Wizard as Wizard).CurrentStep.Uri = new Uri(Request.Url.ToString());
                setWizard(ViewBag.Wizard as Wizard, Session);
            }
        }
        
    }
}
