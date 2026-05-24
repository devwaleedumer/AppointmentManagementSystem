using System.Security.Cryptography;

namespace AppointmentManagementSystem.Helpers
{
    /// <summary>
    /// Generates random passwords that satisfy common Identity complexity rules.
    /// Used when admins onboard doctors (password is emailed, not shown in the UI).
    /// </summary>
    public static class PasswordGenerator
    {
        public static string GeneratePassword(int length = 8)
        {
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string numbers = "0123456789";
            const string special = "!@#$%^&*";

            string validChars = upper + lower + numbers + special;

            // Guarantee at least one character from each required category.
            List<char> password =
            [
                upper[RandomNumberGenerator.GetInt32(upper.Length)],
                lower[RandomNumberGenerator.GetInt32(lower.Length)],
                numbers[RandomNumberGenerator.GetInt32(numbers.Length)],
                special[RandomNumberGenerator.GetInt32(special.Length)],
            ];

            for (int i = password.Count; i < length; i++)
            {
                password.Add(validChars[RandomNumberGenerator.GetInt32(validChars.Length)]);
            }

            return new string(password
                .OrderBy(_ => RandomNumberGenerator.GetInt32(int.MaxValue))
                .ToArray());
        }
    }
}
