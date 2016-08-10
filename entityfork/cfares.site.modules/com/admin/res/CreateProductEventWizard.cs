using System.Linq;
using cfacore.shared.modules.com.admin;
using cfares.domain._event;
using System;
using System.Collections.Generic;
using cfares.domain._event.resevent.store;

namespace cfares.site.modules.com.admin.res
{
    //cfares.site.modules.com.admin.res.CreateProductEventWizard
    public class CreateProductEventWizard:Wizard<IResEvent>
    {
        public CreateProductEventWizard(IResEvent _event, string id) : base(_event, id) { }

        public override IWizard<IResEvent> Prime(IResEvent m)
        {
            GiveawayEvent model = m as GiveawayEvent;

            string root = "/Admin/Event/";
            string q = "";
            string id = model.Id() ?? "";
            ApplyComplete("admin.template", model.Template != null && !string.IsNullOrEmpty(model.TemplateId)).WithUri(root + "Template/" + id + q);
            ApplyComplete("admin.stores", model.ParticipatingStoresList != null && model.ParticipatingStoresList.Any()).WithUri(root + "Stores/" + id + q);
            ApplyComplete("admin.name", model.Status != ResEventStatus.Temp).WithUri(root + "Name/" + id + q);
            //ApplyComplete("admin.times", model.OccurrencesList != null && model.OccurrencesList.Count > 0).WithUri(root + "Times/" + id + q);
            ApplyComplete("admin.times", model.AvailableSlots != null && model.AvailableSlots.Count() > 1).WithUri(root + "Times/" + id + q);

            ApplyComplete("admin.food", model.AllowedProducts!=null&&model.AllowedProducts.Any()).WithUri(root + "Food/" + id + q);
            ApplyComplete("admin.summary", model.Status == ResEventStatus.Live).WithUri(root + "Summary/" + id + q);
            return this;
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

        public override List<WizardStep> GetEmptySteps()
        {
            return new List<WizardStep>
	        {
		        new WizardStep {Name = "Event Template", Key = "admin.template"},
                new WizardStep {Name = "Select Stores", Key = "admin.stores"},
                new WizardStep {Name = "Event Details", Key = "admin.name"},
		        new WizardStep {Name = "Slot Dashboard", Key = "admin.times"},
		        new WizardStep {Name = "Menu Items", Key = "admin.food"},
                new WizardStep {Name = "Summary", Key = "admin.summary"}
	        };
        }
    }
}
