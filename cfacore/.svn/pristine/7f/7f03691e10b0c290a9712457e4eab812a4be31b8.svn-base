using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cfacore.shared.domain._base
{
    public static class DateExtensions
    {
        public static string Suffix(this DateTime dt){
            switch (dt.Day)
				{
					case 1:
					case 21:
					case 31:
						return "st";
						
					case 2:
					case 22:
						return "nd";						
					case 3:
					case 23:
						return "rd";
						
					default:
						return "th";						
				}
        }

        public static string USEnglishDateWithSuffix(this DateTime dt) {
            
            string startSuffix = dt.Suffix();

            return string.Format("{0:dddd, MMMM d}{1}, {0:yyyy}", dt, startSuffix);
        }
    }
}
