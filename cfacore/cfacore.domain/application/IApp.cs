using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.shared.domain.media;
using cfacore.domain._base;
using core.synchronization.Automation;
using System.Data.Linq.Mapping;

namespace cfacore.domain.application
{
    [ITable]
    public interface IApp:IDomainObject
    {
        [Column]
        string Secret { get; set; }
        [Column]
        string Identifier { get; set; }
        [Column]
        Uri ResponseEndPoint { get; set; }
        [Column]
        string Name { get; set; }
        [Column]
        string DefaultScope { get; set; }
        [Column]
        IMedia IconId { get; set; }
    }
}
