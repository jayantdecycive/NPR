
#region Imports

using Omu.ValueInjecter;
using cfacore.domain.store;
using cfacore.shared.domain.store;
using cfares.domain._event;
using cfares.domain.store;
using cfares.repository._event;
using cfares.repository.store;
using cfaresv2.Areas.Admin.Controllers._base;
using System.Web.Mvc;
using System.Linq;

#endregion

namespace cfaresv2.Areas.Admin.Controllers
{
    public class ReservationTypeController : CrudController<ReservationTypeRepository, ReservationType, string>
    {

        public override ReservationTypeRepository GetRepository(cfares.entity.dbcontext.res_event.IResContext context)
        {
            return new ReservationTypeRepository(context);
        }

        protected override void FormValid(ReservationType entity, ReservationType entityViewModel, FormCollection collection)
        {
            /*string[] urls = collection["Urls"].Split(',');
            var urlrepo = new ResSiteUrlRepository(serv.Context);

            var urlsString = string.Join(",", urls.ToList().Select(x => x.Trim('/')).Where(x => !urlrepo.GetAll().Any(y => y.Url.EndsWith(x))));
            entity.Urls = urlsString;*/

            //ModelState.Clear();
            base.FormValid(entity, entityViewModel, collection);
        }

        public override ReservationType Inject(string id, ReservationType entity)
        {
            //var typ = serv.Find(id);
            ReservationType original = serv.Find(id);
            if (original == null)
                return entity;
            //IValueInjecter injecter = new ValueInjecter();
            //injecter.Inject(original, entity);
            original.Urls = entity.Urls;
            original.Description = entity.Description;
            original.Name = entity.Name;
            original.ReservationTypeId = entity.ReservationTypeId;
            return original;
        }
    }
}
