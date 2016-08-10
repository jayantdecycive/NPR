using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfares.site.modules.mail;
using cfares.domain.user;
using cfacore.site.controllers.shared;
using cfares.domain._event._ticket.tours;
using cfacore.site.controllers._event;
using cfares.domain._event.slot;
using cfares.domain._event._ticket;
using cfares.domain._event.slot.tours;
using System.Configuration;
using System.IO;
using System.Net;
using WinSCP;

namespace cfares.routine
{
    class Program
    {
        static void Main(string[] args)
        {
            bool REMINDERS = true;
            bool I_VISITOR = true;

            StringBuilder summary = new StringBuilder();

            ResEmailService emailServce = new ResEmailService();
            TicketService ticketService = new TicketService();
            UserService userService = new UserService();
            SlotService slotServ = new SlotService();
            //ResUser me = userService.LoadByEmail("matthew@thefoundryagency.com");
            //TourTicket myTicket = ticketService.LoadTourWithOwnerAndSlot("461");
            //emailServce.Test("gtg092x@gmail.com","For "+me.Name.Full);
            int errorNum = 0;
            StringBuilder errorLog = new StringBuilder();
            #region slotReminders
            if (REMINDERS)
            {
                string[] eventTypes = new string[] { "story", "lgstory", "team" };
                TimeSpan fromNowForSlotReminders = TimeSpan.FromDays(7);
                foreach (string eventType in eventTypes)
                {
                    TourSlotCollection upcomingTours = GetUpcomingSlotsForEvent(eventType, fromNowForSlotReminders, TimeSpan.FromDays(1));
                    foreach (TourSlot slot in upcomingTours)
                    {
                        TourTicketCollection tickets = GetTicketsFromSlot(slot);
                        foreach (TourTicket ticket in tickets)
                        {
                            Console.WriteLine(ticket.ToChecksum());

							throw new NotSupportedException();
                            try
                            {
								// SH - Not yet supported at this time
                                //emailServce.SendEmailReminder(ticket);
                                summary.AppendLine(string.Format("\nReminder sent for {0}",ticket.Owner.Email));
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                errorNum++;
                                errorLog.AppendLine("Error with reminder:");
                                errorLog.AppendLine( ex.Message);
                            }
                        }
                    }
                }
                Console.WriteLine("Sent customer Reminders using templates at " + Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings["mvc-template-root"]));
            }
            
            #endregion slotReminders

            #region iVisitor
            if (I_VISITOR)
            {
                /*FTP INFO*/
                string path = ConfigurationManager.AppSettings["i-visitor-ftp-path"];
                
                string host = ConfigurationManager.AppSettings["i-visitor-ftp-host"];
                string pass = ConfigurationManager.AppSettings["i-visitor-ftp-pass"];
                string user = ConfigurationManager.AppSettings["i-visitor-ftp-user"];
                Uri hostUri = new Uri(host);

                string thumb = ConfigurationManager.AppSettings["i-visitor-thumbprint"];

                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = hostUri.Host,
                    UserName = user,
                    Password = pass,
                    SshHostKeyFingerprint=thumb,
                };

                /* */

                string[] eventTypes = new string[] { "story", "lgstory", "team" };
                TimeSpan fromNowForiVisitorReminders = TimeSpan.FromDays(1);

                List<string> csvs = new List<string>();

                foreach (string eventType in eventTypes)
                {
                    TourSlotCollection upcomingTours = GetUpcomingSlotsForEvent(eventType, fromNowForiVisitorReminders, TimeSpan.FromDays(1));

                    

                    foreach (TourSlot slot in upcomingTours)
                    {
                        
                        string csv = slotServ.ToiVisitorCSV(slot.Id(),"^",false);
                        if(!string.IsNullOrEmpty(csv))
                            csvs.Add(csv);

                    }

                    if (csvs.Count == 0)
                        continue;

                    
                }

                if (csvs.Count > 0)
                {
                    try
                    {

                        string filename = "ivisitor_chickfila";
                        string href = string.Format("Data\\Csv\\admin\\{0}.csv", filename);
                        string filepath = Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings["mvc-root"]) + href;

                        

                        using (StreamWriter outfile =
                            new StreamWriter(filepath))
                        {
                            outfile.Write(string.Join(Environment.NewLine, csvs.ToArray()));
                        }

                        Uri serverUri = new Uri(host + path + "/" + filename + ".csv");

                        using (Session session = new Session())
                        {
                            // Connect
                            session.Open(sessionOptions);

                            // Upload files
                            TransferOptions transferOptions = new TransferOptions();
                            transferOptions.TransferMode = TransferMode.Binary;

                            TransferOperationResult transferResult;
                            transferResult = session.PutFiles(filepath, path + "/"+filename+".csv", false, transferOptions);

                            // Throw on any error
                            transferResult.Check();

                            // Print results
                            foreach (TransferEventArgs transfer in transferResult.Transfers)
                            {
                                Console.WriteLine("Upload of {0} succeeded", transfer.FileName);
                            }
                        }

                       

                        summary.AppendLine(string.Format("\niVisitor Uploaded to {0} on {1}", serverUri.ToString(),DateTime.Now.ToString()));

                    }catch(Exception ex){ 
                        Console.WriteLine(ex.Message);
                        errorNum++;
                        errorLog.AppendLine("Error with iVisitor Upload:");
                        errorLog.AppendLine(ex.Message);
                    }

                    Console.WriteLine("Sent iVisitor report");
                }
            }
            

            
            #endregion iVisitor

            string emailBody = "CFA Routine has been run on " + DateTime.Now.ToShortDateString() + " With " + errorNum + " errors.\n" + errorLog.ToString() + "\n\n"+summary.ToString();
            emailServce.Message(new string[] { "gtg092x@gmail.com"}, "[Chick-fil-A Tours] Routine Results", emailBody);
            
        }

        static TourSlotCollection GetUpcomingSlotsForEvent(string evnt, TimeSpan fromNow) {
            return GetUpcomingSlotsForEvent(evnt,fromNow,TimeSpan.FromDays(1));
        }

        static TourSlotCollection GetUpcomingSlotsForEvent(string evnt, TimeSpan fromNow, TimeSpan variance)
        { 
            SlotService serv = new SlotService();
            TourSlotCollection slots = serv.DeCacheOrLoadTourByEventTypeWithDateRange(
				evnt, DateTime.Now + fromNow , DateTime.Now + fromNow+variance);
            return slots;
        }

        static TourTicketCollection GetTicketsFromSlot(TourSlot slot)
        {
            TicketService serv = new TicketService();
            TourTicketCollection tickets = serv.LoadToursAndOwnersBySlot(slot.Id());
            foreach (TourTicket t in tickets) {
                t.Slot = slot;
            }
            return tickets;
        }
    }
}
 