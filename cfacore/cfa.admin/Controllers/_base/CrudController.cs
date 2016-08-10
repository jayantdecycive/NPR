using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Omu.ValueInjecter;
using cfa.admin.context.cfa_com;
using cfares.repository._base;

namespace cfa.admin.Controllers._base
{
    public abstract class CrudController<TRepository, TEntity, TKey> : CrudController<TRepository, TEntity, TEntity, TKey, TEntity>
        where TEntity : class,new()
        where TRepository : GenericRepository<CfaComContext, TEntity, TKey>
    { }

    public abstract class ExtendableCrudController<TRepository, TEntity, TKey, ITEntity> : CrudController<TRepository, TEntity, TEntity, TKey, ITEntity>
        where TEntity : class,ITEntity, new()
        where TRepository : GenericRepository<CfaComContext, TEntity, TKey, ITEntity>
    { }

    public abstract class CrudController<TRepository, TEntity, TEntityViewModel, TKey, ITEntity> : Controller
        where TEntity : class,ITEntity, new()
        where TEntityViewModel : class,new()
        where TRepository : GenericRepository<CfaComContext, TEntity, TKey, ITEntity>
    {
        protected TRepository serv;

        protected virtual void FormValid(ITEntity entity, TEntityViewModel entityViewModel, FormCollection collection)
        {

        }



        protected override IAsyncResult BeginExecute(System.Web.Routing.RequestContext requestContext, AsyncCallback callback, object state)
        {
            CfaComContext resContext = new CfaComContext();
            serv = GetRepository(resContext); 
            return base.BeginExecute(requestContext, callback, state);
        }

        public abstract TRepository GetRepository(CfaComContext context);

        //
        // GET: /Admin/TEntity/

        public virtual ActionResult Index()
        {

            return View();
        }


        //
        // GET: /Admin/TEntity/Confirm/

        public virtual ActionResult Confirm()
        {
            return View();
        }


        //
        // GET: /Admin/TEntity/Details/5

        public virtual ActionResult Details(TKey id)
        {
            ITEntity entity = serv.Find(id);

            return View(entity);
        }

        //
        // GET: /Admin/TEntity/Create

        public virtual ActionResult Create()
        {
            TEntityViewModel med = new TEntityViewModel();
            return View(med);
        }

        //
        // POST: /Admin/TEntity/Create

        [HttpPost]
        public virtual ActionResult Create(TEntityViewModel entity, FormCollection collection)
        {
            TEntity newEntity;
            try
            {
                // TODO: Add create logic here                
                newEntity = new TEntity();
                IValueInjecter injecter = new ValueInjecter();
                injecter.Inject(newEntity, entity);
                FormValid(newEntity, entity, collection);
                serv.Add(newEntity);
                serv.Commit();
                return RedirectToAction("Index");

            }
            catch (DbEntityValidationException ex)
            {
                foreach (var error in ex.EntityValidationErrors)
                {
                    foreach (var modelError in error.ValidationErrors)
                        ModelState.AddModelError(modelError.PropertyName, modelError.ErrorMessage);
                }
                return View(entity);
            }
            catch
            {
                return View(entity);
            }
        }



        //
        // GET: /Admin/TEntity/Edit/5

        public virtual ActionResult Edit(TKey id)
        {
            ITEntity entity = serv.Find(id);
            TEntityViewModel viewModel = InjectViewModel(entity);
            return View(entity);
        }

        protected virtual TEntityViewModel InjectViewModel(ITEntity entity)
        {
            return entity as TEntityViewModel;
        }

        //
        // POST: /Admin/TEntity/Edit/5

        public virtual ITEntity Inject(TKey id, TEntityViewModel entity)
        {
            ITEntity original = serv.Find(id);
            IValueInjecter injecter = new ValueInjecter();
            injecter.Inject(original, entity);
            return original;
        }

        [HttpPost]
        public virtual ActionResult Edit(TKey id, TEntityViewModel entity, FormCollection collection)
        {


            try
            {
                // TODO: Add update logic here

                ITEntity original = Inject(id, entity);
                FormValid(original, entity, collection);
                serv.Edit(original);
                serv.Commit();
                return RedirectToAction("Details", new { id = id });
            }
            catch
            {
                return View(entity);
            }
        }

        //
        // GET: /Admin/TEntity/Delete/5

        public virtual ActionResult Delete(TKey id)
        {
            ITEntity entity = serv.Find(id);
            return View(entity);
        }

        //
        // POST: /Admin/TEntity/Delete/5
        public virtual string DeleteMessage(TKey id, ITEntity entity)
        {
            return string.Format("{0} {1} has been deleted", typeof(TEntity).Name, id);
        }

        [HttpPost]
        public virtual ActionResult Delete(TKey id, FormCollection collection)
        {
            ITEntity entity = serv.Find(id);

            try
            {
                // TODO: Add delete logic here
                serv.Delete(entity);
                serv.Commit();
                return RedirectToAction("Index", new { message = DeleteMessage(id, entity) });
            }
            catch
            {
                return View(entity);
            }
        }

    }
}