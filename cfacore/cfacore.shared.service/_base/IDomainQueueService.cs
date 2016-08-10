using System;
using cfacore.shared.domain._base;
namespace cfacore.site.controllers._base
{
    public delegate void DeQueueEventHandler(object sender, DomainServiceEventArgs e);
    public delegate void QueueEventHandler(object sender, DomainServiceEventArgs e);

    interface IDomainQueueService<T>
    {
        T DeQueue(Uri uri);
        T DeQueueOrLoad(Uri uri);
        bool Queue(T obj, string action);
        bool QueueAndSave(T obj, string action);

        void OnDeQueue(DomainServiceEventArgs e);
        void OnQueue(DomainServiceEventArgs e);

        event DeQueueEventHandler Dequeued;
        event DeQueueEventHandler Queued;
    }
}
