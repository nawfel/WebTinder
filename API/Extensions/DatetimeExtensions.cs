namespace API.Extensions
{
    public static class DatetimeExtensions
    {

        public static int CalculateAge(this DateOnly dod)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var age = today.Year-dod.Year;
            if (dod > today.AddDays(age - age)) age--;
            return age;
        }
    }
}
