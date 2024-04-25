using System.Security.Cryptography;

namespace OpenIdClient
{
    public class SecureValueGenerator
    {
        public static string GenerateRandomString(int length)
        {
            byte[] randomBytes = new byte[length];

            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }

            return BitConverter.ToString(randomBytes).Replace("-", "").ToLower();
        }
    }
}
