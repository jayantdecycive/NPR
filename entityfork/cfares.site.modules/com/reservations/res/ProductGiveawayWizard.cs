using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cfacore.shared.modules.com.admin;
using cfares.domain._event;
using cfares.domain._event.menu;
using cfares.domain._event._ticket;
using cfares.domain.store;
using cfares.repository.menuitem;
using cfares.repository.store;
using cfares.site.modules.com.application;

namespace cfares.site.modules.com.reservations.res
{
    public class ProductGiveawayWizard : ReservationWizard<FoodTicket>, IReservationWizard<FoodTicket>
    {

        public ProductGiveawayWizard(FoodTicket model, string id)
            : base(model, id)
        {
        }

        public override IWizard<FoodTicket> Prime(FoodTicket model)
        {
            var baseWizard = base.Prime(model);
            string id = model.Id() ?? "";

            ApplyComplete("Reservation.Food", model.MenuItemId != null).WithRoute("Food", id, Q);
            ApplyComplete("Reservation.Calendar", model.SlotId != null).WithRoute("Calendar", id, Q);

            return baseWizard;
        }

        public IWizard<FoodTicket> Prime(MenuItem model)
        {
            if (model != null)
                this.FoodId = model.DomId;

            ApplyComplete(NextStep, false).WithQuery(Q);
            return this;
        }

        public string FoodId
        {
            get
            {
                if (!Qs.ContainsKey("food"))
                    return null;
                return (Qs["food"]);
            }
            set { Qs["food"] = value; }
        }

        

        public MenuItem GetFood()
        {
            if (this.FoodId != null)
            {
                var foodRepo = new MenuItemRepository(ReservationConfig.GetContext());
                return foodRepo.FindOrRemember(this.FoodId);
            }
            else if (Model != null && Model.GetMenuItem() != null)
            {
                return Model.GetMenuItem();
            }
            return null;
        }

        public override List<WizardStep> GetEmptySteps()
        {
            return new List<WizardStep>
	        {
		        new WizardStep {Name = "SearchByLocation", Key = "Reservation.SearchByLocation"},
		        new WizardStep {Name = "Calendar", Key = "Reservation.Calendar"},
                new WizardStep {Name = "Food", Key = "Reservation.Food"},
                new WizardStep {Name = "Reservation", Key = "Reservation.Reservation"},
		        new WizardStep {Name = "Review", Key = "Reservation.Review"},
		        new WizardStep {Name = "Success", Key = "Reservation.Success"},
	        };
        }

        public override void ApplyContext(System.Web.Routing.RequestContext context)
        {
            ApplyComplete("Reservation.Food", !string.IsNullOrEmpty(FoodId)).WithQuery(Q);
            ApplyComplete("Reservation.Calendar", SlotId!=null).WithQuery(Q);
            base.ApplyContext(context);
        }
        

    }
}
