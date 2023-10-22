using System.Text.RegularExpressions;

namespace Instagram.config.helpers
{
    public static class LinkHelper
    {
        public static bool IsLink(string link)
        {
            // Patrón de expresión regular para validar una URL simple.
            string pattern = @"^(https?://)?([\w-]+(\.[\w-]+)+)(/[\w-./?%&=]*)?$";

            // Utiliza Regex.IsMatch para verificar si la cadena coincide con el patrón.
            return Regex.IsMatch(link, pattern);
        }
    }
}
