using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cfacore.service;
using cfacore.shared.domain.user;
using cfares.Models.datatable;
using cfares.Models;
using cfacore.site.controllers.shared;
using cfares.domain._event;

namespace cfares.Controllers
{
    public class DiagnosticsController : Controller
    {
        //
        // GET: /Diagnostics/

        public ActionResult Index()
        {
            
            AddressService serv = new AddressService();
            //Creating address
            /*if (CreateAddress)
            {
                Address myAddress = new Address();
                myAddress.City = "Atlanta";
                myAddress.State = "GA";
                myAddress.Name = new Name("Matthew Ryan Drake");
                //Name object does this:
                Console.Write(myAddress.Name.First);

                myAddress.Zip = new Zip(30309);
                //Zip object does this:
                Console.Write(myAddress.Zip.Code);
                Console.Write(myAddress.Zip.PlusFour);

                myAddress.Line1 = "195 14th Street NE";
                myAddress.Line2 = "TS2";
                myAddress.Label = "Matts Home Address";
                serv.Save(myAddress);
            }
            //Fetching address
            else
            {
                Address loadedAddress = serv.Load(1);
                Console.Write(loadedAddress);
            }*/


            return View();
        }

        public ActionResult CacheTest()
        {
            string result = "";

            SlotService mySlotService = new SlotService();
            Slot theSlot = mySlotService.DeCache("foo");
            if (theSlot == null) {
                result += "slot not cached";
                theSlot = mySlotService.Load(1912);
                mySlotService.Cache("foo",theSlot);
            }
            result += "<br />slot: "+theSlot.Start.ToLongDateString();
            return Content(result);
        }

        public ActionResult DataTablesSimple()
        {
            return View();
        }

        public ActionResult Backbone()
        {
            return View();
        }

        public ActionResult LoadAlternativeView()
        {
            DataTableInitialization initVars = new DataTableInitialization
            {
                jsonServiceAPI = "/Diagnostics/GetTableOfAddressData",
                useODataQuery = false
            };

            return View(initVars);
        }


        public ActionResult GetTableOfAddressData(DataTableParams tableParams)
        {

            DataTableResponse response = new DataTableResponse
            {
                sEcho = int.Parse(tableParams.sEcho),
                sColumns = "var1,var2,var3,var4",
                iTotalDisplayRecords = 3,
                iTotalRecords = 3,
                aaData = new List<string[]>()
                {
                   new string[] {"1", "Microsoft", "Redmond", "USA"},
                    new string[] {"2", "Google", "Mountain View", "USA"},
                    new string[] {"13", "Gowi", "Pancevo", "Serbia"}
                }
            };

            return Json(response, JsonRequestBehavior.AllowGet);

        }

    }
}
