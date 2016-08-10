using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LumenWorks.Framework.IO.Csv;
using System.IO;
using System.Text;
using cfacore.site.controllers.shared;
using cfares.domain._event.slot.tours;
using cfares.domain._event.ticket.tours;
using cfacore.shared.domain.user;

namespace cfares.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,cfa")]
    public class CsvController : Controller
    {
        //
        // GET: /Admin/Csv/
        [HttpPost]
        public ActionResult Table(string id, FormCollection collection)
        {
            string data = Request.Params["data"];
            

            
            
            string filename = id + "_Table_" + DateTime.Now.ToString("_M-dd-yyyy_m-hh-ss");
            using (StreamWriter outfile =
                new StreamWriter(filename))
            {
                outfile.Write(data);
            }

            
            
            string href = string.Format("/Data/Csv/admin/{0}.csv", filename);
            string filepath = HttpContext.Request.MapPath("~" + href);
            

            return Json(new { file = filepath, filename = filename, href = href });

        }


        [HttpPost]
        public ActionResult iVisitorReport(FormCollection collection)
        {
            SlotService serv = new SlotService();
            
            string id = Request.Params["slot"];
            string data = serv.ToiVisitorCSV(id,",",true,true);
            //string filename = "iVisitor_slot_" + id + "_" + DateTime.Now.ToString("_M-dd-yyyy_m-hh-ss");
            string filename = "cfadailyvisitors";
            string href = string.Format("/Data/Csv/admin/{0}.csv", filename);
            string filepath = HttpContext.Request.MapPath("~" + href);

            using (StreamWriter outfile =
                new StreamWriter(filepath))
            {
                outfile.Write(data);
            }



            

            return Json(new { file = filepath, filename = filename, href = href });

        }

    }

}
