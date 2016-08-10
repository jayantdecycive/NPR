
#region Imports

using System;

#endregion

namespace cfacore.shared.modules.helpers
{
	public static class DateTimeOffsetExtensions
	{
		public static string ToDateStringWithDayOfWeek( this DateTimeOffset dt )
		{
			return dt == DateTimeOffset.MinValue ? string.Empty :
				dt.ToString("dddd, MMMM d, yyyy");
		}

		public static string ToDateString( this DateTimeOffset dt )
		{
			return dt == DateTimeOffset.MinValue ? string.Empty :
				dt.ToDateStringLong();
		}
		public static string ToDateStringLong( this DateTimeOffset dt )
		{
			return dt == DateTimeOffset.MinValue ? string.Empty :
				dt.ToString("MMMM d, yyyy");
		}

		public static string ToTimeString( this DateTimeOffset dt )
		{
			return dt == DateTimeOffset.MinValue ? string.Empty :
				dt.ToTimeStringLong();
		}
		public static string ToTimeStringLong( this DateTimeOffset dt )
		{
			return dt == DateTimeOffset.MinValue ? string.Empty :
				dt.ToString("h:mm tt");
		}

		public static string ToDateTimeString( this DateTimeOffset dt )
		{
			return dt == DateTimeOffset.MinValue ? string.Empty :
				dt.ToString("M/dd/yyyy h:mm tt");
		}

		public static string ToDateTimeStringLong( this DateTimeOffset dt )
		{
			return dt == DateTimeOffset.MinValue ? string.Empty :
				dt.ToString("MMMM d, yyyy h:mm tt");
		}

		public static string ToDateTimeStringExport( this DateTimeOffset dt )
		{
			return dt == DateTimeOffset.MinValue ? string.Empty :
				dt.ToString("M/dd/yyyy h:mm:ss tt");
		}
	}
}
