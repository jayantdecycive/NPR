
#region Imports

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Ninject;
using cfacore.shared.modules.helpers;
using cfares.domain._event;
using cfares.domain._event.slot;
using cfares.repository._event;
using cfares.repository.slot;
using cfares.repository.ticket;
using cfares.repository.user;
using cfares.site.modules.com.Security;
using cfares.site.modules.mail;
using cfaresv2.Areas.Admin.Controllers._base;
using npr.domain._event.slot;
using npr.domain._event.ticket;
using npr.entity.dbcontext.npr_res;
using System.Text;
using System.Xml;
using System.IO;
using System.Security.Cryptography;
using System.Net;
using cfares.domain.user;

#endregion

namespace cfaresv2.Areas.Admin.Controllers
{
    [ReservationSystemAuthorize(Area = "Admin", Roles = "Admin,Operator")]
    public class NPRTicketController : CrudController<TicketRepository<NPRTicket>, NPRTicket, NPRTicket, int,ITicket>
    {
        private const string TourTypeId = "Tour";
        public new ActionResult Request(int id)
        {
            NPRTicket ticket = (NPRTicket)serv.Find(x => x.TicketId == id, "Owner");
            return View(ticket);
        }

		public override ITicket Inject(int id, NPRTicket entity)
		{
            NPRTicket i = (NPRTicket)base.Inject(id, entity);
            if (i.OwnerId != null) i.Owner = new UserRepository(serv.Context).Find(i.OwnerId.Value);
            
			if(i.CardNumber == null) i.CardNumber = Guid.NewGuid().ToString();
            if(i.CardNumber == Guid.Empty.ToString()) i.CardNumber = Guid.NewGuid().ToString();
			return i;
		}

		public override void SetViewBag()
		{
			ViewBag.IgnoreParent = true;
			base.SetViewBag();
		}

		protected override void FormValid(
			ITicket entity, NPRTicket entityViewModel, FormCollection collection)
		{
			base.FormValid(entity, entityViewModel, collection);
			ModelState.Clear();
		}

        protected override Dictionary<string, Action<NPRTicket, string>> QueryInjector()
        {
            return new Dictionary<string,Action<NPRTicket,string>>
                {
                    {"SlotId",(x,y)=>x.SlotId=NullableInt(y)},
                    {"OwnerId",(x,y)=>x.OwnerId=NullableInt(y)},
                };
        }

        [HttpPost]
        public new ActionResult Request(int id,string timeId, FormCollection collection)
        {
            NPRTicket ticket = serv.Find(x=>x.TicketId==id,"Owner") as NPRTicket;
            string controllerSubmission = timeId;
            string did = "new";
            DateTime baseDate = DateTime.MinValue;
            if (controllerSubmission == "new")
                baseDate = DateTime.Parse(collection["new"]);
            else {
                long ticks = long.Parse(controllerSubmission);
                did = ticks.ToString(CultureInfo.InvariantCulture);
	            DateTime? dateTime = ticket.Dates.First(x => x != null && x.Value.Ticks == ticks);
	            if (dateTime != null)
		            baseDate = dateTime.Value;
            }
			if( baseDate == DateTime.MinValue )
				throw new ApplicationException( "Unable to retreive first selected date from ticket" );

			if( string.IsNullOrWhiteSpace( collection["start_" + did] ) )
				return View( ticket ).AddModelStateError( ModelState, "Start time is required" );

			if( string.IsNullOrWhiteSpace( collection["end_" + did] ) )
				return View( ticket ).AddModelStateError( ModelState, "End time is required" );

            DateTimeOffset start = DateTimeOffset.Parse(baseDate.ToShortDateString() + " " + collection["start_" + did]);
            DateTimeOffset end = DateTimeOffset.Parse(baseDate.ToShortDateString() + " " + collection["end_" + did]);

            ResEventRepository eventServ = new ResEventRepository(serv.Context);

            IResEvent campaign = eventServ.Find(x=>
				x.RegistrationStart < start && 
				x.RegistrationEnd > start && 
				x.ReservationTypeId == TourTypeId && 
				x.Status != ResEventStatus.Temp);

            if (campaign == null || campaign.Occurrences == null || campaign.Occurrences.Count == 0)
				return View( ticket ).AddModelStateError( ModelState, 
					string.Format("An event during the time period {0} - {1} " + 
						"must exist with at least one occurrence",start,end) );

            SlotRepository<NPRSlot> slotServ = new SlotRepository<NPRSlot>(eventServ.Context);
            
            ISlot newSlot = campaign.GetKernel().Get<ISlot>();
            newSlot.Visibility = SlotVisibility.Specialty;
            newSlot.Start = start;
            newSlot.End = end;
            newSlot.Cutoff = end;
	        newSlot.Notes = collection["note"];

            Occurrence oc = campaign.Occurrences.First();
            newSlot.OccurrenceId = oc.OccurrenceId;

            newSlot.SetOffset(oc.GMTOffset);
            
            slotServ.Add(newSlot as NPRSlot);
            slotServ.Commit();
            if (newSlot.SlotId == 0)
                throw new Exception("There was an error saving this slot.");

            ticket.SlotId = newSlot.SlotId;
            ticket.ContactNotes.Add( string.Format("Finalized on {0} from date pool [{1}]", DateTime.Now.ToShortDateString(),
                                        string.Join("|", ticket.Dates.Select(x => x != null ? x.Value.ToShortDateString() : null).ToArray())) );
            ticket.DatesString = null;
            ticket.Status=TicketStatus.Reserved;
            serv.Edit(ticket);
            serv.Commit();

			new NprEmailService().SendNprSpecialtyTourConfirmation(ticket);

            return RedirectToAction("Tours","Home");
        }

        public ActionResult Note(int id)
        {
            NPRTicket ticket = (NPRTicket)serv.Find(id);
            return View(ticket);
        }

        [HttpPost]
        public ActionResult Note(int id, NPRTicket ticket, FormCollection collection)
        {
	        if (ticket == null) throw new ArgumentNullException("ticket");
	        ticket = (NPRTicket)serv.Find(id);

			if( string.IsNullOrWhiteSpace( collection["Note"] ) )
			{
				ModelState.AddModelError( "", "Note content is required" );
				return View(ticket);
			}

            ticket.ContactNotes.Add(collection["Note"]);
            serv.Commit();

            return RedirectToAction("Request", "NPRTicket", new { id = id });
        }

        public override TicketRepository<NPRTicket> GetRepository(cfares.entity.dbcontext.res_event.IResContext context)
        {
            return new TicketRepository<NPRTicket>(context as NprContext);
        }

		[HttpPost]
		public override ActionResult Delete(int id, FormCollection collection)
		{
			//System.IO.File.AppendAllText(@"c:\npr-test.log", "Delete() - Enter\r\n");

			//Get Ticket
			var ticket = (NPRTicket)serv.Find(id);

			//Is Refund
			if(ticket.IsPaid)
			{
				//System.IO.File.AppendAllText(@"c:\npr-test.log", "Delete() - IsPaid\r\n");

				//Load Xml
				XmlDocument xml = new XmlDocument();
				xml.LoadXml(ticket.AuthorizationResponse);

				//TransactionTag
				string transactionTag = xml.SelectSingleNode("//Transaction_Tag").InnerText;

				//AuthorizationNumber
				string authorizationNumber = xml.SelectSingleNode("//Authorization_Num").InnerText;

				//Process Refund
				string authResponse = "";
				bool refundSuccessful = this.ProcessRefund(id, ticket.TotalAmount.Value, transactionTag, authorizationNumber, out authResponse);

				//Prepare Auth Response
				string errorMsg = authResponse;
				if(authResponse.StartsWith("<"))
				{ 
					authResponse = authResponse.Replace("<?xml version=\"1.0\" encoding=\"UTF-8\"?>", "");
				}
				else
				{
					errorMsg = authResponse;
					authResponse = "<TransactionResult><Authorization_Num>RETURN</Authorization_Num><Error>" + authResponse + "</Error></TransactionResult>";
				}

				//If payment not successful, show error
				if (!refundSuccessful)
				{
					//Update Ticket
					ticket.AuthorizationResponse = ticket.AuthorizationResponse.Replace("</Transactions>", authResponse+"</Transactions>");
					serv.Commit();

					//System.IO.File.AppendAllText(@"c:\npr-test.log", "!refundSuccessful - "+errorMsg+"\r\n");

					ModelState.AddModelError(string.Empty, errorMsg);
					return View(ticket);
				}
				else
				{
					//Update Ticket
					ticket.AuthorizationResponse = ticket.AuthorizationResponse.Replace("</Transactions>", authResponse+"</Transactions>");
					serv.Commit();

					//System.IO.File.AppendAllText(@"c:\npr-test.log", "Delete() - refund Successful, Deleting Message\r\n");

					var success = new NprEmailService().SendNprPaidEventRefundReceipt(ticket, force: true);

					if(!success)
					{
						ModelState.AddModelError(string.Empty, "Refund was issued, but unable to send email.");
                    }
                    return RedirectToAction("Index", new { message = RefundMessage(id) });
				}
			}
            else 
            {
                //Check Rights
                CheckDeleteRights(ticket);

			    //Process normal delete
			    return base.Delete(id, collection);                
            }
		}

		public bool ProcessRefund(int TicketNumber, decimal Amount, string TransactionTag, string AuthorizationNumber, out string AuthResponse)
		{
			AuthResponse = "";

			StringBuilder apiRequestBuilder = new StringBuilder();
			using (StringWriter apiRequestWriter = new StringWriter(apiRequestBuilder))
			{
				using (XmlTextWriter apiXmlWriter = new XmlTextWriter(apiRequestWriter))
				{
					//build XML string
					apiXmlWriter.Formatting = Formatting.Indented;
					apiXmlWriter.WriteStartElement("Transaction");
					apiXmlWriter.WriteElementString("ExactID", "B21073-01");//Gateway ID
					apiXmlWriter.WriteElementString("Password", "welcome3");//Password
					apiXmlWriter.WriteElementString("Transaction_Type", "34"); //Tagged Refund
					apiXmlWriter.WriteElementString("DollarAmount", Amount.ToString("N2"));
					apiXmlWriter.WriteElementString("Transaction_Tag", TransactionTag);
					apiXmlWriter.WriteElementString("Authorization_Num", AuthorizationNumber);
					
					
					//apiXmlWriter.WriteElementString("VerificationStr2", "123");
					apiXmlWriter.WriteEndElement();
					String xmlString = apiRequestBuilder.ToString();

					//SHA1 hash on XML string
					ASCIIEncoding encoder = new ASCIIEncoding();
					byte[] bytesXml = encoder.GetBytes(xmlString);
					SHA1CryptoServiceProvider sha1_crypto = new SHA1CryptoServiceProvider();
					string hash = BitConverter.ToString(sha1_crypto.ComputeHash(bytesXml)).Replace("-", "");
					string hashed_content = hash.ToLower();

					//assign values to hashing and header variables
					string method = "POST\n";
					string type = "text/xml\n";//REST XML
					string time = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
					string url = "/transaction/v12";
					string keyID = "119693";//key ID
					string key = "zPV2cZcndkzv129awcKiZa6HrRhTFv6Y";//Hmac key
					string hash_data = method + type + hashed_content + "\n" + time + "\n" + url;
					//hmac sha1 hash with key + hash_data
					HMAC hmac_sha1 = new HMACSHA1(Encoding.UTF8.GetBytes(key)); //key
					byte[] hmac_data = hmac_sha1.ComputeHash(Encoding.UTF8.GetBytes(hash_data)); //data
					//base64 encode on hmac_data
					string base64_hash = Convert.ToBase64String(hmac_data);
					string uri = "https://api.globalgatewaye4.firstdata.com/transaction/v12"; //DEMO API Endpoint
					//begin HttpWebRequest 
					HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
					request.Method = "POST";
					request.Accept = "application/xml";
					request.Headers.Add("x-gge4-date", time);
					request.Headers.Add("x-gge4-content-sha1", hashed_content);
					request.ContentLength = xmlString.Length;
					request.ContentType = "text/xml";
					request.Headers["authorization"] = "GGE4_API " + keyID + ":" + base64_hash;

					// send request as stream
					StreamWriter xml = null;
					xml = new StreamWriter(request.GetRequestStream());
					xml.Write(xmlString);
					xml.Close();

					//get response and read into string
					string response;
					try
					{
						HttpWebResponse web_response = (HttpWebResponse)request.GetResponse();
						using (StreamReader response_stream = new StreamReader(web_response.GetResponseStream()))
						{
							response = response_stream.ReadToEnd();
							response_stream.Close();
						}
						
						//load xml
						XmlDocument xmldoc = new XmlDocument();
						xmldoc.LoadXml(response);
						var node = xmldoc.SelectSingleNode("//Transaction_Approved");

						if(node != null && node.InnerText == "false")
						{
							AuthResponse = xmldoc.SelectSingleNode("//Bank_Message").InnerText;

							return false;
						}

						else if(node != null && node.InnerText == "true")
						{
							AuthResponse = response;

							return true;
						}

						return false;


					}
					catch (WebException ex)
					{
						if (ex.Response != null)
						{
							using (HttpWebResponse error_response = (HttpWebResponse)ex.Response)
							{
								using (StreamReader reader = new StreamReader(error_response.GetResponseStream()))
								{
									string remote_ex = reader.ReadToEnd();
									AuthResponse = remote_ex;
								}
							}
						}
						else
						{
							AuthResponse = ex.ToString();
						}


						return false;
					}
				}
			}
		}
    }
}
