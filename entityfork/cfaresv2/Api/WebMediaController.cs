using System.Net.Http.Headers;
using cfacore.shared.domain.media;
using cfacore.shared.modules.repository;
using cfares.domain._event;
using cfares.domain.aggregates;
using cfares.entity.dbcontext.res_event;
using cfares.repository._event;
using cfares.repository.slot;
using cfares.site.modules.com.application;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Objects;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace cfaresv2.Api
{
    

    public class WebMediaController : ApiController
    {
        WebMediaRepository repo;
        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            IResContext context = ReservationConfig.GetContext();
            repo = new WebMediaRepository(context);
            base.Initialize(controllerContext);
        }
        // GET api/reseventaggregate
        public IQueryable<Media> Get()
        {
            return null;
        }

        [ActionName("GetS3Thumb")]
        public string GetS3Thumb(int id)
        {
            var media = repo.Find(id);
            bool cropped = Request.GetQueryNameValuePairs().FirstOrDefault(x => x.Key == "cropped").Value != null;
            var width = ParseNullable(Request.GetQueryNameValuePairs().FirstOrDefault(x => x.Key == "width").Value);
            var height = ParseNullable(Request.GetQueryNameValuePairs().FirstOrDefault(x => x.Key == "height").Value);
            return repo.GetS3Thumb(media, cropped, width, height);
        }

        [HttpGet]
        [ActionName("RedirectS3Thumb")]
        public HttpResponseMessage RedirectS3Thumb(int id)
        {
            var media = repo.Find(id);
            bool cropped = Request.GetQueryNameValuePairs().FirstOrDefault(x => x.Key == "cropped").Value != null;
            var width = ParseNullable(Request.GetQueryNameValuePairs().FirstOrDefault(x => x.Key == "width").Value);
            var height = ParseNullable(Request.GetQueryNameValuePairs().FirstOrDefault(x => x.Key == "height").Value);
            var url= repo.GetS3Thumb(media, cropped, width, height);
            var response = new HttpResponseMessage(HttpStatusCode.MovedPermanently);
            response.Headers.Location = new Uri(url);
            return response;
        }

        /*[ActionName("FindThumb")]
        public HttpContent FindThumb(int id)
        {
            var url = GetS3Thumb(id);
            var response = Request.CreateResponse(HttpStatusCode.Moved);
            response.Headers.Location = new Uri(url);
            
            return response.Content;
        }*/

        protected int? ParseNullable(string val)
        {
            if (string.IsNullOrEmpty(val))
                return null;
            return int.Parse(val);
        }

        // GET api/reseventaggregate/5
        public Media Get(int id)
        {

            return repo.Find(id) as Media;

        }


       
    }
}
