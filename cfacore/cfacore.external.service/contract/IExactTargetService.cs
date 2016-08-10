using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cfacore.external.service.ServiceWrapper
{
    public interface IExactTargetService
    {
        cfacore.external.service.ExactTargetClientS4.Subscriber GetSubscriber(string email);

        bool SubscriberExists(string email);

        cfacore.external.service.ExactTargetClientS4.UpdateResult[] UpdateSubscriber(string siteId, DateTime UpdateDate, cfacore.external.service.ExactTargetClientS4.Subscriber subscriber);
        cfacore.external.service.ExactTargetClientS4.UpdateResult[] UpdateSubscriber(string id, string siteId, DateTime UpdateDate, string FirstName, string LastName, string Email, string Zip, DateTime Birthday, string[] MailLists);

        cfacore.external.service.ExactTargetClientS4.CreateResult[] CreateSubscriber(DateTime CreationDate, cfacore.external.service.ExactTargetClientS4.Subscriber subscriber);
        cfacore.external.service.ExactTargetClientS4.CreateResult[] CreateSubscriber(string siteId, DateTime CreationDate, string FirstName, string LastName, string Email, string Zip, DateTime Birthday, string[] MailLists);

        cfacore.external.service.ExactTargetClientS4.ListSubscriber[] GetListsBySubscriber(cfacore.external.service.ExactTargetClientS4.Subscriber Subscriber);
        cfacore.external.service.ExactTargetClientS4.ListSubscriber[] GetListsBySubscriber(string Email);

        cfacore.external.service.ExactTargetClientS4.APIObject RetrieveWithFilter(string objectType, string primaryKey, string filterValue, string[] properties, out string RequestID, out string ResultStr);
        cfacore.external.service.ExactTargetClientS4.APIObject[] RetrieveListWithFilter(string objectType, string primaryKey, string filterValue, string[] properties, out string RequestID, out string ResultStr);
        
        string GetEmailFromHex(string hex);
        string GetHexFromEmail(string email);

        ExactTargetClientS4.CreateResult SaveSubscriber(ExactTargetClientS4.Subscriber subscriber, out string RequestID, out string ResultStr);

        
    }
}
