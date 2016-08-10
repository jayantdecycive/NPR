using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using cfacore.shared.domain.store;
using cfacore.shared.domain.user;
using cfares.domain.user;
using cfares.entity.dbcontext.res_event;
using sync.locations;
using cfares.domain.store;

namespace cfares.routine.full.locations
{
    public class LocationsParserEntity : LocationsParser
    {
        private entity.dbcontext.res_event.CfaResContext context;
        private System.Xml.XmlDocument xml;

        public LocationsParserEntity(CfaResContext context):base()
        {
            // TODO: Complete member initialization
            this.context = context;            
        }

        public override int Iterate(XmlNode node, int index)
        {

            ResStore existingStore = context.Stores.Find(node.SelectSingleNode("./loc:LocationNumber", _locationXnm).InnerText);
            bool storeExists = existingStore != null;

            ResStore store;
            if (storeExists)
            {
                //Console.WriteLine(string.Format("Parsed store: {0}", existingStore));
                return index;
                store = ParseElement(node, existingStore);
            }
            else {
                store = ParseElement(node);
            }

            
            if (node.SelectSingleNode("./loc:OpenDate", _locationXnm) != null)
                store.OpenDate = DateTime.Parse(node.SelectSingleNode("./loc:OpenDate", _locationXnm).InnerText);


            if (!storeExists)
            {
                Store marketableStore = context.Stores.FirstOrDefault(x => x.MarketableName == store.MarketableName);
                if (marketableStore != null)
                {

                    store.MarketableName = store.MarketableName + "2";
                    Console.WriteLine(string.Format("Temporary Maretable name for {0}: {1}", store.LocationNumber, store.MarketableName));
                    store.MarketableURL = new Uri("http://www.chick-fil-a.com/" + store.MarketableName);
                }
                context.Stores.Add(store);
            }
            else {
                context.Entry(store).State = System.Data.EntityState.Modified;                
            }


            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (DbEntityValidationResult result in ex.EntityValidationErrors)
                {
                    foreach (DbValidationError error in result.ValidationErrors)
                        Console.WriteLine(string.Format("{0} -> {1}", error.PropertyName, error.ErrorMessage));
                }


            }catch(Exception ex){
                Console.WriteLine(ex.Message);
                if (ex.InnerException!=null)
                Console.WriteLine(ex.InnerException.Message);
            }

            Console.WriteLine(string.Format("Parsed store: {0}", store));
            return int.Parse(store.Id());

        }

        public override Distributor ParseAndSaveLocationDistributor(XmlNode node)
        {
            Distributor dist = ParseDistributor(node);
            
            Distributor distExists = context.Distributors.FirstOrDefault(x=>x.ShortName==dist.ShortName);
            if (distExists != null)
            {
                return distExists;
                //dist.Id(distExists.Id());
                
            }
            else {
                context.Distributors.Add(dist);
                try
                {
                    context.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (DbEntityValidationResult result in ex.EntityValidationErrors)
                    {
                        foreach (DbValidationError error in result.ValidationErrors)
                            Console.WriteLine(string.Format("{0} -> {1}", error.PropertyName, error.ErrorMessage));
                    }


                }
            }

            

            return dist;
        }

        public override Address ParseAndSaveLocationAddress(XmlNode node, string label)
        {            
            Address address = ParseAddress(node, label);
            List<Address> oldAddress = context.Addresses.Where(x=>x.Label==label).ToList();
            bool modified = false;
            if (oldAddress.Count > 0)
            {
                address = oldAddress[0];
                modified = true;
            }

            oldAddress = context.Addresses.Where(x => x.Line1 == address.Line1 && x.Line2 == address.Line2 && x.ZipString == address.ZipString).ToList();
            if (oldAddress.Count > 0)
            {
                address = oldAddress[0];
                modified = true;
            }

            if (modified)
                context.Entry(address).State = System.Data.EntityState.Modified;
            else
                context.Addresses.Add(address);

            context.SaveChanges();
            return address;
        }

        public override ResUser ParseAndSaveLocationPerson(XmlNode node, string email)
        {
            if (node == null)
                return null;
            ResUser user = ParseUser(node, email);
            if (user == null)
                return null;
            
            ResUser oldUser = context.ResUsers.FirstOrDefault(x=> x.Authority == user.Authority && x.AuthorityUID== user.AuthorityUID);


            bool modified = false;
            if (oldUser != null)
            {
                user = oldUser;
                modified = true;
            }
            else
            {

                ResUser sameNameUser = context.ResUsers.FirstOrDefault(x=>x.Username==user.Username);
                string newUsername = user.Username;
                if (sameNameUser != null)
                {
                    newUsername = user.Username + "2";

                    sameNameUser = context.ResUsers.FirstOrDefault(x => x.Username == newUsername);
                    if (sameNameUser != null)
                    {
                        newUsername = user.Username + "3";

                        sameNameUser = context.ResUsers.FirstOrDefault(x => x.Username == newUsername);
                        if (sameNameUser != null)
                        {
                            newUsername = user.Username + "4";
                        }
                    }
                }
                user.Username = newUsername;
            }

            if (modified)
                context.Entry(user).State = System.Data.EntityState.Modified;
            else
                context.ResUsers.Add(user);

            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (DbEntityValidationResult result in ex.EntityValidationErrors) {
                    foreach(DbValidationError error in result.ValidationErrors)
                        Console.WriteLine(string.Format("{0} -> {1}", error.PropertyName, error.ErrorMessage));
                }
                
            
            }
            return user;
        }
    }
}
