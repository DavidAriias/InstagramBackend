using System.Text.RegularExpressions;

namespace Instagram.config.helpers
{
    public static class TagHelper
    {
        public static bool AreAllHashtagsValid(List<string> hashtags)
        {
            // Regular expression pattern to validate hashtags.
            string pattern = @"^#[A-Za-z0-9_]+$";

            // Use LINQ to check if all elements are valid hashtags.
            return hashtags.All(hashtag => Regex.IsMatch(hashtag, pattern));
        }
    }
}
