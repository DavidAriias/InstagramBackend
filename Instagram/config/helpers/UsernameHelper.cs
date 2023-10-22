using Pipelines.Sockets.Unofficial.Arenas;
using System.Text.RegularExpressions;

namespace Instagram.config.helpers
{
    public static class UsernameHelper
    {
        public static bool IsValidUsername(string username)
        {
            string re = @"^[^\s]{1,30}$";
            return Regex.IsMatch(username, re);
        }
    }
}
