using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain._base;

namespace cfacore.domain.application
{
    public class App:DomainObject,IApp
    {
        public override string UriBase()
        {
            return "http://data.core.chick-fil-a.com/auth/app/";
        }
               


        
		/// <summary>
		/// Generated code from the Core Model Builder Add-on.
		/// </summary>
		/// <created>
		/// Generated Wednesday, March 07, 2012
		/// </created>
		private string _Secret = null;

        public string Secret
        {
            get{ return this._Secret;}

            set{ this._Secret = value;}
        }

        
		/// <summary>
		/// Generated code from the Core Model Builder Add-on.
		/// </summary>
		/// <created>
		/// Generated Wednesday, March 07, 2012
		/// </created>
		private string _Identifier = null;

        public string Identifier
        {
            get{ return this._Identifier;}

            set{ this._Identifier = value;}
        }

        
		/// <summary>
		/// Generated code from the Core Model Builder Add-on.
		/// </summary>
		/// <created>
		/// Generated Wednesday, March 07, 2012
		/// </created>
		private System.Uri _ResponseEndPoint = null;

        public System.Uri ResponseEndPoint
        {
            get{ return this._ResponseEndPoint;}

            set{ this._ResponseEndPoint = value;}
        }

        
		/// <summary>
		/// Generated code from the Core Model Builder Add-on.
		/// </summary>
		/// <created>
		/// Generated Wednesday, March 07, 2012
		/// </created>
		private string _Name = null;

        public string Name
        {
            get{ return this._Name;}

            set{ this._Name = value;}
        }

        
		/// <summary>
		/// Generated code from the Core Model Builder Add-on.
		/// </summary>
		/// <created>
		/// Generated Wednesday, March 07, 2012
		/// </created>
		private string _DefaultScope = null;

        public string DefaultScope
        {
            get{ return this._DefaultScope;}

            set{ this._DefaultScope = value;}
        }

        
		/// <summary>
		/// Generated code from the Core Model Builder Add-on.
		/// </summary>
		/// <created>
		/// Generated Wednesday, March 07, 2012
		/// </created>
		private cfacore.shared.domain.media.IMedia _IconId = null;

        public cfacore.shared.domain.media.IMedia IconId
        {
            get{ return this._IconId;}

            set{ this._IconId = value;}
        }

        public override string ToChecksum()
        {
            return _Identifier + _Name;
        }
    }
}
