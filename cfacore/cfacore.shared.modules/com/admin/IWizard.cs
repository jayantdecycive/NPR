using System.Web.Routing;
using cfacore.domain._base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cfacore.shared.modules.com.admin
{
    public interface IWizardEventArgs { 
    
    }
    public class WizardEventArgs : EventArgs, IWizardEventArgs
    { 
    
    }

    public delegate void WizardEventHandler(object sender, WizardEventArgs e);

    public interface IWizard // where T:DomainObject,new()
    {
        System.Web.Routing.RouteBase Route { get; set; }

        bool IsCurrent(string key);

        WizardStep FirstStep { get; }
        WizardStep CurrentStep { get; }
        string Description { get; set; }
        WizardStep GetNextStep(string key);
        WizardStep GetPreviousStep(string key);
        WizardStep GetStep(string key);
        string Id { get; set; }
        int Index { get; set; }
        string Name { get; set; }
        WizardStep NextStep { get; }
        void OnComplete(WizardEventArgs e);
        void OnStart(WizardEventArgs e);
        void SetStep(string key);
        
        WizardStep PreviousStep { get; }
        
        System.Collections.Generic.List<WizardStep> Steps { get; }
        List<WizardStep> GetEmptySteps();
        bool Valid { get; }
        bool Validate();
    }

    public interface IWizard<T>:IWizard
    {
        
        IWizard<T> Prime(T model);        
        
    }
}
