using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Vestcom.PaperSign.ACME.Entities;
using System.IO;
using System.Data;
using System.Reflection;
using System.Globalization;

namespace Vestcom.PaperSign.ACME.ProcessDownback.Rules
{
    /// <summary>
    /// Cobol File IMSAL708.CBL
    /// </summary>
    /// 
    public class ProcessDownbackRule : IBusinessRule
    {
        readonly DataTable dtDownBackColumnList = new DataTable("DownBackFileOutputHeader");
        string strFilePath;
        string strReportPath;


        /// <summary>
        /// Adds Header Row to Downback File 
        /// </summary>

        private void CreateDownBackFileHeader()
        {

            IEnumerable<PropertyInfo> Props = typeof(AcmeRecord).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(m => m.GetCustomAttributes((false)).Any(a => a.GetType() == typeof(HeaderAttribute)));
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
        private void WriteRecordtoDownBackFile(AcmeRecord record)
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
        /// Execute Business Rules on Input File which is store in DataBase
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="orderNumber"></param>
        /// <param name="inputFilePath"></param>
        public void ExecuteRules(int clientId, string inputFilePath, string strBatchID, string Action, string reportPath)
        {
            strFilePath = inputFilePath;
            strReportPath = reportPath;
            ProcessDownback.ORDERS_CREATED = 0;
            ProcessDownback.ORDER_SIGNS_CREATED = 0;

            CreateDownBackFileHeader(); // Function call

            // Read input data
            IEnumerable<AcmeRecord> inputFileRecords = ProcessDownback.records.DownBackInputRecords.ToList();
            DeleteExistingReport(strReportPath);


            for (int i = 0; i < inputFileRecords.Count(); i++) // executes once for each inputfile record
            {
                CreateOrdersAndSigns(inputFileRecords.ElementAtOrDefault(i));
            }

            inputFileRecords.ElementAtOrDefault(0).MSG_MESSAGE = string.Empty;
            inputFileRecords.ElementAtOrDefault(0).MSG_MESSAGE = ProcessDownback.ORDERS_CREATED + " Orders and " + ProcessDownback.ORDER_SIGNS_CREATED + " Order-Signs have been created in Work Area " + inputFileRecords.ElementAtOrDefault(0).PASS_WORK_AREA + Constants.Dot;
            inputFileRecords.ElementAtOrDefault(0).MSG_CR = Constants.Y;
            Common.CALL_MSGDRVR(inputFileRecords.ElementAtOrDefault(0), reportPath);


        }


        /// <summary>
        ///Process input record for Orders and Signs
        /// </summary>
        /// <param name="inputRecord"></param>
        public void CreateOrdersAndSigns(AcmeRecord recordToProcess)
        {


            recordToProcess.OS_ZONES = "0";
            recordToProcess.DEPT_NAME_HOLD = string.Empty;
            recordToProcess.FROM_DATE_HOLD = string.Empty;
            recordToProcess.END_DATE_HOLD = string.Empty;
            recordToProcess.OS_STORE_NUMBER = recordToProcess.StoreNo;

            // Initializing MFL-SIZE & EDLP-SIZE
            recordToProcess.MFL_SIZE = recordToProcess.MFL_SIZE2_IN + recordToProcess.MFL_SIZE1_IN + recordToProcess.MFL_SIZE3_IN + recordToProcess.MFL_SIZE6_IN;
            recordToProcess.EDLP_SIZE = recordToProcess.EDLP_SIZE2_IN + recordToProcess.EDLP_SIZE1_IN + recordToProcess.EDLP_SIZE3_IN + recordToProcess.EDLP_SIZE6_IN;

            ReadHeaderAndSignLayFile(recordToProcess); // function call

            CREATE_ORDER_SIGN(recordToProcess); // function call
            recordToProcess.D_DEPT_DESC = recordToProcess.DEPT_NAME_IN;
            recordToProcess.D_PROMO_STARTDATE = recordToProcess.FROM_DATE_IN;
            if (string.IsNullOrEmpty(recordToProcess.OS_SAVINGS) || recordToProcess.OS_SAVINGS.EmptyNull().Trim() == "0")
            {
                recordToProcess.OS_SAVINGS = "0.00";
            }
            GetVestcomTagType(recordToProcess);

            if (recordToProcess.Extreme.ToUpper() == "Y")
            {
                recordToProcess.O_PAPER_TYPE = (Helper.ConvertStringToInteger(recordToProcess.O_PAPER_TYPE) + 200).ToString();
                recordToProcess.D_Vestcom_Tag_Type = (Helper.ConvertStringToInteger(recordToProcess.D_Vestcom_Tag_Type) + 200).ToString();
            }
            if (new string[] { "4050", "4051" }.Contains( recordToProcess.O_SIGN_HEAD))
            {
                recordToProcess.D_LaminationType = "Y";
            }
            else
            {
                recordToProcess.D_LaminationType =string.Empty;
            }


            if (new string[] { "6050", "6051" }.Contains(recordToProcess.O_SIGN_HEAD))
            {
                recordToProcess.D_RigidVinyl = "Y";
                recordToProcess.D_LaminationType = "R";
            }
            else
            {
                recordToProcess.D_RigidVinyl = string.Empty;
            }


            if (!recordToProcess.RECORD_SKIP)
            {
                WriteRecordtoDownBackFile(recordToProcess);
            }
            else
            {
                GenerateReportFile(recordToProcess, strReportPath);
            }


        }

        /// <summary>
        /// Processes the order and sign.
        /// </summary>
        /// <param name="recordToProcess">The record to process.</param>
        /// <param name="columnsToValues">The columns to values.</param>
        public void ProcessOrderAndSign(AcmeRecord recordToProcess, Dictionary<string, string> columnsToValues)
        {
            if (recordToProcess == null)
            {
                return;
            }

            recordToProcess.OS_ZONES = "0";
            recordToProcess.DEPT_NAME_HOLD = string.Empty;
            recordToProcess.FROM_DATE_HOLD = string.Empty;
            recordToProcess.END_DATE_HOLD = string.Empty;
            recordToProcess.OS_STORE_NUMBER = recordToProcess.StoreNo;

            // Initializing MFL-SIZE & EDLP-SIZE
            recordToProcess.MFL_SIZE = recordToProcess.MFL_SIZE2_IN + recordToProcess.MFL_SIZE1_IN + recordToProcess.MFL_SIZE3_IN + recordToProcess.MFL_SIZE6_IN;
            recordToProcess.EDLP_SIZE = recordToProcess.EDLP_SIZE2_IN + recordToProcess.EDLP_SIZE1_IN + recordToProcess.EDLP_SIZE3_IN + recordToProcess.EDLP_SIZE6_IN;

            ReadHeaderAndSignLayFile(recordToProcess); // function call

            CREATE_ORDER_SIGN(recordToProcess); // function call
            recordToProcess.D_DEPT_DESC = recordToProcess.DEPT_NAME_IN;
            recordToProcess.D_PROMO_STARTDATE = recordToProcess.FROM_DATE_IN;
            if (string.IsNullOrEmpty(recordToProcess.OS_SAVINGS) || recordToProcess.OS_SAVINGS.EmptyNull().Trim() == "0")
            {
                recordToProcess.OS_SAVINGS = "0.00";
            }

            GetVestcomTagType(recordToProcess);
            List<string> keys = columnsToValues.Keys.ToList();
            foreach (string key in keys)
            {
                columnsToValues[key] = Convert.ToString(recordToProcess[key.Replace('-', '_')]);
            }
        }

        /// <summary>
        ///Write data into Report File
        /// </summary>
        public static void GenerateReportFile(AcmeRecord skiprecords, string reportFilePath)
        {

            StringBuilder content = new StringBuilder();

            content.Append(Environment.NewLine);

            content.Append(skiprecords.MSG_MESSAGE + Environment.NewLine);

            File.AppendAllText(reportFilePath + "\\Reports.txt", content.ToString());

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
        ///Process input record to Create Orders and OrderSigns
        /// </summary>
        /// <param name="inputRecord"></param>
        public void CREATE_ORDER_SIGN(AcmeRecord inputRecord)
        {

            if ((inputRecord.DEPT_NAME_IN.Equals("BAKERY", StringComparison.InvariantCultureIgnoreCase) && inputRecord.HEADING_BAKERY_FLAG == "Y") ||
                            (inputRecord.DEPT_NAME_IN.Equals("FLORAL", StringComparison.InvariantCultureIgnoreCase) &&
                             inputRecord.HEADING_FLORAL_FLAG == "Y") ||
                            (inputRecord.DEPT_NAME_IN.Equals("G M & H B C", StringComparison.InvariantCultureIgnoreCase) &&
                             inputRecord.HEADING_GM_FLAG == "Y") ||
                            (new string[] { "GROCERY", "GR", "grocery", "Custom Signs", "CUSTOM SIGNS" }.Contains(inputRecord.DEPT_NAME_IN) &&
                             inputRecord.HEADING_GROCERY_FLAG == "Y") ||
                            (inputRecord.DEPT_NAME_IN.Equals("MEAT & BUTCHER BLOCK", StringComparison.InvariantCultureIgnoreCase) &&
                             inputRecord.HEADING_FRESH_MEAT_FLAG == "Y") ||
                            (inputRecord.DEPT_NAME_IN.Equals("MEAT DELI", StringComparison.InvariantCultureIgnoreCase) &&
                             inputRecord.HEADING_PKGD_MEAT_FLAG == "Y") ||
                            (inputRecord.DEPT_NAME_IN.Equals("PRODUCE", StringComparison.InvariantCultureIgnoreCase) &&
                             inputRecord.HEADING_PRODUCE_FLAG == "Y") ||
                             (inputRecord.DEPT_NAME_IN.Equals("SERVICE DELI", StringComparison.InvariantCultureIgnoreCase) &&
                             inputRecord.HEADING_SRVC_DELI_FLAG == "Y") ||
                             (inputRecord.DEPT_NAME_IN.EmptyNull().Trim().ToUpper() == Constants.Grocery_KeHe && inputRecord.HEADING_SRVC_DELI_FLAG == Constants.Y) ||
                            (inputRecord.DEPT_NAME_IN.Equals("SEAFOOD", StringComparison.InvariantCultureIgnoreCase) && inputRecord.HEADING_SEAFOOD_FLAG == "Y"))
            {
                inputRecord.RECORD_SKIP = false;
            }
            else
            {
                inputRecord.RECORD_SKIP = true;
                return;
            }

            if (String.IsNullOrEmpty(inputRecord.SIGNSIZE_IN) || String.IsNullOrEmpty(inputRecord.SIGNHEAD_IN))
            {
                inputRecord.RECORD_SKIP = true;
                inputRecord.MSG_MESSAGE = "Sign Size/Sign Head not present.";
            }


            inputRecord.OS_ORDER_NUMBER = inputRecord.OrderNumber;
            inputRecord.OS_KEY = inputRecord.DATA_NUM_IN.ToString();

            if (inputRecord.DEPT_NAME_IN.ToUpper() != inputRecord.DEPT_NAME_HOLD.ToUpper())
            {
                inputRecord.DEPT_NAME_HOLD = inputRecord.DEPT_NAME_IN;

                CreateOrder(inputRecord); // Function call
            }

            inputRecord.OS_COMMENT10 = inputRecord.DATA_NUM_IN.ToString();

            inputRecord.OS_QUANTITY = inputRecord.SIGN_QTY_IN;

            inputRecord.WORK_DATE = string.Empty;

            if (!string.IsNullOrEmpty(inputRecord.END_DATE_IN))
            {
                inputRecord.OS_PULL_DATE = Convert.ToDateTime(inputRecord.END_DATE_IN).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            }

            inputRecord.OS_ZONES = inputRecord.ZONE_DATA_IN;

            //if (string.IsNullOrEmpty(inputRecord.ZONE_DATA_IN))
            //    {
            //        inputRecord.OS_ZONES = Constants.OS_ZONES;
            //    }
            //    else
            //    {
            //        inputRecord.OS_ZONES = inputRecord.ZONE_DATA_IN;
            //    }

            inputRecord.OS_BP_COMMENT = inputRecord.ITEM_DESC_IN;
            inputRecord.OS_BRAND_NAME = inputRecord.PROD_BRAND_IN;

            //if (inputRecord.PROD_BRAND_IN == Constants.Coke || inputRecord.PROD_BRAND_IN == Constants.Pepsi || inputRecord.PROD_BRAND_IN == Constants.Mtn_Dew) // line no 905
            //{
            //    inputRecord.OS_BRAND_NAME = inputRecord.PROD_BRAND_IN.EmptyNull().Split(new string[] { "   " }, StringSplitOptions.None).ElementAtOrDefault(0) + ", " + inputRecord.ITEM_DESC_IN;
            //}
            //else
            //{
            //    if (!string.IsNullOrEmpty(inputRecord.O_DEPARTMENT_NO) && inputRecord.O_DEPARTMENT_NO.PadLeft(2, '0') == "03" && inputRecord.SL_SIGN_SIZE.PadLeft(4, '0') == Constants._0001 && (inputRecord.PROD_BRAND_IN == Constants.Lancaster_Brand_Beef || inputRecord.PROD_BRAND_IN == Constants.Hatfield_Simply_Tender))
            //    {
            //        inputRecord.OS_BP_COMMENT = inputRecord.PROD_BRAND_IN;
            //        inputRecord.OS_BRAND_NAME = inputRecord.ITEM_DESC_IN;
            //    }
            //    else
            //    {
            //        inputRecord.OS_BRAND_NAME = inputRecord.PROD_BRAND_IN.EmptyNull().Split(new string[] { "   " }, StringSplitOptions.None).ElementAtOrDefault(0) + " " + inputRecord.ITEM_DESC_IN;
            //    }
            //}

            if (!string.IsNullOrEmpty(inputRecord.UPC_IN))
            {
                long upc;
                if (!long.TryParse(inputRecord.UPC_IN, out upc))
                {
                    inputRecord.MSG_MESSAGE = "Incorrect Upc format.Upc value not numeric.";
                    inputRecord.RECORD_SKIP = true;
                }


                inputRecord.UPC_ANALYSIS1 = inputRecord.UPC_IN;
                ProcessUpc(inputRecord); // function call
            }

            inputRecord.OS_PRODUCT_NAME = inputRecord.SIZE_IN;
            inputRecord.OS_EMPHASIS = Constants.B;

            if (!string.IsNullOrEmpty(inputRecord.SECOND_MSG_IN) && inputRecord.SECOND_MSG_IN.Trim().ToUpper() != Constants.MONOPOLY)
            {
                inputRecord.OS_BRAND_NAME2 = inputRecord.SECOND_MSG_IN;
            }

            if (!string.IsNullOrEmpty(inputRecord.LIMIT_IN.Trim()))
            {
                inputRecord.LIMIT_COUNT = 0;
                inputRecord.LIMIT_COUNT = Regex.Matches(inputRecord.SECOND_MSG_IN.ToUpper(), Constants.Limit).Count;
                if (inputRecord.LIMIT_COUNT == 0)
                {
                    inputRecord.OS_PACKAGE_TYPE = Constants.Limit + " " + inputRecord.LIMIT_IN;
                }
            }

            if (!string.IsNullOrEmpty(inputRecord.MUST_BUY_IN))
            {
                inputRecord.MUST_BUY_IN = inputRecord.MUST_BUY_IN.Replace('$', ' ');
                inputRecord.MUST_BUY_IN = inputRecord.MUST_BUY_IN.Replace(".00", "   ");
                inputRecord.OS_COMMENT7 = Constants.Must_buy + " " + inputRecord.MUST_BUY_IN;
            }

            if (inputRecord.HEADING_BOGO_FLAG.Trim().ToUpper() == Constants.Y && !string.IsNullOrEmpty(inputRecord.BOGO_TYPE_INFO_IN.EmptyNull().Trim()))
            {

                if (inputRecord.BOGO_TYPE_INFO_IN.ToUpper().Trim() == Constants.BOGO)
                {
                    inputRecord.OS_COMMENT5 = Constants.Buy1Get1;
                    inputRecord.BOGO_BUY_COUNT_DISPLAY = 2;
                }
                else
                {
                    inputRecord.BOGO_WORK_AREA = string.Empty;
                    inputRecord.BOGO_BUY_COUNT = 0;
                    inputRecord.BOGO_GET_COUNT = 0;

                    inputRecord.DUMMY = inputRecord.BOGO_TYPE_INFO_IN.EmptyNull().Split(new string[] { "B", "G", " " }, StringSplitOptions.None).ElementAtOrDefault(0);

                    inputRecord.BOGO_BUY_COUNT = inputRecord.BOGO_TYPE_INFO_IN.EmptyNull().Split(new string[] { "B", "G", " " }, StringSplitOptions.None).ElementAtOrDefault(1).ConvertStringToInteger();
                    inputRecord.BOGO_GET_COUNT = inputRecord.BOGO_TYPE_INFO_IN.EmptyNull().Split(new string[] { "B", "G", " " }, StringSplitOptions.None).ElementAtOrDefault(2).ConvertStringToInteger();

                    inputRecord.BOGO_BUY_COUNT_DISPLAY = inputRecord.BOGO_BUY_COUNT;
                    inputRecord.BOGO_GET_COUNT_DISPLAY = inputRecord.BOGO_GET_COUNT;

                    inputRecord.OS_COMMENT5 = "Buy " + inputRecord.BOGO_BUY_COUNT_DISPLAY + " Get " + inputRecord.BOGO_GET_COUNT_DISPLAY;
                    inputRecord.BOGO_BUY_COUNT = inputRecord.BOGO_BUY_COUNT + inputRecord.BOGO_GET_COUNT;
                    inputRecord.BOGO_GET_COUNT_DISPLAY = inputRecord.BOGO_GET_COUNT;
                }

                inputRecord.PRICE_IN = inputRecord.SALE_PR_IN;
                ProcessPrice(inputRecord); // function call
                inputRecord.OS_SPECIAL_PRICE = inputRecord.WORK_PRICE;
                inputRecord.OS_SPECIAL_PRICE = string.Format("{0:0.00}", Helper.ConvertStringToDouble(inputRecord.OS_SPECIAL_PRICE), 2);

                if (inputRecord.SALE_MULTI_IN == 0)
                {
                    inputRecord.SALE_MULTI_IN = 1;
                }

                inputRecord.OS_SPECIAL_FOR_NUMERIC = inputRecord.SALE_MULTI_IN;
                inputRecord.OS_SPECIAL_FOR = inputRecord.OS_SPECIAL_FOR_NUMERIC.ToString();
                inputRecord.OS_COMMENT6 = Constants.FREE;
                inputRecord.OS_SPECIAL_UNITS = "04";
                if (inputRecord.MUST_BUY_IN.EmptyNull().Trim() != string.Empty)
                {
                    inputRecord.OS_COMMENT4 = Constants.When_you_buy + inputRecord.MUST_BUY_IN;
                }
                else
                {
                    inputRecord.OS_COMMENT4 = Constants.When_you_buy + inputRecord.BOGO_BUY_COUNT_DISPLAY;
                }

                inputRecord.PRICE_IN = inputRecord.SAVINGS_IN;

                ProcessPrice(inputRecord); // function call

                inputRecord.OS_SAVINGS = inputRecord.WORK_PRICE;
                inputRecord.OS_SAVINGS = string.Format("{0:0.00}", Helper.ConvertStringToDouble(inputRecord.OS_SAVINGS), 2);
                ComputeUnitPrice(inputRecord); // function call
            }


            if (inputRecord.HEADING_SALE_FLAG.Trim().ToUpper() == Constants.Y && string.IsNullOrEmpty(inputRecord.BOGO_TYPE_INFO_IN.EmptyNull().Trim()) && string.IsNullOrEmpty(inputRecord.PERCENT_IN.EmptyNull().Trim()))
            {

                inputRecord.PRICE_IN = inputRecord.SALE_PR_IN;

                ProcessPrice(inputRecord); // function call

                inputRecord.OS_SPECIAL_PRICE = inputRecord.WORK_PRICE;
                inputRecord.OS_SPECIAL_PRICE = string.Format("{0:0.00}", Helper.ConvertStringToDouble(inputRecord.OS_SPECIAL_PRICE), 2);

                if (!string.IsNullOrEmpty(inputRecord.OS_SPECIAL_PRICE.EmptyNull().Trim()) && inputRecord.OS_SPECIAL_PRICE.ConvertStringToDouble() > 0)
                {
                    if (inputRecord.SALE_MULTI_IN == 0)
                    {
                        inputRecord.SALE_MULTI_IN = 1;
                    }
                    inputRecord.OS_SPECIAL_FOR_NUMERIC = inputRecord.SALE_MULTI_IN;
                    inputRecord.OS_SPECIAL_FOR = inputRecord.OS_SPECIAL_FOR_NUMERIC.ToString();
                    inputRecord.OS_SPECIAL_UNITS = "04";

                    if (!string.IsNullOrEmpty(inputRecord.MUST_BUY_IN))
                    {
                        inputRecord.OS_COMMENT4 = Constants.When_you_buy + inputRecord.MUST_BUY_IN;
                    }
                    else
                    {
                        inputRecord.SALE_MULTI_DISPLAY = inputRecord.SALE_MULTI_IN;
                        inputRecord.OS_COMMENT4 = Constants.When_you_buy + inputRecord.SALE_MULTI_DISPLAY;
                    }

                    inputRecord.PRICE_IN = inputRecord.SAVINGS_IN;
                    ProcessPrice(inputRecord); // Function call

                    inputRecord.WORK_SAVINGS = inputRecord.WORK_PRICE;

                    inputRecord.OS_SAVINGS = Convert.ToString(inputRecord.WORK_SAVINGS.ConvertStringToDouble() * inputRecord.SALE_MULTI_IN);
                    inputRecord.OS_SAVINGS = string.Format("{0:0.00}", Helper.ConvertStringToDouble(inputRecord.OS_SAVINGS), 2);

                }
                ComputeUnitPrice(inputRecord); // function call
            }


            if (inputRecord.HEADING_PCNTOFF_FLAG.EmptyNull().Trim().ToUpper() == Constants.Y && !string.IsNullOrEmpty(inputRecord.PERCENT_IN))
            {

                inputRecord.OS_COMMENT2 = "%";
                inputRecord.OS_COMMENT1 = inputRecord.PERCENT_IN;
                inputRecord.PRICE_IN = inputRecord.SALE_PR_IN;
                ProcessPrice(inputRecord); // function call
                inputRecord.OS_SPECIAL_PRICE = inputRecord.WORK_PRICE;
                inputRecord.OS_SPECIAL_PRICE = string.Format("{0:0.00}", Helper.ConvertStringToDouble(inputRecord.OS_SPECIAL_PRICE), 2);

                if (!string.IsNullOrEmpty(inputRecord.OS_SPECIAL_PRICE) && inputRecord.OS_SPECIAL_PRICE.ConvertStringToDouble() > 0)
                {
                    if (inputRecord.SALE_MULTI_IN == 0)
                    {
                        inputRecord.SALE_MULTI_IN = 1;
                    }
                    inputRecord.OS_SPECIAL_FOR_NUMERIC = inputRecord.SALE_MULTI_IN;
                    inputRecord.OS_SPECIAL_FOR = inputRecord.OS_SPECIAL_FOR_NUMERIC.ToString();

                    inputRecord.OS_SPECIAL_UNITS = "04";

                    if (!string.IsNullOrEmpty(inputRecord.MUST_BUY_IN))
                    {
                        inputRecord.OS_COMMENT4 = Constants.When_you_buy + inputRecord.MUST_BUY_IN;
                    }
                    else
                    {
                        inputRecord.SALE_MULTI_DISPLAY = inputRecord.SALE_MULTI_IN;
                        inputRecord.OS_COMMENT4 = Constants.When_you_buy + inputRecord.SALE_MULTI_DISPLAY;
                    }
                    inputRecord.PRICE_IN = inputRecord.SAVINGS_IN;
                    ProcessPrice(inputRecord); // Function call

                    inputRecord.WORK_SAVINGS = inputRecord.WORK_PRICE;
                    inputRecord.OS_SAVINGS = Convert.ToString(inputRecord.WORK_SAVINGS.ConvertStringToDouble() * inputRecord.SALE_MULTI_IN);
                    inputRecord.OS_SAVINGS = string.Format("{0:0.00}", Helper.ConvertStringToDouble(inputRecord.OS_SAVINGS), 2);

                }
                ComputeUnitPrice(inputRecord); // function call
            }

            if (inputRecord.HEADING_EDLP_FLAG.EmptyNull().Trim().ToUpper() == Constants.Y)
            {
                if (inputRecord.SALE_MULTI_IN == 0)
                {
                    inputRecord.SALE_MULTI_IN = 1;
                }
                inputRecord.OS_SPECIAL_FOR_NUMERIC = inputRecord.SALE_MULTI_IN;
                inputRecord.OS_SPECIAL_FOR = inputRecord.OS_SPECIAL_FOR_NUMERIC.ToString();

                inputRecord.PRICE_IN = inputRecord.SALE_PR_IN;
                ProcessPrice(inputRecord); // Function call
                inputRecord.OS_SPECIAL_PRICE = inputRecord.WORK_PRICE;
                inputRecord.OS_SPECIAL_PRICE = string.Format("{0:0.00}", Helper.ConvertStringToDouble(inputRecord.OS_SPECIAL_PRICE), 2);
                inputRecord.OS_SPECIAL_UNITS = "04";
                ComputeUnitPrice(inputRecord); // function call
            }

            if (!string.IsNullOrEmpty(inputRecord.UPC_IN))
            {
                inputRecord.UPC_ANALYSIS1 = inputRecord.UPC_IN;
                ProcessUpc(inputRecord); // function call
            }
            else
            {
                inputRecord.UPC_CODE = inputRecord.OS_KEY;
            }


        }


        /// <summary>
        ///Process the UPC Code
        /// </summary>
        /// <param name="inputRecord"></param>
        public void ProcessUpc(AcmeRecord inputRecord)
        {


            //all the variables are substring of UPC_ANALYSIS1 , are to be intialised
            inputRecord.UPC_ANAL1_PART1 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 0, 5);
            inputRecord.UPC_ANAL1_PART2 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 5, 5);
            inputRecord.UPC_ANAL1_PART3 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 10, 3);

            inputRecord.UPC_ANAL2_PART1 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 0, 5);
            inputRecord.UPC_ANAL2_MIDDLE = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 5, 1);
            inputRecord.UPC_ANAL2_PART2 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 6, 5);
            inputRecord.UPC_ANAL2_PART3 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 11, 2);


            inputRecord.UPC_ANAL3_PART1 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 0, 6);
            inputRecord.UPC_ANAL3_PART2 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 6, 5);
            inputRecord.UPC_ANAL3_PART3 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 11, 2);

            inputRecord.UPC_ANAL4_PART1 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 0, 1);
            inputRecord.UPC_ANAL4_DASH1 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 1, 1);
            inputRecord.UPC_ANAL4_PART2 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 2, 5);
            inputRecord.UPC_ANAL4_DASH2 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 7, 1);
            inputRecord.UPC_ANAL4_PART3 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 8, 5);

            inputRecord.UPC_ANAL5_PART1 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 0, 4);
            inputRecord.UPC_ANAL5_PART2 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 4, 9);

            inputRecord.UPC_ANAL6_PART1 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 0, 6);
            inputRecord.UPC_ANAL6_PART2 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 6, 1);
            inputRecord.UPC_ANAL6_PART3 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 7, 5);
            inputRecord.UPC_ANAL6_PART4 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 12, 1);


            inputRecord.UPC_ANAL7_PART1 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 0, 6);
            inputRecord.UPC_ANAL7_PART2 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 6, 1);
            inputRecord.UPC_ANAL7_PART3 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 7, 5);
            inputRecord.UPC_ANAL7_PART4 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 12, 1);

            inputRecord.UPC_ANAL8_PART1 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 0, 11);
            inputRecord.UPC_ANAL8_PART2 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 11, 2);

            long isnumber;

            if (long.TryParse(inputRecord.UPC_ANAL5_PART1, out isnumber) && (string.IsNullOrEmpty(inputRecord.UPC_ANAL5_PART2) || inputRecord.UPC_ANAL5_PART2 == "000000000"))
            {
                inputRecord.UPC_CODE = inputRecord.UPC_ANAL5_PART1;
            }
            else
            {
                if (long.TryParse(inputRecord.UPC_ANAL3_PART1, out isnumber) && (inputRecord.UPC_ANAL3_PART2 == "00000" || inputRecord.UPC_ANAL3_PART2 == "     ") && inputRecord.UPC_ANAL3_PART3 == "  ")
                {
                    inputRecord.UPC_CODE = inputRecord.UPC_ANAL3_PART1;
                }
                else
                {
                    if (long.TryParse(inputRecord.UPC_ANAL6_PART1, out isnumber) && inputRecord.UPC_ANAL6_PART2 == " " && inputRecord.UPC_ANAL6_PART3 == "00000" && inputRecord.UPC_ANAL6_PART4 == " ")
                    {
                        inputRecord.UPC_CODE = inputRecord.UPC_ANAL6_PART1;
                    }
                    else
                    {
                        if (long.TryParse(inputRecord.UPC_ANAL1_PART1, out isnumber) && long.TryParse(inputRecord.UPC_ANAL1_PART2, out isnumber) && inputRecord.UPC_ANAL1_PART3 == "   ")
                        {
                            inputRecord.UPC_CODE = "0" + inputRecord.UPC_ANAL1_PART1 + inputRecord.UPC_ANAL1_PART2;
                        }
                        else
                        {
                            if (long.TryParse(inputRecord.UPC_ANAL7_PART1, out isnumber) && inputRecord.UPC_ANAL7_PART2 == " " && long.TryParse(inputRecord.UPC_ANAL7_PART3, out isnumber) && inputRecord.UPC_ANAL7_PART4 == " ")
                            {
                                inputRecord.UPC_CODE = inputRecord.UPC_ANAL7_PART1 + inputRecord.UPC_ANAL7_PART3;
                            }
                            else
                            {
                                if (long.TryParse(inputRecord.UPC_ANAL8_PART1, out isnumber) && inputRecord.UPC_ANAL8_PART2 == "  ")
                                {
                                    inputRecord.UPC_CODE = inputRecord.UPC_ANAL8_PART1;
                                }
                                else
                                {
                                    if (inputRecord.UPC_ANAL4_DASH1 == "-" && inputRecord.UPC_ANAL4_DASH2 == "-")
                                    {
                                        if (inputRecord.UPC_ANAL4_PART3 == "00000")
                                        {
                                            inputRecord.UPC_CODE = inputRecord.UPC_ANAL4_PART1 + inputRecord.UPC_ANAL4_PART2;
                                        }
                                        else
                                        {
                                            inputRecord.UPC_CODE = inputRecord.UPC_ANAL4_PART1 + inputRecord.UPC_ANAL4_PART2 + inputRecord.UPC_ANAL4_PART3;
                                        }
                                    }
                                    else
                                    {
                                        if (long.TryParse(inputRecord.UPC_ANAL2_PART1, out isnumber) && long.TryParse(inputRecord.UPC_ANAL2_PART2, out isnumber) && inputRecord.UPC_ANAL2_PART3 == "  " && (inputRecord.UPC_ANAL2_MIDDLE == " " || inputRecord.UPC_ANAL2_MIDDLE == "-"))
                                        {
                                            inputRecord.UPC_CODE = inputRecord.UPC_ANAL2_PART1 + inputRecord.UPC_ANAL2_PART2;
                                        }
                                        else
                                        {
                                            inputRecord.UPC_CODE = inputRecord.UPC_IN;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }

        /// <summary>
        ///Process the work Price
        /// </summary>
        /// <param name="inputRecord"></param>
        public void ProcessPrice(AcmeRecord inputRecord)
        {

            inputRecord.WORK_PRICE = "0000000";
            inputRecord.PRICE_IN = inputRecord.PRICE_IN.EmptyNull().Replace('$', '0');
            inputRecord.PRICE_IN = Regex.Replace(inputRecord.PRICE_IN, @"^\s+|", match => match.Value.Replace(' ', '0'));

            if (!string.IsNullOrEmpty(inputRecord.PRICE_IN) && inputRecord.PRICE_IN.Contains('.'))
            {
                inputRecord.WORK_DOLLARS = inputRecord.PRICE_IN.EmptyNull().Split('.').ElementAtOrDefault(0);
                inputRecord.WORK_CENTS = inputRecord.PRICE_IN.EmptyNull().Split('.').ElementAtOrDefault(1);
                inputRecord.WORK_PRICE = inputRecord.WORK_DOLLARS + "." + inputRecord.WORK_CENTS;
            }
            else if (!string.IsNullOrEmpty(inputRecord.PRICE_IN) && inputRecord.PRICE_IN.Contains(' '))
            {
                inputRecord.WORK_DOLLARS = inputRecord.PRICE_IN.EmptyNull().Split(' ').ElementAtOrDefault(0);
                inputRecord.WORK_CENTS = inputRecord.PRICE_IN.EmptyNull().Split(' ').ElementAtOrDefault(1);
                inputRecord.WORK_PRICE = inputRecord.WORK_DOLLARS + "." + inputRecord.WORK_CENTS;
            }

        }

        /// <summary>
        ///Process the OS-PackageSize & OS-Unit Price
        /// </summary>
        /// <param name="inputRecord"></param>
        public void ComputeUnitPrice(AcmeRecord inputRecord)
        {

            inputRecord.UP_FIELD1 = string.Empty;
            inputRecord.UP_FIELD2 = string.Empty;
            inputRecord.UP_FIELD3 = string.Empty;
            inputRecord.UP_FIELD4 = string.Empty;


            inputRecord.UP_FIELD1 = inputRecord.UOM_IN.EmptyNull().Split(new string[] { " ", "/", "|" }, StringSplitOptions.None).ElementAtOrDefault(0);
            inputRecord.UP_FIELD2 = inputRecord.UOM_IN.EmptyNull().Split(new string[] { " ", "/", "|" }, StringSplitOptions.None).ElementAtOrDefault(1);
            inputRecord.UP_FIELD3 = inputRecord.UOM_IN.EmptyNull().Split(new string[] { " ", "/", "|" }, StringSplitOptions.None).ElementAtOrDefault(2);
            inputRecord.UP_FIELD4 = inputRecord.UOM_IN.EmptyNull().Split(new string[] { " ", "/", "|" }, StringSplitOptions.None).ElementAtOrDefault(3);

            inputRecord.UP_FIELD1 = string.IsNullOrEmpty(inputRecord.UP_FIELD1) ? string.Empty : inputRecord.UP_FIELD1;
            inputRecord.UP_FIELD2 = string.IsNullOrEmpty(inputRecord.UP_FIELD2) ? string.Empty : inputRecord.UP_FIELD2;
            inputRecord.UP_FIELD3 = string.IsNullOrEmpty(inputRecord.UP_FIELD3) ? string.Empty : inputRecord.UP_FIELD3;
            inputRecord.UP_FIELD4 = string.IsNullOrEmpty(inputRecord.UP_FIELD4) ? string.Empty : inputRecord.UP_FIELD4;

            if (string.IsNullOrEmpty(inputRecord.UP_FIELD2))
            {
                if (inputRecord.OS_SPECIAL_FOR_NUMERIC == 1)
                {
                    inputRecord.OS_PACKAGE_SIZE = inputRecord.UOM_IN;
                }

                if (!string.IsNullOrEmpty(inputRecord.UNIT_PR_IN) && !string.IsNullOrEmpty(inputRecord.UOM_IN))
                {
                    inputRecord.OS_UNIT_PRICE = "$" + string.Format("{0:0.00}", Helper.ConvertStringToDouble(inputRecord.UNIT_PR_IN.EmptyNull().Split(' ').ElementAtOrDefault(0))) + "/" + inputRecord.UOM_IN.Split(new string[] { "  " }, StringSplitOptions.None).ElementAtOrDefault(0);
                }
                return;
            }

            if (inputRecord.OS_SPECIAL_FOR_NUMERIC == 1 && string.IsNullOrEmpty(inputRecord.BOGO_TYPE_INFO_IN) && string.IsNullOrEmpty(inputRecord.PERCENT_IN))
            {
                if (!string.IsNullOrEmpty(inputRecord.UP_FIELD4))
                {
                    inputRecord.OS_PACKAGE_SIZE = inputRecord.UP_FIELD4;
                }
                else
                {
                    inputRecord.OS_PACKAGE_SIZE = inputRecord.UP_FIELD3;
                }
            }

            if (!string.IsNullOrEmpty(inputRecord.UP_FIELD1) && !string.IsNullOrEmpty(inputRecord.UP_FIELD2))
            {
                inputRecord.DASH_COUNT = 0;
                inputRecord.DASH_COUNT = inputRecord.UP_FIELD1.EmptyNull().Trim().Count(f => f == '-');
                if (inputRecord.DASH_COUNT > 0)
                {
                    inputRecord.WORK_FIELD = string.Empty;
                    inputRecord.WORK_FIELD = inputRecord.UP_FIELD1.EmptyNull().Split('-').ElementAtOrDefault(0);
                    inputRecord.UP_FIELD1 = inputRecord.WORK_FIELD;
                }

                //inputRecord.UP_CODE_IN = inputRecord.UP_FIELD2.Substring(0, 2);
                inputRecord.UP_CODE_IN = Helper.SafeSubstring(inputRecord.UP_FIELD2, 0, 2);
                inputRecord.UP_TYPE = string.Empty;

                switch (inputRecord.UP_CODE_IN.EmptyNull().ToUpper())
                {
                    case "LB":
                    case "OZ":
                        inputRecord.UP_TYPE = ".d";
                        break;

                    case "CT":
                        inputRecord.UP_TYPE = ".c";
                        break;

                    case "EA":
                        inputRecord.UP_TYPE = ".e";
                        break;

                    case "FL":
                    case "LT":
                        inputRecord.UP_TYPE = ".l";
                        break;

                    case "SQ":
                        inputRecord.UP_TYPE = ".a";
                        break;

                    case "FT":
                    case "FO":
                    case "YD":
                        inputRecord.UP_TYPE = ".f";
                        break;

                    case "PT":
                        inputRecord.UP_TYPE = ".p";
                        break;
                }


                inputRecord.PASS_PRICE = inputRecord.OS_SPECIAL_PRICE;

                inputRecord.OS_SPECIAL_FOR = inputRecord.OS_SPECIAL_FOR_NUMERIC.ToString();

                inputRecord.PASS_FOR = inputRecord.OS_SPECIAL_FOR;
                inputRecord.OS_UNIT_PRICE = string.Empty;
                inputRecord.OS_UNIT_PRICE = inputRecord.UP_TYPE + " " + inputRecord.UP_FIELD1.EmptyNull().Split(' ').ElementAtOrDefault(0) + " " + inputRecord.UP_FIELD2.EmptyNull().Split(' ').ElementAtOrDefault(0);

                UnitPric_Main(inputRecord); // function call to UNITPRIC.CBL main method
            }


        }

        /// <summary>
        ///Process order for input record
        /// </summary>
        /// <param name="inputRecord"></param>
        public void CreateOrder(AcmeRecord inputRecord)
        {
            inputRecord.WORK_WH_NUMBER = "01";

            inputRecord.O_DATE_ORDERED = DateTime.Now.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

            if (string.IsNullOrEmpty(inputRecord.DEPT_NAME_IN))
            {
                inputRecord.DEPT_NAME_IN = string.Empty;
            }

            Department dept = ProcessDownback.records.Departments.FirstOrDefault(dep => dep.DEPARTMENT_NAME.ToUpper() == inputRecord.DEPT_NAME_IN.ToUpper());
            if (dept != null)
            {
                inputRecord.O_DEPARTMENT_NO = dept.DEPARTMENT_NO;

                inputRecord.O_LAYOUT_NO = inputRecord.SL_LAYOUT_NO;
                inputRecord.O_PAPER_TYPE = inputRecord.SL_PAPER_TYPE;
                inputRecord.O_SIGN_SIZE = inputRecord.SL_SIGN_SIZE;
                inputRecord.IMAGE_FILE_SIGN_SIZE = inputRecord.SL_SIGN_SIZE;

                inputRecord.O_SIGN_HEAD = inputRecord.SL_SIGN_HEADING;
                inputRecord.IMAGE_FILE_SIGN_HEADING = inputRecord.SL_SIGN_HEADING;
                inputRecord.IMAGE_FILE_DEPT_NUMBER = inputRecord.O_DEPARTMENT_NO;

                Image img = ProcessDownback.records.Images.FirstOrDefault(s => s.IMAGE_FILE_DEPT_NUMBER == inputRecord.IMAGE_FILE_DEPT_NUMBER && s.IMAGE_FILE_SIGN_HEADING == inputRecord.IMAGE_FILE_SIGN_HEADING.PadLeft(4, '0') && s.IMAGE_FILE_SIGN_SIZE.PadLeft(4, '0') == inputRecord.IMAGE_FILE_SIGN_SIZE.PadLeft(4, '0'));
                if (img != null)
                {
                    inputRecord.O_IMAGE_CODE = img.IMAGE_FILE_IMAGE_CODE;
                }
                else
                {
                    inputRecord.O_IMAGE_CODE = string.Empty;
                }
            }
            else
            {
                inputRecord.MSG_MESSAGE = string.Empty;
                inputRecord.MSG_MESSAGE = "Department " + inputRecord.DEPT_NAME_IN.Split(new string[] { "  " }, StringSplitOptions.None).ElementAtOrDefault(0) + " not recognized. Skipping image processing.";
                inputRecord.MSG_CR = Constants.Y;
                inputRecord.RECORD_SKIP = true;
                Common.CALL_MSGDRVR(inputRecord, strReportPath);


            }

            // below set of statement's are for CREATE-ORDER-PART3
            inputRecord.SIGN_NUMBER = 0;
            ProcessDownback.ORDERS_CREATED++;
            if (!string.IsNullOrEmpty(inputRecord.O_DEPARTMENT_NO) && inputRecord.O_DEPARTMENT_NO.PadLeft(2, '0') == "03" && !string.IsNullOrEmpty(inputRecord.SL_SIGN_SIZE) && inputRecord.SL_SIGN_SIZE.PadLeft(4, '0') == "0002")
            {
                inputRecord.SL_SIGN_SIZE = inputRecord.HOLD_SL_SIGN_SIZE;
                inputRecord.SL_SIGN_HEADING = inputRecord.HOLD_SL_SIGN_HEADING;
                inputRecord.SL_PAPER_TYPE = inputRecord.HOLD_SL_PAPER_TYPE;
                inputRecord.SL_LAYOUT_NO = inputRecord.HOLD_SL_LAYOUT_NO;
            }

        }

        /// <summary>
        ///Reads Header and Signlayout file
        /// </summary>
        /// <param name="inputRecord"></param>
        public void ReadHeaderAndSignLayFile(AcmeRecord inputRecord)
        {


            if (string.IsNullOrEmpty(inputRecord.SL_SIGN_HEADING))
            {
                inputRecord.SL_SIGN_HEADING = string.Empty;
            }

            Heading heading = ProcessDownback.records.Headings.FirstOrDefault(h => h.HEADING_NO.PadLeft(4, '0') == inputRecord.SL_SIGN_HEADING.PadLeft(4, '0'));
            if (heading != null)
            {
                inputRecord.HEADING_SALE_FLAG = heading.HEADING_SALE_FLAG;
                inputRecord.HEADING_BOGO_FLAG = heading.HEADING_BOGO_FLAG;
                inputRecord.HEADING_PCNTOFF_FLAG = heading.HEADING_PCNTOFF_FLAG;
                inputRecord.HEADING_10FOR10_FLAG = heading.HEADING_10FOR10_FLAG;
                inputRecord.HEADING_GROCERY_FLAG = heading.HEADING_GROCERY_FLAG;
                inputRecord.HEADING_BAKERY_FLAG = heading.HEADING_BAKERY_FLAG;
                inputRecord.HEADING_FRESH_MEAT_FLAG = heading.HEADING_FRESH_MEAT_FLAG;
                inputRecord.HEADING_SRVC_DELI_FLAG = heading.HEADING_SRVC_DELI_FLAG;
                inputRecord.HEADING_PKGD_MEAT_FLAG = heading.HEADING_PKGD_MEAT_FLAG;
                inputRecord.HEADING_PRODUCE_FLAG = heading.HEADING_PRODUCE_FLAG;
                inputRecord.HEADING_FLORAL_FLAG = heading.HEADING_FLORAL_FLAG;
                inputRecord.HEADING_GM_FLAG = heading.HEADING_GM_FLAG;
                inputRecord.HEADING_BEER_WINE_FLAG = heading.HEADING_BEER_WINE_FLAG;
                inputRecord.HEADING_FROZEN_FLAG = heading.HEADING_FROZEN_FLAG;
                inputRecord.HEADING_SEAFOOD_FLAG = heading.HEADING_SEAFOOD_FLAG;
                inputRecord.HEADING_SMFL_FLAG = heading.HEADING_SMFL_FLAG;
                inputRecord.HEADING_EDLP_FLAG = heading.HEADING_EDLP_FLAG;
                inputRecord.HEADING_OTHER_FLAG = heading.HEADING_OTHER_FLAG;
                inputRecord.HEADING_DUPLEX_FLAG = heading.HEADING_DUPLEX_FLAG;
            }
            else
            {
                inputRecord.MSG_MESSAGE = string.Empty;
                inputRecord.MSG_MESSAGE = "Heading " + inputRecord.SL_SIGN_HEADING + " is not in the Heading table . . . skipping.";
                inputRecord.RECORD_SKIP = true;
            }

            SignLayout signLay = ProcessDownback.records.SignLayouts.FirstOrDefault(s => s.SL_SIGN_SIZE.PadLeft(4, '0') == inputRecord.SL_SIGN_SIZE.PadLeft(4, '0') && s.SL_SIGN_HEADING.PadLeft(4, '0') == inputRecord.SL_SIGN_HEADING.PadLeft(4, '0'));
            if (signLay != null)
            {
                inputRecord.SL_LAYOUT_NO = signLay.SL_LAYOUT_NO;
                inputRecord.SL_PAPER_TYPE = signLay.SL_PAPER_TYPE;

                inputRecord.HOLD_SL_SIGN_SIZE = inputRecord.SL_SIGN_SIZE;
                inputRecord.HOLD_SL_SIGN_HEADING = inputRecord.SL_SIGN_HEADING;
                inputRecord.HOLD_SL_PAPER_TYPE = inputRecord.SL_PAPER_TYPE;
                inputRecord.HOLD_SL_LAYOUT_NO = inputRecord.SL_LAYOUT_NO;
            }
            else
            {
                inputRecord.MSG_MESSAGE = string.Empty;
                inputRecord.MSG_MESSAGE = "No Layout-Stock record for Size = " + inputRecord.SL_SIGN_SIZE + ", Heading = " + inputRecord.SL_SIGN_HEADING + " - stopping.";
                inputRecord.RECORD_SKIP = true;
            }

        }

        #region UNITPRIC.CBL
        /// <summary>
        ///Process the work Price as per UNITPRIC.cbl
        /// </summary>
        /// <param name="inputRecord"></param>
        public void UnitPric_ComputeUnitPrice(AcmeRecord inputRecord)
        {

            inputRecord.WORK_UNITS_PART1 = "0";
            inputRecord.WORK_UNITS_PART2 = "0";
            inputRecord.PACK_QTY = "0";
            inputRecord.DECIMAL_POINT_COUNT = 0;
            inputRecord.ASTERISK_COUNT = 0;

            if ((!string.IsNullOrEmpty(inputRecord.PS_SIZE)) && inputRecord.PS_SIZE.ToUpper() == "HALF")
            {
                inputRecord.PS_SIZE = ".5";
            }
            else if (((!string.IsNullOrEmpty(inputRecord.PS_SIZE)) && inputRecord.PS_SIZE.ToUpper() == "PER") || ((!string.IsNullOrEmpty(inputRecord.PS_SIZE)) && inputRecord.PS_SIZE.ToUpper() == "EACH"))
            {
                inputRecord.PS_SIZE = "1.0";
            }


            inputRecord.ASTERISK_COUNT = inputRecord.PS_SIZE.Trim().Count(f => f == '*');

            if (inputRecord.ASTERISK_COUNT == 1)
            {
                inputRecord.TEMP_SIZE = string.Empty;
                inputRecord.PACK_QTY = inputRecord.PS_SIZE.EmptyNull().Split(new string[] { "*", "  " }, StringSplitOptions.None).ElementAtOrDefault(0);
                inputRecord.TEMP_SIZE = inputRecord.PS_SIZE.EmptyNull().Split(new string[] { "*", "  " }, StringSplitOptions.None).ElementAtOrDefault(1);
                inputRecord.PS_SIZE = inputRecord.TEMP_SIZE;
            }
            else
            {
                inputRecord.PACK_QTY = "1";
            }

            inputRecord.DECIMAL_POINT_COUNT = inputRecord.PS_SIZE.EmptyNull().Trim().Count(f => f == '.');

            if (inputRecord.DECIMAL_POINT_COUNT == 0)
            {
                inputRecord.WORK_UNITS_PART1 = inputRecord.PS_SIZE.EmptyNull().Split(' ').ElementAtOrDefault(0);
            }
            else
            {
                inputRecord.WORK_UNITS_PART2_COUNT = 0;
                inputRecord.WORK_UNITS_PART1 = inputRecord.PS_SIZE.EmptyNull().Split(new string[] { ".", " " }, StringSplitOptions.None).ElementAtOrDefault(0);
                inputRecord.WORK_UNITS_PART2 = inputRecord.PS_SIZE.EmptyNull().Split(new string[] { ".", " " }, StringSplitOptions.None).ElementAtOrDefault(1);

                inputRecord.WORK_UNITS_PART2_COUNT = inputRecord.WORK_UNITS_PART2.Length;

                if (inputRecord.WORK_UNITS_PART2_COUNT == 1)
                {
                    inputRecord.WORK_UNITS_PART2 = inputRecord.WORK_UNITS_PART2 + "0";
                }
            }


            //inputRecord.WORK_UNITS = inputRecord.WORK_UNITS_PART1.PadLeft(5, '0') + inputRecord.WORK_UNITS_PART2.PadLeft(2, '0');
            inputRecord.WORK_UNITS = inputRecord.WORK_UNITS_PART1 + "." + inputRecord.WORK_UNITS_PART2;

            inputRecord.WORK_UNITS_COMPUTATIONAL = inputRecord.WORK_UNITS;

            inputRecord.WORK_UNITS_COMPUTATIONAL = (inputRecord.WORK_UNITS_COMPUTATIONAL.ConvertStringToDouble() * inputRecord.PACK_QTY.ConvertStringToDouble()).ToString();
            inputRecord.SPECIAL_PRICE_COMPUTATIONAL = inputRecord.OS_SPECIAL_PRICE;

            if (inputRecord.OS_SPECIAL_FOR_NUMERIC > 0)
            {
                inputRecord.SPECIAL_PRICE_COMPUTATIONAL = (inputRecord.SPECIAL_PRICE_COMPUTATIONAL.ConvertStringToDouble() / inputRecord.OS_SPECIAL_FOR_NUMERIC).ToString();
            }
            else
            {
                inputRecord.SPECIAL_PRICE_COMPUTATIONAL = "0.0";
            }

            if (inputRecord.WORK_UNITS_COMPUTATIONAL.ConvertStringToDouble() > 0)
            {
                inputRecord.UNIT_PRICE_COMPUTATIONAL = (inputRecord.SPECIAL_PRICE_COMPUTATIONAL.ConvertStringToDouble() / inputRecord.WORK_UNITS_COMPUTATIONAL.ConvertStringToDouble()).ToString();
            }
            else
            {
                inputRecord.UNIT_PRICE_COMPUTATIONAL = "0.0";
            }

            #region COMPUTE-UNIT-PRICE-PART2

            do
            {
                inputRecord.RE_SEARCH_FLAG = Constants.N;

                if (string.IsNullOrEmpty(inputRecord.PS_TYPE_CHAR2))
                {
                    inputRecord.PS_TYPE_CHAR2 = string.Empty;

                }

                if (string.IsNullOrEmpty(inputRecord.PS_UNITS))
                {
                    inputRecord.PS_UNITS = string.Empty;
                }

                if (string.IsNullOrEmpty(inputRecord.PS_UNITS2))
                {
                    inputRecord.PS_UNITS2 = string.Empty;
                }


                inputRecord.PS_KEY = inputRecord.PS_TYPE_CHAR2 + inputRecord.PS_UNITS + inputRecord.PS_UNITS2;

                UnitPriceEntry upe = ProcessDownback.records.UnitPriceEntrys.FirstOrDefault(u => u.UPE_KEY == inputRecord.PS_KEY);
                if (upe != null)
                {
                    inputRecord.UPE_UNIT = upe.UPE_UNIT;
                    inputRecord.UPE_NUMERATOR = upe.UPE_NUMERATOR;
                    inputRecord.UPE_DENOMINATOR = upe.UPE_DENOMINATOR;
                    AdjustUnitPrice(inputRecord);   // Function Call
                }
                else
                {
                    RejectUnitPriceUnits(inputRecord);

                }
            } while (inputRecord.RE_SEARCH_FLAG == Constants.Y);

            #endregion
            inputRecord.WK_UNIT_PRICE = string.Empty;


            if (inputRecord.UNIT_PRICE_COMPUTATIONAL.ConvertStringToDouble() > .99)
            {

                inputRecord.UNIT_PRICE_COMPUTATIONAL = (inputRecord.UNIT_PRICE_COMPUTATIONAL.ConvertStringToDouble() + .0099).ToString();
                inputRecord.UNIT_PRICE_DISPLAY_DOLLARS = (String.Format("{0:0.00}", Math.Truncate(Helper.ConvertStringToDouble(inputRecord.UNIT_PRICE_COMPUTATIONAL) * 100) / 100)).ToString();
                //inputRecord.UNIT_PRICE_DISPLAY_DOLLARS = string.Format("{0:0.00}", Helper.ConvertStringToDouble(inputRecord.UNIT_PRICE_COMPUTATIONAL));
                inputRecord.WK_UNIT_PRICE = "$" + inputRecord.UNIT_PRICE_DISPLAY_DOLLARS + "/" + inputRecord.DISPLAY_UNITS;
            }
            else
            {
                inputRecord.UNIT_PRICE_COMPUTATIONAL = (inputRecord.UNIT_PRICE_COMPUTATIONAL.ConvertStringToDouble() * 100).ToString();
                inputRecord.UNIT_PRICE_COMPUTATIONAL = (inputRecord.UNIT_PRICE_COMPUTATIONAL.ConvertStringToDouble() + .099).ToString();
                inputRecord.UNIT_PRICE_DISPLAY_CENTS = (String.Format("{0:0.0}", Math.Truncate(Helper.ConvertStringToDouble(inputRecord.UNIT_PRICE_COMPUTATIONAL) * 10) / 10)).ToString();
                //inputRecord.UNIT_PRICE_DISPLAY_CENTS = string.Format("{0:0.0}", Helper.ConvertStringToDouble(inputRecord.UNIT_PRICE_COMPUTATIONAL));
                inputRecord.WK_UNIT_PRICE = inputRecord.UNIT_PRICE_DISPLAY_CENTS + "¢/" + inputRecord.DISPLAY_UNITS;
            }
            inputRecord.OS_UNIT_PRICE = inputRecord.WK_UNIT_PRICE.EmptyNull().TrimStart(' ');

        }


        /// <summary>
        ///Initializes data for Unit Price Processing
        /// </summary>
        /// <param name="inputRecord"></param>
        public void UnitPric_Main(AcmeRecord inputRecord)
        {

            inputRecord.PACKAGE_SIZE_DATA = string.Empty;
            inputRecord.PACKAGE_SIZE_DATA = inputRecord.OS_UNIT_PRICE.EmptyNull().Split(new string[] { "   ", "  ", " " }, StringSplitOptions.None).ElementAtOrDefault(0);
            inputRecord.PS_SIZE = inputRecord.OS_UNIT_PRICE.EmptyNull().Split(new string[] { "   ", "  ", " " }, StringSplitOptions.None).ElementAtOrDefault(1);
            inputRecord.PS_UNITS = inputRecord.OS_UNIT_PRICE.EmptyNull().Split(new string[] { "   ", "  ", " " }, StringSplitOptions.None).ElementAtOrDefault(2);
            inputRecord.PS_UNITS2 = inputRecord.OS_UNIT_PRICE.EmptyNull().Split(new string[] { "   ", "  ", " " }, StringSplitOptions.None).ElementAtOrDefault(3);

            inputRecord.PS_TYPE_CHAR1 = inputRecord.PACKAGE_SIZE_DATA.SafeSubstring(0, 1);
            inputRecord.PS_TYPE_CHAR2 = inputRecord.PACKAGE_SIZE_DATA.SafeSubstring(1, 1);

            if (inputRecord.OS_UNIT_PRICE.Split(new string[] { "   ", "  ", " " }, StringSplitOptions.None).Length == 1)
            {
                inputRecord.PS_SIZE = inputRecord.PACKAGE_SIZE_DATA.SafeSubstring(22, 10);
                inputRecord.PS_UNITS = inputRecord.PACKAGE_SIZE_DATA.SafeSubstring(2, 10);
                inputRecord.PS_UNITS2 = inputRecord.PACKAGE_SIZE_DATA.SafeSubstring(12, 10);

            }

            if (!string.IsNullOrEmpty(inputRecord.PS_UNITS2))
            {
                inputRecord.WORK_STRING = string.Empty;
                inputRecord.WORK_STRING = inputRecord.PS_UNITS.EmptyNull().Split(' ').ElementAtOrDefault(0) + " " + inputRecord.PS_UNITS2.Split(' ').ElementAtOrDefault(0);
                inputRecord.PS_UNITS = inputRecord.WORK_STRING;
                inputRecord.PS_UNITS2 = string.Empty;
            }

            inputRecord.COMPUTE_UNIT_PRICE_FLAG = Constants.N;
            inputRecord.S_I_INIT_FIELD = 0;

            if (inputRecord.PS_TYPE_CHAR1 == ".")
            {
                inputRecord.PS_TYPE_CHAR2 = inputRecord.PS_TYPE_CHAR2.ToUpper();

                if (Constants.ChkList_PS_TYPE_CHAR2.Contains(inputRecord.PS_TYPE_CHAR2))
                {
                    UnitPric_ComputeUnitPrice(inputRecord);
                    inputRecord.S_I_REDISPLAY = "R";
                    inputRecord.S_I_DISP_FLD = 25;
                }
                else
                {
                    inputRecord.S_I_INIT_FIELD = inputRecord.S_I_INIT_FIELD + 1;
                }
            }
            else
            {
                inputRecord.S_I_INIT_FIELD = inputRecord.S_I_INIT_FIELD + 1;
            }


        }

        /// <summary>
        ///Left Align Data
        /// </summary>
        /// <param name="inputRecord"></param>
        public void SlideUpLeft(AcmeRecord inputRecord)
        {

            inputRecord.WORK_STRING = inputRecord.WORK_STRING.EmptyNull().TrimStart(' ');
            inputRecord.WK_UNIT_PRICE = inputRecord.WORK_STRING;

        }

        /// <summary>
        ///Adjust's Unit Price
        /// </summary>
        /// <param name="inputRecord"></param>
        public void AdjustUnitPrice(AcmeRecord inputRecord)
        {

            inputRecord.DISPLAY_UNITS = inputRecord.UPE_UNIT;

            if (inputRecord.UPE_DENOMINATOR.ConvertStringToDouble() > 0)
            {
                inputRecord.UNIT_PRICE_COMPUTATIONAL = ((inputRecord.UNIT_PRICE_COMPUTATIONAL.ConvertStringToDouble() * inputRecord.UPE_NUMERATOR.ConvertStringToDouble()) / inputRecord.UPE_DENOMINATOR.ConvertStringToDouble()).ToString();
            }
            else
            {
                inputRecord.UNIT_PRICE_COMPUTATIONAL = "0.0";
            }

        }


        /// <summary>
        ///Rejects's Unit Price units
        /// </summary>
        /// <param name="inputRecord"></param>
        public void RejectUnitPriceUnits(AcmeRecord inputRecord)
        {

            switch (inputRecord.PS_TYPE_CHAR2)
            {
                case "D":
                    inputRecord.PS_TYPE_CHAR2 = "L";
                    inputRecord.RE_SEARCH_FLAG = Constants.Y;
                    break;

                case "L":
                case "P":
                    inputRecord.PS_TYPE_CHAR2 = "C";
                    inputRecord.RE_SEARCH_FLAG = Constants.Y;
                    break;

                case "C":
                    inputRecord.PS_TYPE_CHAR2 = "F";
                    inputRecord.RE_SEARCH_FLAG = Constants.Y;
                    break;

                case "F":
                    inputRecord.PS_TYPE_CHAR2 = "E";
                    inputRecord.RE_SEARCH_FLAG = Constants.Y;
                    break;

                case "E":
                    inputRecord.PS_TYPE_CHAR2 = "A";
                    inputRecord.RE_SEARCH_FLAG = Constants.Y;
                    break;

                default:
                    inputRecord.RE_SEARCH_FLAG = Constants.N;
                    inputRecord.MSG_MESSAGE = string.Empty;
                    inputRecord.MSG_MESSAGE = "Unknown unit of measure in Package-Size: " + inputRecord.PS_KEY.EmptyNull().Split(new string[] { "  " }, StringSplitOptions.None).ElementAtOrDefault(0) + " - check Unit Price!";
                    inputRecord.MSG_CR = Constants.Y;
                    Common.CALL_MSGDRVR(inputRecord, strReportPath);
                    break;
            }

        }


        #endregion UNITPRIC.CBL

        /// <summary>
        /// Custom Function: Get VestcomTag Type function for setting Vestcom Tag based on Substitute Stock
        /// </summary>
        /// <param name="lbl"></param>
        public static void GetVestcomTagType(AcmeRecord lbl)
        {

            if (!string.IsNullOrEmpty(lbl.ARTWORK))
            {
                lbl.D_BACKGROUND_IMAGE = lbl.ARTWORK;
            }

            SubstituteStock record;
            record = ProcessDownback.records.SubstituteStock.FirstOrDefault(s => s.MAIN_STOCK == lbl.SL_PAPER_TYPE);

            if (record != null)
            {
                string fileExtn = record.BG_IMG.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase) ? string.Empty : Constants.IMAGEFILEEXTENSION;
                //Set Default image if TYPESET_BACKGROUND_NAME is empty.
                if (string.IsNullOrEmpty(lbl.D_BACKGROUND_IMAGE) && !string.IsNullOrEmpty(record.BG_IMG))
                    lbl.D_BACKGROUND_IMAGE = Path.Combine(Constants.ImageFolderpath, (record.BG_IMG + fileExtn));
                if (!string.IsNullOrEmpty(record.SUBSTITUTE_STOCK.EmptyNull().Trim()))
                    lbl.D_Vestcom_Tag_Type = record.SUBSTITUTE_STOCK.EmptyNull().Trim();
                else
                    lbl.D_Vestcom_Tag_Type = lbl.SL_PAPER_TYPE;

                if (record.SIMPDUP == "S")
                {
                    lbl.D_PAPER_SIMP_DUP = "1";
                }
                if (record.SIMPDUP == "D")
                {
                    lbl.D_PAPER_SIMP_DUP = "2";
                }
            }
            if (!string.IsNullOrEmpty(lbl.OS_COMMENT3))
            {
                lbl.D_BACKGROUND_IMAGE = Path.Combine(Constants.ImageFolderpath,
                    (lbl.OS_COMMENT3 + (lbl.OS_COMMENT3.EndsWith(Constants.IMAGEFILEEXTENSION, StringComparison.InvariantCultureIgnoreCase) ? string.Empty :
                    lbl.OS_COMMENT3.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase) ? string.Empty : Constants.IMAGEFILEEXTENSION)));
            }

            if (!string.IsNullOrEmpty(lbl.D_BACKGROUND_IMAGE) && lbl.D_BACKGROUND_IMAGE.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase))
            {
                //lbl.D_BACKGROUND_IMAGE = lbl.D_BACKGROUND_IMAGE.Replace(".pdf", Constants.IMAGEFILEEXTENSION);
                lbl.D_BACKGROUND_IMAGE = lbl.D_BACKGROUND_IMAGE;
            }

            if (!string.IsNullOrEmpty(lbl.D_BACKGROUND_IMAGE) && lbl.D_BACKGROUND_IMAGE.Length > Constants.MAX_PATH)
            {
                lbl.MSG_MESSAGE = string.Empty;
                lbl.MSG_MESSAGE = "Total Charachers in File Path is longer than " + Constants.MAX_PATH + " characters ...." + lbl.D_BACKGROUND_IMAGE;
                lbl.RECORD_SKIP = true;
            }
            else if (!File.Exists(lbl.D_BACKGROUND_IMAGE))
            {
                lbl.MSG_MESSAGE = lbl.MSG_MESSAGE + "ART IS MISSING!!!FOR UPC: " + lbl.UPC_CODE + " AND IMAGE NAME: " + lbl.D_BACKGROUND_IMAGE + " PLEASE CREATE AND ADD TO IMAGE LIBRARY ASAP.";
                lbl.RECORD_SKIP = true;
            }
        }
    }
}
