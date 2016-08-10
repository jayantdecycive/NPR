#region include

using System.Collections.ObjectModel;
using System.Diagnostics;
using Omu.ValueInjecter;
using cfacore.shared.domain.user;
using cfares.domain._event;
using cfares.domain.user;
using cfares.repository.slot;
using cfares.site.modules.com.reservations.npr;
using cfares.site.modules.mail;
using cfares.site.modules.repository.ticket;
using cfaresv2.Areas.npr.Models;
using cfaresv2.Controllers;
using Microsoft.Data.Edm.Validation;
using npr.domain._event.ticket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using cfacore.shared.modules.helpers;
using System.Text;
using System.IO;
using System.Xml;
using System.Security.Cryptography;
using System.Net;
using cfares.repository.ticket;
using cfares.entity.dbcontext.res_event;
using cfares.site.modules.com.application;
using cfares.domain._event.ticket;
using npr.entity.dbcontext.npr_res;
using System.Data.SqlClient;
using System.Web.UI;

#endregion

namespace cfaresv2.Areas.npr.Controllers
{
    public class ReservationController : ReservationControllerBase
    {
        #region on executing

        ResTicketRepository<NPRTicket> repo;
        SlotRepository<Slot> slotRepo;
        TicketGuestsRepository guestRepo;

        //protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        //{
        //    IResContext context = ReservationConfig.GetContext();

        //    base.Initialize(requestContext);
        //}

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            repo = new ResTicketRepository<NPRTicket>(ResContext);
            guestRepo = new TicketGuestsRepository(ResContext);
            slotRepo = new SlotRepository<Slot>(ResContext);

            if (getSubDomain(Request.Url) == null || getSubDomain(Request.Url).ToLower() == "events" || Request.Url.ToString().ToLower().Contains("events"))
                ViewBag.Subdomain = "events"; //getSubDomain(Request.Url).ToLower();
        }

        #endregion

        //=====================================================================================
        //REGISTER

        public ActionResult Register(int? id)
        {

            if (id != null)
            {
                //edit existing ticket
                NPRTicket ticket = GetTicket(id) as NPRTicket;
                var wiz = InitializeWizard("Reservation.Registration", ticket == null ? GetTicket(Request) : ticket) as ReservationWizard;
                wiz.Prime(ticket);

                //max guest size list
                var slot = GetSlot(ticket.SlotId);
                if (!ticket.IsSpecialtyTicket)
                    ViewBag.TicketsAvailable = ticket.GroupSize + slot.TicketsAvailable;

                return View(ticket);
            }
            else
            {
                NPRTicket ticket;
                if (Request["type"] == "custom")
                    ticket = new NPRTicket(true);
                else
                    ticket = GetTicket(id) as NPRTicket;



                int slotId = Convert.ToInt32(Request["slotId"]);
                int size = Convert.ToInt32(Request["GroupSize"]);

                if (slotId == 0 && !ticket.IsSpecialtyTicket)
                    return RedirectToAction("Index", "Home");
                if (size == 0)
                    size = 1;

                ticket.Owner = new cfares.domain.user.ResUser() { BirthDay = DateTime.Parse("1/1/1900") };
                ticket.GroupSize = size;
                ticket.SlotId = slotId;

                var slot = GetSlot(slotId);
                if (slot != null)
                {
                    ticket.Slot = slot;

                    var resEvent = slot.GetEvent();
                    if (resEvent != null)
                    {
                        ticket.IsPaid = resEvent.IsPaid;
                        ticket.TicketAmount = resEvent.TicketAmount;
                    }
                }
                ticket.CreatedDate = DateTime.Now;
                ticket.VisitorOfWebsite = false;
                ticket.AllocatedCapacity = 0;

                //max guest size list
                if (!ticket.IsSpecialtyTicket)
                    ViewBag.TicketsAvailable = slot.TicketsAvailable;
                //repo.Add(ticket);
                //repo.Commit();
                var wiz = InitializeWizard("Reservation.Registration", ticket == null ? GetTicket(Request) : ticket) as ReservationWizard;
                return View(ticket);
            }
        }

        [HttpPost]
        public ActionResult Register(int? id, NPRTicket postedTicket, FormCollection collection)
        {
            NPRTicket ticket = GetTicket(id) as NPRTicket;
            if (ticket == null)
            {
                ModelState.AddModelError("", string.Format("Unable to locate ticket with confirmation # {0}", id));
                return View(postedTicket);
            }

            var date1 = collection["Dates[1]"];
            //IValueInjecter injecter = new ValueInjecter();
            //injecter.Inject(ticket, postedTicket);      
            ticket.ContactPreference = postedTicket.ContactPreference;
            ticket.Phone = postedTicket.Phone;
            ticket.OwnerHomePhone = postedTicket.OwnerHomePhone;
            ticket.GroupSize = postedTicket.GroupSize;
            ticket.Dates = postedTicket.Dates;
            ticket.GuestList = postedTicket.GuestList;
            ticket.GroupType = postedTicket.GroupType;
            ticket.IsSpecialtyTicket = postedTicket.IsSpecialtyTicket;
            ticket.IsPaid = false;
            ticket.ListenToNprStation = postedTicket.ListenToNprStation;
            ticket.VisitorOfWebsite = postedTicket.VisitorOfWebsite;
            ticket.Age = postedTicket.Age;
            ticket.Race = postedTicket.Race;
            ticket.TopicsOfInterest = postedTicket.TopicsOfInterest;

            var slot = GetSlot(postedTicket.SlotId);

            //if (slot != null)
            //{
            //    var p = slot.Tickets.Where(o => o.SlotId == slot.SlotId && o.Status == TicketStatus.Partial).ToList();
            //    var r = slot.Tickets.Where(o => o.SlotId == slot.SlotId && o.Status == TicketStatus.Reserved).ToList();
            //    if (slot.TicketsAvailable > slot.Capacity-p.Count)
            //    {
            //        ModelState.AddModelError("", "ticket has been booked");
            //        return View(postedTicket);
            //    }
            //}


            if (slot != null)
            {
                ticket.Slot = slot;
                var resEvent = slot.GetEvent();
                if (resEvent != null && resEvent.IsPaid)
                {
                    ticket.IsPaid = true;
                    ticket.TicketAmount = resEvent.TicketAmount;
                    ticket.TotalAmount = ticket.TicketAmount.Value * (decimal)ticket.GroupSize;
                    //    ticket.Status = TicketStatus.Reserved;
                }
            }


            if (ticket.IsSpecialtyTicket && String.IsNullOrWhiteSpace(ticket.DatesString))
            {
                ModelState.AddModelError("", "Must select at least one preferred date");
                return View(postedTicket);
            }

            try
            {
                if (ticket.Owner != null && ticket.Owner.BirthDay.Year <= 1900)
                    ticket.Owner.BirthDay = DateTime.Parse("1/1/1900");

                ResUser existingUser = UserMembershipRepo.Find(x => x.Email == postedTicket.Owner.Email);
                if (existingUser != null)
                {
                    existingUser.FirstName = postedTicket.Owner.FirstName;
                    existingUser.LastName = postedTicket.Owner.LastName;
                    existingUser.HomePhoneString = postedTicket.Owner.HomePhoneString;

                    UserMembershipRepo.Edit(existingUser);
                    UserMembershipRepo.Commit();

                    ticket.Owner = existingUser;
                    ticket.OwnerId = existingUser.UserId;
                }
                else
                {
                    MembershipCreateStatus status;
                    string pass;
                    UserAccount<ResUser> nu = UserMembershipRepo.CreateUser(
                        postedTicket.Owner, out status, out pass);
                    ticket.Owner = nu.ApplicationUser;
                    ticket.OwnerId = nu.ApplicationUser.UserId;
                }

                if (!postedTicket.IsSpecialtyTicket)
                    ticket.Slot = GetSlot(postedTicket.SlotId);
                else
                    ticket.SlotId = null;

                if (ticket.TicketId == 0)
                    repo.Add(ticket);
                else
                    repo.Edit(ticket);

                repo.Commit();

                TicketGuests ticketguest;

                foreach (var guestname in postedTicket.TicketGuestList)
                {
                    ticketguest = new TicketGuests();

                    ticketguest.TicketId = ticket.TicketId;
                    ticketguest.GuestName = guestname;
                    guestRepo.Add(ticketguest);
                    guestRepo.Commit();

                }
                //var slot1 = GetSlot(postedTicket.SlotId);
                //if (slot1.TicketsAvailable <= 0)
                //{
                //    ModelState.AddModelError("", "ticket has been booked");
                //    return View(postedTicket);
                //}

                var wiz = InitializeWizard("Reservation.Registration", ticket == null ? GetTicket(Request) : ticket) as ReservationWizard;
                var a = ticket;
                wiz.Prime(ticket);

                if (ticket.IsPaid) return Redirect(wiz.GetStep("Reservation.Payment").Uri(ControllerContext));
                else return Redirect(wiz.GetStep("Reservation.Review").Uri(ControllerContext));

            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) ex = ex.InnerException;
                ModelState.AddModelError("", "Error: " + ex.Message);
                return View(ticket);
            }
        }

        //=====================================================================================
        //OVERVIEW

        public ActionResult Overview(int? id)
        {
            if (id == null) { return RedirectToAction("Register"); }
            NPRTicket ticket = GetTicket(id) as NPRTicket;
            var wiz = InitializeWizard("Reservation.Review", ticket == null ? GetTicket(Request) : ticket) as ReservationWizard;
            wiz.Prime(ticket);
            return View(ticket);
        }

        [HttpPost]
        public ActionResult Overview(int? id, NPRTicket ticket, FormCollection collection)
        {
            try
            {
                ModelState.Clear();

                Debug.Assert(id != null, "id != null");
                ticket = (NPRTicket)repo.Find(id.Value);
                if (ticket.CardNumber == null) ticket.CardNumber = Guid.NewGuid().ToString();

                // User is "Completing the Reservation" at this point .. since the reservation
                // .. could be modified, ensure the ticket at this stage is marked as partial
                // .. first and then move status to reserved.  This ensures that activation emails 
                // .. are triggered and modified reservations receive notifications per NPR-509

                ticket.Status = TicketStatus.Partial;
                repo.Commit();

                ticket.Status = TicketStatus.Reserved;
                repo.Commit();

                ReservationWizard wiz = InitializeWizard("Reservation.Review", ticket == null ? GetTicket(Request) : ticket) as ReservationWizard;
                wiz.Prime(ticket);
                return Redirect(wiz.NextStep.Uri(ControllerContext));
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return RedirectToAction("Message", "Home");
            }
        }

        //=====================================================================================
        //OVERVIEW

        public ActionResult Payment(int? id)
        {
            if (id == null) { return RedirectToAction("Register"); }
            NPRTicket ticket = GetTicket(id) as NPRTicket;

            var wiz = InitializeWizard("Reservation.Payment", ticket == null ? GetTicket(Request) : ticket) as ReservationWizard;
            wiz.Prime(ticket);

            return View(ticket);
        }

        [HttpPost]
        public ActionResult Payment(int? id, NPRTicket ticket, FormCollection collection)
        {
            // NPRTicket ticket1 = GetTicket(id) as NPRTicket;
            //var a = ticket1.GroupSize;
            try
            {
                ModelState.Clear();

                Debug.Assert(id != null, "id != null");
                ticket = (NPRTicket)repo.Find(id.Value);

                var slot = GetSlot(ticket.SlotId);

                if (slot != null)
                {
                    var r = slot.Tickets.Where(o => o.SlotId == slot.SlotId && o.Status == TicketStatus.Reserved).ToList();

                    if (slot.Capacity == r.Count)
                    {
                        ModelState.AddModelError(string.Empty, "Sorry, there are no tickets available at this time. Please check back for possible cancellations.");
                        return View(ticket);
                    }

                    //ticket.Status = TicketStatus.Reserved;
                    //repo.Commit();

                    if (slot.Capacity < r.Count)
                    {
                        ticket.Status = TicketStatus.Partial;
                        repo.Commit();
                        ModelState.AddModelError(string.Empty, "Sorry, there are no tickets available at this time. Please check back for possible cancellations.");
                        return View(ticket);
                    }
                    //   var lastRecord = slot.Tickets.Where(o => o.SlotId == slot.SlotId && o.Status == TicketStatus.Reserved).OrderByDescending(o => o.Status).FirstOrDefault();
                    ticket.Status = TicketStatus.Partial;
                    if (slot.Capacity - 1 == r.Count)
                    {
                        string query = "update Tickets set Status = 2 where TicketId =" + ticket.TicketId;
                        DOSQL(query);
                    }
                }

                //Get Payment Into

                string ccName = ticket.CCName = collection["CCName"];
                string ccNumber = ticket.CCNumber = collection["CCNumber"];
                string ccType = ticket.CCType = collection["CCType"];
                int expYear = collection["CCExpDateYear"].ToInt();
                ticket.CCExpDateYear = expYear;
                int expMonth = collection["CCExpDateMonth"].ToInt();
                ticket.CCExpDateMonth = expMonth;
                decimal amount = ticket.TotalAmount ?? ticket.TicketAmount ?? 0M;
                string ccEmail = ticket.Email;

                //Validate Payment Information
                ticket.CCNumber = "xxxxxxxxxxxx" + ccNumber.PadRight(16).Substring(12, 4);
                string authResponse = "";
                bool paymentSuccessful = this.ProcessPayment(id.Value, amount, ccName, ccEmail, ccType, ccNumber, expYear, expMonth, out authResponse);

                //Save
                string errorMsg = "";
                if (authResponse.StartsWith("<"))
                {
                    authResponse = authResponse.Replace("<?xml version=\"1.0\" encoding=\"UTF-8\"?>", "");
                }
                else
                {
                    errorMsg = authResponse;
                    authResponse = "<TransactionResult><Error>" + authResponse + "</Error></TransactionResult>";
                }


                ticket.AuthorizationResponse = String.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<Transactions>{0}</Transactions>", authResponse);
                ticket.IsAuthSuccessful = false;
                repo.Commit();

                //If payment not successful, show error
                if (!paymentSuccessful)
                {
                    ticket.Status = TicketStatus.Partial;
                    repo.Commit();
                    string query = "update Tickets set Status = 0 where TicketId =" + ticket.TicketId;
                    DOSQL(query);
                    ModelState.AddModelError(string.Empty, errorMsg);
                    return View(ticket);
                }


                //    ticket.Status = TicketStatus.Partial;
                //  repo.Commit();

                //Else, proceed
                ticket.IsAuthSuccessful = true;

                ticket.Status = TicketStatus.Reserved;
                repo.Commit();



                var wiz = InitializeWizard("Reservation.Review", ticket == null ? GetTicket(Request) : ticket) as ReservationWizard;
                wiz.Prime(ticket);

                return Redirect(String.Concat(wiz.GetStep("Reservation.Success").Uri(ControllerContext), "#no-back"));
            }
            catch (Exception e)
            {
                ticket.Status = TicketStatus.Partial;
                repo.Commit();
                Console.Write(e.Message);
                return RedirectToAction("Message", "Home");
            }
        }

        private void DOSQL(string query)
        {
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.
ConnectionStrings["CfaResContext"].ConnectionString);
            SqlCommand myCommand = new SqlCommand(query, conn);
            try
            {
                conn.Open();
                myCommand.ExecuteNonQuery();
            }
            catch
            {
            }
            finally
            {
                conn.Close();
            }
        }

        public bool ProcessPayment(int TicketNumber, decimal Amount, string BillingName, string CustomerEmail, string CCType, string CCNumber, int ExpYear, int ExpMonth, out string AuthResponse)
        {
            AuthResponse = "";

            //Build ExpDate
            var expDate = String.Format("{0}{1}", ExpMonth.ToString().PadLeft(2, '0'), ExpYear.ToString().Substring(2));

            StringBuilder apiRequestBuilder = new StringBuilder();
            using (StringWriter apiRequestWriter = new StringWriter(apiRequestBuilder))
            {
                using (XmlTextWriter apiXmlWriter = new XmlTextWriter(apiRequestWriter))
                {
                    //build XML string
                    apiXmlWriter.Formatting = Formatting.Indented;
                    apiXmlWriter.WriteStartElement("Transaction");
                     apiXmlWriter.WriteElementString("ExactID", "B21073-01");//Live Gateway ID
                    //apiXmlWriter.WriteElementString("ExactID", "AH7045-01");//Demo Gateway ID
                    // apiXmlWriter.WriteElementString("Password", "welcome3");//Password
                    apiXmlWriter.WriteElementString("Password", "welcome3");//Live Password
                    apiXmlWriter.WriteElementString("Transaction_Type", "00");
                    apiXmlWriter.WriteElementString("DollarAmount", Amount.ToString("N2"));
                    apiXmlWriter.WriteElementString("Expiry_Date", expDate);
                    apiXmlWriter.WriteElementString("CardHoldersName", BillingName);
                    apiXmlWriter.WriteElementString("Card_Number", CCNumber);
                    apiXmlWriter.WriteElementString("Reference_No", TicketNumber.ToString());
                    apiXmlWriter.WriteElementString("Client_Email", CustomerEmail);


                    //apiXmlWriter.WriteElementString("VerificationStr2", "123");
                    apiXmlWriter.WriteEndElement();
                    String xmlString = apiRequestBuilder.ToString();

                    //SHA1 hash on XML string
                    ASCIIEncoding encoder = new ASCIIEncoding();
                    byte[] bytesXml = encoder.GetBytes(xmlString);
                    SHA1CryptoServiceProvider sha1_crypto = new SHA1CryptoServiceProvider();
                    string hash = BitConverter.ToString(sha1_crypto.ComputeHash(bytesXml)).Replace("-", "");
                    string hashed_content = hash.ToLower();


                    // FOR STAGING 
               /*    string method = "POST\n";
                       string type = "text/xml\n";//REST XML
                       string time = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
                       string url = "/transaction/v12";
                       string keyID = "215369";//key ID
                       string key = "m1i7veRp4pmHm0oXzz73AQtTFZx1LLBn";//Hmac key
                    */
                     

                    // FOR Live PRODUCTION
                        string method = "POST\n";
                          string type = "text/xml\n";//REST XML
                          string time = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
                          string url = "/transaction/v12";
                          string keyID = "119693";//key ID
                          //string key = "aVwkDsTiqKUDhCVXjgBt_kQ6uJfddh~v";//Hmac key  OLD KEY WAS zPV2cZcndkzv129awcKiZa6HrRhTFv6Y
                          string key = "zPV2cZcndkzv129awcKiZa6HrRhTFv6Y";
                     

                    //New Account Try
             /*       string method = "POST\n";
                    string type = "text/xml\n";//REST XML
                    string time = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
                    string url = "/transaction/v12";
                    string keyID = "221828";//key ID
                    string key = "VGg5yXlovJQfi7x0g7MLGSZ8j2E=";//Hmac key
*/
                    string hash_data = method + type + hashed_content + "\n" + time + "\n" + url;
                    //hmac sha1 hash with key + hash_data
                    HMAC hmac_sha1 = new HMACSHA1(Encoding.UTF8.GetBytes(key)); //key
                    byte[] hmac_data = hmac_sha1.ComputeHash(Encoding.UTF8.GetBytes(hash_data)); //data
                    //base64 encode on hmac_data
                    string base64_hash = Convert.ToBase64String(hmac_data);

                    string uri = "https://api.globalgatewaye4.firstdata.com/transaction/v12"; //Live API Endpoint
                                  

                    //string uri = "https://api.demo.globalgatewaye4.firstdata.com/transaction "; //DEMO API Endpoint

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

                        if (node != null && node.InnerText == "false")
                        {
                            AuthResponse = xmldoc.SelectSingleNode("//Bank_Message").InnerText;

                            return false;
                        }

                        else if (node != null && node.InnerText == "true")
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



        //=====================================================================================
        //CONFIRM/CANCEL

        public ActionResult Confirm(int? id)
        {
            if (id == null) { return RedirectToAction("Overview"); }
            try
            {
                NPRTicket ticket = GetTicket(id) as NPRTicket;
                var wiz = InitializeWizard("Reservation.Success", ticket == null ? GetTicket(Request) : ticket) as ReservationWizard;
                return View(ticket);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return RedirectToAction("Message", "Home");
            }
        }

        public ActionResult Cancel(int? id)
        {
            if (id != null) { RedirectToAction("Index", "Home"); }

            NPRTicket ticket = GetTicket(id) as NPRTicket;
            if (ticket == null) throw new ApplicationException(string.Format(
               "Invalid ticket [ Confirmation #{0} ]", id));


            if (ticket.Slot.Occurrence.ResEvent.ReservationTypeId == "SpecialEvent")
            {
                new NprEmailService().SendNprEventReservationCancellation(ticket);
            }
            else
            {
                new NprEmailService().SendNprReservationCancellation(ticket);
            }


            if (ticket.ContactNotes == null) ticket.ContactNotes = new List<string>();
            ticket.ContactNotes.Add(string.Format("Ticket canceled on [ {0} ] by user [ {1} ]",
                DateTimeOffset.Now, AppContext.User == null ? "Anonymous" : AppContext.User.Username));
            ticket.Status = TicketStatus.Canceled;
            ticket.CanceledDate = DateTime.Now;
            repo.Edit(ticket);
            repo.Commit();

            return View();
        }

        //=====================================================================================
        //MODIFY

        public ActionResult Modify()
        {
            return View(new ModifyTicketViewModel());
        }

        [HttpPost]
        public ActionResult Modify(FormCollection collection, ModifyTicketViewModel model, string Cancel, string ConfirmNumber, string Email)
        {
            try
            {
                var Message = "";
                //if (Cancel != null)
                //{
                //    NprContext db = new NprContext();
                //    var s = db.Tickets.Where(o => o.CardNumber.Substring(0, 8).Equals(model.ConfirmNumber, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                //    if (s != null)
                //    {
                //        db.Tickets.D
                //        return RedirectToAction("Tours", "Home");
                //    }
                //    else {
                //        ViewBag.Message = "Ticket not Found";
                //        return View();
                //    }

                // }
                NPRTicket ticket = (NPRTicket)repo.Find(x => x.CardNumber.Substring(0, 8)
                    .Equals(model.ConfirmNumber, StringComparison.OrdinalIgnoreCase));
                if (ticket == null || ticket.OwnerId == null)
                {
                    ViewBag.Message = "Ticket not Found";
                    Message = "Ticket not Found";
                    return Json(new { Message }, JsonRequestBehavior.AllowGet);
                    //return View();
                }

                //var paid = ticket.TicketAmount;
                if (Cancel != null)
                {
                    if (ticket.IsPaid)
                    {
                        ViewBag.Message = "We are sorry there are no refunds or changes for online transactions. Please contact NPR Events at  202-513-2750 with any questions or concerns.";
                        return View();
                    }
                    else
                    {
                        repo.Delete(ticket);
                        repo.Context.SaveChanges();                       
                        return RedirectToAction("Tours", "Home");
                    }
                }
                if (ticket.IsPaid)
                {

                    ViewBag.Message = "Because you have a paid ticket, to cancel or change your reservation, please email us at events@npr.org or call 202-513-2031.";
                }
                else
                {
                    ticket.Owner = UserMembershipRepo.Find(ticket.OwnerId.Value) as ResUser;
                    if (ticket.Owner.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase)
                        && ticket.Status != TicketStatus.Canceled
                        && ticket.Status != TicketStatus.Partial)
                    {
                        ViewBag.Page = "Modify";
                        return RedirectToAction("Register", new { id = ticket.TicketId });
                    }
                    ViewBag.Message = "Ticket not Found";
                }
                return View();
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                ViewBag.Message = "Problem finding your ticket.";
                return View();
            }
        }


        public ActionResult TicketList(int? id)
        {
            if (id == null) { return RedirectToAction("Index", "Home"); }
            try
            {
                NPRTicket ticket = GetTicket(id) as NPRTicket;

                // get and check ticket guests
                var guests = guestRepo.GetByTicket((Ticket)ticket);
                if (null != guests)
                {
                    ViewBag.Guests = guests.ToList();
                }
                else
                {
                    ViewBag.Guests = "";
                }

                return View(ticket);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return RedirectToAction("Message", "Home");
            }
        }

        public ActionResult ReprintTickets(string ConfirmNumber)
        {
            var Message = "";
            try
            {
                NPRTicket ticket = (NPRTicket)repo.Find(x => x.CardNumber.Substring(0, 8)
                    .Equals(ConfirmNumber, StringComparison.OrdinalIgnoreCase));
                if (ticket == null || ticket.OwnerId == null)
                {
                    Message = "Ticket not Found";
                    return Json(new { Message }, JsonRequestBehavior.AllowGet);
                }
                Message = "sucess";
                int aId = ticket.TicketId;
                return Json(new { Message, aId }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                Message = "Problem finding your ticket.";
                return Json(new { Message }, JsonRequestBehavior.AllowGet);
            }

        }

        //=====================================================================================
        //UTILS

        #region utils

        private ITicket GetTicket(System.Web.HttpRequestBase Request)
        {
            if (Request == null || string.IsNullOrEmpty(Request.QueryString["ticket"]))
                return new NPRTicket();

            return repo.Find(int.Parse(Request.QueryString["ticket"]));
        }

        private ITicket GetTicket(int? id)
        {
            if (id == null)
                return new NPRTicket();
            var ticketRepo = new ResTicketRepository<NPRTicket>(ResContext);
            ITicket ticket = ticketRepo.Find(id.Value);
            if (ticket.OwnerId != null)
                ticket.Owner = UserMembershipRepo.Find(ticket.OwnerId.Value) as ResUser;
            return ticket;
        }

        private Slot GetSlot(int? id)
        {
            if (id != null)
            {
                var slot = slotRepo.Find(id.Value);
                return (Slot)slot;
            }
            else
            {
                return new Slot();
            }

        }

        private Ticket GetTicketFrom(int? id)
        {
            if (id != null)
            {
                var tick = repo.Find(id.Value);
                return (Ticket)tick;
            }
            else
            {
                return new Ticket();
            }
        }

        private string getSubDomain(Uri url)
        {
            string host = Request.Url.Host;
            if (host.Split('.').Length > 1)
            {
                int index = host.IndexOf(".");
                return host.Substring(0, index);
            }
            return null;
        }

        #endregion

    }
}
