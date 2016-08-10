
using System;
using System.Collections.Generic;
using System.Linq;

namespace cfacore.shared.modules.helpers
{
	public static class StringHelpers
	{
		public static string ToStringSafe( this object value )
		{
			if( value == null ) return string.Empty;
			if( value is string && 
				string.IsNullOrWhiteSpace( value.ToString() ) ) return string.Empty;
			return value.ToString();
		}

		public static DateTimeOffset ToDateTimeOffset( this string value )
		{
			if( string.IsNullOrWhiteSpace( value ) ) return default( DateTimeOffset );
			DateTimeOffset offset;
			if( ! DateTimeOffset.TryParse( value, out offset ) ) return default( DateTimeOffset );
			return offset.Year == DateTimeOffset.MinValue.Year ? default( DateTimeOffset ) : offset;
		}

		public static IList<int> ToIntList( this string value )
		{
			if( ! value.IsValid() ) return new List<int>();
			string[] l = value.Split( new[] { ',' }, StringSplitOptions.RemoveEmptyEntries );
			return l.Select( s => s.ToInt() ).ToList();
		}

		public static int ToInt( this string value )
		{
			return ToInt( value, default( int ) );
		}

		public static int ToInt( this string value, int defaultValue )
		{
			if( value == null ) return defaultValue;
			int i;
			return int.TryParse( value, out i ) ? i : defaultValue ;
		}

		public static int? ToNullableInt( this string value )
		{
			return ToNullableInt( value, default( int? ) );
		}

		public static int? ToNullableInt( this string value, int? defaultValue )
		{
			if( value == null ) return null;
			int i;
			return ! int.TryParse( value, out i ) ? 
				defaultValue : ( i == 0 || i == -1 ? (int?) null : i );
		}

		public static bool ToBoolean( this object value )
		{
			return ToBoolean( value.ToStringSafe() );
		}

		public static bool ToBoolean( this string value )
		{
			if( string.IsNullOrWhiteSpace( value ) ) return false;
			if( value.Equals( "true,false", StringComparison.OrdinalIgnoreCase ) ) 
				return true; // special case for EditorFor's

			bool v;
			return bool.TryParse( value, out v ) && v;
		}

		public static bool IsValid( this string value )
		{
			return ! string.IsNullOrWhiteSpace( value );
		}

		static readonly string[] ones = new[] { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };
		static readonly string[] teens = new[] { "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
		static readonly string[] tens = new[] { "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
		static readonly string[] thousandsGroups = { "", " Thousand", " Million", " Billion" };

		private static string FriendlyInteger(this int n, string leftDigits, int thousands)
		{
			if (n == 0)
			{
				return leftDigits;
			}
			string friendlyInt = leftDigits;
			if (friendlyInt.Length > 0)
			{
				friendlyInt += " ";
			}

			if (n < 10)
			{
				friendlyInt += ones[n];
			}
			else if (n < 20)
			{
				friendlyInt += teens[n - 10];
			}
			else if (n < 100)
			{
				friendlyInt += FriendlyInteger(n % 10, tens[n / 10 - 2], 0);
			}
			else if (n < 1000)
			{
				friendlyInt += FriendlyInteger(n % 100, (ones[n / 100] + " Hundred"), 0);
			}
			else
			{
				friendlyInt += FriendlyInteger(n % 1000, FriendlyInteger(n / 1000, "", thousands+1), 0);
			}

			return friendlyInt + thousandsGroups[thousands];
		}

		public static string ToWrittenNumber(this int n)
		{
			if (n == 0)
			{
				return "Zero";
			}
			if (n < 0)
			{
				return "Negative " + ToWrittenNumber(-n);
			}

			return FriendlyInteger(n, "", 0);
		}
	}
}
