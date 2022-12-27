using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vestcom.PaperSign.ACME.ProcessDownback
{
    /// <summary>
    /// Helper class, defines common utility methods
    /// </summary>
    static class Helper
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
        /// Gets Substring from a string
        /// </summary>
        /// <param name="text"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        public static string SafeSubstring(this string text, int start, int length)
        {
            return (string.IsNullOrEmpty(text) || text.Length <= start) ? string.Empty
                : text.Length - start <= length ? text.Substring(start)
                : text.Substring(start, length);
        }

        /// <summary>
        /// Gets Substring from a string
        /// </summary>
        /// <param name="text"></param>
        /// <param name="start"></param>
        
        public static string SafeSubstring(this string text, int start)
        {
            return (string.IsNullOrEmpty(text) || text.Length <= start) ? string.Empty : text.Substring(start);
        }

        /// <summary>
        /// Convert StringToInteger
        /// </summary>
        /// <param name="value"></param>
        public static int ConvertStringToInteger(this string value)
        {
            int result;
            return int.TryParse(value, out result) ? Convert.ToInt32(value) : 0;
        }

        /// <summary>
        /// Convert StringToInteger
        /// </summary>
        /// <param name="value"></param>
        public static double ConvertStringToDouble(this string value)
        {
            double result;
            return Double.TryParse(value, out result) ? Convert.ToDouble(value) : 0.0;
        }

        /// <summary>
        /// Converts the string to date time.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static DateTime ConvertStringToDateTime(this string value)
        {
            DateTime result;
            DateTime.TryParse(value, out result);
            return result;
        }



    }
}
