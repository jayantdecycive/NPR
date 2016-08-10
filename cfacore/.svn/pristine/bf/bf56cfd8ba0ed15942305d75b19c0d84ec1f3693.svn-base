using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data.Linq.Mapping;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;

namespace cfacore.shared.domain.user
{
    [DataContract]
    
    public class Phone:IPhone
    {
        public Phone(string phone) {
            if (!string.IsNullOrEmpty(phone))
            {

                phone = Regex.Replace(phone, @"[^0-9]*", "");
                if (phone.Length >= 10)
                {
                    string areaCode = phone.Substring(0, 3);
                    string number = phone.Substring(3, 7);
                    AreaCode = int.Parse(areaCode);
                    Number = int.Parse(number);
                }
                else if (phone.Length >= 7)
                {
                    string number = phone.Substring(0, 7);
                    Number = int.Parse(number);
                }
            }
        }
        public Phone() { 
        }

        /*
		* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
		private int _Number = 0;
        [Column]
        [DataMember]
        [Required(ErrorMessage = "Phone number is required.")]
        public int Number
        {
            get{ return this._Number;}

            set{ this._Number = value;}
        }

        /*
		* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */        
        public long Full
        {
            get { return long.Parse(this.AreaCode+""+this.Number); }
            set {
                if (value.ToString().Length >= 10)
                {
                    this.AreaCode = int.Parse(value.ToString().Substring(0, 3));
                    this.Number = int.Parse(value.ToString().Substring(3, 4));
                }
            }
        }

        /*
		* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
        private int _AreaCode = 0;
        [Column]
        [DataMember]
        [Required(ErrorMessage = "Area code is required.")]
        public int AreaCode
        {
            get { return this._AreaCode; }

            set { this._AreaCode = value; }
        }

        /*
		* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
        private string _Extension = null;
        [Column]
        [DataMember]
        public string Extension
        {
            get{ return this._Extension;}

            set{ this._Extension = value;}
        }

        /*
		* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
        private cfacore.shared.domain.user.PhoneType _Type = cfacore.shared.domain.user.PhoneType.Mobile;
        [Column(DbType = "NVarChar(10) NOT NULL")]
        public string Type { get; set; }

        public cfacore.shared.domain.user.PhoneType PhoneType()
        {
            return _Type;
        }
        


        /*
		* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
		private string _Carrier = null;
        [DataMember]
        public string Carrier
        {
            get{ return this._Carrier;}

            set{ this._Carrier = value;}
        }

        public override string ToString()
        {
            if (AreaCode == 0 && Number == 0)
                return "0";
            return string.Format("{0}{1}",AreaCode,Number);
        }

        public string ToFormattedString()
        {
	        if (Full.ToString().Length == 10)
                return string.Format("({0}) {1}-{2}", AreaCode, Number.ToString().Substring(0, 3), Number.ToString().Substring(3, 4));
	
			if( Number <= 0 ) return string.Empty;
			return Number.ToString();
        }
    }
}
