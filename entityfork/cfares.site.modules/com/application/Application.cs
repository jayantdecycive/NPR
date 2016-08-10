
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace cfares.site.modules.com.application
{
	public sealed class Application {

		public static readonly Application CFA = new Application( "res" );
		public static readonly Application NPR = new Application( "npr" );
        public static readonly Application CFATour = new Application("cfatours");        

		#region String Enumeration Support

		private readonly string _key;
		// RE: http://stackoverflow.com/questions/424366/c-sharp-string-enums

		private Application( string key ){
			_key = key;
		}

		public override string ToString() {
			return _key;
		}

		#region Implicit conversion operators

		public static implicit operator string( Application application )
        {
		    return application._key;
        }

		private static FieldInfo[] _fieldInfoCache;
		private static IEnumerable<FieldInfo> FieldInfoCache
		{
			get { return _fieldInfoCache ?? (_fieldInfoCache = typeof (Application).GetFields()); }
		}

		public static explicit operator Application( string application )
		{
			return FieldInfoCache.Select( fi => (Application) fi.GetValue(null) )
				.FirstOrDefault( a => a._key == application );
		}

		#endregion

		#endregion

        public static string GetApplicationLayout(){
            switch(cfares.site.modules.com.application.AppContext.Current.Configuration.ApplicationId){
                case "res":
                    return "~/Areas/res/Views/Shared/_Layout.cshtml";
                case "npr":
                    return "~/Areas/npr/Views/Shared/_Layout.cshtml";
                case "cfatours":
                    return "~/Areas/cfatours/Views/Shared/_Layout.cshtml";
                default:
                    return "~/Views/Shared/_Layout.cshtml";
            }
        }
	}
}
