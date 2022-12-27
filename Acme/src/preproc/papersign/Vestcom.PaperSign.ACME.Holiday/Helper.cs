using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vestcom.PaperSign.ACME.Holiday
{
    /// <summary>
    /// Class Which Define Extention Methods
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
        /// Convert StringToInteger
        /// </summary>
        /// <param name="value"></param>
        public static int ConvertStringToInteger(this string value)
        {
            int result;
            return int.TryParse(value, out result) ? Convert.ToInt32(value) : 0;
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
