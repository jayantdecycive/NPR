using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using cfacore.external.service.ExactTargetClientS4;

namespace cfacore.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Connect" in code, svc and config file together.
    public class Connect : IConnect
    {
        public void DoWork()
        {
        }

        public external.service.ExactTargetClientS4.DetailedCreateResult CreateSubscriber(string site, string email, string firstName, string lastName, string dob, string state, string zip)
        {
            if (string.IsNullOrEmpty(site))
                site = "WebApi";
            else
                site += "-WebApi";
            ExtendedSubscriber subscriber = new ExtendedSubscriber(null);
            subscriber.SourceID = site;
            subscriber.Email = email;
            subscriber.FirstName = firstName;
            subscriber.LastName = lastName;
            if (!string.IsNullOrEmpty(dob))
                subscriber.Birthday = DateTime.Parse(dob);
            subscriber.State = state;
            subscriber.ZipCode = zip;

            string RequestID;
            string ResultStr;
            cfacore.external.service.ServiceWrapper.ExactTargetService service = new cfacore.external.service.ServiceWrapper.ExactTargetService();
            external.service.ExactTargetClientS4.DetailedCreateResult result = new external.service.ExactTargetClientS4.DetailedCreateResult();            
            service.SaveSubscriber(subscriber.subscriber, out RequestID, out ResultStr);
            result.RequestID = RequestID;
            result.ResultStr = ResultStr;
            return result;
        }
    }
}
