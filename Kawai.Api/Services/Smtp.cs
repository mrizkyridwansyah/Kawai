using System.Net.Mail;
using System.Net;

namespace Kawai.Api;

public class Smtp(IConfiguration configuration, ILoggerFactory logger)
{
    protected IConfiguration Configuration { get; set; } = configuration;
    protected ILogger Logger { get; set; } = logger.CreateLogger("Info");

    public void Send(string to, string subject, string content, string displayName = null)
    {
        var client = new SmtpClient(Configuration["Smtp:Host"], Convert.ToInt32(Configuration["Smtp:Port"]))
        {
            Credentials = new NetworkCredential(Configuration["Smtp:Email"], Configuration["Smtp:Password"]),
            EnableSsl = true,
        };

        MailMessage message = new()
        {
            From = new MailAddress(Configuration["Smtp:Email"],
            displayName ?? Configuration["Smtp:Name"])
        };
        message.To.Add(new MailAddress(to));

        message.Subject = subject;
        message.IsBodyHtml = true; //to make message body as html  
        message.Body = content;
        try
        {
            client.Send(message);
            Logger.LogInformation("Email Sent");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message);
            throw;
        }
    }
}
