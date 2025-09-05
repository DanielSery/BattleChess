using System.Net.Mail;

namespace CrownsGuard.Multiplayer.Mail;

public interface IEmailClient
{
    Task SendVerificationEmail(MailAddress toAddress, string playerName, string privateVerificationCode, CancellationToken cancellationToken);
}