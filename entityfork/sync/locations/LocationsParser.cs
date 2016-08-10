using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain.user;
using sync._base;
using cfacore.shared.domain.store;
using System.Xml;
using cfacore.site.controllers.shared;
using cfacore.shared.domain.user;
using cfares.domain.user;
using cfacore.service.shared;
using cfacore.service;
using cfacore.mysql.dao.shared;
using cfacore.domain.store;
using cfares.domain.store;

namespace sync.locations
{
    public class LocationsParser :XmlParser<ResStore>
    {
        protected XmlNamespaceManager _locationXnm = null;
        protected StoreService storeServ = null;
        protected StoreMySqlAccess access = null;
        protected XmlDocument locationXml;

        public LocationsParser() {
            
        }

        public LocationsParser(StoreMySqlAccess serv)
        {
            access = serv;
            storeServ = new StoreService();
        }

        public override ResStore ParseElement(XmlNode node) {
            return ParseElement(node,new ResStore());
        }

        public virtual ResStore ParseElement(XmlNode node, ResStore store)
        {
            
            store.LocationNumber = (node.SelectSingleNode("./loc:LocationNumber",_locationXnm).InnerText);
            store.Name = node.SelectSingleNode("./loc:Name", _locationXnm).InnerText;

            store.MarketableURL = new Uri(node.SelectSingleNode("./loc:MarketableURL", _locationXnm).InnerText);
            if (node.SelectSingleNode("./loc:MarketableName", _locationXnm) != null)
                store.MarketableName = node.SelectSingleNode("./loc:MarketableName", _locationXnm).InnerText;
            else
            {
                store.MarketableName = Urlify(Domify(store.Name));
                store.MarketableURL = new Uri("http://www.chick-fil-a.com/" + store.MarketableName);
            }
            if (node.SelectSingleNode("./loc:ProjectedOpenDate", _locationXnm)!=null)
                store.ProjectedOpenDate = DateTime.Parse(node.SelectSingleNode("./loc:ProjectedOpenDate", _locationXnm).InnerText);            
            
            //contact
                
                store.Email = node.SelectSingleNode("./loc:LocationContact/common:EmailAddress", _locationXnm).InnerText;
                store.VoiceMail.Extension = (node.SelectSingleNode("./loc:LocationContact/common:VoiceMail", _locationXnm).InnerText);
                if(node.SelectSingleNode("./loc:LocationContact/common:DaytimePhone",_locationXnm)!=null)
                store.Phone = new Phone(node.SelectSingleNode("./loc:LocationContact/common:DaytimePhone/common:AreaCode", _locationXnm).InnerText+
                    node.SelectSingleNode("./loc:LocationContact/common:DaytimePhone/common:PhoneNumber", _locationXnm).InnerText);
                if (node.SelectSingleNode("./loc:LocationContact/common:FaxNumber", _locationXnm) != null)
                store.Fax = new Phone(node.SelectSingleNode("./loc:LocationContact/common:FaxNumber/common:AreaCode", _locationXnm).InnerText +
                        node.SelectSingleNode("./loc:LocationContact/common:FaxNumber/common:PhoneNumber", _locationXnm).InnerText);
            //contact

            

            store.Status = (StoreStatus)Enum.Parse(typeof(StoreStatus), node.SelectSingleNode("./loc:Status", _locationXnm).InnerText.Trim());
            if (node.SelectSingleNode("./loc:SiteStatus", _locationXnm)!=null)
                store.SiteStatus = node.SelectSingleNode("./loc:SiteStatus", _locationXnm).InnerText;            
            

            //coordinates
            if (node.SelectSingleNode("./loc:GPS/loc:Latitude", _locationXnm) != null)
            {
                store.Coordinates.Latitude = double.Parse(node.SelectSingleNode("./loc:GPS/loc:Latitude", _locationXnm).InnerText);
                store.Coordinates.Longitude = double.Parse(node.SelectSingleNode("./loc:GPS/loc:Longitude", _locationXnm).InnerText);
            }
            //coordinates
            store.ConceptCode = (ConceptCode)Enum.Parse(typeof(ConceptCode), node.SelectSingleNode("./loc:ConceptCode", _locationXnm).InnerText.Trim());
            store.LocationCode = (LocationCode)Enum.Parse(typeof(LocationCode), node.SelectSingleNode("./loc:LocationCode", _locationXnm).InnerText.Trim());

            if (node.SelectSingleNode("./loc:OperatorTeamName", _locationXnm)!=null)
            {
                store.OperatorTeamName = node.SelectSingleNode("./loc:OperatorTeamName", _locationXnm).InnerText;
            }
            if (node.SelectSingleNode("./loc:ServiceTeamName", _locationXnm)!=null)
            {
                store.ServiceTeamName = node.SelectSingleNode("./loc:ServiceTeamName", _locationXnm).InnerText;
            }
            if (node.SelectSingleNode("./loc:RegionName", _locationXnm)!=null)
            {
            store.RegionName = node.SelectSingleNode("./loc:RegionName", _locationXnm).InnerText;
            }
            if (node.SelectSingleNode("./loc:GMTOffset", _locationXnm)!=null)
                store.GMTOffset = node.SelectSingleNode("./loc:GMTOffset", _locationXnm).InnerText;
            if (node.SelectSingleNode("./loc:PriceGroupNumber", _locationXnm)!=null)
                store.PriceGroupNumber = node.SelectSingleNode("./loc:PriceGroupNumber", _locationXnm).InnerText;


            //features
                store.Features.Playground = node.SelectSingleNode("./loc:Features/loc:Playground", _locationXnm).InnerText;
                store.Features.OffersOnlineOrdering = node.SelectSingleNode("./loc:Features/loc:OffersOnlineOrdering", _locationXnm).InnerText.ToLower() == "true";
                store.Features.HasDriveThru = node.SelectSingleNode("./loc:Features/loc:HasDriveThru", _locationXnm).InnerText.ToLower() == "true";
                store.Features.AcceptsCfaCard = node.SelectSingleNode("./loc:Features/loc:AcceptsCfaCard", _locationXnm).InnerText.ToLower() == "true";
                store.Features.HasDiningRoom = node.SelectSingleNode("./loc:Features/loc:HasDiningRoom", _locationXnm).InnerText.ToLower() == "true";
                store.Features.ServesBreakfast = node.SelectSingleNode("./loc:Features/loc:ServesBreakfast", _locationXnm).InnerText.ToLower() == "true";
                store.Features.OffersWireless = node.SelectSingleNode("./loc:Features/loc:OffersWireless", _locationXnm).InnerText.ToLower() == "true";
            //features


                //communication
                store.Distributor = ParseAndSaveLocationDistributor(node.SelectSingleNode("./dist:Distributor", _locationXnm));
                //communication
                //consultants
                store.BusinessConsultant = ParseAndSaveLocationPerson(node.SelectSingleNode("./people:BusinessConsultant", _locationXnm));
                store.MarketingConsultant = ParseAndSaveLocationPerson(node.SelectSingleNode("./people:MarketingConsultant", _locationXnm));
                store.FinancialConsultant = ParseAndSaveLocationPerson(node.SelectSingleNode("./people:FinancialConsultant", _locationXnm));
                store.UnitMarketingDirector = ParseAndSaveLocationPerson(node.SelectSingleNode("./people:UnitMarketingDirector", _locationXnm));
                //consultants
                //db
                store.StreetAddress = ParseAndSaveLocationAddress(node.SelectSingleNode("./loc:LocationContact/common:StreetAddress", _locationXnm), string.Format("Street for location {0:d5}", store.LocationNumber));
                store.ShippingAddress = ParseAndSaveLocationAddress(node.SelectSingleNode("./loc:LocationContact/common:ShippingAddress", _locationXnm), string.Format("Shipping for location {0:d5}", store.LocationNumber));
                store.BillingAddress = ParseAndSaveLocationAddress(node.SelectSingleNode("./loc:LocationContact/common:BillingAddress", _locationXnm), string.Format("Billing for location {0:d5}", store.LocationNumber));
                store.Operator = ParseAndSaveLocationPerson(node.SelectSingleNode("./people:Operator", _locationXnm), store.Email);
                //db


            return store;
        }

        public ResUser ParseUser(XmlNode node, string email) {
            return ParseUser(node,email,new ResUser());
        }

        public ResUser ParseUser(XmlNode node,string email,ResUser user) {
            

            if (node == null)
                return user;
            user.AuthorityUID = node.SelectSingleNode("./people:PersonID", _locationXnm).InnerText;
            user.Authority = "CFAPeople";
            user.Name = new Name(node.SelectSingleNode("./people:Name", _locationXnm).InnerText);
            user.Email = email;
            user.Username = Domify(user.Name.ToString());
            return user;
        }

        public virtual Distributor ParseAndSaveLocationDistributor(XmlNode node)
        {
            Distributor dist = ParseDistributor(node);
            DistributorService serv = new DistributorService();
            Distributor distExists = serv.LoadByShortName(dist.ShortName);
            if (distExists != null)
            {
                dist.Id(distExists.Id());
            }



            serv.Save(dist);
            return dist;
        }

        public virtual Address ParseAndSaveLocationAddress(XmlNode node, string label)
        {
            AddressService serv = new AddressService();
            Address address = ParseAddress(node, label);
            List<Address> oldAddress = serv.LoadByLabel(label);
            if (oldAddress.Count > 0)
                address = oldAddress[0];

            oldAddress = serv.LoadByLine1Line2AndZip(address.Line1, address.Line2, address.Zip.Code);
            if (oldAddress.Count > 0)
                address = oldAddress[0];

            serv.Save(address);
            return address;
        }

        public virtual ResUser ParseAndSaveLocationPerson(XmlNode node,string email)
        {
            ResUser user = ParseUser(node, email);   
            UserService serv = new UserService();
            ResUser oldUser = serv.LoadByDatasource(user.Authority,user.AuthorityUID);



            if (oldUser != null)
            {
                user.Id(oldUser.Id());
                user.Username = oldUser.Username;
                user.Email = oldUser.Email;
            }
            else
            {

                ResUser sameNameUser = serv.LoadByUsername(user.Username);
                string newUsername = user.Username;
                if (sameNameUser != null)
                {
                    newUsername = user.Username + "2";

                    sameNameUser = serv.LoadByUsername(newUsername);
                    if (sameNameUser != null)
                    {
                        newUsername = user.Username + "3";

                        sameNameUser = serv.LoadByUsername(newUsername);
                        if (sameNameUser != null)
                        {
                            newUsername = user.Username + "4";
                        }
                    }
                }
                user.Username = newUsername;
            }

            serv.Save(user);
            return user;
        }

        public ResUser ParseAndSaveLocationPerson(XmlNode node)
        {
            string newEmail = "placeholder."+RandomString(8).ToLower() + "@chick-fil-a.com";
            return ParseAndSaveLocationPerson(node,newEmail);
        }

        public Distributor ParseDistributor(XmlNode node) {
            return ParseDistributor(node, new Distributor());
        }

        public Distributor ParseDistributor(XmlNode node,Distributor dist) {
            

            dist.Name = node.SelectSingleNode("./dist:Name", _locationXnm).InnerText;
            dist.ShortName = node.SelectSingleNode("./dist:ShortName", _locationXnm).InnerText;
            dist.DistributionCenter = node.SelectSingleNode("./dist:DistributionCenter", _locationXnm).InnerText;
            return dist;
        }

        

        public Address ParseAndSaveLocationAddress(XmlNode node) { 
            return ParseAndSaveLocationAddress(node,string.Format("Generated on {0}",DateTime.Now.ToString("MM-dd-yyyy")));
        }

        public Address ParseAddress(XmlNode node, string label) {
            Address address = new Address();
            address.Label = label;

            if (node.SelectSingleNode("./common:Address1", _locationXnm) != null)
                address.Line1 = node.SelectSingleNode("./common:Address1", _locationXnm).InnerText;
            else
                address.Line1 = "";

            if (node.SelectSingleNode("./common:Address2", _locationXnm) != null)
            {
                address.Line2 = node.SelectSingleNode("./common:Address2", _locationXnm).InnerText;
            }
            if (node.SelectSingleNode("./common:Address3", _locationXnm) != null)
            {
                address.Line3 = node.SelectSingleNode("./common:Address3", _locationXnm).InnerText;
            }



            address.City = node.SelectSingleNode("./common:City", _locationXnm).InnerText;
            address.County = node.SelectSingleNode("./common:County", _locationXnm).InnerText;
            address.State = node.SelectSingleNode("./common:State", _locationXnm).InnerText;
            string zipOne = node.SelectSingleNode("./common:ZipCode/common:Zip", _locationXnm).InnerText;
            address.Zip = new Zip(int.Parse(zipOne));
            if (node.SelectSingleNode("./common:ZipCode/common:ZipExtension", _locationXnm) != null)
                address.Zip.PlusFour = int.Parse(node.SelectSingleNode("./common:ZipCode/common:ZipExtension", _locationXnm).InnerText);
            return address;
        }

        

        public override int Iterate(XmlNode node, int index)
        {           

            ResStore existingStore = storeServ.Load(int.Parse(node.SelectSingleNode("./loc:LocationNumber", _locationXnm).InnerText));
            bool storeExists = existingStore != null;

            if (storeExists)
            {
                //User op = existingStore.Operator;
                
                Console.WriteLine(string.Format("Parsed store: {0}", existingStore));
                return index;
            }

            ResStore store = ParseElement(node);
            if (node.SelectSingleNode("./loc:OpenDate", _locationXnm)!=null)
                store.OpenDate = DateTime.Parse(node.SelectSingleNode("./loc:OpenDate", _locationXnm).InnerText);

            
            if (!storeExists){
                store.UnBind();            
                ResStore marketableStore = storeServ.LoadByMarketableName(store.MarketableName);
                if (marketableStore != null)
                {
                    store.MarketableName = store.MarketableName + "2";
                    store.MarketableURL = new Uri("http://www.chick-fil-a.com/"+store.MarketableName);
                }
            }

            storeServ.Save(store);
            
            Console.WriteLine(string.Format("Parsed store: {0}",store));
            return int.Parse(store.Id());

        }
        public bool Main()
        {
            return Main(locationXml);
        }

        public override bool Main(XmlDocument doc)
        {
           
            
            _locationXnm = new XmlNamespaceManager(doc.NameTable);
            _locationXnm.AddNamespace("loc", "http://xmlns.chick-fil-a.com/enterprise/location/v2");
            _locationXnm.AddNamespace("people", "http://xmlns.chick-fil-a.com/enterprise/people/v2");
            _locationXnm.AddNamespace("dist", "http://xmlns.chick-fil-a.com/enterprise/distributor/v1");
            _locationXnm.AddNamespace("svc", "http://xmlns.chick-fil-a.com/services/enterprise/location/v2");
            _locationXnm.AddNamespace("common", "http://xmlns.chick-fil-a.com/enterprise/common/v3");

            XmlNodeList nodes = doc.DocumentElement.SelectNodes("./loc:Location",_locationXnm);
            int total = nodes.Count;
            for (int i = 0; i < total; i++)
            {
                Iterate(nodes[i],i);
                double percentFinished = Math.Ceiling((double)i / (double)total * 100.00);
                int Unfinished = (int)Math.Ceiling((100 - (int)percentFinished) / 4.0);
                int Finished = (int)Math.Floor(((int)percentFinished) / 4.0);
                Console.WriteLine("[" + "".PadLeft(Finished, '=')+"".PadRight(Unfinished, ' ') + "]  " + string.Format("{0}% complete", percentFinished));
                

            }
            return true;
        }


        public override bool Init()
        {
            string default_src = System.IO.Path.GetFullPath("../../locations/data/locations.xml");
            return Init(default_src);
        }


        

        public override bool ClearTables(){
            return access.AssertToStoredProcedure("_Parser_ClearLocations")>0;             
        }
    }


    public class FutureLocationsParser : LocationsParser {

        public override bool Init() {
            string default_src = System.IO.Path.GetFullPath("../../locations/data/locationsfuture.xml");
            return Init(default_src);
        }

        public override int Iterate(XmlNode node, int index)
        {
            ResStore existingStore = storeServ.Load(int.Parse(node.SelectSingleNode("./loc:LocationNumber", _locationXnm).InnerText));
            bool storeExists = existingStore != null;

            if (storeExists)
            {
                Console.WriteLine(string.Format("Parsed future store: {0}", existingStore));
                return index;
            }

            ResStore store = ParseElement(node);
            if (node.SelectSingleNode("./loc:OpenDate", _locationXnm) != null)
                store.OpenDate = DateTime.Parse(node.SelectSingleNode("./loc:OpenDate", _locationXnm).InnerText);


            if (!storeExists)
            {
                store.UnBind();
                ResStore marketableStore = storeServ.LoadByMarketableName(store.MarketableName);
                if (marketableStore != null)
                {
                    store.MarketableName = store.MarketableName + "2";
                    store.MarketableURL = new Uri("http://www.chick-fil-a.com/" + store.MarketableName);
                }
            }

            storeServ.Save(store);

            Console.WriteLine(string.Format("Parsed future store: {0}", store));
            return int.Parse(store.Id());
        }

    }
}
