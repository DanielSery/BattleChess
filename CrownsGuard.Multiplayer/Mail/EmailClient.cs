using System.Net;
using System.Net.Mail;

namespace CrownsGuard.Multiplayer.Mail;

public class EmailClient : IEmailClient
{
    public async Task SendVerificationEmail(MailAddress toAddress, string playerName, string privateVerificationCode, CancellationToken cancellationToken)
    {
        var fromAddress = new MailAddress("crownsguardofficial@gmail.com", "Crown's guard");

        const string subject = "Crown's guard verification code";
        var body = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
<meta charset=""UTF-8"">
<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
<title>Verification Code</title>
<style>
  /* Reset some styles */
  body, p, h1, h2, h3, a {{
    margin: 0;
    padding: 0;
    font-family: Arial, sans-serif;
  }}
  body {{
    background-color: #BCB09A;
    color: #333333;
  }}
  .container {{
    max-width: 600px;
    margin: 40px auto;
    background-color: #ffffff;
    border-radius: 10px;
    overflow: hidden;
    box-shadow: 0 2px 8px rgba(0,0,0,0.1);
  }}
  .header {{
    background-color: #BCB09A;
    color: #ffffff;
    padding: 20px;
    text-align: center;
  }}
  .header h1 {{
    font-size: 24px;
  }}
  .body {{
    padding: 30px 20px;
    text-align: center;
  }}
  .body p {{
    font-size: 16px;
    margin-bottom: 20px;
  }}
  .code {{
    font-size: 28px;
    font-weight: bold;
    background-color: #f1f5f9;
    padding: 15px 20px;
    border-radius: 8px;
    display: inline-block;
    letter-spacing: 4px;
    margin-bottom: 20px;
  }}
  .footer {{
    background-color: #f4f4f6;
    color: #777777;
    padding: 20px;
    font-size: 12px;
    text-align: center;
  }}
  @media only screen and (max-width: 600px) {{
    .body {{
      padding: 20px 10px;
    }}
    .code {{
      font-size: 24px;
      padding: 12px 15px;
    }}
    .header h1 {{
      font-size: 20px;
    }}
  }}
</style>
</head>
<body>
  <div class=""container"">
    <div class=""header"">
      <h1>Verify Your Email</h1>
    </div>
    <div class=""body"">
      <p>Verify you Crown's guard account.</p>
      <p>Paste this code below to Crown's guard to complete account creation:</p>
      <div class=""code"">{privateVerificationCode}</div>
      <p>If you did not request this, please ignore this email.</p>
    </div>
    <div class=""footer"">
      &copy; 2025 Your Company. All rights reserved.<br>
    </div>
  </div>
</body>
</html>
";
        var smtp = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Credentials = new NetworkCredential(fromAddress.Address, Secrets.GmailString),
            Timeout = 20000
        };

        using var message = new MailMessage(fromAddress, toAddress);
        message.Subject = subject;
        message.Body = body;
        message.IsBodyHtml = true;
        await smtp.SendMailAsync(message, cancellationToken);
    }
}