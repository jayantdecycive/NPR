using System;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using cfacore.shared.domain.media;
using cfares.entity.dbcontext.res_event;
using cfares.repository.store;
using cfares.site.modules.com.Security;
using cfares.site.modules.com.application;
using cfaresv2.Areas.Admin.Controllers._base;
using cfacore.shared.modules.repository;
using cfares.site.modules.user;

namespace cfaresv2.Areas.Admin.Controllers
{
    [ReservationSystemAuthorize( Area = "Admin", Roles = "Admin" )]
    public class MediaController : CrudControllerBase<WebMediaRepository, Media, Media,int,IMedia>
    {

        protected override IAsyncResult BeginExecute(System.Web.Routing.RequestContext requestContext, AsyncCallback callback, object state)
        {
            ViewBag.EncType = "multipart/form-data";
            return base.BeginExecute(requestContext, callback, state);
        }

        public ActionResult Crop(int? id)
        {
            ViewBag.EncType = null;
            IMedia m = serv.Find(id.Value);
            return View(m as Media);
        }

        [HttpPost]
        public ActionResult Crop(int? id,Media media, FormCollection collection)
        {
            ViewBag.EncType = null;
            try
            {
                Rectangle crop =new Rectangle();
                try
                {
                    crop = new JavaScriptSerializer().Deserialize<Rectangle>(collection["Crop"]);
                    if (ModelState.ContainsKey("Crop"))
                        ModelState["Crop"].Errors.Clear();
                }
                catch
                {
                    
                }
                IMedia m = serv.Find(id.Value);
                m.Crop = crop;
                serv.Commit();
                return RedirectToAction("Details", new {id = id.Value});
            }
            catch 
            {
                IMedia m = serv.Find(id.Value);
                return View(m as Media);
            }
            
        }

        //
        // POST: /Admin/Media/Create
        [HttpPost]
        public ActionResult Create(Media media, FormCollection collection, HttpPostedFileBase media_upload)
        {
           
            var membershipService = new UserMembershipRepository(HttpContext, serv.Context);
            //try
            {
                // TODO: Add create logic here

                media.CreatedDate = DateTime.Now;
                media.Owner = membershipService.Current;
                

                if (media_upload != null)
                {
                    
                    media.Uri(serv.GenUri(media.Name, "panel", media_upload.FileName));
                    media=serv.UploadFromInput(media, media_upload) as Media;
                }
                else if (!string.IsNullOrEmpty(media.ExternalUriStr))
                {

                    media.Uri(serv.GenUri(media.Name, "panel", media.ExternalUriStr));
                    media = serv.DownloadFromUri(media) as Media;
                    
                }
                else {
                    ModelState.AddModelError("", "You must upload a file or provide a url.");
                    return View(media);
                }
                serv.Add(media);
                serv.Commit();
                return RedirectToAction("Crop", new { id = media.MediaId, message = "Please Crop Your Image", continued = "true" });
                
            }
            /*catch(Exception ex)
            {
                Console.WriteLine(ex);
                return View(media);
            }*/
        }
        

        //
        // POST: /Admin/Media/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, Media media, FormCollection collection, HttpPostedFileBase media_upload)
        {
            try
            {
                // TODO: Add update logic here
                if (media_upload != null)
                {

                    //media.Uri(serv.GenUri(media.Name, "panel", media_upload.FileName));
                    media = serv.UploadFromInput(media, media_upload) as Media;
                }
                else if (!string.IsNullOrEmpty(media.ExternalUriStr))
                {

                    //media.Uri(serv.GenUri(media.Name, "panel", media.ExternalUriStr));
                    media = serv.DownloadFromUri(media) as Media;

                }
                



                IMedia original = Inject(id, media);
                CheckEditRights(original);
                FormValid(original, media as Media, collection);
                serv.Edit(original);
                serv.Commit();
                return RedirectToAction("Crop", new { id = id, message = "Please Crop Your Image", continued = "true" });
            }
            catch (DbEntityValidationException ex)
            {
                Console.Write(ex.Message);
                return View(media);
            }
        }

        


        public override WebMediaRepository GetRepository(IResContext context)
        {
            return new WebMediaRepository(context);
        }
    }
}
