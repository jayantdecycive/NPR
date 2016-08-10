using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cfares.Areas.tours.Models.Service
{
    public abstract class FormModelService<T> where T : class
    {
        private HttpContextBase context;        
        public FormModelService(HttpContextBase context) {
            this.context = context;
        }

        public abstract string Id { get; }

        public FormModelService()
        {
            this.context = null;
        }

        public virtual void Clean()
        {
            context.Session.Remove(Id);
        }

        public virtual void Store(T t)
        {
            context.Session[Id] = t;
        }

        public virtual T Pull()
        {
            return context.Session[Id] as T;
        }

        public abstract void Commit(T t);

        public abstract void Initialize(T t);
    }

    
}