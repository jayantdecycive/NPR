using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using cfacore.domain._base;
using cfacore.domain.user;

namespace cfacore.domain.card
{
    public class Card : DomainObject, ICard
    {
        #region Constructors

        public Card(string id)
        {
            this._Id = id;
        }
        public Card()
        {
        }

        #endregion

        #region Properties

        private int _userID;
        public int UserID
        {
            get { return this._userID; }
            set { this._userID = value; }
        }

        private string _number;
        [Required(ErrorMessage = "Card number is required.")]
        [StringLength(14, MinimumLength = 14)]
        public string Number
        {
            get { return this._number; }
            set { this._number = value; }
        }

        private string _securityNumber;
        [Required(ErrorMessage = "Security number is required.")]
        [StringLength(5, MinimumLength = 5)]
        public string SecurityNumber
        {
            get { return this._securityNumber; }
            set { this._securityNumber = value; }
        }

        private IUser _owner;
        public IUser Owner
        {
            get { return this._owner; }
            set { this._owner = value; }
        }

        private IBonusPlanStandingCollection _bonuses;
        public IBonusPlanStandingCollection Bonuses
        {
            get { return this._bonuses; }
            set { this._bonuses = value; }
        }

        #endregion

        public override string UriBase()
        {
            return "http://data.chick-fil-a.com/profile/user/card/";
        }

        public override string ToChecksum()
        {
            return _Id + _number;
        }
    }
}
