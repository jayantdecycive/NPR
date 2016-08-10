using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Net;
using System.Security.Permissions;
using System.Text;
using System.IO;
using System.Security.Cryptography;



namespace cfacore.external.service.ExactTargetClientS4
{
    public class DetailedResult {        
        public string ResultStr { get; set; }
        public string RequestID { get; set; }
    }

    public class DetailedCreateResult:DetailedResult
    {
        //public external.service.ExactTargetClientS4.CreateResult CreateResult { get; set; }        
    }

    public class DetailedAPIObjectResult : DetailedResult
    {
        //public external.service.ExactTargetClientS4.APIObject APIObject { get; set; }
    }

    public class DetailedAPIObjectListResult : DetailedResult
    {
        //public external.service.ExactTargetClientS4.APIObject[] APIObjects { get; set; }
    }

    [DataContract]
    public partial class ExtendedSubscriber 
    {
        private Subscriber _subscriber=null;
        public Subscriber subscriber{
            get {
                if(_subscriber==null)
                    _subscriber=new Subscriber();
                return _subscriber;
            }
            set {
                _subscriber = value;
            }
        }

        public ExtendedSubscriber(Subscriber subscriber) {
            this.subscriber = subscriber;
        }

        public string this[string index]
        {
            get
            {
                if (this.subscriber.Attributes == null)
                    return null;
                foreach (ExactTargetClientS4.Attribute attr in this.subscriber.Attributes)
                {
                    if (attr.Name == index)
                        return attr.Value;
                }
                return null;
            }
            set
            {
                bool modified = false;
                if (this.subscriber.Attributes != null)
                {
                    foreach (ExactTargetClientS4.Attribute attr in this.subscriber.Attributes)
                    {
                        if (attr.Name == index)
                        {
                            attr.Value = value;
                            modified = true;
                            break;
                        }
                    }
                }
                else {
                    this.subscriber.Attributes = new ExactTargetClientS4.Attribute[0];
                }
                if (!modified) {
                    List<ExactTargetClientS4.Attribute> attrs = new List<ExactTargetClientS4.Attribute>(this.subscriber.Attributes);
                    ExactTargetClientS4.Attribute newAtt = new ExactTargetClientS4.Attribute();
                    newAtt.Value = value;
                    newAtt.Name = index;
                    attrs.Add(newAtt);
                    this.subscriber.Attributes = attrs.ToArray();
                }
            }
        }

        [XmlIgnore]
        [IgnoreDataMember]
        public string CowPhotoContest2011_FirstName
        {
            get { return this["CowPhotoContest2011_First_Name"]; }
            set { this["CowPhotoContest2011_First_Name"] = value; }
        }

        [DataMember]
        public string CowPhotoContest2011_PhotoLink
        {
            get { return this["CowPhotoContest2011_PhotoLink"]; }
            set { this["CowPhotoContest2011_PhotoLink"] = value; }
        }

        [DataMember]
        public string Address1
        {
            get { return this["Address1"]; }
            set { this["Address1"] = value; }
        }

        [DataMember]
        public bool AgeVerification
        {
            get { return this["AgeVerification"]=="T"; }
            set { this["AgeVerification"] = value ? "T" : "F" ; }
        }

        [DataMember] 
        public string FirstName
        {
            get { return this["First Name"]; }
            set { this["First Name"] = value; }
        }

         [DataMember]
        public string State
        {
            get { return this["State"]; }
            set { this["State"] = value; }
        }

        [DataMember]
        public string ZipCode
        {
            get { return this["Zip Code"]; }
            set { this["Zip Code"] = value; }
        }

        [DataMember]
        public DateTime Birthday
        {
            get { 
                string bday = this["Birthday"];
                if (string.IsNullOrEmpty(bday))
                    return new DateTime();
                return DateTime.Parse(bday); 
            }
            set { this["Birthday"] = value.ToString("MM/dd/yyyy"); }
        }

        [DataMember]
        public string Stories
        {
            get { return this["Stories"]; }
            set { this["Stories"] = value; }
        }

        [DataMember]
        public string SourceID
        {
            get { return this["SourceID"]; }
            set { this["SourceID"] = value; }
        }

        [DataMember]
        public string City
        {
            get { return this["City"]; }
            set { this["City"] = value; }
        }

         [DataMember]
        public string Address2
        {
            get { return this["Address2"]; }
            set { this["Address2"] = value; }
        }

        [DataMember]
        public string DateAdded
        {
            get { return this["Date added"]; }
            set { this["Date added"] = value; }
        }

        [DataMember]
        public string KidsandFamilyNewsletter
        {
            get { return this["Kids  and Family Newsletter"]; }
            set { this["Kids  and Family Newsletter"] = value; }
        }

        [DataMember]
        public bool CFAStaff
        {
            get { return this["CFA_Staff"]=="True"; }
            set { this["CFA_Staff"] = value?"True":"False"; }
        }

        [DataMember]
        public string SpicyChickenSandwichReservation
        {
            get { return this["SpicyChickenSandwich_Reservation"]; }
            set { this["SpicyChickenSandwich_Reservation"] = value; }
        }

        [DataMember]
        public string LastName
        {
            get { return this["Last Name"]; }
            set { this["Last Name"] = value; }
        }

        [DataMember]
        public string load_50411
        {
            get { return this["load_50411"]; }
            set { this["load_50411"] = value; }
        }

        [DataMember]
        public DateTime DateUpdated
        {
            get
            {
                string dateUpdated = this["Date updated"];
                if (string.IsNullOrEmpty(dateUpdated))
                    return new DateTime();
                return DateTime.Parse(dateUpdated);            
            }
            set { this["Date updated"] = value.ToString("MM/dd/yyyy"); }
        }

        [DataMember]
        public string CFACows
        {
            get { return this["CFA_Cows"]; }
            set { this["CFA_Cows"] = value; }
        }

        [DataMember]
        public bool HealthyLifestyle
        {
            get { return this["healthy_lifestyle"] == "T"; }
            set { this["healthy_lifestyle"] = value ? "T" : "F"; }
        }

        [DataMember]
        public bool FamilyActivities
        {
            get { return this["family_activities"] == "T"; }
            set { this["family_activities"] = value ? "T" : "F"; }
        }

        [DataMember]
        public bool SportingEvents
        {
            get { return this["sporting_events"] == "T"; }
            set { this["sporting_events"] = value ? "T" : "F"; }
        }

        [DataMember]
        public bool Leadership
        {
            get { return this["leadership"] == "T"; }
            set { this["leadership"] = value ? "T" : "F"; }
        }

        [DataMember]
        public bool KidsNutrition
        {
            get { return this["kids_nutrition"] == "T"; }
            set { this["kids_nutrition"] = value ? "T" : "F"; }
        }

        [DataMember]
        public string Email
        {
            get { return subscriber.EmailAddress; }
            set { this["Email Address"] = value; subscriber.EmailAddress = value; }
        }

        [DataMember]
        public int ID
        {
            get { return subscriber.ID; }
            set { subscriber.ID = value; }
        }

   


    }
}

namespace cfacore.external.service.ServiceWrapper
{
    class ETActionAddBehavior : IEndpointBehavior
    {
        #region IEndpointBehavior Members

        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new ETActionAddInspector());
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {

        }

        public void Validate(ServiceEndpoint endpoint)
        {

        }

        #endregion
    }
    class ETActionAddInspector : IClientMessageInspector
    {
        #region IClientMessageInspector Members

        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {

        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel)
        {
            //request.Headers.Action = "Create";
            return null;
        }

        #endregion
    }

    public class ExactTargetService : IExactTargetService
    {

        public static string MID = "1030053";
        public static int LID = 32352;

        public static int CAL_REMINDER_LID = 135965;

        public ExactTargetClientS4.Subscriber GetSubscriber(string email)
        {
            String requestID;
            String status;

            ExactTargetClientS4.APIObject result = RetrieveWithFilter("Subscriber",  "EmailAddress", email,
                new string[] { "ID", "CreatedDate", "Client.ID", "EmailAddress", "SubscriberKey", "UnsubscribedDate", "Status", "EmailTypePreference" },
                out requestID, out status);

            ExactTargetClientS4.Subscriber res = result as ExactTargetClientS4.Subscriber;
            return res;

        }

        public ExactTargetClientS4.Account GetAccount(string email)
        {
            String requestID;
            String status;

            ExactTargetClientS4.APIObject result = RetrieveWithFilter("Account", "Email", email,
                new string[] { "ID", "Address", "City", "Email", "Name", "Zip" },
                out requestID, out status);

            return result as ExactTargetClientS4.Account;

        }


        public ExactTargetClientS4.ListSubscriber[] GetListsBySubscriber(ExactTargetClientS4.Subscriber Subscriber)
        {            
            return GetListsBySubscriber(Subscriber.EmailAddress);
        }

        public ExactTargetClientS4.ListSubscriber[] GetListsBySubscriber(string email)
        {
            String requestID;
            String status;

            ExactTargetClientS4.APIObject[] result = RetrieveListWithFilter("ListSubscriber", "SubscriberKey", email,
                new string[] { "ListID", "SubscriberKey", "Status" },
                out requestID, out status);
            List<ExactTargetClientS4.ListSubscriber> typedResult = new List<ExactTargetClientS4.ListSubscriber>();
            if(result!=null)
            for(int i =0; i< result.Length;i++)
                typedResult.Add((ExactTargetClientS4.ListSubscriber)result[i]);

            return typedResult.ToArray();
        }

        public ExactTargetClientS4.UpdateResult[] UpdateSubscriber(string siteId, DateTime UpdateDate, ExactTargetClientS4.Subscriber subscriber)
        {
            throw new NotImplementedException();
        }

        public ExactTargetClientS4.UpdateResult[] UpdateSubscriber(string id, string siteId, DateTime UpdateDate, string FirstName, string LastName, string Email, string Zip, DateTime Birthday, string[] MailLists)
        {
            throw new NotImplementedException();
        }

        public ExactTargetClientS4.CreateResult[] CreateSubscriber(DateTime CreationDate, ExactTargetClientS4.Subscriber subscriber)
        {
            throw new NotImplementedException();
        }

        public ExactTargetClientS4.CreateResult[] CreateSubscriber(string siteId, DateTime CreationDate, string FirstName, string LastName, string Email, string Zip, DateTime Birthday, string[] MailLists)
        {
            throw new NotImplementedException();
        }

        


       


        private ExactTargetClientS4.SoapClient GetDefaultClient()
        {
            // Create the binding            
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.Name = "UserNameSoapBinding";
            binding.Security.Mode = BasicHttpSecurityMode.TransportWithMessageCredential;
            binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            binding.OpenTimeout = new TimeSpan(0, 5, 0);
            binding.CloseTimeout = new TimeSpan(0, 5, 0);
            binding.SendTimeout = new TimeSpan(0, 5, 0);

           
            // Set the transport security to UsernameOverTransport for Plaintext usernames
            
            EndpointAddress endpoint = new EndpointAddress("https://webservice.s4.exacttarget.com/Service.asmx");

            

            // Create the SOAP Client (and pass in the endpoint and the binding)
            ExactTargetClientS4.SoapClient etFramework = new cfacore.external.service.ExactTargetClientS4.SoapClient(binding, endpoint);

            WebPermission permission = new WebPermission(PermissionState.None);
            //permission.AddPermission(NetworkAccess.Connect, "https://webservice.s4.exacttarget.com");
            permission.AddPermission(NetworkAccess.Connect, "https://webservice.s4.exacttarget.com/Service.asmx");
            permission.Assert();

            etFramework.Endpoint.Behaviors.Add(new ETActionAddBehavior());

            etFramework.ClientCredentials.UserName.UserName = "cfaapi";
            etFramework.ClientCredentials.UserName.Password = "Et_n+N8v";

            return etFramework;

        }
        public ExactTargetClientS4.CreateResult UnsubSubscriber(ExactTargetClientS4.Subscriber subscriber, out string RequestID, out string ResultStr) 
        {
            ExactTargetClientS4.SoapClient client = GetDefaultClient();

            subscriber.Lists = new ExactTargetClientS4.SubscriberList[] { new ExactTargetClientS4.SubscriberList() };
            subscriber.Lists[0].ID = LID;
            subscriber.Lists[0].IDSpecified = true;
            subscriber.Lists[0].Status = ExactTargetClientS4.SubscriberStatus.Unsubscribed;
            subscriber.Lists[0].StatusSpecified = true;
            subscriber.Lists[0].IDSpecified = true;
            
            
            return SaveSubscriber(subscriber, out RequestID, out ResultStr);
        }

        public ExactTargetClientS4.CreateResult SaveSubscriber(ExactTargetClientS4.Subscriber subscriber, out string RequestID, out string ResultStr)
        {
            ExactTargetClientS4.SoapClient client = GetDefaultClient();
            
            //Create an Array of Lists
            if (subscriber.ID == 0)
            {
                subscriber.Lists = new ExactTargetClientS4.SubscriberList[]{new ExactTargetClientS4.SubscriberList()};//If a list is not specified the Subscriber will be added to the "All Subscribers" List
                subscriber.Lists[0].ID = LID;
                subscriber.Lists[0].IDSpecified = true;
                subscriber.Lists[0].Status = ExactTargetClientS4.SubscriberStatus.Active;
                subscriber.Lists[0].StatusSpecified = true;
                subscriber.Lists[0].IDSpecified = true;
                subscriber.SubscriberKey = subscriber.EmailAddress;

                //add ChannelMemberID through attribute
                //subscriber.Attributes = new ExactTargetClientS4.Attribute[1];
                //subscriber.Attributes[0] = new ExactTargetClientS4.Attribute();
                //subscriber.Attributes[0].Name = "ChannelMemberID";
                //subscriber.Attributes[0].Value = MID;
            }
            
            ExactTargetClientS4.CreateOptions co = new ExactTargetClientS4.CreateOptions();
            co.SaveOptions = new ExactTargetClientS4.SaveOption[1];
            co.SaveOptions[0] = new ExactTargetClientS4.SaveOption();
            co.SaveOptions[0].SaveAction = ExactTargetClientS4.SaveAction.UpdateAdd;//This set this call to act as an UpSert, meaning if the Subscriber doesn't exist it will Create if it does it will Update
            co.SaveOptions[0].PropertyName = "*";

            


            ExactTargetClientS4.CreateResult[] createResults = client.Create(co, new ExactTargetClientS4.APIObject[] { subscriber }, out RequestID, out ResultStr);

            if (ResultStr.Contains("Error"))
                throw new Exception(ResultStr);

            if (createResults != null && createResults.Length > 0)
                return createResults[0];
            return null;
        }

        public ExactTargetClientS4.CreateResult SaveSubscriberWithList(ExactTargetClientS4.Subscriber subscriber, int list, out string RequestID, out string ResultStr) {

            
            ExactTargetClientS4.CreateResult result;

            ExactTargetClientS4.ListSubscriber[] userLists = GetListsBySubscriber(subscriber);


            if (subscriber.ID == 0)
            {
                result = SaveSubscriberNoListing(subscriber, out RequestID, out ResultStr);
                if (subscriber.ID == 0)
                    return result;
            }
            
            
            ExactTargetClientS4.SubscriberList newlist = new ExactTargetClientS4.SubscriberList();

            newlist.ID = list;

            newlist.IDSpecified = true;

            newlist.Status = ExactTargetClientS4.SubscriberStatus.Active;

            subscriber.Lists = new ExactTargetClientS4.SubscriberList[]{newlist};

            result = SaveSubscriber(subscriber,out RequestID,out ResultStr);

            return result;
        }

        public ExactTargetClientS4.CreateResult SaveSubscriberNoListing(ExactTargetClientS4.Subscriber subscriber, out string RequestID, out string ResultStr)
        {
            ExactTargetClientS4.SoapClient client = GetDefaultClient();
                       

            ExactTargetClientS4.CreateOptions co = new ExactTargetClientS4.CreateOptions();
            co.SaveOptions = new ExactTargetClientS4.SaveOption[1];
            co.SaveOptions[0] = new ExactTargetClientS4.SaveOption();
            co.SaveOptions[0].SaveAction = ExactTargetClientS4.SaveAction.UpdateAdd;//This set this call to act as an UpSert, meaning if the Subscriber doesn't exist it will Create if it does it will Update
            co.SaveOptions[0].PropertyName = "*";




            ExactTargetClientS4.CreateResult[] createResults = client.Create(co, new ExactTargetClientS4.APIObject[] { subscriber }, out RequestID, out ResultStr);

            if (ResultStr.Contains("Error"))
                throw new Exception(ResultStr);

            if (createResults != null && createResults.Length > 0)
                return createResults[0];
            return null;
        }


        public ExactTargetClientS4.APIObject RetrieveWithFilter(string objectType, string primaryKey, string filterValue, string[] properties, out string RequestID, out string ResultStr)
        {
            ExactTargetClientS4.APIObject[] objects = RetrieveListWithFilter(objectType, primaryKey, filterValue, properties, out RequestID, out ResultStr);
            if (objects != null && objects.Length>0)
                return objects[0];
            return null;
        }

        public ExactTargetClientS4.APIObject[] RetrieveListWithFilter(string objectType, string primaryKey, string filterValue, string[] properties, out string RequestID, out string ResultStr)
        {
            try
            {
                ExactTargetClientS4.SoapClient client = GetDefaultClient();
                //Retrieve Subscriber

                //Local variables
                cfacore.external.service.ExactTargetClientS4.APIObject[] Results;
                
                

                // Instantiate the retrieve request
                cfacore.external.service.ExactTargetClientS4.RetrieveRequest rr = new cfacore.external.service.ExactTargetClientS4.RetrieveRequest();
                rr.ObjectType = objectType;//required

                // Setting up a simple filter
                cfacore.external.service.ExactTargetClientS4.SimpleFilterPart sf = new cfacore.external.service.ExactTargetClientS4.SimpleFilterPart();
                sf.SimpleOperator = cfacore.external.service.ExactTargetClientS4.SimpleOperators.equals;
                sf.Property = primaryKey;
                sf.Value = new String[] { filterValue };

                //Add Filter
                rr.Filter = sf;

                rr.Properties = properties;//required //Adding "ID" triggers all of the Subscriber Attributes to be returned

                

                ResultStr = client.Retrieve(rr, out RequestID, out Results);
                
                if (ResultStr.Contains("Error"))
                    throw new Exception(ResultStr);
                if (Results == null || Results.Length == 0)
                    return null;
                return Results;
            }
            catch (Exception ex)
            {
                RequestID = string.Empty;
                ResultStr = string.Empty;
                throw new Exception(ex.Message);
                
            }
        }

        public bool SubscriberExists(string email)
        {
            String requestID;
            String status;

            ExactTargetClientS4.APIObject result = RetrieveWithFilter("Subscriber", "EmailAddress", email,
                new string[] { "ID" },
                out requestID, out status);

            ExactTargetClientS4.Subscriber res = result as ExactTargetClientS4.Subscriber;
            return res != null && res.ID != 0;
        }

        static string BrightWaveToAes(string toEncrypt)
        {
            return encryptStringToString_AES(toEncrypt, Salt, InitializationVector);
        }

        static string BrightWaveFromAes(string toDecrypt)
        {
            return decryptStringFromString_AES(toDecrypt, Salt, InitializationVector);
        }

        public string GetEmailFromHex(string hex)
        {
            // do salting here

            return DecodeFrom64(hex);
        }

        public string GetHexFromEmail(string email)
        {
            // do salting here
            string escapedEmail = EncodeTo64(email);
            return escapedEmail;
        }


        #region Encryption

        public static string Salt = "0A1B2C3D4E5F6789";//"im9Mh1TK";
        public static string InitializationVector = "0A1B2C3D4E5F67890A1B2C3D4E5F6789";//"kOzlIgdiis";
        public static string Password = "p@ssw0rd";

        public static string EncodeTo64(string asciiString)
        {
            string hex = "";
            foreach (char c in asciiString)
            {
                int tmp = c;
                hex += String.Format("{0:x2}", (uint)System.Convert.ToUInt32(tmp.ToString()));
            }
            return hex;
        }

        
        public static string DecodeFrom64(string hex)
        {

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i <= hex.Length - 2; i += 2)
            {

                sb.Append(Convert.ToString(Convert.ToChar(Int32.Parse(hex.Substring(i, 2),

                System.Globalization.NumberStyles.HexNumber))));

            }

            return sb.ToString();

        }


        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        static string encryptStringToString_AES(string plainText, string Key, string IV)
        {
            return GetString(encryptStringToBytes_AES(plainText, GetBytes(Salt), GetBytes(InitializationVector)));
        }

        static byte[] encryptStringToBytes_AES(string plainText, string Key, string IV) {
            return encryptStringToBytes_AES(plainText, GetBytes(Salt),GetBytes(InitializationVector));
        }

        static byte[] encryptStringToBytes_AES(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");

            // Declare the streams used
            // to encrypt to an in memory
            // array of bytes.
            MemoryStream msEncrypt = null;
            CryptoStream csEncrypt = null;
            StreamWriter swEncrypt = null;

            // Declare the RijndaelManaged object
            // used to encrypt the data.
            RijndaelManaged aesAlg = null;

            try
            {
                // Create a RijndaelManaged object
                // with the specified key and IV.
                aesAlg = new RijndaelManaged();
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                
                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                msEncrypt = new MemoryStream();
                csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
                swEncrypt = new StreamWriter(csEncrypt);

                //Write all data to the stream.
                swEncrypt.Write(plainText);

            }
            finally
            {
                // Clean things up.

                // Close the streams.
                if (swEncrypt != null)
                    swEncrypt.Close();
                if (csEncrypt != null)
                    csEncrypt.Close();
                if (msEncrypt != null)
                    msEncrypt.Close();

                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            // Return the encrypted bytes from the memory stream.
            return msEncrypt.ToArray();

        }
        
        static string decryptStringFromString_AES(string cipherText, string Key, string IV)
        {
            return decryptStringFromBytes_AES(GetBytes(cipherText), GetBytes(Key), GetBytes(IV));
        }
        
        static string decryptStringFromBytes_AES(byte[] cipherText, string Key, string IV) {
            return decryptStringFromBytes_AES(cipherText, GetBytes(Key), GetBytes(IV));
        }

        static string decryptStringFromBytes_AES(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");

            // TDeclare the streams used
            // to decrypt to an in memory
            // array of bytes.
            MemoryStream msDecrypt = null;
            CryptoStream csDecrypt = null;
            StreamReader srDecrypt = null;

            // Declare the RijndaelManaged object
            // used to decrypt the data.
            RijndaelManaged aesAlg = null;

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            try
            {
                // Create a RijndaelManaged object
                // with the specified key and IV.
                aesAlg = new RijndaelManaged();
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                msDecrypt = new MemoryStream(cipherText);
                csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                srDecrypt = new StreamReader(csDecrypt);

                // Read the decrypted bytes from the decrypting stream
                // and place them in a string.
                plaintext = srDecrypt.ReadToEnd();
            }
            finally
            {
                // Clean things up.

                // Close the streams.
                if (srDecrypt != null)
                    srDecrypt.Close();
                if (csDecrypt != null)
                    csDecrypt.Close();
                if (msDecrypt != null)
                    msDecrypt.Close();

                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            return plaintext;

        }

        
        #endregion Encryption

    }

    
}