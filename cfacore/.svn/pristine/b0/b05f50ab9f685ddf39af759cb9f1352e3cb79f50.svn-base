using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain._base;

namespace cfacore.dao._base
{
    
    public abstract class CacheAccess<T> : DataAccessObject<T>, ICacheAccess<T> where T : IDomainObject, new()
    {

        /// <summary>
        /// The Connection String
        /// </summary>
        protected string _ConnectionString = string.Empty;

        /// <summary>
        /// Gets the Default Timeout.
        /// </summary>
        public TimeSpan DEFAULT_TIMEOUT
        {
            get { return new TimeSpan(0, 10, 0); }
        }

        public virtual string ConnectionString { 
            get {
                return _ConnectionString;
            } 
            set {
                _ConnectionString=value;
            } 
        }

        public abstract bool Delete(Uri uri);
        public virtual bool Delete(string key) {
            T t = new T();
            return Delete(new Uri(t.UriBase(key)));
        }
        
        public virtual bool Save(Uri uri, T obj){
            return Save(uri.ToString(),obj);
        }

        public virtual bool Save(string key, T obj) {             
            return Save<T>(obj.UriBase(key),obj);
        }

        public virtual bool Save<Q>(Uri uri, Q obj) where Q : T, new()
        {
            return Save<Q>(uri.ToString(),obj);
        }

        public virtual bool Save<Q>(Q obj) where Q : T, new()
        {
            return Save<Q>(obj,DEFAULT_TIMEOUT);
        }


        public virtual bool Save<Q>(Q obj, TimeSpan timeout) where Q : T, new()
        {
            return Save<Q>(obj.Uri().ToString(),obj,timeout);
        }
        
        public virtual bool Save<Q>(string key, Q obj) where Q : T,new(){
            return Save<Q>(key,obj,DEFAULT_TIMEOUT);
        }

        public virtual bool Save<Q>(string key, Q obj, TimeSpan timeout) where Q : T,new() {
            Q q = new Q();
            q.Id(key);
            return Save(q.Uri(),obj,timeout);
        }

        public abstract bool Save<Q>(Uri key, Q obj, TimeSpan timeout) where Q : T, new();

        public override T Load(Uri uri){
            return Load<T>(uri);
        }
        public override T Load(string uri)
        {
            return Load<T>(uri);
        }

        public abstract Q Load<Q>(Uri uri) where Q : T;

        public virtual Q Load<Q>(string key) where Q : T,new() {
            Q q = new Q();
            return Load<Q>(new Uri(q.UriBase(key)));
        }






        public override bool Save(T obj)
        {
            return Save<T>(obj);
        }

        public override bool Delete(T obj)
        {
            return Delete(obj.Uri());
        }
    }
    
}
