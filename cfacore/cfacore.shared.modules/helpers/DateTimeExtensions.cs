
#region Imports

using System;
using System.Web;

#endregion

namespace cfacore.shared.modules.helpers
{
	public static class DateTimeExtensions
	{
		public static string ToUtcString( this DateTimeOffset value )
		{
			// Must be in HH ( no tt ) form for 'Date.parse'
			if( value.Year == DateTimeOffset.MinValue.Year ) return string.Empty;
			return value == DateTimeOffset.MinValue ? string.Empty : 
				value.ToString("MM/dd/yyyy HH:mm");

			// SH - Modifying from below to ( no zzz ) due to IE unable to parse via new Date()
			// .. Issue manifests itself only on the 
			//	value.ToString("MM/dd/yyyy HH:mm zzz");
		}

		public static string ToUtcTimeString( this DateTimeOffset value )
		{
			if( value.Year == DateTimeOffset.MinValue.Year ) return string.Empty;
			return value == DateTimeOffset.MinValue ? string.Empty : 
				value.ToString("HH:mm zzz");
		}

		public static DateTimeOffset ToUtcFromClientTimezoneOffset( this DateTimeOffset value )
		{
			if( HttpContext.Current == null ) return value;
			HttpRequest r = HttpContext.Current.Request;
			if( r[ "ClientTimezoneOffset" ] == null ) return value;
			TimeSpan offset = new TimeSpan( r[ "ClientTimezoneOffset" ].ToInt(), 0, 0 );
			return value.Add( offset );
		}

		public static string ToDateStringWithDayOfWeek( this DateTime? dt )
		{
			return ! dt.HasValue ? string.Empty : ToDateStringWithDayOfWeek( dt.Value );
		}
		public static string ToDateStringWithDayOfWeek( this DateTime dt )
		{
			return dt.ToString("dddd, MMMM d, yyyy");
		}

		public static string ToDateString( this DateTime? dt )
		{
			return ! dt.HasValue ? string.Empty : ToDateString( dt.Value );
		}
		public static string ToDateString( this DateTime dt )
		{
			return dt.ToDateStringLong();
		}
		public static string ToDateStringLong( this DateTime? dt )
		{
			return ! dt.HasValue ? string.Empty : ToDateStringLong( dt.Value );
		}
		public static string ToDateStringLong( this DateTime dt )
		{
			return dt.ToString("MMMM d, yyyy");
		}

		public static string ToTimeString( this DateTime? dt )
		{
			return ! dt.HasValue ? string.Empty : ToTimeString( dt.Value );
		}
		public static string ToTimeString( this DateTime dt )
		{
			return dt.ToTimeStringLong();
		}
		public static string ToTimeStringLong( this DateTime? dt )
		{
			return ! dt.HasValue ? string.Empty : ToTimeStringLong( dt.Value );
		}
		public static string ToTimeStringLong( this DateTime dt )
		{
			return dt.ToString("h:mm tt");
		}

		public static string ToDateTimeString( this DateTime? dt )
		{
			return ! dt.HasValue ? string.Empty : ToDateTimeString( dt.Value );
		}
		public static string ToDateTimeString( this DateTime dt )
		{
			return dt.ToDateTimeStringLong();
		}
		public static string ToDateTimeStringLong( this DateTime? dt )
		{
			return ! dt.HasValue ? string.Empty : ToDateTimeStringLong( dt.Value );
		}
		public static string ToDateTimeStringLong( this DateTime dt )
		{
			return dt.ToString("MMMM d, yyyy h:mm tt");
		}
	}
}
