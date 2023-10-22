using System.Text.RegularExpressions;

namespace Instagram.config.helpers
{
    public static class PhoneNumberHelper
    {
        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            // Expresión regular para validar un número de teléfono en formato internacional
            string telefonoPattern = @"^\+\d{1,4}\d{10}$";

            return Regex.IsMatch(phoneNumber, telefonoPattern);
        }
    }
}
