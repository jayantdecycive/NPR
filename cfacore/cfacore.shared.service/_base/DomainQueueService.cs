using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain._base;
using cfacore.shared.domain._base;

namespace cfacore.site.controllers._base
{
    public abstract class DomainQueueService<T> : DomainCacheService<T>, IDomainQueueService<T> where T : IDomainObject, new()
    {
        public abstract bool Queue(T obj, string action);
        public abstract T DeQueue(Uri uri);

        public abstract bool QueueAndSave(T obj, string action);
        public abstract T DeQueueOrLoad(Uri uri);


        
        public event DeQueueEventHandler Dequeued;        
        public event DeQueueEventHandler Queued;


        public void OnDeQueue(DomainServiceEventArgs e)
        {
            if (Dequeued != null)
                Dequeued(this, e);
        }

        public void OnQueue(DomainServiceEventArgs e)
        {
            if (Queued != null)
                Queued(this, e);
        }
    }    
}
