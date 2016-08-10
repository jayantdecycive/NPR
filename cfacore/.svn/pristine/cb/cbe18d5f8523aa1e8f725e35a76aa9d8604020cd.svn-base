using System.Web;
using cfacore.domain._base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Xml;

namespace cfacore.shared.modules.com.admin
{
    [Serializable]
    public abstract class Wizard<T> : IWizard<T> // where T:DomainObject,new()
    {
        public virtual T Model { get; set; }
        
        public Wizard(T model,string id) {

            if (Qs == null)
            {
                Qs = new Dictionary<string, string>();
            }

            this.Id = id;
            this.Model = model;
            Prime(model);
        }

        public Wizard()
        {

            if (Qs == null)
            {
                Qs = new Dictionary<string, string>();
            }
            
        }

        public bool IsCurrent(string key)
        {
            return GetStep(key) == CurrentStep;
        }

        public bool IsComplete(string key)
        {
            return GetStep(key).Complete;
        }

        protected IDictionary<string, string> ToDictionary(string qs)
        {
            var collec = HttpUtility.ParseQueryString(qs);
            return collec.AllKeys.ToDictionary(k => k, k => collec[k]);
        }

        public string Q
        {
            get { return ToQueryString(Qs); }
        }

        public Wizard(T model, string id, string querystring):this(model,id)
        {
            Qs = ToDictionary(querystring);
        }

        protected string[] SystemRequestKeys
        {
            get { return new[] { "message" }; }
        }

        protected string ToQueryString(IDictionary<string, string> nvc)
        {
            if (nvc == null) return string.Empty;
            return "?" + string.Join("&", Array.ConvertAll(
                nvc.Keys.Where(x => !SystemRequestKeys.Contains(x)).ToArray(),
                    key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(nvc[key]))));
        }

        protected IDictionary<string, string> Qs;

        protected virtual WizardStep ApplyComplete(string key, bool predicate)
        {
            WizardStep step = Steps.Find(x=>x.Key==key);
            if (step == null)
                throw new Exception("Cannot find wizard step for key:"+key);
            step.Complete = predicate || step.Complete;
            return step;
        }

        protected virtual WizardStep ApplyComplete(WizardStep step, bool predicate)
        {
            step.Complete = predicate;
            return step;
        }

        public bool Complete {
            get { return Steps.All(x => x.Complete); }
        }

        public abstract IWizard<T> Prime(T model);

        public abstract List<WizardStep> GetEmptySteps();

        private List<WizardStep> _Steps;
        public virtual List<WizardStep> Steps { get{
            if (_Steps == null)
            {
                //_Steps = new List<WizardStep>();
                _Steps = this.GetEmptySteps();
            }
            return _Steps;
        } }

        public virtual WizardStep CurrentStep
        {
            get { return Steps[Index]; }
        }

        public virtual WizardStep FirstStep
        {
            get { return Steps.First(); }
        }

        public virtual WizardStep PreviousStep
        {
            get {
                if (Index == 0)
                    return null;
                return Steps[Index-1];             
            }
        }

        public virtual void SetStep(string key){
            this.Index = this.Steps.FindIndex(x=>x.Key==key);
        }

        public virtual WizardStep GetPreviousStep(string key)
        {
            return Steps[Steps.FindIndex(x => x.Key == key)-1];
        }

        public virtual WizardStep GetStep(string key)
        {
            return Steps.Find(x=>x.Key==key);
        }

        public virtual WizardStep GetNextStep(string key)
        {
            return Steps[Steps.FindIndex(x => x.Key == key)+1];
        }

        public virtual WizardStep NextStep
        {
            get
            {
                if (Index >= (Steps.Count-1))
                    return null;
                return Steps[Index + 1];
            }
        }

        public virtual string SessionId
        {
            get { return DefaultSessionId; }
        }


        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public virtual int Index
        {
            get;
            set;
        }


        public virtual string Id { get; set; }



        public virtual bool Valid
        {
            get {
                foreach (WizardStep step in Steps) {
                    if (!step.Valid)
                        return false;
                }
                return true;
            }
        }

        public virtual bool Validate()
        {
            foreach (WizardStep step in Steps)
            {
                if (!step.Validate())
                    return false;
            }
            return Valid;
        }

        public virtual string Description { get; set; }
        public virtual string Name { get; set; }

        public virtual void OnComplete(WizardEventArgs e)
        {
            throw new NotImplementedException();
        }
        
        public virtual void OnStart(WizardEventArgs e)
        {
            throw new NotImplementedException();
        }


        public static string DefaultSessionId { get { return "wizard"; } }

        public System.Web.Routing.RouteBase Route { get; set; }
    }
}
