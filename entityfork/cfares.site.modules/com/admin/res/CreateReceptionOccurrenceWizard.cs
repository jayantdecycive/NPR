using cfacore.shared.modules.com.admin;
using cfares.domain._event;
using System;
using System.Linq;
using System.Collections.Generic;
using cfares.domain._event.occ;

namespace cfares.site.modules.com.admin.res
{
    public class CreateReceptionOccurrenceWizard : Wizard<IOccurrence>
    {
        public CreateReceptionOccurrenceWizard(IOccurrence occurrence, string id)
            : base(occurrence, id)
        {        
        }

        public override IWizard<IOccurrence> Prime(IOccurrence model)
        {
            string root = "/Admin/Occurrence/";
            string q = "";
            string id = model.Id() ?? "";

            ApplyComplete("admin.capacity", true).WithUri(root + "Capacity/" + id + q);
            ApplyComplete("admin.summary", model.Status == OccurrenceStatus.Live).WithUri(root + "Summary/" + id + q);

     
            
			return this;
        }

        

        
        public override List<WizardStep> GetEmptySteps()
        {
            List<WizardStep> _Steps = new List<WizardStep>();

            
            _Steps.Add(new WizardStep { Name = "Capacity", Key = "admin.capacity" });
//            _Steps.Add(new WizardStep { Name = "Details", Key = "admin.food" });
            //steps.Add(new WizardStep {Name = "Aloha",Key="admin.aloha"});
            
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
