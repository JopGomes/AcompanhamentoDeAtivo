using System;

using System.Net.Mail;
using System.Net;


namespace stock_quote_alert
{
    public class EmailHost
    {
        private String emailFrom;
        private String password;
        public EmailHost(String emailFrom,String password)
        {
            this.emailFrom = emailFrom;
            this.password = password;
        }
        public void sendEmail( String body, String subtitulo, EmailTarget to)
        {
            try
            {
                var smtpClient = new SmtpClient(to.getClient())
                {
                    Port = to.getPort(),
                    Credentials = new NetworkCredential(emailFrom, password),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(emailFrom),
                    Subject = subtitulo,
                    Body = body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(to.getEmail());

                smtpClient.Send(mailMessage);

            }
            catch (Exception e) {
                Console.WriteLine("Verifique se você está conectado a internet e se o email destino está correto.");
            }
        }
    }
}
