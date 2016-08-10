using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using cfacore.domain._base;
using cfacore.shared.domain.attributes;
using cfacore.shared.domain.user;
using System.ComponentModel.DataAnnotations;
using cfacore.shared.domain.common;
using System.Runtime.Serialization;

namespace cfacore.domain.user
{
    [Serializable]
    [DataContract]
    public class User : DomainObject, IUser, ILoadableAdminReference
    {
        public User(string id)
        {
            LastActivity = DateTime.Now;
            Username = null;
            AuthorityUID = null;
            Authority = null;
            HomePhone = null;
            Email = null;
            this._Id = id;
        }

        public User()
        {
            LastActivity = DateTime.Now;
            Username = null;
            AuthorityUID = null;
            Authority = null;
            HomePhone = null;
            Email = null;
        }


        public override string UriBase()
        {
            return "http://data.chick-fil-a.com/profile/user/";
        }



        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>

        [System.Data.Linq.Mapping.Column]
        [DataMember]
        [DataType("jqui/_DatePicker")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }


        [System.Data.Linq.Mapping.Column, Display(Name = "Last Activity"), DataMember, DataType("jqui/_DatePicker")]
        public DateTime LastActivity { get; set; }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>

        [System.Data.Linq.Mapping.Column]
        [IgnoreDataMember]
        [Display(Name = "Account Status")]
        [DataType("Enum/_UserAccountStatus")]
        public UserAccountStatus AccountStatus
        {
            get;
            set;
        }

        [Display(Name = "Birthday")]
        public DateTime BirthDay { get; set; }

        [Display(Name = "Birthday")]
        public string BirthDayString
        {
            get { return BirthDay.ToShortDateString(); }
            set
            {
                BirthDay = DateTime.Parse(value);
                if (BirthDay.Year < 1900)
                {
                    BirthDay = new DateTime(1900, BirthDay.Month, BirthDay.Day);
                }
            }
        }

        [Required, DataType(DataType.EmailAddress), RegularExpression(@"[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?", ErrorMessage = "Please enter a valid email address"), Display(Name = "Email Address"), DataMember]
        public string Email { get; set; }


        [System.Data.Linq.Mapping.Column, DataMember, Display(Name = "Username")]
        public string Username { get; set; }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>
        [System.Data.Linq.Mapping.Column]
        [DataMember]
        public string UID
        {
            get;
            set;
        }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>
        [System.Data.Linq.Mapping.Column]
        [DataMember]
        public string DN { get; set; }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>
        private Name _Name = null;

        [IgnoreDataMember]
        public Name Name
        {
            get
            {
                if (this._Name == null)
                    this._Name = new Name();
                return this._Name;
            }

            set { this._Name = value; }
        }

        [System.Data.Linq.Mapping.Column]
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required.")]
        [DataType("Text")]
        [DataMember]
        public string NameString
        {
            get
            {
                if (this._Name == null)
                    this._Name = new Name();
                return this._Name.ToString();
            }
            set { this._Name = new Name(value); }
        }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string FirstName
        {
            get { return Name.First; }
            set { Name.First = value; }
        }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string LastName
        {
            get { return Name.Last; }
            set { Name.Last = value; }
        }

        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>
        [System.Data.Linq.Mapping.Association(Name = "FK_Address", ThisKey = "AddressId", OtherKey = "HomeId")]
        [DataType(DataType.MultilineText)]
        [UIHint("ToString")]
        [IgnoreDataMember]
        public virtual Address Address
        {
            get;
            set;
        }


        [System.Data.Linq.Mapping.Column, Display(Name = "Home Phone"), DataType(DataType.Text), IgnoreDataMember]
        public Phone HomePhone { get; set; }

        [DataMember]
        [Display(Name = "Home Phone")]
        public string HomePhoneString
        {
            get
            {
                if (this.HomePhone == null)
                    return null;
                return this.HomePhone.ToFormattedString();
            }

            set { this.HomePhone = new Phone(value); }
        }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>
        private Phone _MobilePhone = null;
        [System.Data.Linq.Mapping.Column]
        [Display(Name = "Mobile Phone")]
        [DataType(DataType.Text)]
        [IgnoreDataMember]
        public Phone MobilePhone
        {
            get { return this._MobilePhone; }

            set { this._MobilePhone = value; }
        }
        [DataMember]
        [Display(Name = "Mobile Phone")]
        public string MobilePhoneString
        {
            get
            {
                if (this.MobilePhone == null)
                    return null;
                return this.MobilePhone.ToFormattedString();
            }

            set { this.MobilePhone = new Phone(value); }
        }


        [System.Data.Linq.Mapping.Column, DataMember]
        public string Authority { get; set; }

        [System.Data.Linq.Mapping.Column, DataMember]
        public string AuthorityUID { get; set; }

        [Key]
        [Column]
        [DataMember]
        public int UserId
        {
            get { return IntId(); }
            set { Id(value); }
        }

        private string _autoDetectedLocationNumber;
        public string AutoDetectedLocationNumber
        {
            get
            {
                if (_autoDetectedLocationNumber == null)
                {
                    _autoDetectedLocationNumber = string.Empty;
                    if ( ! string.IsNullOrWhiteSpace( Username ) && Username.Contains("@chick-fil-a.com") )
                    {
                        string token = Username.Replace("@chick-fil-a.com", string.Empty).Trim();
                        int i; if (int.TryParse(token, out i))
                            _autoDetectedLocationNumber = i.ToString("D5");
                    }
                    if ( ! string.IsNullOrWhiteSpace( Email ) && _autoDetectedLocationNumber == string.Empty && Email.Contains("@chick-fil-a.com"))
                    {
                        string token = Email.Replace("@chick-fil-a.com", string.Empty).Trim();
                        int i; if (int.TryParse(token, out i))
                            _autoDetectedLocationNumber = i.ToString("D5");
                    }
                }
                return _autoDetectedLocationNumber;
            }
        }

        public bool isStoreNumberLogin
        {
            get
            {
                return !String.IsNullOrEmpty(this.AutoDetectedLocationNumber);
            }
        }

        public override string ToString()
        {
            if (Name != null)
                return string.Format("{1} from {0}", Authority, Name.ToString());

            return string.Format("User Id {0}", UserId);
        }

        public string AnchorLabel()
        {
            string name = "View User";
            if (IsBound())
                name = Name.ToString();
            return name;
        }
        public string AnchorHref()
        {
            return string.Format("/Admin/User/Details/{0}", UserId);
        }

        public string AnchorAbsoluteHref()
        {
            return "http://tours.chick-fil-a.com" + AnchorHref();
        }

        public string ClientModel()
        {
            return "DomainModel.ResUser";
        }

        public string ClientLabel()
        {
            return "Name";
        }

        public override string ToChecksum()
        {
            return AuthorityUID + "-" + Name + "-" + Email;
        }

        public string HexedEmail()
        {
            return ConvertToHex(Email);
        }

        public void HexedEmail(string Email)
        {
            this.Email = HexString2Ascii(Email);

        }

        public string ZipCode
        {
            get
            {
                if (this.AddressId == null && this.Address == null)
                    return null;
                return Address.ZipString;
            }
            set
            {
                if (this.AddressId == null && this.Address == null)
                    this.Address = new Address();
                this.Address.ZipString = value;
            }
        }

        public string HexedId()
        {
            return ConvertToHex(Id());
        }

        public void HexedId(string Id)
        {

            this.Id(HexString2Ascii(Id));
        }

        private string HexString2Ascii(string hexString)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= hexString.Length - 2; i += 2)
            {
                sb.Append(Convert.ToString(Convert.ToChar(Int32.Parse(hexString.Substring(i, 2), System.Globalization.NumberStyles.HexNumber))));
            }
            return sb.ToString();
        }

        private string ConvertToHex(string asciiString)
        {
            string hex = "";
            foreach (char c in asciiString)
            {
                int tmp = c;
                hex += String.Format("{0:x2}", (uint)System.Convert.ToUInt32(tmp.ToString()));
            }
            return hex;
        }

        public int? AddressId { get; set; }

        public new string GetEntityType()
        {
            return "ResUser";
        }

        // Note - SH - Had to comment out 'Creation' in order to get the reg pg to work
        // .. After speaking with MD, seems the column should be deprecated, and since
        // .. no other code seemingly depends on this field in the solution, it's been
        // .. commented out for the time being - Feel free to reinstantiate if needed
        // .. but if so, ensure that the registration form work please - ty

        // Should be replaced with dynamic creation field below 
        // .. ( required 'ignore' in context OnModelCreating )

        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //public DateTime Creation { get; set; }

        public DateTime Creation
        {
            get { return CreatedDate; }
            set
            {
                CreatedDate = value;
            }
        }

        [Column]
        [DataMember]
        public Gender Gender{
            get;
            set;
        }
    }

    public class TokenAndRecipient
    {
        public Token Token { get; set; }
        public User Recipient { get; set; }
    }

    
}
