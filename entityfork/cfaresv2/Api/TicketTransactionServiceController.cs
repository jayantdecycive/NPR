using System.Net;
using cfares.domain._event;
using cfares.entity.dbcontext.res_event;
using cfares.repository._event;
using cfares.repository.ticket;
using cfares.site.modules.com.application;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using cfares.site.modules.repository.ticket;

using npr.domain._event.ticket;
using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Web.Script.Services;

namespace cfaresv2.Api
{
    public class TicketTransactionServiceController : ApiController
    {
        TicketTransactionRepository repo;
        TicketGuestsRepository guestRepo;
        //TicketRepository ticketRepo;

        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            IResContext context = ReservationConfig.GetContext();
            repo = new TicketTransactionRepository(context);
            guestRepo = new TicketGuestsRepository(context);
            //ticketRepo = new TicketRepository(context);
            base.Initialize(controllerContext);
        }

        public class TicketTransactionRedeemRequest
        {
           // public string CardNumber;
            public int ResEventId;
            public int TicketGuestsId;
        }

        [HttpPost]
        public TicketTransaction Redeem1(TicketTransactionRedeemRequest redeemRequest)
        {
            //checks first 8 digits of guid for match
            const int max = 8;
            int maxSize = 1;
          //  string CardNumber = redeemRequest.CardNumber.Length > max ? redeemRequest.CardNumber.Substring(0, max) : redeemRequest.CardNumber;
            int ResEventId = redeemRequest.ResEventId;

            int TicketGuestId = redeemRequest.TicketGuestsId;

            // get ticket guests
            var guestRecord = guestRepo.GetAll().FirstOrDefault(x=>x.TicketGuestsId == TicketGuestId);

            // get ticket
            var ticketRepo = new TicketRepository(repo.Context);
            var ticket = ticketRepo.GetAll().Include("Owner").Include("Slot.Occurrence").FirstOrDefault(x => x.TicketId == guestRecord.TicketId && x.Slot.Occurrence.ResEventId == ResEventId);
            if (ticket == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            
            //get ticket
            //var ticketRepo = new TicketRepository(repo.Context);
            //var ticket = ticketRepo.GetAll().Include("Owner").Include("Slot.Occurrence").FirstOrDefault(x => x.CardNumber.Substring(0, max) == CardNumber && x.Slot.Occurrence.ResEventId == ResEventId);
            //if (ticket == null)
            //    throw new HttpResponseException(HttpStatusCode.NotFound);

            // get and check ticket transactions
            var transaction = repo.GetByTicket((Ticket)ticket);
            if (transaction == null || ticket.AllocatedCapacity == null) 
                throw new HttpResponseException(HttpStatusCode.NotFound);  

            // check if ticket has already been redeemed
            if(transaction.Count(x => x.Action == TicketTransactionAction.Redeeem) >= ticket.AllocatedCapacity)
                return new TicketTransaction();

            var newTransaction = new TicketTransaction()
            {
                TicketId = ticket.TicketId,
                CustomerId = ticket.OwnerId,
                Action = TicketTransactionAction.Redeeem,
                TicketGuestsId = TicketGuestId
            };
            repo.Add(newTransaction);
            repo.Commit();
            return newTransaction;



            /*
            //get most recent transaction
            if (transaction != null && transaction.Count(x=>x.Action == TicketTransactionAction.Redeeem) > 0)
                return transaction.OrderByDescending(x => x.CreateDate).First(x=>x.Action==TicketTransactionAction.Redeeem);
            
            //create new transaction if no existing transactions
            else{
                var newTransaction = new TicketTransaction(){
                        TicketId=ticket.TicketId,
                        CustomerId = ticket.OwnerId,
                        Action=TicketTransactionAction.Redeeem
                    };
                repo.Add(newTransaction);
                repo.Commit();
                return newTransaction;
            }
            */

        }


       
        //
       // [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         //[HttpPost]
        [HttpGet]
        public TicketTransaction Redeem(int ResEventId, int TicketGuestsId)
        {
            //checks first 8 digits of guid for match
            const int max = 8;
            int maxSize = 1;
            //  string CardNumber = redeemRequest.CardNumber.Length > max ? redeemRequest.CardNumber.Substring(0, max) : redeemRequest.CardNumber;
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);
                       
            // get ticket guests
            var guestRecord = guestRepo.GetAll().FirstOrDefault(x => x.TicketGuestsId == TicketGuestsId);

            // get ticket
           TicketRepository ticketRepo = new TicketRepository(repo.Context);
            var ticket = ticketRepo.GetAll().Include("Owner").Include("Slot.Occurrence").FirstOrDefault(x => x.TicketId == guestRecord.TicketId && x.Slot.Occurrence.ResEventId == ResEventId);

            if (ticket == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            //get ticket
            //var ticketRepo = new TicketRepository(repo.Context);
            //var ticket = ticketRepo.GetAll().Include("Owner").Include("Slot.Occurrence").FirstOrDefault(x => x.CardNumber.Substring(0, max) == CardNumber && x.Slot.Occurrence.ResEventId == ResEventId);
            //if (ticket == null)
            //    throw new HttpResponseException(HttpStatusCode.NotFound);

            // get and check ticket transactions
            var transaction = repo.GetByTicket((Ticket)ticket);
            if (transaction == null || ticket.AllocatedCapacity == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            // check if ticket has already been redeemed
            if (transaction.Count(x => x.Action == TicketTransactionAction.Redeeem) >= ticket.AllocatedCapacity)
            {
                  return new TicketTransaction();
              //  return  transaction;
            }
            var newTransaction = new TicketTransaction()
            {
                TicketId = ticket.TicketId,
                CustomerId = ticket.OwnerId,
                Action = TicketTransactionAction.Redeeem,
                TicketGuestsId = TicketGuestsId
            };
            repo.Add(newTransaction);
            repo.Commit();
         
        
         
            //   return JsonConvert.SerializeObject(newTransaction);
         //   return Request.CreateResponse(HttpStatusCode.OK, newTransaction);
            return newTransaction;



            /*
            //get most recent transaction
            if (transaction != null && transaction.Count(x=>x.Action == TicketTransactionAction.Redeeem) > 0)
                return transaction.OrderByDescending(x => x.CreateDate).First(x=>x.Action==TicketTransactionAction.Redeeem);
            
            //create new transaction if no existing transactions
            else{
                var newTransaction = new TicketTransaction(){
                        TicketId=ticket.TicketId,
                        CustomerId = ticket.OwnerId,
                        Action=TicketTransactionAction.Redeeem
                    };
                repo.Add(newTransaction);
                repo.Commit();
                return newTransaction;
            }
            */

        }

       /* [HttpGet]
        public TicketTransaction Redeem(string CardNumber, int ResEventId)
        {
            repo.Context.Configuration.ProxyCreationEnabled = false;
            var t = repo.GetAll().Include("Slot.Occurrence").FirstOrDefault(o => o.CardNumber == CardNumber && o.Slot.Occurrence.ResEventId == ResEventId);

        }*/
    }
}
