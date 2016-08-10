
#region Imports

using cfacore.domain.user;
using System.Data.Linq.Mapping;

#endregion

namespace cfacore.shared.domain.user
{
    public class Zip:IZip
	{
		#region Constructors

		public Zip() {}

		public Zip(int zip) {
            Code = zip;
            PlusFour = null;
        }

		public Zip(int zip,int plusfour) {
            Code = zip;
            PlusFour = plusfour;
        }

        public Zip(string zip)
        {
            if (!string.IsNullOrEmpty(zip))
            {
                string[] parts = zip.Trim().Replace(" ","-").Split('-');
                if (parts.Length >= 1)
                    Code = ToInt( parts[0] );
                if (parts.Length >= 2)
                    PlusFour = ToInt( parts[1] );
            }
        }

		#region Private Helpers

		// Future - Fix with proper global extension library ( Eric in progress )

	    private static int ToInt( string value, int defaultValue = default( int ) )
		{
			if( value == null ) return defaultValue;
			int i;
			return int.TryParse( value, out i ) ? i : defaultValue ;
		}

		#endregion

		#endregion

		/* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
	    [Column]
	    public int? PlusFour { get; set; }

	    /* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
	    [Column]
	    public int Code { get; set; }

		#region ToString override

		public override string ToString()
        {
            return Code.ToString("D5")+(string.IsNullOrEmpty(PlusFour.ToString())?"":string.Format("-{0}",PlusFour));
		}

		#endregion

		#region Implicit conversion operators

		public static implicit operator int(Zip z) 
		{
			return z.Code;
		}

		public static implicit operator Zip(int i) 
		{
			return new Zip( i );
		}

		#endregion
	}
}
