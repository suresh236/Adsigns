using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vestcom.PaperSign.ACME.Entities
{
   public static class Helper
    {
        /// <summary>
        /// Check if str is null then assign string.Empty
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EmptyNull(this string str)
        {
            return str ?? string.Empty;
        }

        /// <summary>
        /// Check if text is null then assign string.Empty
        /// </summary>
        /// <param name="text"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string SafeSubstring(this string text, int start, int length)
        {
            return (string.IsNullOrEmpty(text) || text.Length <= start) ? string.Empty
                : text.Length - start <= length ? text.Substring(start)
                : text.Substring(start, length);
        }

        /// <summary>
        /// Check if text is null then assign string.Empty
        /// </summary>
        /// <param name="text"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static string SafeSubstring(this string text, int start)
        {
            return (string.IsNullOrEmpty(text) || text.Length <= start) ? string.Empty : text.Substring(start);
        }

        /// <summary>
        /// Check value is Integer if not then return zero
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ConvertStringToInteger(this string value)
        {
            int result;
            return int.TryParse(value, out result) ? Convert.ToInt32(value) : 0;
        }
    }
}
