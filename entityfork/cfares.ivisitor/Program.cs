
#region Imports

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using WinSCP;
using cfares.domain._event;
using cfares.repository.slot;
using cfares.site.modules.com.application;
using cfares.site.modules.mail;
using npr.domain._event.slot;
using cfacore.shared.modules.helpers;

#endregion

namespace cfares.ivisitor
{
    class Program
    {
        static void Main()
        {
            StringBuilder summary = new StringBuilder();

            ResEmailService emailServce = new ResEmailService();
            var context = ReservationConfig.GetContext();
            var slotServ = new SlotRepository<NPRSlot>(context);

            int errorNum = 0;
            StringBuilder errorLog = new StringBuilder();

            #region iVisitor Export

            bool iVisitorEnabled = ConfigurationManager.AppSettings["i-visitor-enabled"].ToBoolean();
            if (iVisitorEnabled)
            {
                /*FTP INFO*/
                string path = ConfigurationManager.AppSettings["i-visitor-ftp-path"];
                string host = ConfigurationManager.AppSettings["i-visitor-ftp-host"];
                string pass = ConfigurationManager.AppSettings["i-visitor-ftp-pass"];
                string user = ConfigurationManager.AppSettings["i-visitor-ftp-user"];
                string tkey = ConfigurationManager.AppSettings["i-visitor-thumbprint"];

                int timeSpanOffsetFromNowForSlotQueryInDays = ConfigurationManager.AppSettings["i-visitor-offset-from-now"].ToInt();
                int timeSpanLengthForSlotQueryInDays = ConfigurationManager.AppSettings["i-visitor-timespan-length"].ToInt();

                //	Uri hostUri = new Uri(host);
                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = host,
                    UserName = user,
                    Password = pass,
                    SshHostKeyFingerprint = tkey,
                };

                string[] eventTypes = new[] { "Tour", "SpecialEvent" };
                TimeSpan timeSpanOffsetFromNowForSlotQuery = TimeSpan.FromDays(timeSpanOffsetFromNowForSlotQueryInDays);
                TimeSpan timeSpanLengthForSlotQuery = TimeSpan.FromDays(timeSpanLengthForSlotQueryInDays);
                List<string> csvs = new List<string>();

                foreach (string eventType in eventTypes)
                {
                    ICollection<NPRSlot> upcomingTours = slotServ.GetUpcomingSlotsForEvent(
                        eventType, timeSpanOffsetFromNowForSlotQuery, timeSpanLengthForSlotQuery);

                    foreach (NPRSlot slot in upcomingTours)
                    {
                        string csv = slotServ.ToiVisitorCSV(slot.SlotId, "^", false, false);
                        if (!string.IsNullOrEmpty(csv))
                            csvs.Add(csv);
                    }
                }

                // Always write file to target even if no records ( guards against possibility of duplicates )
                try
                {
                    const string filename = "ivisitor_npr";
                    string href = string.Format("Data\\Csv\\admin\\{0}.csv", filename);
                    string filepath = Environment.ExpandEnvironmentVariables(
                        ConfigurationManager.AppSettings["mvc-root"]) + href;

                    if (csvs.Count == 0)
                        using (StreamWriter outfile = new StreamWriter(filepath))
                            outfile.Write(Environment.NewLine);
                    else
                        using (StreamWriter outfile = new StreamWriter(filepath))
                            outfile.Write(string.Join(Environment.NewLine, csvs.ToArray()));

                    //   Uri serverUri = new Uri(host + path + "/" + filename + ".csv");
                    string serverUri = host + path + "/" + filename + ".csv";

                    using (Session session = new Session())
                    {
                        // Connect
                        session.DisableVersionCheck = true;
                        session.ExecutablePath = ConfigurationManager.AppSettings["i-visitor-ExecutablePath"];
                        session.Open(sessionOptions);

                        // Upload files
                        TransferOptions transferOptions = new TransferOptions();
                        transferOptions.TransferMode = TransferMode.Binary;

                        TransferOperationResult transferResult = session.PutFiles(
                            filepath, path + "/" + filename + ".csv", false, transferOptions);

                        // Throw on any error
                        transferResult.Check();

                        // Print results
                        foreach (TransferEventArgs transfer in transferResult.Transfers)
                            Console.WriteLine("Upload of {0} succeeded", transfer.FileName);
                    }

                    summary.AppendLine(string.Format("\niVisitor Uploaded to {0} on {1}", serverUri, DateTime.Now.ToString(CultureInfo.InvariantCulture)));

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    errorNum++;
                    errorLog.AppendLine("Error with iVisitor Upload:");
                    errorLog.AppendLine(ex.Message);
                }

                Console.WriteLine("Sent iVisitor report");
            }

            #endregion iVisitor

            string emailBody = "NPR Routine has been run on " + DateTime.Now.ToShortDateString() + " With " +
                errorNum + " errors.\n" + errorLog + "\n\n" + summary;

            emailServce.Message(new[] { "gtg092x@gmail.com", "sean.harms@thefoundryagency.com", "carson.britt@thefoundryagency.com" },
                "[NPR Tours] Routine Results", emailBody);
            //Console.ReadKey();
        }
    }

    public static class SlotRepositoryExtensions
    {
        public static ICollection<NPRSlot> GetUpcomingSlotsForEvent(
            this SlotRepository<NPRSlot> repository, string eventType,
            TimeSpan timeSpanOffsetFromNowForSlotQuery, TimeSpan timeSpanLengthForSlotQuery)
        {
            var start = DateTime.Now + timeSpanOffsetFromNowForSlotQuery;
            var end = start + timeSpanLengthForSlotQuery;

            return repository.GetAll()
                //.Include("Occurrence.ResEvent") // Migrations on 6/18/2013 prompted this comment ( incompatible DB )
                .Where(x =>
                    x.Occurrence.ResEvent.ReservationTypeId == eventType &&
                    (x.Status == SlotStatus.Active || x.Status == SlotStatus.Hidden) &&
                    (x.Occurrence.ResEvent.Status == ResEventStatus.Live || x.Occurrence.ResEvent.Status == ResEventStatus.Hidden) &&
                    x.Start >= start &&
                    x.Start < end).Cast<NPRSlot>().ToList();
        }
    }
}
