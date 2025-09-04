using System.Net.Mail;

namespace BattleChess3.Multiplayer.Mail;

public interface IEmailClient
{
    Task SendVerificationEmail(MailAddress toAddress, string privateVerificationCode, CancellationToken cancellationToken);
}