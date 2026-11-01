namespace TravelAgency.Core.Common
{
    public static class PassportUtils
    {
        public static string Mask(string passport)
        {
            if (string.IsNullOrEmpty(passport) || passport.Length < 4)
                return passport;

            int length = passport.Length;
            string start = passport.Substring(0, 2);
            string end = passport.Substring(length - 2);
            string middle = new string('*', length - 4);

            return start + middle + end;
        }
    }
}