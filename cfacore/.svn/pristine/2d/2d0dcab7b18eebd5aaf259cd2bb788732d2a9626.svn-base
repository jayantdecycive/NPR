using System;
using System.Web;
using System.ComponentModel;
using Ninject;
using ServiceStack.CacheAccess;
using cfacore.shared.domain._base;

namespace cfacore.domain._base
{
    [Serializable]
    public abstract class DomainObject : IDomainObject
    {
        public DomainObject() { 
        
        }

	    private static IExtendedCacheClient _cache;
		public static IExtendedCacheClient Cache
		{
			get
			{
				if( _cache == null )
				{
					StandardKernel kernel = new StandardKernel();
					//kernel.Bind<IRedisClientsManager>().ToMethod( c => new BasicRedisClientManager( "localhost:6379" ) ).InSingletonScope();
					//kernel.Bind<ICacheClient>().ToMethod(c => c.Kernel.Get<IRedisClientsManager>().GetCacheClient());
					kernel.Bind<ICacheClient>().To<ExtendedCacheClient>();
					kernel.Bind<IExtendedCacheClient>().To<ExtendedCacheClient>();
					_cache = kernel.Get<IExtendedCacheClient>();
				}
				return _cache;
			}
		}

        public virtual string GetEntityType()
        {
            return this.GetType().Name;
        }
        

        public DomainObject(DomainObject toClone):this() {
            if (toClone != null)
            {
                this.Id(toClone.Id());
                this._Bound = toClone.IsBound();
            }
        }

        public virtual string ToHtmlString()
        {
	        return HttpContext.Current != null ? 
				HttpContext.Current.Server.HtmlEncode( ToString() ) : 
				ToString();
        }

	    public virtual Uri Uri(){ return new Uri(UriBase() + Id()); }
        public virtual void Uri(Uri uri){ /*pass*/  }
        [Bindable(false)]
        public abstract string UriBase();

        public virtual string UriBase(string toAdd)
        {
            return UriBase() + toAdd;
        }

        [Bindable(false)]
        public virtual bool IsBound(){
            return !string.IsNullOrEmpty(_Id)&&_Bound;
        }
        protected bool _Bound = false;
        [Bindable(false)]
        public virtual void UnBind() {
            this._Id = null;
            this._Bound = false;
        }
        public virtual void Bind()
        {
            this._Bound = true;
        }
        protected string _Id = null;

        

        private bool _Loaded { get; set; }
        public bool Loaded() {
            return _Loaded;
        }

        public void Loaded(bool isLoaded)
        {
            _Loaded = isLoaded;
        }

        public virtual void Id(string Id)
        {
            if(!string.IsNullOrEmpty(Id))
                this.Bind();
            this._Id=Id;
        }

        public virtual void Id(int Id)
        {
            if (Id==0)
                this.Bind();
            this._Id = Id.ToString();
        }

        public virtual string Id()
        {
            if (string.IsNullOrEmpty(_Id))
                return null;
            return this._Id;
        }

        public virtual int IntId()
        {
            if (_Id == null)
                return 0;
            return int.Parse(_Id);
        }

        

        public virtual void Id(string Id,bool bind)
        {
            if (bind)
                this.Bind();
            this._Id = Id;
        }

        
        public virtual string StrUri() {
             return Uri()!=null?Uri().ToString():string.Empty;             
        }
        public virtual void StrUri(string value) { 
            if(!string.IsNullOrEmpty(value))Uri(new Uri(value));
        }

        
        
        public abstract string ToChecksum();
        
    }
}
