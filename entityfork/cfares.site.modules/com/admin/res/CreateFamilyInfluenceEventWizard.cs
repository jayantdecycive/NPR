using cfacore.shared.modules.com.admin;
using cfares.domain._event;
using System;
using System.Linq;
using System.Collections.Generic;

namespace cfares.site.modules.com.admin.res
{
    public class CreateFamilyInfluenceEventWizard : Wizard<IResEvent>
    {
        public CreateFamilyInfluenceEventWizard(IResEvent _event, string id)
            : base(_event, id)
        {        
        }
		
        public override IWizard<IResEvent> Prime(IResEvent model)
        {
            string root = "/Admin/Event/";
            string q = "";
            string id = model.Id() ?? "";
            ApplyComplete("admin.template",model.Template != null && !string.IsNullOrEmpty(model.TemplateId)).WithUri(root+"Template/"+id+q);
            ApplyComplete("admin.stores", model.ParticipatingStoresList != null && model.ParticipatingStoresList.Any()).WithUri(root + "Stores/" + id + q);
            ApplyComplete("admin.name", model.Status!=ResEventStatus.Temp).WithUri(root + "Name/" + id + q);
            //ApplyComplete("admin.times", model.OccurrencesList != null && model.OccurrencesList.Count > 0).WithUri(root + "Times/" + id + q);
            ApplyComplete("admin.times", model.AvailableSlots!=null && model.AvailableSlots.Count()>1).WithUri(root + "Times/" + id + q);
            
//            ApplyComplete("admin.food", model);
            ApplyComplete("admin.summary", model.Status == ResEventStatus.Live).WithUri(root + "Summary/" + id + q);
            
			return this;
        }

        public override List<WizardStep> GetEmptySteps()
        {
            return new List<WizardStep>
	        {
		        new WizardStep {Name = "Template", Key = "admin.template"},
		        new WizardStep {Name = "Restaurant", Key = "admin.stores"},
		        new WizardStep {Name = "Name", Key = "admin.name"},
		        new WizardStep {Name = "Times", Key = "admin.times"},
		        new WizardStep {Name = "Confirmation", Key = "admin.summary"}
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
