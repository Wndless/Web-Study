namespace yohye
{
    public class Common
    {
        public static string GetDomainUrl(bool isDev = false)
        {
            if (isDev)
            {
                return "https://localhost:7296/";
            }
            else
            {
                return "https://yohyewebtest.azurewebsites.net/";
            }
        }
    }
}
