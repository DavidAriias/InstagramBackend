namespace Instagram.config.helpers
{
    public static class MediaHelper
    {
        public static bool IsImage(string url)
        {
            string extension = System.IO.Path.GetExtension(url);
            return extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase);
        }

        public static string GetLocalPath(string imageUrl)
        {
            Uri uri = new Uri(imageUrl);
            return uri.LocalPath;
        }
    }
}
