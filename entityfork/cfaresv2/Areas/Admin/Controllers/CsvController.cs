using System;
using System.Web.Mvc;
using System.IO;
using cfares.domain._event.slot.tours;
using cfares.entity.dbcontext.res_event;
using cfares.site.modules.com.Security;
using cfares.site.modules.com.application;
using cfaresv2.Areas.Admin.Controllers._base;

namespace cfaresv2.Areas.Admin.Controllers
{
    [ReservationSystemAuthorize( Area = "Admin", Roles = "Admin" )]
    public class CsvController : AdminController
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
            IResContext serv = ReservationConfig.GetContext();
            
            string id = Request.Params["slot"];
            TourSlot slot = serv.TourSlots.Find(id);
            string data = slot.ToiVisitorCSV(",",true,true);
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
