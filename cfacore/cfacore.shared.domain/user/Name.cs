using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain;
using System.Text.RegularExpressions;
using System.Data.Linq.Mapping;
using System.ComponentModel.DataAnnotations;

namespace cfacore.shared.domain.user
{
    [Serializable]
    public class Name:IName
    {
        public Name(string first, string middle, string last) {
            this.First = first;
            this.Last = last;
            this.Middle = middle;
        }
        public Name(string full)
        {
            this.Parse(full);            
        }

        public Name() { 
        
        }

        public void Parse(string full) {
            if (full == null)
                return;
            full = full.Trim();
            int pLocation = full.IndexOf('(');
            if (pLocation != -1)
            {
                Match nicknameMatch = Regex.Match(full, @"\(([^)]*)\)$");
                if (nicknameMatch.Success)
                {                    
                    this.NickName = nicknameMatch.Groups[1].Value;
                    full = full.Remove(pLocation);
                }
            }


            if (full.IndexOf(',') != -1) {
                string[] namePartsReversed = full.Split(',');
                this.Last = namePartsReversed[0];
                full = namePartsReversed[1] + this.Last;
            }

            string[] nameParts = full.Split(' ');
            switch (nameParts.Length)
            {
                case 0:
                    break;
                case 1:
                    this.First = nameParts[0];
                    break;
                case 2:
                    this.First = nameParts[0];
                    this.Last = nameParts[1];
                    break;
                case 3:
                    this.First = nameParts[0];
                    this.Middle = nameParts[1];
                    this.Last = nameParts[2];
                    break;
                case 4:
                default:
                    this.Surname = nameParts[0];
                    this.First = nameParts[1];
                    this.Middle = nameParts[2];
                    this.Last = nameParts[3];
                    break;
            }
            
        }
        

        public string Full {
            get {
                string[] names = new string[] { Surname,First,Middle,Last};
                List<string> validNames = new List<string>();
                foreach(string name in names){
                    if(!string.IsNullOrEmpty(name))
                        validNames.Add(name);
                }
                return string.Join(" ",validNames.ToArray());
            }
        }

        /*
		* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
		private string _First = null;
        [Column]
        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "First name")]
        [StringLength(20)]
        public string First
        {
            get{ return this._First;}

            set{ this._First = value;}
        }

        /*
		* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
        private string _NickName = null;
        [Column]
        public string NickName
        {
            get { return this._NickName; }

            set { this._NickName = value; }
        }

        /*
		* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
		private string _Last = null;
        [Column]
        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Last name")]
        [StringLength(20)]
        public string Last
        {
            get{ return this._Last;}

            set{ this._Last = value;}
        }

        /*
		* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
		private string _Middle = null;
        [Column]
        public string Middle
        {
            get{ return this._Middle;}

            set{ this._Middle = value;}
        }

        /*
		* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
		private string _Surname = null;
        [Column]
        public string Surname
        {
            get{ return this._Surname;}

            set{ this._Surname = value;}
        }

        public override string ToString()
        {
            return this.Full + (!string.IsNullOrEmpty(NickName)?" ("+NickName+")":"");
        }
    }
}
