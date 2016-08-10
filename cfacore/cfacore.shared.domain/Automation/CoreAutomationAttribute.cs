using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq.Mapping;

namespace core.synchronization.Automation
{
    [AttributeUsage(AttributeTargets.Interface|AttributeTargets.Class)]
    public class ITableAttribute : DataAttribute
    {
        
    }
    /*
    [AttributeUsage(AttributeTargets.Class)]
    public class CoreAutomationTableAttribute : Attribute 
    {
        private string table;
        public CoreAutomationTableAttribute(string table)
        {
            this.table = table;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class CoreAutomationPropertyAttribute : Attribute
    {
        private string property;
        public CoreAutomationPropertyAttribute(string property)
        {
            this.property = property;
        }
    }
     * */
}
