using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;

namespace CrownsGuard.Multiplayer.Utilities;

public static class HashingHelper
{
    public static string GetSalt()
    {
        var salt = RandomNumberGenerator.GetBytes(16); // Generate 16-byte salt
        return Convert.ToBase64String(salt);
    }

    public static string GetEmailHash(string email)
    {
        var salt = Convert.FromBase64String(Secrets.EmailSalt);
        using var pbkdf2 = new Rfc2898DeriveBytes(email, salt, 100000, HashAlgorithmName.SHA256);
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

            using var pbkdf2 = new Rfc2898DeriveBytes(Marshal.PtrToStringUni(unmanagedString)!, salt, 100000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32); // 256-bit hash

            return Convert.ToBase64String(hash);
        }
        finally
        {
            Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString); // Clear memory
        }
    }
}