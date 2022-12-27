using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Vestcom.PaperSign.ACME.Entities;

namespace Vestcom.PaperSign.ACME.ProcessInput.Rules
{
    /// <summary>
    /// Common class. It defines the common methods used acoss the project.
    /// </summary>
    public static class Common
    {
        /// <summary>
        /// This function split input string on specific characters & convert the First Letter of splited values to Upper Case except those which contains either 'or' or 'and' keyword
        /// </summary>
        /// <param name="inputValue"></param>
        /// <returns></returns>
        public static string UpLowCas(string inputValue)
        {
            string[] values = inputValue.ToLowerInvariant().Split(new char[] { ' ', '|', '-', '$', '`' });

            for (int i = 0; i < values.Count(); i++)
            {
                if (!new string[] { "or", "and" }.Contains(values[i].Trim()))
                {
                    values[i] = values[i].FirstLetterToUpper();
                }
            }

            return string.Join(" ", values);
        }

        /// <summary>
        ///Writes Rejected data to File
        /// </summary>
        /// <param name="inputRecord"></param>
        public static void CALL_MSGDRVR(InputFile lbl, string packagepath)
        {
            string RejectFilePath = Path.Combine(packagepath, Constants.RejectFileName);
            if (File.Exists(RejectFilePath))
            {
                File.AppendAllText(RejectFilePath, "MSG_CR= " + lbl.MSG_CR + ", MSG_MESSAGE= " + lbl.MSG_MESSAGE + Environment.NewLine);
            }
            else
            {
                File.WriteAllText(RejectFilePath, "MSG_CR= " + lbl.MSG_CR + ", MSG_MESSAGE= " + lbl.MSG_MESSAGE + Environment.NewLine);
            }
        }


        /// <summary>
        /// This program removes quotes around a field and changes all double quotations to single.
        /// </summary>
        /// <param name="inputValue"></param>
        /// <returns></returns>
        public static string UnQuote(string inputValue)
        {
            if (inputValue.SafeSubstring(0, 1) == Constants.DoubleQuote)
            {
                var regex = new Regex(Regex.Escape("&*"));

                int Double_Quote_Count = 0;
                string Work_String_Remainder = inputValue.SafeSubstring(1);

                if (Work_String_Remainder.Contains(Constants.DoubleQuote + Constants.DoubleQuote))
                {
                    Double_Quote_Count = Work_String_Remainder.Split(new string[] { Constants.DoubleQuote + Constants.DoubleQuote }, StringSplitOptions.None).Length - 1;

                    Work_String_Remainder.Replace(Constants.DoubleQuote + Constants.DoubleQuote, "&*");
                    Work_String_Remainder.Replace(Constants.DoubleQuote, Constants.Space);
                }

                if (Double_Quote_Count > 1)
                {
                    Work_String_Remainder = regex.Replace(Work_String_Remainder, " \"", 1);
                }

                Work_String_Remainder = Work_String_Remainder.Replace("&*", Constants.doubleQuoteAndSpace);

                inputValue = Work_String_Remainder;
            }
            return inputValue;
        }


        /// <summary>
        /// Check Ascii Value in InputFile Record and replace Ascii Character è to e
        /// </summary>
        /// <param name="lbl"></param>
        public static void CheckAccentE(InputFile lbl)
        {
            string accent_E = Char.ConvertFromUtf32(Constants.Accent_E_AsciiValue);

            var properties = lbl.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (!property.Name.Equals("Item", StringComparison.CurrentCultureIgnoreCase) && !property.Name.Equals("Common", StringComparison.CurrentCultureIgnoreCase) && !property.Name.Equals("Department", StringComparison.CurrentCultureIgnoreCase))
                {
                    string val = Convert.ToString(lbl[property.Name]);

                    if (val.EmptyNull().Contains(accent_E))
                    {
                        lbl[property.Name] = val.Replace(accent_E, Constants.EValue);
                    }
                }
            }
        }


        /// <summary>
        /// Convert Source Entity to Destination Entity
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static void Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            var props = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var type = typeof(TDestination);

            foreach (var prop in props)
            {
                if (!prop.Name.Equals("Item", StringComparison.CurrentCultureIgnoreCase) && !prop.Name.Equals("Common", StringComparison.CurrentCultureIgnoreCase))
                {
                    object value = prop.GetValue(source, null);

                    var prop2 = type.GetProperty(prop.Name);
                    if (prop2 == null)
                        continue;

                    if (prop.PropertyType != prop2.PropertyType)
                        continue;

                    prop2.SetValue(destination, value, null);
                }
            }
        }
    }
}
