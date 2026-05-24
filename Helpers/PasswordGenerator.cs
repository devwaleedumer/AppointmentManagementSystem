using System.Security.Cryptography;

namespace AppointmentManagementSystem.Helpers
{
    public static class PasswordGenerator
    {
        public static string GeneratePassword(int length = 8)
        {
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string numbers = "0123456789";
            const string special = "!@#$%^&*";

            string validChars = upper + lower + lower + special;

            List<char> password = new()
            {
                upper[RandomNumberGenerator.GetInt32(upper.Length)],
                lower[RandomNumberGenerator.GetInt32(lower.Length)],
                numbers[RandomNumberGenerator.GetInt32(numbers.Length)],
                special[RandomNumberGenerator.GetInt32(special.Length)],
            };

            for (int i = password.Count; i < length; i++)
            {
                password.Add(validChars[RandomNumberGenerator.GetInt32(validChars.Length)]);
            }
            // Shuffle
            return new string(password
                .OrderBy(x => RandomNumberGenerator.GetInt32(int.MaxValue))
                .ToArray());
        }
        }
}
