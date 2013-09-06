using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class StringExtensions
    {
        public static string Args(this string instance, params string[] args)
        {
            return string.Format(instance, args);
        }

        public static string Args(this string instance, params object[] args)
        {
            return string.Format(instance, args);
        }
    }
}
