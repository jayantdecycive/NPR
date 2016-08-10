using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfares.testconsole.locations;
using cfacore.mysql.dao._event;
using cfares.domain._event.slot.tours;
using cfacore.site.controllers._event;
using cfares.domain._event._ticket.tours;
using cfares.domain.user;
using cfares.domain._event;
using cfacore.shared.domain.media;
using cfacore.site.controllers.shared;
using System.Net.Mail;
using System.Net;
using cfares.domain._event.resevent;
using cfacore.mysql.dao.res;
using HttpMethodOverrideOperationSelection;
using LumenWorks.Framework.IO.Csv;
using System.IO;
using System.Text.RegularExpressions;
using cfacore.shared.domain.user;
using cfares.domain._event.slot;
using cfacore.shared.modules.user;
using System.Web.Security;
using ExcelLibrary.SpreadSheet;
using cfares.domain._event._ticket;
using cfacore.shared.service.email;
using cfares.site.modules.mail;
using cfacore.domain.user;
using System.Runtime.Serialization.Json;

namespace cfares.testconsole
{
    class Program
    {
        static void Main(string[] args)
        {

            //EmailService service = new EmailService();
            //service.Test("gtg092x@gmail.com");

            User u = new User();
            u.Username = "foo";
            MemoryStream stream1 = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(User));
            ser.WriteObject(stream1, u);
            stream1.Position = 0;
            StreamReader sr = new StreamReader(stream1);
            Console.Write("JSON form of Person object: ");
            Console.WriteLine(sr.ReadToEnd());
            Console.WriteLine("");




            /*TicketService serv = new TicketService();
            ResEmailService emailServce = new ResEmailService();

            TourTicket ticket = serv.LoadTourWithOwnerAndSlot("406");
            ticket.Owner.Email = "gtg092x@gmail.com";
            emailServce.SendEmailReminder(ticket, true);*/

        }
        protected static string RandomEmail() {
            return "placeholder." + RandomString(10) + "@chick-fil-a.com";
        }

        protected static Random random = new Random((int)DateTime.Now.Ticks);
        protected static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        protected static string Urlify(string str)
        {
            return Regex.Replace(str.Replace('+', ' '), @"[^A-Za-z0-9 ]", "").Trim().Replace(' ', '-').Replace("--", "-");
        }
        protected static string Domify(string str)
        {
            return Regex.Replace(Urlify(str).ToLower(), @"-", "_");
        }
        
        }

        
    }

