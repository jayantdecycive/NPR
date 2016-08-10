using cfacore.domain._base;
using cfacore.shared.domain.user;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace npr.domain._event.ticket
{
    public interface INPRTicket : cfares.domain._event.ITicket, IDomainObject
    {
        string FirstName { get; set; }

        string LastName { get; set; }

        string Email { get; set; }

        string EmailConfirm { get; set; }

        string Phone { get; set; }

        ContactPreference ContactPreference { get; set; }

        string GroupType { get; set; }

        int GroupSize { get; set; }

        IEnumerable<Name> GuestList { get; set; }

        string GuestListString { get;set; }

        IList<DateTime?> Dates { get; set; }
        string DatesString { get; set; }

        ICollection<string> ContactNotes { get; set; }

        string Notes { get; set; }

        bool IsSpecialtyTicket { get; set; }

		bool IsPaid { get; set; }
		decimal? TicketAmount { get; set; }
		decimal? TotalAmount { get; set; }
		bool? IsAuthSuccessful { get; set; }
		string AuthorizationResponse { get; set; }
		string CCNumber { get; set; }
		int? CCExpDateMonth { get; set; }
		int? CCExpDateYear { get; set; }
		string CCName { get; set; }
		string CCType { get; set; }

		string ListenToNprStation { get; set; }
		bool VisitorOfWebsite { get; set; }
		string Age { get; set; }
		string Race { get; set; }
		string TopicsOfInterest { get; set; }

		string TopicsOfInterest1 { get; set; }
		string TopicsOfInterest2 { get; set; }
		string TopicsOfInterest3 { get; set; }
    }

    public enum ContactPreference { Email, Phone }
}
