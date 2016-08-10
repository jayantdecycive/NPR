using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net.Configuration;
using System.Configuration;

namespace cfares.monitor
{
    class Program
    {
        static void Main(string[] args)
        {
            SmtpClient Client = new SmtpClient();
            Client.UseDefaultCredentials = false;

            SmtpSection settings = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

            Client.Credentials = new System.Net.NetworkCredential(
                settings.Network.UserName,
                settings.Network.Password);
            Client.EnableSsl = true;

            MailMessage message = new MailMessage();
            message.From = new MailAddress("server-health@chick-fil-a.com");

            message.Subject = "[Memory Violation] Please Observe";
            message.IsBodyHtml = true;
            message.Body = "There has been a memory overrage on your server. Please address.";

            message.To.Add("gtg092x@gmail.com");

            try
            {
                Client.Send(message);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);                
            }

        }
    }
}
