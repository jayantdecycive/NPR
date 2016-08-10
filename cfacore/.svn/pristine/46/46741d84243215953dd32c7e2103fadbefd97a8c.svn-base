using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain.store;

namespace cfacore.domain.user
{
    
    public class CoreUser : User,ICoreUser
    {
        public CoreUser(string Id) {
            this._Id = Id;
        }
        public CoreUser() { 
        
        }

        public override string UriBase()
        {
            return "http://data.core.chick-fil-a.com/profile/user/";
        }

        /*
		* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
		private cfacore.domain.user.IApplicationCollection _Applications = null;

        public cfacore.domain.user.IApplicationCollection Applications
        {
            get{ return this._Applications;}

            set{ this._Applications = value;}
        }

        /*
		* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
		private cfacore.domain.card.ICardCollection _Cards = null;

        public cfacore.domain.card.ICardCollection Cards
        {
            get{ return this._Cards;}

            set{ this._Cards = value;}
        }

        /*
		* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
		private cfacore.domain.store.IStoreCollection _Stores = null;

        public cfacore.domain.store.IStoreCollection Stores
        {
            get{ return this._Stores;}

            set{ this._Stores = value;}
        }


        public shared.domain.user.IHome Home
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
