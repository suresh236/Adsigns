using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vestcom.PaperSign.ACME.Entities;

namespace Vestcom.PaperSign.ACME.ProcessInput.Rules
{
    /// <summary>
    /// Process Input rule for ISM 
    /// </summary>
    public class ProcessISMInputRule
    {
        /// <summary>
        /// Applies the rules.
        /// </summary>
        /// <param name="inputFileRecord">The input file record.</param>
        /// <param name="records">The records.</param>
        public void ApplyRules(InputFile inputFileRecord, InputRecords records)
        {
            AssignSignDetails(inputFileRecord, records);
            ReadSignLays(inputFileRecord, records);
        }

        /// <summary>
        /// Assigns the sign details.
        /// </summary>
        /// <param name="lbl">The label.</param>
        /// <param name="records">The records.</param>
        private void AssignSignDetails(InputFile lbl, InputRecords records)
        {
            //Extract Sign Size from Image Name
            lbl.SL_SIGN_SIZE = GetSignSizeFromImageName(lbl.ImageName);

 
            string[] laminated = { "laminated", "y" };
            bool laminatedFlag = laminated.Contains(lbl.LaminationType.ToLower().EmptyNull().Trim()) ? true : false;

            string[] singleX = { "single", "simplex" };
            bool singleXFlag = singleX.Contains(lbl.Side.ToLower().EmptyNull().Trim()) ? true : false;

            bool rigidVinylFlag= lbl.rigidVinyl.EmptyNull().ToLower().Trim()=="y" ? true : false; ;

            string[] doubleX = { "double", "duplex" };
            bool doubleXFlag = doubleX.Contains(lbl.Side.ToLower().EmptyNull().Trim()) ? true : false;

            if (singleXFlag && !laminatedFlag && !rigidVinylFlag)
            {
                lbl.SL_SIGN_HEADING = "3050";
            }
            else if (doubleXFlag && !laminatedFlag && !rigidVinylFlag)
            {
                lbl.SL_SIGN_HEADING = "3051";
            }
            else if (singleXFlag && laminatedFlag)
            {
                lbl.SL_SIGN_HEADING = "4050";
            }
            else if (doubleXFlag && laminatedFlag)
            {
                lbl.SL_SIGN_HEADING = "4051";
            }

            else if (singleXFlag && rigidVinylFlag)
            {
                lbl.SL_SIGN_HEADING = "6050";
            }
            else if (doubleXFlag && rigidVinylFlag)
            {
                lbl.SL_SIGN_HEADING = "6051";
            }



        }

        /// <summary>
        /// Gets the name of the sign size from image.
        /// </summary>
        /// <param name="imageName">Name of the image.</param>
        /// <returns></returns>
        private string GetSignSizeFromImageName(string imageName)
        {
            if (string.IsNullOrEmpty(imageName))
            {
                return string.Empty;
            }

            //char[] chars = imageName.ToUpper().ToCharArray();
            //int index = 0;
            //int X_Count = 0;
            //for (; index < chars.Length; index++)
            //{
            //    if (chars[index] == 'X')
            //    {
            //        X_Count++;
            //    }

            //    if ((!Char.IsDigit(chars[index]) && !chars[index].Equals('X')) || (chars[index].Equals('X') && X_Count > 1))
            //    {
            //        break;
            //    }
            //}

            //string signSize = imageName.Substring(0, 4);
            //if (signSize.ToString().ToLower() == "11x7")
            //{
            //    return "0002";
            //}
            //else
            //{
            //    signSize = imageName.Substring(0, 5);
            //}
            //signSize = imageName.Substring(0, 3);
            //if (signSize.ToString().ToLower() == "5x3")
            //{
            //    return "0004";
            //}
            //else
            //{
            //    signSize = imageName.Substring(0, 5);
            //}
            //signSize = imageName.Substring(0, 6);
            //if (signSize.ToString().ToLower() == "14x8.5")
            //{
            //    return "0013";
            //}
            //else
            //{
            //    signSize = imageName.Substring(0, 5);
            //}
            //signSize = imageName.Substring(0, 7);
            //if (signSize.ToString().ToLower() == "8.5x3.5")
            //{
            //    return "0001";
            //}
            //else
            //{
            //    signSize = imageName.Substring(0, 5);
            //}
            //string signSize = imageName.EmptyNull().Trim().Split('_').ElementAtOrDefault(0).ConvertStringToInteger();
            string signSize = imageName.EmptyNull().Trim().Split('_').ElementAtOrDefault(0).ToString();
            switch (signSize.ToString().ToLower())
            {
                case "85x35":
                    return "0001";
                case "55x35":
                    return "0003";
                case "14x11":
                    return "0005";
                case "6x2.5":
                    return "0006";
                case "11x14":
                    return "0008";
                case "11x10":
                    return "0009";
                case "85x11":
                    return "0010";
                case "12x18":
                    return "0011";
                case "538x3":
                    return "0012";
                case "11x17":
                    return "0014";
                case "48x12":
                    return "0015";
                case "48x18":
                    return "0016";
                case "11x7":
                    return "0002";
                case "8.5x3.5":
                    return "0001";
                case "8.5x3.5w":
                    return "1001";
                case "11x7w":
                    return "1002";
                case "5x3":
                    return "0004";
                case "14x8.5":
                    return "0013";
                case "8.5x11":
                    return "0010";
                case "12x18w":
                    return "1011";
                case "5.38x3.38":
                    return "0012";
                case "11x8.5w":
                    return "0017";
                case "11x3.5":
                    return "0018";
                case "5x5":
                    return "0019";
                case "12x10":
                    return "0021";
                case "11x8.5":
                    return "0019";
                case "8.5x11w":
                    return "1017";
            }
            return string.Empty;
        }

        /// <summary>
        /// Reads the sign lays.
        /// </summary>
        /// <param name="lbl">The label.</param>
        /// <param name="records">The records.</param>
        private void ReadSignLays(InputFile lbl, InputRecords records)
        {
            if (String.IsNullOrEmpty(lbl.SL_SIGN_SIZE) || String.IsNullOrEmpty(lbl.SL_SIGN_HEADING))
            {
                WriteExceptions(lbl, "Sign Size/Sign Head not present.", "SignSize/SignHead", records);
                return;
            }

            SignLayout signLay = records.SignLayouts.FirstOrDefault(s => s.SL_SIGN_SIZE.PadLeft(4, '0') == lbl.SL_SIGN_SIZE.PadLeft(4, '0') && s.SL_SIGN_HEADING.PadLeft(4, '0') == lbl.SL_SIGN_HEADING.PadLeft(4, '0'));
            if (signLay != null)
            {
                lbl.SL_LAYOUT_NO = signLay.SL_LAYOUT_NO;
                lbl.SL_PAPER_TYPE = signLay.SL_PAPER_TYPE;
                lbl.SignSizeId = signLay.SignSizeId;
                lbl.SignHeaderId = signLay.SignHeaderId;
            }
            else
            {
                lbl.MSG_MESSAGE = string.Empty;
                lbl.MSG_MESSAGE = "No Layout-Stock record for Size = " + lbl.SL_SIGN_SIZE + ", Heading = " + lbl.SL_SIGN_HEADING + " - stopping.";
                WriteExceptions(lbl, lbl.MSG_MESSAGE, "SL_SIGN_SIZE", records);
            }
        }

        /// <summary>
        ///Write Exceptions
        /// </summary>
        /// <param name="records"></param>
        /// <param name="lbl"></param>
        /// <param name="exceptionDescription"></param>
        /// <param name="fieldName"></param>
        private void WriteExceptions(InputFile lbl, string exceptionDescription, string fieldName, InputRecords records)
        {
            lbl.MSG_MESSAGE = exceptionDescription;
            ExceptionReport exceptions = new ExceptionReport();

            //Keep sign id as -1 if it is a child record
            exceptions.SignId = lbl.SignDataId;
            exceptions.OrderId = String.IsNullOrEmpty(lbl.ORDER_ID) ? 0 : Helper.ConvertStringToInteger(lbl.ORDER_ID);
            exceptions.ExceptionDescription = exceptionDescription;
            exceptions.ExceptionStatus = "Information";
            exceptions.FieldName = fieldName;
            exceptions.RowNum = ProcessInput.recordNumber;
            records.Exceptions.Add(exceptions);
        }
    }
}
