
#region Imports

using cfacore.shared.modules.com.admin;
using cfares.domain._event;
using System;
using System.Linq;
using System.Collections.Generic;

#endregion

namespace cfares.site.modules.com.admin.cfatours
{
    public class CreateStoryEventWizard : Wizard<IResEvent>
    {
        public CreateStoryEventWizard(IResEvent _event, string id) : base(_event, id) { }
		
        public override IWizard<IResEvent> Prime(IResEvent model)
        {
            const string root = "/Admin/Event/";
            const string q = "";
            string id = model.Id() ?? "";

            ApplyComplete( "admin.start", model.Status != ResEventStatus.Temp )
				.WithUri( root + "Create-Dash/" + id + q );
            
			ApplyComplete( "admin.times", model.Status != ResEventStatus.Temp 
				&& model.AvailableSlots != null && model.AvailableSlots.Any() )
				.WithUri( root + "Times/" + id + q );
            
			ApplyComplete( "admin.summary", model.Status == ResEventStatus.Live
				|| model.Status == ResEventStatus.Hidden )
                .WithUri(root + "Tour-Summary/" + id + q);

			return this;
        }

		public override List<WizardStep> GetEmptySteps()
        {
            return new List<WizardStep>
	        {
		        new WizardStep {Name = "Event Details", Key = "admin.start"},
		        new WizardStep {Name = "Slot Dashboard", Key = "admin.times"},
		        new WizardStep {Name = "Tour Summary", Key = "admin.summary"}
	        };
        }

        public override WizardStep GetPreviousStep(string key)
        {
            throw new NotImplementedException();
        }

        public override WizardStep GetStep(string key)
        {
            throw new NotImplementedException();
        }

        public override WizardStep GetNextStep(string key)
        {
            throw new NotImplementedException();
        }
    }
}
