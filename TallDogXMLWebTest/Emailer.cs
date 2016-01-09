using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net.Mime;
using System.Configuration;


namespace TallDogXMLWebTest
{
    class Emailer
    {

        private SmtpClient client;
        private MailAddress addrFrom;

        private List<MailAddress> toList = new List<MailAddress>();
        private List<MailAddress> CClist = new List<MailAddress>();
        private List<MailAddress> BCClist = new List<MailAddress>();

        #region constructors & initialization

        public Emailer()
        {
            Initialize();
        }

        public Emailer(List<string> sendTo, List<string> sendCC, List<string> sendBCC)
        {

            Initialize();
            foreach (string addrStr in sendTo)
                AddToAddresses(addrStr);
            foreach (string addrStr in sendCC)
                AddCcAddresses(addrStr);
            foreach (string addrStr in sendBCC)
                AddBccAddresses(addrStr);
        }

        private void Initialize()
        {
            // Setup email base from configuration settings

            client = new SmtpClient(ConfigurationManager.AppSettings["emailserver"]);
            client.Credentials = new System.Net.NetworkCredential(
                ConfigurationManager.AppSettings["emailfromusername"],
                ConfigurationManager.AppSettings["emailfrompassword"]);
            client.Port = int.Parse(ConfigurationManager.AppSettings["emailport"]);

            addrFrom = new MailAddress(ConfigurationManager.AppSettings["emailfrom"]);

        }

        #endregion

        public void SendEmail(string addressTo, string subject, string message, string[] attachments)
        {
            AddToAddresses(addressTo);
            SendEmail(subject, message, attachments);
        }
        public void SendEmail(string subject, string message, string[] attachments)
        {
            // Send an email message
            // code adapted from EACServer

            MailMessage mail = new MailMessage();
            try
            {
                addAddressing(mail);
                if (mail.To.Count() < 1)
                    throw new Exception("Email send-to not defined");

                mail.From = addrFrom;
                mail.Subject = subject;
                mail.Body = message;

                if (attachments != null && attachments.Length > 0)
                {
                    foreach (String attachment in attachments)
                    {
                        Attachment data = new Attachment(attachment, MediaTypeNames.Application.Octet);

                        // Add time stamp information for the file.
                        ContentDisposition disposition = data.ContentDisposition;
                        disposition.CreationDate = System.IO.File.GetCreationTime(attachment);
                        disposition.ModificationDate = System.IO.File.GetLastWriteTime(attachment);
                        disposition.ReadDate = System.IO.File.GetLastAccessTime(attachment);

                        // Add the file attachment to this e-mail message.
                        mail.Attachments.Add(data);

                    }

                }

                client.Send(mail);
                if (mail.Attachments.Count > 0)
                {
                    mail.Attachments.Dispose();
                }


            }
            catch (Exception ex)
            {
                if (mail.Attachments.Count > 0)
                {
                    mail.Attachments.Dispose();
                }
                throw ex;
            }
        }

        public void AddToAddresses(string emailAddress)
        {
            // add an email address to the member MailAddress list
            toList.Add(new MailAddress(emailAddress));
        }
        public void AddCcAddresses(string emailAddress)
        {
            // add an email address to the member MailAddress list
            CClist.Add(new MailAddress(emailAddress));
        }
        public void AddBccAddresses(string emailAddress)
        {
            // add an email address to the member MailAddress list
            CClist.Add(new MailAddress(emailAddress));
        }

        #region private (class internal) methods

        private void addAddressing(MailMessage mailMsg)
        {
            // fill email message with class member addresses

            foreach (MailAddress mAddr in toList)
            {
                mailMsg.To.Add(mAddr);
            }
            foreach (MailAddress mAddr in CClist)
            {
                mailMsg.CC.Add(mAddr);
            }
            foreach (MailAddress mAddr in BCClist)
            {
                mailMsg.Bcc.Add(mAddr);
            }


        }

        #endregion

    }
}
