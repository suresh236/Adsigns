using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vestcom.PaperSign.ACME.Entities;

namespace Vestcom.PaperSign.ACME.ProcessInput.Rules
{
    public class ProcessHolidayInputRule 
    {
        public void ApplyRules(HolidayInput lbl, HolidayInputRecords records, int clientid, string packagepath)
        {
            ReadHeaderAndSignLayFile(lbl, records);
        }


        public void ReadHeaderAndSignLayFile(HolidayInput inputRecord, HolidayInputRecords records)
        {
            inputRecord.SL_SIGN_SIZE = Constants.holidaySignCode;

            var sh = records.Headings.FirstOrDefault(s => s.SignHeaderDesc == inputRecord.Holiday);
            if (sh != null)
            {
                inputRecord.SL_SIGN_HEADING = sh.HEADING_NO;
                inputRecord.SignHeaderId = sh.ID;
            }

            if (string.IsNullOrEmpty(inputRecord.SL_SIGN_HEADING))
            {
                inputRecord.SL_SIGN_HEADING = string.Empty;
            }

            SignLayout signLay = records.SignLayouts.FirstOrDefault(s => s.SL_SIGN_SIZE.PadLeft(4, '0') == inputRecord.SL_SIGN_SIZE.PadLeft(4, '0') && s.SL_SIGN_HEADING.PadLeft(4, '0') == inputRecord.SL_SIGN_HEADING.PadLeft(4, '0'));
            if (signLay != null)
            {
                inputRecord.SignSizeId = signLay.SignSizeId;
                inputRecord.SignHeaderId = signLay.SignHeaderId;
            }
            else
            {
                inputRecord.MSG_MESSAGE = string.Empty;
                inputRecord.MSG_MESSAGE = "No Layout-Stock record for Size = " + inputRecord.SL_SIGN_SIZE + ", Heading = " + inputRecord.SL_SIGN_HEADING + " - stopping.";
                WriteExceptions(inputRecord, inputRecord.MSG_MESSAGE, "SL_SIGN_SIZE", records);
            }

        }


        private void WriteExceptions(HolidayInput lbl, string exceptionDescription, string fieldName, HolidayInputRecords records)
        {
            lbl.MSG_MESSAGE = exceptionDescription;
            ExceptionReport exceptions = new ExceptionReport();
            exceptions.SignId = lbl.DATA_NUM_IN;
            exceptions.OrderId = String.IsNullOrEmpty(lbl.OrderID) ? 0 : Helper.ConvertStringToInteger(lbl.OrderID);
            exceptions.ExceptionDescription = exceptionDescription;
            exceptions.ExceptionStatus = "Information";
            exceptions.FieldName = fieldName;
            exceptions.RowNum = ProcessInput.recordNumber;
            records.Exceptions.Add(exceptions);
        }


    }
}
