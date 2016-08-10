using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain._base;

namespace cfacore.domain.application
{
    public class AppAuthorization:DomainObject,IAppAuthorization
    {
        public override string UriBase()
        {
            return "http://data.core.chick-fil-a.com/auth/appauth/";
        }

        /*
		* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
		private System.DateTime _CreationDate = DateTime.Now;

        public System.DateTime CreationDate
        {
            get{ return this._CreationDate;}

            set{ this._CreationDate = value;}
        }

        /*
		* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
		private System.DateTime _ExpirationDate = DateTime.Now;

        public System.DateTime ExpirationDate
        {
            get{ return this._ExpirationDate;}

            set{ this._ExpirationDate = value;}
        }

        /*
		* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
		private cfacore.domain.application.IApp _App = null;

        public cfacore.domain.application.IApp App
        {
            get{ return this._App;}

            set{ this._App = value;}
        }

        /*
		* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
		private cfacore.domain.user.IUser _User = null;

        public cfacore.domain.user.IUser User
        {
            get{ return this._User;}

            set{ this._User = value;}
        }

        /*
		* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
		private string _Scope = null;

        public string Scope
        {
            get{ return this._Scope;}

            set{ this._Scope = value;}
        }

        public override string ToChecksum()
        {
            return _Id + _App.Identifier;
        }
    }
}
