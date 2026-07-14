using System.Text.RegularExpressions;

namespace TechStore.API.Helpers
{
    public static class InputValidationHelper
    {
        public static bool IsEmailValid(string email)
        {
            return !string.IsNullOrWhiteSpace(email) &&
                Regex.IsMatch(email.Trim(), @"^[^\s@]+@[^\s@]+\.[^\s@]+$");
        }

        public static bool IsPhoneValid(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                return false;
            }

            string normalizedPhone = Regex.Replace(phone, @"\s+", string.Empty);

            return Regex.IsMatch(normalizedPhone, @"^(?:5\d{9}|05\d{9})$");
        }
    }
}
