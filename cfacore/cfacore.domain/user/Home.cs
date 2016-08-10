using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain._base;

namespace cfacore.shared.domain.user
{
    public class Home:DomainObject,IHome
    {
        /*
		* Generated code from the Core Model Builder Add-on
		Generated Monday, March 05, 2012 */
		private cfacore.shared.domain.user.IUserCollection _Users = null;

        public cfacore.shared.domain.user.IUserCollection Users
        {
            get{ return this._Users;}

            set{ this._Users = value;}
        }

        /*
		* Generated code from the Core Model Builder Add-on
		Generated Monday, March 05, 2012 */
		private cfacore.domain.user.IAddress _Address = null;

        public cfacore.domain.user.IAddress Address
        {
            get{ return this._Address;}

            set{ this._Address = value;}
        }

        /*
		* Generated code from the Core Model Builder Add-on
		Generated Monday, March 05, 2012 */
		private string _Label = null;

        public string Label
        {
            get{ return this._Label;}

            set{ this._Label = value;}
        }

        public override string UriBase()        {            return "http://data.chick-fil-a.com/profile/home/";        }

        public override string ToChecksum()
        {
            return _Id + _Label;
        }
    }
}
