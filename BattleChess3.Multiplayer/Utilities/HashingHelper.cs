using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;

namespace BattleChess3.Multiplayer.Utilities;

public static class HashingHelper
{
    public static string GetEmailHash(string email)
    {
        return GetHash(email, Secrets.EmailSalt);
    }

    public static string GetHash(string str, string saltString)
    {
        var salt = Convert.FromBase64String(saltString);
        var pbkdf2 = new Rfc2898DeriveBytes(str, salt, 100000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32); // 256-bit hash
        return Convert.ToBase64String(hash);
    }

    public static string GetHash(SecureString secureString, string saltString)
    {
        ArgumentNullException.ThrowIfNull(secureString);

        var unmanagedString = IntPtr.Zero;
        try
        {
            var salt = Convert.FromBase64String(saltString);
            unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);

            var pbkdf2 = new Rfc2898DeriveBytes(Marshal.PtrToStringUni(unmanagedString)!, salt, 100000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32); // 256-bit hash

            return Convert.ToBase64String(hash);
        }
        finally
        {
            Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString); // Clear memory
        }
    }
}