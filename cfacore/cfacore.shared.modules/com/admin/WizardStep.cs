using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using MvcDomainRouting.Code;

namespace cfacore.shared.modules.com.admin
{
    public delegate void WizardStepEventHandler(object sender, WizardStepEventArgs e);
    

    public class WizardStep:IWizardStep
    {
        private string _uri;

        public WizardStep(){
    
        }
        public string Key { get; set; }
        public bool Complete
        {
            get;
            set;
        }
        public bool Valid
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public string Summary
        {
            get;
            set;
        }

       

        
         public string Uri(RequestContext request)
        {
            if (this.RouteValueDictionary != null && request.RouteData.Route is DomainRoute)
            {
                var dRoute = (DomainRoute)request.RouteData.Route;
                var helper = new UrlHelper(request);
                var url = helper.RouteUrl(dRoute.Name, this.RouteValueDictionary);


                return url + this.QueryString;
            }
            else if(string.IsNullOrEmpty(_uri)){                
                var helper = new UrlHelper(request);
                var url = helper.RouteUrl(this.RouteValueDictionary);


                return url + this.QueryString;
            }
            return _uri; 
        }


         
         public string Uri(ControllerContext controller)
        {
            
            return Uri(controller.RequestContext); 
        }

         public string Uri(ViewContext vc)
         {

             return Uri(vc.RequestContext);
         }
         
         


        public WizardStep WithUri(string uri)
        {
            this._uri = uri;
            return this;
        }

        public WizardStep WithRoute(RouteValueDictionary dict, string q)
        {
            this.RouteValueDictionary = dict;
            this.QueryString = q;
            
            return this;
        }

        public WizardStep WithQuery(string q)
        {
            
            this.QueryString = q;

            return this;
        }

        public WizardStep WithRoute(string action, string id, string q)
        {
            var dict = new RouteValueDictionary()
                {
                    {"Action",action},
                    {"id",id},
                };
            return WithRoute(dict,q);
        }

       

        public bool Validate()
        {
            throw new NotImplementedException();
        }

        public void OnStep(WizardStepEventArgs e)
        {
            throw new NotImplementedException();
        }
        #pragma warning disable
        public event WizardStepEventHandler Stepped;

        public WizardStep WithName(string p)
        {
            this.Name = p;
            return this;
        }

        public RouteValueDictionary RouteValueDictionary { get; set; }

        public string QueryString { get; set; }



        
    }
}
