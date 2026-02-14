using System.Net;
using System.Net.Mail;

namespace RouteG01.Pl.Helper
{
    public static  class EmailSettings
    {
        public static void SendEmail (Email email)
        {
            var Client = new SmtpClient("smtp.gmail.com", 587);
            Client.EnableSsl = true;
            Client.Credentials = new NetworkCredential("alaa99en@gmail.com", "Alaa@2004");
            Client.Send("alaa99en@gmail.com", email.To, email.Subject, email.Body);
             

        }
    }
}
