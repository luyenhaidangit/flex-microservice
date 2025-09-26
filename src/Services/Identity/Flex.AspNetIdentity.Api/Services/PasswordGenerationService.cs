using System.Security.Cryptography;
using Flex.AspNetIdentity.Api.Services.Interfaces;

namespace Flex.AspNetIdentity.Api.Services
{
    /// <summary>
    /// Service for generating secure random passwords
    /// </summary>
    public class PasswordGenerationService : IPasswordGenerationService
    {
        private const string UppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string LowercaseChars = "abcdefghijklmnopqrstuvwxyz";
        private const string NumberChars = "0123456789";
        private const string SpecialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

        /// <summary>
        /// Generates a secure random password with default settings (12 characters, all character types)
        /// </summary>
        /// <returns>Generated password</returns>
        public string GenerateRandomPassword()
        {
            return GenerateRandomPassword(12, true, true, true, true);
        }

        /// <summary>
        /// Generates a secure random password with custom settings
        /// </summary>
        /// <param name="length">Password length (minimum 8, maximum 128)</param>
        /// <param name="includeUppercase">Include uppercase letters</param>
        /// <param name="includeLowercase">Include lowercase letters</param>
        /// <param name="includeNumbers">Include numbers</param>
        /// <param name="includeSpecialChars">Include special characters</param>
        /// <returns>Generated password</returns>
        public string GenerateRandomPassword(int length = 12, bool includeUppercase = true, bool includeLowercase = true, bool includeNumbers = true, bool includeSpecialChars = true)
        {
            // ===== Validate parameters =====
            if (length < 8 || length > 128)
            {
                throw new ArgumentException("Password length must be between 8 and 128 characters.", nameof(length));
            }

            if (!includeUppercase && !includeLowercase && !includeNumbers && !includeSpecialChars)
            {
                throw new ArgumentException("At least one character type must be included.");
            }

            // ===== Build character set =====
            var charSet = string.Empty;
            if (includeUppercase) charSet += UppercaseChars;
            if (includeLowercase) charSet += LowercaseChars;
            if (includeNumbers) charSet += NumberChars;
            if (includeSpecialChars) charSet += SpecialChars;

            // ===== Generate password =====
            using var rng = RandomNumberGenerator.Create();
            var password = new char[length];
            var bytes = new byte[length];

            // ===== Fill password with random characters =====
            rng.GetBytes(bytes);
            for (int i = 0; i < length; i++)
            {
                password[i] = charSet[bytes[i] % charSet.Length];
            }

            // ===== Ensure password contains at least one character from each required type =====
            EnsurePasswordContainsRequiredTypes(password, charSet, includeUppercase, includeLowercase, includeNumbers, includeSpecialChars);

            return new string(password);
        }

        /// <summary>
        /// Ensures the generated password contains at least one character from each required type
        /// </summary>
        private static void EnsurePasswordContainsRequiredTypes(char[] password, string charSet, bool includeUppercase, bool includeLowercase, bool includeNumbers, bool includeSpecialChars)
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[1];

            // ===== Check and ensure uppercase =====
            if (includeUppercase && !password.Any(c => UppercaseChars.Contains(c)))
            {
                rng.GetBytes(bytes);
                var randomIndex = bytes[0] % password.Length;
                password[randomIndex] = UppercaseChars[bytes[0] % UppercaseChars.Length];
            }

            // ===== Check and ensure lowercase =====
            if (includeLowercase && !password.Any(c => LowercaseChars.Contains(c)))
            {
                rng.GetBytes(bytes);
                var randomIndex = bytes[0] % password.Length;
                password[randomIndex] = LowercaseChars[bytes[0] % LowercaseChars.Length];
            }

            // ===== Check and ensure numbers =====
            if (includeNumbers && !password.Any(c => NumberChars.Contains(c)))
            {
                rng.GetBytes(bytes);
                var randomIndex = bytes[0] % password.Length;
                password[randomIndex] = NumberChars[bytes[0] % NumberChars.Length];
            }

            // ===== Check and ensure special characters =====
            if (includeSpecialChars && !password.Any(c => SpecialChars.Contains(c)))
            {
                rng.GetBytes(bytes);
                var randomIndex = bytes[0] % password.Length;
                password[randomIndex] = SpecialChars[bytes[0] % SpecialChars.Length];
            }
        }
    }
}