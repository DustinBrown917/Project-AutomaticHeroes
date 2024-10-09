
namespace SegoLib
{
    public static class NumberSimplifier
    {
        public static string GetSimplifiedNumber(double num)
        {
            if (num < 1_000)
                return num.ToString();
            else if (num < 1_000_000)
                return ((long)(num / 1_000)).ToString() + "K";
            else if (num < 1_000_000_000)
                return ((long)(num / 1_000_000)).ToString() + "M";
            else if(num < 1_000_000_000_000)
                return ((long)(num / 1_000_000_000)).ToString() + "B";

            return num.ToString();
        }
    } 
}
