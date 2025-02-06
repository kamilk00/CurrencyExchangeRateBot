using System.Net;
using System.Net.Mail;

public class SMTPMailManager
{

    private readonly string smtpServer;
    private readonly int smtpPort;
    private readonly string emailUser;
    private readonly string emailPassword;

    public SMTPMailManager(string smtpServer, int smtpPort, string emailUser, string emailPassword)
    {
        this.smtpServer = smtpServer;
        this.smtpPort = smtpPort;
        this.emailUser = emailUser;
        this.emailPassword = emailPassword;
    }

    public void SendEmail(string subject, string body, string recipientEmail)
    {
        try
        {
            using SmtpClient client = new SmtpClient(this.smtpServer, this.smtpPort);
            client.Credentials = new NetworkCredential(this.emailUser, this.emailPassword);
            client.EnableSsl = true;

            MailMessage message = new MailMessage()
            {
                From = new MailAddress(this.emailUser),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            message.To.Add(recipientEmail);
            client.Send(message);
            Console.WriteLine("Mail sent successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
        }
    }

}