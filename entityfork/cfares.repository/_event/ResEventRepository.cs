
#region Imports

using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using cfares.domain._event.resevent;
using cfares.repository._base;
using cfares.entity.dbcontext.res_event;
using cfares.domain._event;

#endregion

namespace cfares.repository._event
{
	#region Nongeneric Implementation

	public class ResEventRepository : ResEventRepository<ResEvent> {
        public ResEventRepository(IResContext context) : base(context) { }

        
    }

	#endregion

	public class ResEventRepository<TResEvent> : GenericRepository<IResContext, TResEvent, int,IResEvent>,IResEventRepository
        where TResEvent : class,IResEvent, new()
	{
		#region Constructor / Settings

        public static IResEventRepository Get(IResEvent evt,IResContext context)
        {
            var repoType = typeof (ResEventRepository<ResEvent>);
            var newRepoType = repoType.GetGenericTypeDefinition().MakeGenericType(evt.GetType());
            return Activator.CreateInstance(newRepoType,new object[]{context}) as IResEventRepository;
        }

		public ResEventRepository(IResContext context) : base(context) { }

		// ReSharper disable StaticFieldInGenericType
	    
		public static readonly string[] DefaultEntityIncludes = 
			new[] { "ReservationType", "Template","SiteUrls","Media" };

		// ReSharper restore StaticFieldInGenericType

		#endregion

		#region Get Queries ( Multiple Results )

        public IQueryable<IResEvent> GetActive()
        {
			DateTime now = DateTime.Now;
			return GetAll( DefaultEntityIncludes )
				.Where( e => e.RegistrationStart < now && now < e.RegistrationEnd
								  && e.SiteStart < now && now < e.SiteEnd
						&& e.Status != ResEventStatus.Cancelled && e.Status != ResEventStatus.Archive && e.Status != ResEventStatus.Draft);
        }

        public IQueryable<IResEvent> GetByActiveAndStateProvinceCode(string stateProvinceCode)
        {
			// Case insensitive based on DB collation settings
			return GetActive()
				.Join( Context.Occurrences, e => e.ResEventId, o => o.ResEventId, 
					(e, o) => new {
						Event = e,
						Occurence = o,
					} )
				.Where( i => i.Occurence.Store.StreetAddress.State == stateProvinceCode )
				.Select( i => i.Event );
		}

        

        public override void Delete(IResEvent entity)
        {
            var ocRepo = new OccurrenceRepository(Context);
            foreach (IOccurrence oc in entity.Occurrences.ToList())
            {
                ocRepo.Delete(oc);
            }
            base.Delete(entity);
        }

		#endregion

		#region Find Queries ( Single Result )

		public IResEvent FindById(int id) {
            return Find(x=>x.ResEventId==id, DefaultEntityIncludes);
        }

        public IResEvent FindByUri(Uri uri)
        {
			return FindByUrl( FormatUrlAndPath( uri ) );
        }

		public static string FormatUrlAndPath( Uri uri )
		{
			return string.Format("{0}{1}{2}{3}{4}",
				uri.Scheme,
				Uri.SchemeDelimiter,
				uri.Host,
				uri.Port == 80 ? string.Empty : ":" + uri.Port,
				uri.AbsolutePath );
		}

        public IResEvent FindByHost(string host)
        {
            // Case insensitive based on DB collation settings
            return GetAll( "SiteUrls" ).Where( o => o.SiteUrls.Any( u => host == u.Url ) )
				.OrderByDescending( i => i.SiteUrls.Max( ii => ii.Url.Length ) )
				.FirstOrDefault();
        }

        public IResEvent FindByUrl(string url)
        {
            // Case insensitive based on DB collation settings
            var uri = new Uri(url);
            var host = uri.Host;
            return SiteUrlEventCache.Where(o => o.SiteUrls.Any(u => u.Url.Contains("//" + host)))
				.Where(x=>!x.IsEventEnded)
				.OrderByDescending( i => i.SiteUrls.Max( ii => ii.Url.Length ) )
				.FirstOrDefault();
        }

		private IList<IResEvent> _siteUrlEventCache;
		protected IList<IResEvent> SiteUrlEventCache
		{
			get { return _siteUrlEventCache ?? (_siteUrlEventCache = GetAll("SiteUrls").ToList()); }
			set { _siteUrlEventCache = value; }
		}

		public override IResEvent Find(int key)
        {
            return Find(x => x.ResEventId == key, DefaultEntityIncludes);
        }

        public IResEvent FindWithOccurrences(int key)
        {
            var e = Find(x => x.ResEventId == key, DefaultEntityIncludes);
			_entities.Entry( e ).Collection( "Occurrences" ).Load();
	        return e;
        }

        public override IResEvent FindBySlug(string slug)
        {
            return FindByUrl(slug);
        }

		#endregion

		#region Temporary Event Generation

        public IResEvent TempEvent()
        {
            return TempEvent(null);
        }

        public IResEvent TempEvent(ReservationType resType)
        {
            IResEvent evnt;
            if (resType == null)
            {
                evnt = new TResEvent();
            }
            else
            {
                evnt = resType.GetKernel().Get<IResEvent>();
            }
            
            evnt.Status = ResEventStatus.Temp;
            evnt.Name = "Temp Event " + Guid.NewGuid().ToString().Substring(0, 6);
            evnt.RegistrationStart = DateTime.Now;
            evnt.RegistrationEnd = DateTime.Now;
            evnt.SiteEnd = DateTime.Now;
            evnt.SiteStart = DateTime.Now;
            evnt.Urls = Slugify(evnt.Name);

            return evnt;
        }

		#endregion

        #region Events

       
        public override void OnSave(object sender, SavedEventArgs<TResEvent> args)
        {
            //mdrake: need to kill removed site urls
            var deadResSiteUrls = Context.SiteUrls.Where(x => (x.ResEventId == null && x.ResEventTypeId == null)||x.Url==string.Empty||x.Url==null);
            foreach (var deadResSiteUrl in deadResSiteUrls)
            {
                Context.SiteUrls.Remove(deadResSiteUrl);    
            }

          

            base.OnSave(sender, args);
        }

        

        #endregion


        public IResEvent FindOrRemember(int p)
        {
            return Find(p);
        }

        public ResSiteUrl[] FindUsedUrls(string[] urls)
        {
            return Context.SiteUrls.Where(x=>urls.Contains(x.Url)).ToArray();
        }

        public ResSiteUrl[] FindUsedUrls(IResEvent except, string[] urls)
        {
            //urls = urls.Where(x => Context.SiteUrls.All(u => u.Url == x && u.ResEventId!=except.ResEventId)).ToArray();
            return Context.SiteUrls.Where(x => urls.Contains(x.Url)&&x.ResEventId!=except.ResEventId).ToArray();
            //Context.SiteUrls.Where(x => urls.Contains(x.Url)).Where(u=>u.ResEventId!=except.ResEventId).Select(s => s.Url).ToArray();
        }


        
    }
}
