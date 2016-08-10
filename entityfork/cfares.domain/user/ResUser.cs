
#region Imports

using System;
using System.Collections.Generic;
using System.Linq;
using cfacore.domain.user;
using cfacore.shared.domain.user;
using cfacore.shared.domain.attributes;
using cfares.domain._event;
using cfares.domain._event._ticket;
using System.Data.Linq.Mapping;
using System.ComponentModel.DataAnnotations;
using cfares.domain.store;

#endregion

namespace cfares.domain.user
{
    [SyncUrl("/api/ResUser")]
    [Serializable]
    public class ResUser : User, IResUser
    {
	    public ResUser() {}
        public ResUser(string id) : base(id)
        {

        }

        public override string ToChecksum()
        {
            return base.ToChecksum()+"-Role"+OperationRole;
        }

        public IList<IOccurrence> GetAvailablePreferredOccurrences(IReservationType reservationType)
        {
            var prefferedLocations = this.UserPreferredLocationSubscriptions.Select(x => x.StoreId).ToList();
            if (!prefferedLocations.Any())
                return null;
            return reservationType.ActiveEvents.SelectMany(x => x.ActiveOccurrences)
                .Where(x => prefferedLocations.Any(o => o == x.StoreId)).Cast<IOccurrence>().ToList();
        }

        public IList<IOccurrence> GetAvailablePreferredOccurrences(IResEvent resEvent)
        {
            var prefferedLocations = this.UserPreferredLocationSubscriptions.Select(x=>x.StoreId).ToList();
            if (!prefferedLocations.Any())
                return null;
            return resEvent.ActiveOccurrences.Cast<IOccurrence>().Where(x => prefferedLocations.Any(o => o == x.StoreId)).ToList();
        }

        public static ResUser System
        {
            get
            {
                ResUser systemUser = new ResUser();
                systemUser.Id("0");
                systemUser.Name = new Name("System");

                return systemUser;
            }
        }

        public TicketCollection Tickets
        {
            get;
            set;
        }

        public bool EmailInsiders { get; set; }
        
	    private ICollection<LocationSubscription> _userPreferredLocationSubscriptions;
        public virtual ICollection<LocationSubscription> UserPreferredLocationSubscriptions
        {
	        get {
		        return _userPreferredLocationSubscriptions ??
		               (_userPreferredLocationSubscriptions = new List<LocationSubscription>());
	        }
	        set { _userPreferredLocationSubscriptions = value; }
        }

	    public virtual IEnumerable<ResStore> UserPreferredLocations
        {
            get { return UserPreferredLocationSubscriptions.Select(x => x.Store); }
        }

        [Column]
        [Display(Name = "Operation Role")]
        [DataType("Enum/_UserOperationRole")]
        public UserOperationRole OperationRole
        {
            get;
            set;
        }

        /*
         * Mdrake: these are alread in the model (email insiders)
         * */
        /*
        [Column]
        [Display(Name = "Join E-mail Newsletters")]
	    public bool EmailJoinNewsletters { get; set; }

        [Column]
        [Display(Name = "Join E-mail Promotions")]
		public bool EmailJoinPromotions { get; set; }
        */

        /*Mdrake: belongs in membership
         * */
        /*
	    public string Password { get; set; }
	    public string PasswordConfirmation { get; set; }
        */

        /*
         * Not really stored, legal requirement
         * */
        /*
        [Column]
        [Display(Name = "I am 13 years of age or older")]
	    public bool OfAge { get; set; }
         * */

        /*mdrake:already have
         * */
        /*
        [Column]
	    public DateTime Birthday { get; set; }
        */

        public string BirthMonth
        {
            get
            {
                return BirthDay.ToString("MMMM");
            }
            set
            {
                BirthDay = DateTime.Parse(string.Format("{0}/{1}/{2}", value, BirthDate,BirthDay.Year));
            }
        }

        public int BirthDate
        {
            get
            {
                return BirthDay.Day;
            }
            set
            {
                BirthDay = DateTime.Parse(string.Format("{0}/{1}/{2}", BirthMonth, value,BirthDay.Year));
            }
        }

        public string LastActivityString {
            get { return LastActivity.ToString(); }
            set { LastActivity = DateTime.Parse(value); }
        }

        public static string GenerateEmail()
        {
            return "generated-"+Guid.NewGuid().ToString().Replace("-","")+"@generated.chick-fil-a.com";
        }


    }
}
