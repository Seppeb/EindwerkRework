using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;


namespace ApplicationRequestIt.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {

        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(Options.SendGridKey, subject, message, email);
        }

        public Task Execute(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("RequestIt@dobit.com", "RequestIt"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.TrackingSettings = new TrackingSettings
            {
                ClickTracking = new ClickTracking { Enable = false }
            };

            return client.SendEmailAsync(msg);
        }

        //public Task SendEmailAsync(string email, string subject, string message)
        //{
        //    return Task.CompletedTask;
        //}

        //public EmailSender(IOptions<SendGridSenderOptions> optionAccessor)
        //{
        //    Options = optionAccessor.Value;
        //}

        //public SendGridSenderOptions Options { get; }

        //public Task SendEmailAsync(string email, string subject, string message)
        //{
        //    return Execute(Options.SendGridKey, subject, message, email);
        //}

        //public Task Execute(string apikey, string subject, string message, string email)
        //{
        //    var client = new SendGridClient(apikey);
        //    var msg = new SendGridMessage()
        //    {
        //        From = new EmailAddress("RequestIt@dobit.com", "RequestIt"),
        //        Subject = subject,
        //        PlainTextContent = message,
        //        HtmlContent = message
        //    };
        //    msg.AddTo(new EmailAddress(email));
        //    return client.SendEmailAsync(msg);
        //}


    }
}
