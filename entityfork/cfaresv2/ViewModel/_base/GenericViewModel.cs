using cfacore.domain._base;
using Omu.ValueInjecter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cfaresv2.ViewModel._base
{
    public abstract class GenericViewModel<TEntity> : DomainObject
        where TEntity : DomainObject, new()
    {
        public virtual void Inject(TEntity entity) {
            IValueInjecter injecter = new ValueInjecter();            
            injecter.Inject(this, entity);            
        }

        public override string UriBase()
        {
            throw new NotImplementedException();
        }

        public override string ToChecksum()
        {
            throw new NotImplementedException();
        }
    }
}