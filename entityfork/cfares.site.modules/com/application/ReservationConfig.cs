
#region Imports

using System;
using System.Data.Entity;
using System.Linq;
using Ninject;
using cfares.domain._event;
using cfares.entity.dbcontext.res_event;
using npr.domain._event.slot;
using npr.domain._event.ticket;
using npr.entity.dbcontext.npr_res;
using System.Configuration;
using System.Web;
using cfatours.entity.dbcontext;

#endregion

namespace cfares.site.modules.com.application
{
    public class ReservationConfig : ConfigurationSection, IResApplicationConfiguration
    {
		public IKernel GetKernel( ReservationType type, IKernel k = null )
		{
            if( k == null ) k = type.GetKernel();
		    
			if( ( type.Id() == "Tour" || type.Id() == "SpecialEvent" ) &&
                AppContext.Current!=null&&
                AppContext.Current.Configuration.ApplicationId == Application.NPR)
		    {
                k.Rebind<ITicket>().To<NPRTicket>();
                k.Rebind<ISlot>().To<NPRSlot>();
		    }
		    return k;
		}

        public bool HasWizardNav()
        {
            return ApplicationId == Application.NPR;
        }

        public string LogOnTitle()
        {
	        return ApplicationId == Application.NPR ? 
				"NPR Reservation System" : "Log On";
        }

	    public bool HasToursAndEvents()
        {
            return ApplicationId == Application.NPR;
        }

        public bool HasTitle()
        {
            return ApplicationId == Application.CFA || ApplicationId == Application.CFATour;
        }

        [ConfigurationProperty("id", DefaultValue = "res", IsRequired = true, IsKey = true)]
        public string ApplicationId { 
            get { return this["id"] as string; } 
            set { this["id"] = value; } 
        }

        [ConfigurationProperty("enablePerformanceOptimizations", DefaultValue = "true", IsRequired = false, IsKey = false)]
        public bool EnablePerformanceOptimizations { 
            get { return bool.Parse( this["enablePerformanceOptimizations"].ToString() ); } 
            set { this["enablePerformanceOptimizations"] = value; }
        }

        [ConfigurationProperty("enableBundling", DefaultValue = "true", IsRequired = false, IsKey = false)]
        public bool EnableBundling { 
            get { return bool.Parse( this["enableBundling"].ToString() ); } 
            set { this["enableBundling"] = value; }
        }

        public IResContext Context
        {
            get { return GetContext(); }
        }

        public DbContext DbContext
        {
            get { return (DbContext) GetContext(); }
        }

        [ConfigurationProperty("key", IsRequired = false, IsKey=false )]
        public string Key { 
            get { return this["key"] as string; } 
            set { this["key"] = value; } 
        }

        [ConfigurationProperty("proximitySearchDisabled", IsRequired = false, IsKey=false )]
        public string ProximitySearchDisabled { 
            get { return this["proximitySearchDisabled"] as string; } 
            set { this["proximitySearchDisabled"] = value; } 
        }

        [ConfigurationProperty("admin")]
        public ReservationConfigAdminElement Admin {
            get { return (ReservationConfigAdminElement)this["admin"]; }
            set { this["admin"] = value; }
        }

        [ConfigurationProperty("sso")]
        public ReservationConfigSSOElement SSO {
            get { return (ReservationConfigSSOElement)this["sso"]; }
            set { this["sso"] = value; }
        }

        [ConfigurationProperty("organization")]
        public ReservationConfigOrganizationElement Organization {
            get { return (ReservationConfigOrganizationElement)this["organization"]; }
            set { this["organization"] = value; }
        }

        [ConfigurationProperty("eventTypes", IsRequired = false, IsDefaultCollection = true)]
        public ReservationConfigEventTypesCollection EventTypes {
            get { return (ReservationConfigEventTypesCollection)this["eventTypes"]; }
            set { this["eventTypes"] = value; }
        }

        public class ReservationConfigEventTypesCollection : ConfigurationElementCollection 
        {
			public AddElement GetByKey( string key )
			{
				return this.OfType<AddElement>()
					.FirstOrDefault( a => a.Name.Equals( key, StringComparison.OrdinalIgnoreCase ) );
			}

	        protected override ConfigurationElement CreateNewElement()
	        {
		        return new AddElement();
	        }

	        protected override object GetElementKey(ConfigurationElement element)
	        {
				return ((AddElement) element).Name;
	        }
        }

		public class AddElement : ConfigurationElement
		{
			[ConfigurationProperty("name", IsKey = true, IsRequired = true)]
			public string Name 
			{
				get { return (string) base["name"]; }
				set { base["name"] = value; }
			}

			[ConfigurationProperty("defaultLocation", IsKey = false, IsRequired = false)]
			public string DefaultLocation
			{
				get { return (string) base["defaultLocation"]; }
				set { base["defaultLocation"] = value; }
			}

            [ConfigurationProperty("defaultTemplate", IsKey = false, IsRequired = false)]
            public string DefaultTemplate
            {
                get { return (string)base["defaultTemplate"]; }
                set { base["defaultTemplate"] = value; }
            }
		}

		public class ReservationConfigOrganizationElement : ConfigurationElement
        {
            [ConfigurationProperty( "id", DefaultValue = "", IsRequired = true, IsKey = true)]
            public string Id { 
                get { return this["id"] as string; } 
                set { this["id"] = value; } }

            [ConfigurationProperty( "name", DefaultValue = "", IsRequired = true )]
            public string Name { 
                get { return this["name"] as string; } 
                set { this["name"] = value; } }

            [ConfigurationProperty( "displayName", DefaultValue = "", IsRequired = true )]
            public string DisplayName { 
                get { return this["displayName"] as string; } 
                set { this["displayName"] = value; } }

			[ConfigurationProperty( "adminEmailAddress", DefaultValue = "", IsRequired = false )]
            public string AdminEmailAddress { 
                get { return ( this["adminEmailAddress"] as string ) ?? string.Empty; } 
                set { this["adminEmailAddress"] = value; } 
            }

			[ConfigurationProperty( "websiteUrl", DefaultValue = "", IsRequired = false )]
            public string WebsiteUrl { 
                get { return this["websiteUrl"] as string; } 
                set { this["websiteUrl"] = value; } }

            [ConfigurationProperty( "websiteDisplayName", DefaultValue = "", IsRequired = false )]
            public string WebsiteDisplayName { 
                get { return this["websiteDisplayName"] as string; } 
                set { this["websiteDisplayName"] = value; } }

            [ConfigurationProperty( "socialFooterEnabled", DefaultValue = "true", IsRequired = false )]
            public bool SocialFooterEnabled { 
                get { return bool.Parse( this["socialFooterEnabled"].ToString() ); } 
                set { this["socialFooterEnabled"] = value; } }

            [ConfigurationProperty( "facebookFollowUrl", DefaultValue = "", IsRequired = true )]
            public string FacebookFollowUrl { 
                get { return this["facebookFollowUrl"] as string; } 
                set { this["facebookFollowUrl"] = value; } }

            [ConfigurationProperty( "twitterFollowUrl", DefaultValue = "", IsRequired = true )]
            public string TwitterFollowUrl { 
                get { return this["twitterFollowUrl"] as string; } 
                set { this["twitterFollowUrl"] = value; } }

            [ConfigurationProperty( "twitterFollowDisplayName", DefaultValue = "", IsRequired = true )]
            public string TwitterFollowDisplayName { 
                get { return this["twitterFollowDisplayName"] as string; } 
                set { this["twitterFollowDisplayName"] = value; } }

            [ConfigurationProperty( "userLoginEnabled", DefaultValue = "true", IsRequired = false )]
            public bool UserLoginEnabled { 
                get { return bool.Parse( this["userLoginEnabled"].ToString() ); } 
                set { this["userLoginEnabled"] = value; } }

            [ConfigurationProperty( "eventEndedEmail", DefaultValue = "", IsRequired = true )]
            public string EventEndedEmail { 
                get { return this["eventEndedEmail"] as string; } 
                set { this["eventEndedEmail"] = value; } }

            [ConfigurationProperty( "mailUri", DefaultValue = "", IsRequired = true )]
            public string MailUri { 
                get { return this["mailUri"] as string; } 
                set { this["mailUri"] = value; } }

			[ConfigurationProperty( "postageAppClientKey", DefaultValue = "", IsRequired = false )]
            public string PostageAppClientKey { 
                get { return ( this["postageAppClientKey"] as string ) ?? string.Empty; } 
                set { this["postageAppClientKey"] = value; } 
            }

			[ConfigurationProperty( "postageAppBaseUri", DefaultValue = "", IsRequired = false )]
            public string PostageAppBaseUri { 
                get { return ( this["postageAppBaseUri"] as string ) ?? string.Empty; } 
                set { this["postageAppBaseUri"] = value; } 
            }
		}

        public class ReservationConfigSSOElement : ConfigurationElement
        {
            [ConfigurationProperty( "enabled", DefaultValue = "false", IsRequired = false )]
            public bool IsEnabled { 
                get { return bool.Parse( this["enabled"].ToString() ); } 
                set { this["enabled"] = value; } }

            [ConfigurationProperty( "usernameEmailDomain", DefaultValue = "", IsRequired = false )]
            public string UsernameEmailDomain { 
                get { return this["usernameEmailDomain"] as string; } 
                set { this["usernameEmailDomain"] = value; } }

            [ConfigurationProperty( "signinPageUrl", DefaultValue = "", IsRequired = false )]
            public string SigninPageUrl { 
                get { return this["signinPageUrl"] as string; } 
                set { this["signinPageUrl"] = value; } }

            [ConfigurationProperty( "signoutPageUrl", DefaultValue = "", IsRequired = false )]
            public string SignoutPageUrl { 
                get { return this["signoutPageUrl"] as string; } 
                set { this["signoutPageUrl"] = value; } }

            [ConfigurationProperty( "changePasswordUrl", DefaultValue = "", IsRequired = true )]
            public string ChangePasswordUrl { 
                get { return this["changePasswordUrl"] as string; } 
                set { this["changePasswordUrl"] = value; } }

            [ConfigurationProperty( "realmUri", DefaultValue = "", IsRequired = true )]
            public string RealmUri { 
                get { return this["realmUri"] as string; } 
                set { this["realmUri"] = value; } }
        }

        public class ReservationConfigAdminElement : ConfigurationElement
        {
            [ConfigurationProperty( "hostName", DefaultValue = "", IsRequired = true )]
            public string HostName { 
                get { return ( this["hostName"] as string ) ?? string.Empty; } 
                set { this["hostName"] = value; } 
            }

            [ConfigurationProperty( "area", DefaultValue = "Admin", IsRequired = false )]
            public string Area { 
                get { return ( this["area"] as string ) ?? string.Empty; } 
                set { this["area"] = value; } 
            }

            [ConfigurationProperty( "appTitle", DefaultValue = "Reservation Admin", IsRequired = false )]
            public string AppTitle { 
                get { return ( this["appTitle"] as string ) ?? string.Empty; } 
                set { this["appTitle"] = value; } 
            }
        }

        public const string ContextKey = "reservation-config";
        public const string ViewDataKey = "ReservationConfig";

        private const string ConfigurationSectionName = "reservationConfig";

        public static void SetConfig(ReservationConfig config) {
            if (HttpContext.Current != null)
                HttpContext.Current.Items[ContextKey] = config;
        }

        public static ReservationConfig GetConfig() 
        {
            if (HttpContext.Current != null && HttpContext.Current.Items.Contains(ContextKey))
                return (ReservationConfig)HttpContext.Current.Items[ContextKey];

            return (ReservationConfig)ConfigurationManager
                .GetSection(ConfigurationSectionName)??new ReservationConfig();
        }

        public static IResContext GetContext()
        {
	        var config = GetConfig();
            if (config.ApplicationId == Application.NPR)
                return new NprContext();
	        if (config.ApplicationId == Application.CFATour)
		        return new CfaToursContext();

	        return new CfaResContext();
        }

	    public static IResContext GetContext(string connectionString) {
            var config = GetConfig();
            if (config.ApplicationId == Application.NPR)
                return new NprContext(connectionString);
		    if (config.ApplicationId == Application.CFATour)
			    return new CfaToursContext(connectionString);

		    return new CfaResContext(connectionString);
        }

        public static string GetS3Bucket()
        {
	        var config = GetConfig();
	        if (config.ApplicationId == Application.NPR)
		        return "s3.npr.org";

		    return "s3.chick-fil-a.com";
        }

	    public static string GetS3BucketRoot()
	    {
		    var config = GetConfig();
	        if (config.ApplicationId == Application.NPR)
                return "http://s3.npr.org.s3-website-us-east-1.amazonaws.com";

			return "http://s3.chick-fil-a.com.s3-website-us-east-1.amazonaws.com";
	    }
    }
}