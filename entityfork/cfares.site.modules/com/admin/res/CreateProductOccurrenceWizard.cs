using cfacore.shared.modules.com.admin;
using cfares.domain._event;
using System;
using System.Linq;
using System.Collections.Generic;
using cfares.domain._event.occ;
using cfares.domain._event.resevent.store;

namespace cfares.site.modules.com.admin.res
{
    public class CreateProductOccurrenceWizard : Wizard<IOccurrence>
    {
        public CreateProductOccurrenceWizard(IOccurrence occurrence, string id)
            : base(occurrence, id)
        {        
        }

        public override IWizard<IOccurrence> Prime(IOccurrence model)
        {
            string root = "/Admin/Occurrence/";
            string q = "";
            string id = model.Id() ?? "";

	        GiveawayOccurrence o = model as GiveawayOccurrence;
            //GiveawayEvent e = model.ResEvent as GiveawayEvent;

            ApplyComplete("admin.capacity", true).WithUri(root + "Capacity/" + id + q);
			ApplyComplete("admin.food", o != null && o.ItemsAvailableWithoutDefaults.Count > 0).WithUri(root + "Food/" + id + q);
			ApplyComplete("admin.summary", model.Status == OccurrenceStatus.Live && o != null && o.ItemsAvailableWithoutDefaults.Count > 0 ).WithUri(root + "Summary/" + id + q);

            /*ApplyComplete("admin.template",model.Template != null && !string.IsNullOrEmpty(model.TemplateId)).WithUri(root+"Template/"+id+q);
            ApplyComplete("admin.stores", model.ParticipatingStoresList != null && model.ParticipatingStoresList.Count() > 0).WithUri(root + "Stores/" + id + q);
            ApplyComplete("admin.name", model.Status!=ResEventStatus.Temp).WithUri(root + "Name/" + id + q);
            
            ApplyComplete("admin.times", true).WithUri(root + "Times/" + id + q);
            
//            ApplyComplete("admin.food", model);
            ApplyComplete("admin.summary", model.Status == ResEventStatus.Live).WithUri(root + "Summary/" + id + q);*/
            
			return this;
        }

        public override List<WizardStep> GetEmptySteps()
        {
            List<WizardStep> _Steps = new List<WizardStep>();
            
            _Steps.Add(new WizardStep { Name = "Capacity", Key = "admin.capacity" });
//            _Steps.Add(new WizardStep { Name = "Details", Key = "admin.food" });
            //steps.Add(new WizardStep {Name = "Aloha",Key="admin.aloha"});
            _Steps.Add(new WizardStep { Name = "Products", Key = "admin.food" });
            _Steps.Add(new WizardStep { Name = "Confirmation", Key = "admin.summary" });

            return _Steps;
                     
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
