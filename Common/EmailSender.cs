using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using ComputerDeviceShopping.ViewModel;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string htmlMessage);
}

public class EmailSender : IEmailSender
{
    private readonly SmtpSettings _smtpSettings;

    public EmailSender(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        try
        {
            using (var smtp = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port))
            {
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.Username, "Cửa hàng bán thiết bị máy tính TECH GEARS"),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                await smtp.SendMailAsync(mailMessage);
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions here (logging, etc.)
            throw new ApplicationException($"Error sending email: {ex.Message}");
        }
    }
}

