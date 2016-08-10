using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cfares.site.modules.common
{
    interface IRendersView
    {
        string ViewToString(string viewName, object model);
    }
}
