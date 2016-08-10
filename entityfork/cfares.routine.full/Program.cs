using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using cfacore.domain.user;
using cfacore.shared.domain.store;
using cfacore.shared.domain.user;
using cfares.domain.user;
using cfares.entity.dbcontext.res_event;
using cfares.routine.full.ado;
using cfares.routine.full.locations;
using sync.locations;
using cfares.domain.store;

namespace cfares.routine.full
{
    class Program
    {
        
        static void Main(string[] args)
        {
            CfaResContext context = new CfaResContext();

            var stores = context.Stores.Include("Operator").Include("StreetAddress").Include("BillingAddress").Include("ShippingAddress").ToList();

            CfaComEntities comContext = new CfaComEntities();
            var comStores = comContext.StoreInfoes.ToList();

            Console.WriteLine("Stores: " + stores.Count);
            Console.WriteLine("comStores: " + comStores.Count);
            Console.WriteLine("\n");


            foreach (var comStore in comStores)
            {

                var existingStore = stores.FirstOrDefault(x => x.LocationNumber == comStore.LocationNumber);
                if (existingStore == null)
                {
                    //continue;
                    //mdrake not doing this right now
                    //carson: doing this now
                    existingStore = new ResStore()
                    {
                        BillingAddress = new Address(),
                        ShippingAddress = new Address(),
                        StreetAddress = new Address(),
                    };
                }
                existingStore.LocationNumber = comStore.LocationNumber;
                context.Stores.Add(existingStore);
                Console.WriteLine("Adding new Store: " + comStore.LocationNumber + " " + comStore.Name);
                
                existingStore.AcceptsCfaCard = comStore.AcceptsCfaCard.Value;
                existingStore.CorporateOwned = comStore.CorporateOwned;
                existingStore.HasDiningRoom = comStore.HasDiningRoom.Value;
                existingStore.HasDriveThru = comStore.HasDriveThru.Value;
                existingStore.LocationDescription = comStore.LocationDescription;
                existingStore.MarketableName = String.IsNullOrWhiteSpace(comStore.MarketableName) ? comStore.Name.ToLower().Replace(" ", "") : comStore.MarketableName ;
                existingStore.MarketableURL = new Uri(comStore.MarketableURL);
                existingStore.Message = comStore.Message;
                existingStore.Name = comStore.Name;
                existingStore.OffersOnlineOrdering = comStore.OffersOnlineOrdering.Value;
                existingStore.OffersWireless = comStore.OffersWireless.Value;
                existingStore.OpenDate = comStore.OpenDate.Value;
                existingStore.Playground = comStore.Playground;
                existingStore.ProjectedOpenDate = comStore.ProjectedOpenDate.Value;
                existingStore.ServesBreakfast = comStore.ServesBreakfast.Value;
                existingStore.VoiceMailString = comStore.VoiceMail;
                existingStore.PhoneString = comStore.PhoneNumber;
                existingStore.FaxString = comStore.FaxNumber;
                existingStore.Email = comStore.EmailAddress;

                existingStore.BillingAddress.Line1 = comStore.BillingAddress_Address1;
                existingStore.BillingAddress.City = comStore.BillingAddress_City;
                existingStore.BillingAddress.County = comStore.BillingAddress_County;
                existingStore.BillingAddress.State = comStore.BillingAddress_State;
                existingStore.BillingAddress.ZipString = comStore.BillingAddress_Zip + " " + comStore.BillingAddress_ZipExtension;
                existingStore.BillingAddress.State = comStore.BillingAddress_State;

                existingStore.StreetAddress.Line1 = comStore.StreetAddress_Address1;
                existingStore.StreetAddress.City = comStore.StreetAddress_City;
                existingStore.StreetAddress.County = comStore.StreetAddress_County;
                existingStore.StreetAddress.State = comStore.StreetAddress_State;
                existingStore.StreetAddress.ZipString = comStore.StreetAddress_Zip + " " + comStore.StreetAddress_ZipExtension;
                existingStore.StreetAddress.State = comStore.StreetAddress_State;

                existingStore.ShippingAddress.Line1 = comStore.ShippingAddress_Address1;
                existingStore.ShippingAddress.City = comStore.ShippingAddress_City;
                existingStore.ShippingAddress.County = comStore.ShippingAddress_County;
                existingStore.ShippingAddress.State = comStore.ShippingAddress_State;
                existingStore.ShippingAddress.ZipString = comStore.ShippingAddress_Zip + " " + comStore.ShippingAddress_ZipExtension;
                existingStore.ShippingAddress.State = comStore.ShippingAddress_State;

                if (!string.IsNullOrEmpty(comStore.GPS_Latitude))
                {
                    existingStore.Lat = double.Parse(comStore.GPS_Latitude);
                    existingStore.Lon = double.Parse(comStore.GPS_Longitude);
                }

                if (existingStore.Operator != null)
                    existingStore.Operator.UID = comStore.OperatorContact_PersonID.ToString();
            }

            context.SaveChanges();




            /*
            foreach (var comStore in comStores)
            {
                var existingStore = stores.FirstOrDefault(x => x.LocationNumber == comStore.LocationNumber);
                if (existingStore == null)
                {


                    //continue;
                    //mdrake not doing this right now
                    existingStore = new ResStore()
                    {
                        BillingAddress = new Address(),
                        ShippingAddress = new Address(),
                        StreetAddress = new Address(),
                        Operator = new User(),
                        //Distributor = new Distributor(),
                        FinancialConsultant = new User(),
                        Phone = new Phone(),
                    };

                    existingStore.LocationNumber = comStore.LocationNumber;
                    context.Stores.Add(existingStore);
                    Console.WriteLine("Adding new Store for " + comStore.LocationNumber);
                }

                
                existingStore.AcceptsCfaCard = comStore.AcceptsCfaCard.Value;
                //existingStore.AreaCode = comStore.AreaCode;
                //existingStore.ConceptCode = comStore.ConceptCode;
                existingStore.CorporateOwned = comStore.CorporateOwned;
                //existingStore.FaxAreaCode = comStore.FaxAreaCode;
                //existingStore.FaxNumber = comStore.FaxNumber;
                existingStore.HasDiningRoom = comStore.HasDiningRoom.Value;
                existingStore.HasDriveThru = comStore.HasDriveThru.Value;
                //existingStore.LocationCode = comStore.LocationCode;
                existingStore.LocationDescription = comStore.LocationDescription;
                existingStore.MarketableName = comStore.MarketableName;
                existingStore.MarketableURL = new Uri(comStore.MarketableURL);
                existingStore.Message = comStore.Message;
                existingStore.Name = comStore.Name;
                //existingStore.NoUpdate = comStore.NoUpdate;
                existingStore.OffersOnlineOrdering = comStore.OffersOnlineOrdering.Value;
                existingStore.OffersWireless = comStore.OffersWireless.Value;
                existingStore.OpenDate = comStore.OpenDate.Value;
                //existingStore.OperatorContact_Name = comStore.OperatorContact_Name;
                //existingStore.OperatorContact_PersonID = comStore.OperatorContact_PersonID;
                //existingStore.PhoneNumber = comStore.PhoneNumber;
                existingStore.Playground = comStore.Playground;
                existingStore.ProjectedOpenDate = comStore.ProjectedOpenDate.Value;
                existingStore.ServesBreakfast = comStore.ServesBreakfast.Value;
                //existingStore.Status = comStore.Status;
                existingStore.VoiceMail = new Phone(comStore.VoiceMail);
                existingStore.DistributorId = "mbm";


                existingStore.BillingAddress.Line1 = comStore.BillingAddress_Address1;
                existingStore.BillingAddress.City = comStore.BillingAddress_City;
                existingStore.BillingAddress.County = comStore.BillingAddress_County;
                existingStore.BillingAddress.State = comStore.BillingAddress_State;
                existingStore.BillingAddress.ZipString = comStore.BillingAddress_Zip + " " + comStore.BillingAddress_ZipExtension;
                existingStore.BillingAddress.State = comStore.BillingAddress_State;

                existingStore.StreetAddress.Line1 = comStore.StreetAddress_Address1;
                existingStore.StreetAddress.City = comStore.StreetAddress_City;
                existingStore.StreetAddress.County = comStore.StreetAddress_County;
                existingStore.StreetAddress.State = comStore.StreetAddress_State;
                existingStore.StreetAddress.ZipString = comStore.StreetAddress_Zip + " " + comStore.StreetAddress_ZipExtension;
                existingStore.StreetAddress.State = comStore.StreetAddress_State;

                existingStore.ShippingAddress.Line1 = comStore.ShippingAddress_Address1;
                existingStore.ShippingAddress.City = comStore.ShippingAddress_City;
                existingStore.ShippingAddress.County = comStore.ShippingAddress_County;
                existingStore.ShippingAddress.State = comStore.ShippingAddress_State;
                existingStore.ShippingAddress.ZipString = comStore.ShippingAddress_Zip + " " + comStore.ShippingAddress_ZipExtension;
                existingStore.ShippingAddress.State = comStore.ShippingAddress_State;

                if (!string.IsNullOrEmpty(comStore.GPS_Latitude))
                {
                    existingStore.Lat = double.Parse(comStore.GPS_Latitude);
                    existingStore.Lon = double.Parse(comStore.GPS_Longitude);
                }

                if (existingStore.Operator != null)
                    existingStore.Operator.UID = comStore.OperatorContact_PersonID.ToString();


            }
            Console.WriteLine("Stores: " + context.Stores.Count());
            Console.WriteLine("comStores: " + comStores.Count);

            context.SaveChanges();

             */
          
            /*
            XmlDocument xml = new XmlDocument();
            string default_src = System.IO.Path.GetFullPath("../../locations/data/locations.xml");
            
            xml.Load(default_src);
            
            List<ResUser> users = context.ResUsers.ToList();
            foreach(ResUser user in users){
            //    context.ResUsers.Remove(user);
            }
            //context.SaveChanges();
            LocationsParserEntity lparse = new LocationsParserEntity(context);
            lparse.Main(xml);

            foreach(Store store in stores){
                Console.WriteLine(store.Name);
            }*/

        }
    }
}
