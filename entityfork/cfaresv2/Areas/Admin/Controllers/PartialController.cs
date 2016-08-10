
#region Imports

using System.Linq;
using System.Web.Mvc;
using cfacore.domain.store;
using cfares.entity.dbcontext.res_event;
using cfares.site.modules.com.application;
using cfaresv2.Areas.Admin.Controllers._base;

#endregion

namespace cfaresv2.Areas.Admin.Controllers
{
    public class PartialController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        [ActionName("Picker/ResTemplate")]
        public ActionResult EditorTemplates_Picker_ResTemplate(string fieldName,string value)
        {
            ViewData["FieldName"] = fieldName;
            if (value != null)
            ViewData["Value"] = value;
            IResContext resService = ReservationConfig.GetContext();
            return PartialView("ChildActions/Picker/_ResTemplate",resService.ResTemplate.ToList());
        }

        [ChildActionOnly]
        [ActionName("Picker/ResType")]
        public ActionResult EditorTemplates_Picker_ResType(string fieldName,string value)
        {
            ViewData["FieldName"] = fieldName;
            if(value!=null)
            ViewData["Value"] = value;
            IResContext resService = ReservationConfig.GetContext();    
            return PartialView("ChildActions/Picker/_ResType",resService.ReservationTypes.OrderBy( o => o.Name ).ToList());
        }

        [ChildActionOnly]
        [ActionName("Picker/ResCategory")]
        public ActionResult EditorTemplates_Picker_ResCategory( string fieldName, string value )
        {
            ViewData["FieldName"] = fieldName;
            if( value != null ) ViewData["Value"] = value;
            IResContext resService = ReservationConfig.GetContext();
            return PartialView("ChildActions/Picker/_ResCategory",
                resService.ReservationCategories.OrderBy( o => o.Name ).ToList());
        }
    }
}
