using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cfares.site.modules.com.admin;
using System.Xml;
using cfacore.shared.modules.com.admin;

namespace cfares.Areas.Tours.Controllers
{
    public class WizardController : Controller
    {
        [HttpPost]
        public ActionResult Index(string wizard, string step, string domainid, FormCollection collection)
        {
            return View();
        }

        //
        // GET: /Admin/Wizard/

        public ActionResult Index(string wizard, string step, string domainid)
        {

            Wizard wizardInst = (Wizard)LoadWizardFromXml(wizard);
            if (step == "0")
            {
                wizardInst.OnStart(null);
                return RedirectToRoute(new { controller = "Wizard", wizard = wizard, step = "1", domainid = domainid });
            }
            int iStep = int.Parse(step);
            wizardInst.Index = iStep;
            if (wizardInst.Complete)
            {
                wizardInst.OnComplete(null);
                return RedirectToRoute(new { controller = "Home", action = "Index" });
            }

            ViewBag.Wizard = wizardInst;


            return View();
        }


        private IWizard LoadWizardFromXml(string name)
        {
            XmlDocument doc = new XmlDocument();
            string src = HttpContext.Request.MapPath(string.Format("~/Data/Xml/Wizards/{0}.xml", name));
            try
            {
                doc.Load(src);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

            return Wizard.LoadFromXml(doc.DocumentElement);
        }

    }
}
