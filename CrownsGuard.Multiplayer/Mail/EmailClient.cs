using System.Net;
using System.Net.Mail;

namespace CrownsGuard.Multiplayer.Mail;

public class EmailClient : IEmailClient
{
    public async Task SendVerificationEmail(MailAddress toAddress, string privateVerificationCode, CancellationToken cancellationToken)
    {
        var fromAddress = new MailAddress("theCrownsGuard@gmail.com", "BattleChess 3");

        const string subject = "BattleChess 3 verification code";
        var body = $"Hello,\n\nYour verification code is: {privateVerificationCode}\nPaste it into the Battle Chess 3 verification field.\n\nHave a nice day!\nBattleChess 3";

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
        await smtp.SendMailAsync(message, cancellationToken);
    }
}