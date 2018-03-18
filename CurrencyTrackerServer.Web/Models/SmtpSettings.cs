namespace CurrencyTrackerServer.Web.Models
{
    public class SmtpSettings
    {
      public string Server { get; set; }
      public int Port { get; set; }
      public string FromAddress { get; set; }
      public string FromName { get; set; }
      public string Username { get; set; }
      public string Password { get; set; }
      public bool SendToDirectory { get; set; }
      public string DirectoryLocation { get; set; }

    
  }
}
