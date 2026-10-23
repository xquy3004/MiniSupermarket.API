using System.Security.Cryptography;

namespace MiniSupermarket.API.Helpers
{
    // Băm mật khẩu bằng PBKDF2 (có sẵn trong .NET, không cần cài thêm gói)
    public static class PasswordHelper
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 100000;

        // Kết quả có dạng: "số vòng lặp.muối.băm"
        public static string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);
            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
        }

        public static bool Verify(string password, string stored)
        {
            if (string.IsNullOrEmpty(stored) || string.IsNullOrEmpty(password)) return false;

            // Kiểm tra mã băm chuẩn PBKDF2 (dạng: iterations.salt.key)
            try
            {
                var parts = stored.Split('.');
                if (parts.Length != 3 || !int.TryParse(parts[0], out int iterations)) return false;

                byte[] salt = Convert.FromBase64String(parts[1]);
                byte[] expected = Convert.FromBase64String(parts[2]);
                byte[] actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);

                return CryptographicOperations.FixedTimeEquals(actual, expected);
            }
            catch
            {
                return false;
            }
        }
    }
}
