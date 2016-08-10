using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cfacore.domain.application
{
    class Nonce
    {
        /*
		* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
		private string _Context = null;

        public string Context
        {
            get{ return this._Context;}

            set{ this._Context = value;}
        }

        /*
		* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
		private string _Code = null;

        public string Code
        {
            get{ return this._Code;}

            set{ this._Code = value;}
        }

        /*
		* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
		private System.DateTime _NonceTimeStamp = DateTime.Now;

        public System.DateTime NonceTimeStamp
        {
            get{ return this._NonceTimeStamp;}

            set{ this._NonceTimeStamp = value;}
        }
    }
}
