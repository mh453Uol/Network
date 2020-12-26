using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Network
{
    public static class Utils
    {
        public static string Truncate(this string value, int maxLength = 500)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            if (value.Length <= maxLength)
            {
                return value;
            }

            return value.Substring(0, Math.Min(value.Length, 500));
        }
    }
}
