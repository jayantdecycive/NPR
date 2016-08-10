
#region Imports

using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using Omu.ValueInjecter;
using cfacore.shared.modules.helpers;
using cfares.domain.store;
using cfares.repository.store;
using cfares.site.modules.com.application;
using cfaresv2.Areas.Admin.Controllers._base;
using System.Web.Mvc;

#endregion

namespace cfaresv2.Areas.Admin.Controllers
{
    public class LocationController : CrudController<LocationRepository, ResStore, string>
    {

        public override LocationRepository GetRepository(cfares.entity.dbcontext.res_event.IResContext context)
        {
            return new LocationRepository(context);
        }

        //
        // GET: /Admin/TEntity/Create

        public virtual ActionResult CreateShort()
        {
            CheckCreateRights();
            ResStore med = new ResStore();
            med.Email = Uri.EscapeUriString(ReservationConfig.GetConfig().Organization.AdminEmailAddress);
            med.Playground = "none";

            InjectFromRequest(Request, med);
            return View(med);
        }

        [HttpPost]
        public virtual ActionResult CreateShort(ResStore entity, FormCollection collection)
        {
            CheckCreateRights();
            ResStore newEntity;
            try
            {
                // TODO: Add create logic here                
                newEntity = new ResStore();
                IValueInjecter injecter = new ValueInjecter();
                injecter.Inject(newEntity, entity);
                FormValid(newEntity, entity, collection);
                serv.Add(newEntity);
                serv.Commit();
                return RedirectToAction("Details", new { id = newEntity.Id(),label=string.Format("{0} ({1})",newEntity.Name,newEntity.MaximumCapacity) });

            }
            catch (DbUpdateException ex)
            {
	            Exception e = ex;
				while( e.InnerException != null ) e = e.InnerException;
				if( e.Message.Contains( "PK_dbo.Stores" ) && e.Message.Contains( "duplicate key value" ) )
					return View(entity).AddModelStateError( ModelState, "Location number already in use" );

				return View(entity).AddModelStateError( ModelState, e.Message );
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var error in ex.EntityValidationErrors)
                    foreach (var modelError in error.ValidationErrors)
                        ModelState.AddModelError(modelError.PropertyName, modelError.ErrorMessage);

				return View(entity);
            }
        }


        //
        // GET: /Admin/TEntity/Edit/5

        public virtual ActionResult EditShort(string id)
        {

            ResStore entity = serv.Find(id);

            CheckEditRights(entity);
            ResStore viewModel = InjectViewModel(entity);
            InjectFromRequest(Request, viewModel);
            return View(entity);
        }

        [HttpPost]
        public override ActionResult Create(ResStore entity, FormCollection collection)
        {
            CheckCreateRights();
	        try
            {
                // TODO: Add create logic here                
                ResStore newEntity = new ResStore();
                
                IValueInjecter injecter = new ValueInjecter();
                injecter.Inject(newEntity, entity);
                FormValid(newEntity, entity, collection);

                if (string.IsNullOrEmpty(entity.Email))
                {
                    newEntity.Email = ReservationConfig.GetConfig().Organization.AdminEmailAddress;
                }
                if (string.IsNullOrEmpty(entity.Playground))
                {
                    newEntity.Playground = "none";
                }

                serv.Add(newEntity);
                serv.Commit();
                return RedirectToAction("Details", new { id = newEntity.Id() });

            }
            catch (DbEntityValidationException ex)
            {
                foreach (var error in ex.EntityValidationErrors)
                    foreach (var modelError in error.ValidationErrors)
                        ModelState.AddModelError(modelError.PropertyName, modelError.ErrorMessage);

				return View(entity);
            }
            /*catch
            {
                return View(entity);
            }*/
        }

        [HttpPost]
        public override ActionResult Edit(string id, ResStore entity, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                ResStore original = Inject(id, entity);
                CheckEditRights(original);
                FormValid(original, entity, collection);
                if (original.StreetAddressId != null && original.StreetAddress != null &&
                    original.StreetAddress.AddressId != original.StreetAddressId)
                {
                    original.StreetAddress.AddressId = original.StreetAddressId.Value;
                }
                serv.Edit(original);
                serv.Commit();
                return RedirectToAction("Details", new { id = id });
            }
            catch (DbEntityValidationException ex)
            {
                Console.Write(ex.Message);
                return View(entity);
            }
        }

        [HttpPost]
        public virtual ActionResult EditShort(string id, ResStore entity, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                ResStore original = Inject(id, entity);
                CheckEditRights(original);
                FormValid(original, entity, collection);
                serv.Edit(original);
                serv.Commit();
                return RedirectToAction("Details", new { id = id });
            }
            catch (DbEntityValidationException ex)
            {
                Console.Write(ex.Message);
                return View(entity);
            }
        }

    }
}
