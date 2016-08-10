using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cfares.service
{
    public class ConfigManager
    {
        public static string MySqlConnectionString {
            get {
                return System.Configuration.ConfigurationManager.ConnectionStrings["cfa-res-mysql"].ConnectionString;
                //return System.Configuration.ConfigurationManager.ConnectionStrings["cfa-res-mysql-staging"].ConnectionString;
            }
        }

        public static string AppFabricConnectionString
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings["cfa-app-fabric"].ConnectionString;
            }
        }

        public static string ElastiCacheConnectionString
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings["cfa-elasticache"].ConnectionString;
            }
        }

        public static string TargetDomainPort
        {
            get {
                return "tours.chick-fil-a.com";
            }
        }

        public static string QueueConnectionString { get { return System.Configuration.ConfigurationManager.ConnectionStrings["cfa-queue"].ConnectionString; } }
    }
}
