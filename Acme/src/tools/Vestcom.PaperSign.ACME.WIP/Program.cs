using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Vestcom.PaperSign.ACME.WIP.Data;

namespace Vestcom.PaperSign.ACME.WIP
{
    class Program
    {
        static void Main(string[] args)
        {
            //args = new []{"2", "2678507" };  //For testing 

            if (args.Length == 2 && int.TryParse(args[0], out var clientId) && int.TryParse(args[1], out var signId))
            {
                Dictionary<string, string> downBackData = new PaperSignsDataContext().GetACMERecord(clientId, signId);
                foreach (var keyValuePair in downBackData.ToArray())
                {
                    downBackData[keyValuePair.Key] = ConvertStringToUnicode(downBackData[keyValuePair.Key]);    //Convert the key in the downBackData to unicode to be able to serialize downBackData
                }

                Console.WriteLine(JsonConvert.SerializeObject(downBackData, Formatting.Indented).Replace("\\\\", @"\"));
            }
        }

        /// <summary>
        /// Converts the string to unicode for serializing.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public static string ConvertStringToUnicode(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }

            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in s)
            {
                if (c > 160 || c == 92) // Special characters and the '\'
                {
                    stringBuilder.Append(@"\u" + ((int)c).ToString("X4"));
                }
                else
                {
                    stringBuilder.Append(c);
                }
            }
            return stringBuilder.ToString();
        }
    }
}
