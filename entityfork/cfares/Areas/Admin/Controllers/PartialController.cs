using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cfacore.site.controllers._event;
using cfares.domain._event;

namespace cfares.Areas.Admin.Controllers
{
    
    public class PartialController : Controller
    {
        //
        // GET: /Admin/Partial/

        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        [ActionName("Picker/ResTemplate")]
        public ActionResult EditorTemplates_Picker_ResTemplate(string FieldName,ResTemplate Value)
        {
            ViewData["FieldName"] = FieldName;
            if (Value != null)
            ViewData["Value"] = Value.ResTemplateId;
            ResEventTemplateService resService = new ResEventTemplateService();
            return PartialView("ChildActions/Picker/_ResTemplate",resService.LoadAll());
        }

        [ChildActionOnly]
        [ActionName("Picker/ResType")]
        public ActionResult EditorTemplates_Picker_ResType(string FieldName,ReservationType Value)
        {
            ViewData["FieldName"] = FieldName;
            if(Value!=null)
            ViewData["Value"] = Value.ReservationTypeId;
            ResEventTypeService resService = new ResEventTypeService();            
            return PartialView("ChildActions/Picker/_ResType",resService.LoadAll());
        }

    }
}
