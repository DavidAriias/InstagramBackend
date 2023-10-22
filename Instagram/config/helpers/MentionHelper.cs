using System.Text.RegularExpressions;

namespace Instagram.config.helpers
{
    public static class MentionHelper
    {
        public static bool IsMentionValid(string mention)
        {
            // Regular expression pattern to validate mentions.
            string pattern = @"^@[A-Za-z0-9_.-]+$";

            // Use LINQ to check if all elements are valid mentions.
            return Regex.IsMatch(mention, pattern);
        }
    }
}
