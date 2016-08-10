using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq.Mapping;

namespace cfacore.shared.domain.attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class SyncUrlAttribute : Attribute
    {
        public string uri = "";
        public string update = "";
        public string create = "";
        public string delete = "";
        public SyncUrlAttribute(string uri)
        {
            this.uri = uri;
            this.update = uri;
            this.create = uri;
            this.delete = uri;
        }

        public SyncUrlAttribute(string uri, string insert)
        {
            this.uri = uri;
            this.create = insert;
            this.update = uri;
            this.delete = uri;
        }

        public SyncUrlAttribute(string uri, string insert, string update)
        {
            this.uri = uri;
            this.update = update;
            this.create = insert;
            this.delete = uri;
        }

        public SyncUrlAttribute(string uri, string insert, string update, string delete)
        {
            this.uri = uri;
            this.update = update;
            this.create = insert;
            this.delete = delete;
        }
    }
}
