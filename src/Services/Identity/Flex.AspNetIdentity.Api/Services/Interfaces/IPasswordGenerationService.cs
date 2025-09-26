namespace Flex.AspNetIdentity.Api.Services.Interfaces
{
    /// <summary>
    /// Service for generating secure random passwords
    /// </summary>
    public interface IPasswordGenerationService
    {
        /// <summary>
        /// Generates a secure random password with default settings
        /// </summary>
        /// <returns>Generated password</returns>
        string GenerateRandomPassword();

        /// <summary>
        /// Generates a secure random password with custom settings
        /// </summary>
        /// <param name="length">Password length (minimum 8, maximum 128)</param>
        /// <param name="includeUppercase">Include uppercase letters</param>
        /// <param name="includeLowercase">Include lowercase letters</param>
        /// <param name="includeNumbers">Include numbers</param>
        /// <param name="includeSpecialChars">Include special characters</param>
        /// <returns>Generated password</returns>
        string GenerateRandomPassword(int length = 12, bool includeUppercase = true, bool includeLowercase = true, bool includeNumbers = true, bool includeSpecialChars = true);
    }
}