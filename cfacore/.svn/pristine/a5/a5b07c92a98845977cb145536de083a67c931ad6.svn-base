using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace cfacore.shared.modules.com.admin
{
    public interface IWizardStepEventArgs
    {

    }
    public class WizardStepEventArgs : EventArgs, IWizardStepEventArgs
    {

    }

    public interface IWizardStep
    {
        
        void OnStep(WizardStepEventArgs e);
        event WizardStepEventHandler Stepped;
        string Key { get; set; }

        bool Complete { get; set; }
        bool Valid { get; set; }
        string Name { get; set; }
        string Summary { get; set; }
        string Uri(RequestContext context);
        string Uri(ViewContext context);
        string Uri(ControllerContext context);
        bool Validate();
    }
}
