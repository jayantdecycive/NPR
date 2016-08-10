using cfacore.domain._base;
using cfares.entity.dbcontext.res_event;
using cfares.repository._base;
using Omu.ValueInjecter;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cfares.site.modules.user;
using cfares.site.modules.com.application;
using cfares.repository.ticket;

namespace cfaresv2.Areas.Admin.Controllers._base
{
 

    public abstract class CrudControllerBase<TRepository, TEntity, TEntityViewModel, TKey,ITEntity> : AdminController
        where TEntity : class,IDomainObject, ITEntity, new()
        where TEntityViewModel : class,new()
        where TRepository : GenericRepository<IResContext, TEntity, TKey, ITEntity>
    {
        protected TRepository serv;
        protected TicketGuestsRepository guestRepo;

        protected virtual void FormValid(ITEntity entity, TEntityViewModel entityViewModel, FormCollection collection)
        {
            
        }

        public int? NullableInt(string str)
        {
            int i;
            if (int.TryParse(str, out i))
                return i;
            return null;
        }

        protected override IAsyncResult BeginExecute(System.Web.Routing.RequestContext requestContext, AsyncCallback callback, object state)
        {
            IResContext resContext = ReservationConfig.GetContext();
            serv = GetRepository(resContext);
            guestRepo = new TicketGuestsRepository(resContext);
            return base.BeginExecute(requestContext, callback, state);
        }

        public abstract TRepository GetRepository(IResContext context);

        //
        // GET: /Admin/TEntity/

        public virtual ActionResult Index()
        {
            
            return View(serv.GetAll());
        }


        //
        // GET: /Admin/TEntity/Confirm/

        public virtual ActionResult Confirm()
        {
            return View();
        }

        protected virtual bool AllowView(ITEntity entity)
        {
            return true;
        }

        protected virtual bool AllowCreate()
        {
            return true;
        }

        protected virtual bool AllowEdit(ITEntity entity)
        {
            return AllowCreate();
        }

        protected virtual bool AllowDelete(ITEntity entity)
        {
            return AllowEdit(entity);
        }

        protected void RigRights()
        {
            ViewData["CanCreate"] = AllowCreate();
            ViewBag.CanCreate = ViewData["CanCreate"];
        }

        protected void RigRights(ITEntity entity)
        {
            ViewData["CanEdit"] = AllowEdit(entity);
            ViewData["CanView"] = AllowView(entity);
            ViewData["CanDelete"] = AllowDelete(entity);
            ViewBag.CanEdit = ViewData["CanEdit"];
            ViewBag.CanView = ViewData["CanView"];
            ViewBag.CanDelete = ViewData["CanDelete"];
            RigRights();
        }

        protected void CheckEditRights(ITEntity entity)
        {
            if (!AllowEdit(entity))
                throw new Exception("You do not have permissions for this.");

            RigRights(entity);
        }

        protected void CheckCreateRights()
        {
            if (!AllowCreate())
                throw new Exception("You do not have permissions for this.");

            RigRights();
        }

        protected void CheckDeleteRights(ITEntity entity)
        {
            if (!AllowDelete(entity))
                throw new Exception("You do not have permissions for this.");

            RigRights(entity);
        }

        protected void CheckDeleteRights(ICollection<ITEntity> entities)
        {
            foreach (var entity in entities)
            {
                CheckDeleteRights(entity);
            }
        }

        protected void CheckViewRights(ITEntity entity)
        {
            if (!AllowView(entity))
                throw new Exception("You do not have permissions for this.");

            RigRights(entity);
        }

        //
        // GET: /Admin/TEntity/Details/5

        public virtual void SetViewBag()
        {

        }

        public virtual ActionResult Details(TKey id)
        {
            ITEntity entity = serv.Find(id);
            CheckViewRights(entity);
            SetViewBag();
            return View(entity);
        }

        
        protected virtual void InjectFromRequest(HttpRequestBase Request, TEntityViewModel med)
        {
            var d = QueryInjector();
            var appliedKeys = Request.QueryString.AllKeys.Where(d.ContainsKey);
            foreach (var appliedKey in appliedKeys)
            {
                d[appliedKey].Invoke(med, Request.QueryString[appliedKey]);
            }
        }

        protected virtual Dictionary<string, Action<TEntityViewModel, string>> QueryInjector()
        {
            return new Dictionary<string, Action<TEntityViewModel, string>>();
        }

        //
        // POST: /Admin/TEntity/Create


        //
        // GET: /Admin/TEntity/Create

        public virtual ActionResult Create()
        {
            CheckCreateRights();
            SetViewBag();
            TEntityViewModel med = new TEntityViewModel();
            InjectFromRequest(Request, med);
            return View(med);
        }


        //
        // GET: /Admin/TEntity/Edit/5

        public virtual ActionResult Edit(TKey id)
        {
            
            ITEntity entity = serv.Find(id);
            SetViewBag();
            CheckEditRights(entity);
            TEntityViewModel viewModel = InjectViewModel(entity);
            InjectFromRequest(Request, viewModel);
            return View(entity);
        }

        protected virtual TEntityViewModel InjectViewModel(ITEntity entity)
        {
            return entity as TEntityViewModel;
        }



        //
        // POST: /Admin/TEntity/Edit/5

        public virtual ITEntity Inject(TKey id,TEntityViewModel entity)
        {
            //ITEntity original = serv.Find(id);
            //IValueInjecter injecter = new ValueInjecter();
            //injecter.Inject(original, entity);
            return entity as TEntity;
        }

   
        //
        // GET: /Admin/TEntity/Delete/5

        public virtual ActionResult Delete(TKey id)
        {
            ITEntity entity = serv.Find(id);
            SetViewBag();
            CheckDeleteRights(entity);
            return View(entity);
        }

        //
        // POST: /Admin/TEntity/Delete/5
        public virtual string DeleteMessage(TKey id, ITEntity entity)
        {
            return string.Format("{0} {1} has been deleted", typeof (TEntity).Name, id);
        }

        public virtual string RefundMessage(TKey id)
        {
            return string.Format("Refund for Ticket {0} has been issued", id);
        }

        [HttpPost]
        public virtual ActionResult Delete(TKey id, FormCollection collection)
        {
            ITEntity entity = serv.Find(id);
            CheckDeleteRights(entity);
            SetViewBag();
            try
            {

                // TODO: Add delete logic here
                serv.Delete(entity);
                serv.Commit();
                return RedirectToAction("Index",new {message=DeleteMessage(id,entity)});
            }
            catch( Exception ex )
            {
				while( ex.InnerException != null ) ex = ex.InnerException;
				if( ex.Message.Contains("FK_dbo.Tickets_dbo.Slots_SlotId") )
					ModelState.AddModelError(string.Empty, "Unable to delete.  One or more tickets exist for this slot.  " + 
						"In the interest of not losing any data, please mark this record as non-Active instead of deleting.");
				else
					ModelState.AddModelError(string.Empty, ex.Message);

                return View(entity);
            }
        }

    }
}