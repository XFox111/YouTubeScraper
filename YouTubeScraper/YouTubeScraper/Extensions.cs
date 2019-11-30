using System;

namespace YouTube
{
    internal static class Extensions
    {
        internal static Uri ToUri(this string str)
        {
            try { return new Uri(str); }
            catch { return null; }
        }

        internal static int RangeOffset(int value, int min, int max)
        {
            if (value < min)
                return -1;
            else if (value > max)
                return 1;
            else
                return 0;
        }
    }
}
