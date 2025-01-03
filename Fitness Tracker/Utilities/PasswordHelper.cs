using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Konscious.Security.Cryptography;

namespace Fitness_Tracker.Utilities
{
    public class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            using (var hasher = new Argon2id(Encoding.UTF8.GetBytes(password)))
            {
                hasher.Salt = GenerateSalt();
                hasher.DegreeOfParallelism = 4; // Number of threads to use
                hasher.MemorySize = 65536;     // Amount of memory to use in KB
                hasher.Iterations = 3;         // Number of iterations

                byte[] hash = hasher.GetBytes(32); // Generate 256-bit hash
                return Convert.ToBase64String(hash) + ":" + Convert.ToBase64String(hasher.Salt);
            }
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            string[] parts = hashedPassword.Split(':');
            if (parts.Length != 2) return false;

            byte[] hashBytes = Convert.FromBase64String(parts[0]);
            byte[] salt = Convert.FromBase64String(parts[1]);

            using (var hasher = new Argon2id(Encoding.UTF8.GetBytes(password)))
            {
                hasher.Salt = salt;
                hasher.DegreeOfParallelism = 4;
                hasher.MemorySize = 65536;
                hasher.Iterations = 3;

                byte[] newHash = hasher.GetBytes(32);
                return CryptographicEquals(hashBytes, newHash);
            }
        }

        private static byte[] GenerateSalt()
        {
            byte[] salt = new byte[16];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        private static bool CryptographicEquals(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;

            bool result = true;
            for (int i = 0; i < a.Length; i++)
            {
                result &= a[i] == b[i];
            }
            return result;
        }
    }
}
