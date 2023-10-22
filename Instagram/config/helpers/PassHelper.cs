using System.Text.RegularExpressions;

namespace Instagram.config.helpers
{
    public static class PassHelper
    {
        public static bool IsValidPass(string pass)
        {
            // Define una expresión regular para verificar la contraseña
            // La expresión regular requiere al menos una letra mayúscula, una minúscula, un número, un símbolo y una longitud mínima de 8 caracteres
            string re = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{8,16}$";

            // Usa Regex.IsMatch para verificar si la contraseña coincide con el patrón
            return Regex.IsMatch(pass, re);
        }
    }
}
