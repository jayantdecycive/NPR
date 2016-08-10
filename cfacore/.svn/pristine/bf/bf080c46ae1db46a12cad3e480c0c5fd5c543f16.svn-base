using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain._base;
using cfacore.shared.domain._base;

namespace cfacore.site.controllers._base
{
    public abstract class DomainService<T> : IDomainService<T> where T : IDomainObject, new()
    {
        //protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public abstract T[] Search(KeyValuePair<string, string>[] criteria);
        public abstract bool Save(T obj);
        public abstract bool Delete(T obj);
        public abstract T Load(string ID);
        public abstract T Load(Uri uri);
        public abstract List<T> GetAll();
        
        public virtual T Load(int ID) {
            return Load(ID.ToString());
        }


        public event SearchEventHandler Searched;
        public event LoadEventHandler Loaded;        
        public event SaveEventHandler Saved;
        public event SaveEventHandler BeforeSaved;
        public event DeleteEventHandler Deleted;


        public void OnLoad(DomainServiceEventArgs e)
        {
            if (e.target == null)
                return;
            e.target.Loaded(true);
            if (Loaded != null)
                Loaded(this, e);
        }

        public void OnLoadCollection(IList<T> collection)
        {
            foreach (T t in collection)
                this.OnLoad(new DomainServiceEventArgs { target = t, isBatch = true});            
        }

        public void OnSave(DomainServiceEventArgs e)
        {
            if (Saved != null)
                Saved(this, e);
        }

        public void OnBeforeSave(DomainServiceEventArgs e)
        {
            if (BeforeSaved != null)
                BeforeSaved(this, e);
        }

        public void OnDelete(DomainServiceEventArgs e)
        {
            if (Deleted != null)
                Deleted(this, e);
        }

        public void OnSearch(DomainServiceEventArgs e)
        {
            if (Searched != null)
                Searched(this, e);
        }
    }
}
