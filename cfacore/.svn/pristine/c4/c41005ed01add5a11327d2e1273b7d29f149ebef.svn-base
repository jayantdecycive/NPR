using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cfacore.shared.domain.common
{
    public interface IAdminReference
    {
        string AnchorHref();
        string AnchorLabel();
        string Id();
        bool IsBound();
        bool Loaded();
    }

    public interface ILoadableAdminReference:IAdminReference
    {   
        string ClientModel();
        string ClientLabel();
        string GetEntityType();
    }
}
