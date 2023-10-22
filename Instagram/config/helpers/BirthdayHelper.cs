namespace Instagram.config.helpers
{
    public static class BirthdayHelper
    {
        public static bool IsValidAge(DateOnly birthday)
        {

            DateTime dateNow = DateTime.Now;

            int age = dateNow.Year - birthday.Year;

            if (dateNow.Month < birthday.Month || (dateNow.Month == birthday.Month && dateNow.Day < birthday.Day))
            {
                age--;
            }

            return age > 13;
        }

        public static bool CanChangeBirthday(DateTime lastBirthdayChangeDate)
        {
            DateTime dateNow = DateTime.Now;

            int daysSinceLastChange = (int)(dateNow - lastBirthdayChangeDate).TotalDays;

            return daysSinceLastChange >= 3;
        }

        public static int CalculateAge(DateOnly birthday) 
        { 

            DateTime currentDate = DateTime.Now;

            int age = currentDate.Year - birthday.Year;

            // Verifica si aún no se ha celebrado el cumpleaños en este año
            if (currentDate.Month < birthday.Month || (currentDate.Month == birthday.Month && currentDate.Day < birthday.Day))
            {
                age--;
            }

            return age;

        }
    }
}
