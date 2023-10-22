using dotenv.net;

namespace Instagram.config.constants
{
    public static class EnvironmentConfig
    {
        public static string? ClientId { get; private set; }
        public static string? ClientSecret { get; private set; }
        public static string? BlobKey { get; private set; }
        public static string? AccountSid { get; private set; }
        public static string? AccountToken { get; private set; }
        public static string? PathSid { get; private set; }
        public static string? ConnectionStringNotification { get; private set; }
        public static string? NotificationHubName { get; private set; }
        
        static EnvironmentConfig()
        {
            DotEnv.Load();

            ClientId = Environment.GetEnvironmentVariable("CLIENT_ID");
            ClientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET");
            BlobKey = Environment.GetEnvironmentVariable("BLOB_KEY");
            AccountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            AccountToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
            PathSid = Environment.GetEnvironmentVariable("PATH_SERVICE_SID");
            ConnectionStringNotification = Environment.GetEnvironmentVariable("NOTIFICATION_CONECTION");
            NotificationHubName = Environment.GetEnvironmentVariable("NOTIFICATION_HUB");

        }
        
    }
}
