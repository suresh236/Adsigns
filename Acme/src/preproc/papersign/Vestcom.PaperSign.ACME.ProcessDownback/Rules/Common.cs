using System;
using Vestcom.PaperSign.ACME.Entities;
using System.IO;

namespace Vestcom.PaperSign.ACME.ProcessDownback.Rules
{
    public static class Common
    {

        /// <summary>
        ///Writes Rejected data to File
        /// </summary>
        /// <param name="inputRecord"></param>
        public static void CALL_MSGDRVR(AcmeRecord lbl, string strFilePath)
        {
            string rejectfilePath = Path.Combine(strFilePath, Constants.RejectFileName);
            string content = string.Format("MSG_CR= {0}, MSG_MESSAGE= {1}{2}", lbl.MSG_CR, lbl.MSG_MESSAGE, Environment.NewLine);

            if (File.Exists(rejectfilePath))
            {
                File.AppendAllText(rejectfilePath, content);
            }
            else
            {
                File.WriteAllText(rejectfilePath, content);
            }
        }

        /// <summary>
        ///Writes Rejected data to File
        /// </summary>
        /// <param name="inputRecord"></param>
        public static void CALL_MSGDRVR(ACMEHolidayRecord lbl, string strFilePath)
        {
            string rejectfilePath = Path.Combine(strFilePath, Constants.RejectFileName);
            string content = string.Format("MSG_CR= {0}, MSG_MESSAGE= {1}{2}", lbl.MSG_CR, lbl.MSG_MESSAGE, Environment.NewLine);

            if (File.Exists(rejectfilePath))
            {
                File.AppendAllText(rejectfilePath, content);
            }
            else
            {
                File.WriteAllText(rejectfilePath, content);
            }
        }

    }
}


