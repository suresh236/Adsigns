using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Vestcom.PaperSign.ACME.Entities;

namespace Vestcom.PaperSign.ACME.ProcessDownback.Rules
{

    public class ProcessHolidayDownbackRule : IHolidayBusinessRule
    {
        private DataTable dtDownBackColumnList;
        string strFilePath;
        string strReportPath;

        void IHolidayBusinessRule.ExecuteRules(int clientId, ACMEHolidayDownbackRecords records, string inputFilePath, string strBatchID, string Action, string reportPath)
        {
            strFilePath = inputFilePath;
            strReportPath = reportPath;
            ProcessDownback.ORDERS_CREATED = 0;
            ProcessDownback.ORDER_SIGNS_CREATED = 0;
            dtDownBackColumnList = new DataTable("DownBackFileOutputHeader");
            CreateDownBackFileHeader(); // Function call
 
            // Read input data
            ACMEHolidayRecord inputFileRecords = new ACMEHolidayRecord();
            DeleteExistingReport(strReportPath);


            foreach (var inputfilerecords in records.DownBackInputRecords) // executes once for each inputfile record
            {
                if (inputFileRecords == null)
                {
                    inputFileRecords = inputfilerecords;
                }

                CreateOrdersAndSigns(inputfilerecords, records);
            }

            inputFileRecords.MSG_MESSAGE = string.Empty;
            inputFileRecords.MSG_MESSAGE = ProcessDownback.ORDERS_CREATED + " Orders and " + ProcessDownback.ORDER_SIGNS_CREATED
                + " Order-Signs have been created in Work Area ";
            inputFileRecords.MSG_CR = Constants.Y;
            Common.CALL_MSGDRVR(inputFileRecords, reportPath);
        }

        /// <summary>
        ///Process input record for Orders and Signs
        /// </summary>
        /// <param name="inputRecord"></param>
        public void CreateOrdersAndSigns(ACMEHolidayRecord recordToProcess, ACMEHolidayDownbackRecords records)
        {

            ReadHeaderAndSignLayFile(recordToProcess, records);

            recordToProcess.O_LAYOUT_NO = recordToProcess.SL_LAYOUT_NO;
            recordToProcess.O_PAPER_TYPE = recordToProcess.SL_PAPER_TYPE;
            recordToProcess.O_SIGN_SIZE = recordToProcess.SIGNSIZE_IN;
            recordToProcess.O_SIGN_HEAD = recordToProcess.SIGNHEAD_IN;

            recordToProcess.OS_KEY = recordToProcess.DATA_NUM_IN.ToString();

            recordToProcess.OS_STORE_NUMBER = recordToProcess.OS_STORE_NUMBER.PadLeft(5, '0');

            recordToProcess.D_Begin_Date = string.Format("{0:MM/dd/yyyy}", Helper.ConvertStringToDateTime(recordToProcess.D_Begin_Date));
            recordToProcess.D_End_Date = string.Format("{0:MM/dd/yyyy}", Helper.ConvertStringToDateTime(recordToProcess.D_End_Date));

            if (!string.IsNullOrEmpty(recordToProcess.Store_Hours_Open))
            {
                recordToProcess.D_Store_Hours = GetTime(recordToProcess.Store_Hours_Open) + " to " + GetTime(recordToProcess.Store_Hours_Close);
            }

            if (recordToProcess.RX_Hours_Open.EmptyNull().ToUpper() == "CLOSED")
            {
                recordToProcess.D_RX_Hours = "CLOSED";
            }
            else if (!string.IsNullOrEmpty(recordToProcess.RX_Hours_Open))
            {
                recordToProcess.D_RX_Hours = GetTime(recordToProcess.RX_Hours_Open) + " - " + GetTime(recordToProcess.RX_Hours_Close);
            }

            if (!string.IsNullOrEmpty(recordToProcess.RX_Hours_Open))
            {
                recordToProcess.D_RX_Image_Call = Path.Combine(Constants.ImageFolderpath, ("pharmacy" + Constants.IMAGEFILEEXTENSION));
            }

            GetVestcomTagType(recordToProcess, records);


            if (!recordToProcess.RECORD_SKIP)
            {
                WriteRecordtoDownBackFile(recordToProcess);
            }
            else
            {
                GenerateReportFile(recordToProcess, strReportPath);
            }


        }

        private string GetTime(string time)
        {
            time = time.Replace(":", ".");
            if (!string.IsNullOrEmpty(time))
            {
                int svalue = Helper.ConvertStringToInteger(time);
                string slimit = string.Empty;
                for (int i = 0; i < time.Length - 1; i++)
                {
                    slimit = slimit + "0";
                }
                slimit = 1 + slimit;
                int digit = Helper.ConvertStringToInteger(slimit);

                svalue = Helper.ConvertStringToInteger(Convert.ToString((Convert.ToDouble(svalue) / Convert.ToDouble(digit))).Replace(".", ""));
                string timeUom = "am";
                if (svalue >= 12)
                {
                    svalue = svalue >= 13 ? svalue - 12 : svalue;
                    timeUom = "pm";
                }

                return Convert.ToString(svalue).Replace(".", ":") + timeUom;
            }
            return time;
        }

        /// <summary>
        ///Reads Header and Signlayout file
        /// </summary>
        /// <param name="inputRecord"></param>
        public void ReadHeaderAndSignLayFile(ACMEHolidayRecord inputRecord, ACMEHolidayDownbackRecords records)
        {
            if (!(string.IsNullOrEmpty(inputRecord.SIGNSIZE_IN) && string.IsNullOrEmpty(inputRecord.SIGNHEAD_IN)))
            {

                Heading heading = records.Headings.FirstOrDefault(h => h.HEADING_NO.PadLeft(4, '0') == inputRecord.SIGNHEAD_IN.PadLeft(4, '0'));

                SignLayout signLay = records.SignLayouts.FirstOrDefault(s => s.SL_SIGN_SIZE.PadLeft(4, '0') == inputRecord.SIGNSIZE_IN.PadLeft(4, '0')
                                                                    && s.SL_SIGN_HEADING.PadLeft(4, '0') == inputRecord.SIGNHEAD_IN.PadLeft(4, '0'));
                if (signLay != null)
                {
                    inputRecord.SL_LAYOUT_NO = signLay.SL_LAYOUT_NO;
                    inputRecord.SL_PAPER_TYPE = signLay.SL_PAPER_TYPE;
                }
                else
                {
                    inputRecord.MSG_MESSAGE = string.Empty;
                    inputRecord.MSG_MESSAGE = "No Layout-Stock record for Size = " + inputRecord.SL_SIGN_SIZE + ", Heading = " + inputRecord.SL_SIGN_HEADING + " - stopping.";
                    inputRecord.RECORD_SKIP = true;
                }
            }
            else
            {
                inputRecord.MSG_MESSAGE = string.Empty;
                inputRecord.MSG_MESSAGE = $"Missing SignSize or Heading - stopping for {inputRecord.DATA_NUM_IN}.";
                inputRecord.RECORD_SKIP = true;

            }

        }

        /// <summary>
        /// Adds Header Row to Downback File 
        /// </summary>
        private void CreateDownBackFileHeader()
        {
            IEnumerable<PropertyInfo> Props = typeof(ACMEHolidayRecord).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(m => m.GetCustomAttributes((false)).Any(a => a.GetType() == typeof(HeaderAttribute)));
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                string colname = prop.Name.Replace('_', '-');
                dtDownBackColumnList.Columns.Add(colname);
            }

            if (File.Exists(strFilePath))
            {
                File.Delete(strFilePath);
            }

            StringBuilder content = new StringBuilder();
            // Adding Column Header to downback file
            for (int col = 0; col < dtDownBackColumnList.Columns.Count; col++)
            {
                content.Append(dtDownBackColumnList.Columns[col].ColumnName);
                if (col < dtDownBackColumnList.Columns.Count - 1)
                {
                    content.Append(Constants.pipeSeparator);
                }
            }
            File.WriteAllText(strFilePath, content.ToString());
        }

        /// <summary>
        /// Gets down back file columns.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetDownBackFileColumns()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            IEnumerable<PropertyInfo> properties = typeof(AcmeRecord).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(m => m.GetCustomAttributes((false)).Any(a => a.GetType() == typeof(HeaderAttribute)));
            foreach (PropertyInfo prop in properties)
            {
                string column = prop.Name.Replace('_', '-');
                dictionary.Add(column, string.Empty);
            }
            return dictionary;
        }

        /// <summary>
        ///Writes Processed input record to Downback File 
        /// </summary>
        /// <param name="record"></param>
        private void WriteRecordtoDownBackFile(ACMEHolidayRecord record)
        {

            if (File.Exists(strFilePath))
            {
                StringBuilder content = new StringBuilder();
                content.Append(Environment.NewLine);
                for (int col = 0; col < dtDownBackColumnList.Columns.Count; col++)
                {
                    string d = Convert.ToString(record[dtDownBackColumnList.Columns[col].ColumnName.Replace('-', '_')]);

                    content.Append(d);
                    if (col < dtDownBackColumnList.Columns.Count - 1)
                    {
                        content.Append(Constants.pipeSeparator);
                    }
                }
                File.AppendAllText(strFilePath, content.ToString());
            }

        }

        /// <summary>
        /// Deletes the existing report.
        /// </summary>
        /// <param name="reportFilePath">The report file path.</param>
        public static void DeleteExistingReport(string reportFilePath)
        {
            File.Delete(reportFilePath + "\\Reports.txt");
        }


        /// <summary>
        /// Custom Function: Get VestcomTag Type function for setting Vestcom Tag based on Substitute Stock
        /// </summary>
        /// <param name="lbl"></param>
        public static void GetVestcomTagType(ACMEHolidayRecord lbl, ACMEHolidayDownbackRecords records)
        {

            if (!string.IsNullOrEmpty(lbl.ARTWORK))
            {
                lbl.D_BACKGROUND_IMAGE = lbl.ARTWORK;
            }

            SubstituteStock record;
            record = records.SubstituteStock.FirstOrDefault(s => s.MAIN_STOCK == lbl.SL_PAPER_TYPE);

            if (record != null)
            {
                //Set Default image if TYPESET_BACKGROUND_NAME is empty.
                if (string.IsNullOrEmpty(lbl.D_BACKGROUND_IMAGE) && !string.IsNullOrEmpty(record.BG_IMG))
                {
                    lbl.D_BACKGROUND_IMAGE = Path.Combine(Constants.ImageFolderpath, (record.BG_IMG + Constants.IMAGEFILEEXTENSION));
                }
                if (!string.IsNullOrEmpty(record.SUBSTITUTE_STOCK.EmptyNull().Trim()))
                {
                    lbl.D_Vestcom_Tag_Type = record.SUBSTITUTE_STOCK.EmptyNull().Trim();
                }
                else
                {
                    lbl.D_Vestcom_Tag_Type = lbl.SL_PAPER_TYPE;
                }

            }
            if (lbl.D_BACKGROUND_IMAGE.Length > Constants.MAX_PATH)
            {
                lbl.MSG_MESSAGE = string.Empty;
                lbl.MSG_MESSAGE = "Total Charachers in File Path is longer than " + Constants.MAX_PATH + " characters ...." + lbl.D_BACKGROUND_IMAGE;
                lbl.RECORD_SKIP = true;
            }
            else if (!File.Exists(lbl.D_BACKGROUND_IMAGE))
            {
                lbl.MSG_MESSAGE = lbl.MSG_MESSAGE + "ART IS MISSING!!! IMAGE NAME: " + lbl.D_BACKGROUND_IMAGE + " PLEASE CREATE AND ADD TO IMAGE LIBRARY ASAP.";
                lbl.RECORD_SKIP = true;
            }

        }

        /// <summary>
        ///Write data into Report File
        /// </summary>
        public static void GenerateReportFile(ACMEHolidayRecord skiprecords, string reportFilePath)
        {

            StringBuilder content = new StringBuilder();

            content.Append(Environment.NewLine);

            content.Append(skiprecords.MSG_MESSAGE + Environment.NewLine);

            File.AppendAllText(reportFilePath + "\\Reports.txt", content.ToString());

        }

    }


}
