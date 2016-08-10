using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LumenWorks.Framework.IO.Csv;
using System.IO;
using ExcelLibrary.SpreadSheet;

using cfares.domain._event.slot.tours;

using cfares.domain._event;
using cfares.domain.user;
using cfares.entity.dbcontext.res_event;
using cfacore.shared.modules.com.admin;
using cfares.site.modules.com.Security;
using cfares.site.modules.com.application;
using cfaresv2.Areas.Admin.Controllers._base;


namespace cfaresv2.Areas.Admin.Controllers
{
    [ReservationSystemAuthorize( Area = "Admin", Roles = "Admin,Operator" )]
    public class ExcelController : AdminController
    {
        //
        // GET: /Admin/Excel/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EventSummary(string id,  FormCollection collection) {
            IResContext serv = ReservationConfig.GetContext(); ;
            ResEvent evnt = serv.ResEvents.Find(id);
            

            string data = Request.Params["data"];
            Workbook workbook = new Workbook();
            Worksheet eventsheet = new Worksheet("Event "+evnt.Name);
            int baseline = 0;

            string[][] eventdata = new string[][] { 
                new string[]{"Event Name",evnt.Name}, 
                new string[]{"Event Type",evnt.ReservationType.ReservationTypeId}, 
                new string[]{"Description",evnt.Description}, 
                new string[]{"Status",Enum.GetName(typeof(ResEventStatus),evnt.Status)}, 
                new string[]{"Site Available On",evnt.SiteStart.ToLongDateString()+" "+evnt.SiteStart.ToLongTimeString()}, 
                new string[]{"Site Unavailable On",evnt.SiteEnd.ToLongDateString()+" "+evnt.SiteEnd.ToLongTimeString()} };

            for (int i = 0; i < eventdata.Length; i++) {                
                    eventsheet.Cells[0, i] = new Cell(eventdata[i][0]);
                    eventsheet.Cells[1, i] = new Cell(eventdata[i][1]);
            }
                //eventsheet.Cells[baseline,int]


                workbook.Worksheets.Add(eventsheet);

            Worksheet slotsheet = new Worksheet("Slots for Event " + evnt.Name);
            baseline = 0;

            using (CsvReader csv =
           new CsvReader(new StringReader(data), true, ',', '\'', '\\', '#', ValueTrimmingOptions.QuotedOnly))
            {
                int fieldCount = csv.FieldCount;

                string[] headers = csv.GetFieldHeaders();
                for (int i = 0; i < headers.Length; i++)
                {                    
                    slotsheet.Cells[baseline, i] = new Cell(headers[i]);
                }
                baseline++;
                
                while (csv.ReadNextRecord())
                {
                    baseline++;
                    for (int i = 0; i < fieldCount; i++)
                    {
                        slotsheet.Cells[baseline, i] = new Cell(csv[i]);
                    }
                    
                }
            }

            
            workbook.Worksheets.Add(slotsheet);

            string filename = "ReservationEvent" + id + DateTime.Now.ToString("_M-dd-yyyy_m-hh-sstt");
            string href = string.Format("/Data/Xls/admin/{0}.xls", filename);
            string filepath = HttpContext.Request.MapPath("~"+href);
            workbook.Save(filepath);
            

            return Json(new { file=filepath, filename=filename,href=href});

        }

        [HttpPost]
        public ActionResult TourSlotSummary(string id, FormCollection collection)
        {
            IResContext serv = ReservationConfig.GetContext(); 
            TourSlot slot = serv.TourSlots.Find(id);


            string data = Request.Params["data"];
            Workbook workbook = new Workbook();
            Worksheet eventsheet = new Worksheet("Slot " + slot.SlotId);
            int baseline = 0;

            string[][] eventdata = new string[][] { 
                new string[]{"Event Name",slot.Occurrence.ResEvent.Name}, 
                new string[]{"Event Type",slot.Occurrence.ResEvent.ReservationType.ReservationTypeId}, 
                new string[]{"Event Description",slot.Occurrence.ResEvent.Description}, 
                new string[]{"Event Status",Enum.GetName(typeof(ResEventStatus),slot.Occurrence.ResEvent.Status)}, 
                new string[]{"Event Site Available On",slot.Occurrence.ResEvent.SiteStart.ToLongDateString()+" "+slot.Occurrence.ResEvent.SiteStart.ToLongTimeString()}, 
                new string[]{"Event Site Unavailable On",slot.Occurrence.ResEvent.SiteEnd.ToLongDateString()+" "+slot.Occurrence.ResEvent.SiteEnd.ToLongTimeString()} };

            for (int i = 0; i < eventdata.Length; i++)
            {
                eventsheet.Cells[baseline, i] = new Cell(eventdata[i][0]);
                eventsheet.Cells[baseline+1, i] = new Cell(eventdata[i][1]);
            }

            baseline += 3;
            string guideMobile = "";
            string guideEmail = "";
            string guideName = "Guide Not Assigned";
            string guideProfile = "";

            if(slot.Guide!=null){
            guideMobile = slot.Guide.MobilePhone != null ? slot.Guide.MobilePhone.ToFormattedString() : "";
            guideEmail = slot.Guide.Email;
            guideName = slot.Guide.Name.ToString();
            guideProfile = slot.Guide.AnchorAbsoluteHref();
            }

            string[][] slotdata = new string[][] { 
                new string[]{"Slot Id",slot.Id()}, 
                new string[]{"Slot Capacity",slot.Capacity.ToString()},                 
                new string[]{"Lead Guide",guideName}, 
                new string[]{"Lead Guide Email",guideEmail}, 
                new string[]{"Lead Guide Phone",guideMobile}, 
                new string[]{"Lead Guide Profile",guideProfile}, 
                new string[]{"Slot Status",Enum.GetName(typeof(SlotStatus),slot.Status)}, 
                new string[]{"Slot Start",slot.Start.LocalDateTime.ToLongDateString()+" "+slot.Start.LocalDateTime.ToLongTimeString()}, 
                new string[]{"Slot End",slot.End.LocalDateTime.ToLongDateString()+" "+slot.End.LocalDateTime.ToLongTimeString()} };

            for (int i = 0; i < slotdata.Length; i++)
            {
                eventsheet.Cells[baseline, i] = new Cell(slotdata[i][0]);
                eventsheet.Cells[baseline + 1, i] = new Cell(slotdata[i][1]);
            }

            baseline += 2;


            string[] cameos=new string[]{"Additional Guides","Executive Appearances","Cathys","Cows","Staff"};

            for(int i =0;i<cameos.Length;i++){
                string cameoMessage = cameos[i];
                ResUserCollection cameoSet=null;
                switch(i){
                    case 0:
                        cameoSet = new ResUserCollection(slot.CameoSets.AdditionalGuides.ToList());
                        break;
                    case 1:
                        cameoSet = new ResUserCollection(slot.CameoSets.ExecutiveCameos.ToList());
                        break;
                    case 2:
                        cameoSet = new ResUserCollection(slot.CameoSets.CathyCameos.ToList());
                        break;
                    case 3:
                        cameoSet = new ResUserCollection(slot.CameoSets.CowCameos.ToList());
                        break;
                    case 4:
                    default:
                        cameoSet = new ResUserCollection(slot.CameoSets.StaffCameos.ToList());
                        break;
                }
                if (cameoSet.Count == 0)
                    continue;

                baseline += 1;
                eventsheet.Cells[baseline, 0] = new Cell(cameoMessage.ToUpper());
                baseline += 1;

                

                for (int j = 0; j < cameoSet.Count; j++)
                {
                    ResUser cameo = cameoSet[j];

                    string[][] cameodata = new string[][] {                 
                        new string[]{"Name",cameo.Name.ToString()}, 
                        new string[]{"Email",cameo.Email},                 
                        new string[]{"Phone",cameo.MobilePhone.ToFormattedString()}, 
                        new string[]{"Profile",cameo.AnchorAbsoluteHref()}, 
                    };

                    if (j == 0)
                    {
                        eventsheet.Cells[baseline, j] = new Cell(cameodata[j][0]);
                        baseline++;
                        eventsheet.Cells[baseline + 1, j] = new Cell(cameodata[j][1]);
                    }
                    else {
                        eventsheet.Cells[baseline, j] = new Cell(cameodata[j][1]);
                    }
                    baseline++;
                }

            }

            //eventsheet.Cells[baseline,int]


            workbook.Worksheets.Add(eventsheet);

            Worksheet slotsheet = new Worksheet("Tickets for Slot " + slot.SlotId);
            baseline = 0;

            using (CsvReader csv =
           new CsvReader(new StringReader(data), true, ',', '\'', '\\', '#', ValueTrimmingOptions.QuotedOnly))
            {
                

                string[] headers = csv.GetFieldHeaders();
                for (int i = 0; i < headers.Length; i++)
                {
                    slotsheet.Cells[baseline, i] = new Cell(headers[i]);
                    
                }
                baseline++;

                int fieldCount = csv.FieldCount;

                while (csv.ReadNextRecord())
                {
                    baseline++;
                    for (int i = 0; i < fieldCount; i++)
                    {
                        slotsheet.Cells[baseline, i] = new Cell(csv[i]);
                    }

                }
            }


            workbook.Worksheets.Add(slotsheet);

            string filename = "SlotSummary" + slot.Start.ToString("_M-dd-yyyy_m-hh-ss");
            string href = string.Format("/Data/Xls/admin/{0}.xls", filename);
            string filepath = HttpContext.Request.MapPath("~" + href);
            workbook.Save(filepath);


            return Json(new { file = filepath, filename = filename, href = href });

        }


        [HttpPost]
        public ActionResult Table(string id, FormCollection collection)
        {
            

            string data = Request.Params["data"];
            Workbook workbook = new Workbook();
            
            int baseline = 0;


            

            Worksheet datasheet = new Worksheet("Table Data for "+id);
            baseline = 0;

            using (CsvReader csv =
           new CsvReader(new StringReader(data), true, ',', '\'', '\\', '#', ValueTrimmingOptions.QuotedOnly))
            {

                
                string[] headers = csv.GetFieldHeaders();
                for (int i = 0; i < headers.Length; i++)
                {
                    Cell headerCell = new Cell(headers[i]);
                    
                    datasheet.Cells[baseline, i] = headerCell;

                }
                baseline++;

                int fieldCount = csv.FieldCount;

                while (csv.ReadNextRecord())
                {
                    baseline++;
                    for (int i = 0; i < fieldCount; i++)
                    {
                        datasheet.Cells[baseline, i] = new Cell(csv[i]);
                    }

                }
                while (baseline <= 25) {
                    baseline++;
                    for (int i = 0; i < fieldCount; i++)
                    {
                        datasheet.Cells[baseline, i] = new Cell("");
                    }
                }
            }


            workbook.Worksheets.Add(datasheet);

            string filename = id+"_Table_" + DateTime.Now.ToString("_M-dd-yyyy_m-hh-ss");
            string href = string.Format("/Data/Xls/admin/{0}.xls", filename);
            string filepath = HttpContext.Request.MapPath("~" + href);
            workbook.Save(filepath);


            return Json(new { file = filepath, filename = filename, href = href });

        }

    }
}
