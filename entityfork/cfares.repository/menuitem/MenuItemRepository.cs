using System;
using System.Linq;
using cfacore.shared.domain.common;
using cfacore.shared.domain.media;
using cfares.domain._event;
using cfares.domain._event.menu;
using cfares.entity.dbcontext.res_event;
using cfares.repository._base;
using cfares.repository.store;

namespace cfares.repository.menuitem
{
    public class MenuItemRepository : GenericRepository<IResContext, MenuItem, string>
    {
        public override MenuItem FindBySlug(string slug)
        {
            return Find(x => x.DomId == slug);
        }



        public MenuItemRepository(IResContext context)
            : base(context)
        {
        }



        public MenuItem FindOrCreateBySlug(string p)
        {
            var item = FindBySlug(p);
            if (item != null)
            {
                return item;
            }
            item = new MenuItem() { DomId = p };
            Add(item);
            Commit();
            return item;
        }



        public MenuItem FindOrRemember(string id)
        {
            //todo: implement a cache or read-only db
            return Find(id);
        }
    }
}
