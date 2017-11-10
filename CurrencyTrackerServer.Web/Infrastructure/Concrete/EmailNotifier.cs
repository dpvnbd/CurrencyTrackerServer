using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Web.Models;
using Microsoft.Extensions.Options;
using Serilog;

namespace CurrencyTrackerServer.Web.Infrastructure.Concrete
{
  public class EmailNotifier : IMessageNotifier
  {
    private readonly SmtpSettings _s;
    private static int count = 0;
    private static int ecount = 0;
    public EmailNotifier(IOptions<SmtpSettings> config)
    {
      _s = config.Value;
      Log.Debug("creating notifier " + count++);
    }

    public async Task<bool> SendMessage(string address, string text)
    {
      Log.Debug("someone sending message " + ecount++);

      var fromAddress = new MailAddress(_s.FromAddress, _s.FromName);
      var toAddress = new MailAddress(address, null);
      var subject = "CTS";
      var body = text;

      var smtp = new SmtpClient
      {
        Host = _s.Server,
        Port = _s.Port,
        EnableSsl = false,
        DeliveryMethod = _s.SendToDirectory ? SmtpDeliveryMethod.SpecifiedPickupDirectory : SmtpDeliveryMethod.Network,
        UseDefaultCredentials = false,
        Credentials = new NetworkCredential(_s.Username, _s.Password),
        PickupDirectoryLocation = _s.DirectoryLocation
      };
      using (var message = new MailMessage(fromAddress, toAddress)
      {
        Subject = subject,
        Body = body
      })
      {
        try
        {
          Log.Warning($"Sending email to {address}: {text}");
          await smtp.SendMailAsync(message);
        }
        catch (Exception e)
        {
          Log.Error(e, "");
          return false;
        }
      }

      return true;
    }
  }
}
