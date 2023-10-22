namespace Instagram.config.helpers
{
    public static class NameHelper
    {
        public static bool CanChangeName(DateTime lastNameChangeDate)
        {
            DateTime dateNow = DateTime.Now;

            int daysSinceLastChange = (int)(dateNow - lastNameChangeDate).TotalDays;

            return daysSinceLastChange >= 3;
        }
    }
}
