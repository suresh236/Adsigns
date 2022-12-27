using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Data;
using Vestcom.Core.Utilities.ExcelReader;
using System.Reflection;
using Vestcom.PaperSign.ACME.Holiday.Records;
using Vestcom.PaperSign.ACME.Holiday.Entities;
using Vestcom.Core.Database.Entities;

namespace Vestcom.PaperSign.ACME.Holiday.Rules
{
    public class BusinessLogic: IBusinessLogic
    {
        List<AcmeHolidayRecord> acmeHolidayRecordList = null;
        private DataTable dtDownBackColumnList;
        string strFilePath = string.Empty;
        string strReportPath = string.Empty;

        /// <summary>
        /// This is the starting point for the Business Rules.
        /// </summary>
        /// <param name="lbl"></param>
        public void ApplyRules(string filePath, string DownLoadFilePath, string reportPath)
        {
            if (acmeHolidayRecordList == null)
            {
                ImportAcmeHolidayExcelData(filePath);
                dtDownBackColumnList = new DataTable("DownBackFileOutputHeader");
                if (string.IsNullOrEmpty(strFilePath))
                {
                    strFilePath = DownLoadFilePath;
                }
                if (string.IsNullOrEmpty(strReportPath))
                {
                    strReportPath = reportPath;
                }

                CreateDownBackFileHeader();
            }
            List<AcmeHolidayRecord> holidayRecords = GetHolidayRecords();
            foreach (var lbl in holidayRecords)
            {
                if (!string.IsNullOrEmpty(lbl.Store_Hours_Open) && !string.IsNullOrEmpty(lbl.Store_Hours_Close))
                {
                    CreateOrdersAndSigns(lbl);
                }
            }
        }

        /// <summary>
        /// Import data from Excel
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        private void ImportAcmeHolidayExcelData(string filePath)
        {
            DataSet dataset = ReadExcelFile.Read(filePath, false);
            SaveDataIntoList(dataset.Tables[0]);
        }

        /// <summary>
        /// Save data into List from Dataset
        /// </summary>
        /// <param name="dtData"></param>
        private void SaveDataIntoList(DataTable dtData)
        {
            acmeHolidayRecordList = new List<AcmeHolidayRecord>();
            #region Create DataRow
            DataTable dtRawData = new DataTable();
            dtRawData.Columns.Add("OrderNumber");
            dtRawData.Columns.Add("Store_Num");
            dtRawData.Columns.Add("Description");
            dtRawData.Columns.Add("District");
            dtRawData.Columns.Add("Store_Phone_Number");
            dtRawData.Columns.Add("Pharmacy_Phone");
            dtRawData.Columns.Add("Holiday");
            dtRawData.Columns.Add("Begin_Date");
            dtRawData.Columns.Add("End_Date");
            dtRawData.Columns.Add("Store_Open");
            dtRawData.Columns.Add("Store_Close");
            dtRawData.Columns.Add("RX_Open");
            dtRawData.Columns.Add("RX_Close");
            dtRawData.Columns.Add("RegularStore_Hours_Mon_Fri_Open");
            dtRawData.Columns.Add("RegularStore_Hours_Mon_Fri_Close");
            dtRawData.Columns.Add("RegularStore_Hours_Sat_Open");
            dtRawData.Columns.Add("RegularStore_Hours_Sat_Close");
            dtRawData.Columns.Add("RegularStore_Hours_Sun_Open");
            dtRawData.Columns.Add("RegularStore_Hours_Sun_Close");
            dtRawData.Columns.Add("RegularPharmacy_Hours_Mon_Fri_Open");
            dtRawData.Columns.Add("RegularPharmacy_Hours_Mon_Fri_Close");
            dtRawData.Columns.Add("RegularPharmacy_Hours_Sat_Open");
            dtRawData.Columns.Add("RegularPharmacy_Hours_Sat_Close");
            dtRawData.Columns.Add("RegularPharmacy_Hours_Sun_Open");
            dtRawData.Columns.Add("RegularPharmacy_Hours_Sun_Close");
            dtRawData.Columns.Add("Division");

            #endregion
            //variable for Column header position in file
            int storenumloc = 999;
            int descriptionloc = 999;
            int districtloc = 999;
            int storephonenumberloc = 999;
            int pharmacyphone = 999;
            int regularstorehours = 999;
            int regularpharmacyhours = 999;
            int storetime = 999;
            int RXtime = 999;
            int begindate = 999;
            int enddate = 999;
            int division = 999;

            int holidayrow = 0;

            foreach (DataRow row in dtData.Rows)
            {
                int drItemArrayCount = row.ItemArray.Count();

                //To get the excel file first cols header
                if (storenumloc == 999)
                {
                    holidayrow = holidayrow + 1;
                    for (int i = 0; i < drItemArrayCount; i++)
                    {
                        if (Convert.ToString(row[i]).EmptyNull().Trim().ToUpper() == "STORE NUM")
                        {
                            storenumloc = i;
                        }
                        else if (Convert.ToString(row[i]).EmptyNull().Trim().ToUpper() == "DESCRIPTION")
                        {
                            descriptionloc = i;
                        }
                        else if (Convert.ToString(row[i]).EmptyNull().Trim().ToUpper() == "DISTRICT")
                        {
                            districtloc = i;
                        }
                        else if (Convert.ToString(row[i]).EmptyNull().Trim().ToUpper() == "STORE PHONE NUMBER")
                        {
                            storephonenumberloc = i;
                        }
                        else if (Convert.ToString(row[i]).EmptyNull().Trim().ToUpper() == "PHARMACY PHONE")
                        {
                            pharmacyphone = i;
                        }
                        //Regular Store Hours
                        else if (Convert.ToString(row[i]).EmptyNull().Trim().ToUpper() == "REGULAR STORE HOURS")
                        {
                            regularstorehours = i;
                        }
                        else if (Convert.ToString(row[i]).EmptyNull().Trim().ToUpper() == "REGULAR PHARMACY HOURS")
                        {
                            regularpharmacyhours = i;
                        }
                        else if (Convert.ToString(row[i]).EmptyNull().Trim().ToUpper() == "DIVISION")
                        {
                            division = i;
                        }
                    }
                    continue;
                }
                else if (storetime == 999)
                {
                    for (int i = 0; i < drItemArrayCount; i++)
                    {
                        if (Convert.ToString(row[i]).EmptyNull().Trim().ToUpper() == "STORE")
                        {
                            storetime = i;
                        }
                        if (Convert.ToString(row[i]).EmptyNull().Trim().ToUpper() == "RX")
                        {
                            RXtime = i;
                        }
                    }
                    continue;
                }
                else if (begindate == 999)
                {
                    for (int i = 0; i < drItemArrayCount; i++)
                    {
                        if (Convert.ToString(row[i]).EmptyNull().Trim().ToUpper() == "BEGIN DATE")
                        {
                            begindate = i;
                        }
                        if (Convert.ToString(row[i]).EmptyNull().Trim().ToUpper() == "END DATE")
                        {
                            enddate = i;
                        }
                    }
                    continue;
                }

                int iCounter = 0;

                DataRow drow = dtRawData.NewRow();
                AcmeHolidayRecord acmeHolidayRecord = new AcmeHolidayRecord();
                if (iCounter < drItemArrayCount)
                {
                    //drow["OrderNumber"] = strOrderNumber;
                    //acmeHolidayRecord.OrderNumber = drow["OrderNumber"].ToString();
                }
                else
                {
                    continue;
                }
                if (string.IsNullOrEmpty(Convert.ToString(row[iCounter])))
                {
                    continue;
                }

                if (storenumloc != 999)
                {
                    drow["Store_Num"] = Convert.ToString(row[storenumloc]);
                    acmeHolidayRecord.Store_Num = drow["Store_Num"].ToString();
                }
                if (descriptionloc != 999)
                {
                    drow["Description"] = Convert.ToString(row[descriptionloc]);
                    acmeHolidayRecord.Description = drow["Description"].ToString();
                }
                if (districtloc != 999)
                {
                    drow["District"] = Convert.ToString(row[districtloc]);
                    acmeHolidayRecord.District = drow["District"].ToString();
                }
                if (storephonenumberloc != 999)
                {
                    drow["Store_Phone_Number"] = Convert.ToString(row[storephonenumberloc]);
                    acmeHolidayRecord.Store_Phone_Number = drow["Store_Phone_Number"].ToString();
                }
                if (pharmacyphone != 999)
                {
                    drow["Pharmacy_Phone"] = Convert.ToString(row[pharmacyphone]);
                    acmeHolidayRecord.Pharmacy_Phone = drow["Pharmacy_Phone"].ToString();
                }
                if (begindate != 999)
                {
                    drow["Holiday"] = Convert.ToString(dtData.Rows[holidayrow - 1][begindate]);
                    acmeHolidayRecord.Holiday = drow["Holiday"].ToString();
                    drow["Begin_Date"] = Convert.ToString(row[begindate]);
                    acmeHolidayRecord.Begin_Date = drow["Begin_Date"].ToString();
                }
                if (enddate != 999)
                {
                    drow["End_Date"] = Convert.ToString(row[enddate]);
                    acmeHolidayRecord.End_Date = drow["End_Date"].ToString();
                }
                if (storetime != 999)
                {
                    drow["Store_Open"] = Convert.ToString(row[storetime]);
                    acmeHolidayRecord.Store_Open = drow["Store_Open"].ToString();
                    drow["Store_Close"] = Convert.ToString(row[storetime + 1]);
                    acmeHolidayRecord.Store_Close = drow["Store_Close"].ToString();
                }

                if (RXtime != 999)
                {
                    drow["RX_Open"] = Convert.ToString(row[RXtime]);
                    acmeHolidayRecord.RX_Open = drow["RX_Open"].ToString();
                    drow["RX_Close"] = Convert.ToString(row[RXtime + 1]);
                    acmeHolidayRecord.RX_Close = drow["RX_Close"].ToString();
                }

                if (regularstorehours != 999)
                {
                    drow["RegularStore_Hours_Mon_Fri_Open"] = Convert.ToString(row[regularstorehours]);
                    drow["RegularStore_Hours_Mon_Fri_Close"] = Convert.ToString(row[regularstorehours + 1]);
                    drow["RegularStore_Hours_Sat_Open"] = Convert.ToString(row[regularstorehours + 2]);
                    drow["RegularStore_Hours_Sat_Close"] = Convert.ToString(row[regularstorehours + 3]);
                    drow["RegularStore_Hours_Sun_Open"] = Convert.ToString(row[regularstorehours + 4]);
                    drow["RegularStore_Hours_Sun_Close"] = Convert.ToString(row[regularstorehours + 5]);

                    acmeHolidayRecord.RegularStore_Hours_Mon_Fri_Open = drow["RegularStore_Hours_Mon_Fri_Open"].ToString();
                    acmeHolidayRecord.RegularStore_Hours_Mon_Fri_Close = drow["RegularStore_Hours_Mon_Fri_Close"].ToString();
                    acmeHolidayRecord.RegularStore_Hours_Sat_Open = drow["RegularStore_Hours_Sat_Open"].ToString();
                    acmeHolidayRecord.RegularStore_Hours_Sat_Close = drow["RegularStore_Hours_Sat_Close"].ToString();
                    acmeHolidayRecord.RegularStore_Hours_Sun_Open = drow["RegularStore_Hours_Sun_Open"].ToString();
                    acmeHolidayRecord.RegularStore_Hours_Sun_Close = drow["RegularStore_Hours_Sun_Close"].ToString();
                }


                if (regularpharmacyhours != 999)
                {
                    drow["RegularPharmacy_Hours_Mon_Fri_Open"] = Convert.ToString(row[regularpharmacyhours]);
                    drow["RegularPharmacy_Hours_Mon_Fri_Close"] = Convert.ToString(row[regularpharmacyhours + 1]);
                    drow["RegularPharmacy_Hours_Sat_Open"] = Convert.ToString(row[regularpharmacyhours + 2]);
                    drow["RegularPharmacy_Hours_Sat_Close"] = Convert.ToString(row[regularpharmacyhours + 3]);
                    drow["RegularPharmacy_Hours_Sun_Open"] = Convert.ToString(row[regularpharmacyhours + 4]);
                    drow["RegularPharmacy_Hours_Sun_Close"] = Convert.ToString(row[regularpharmacyhours + 5]);

                    acmeHolidayRecord.RegularPharmacy_Hours_Mon_Fri_Open = drow["RegularStore_Hours_Mon_Fri_Open"].ToString();
                    acmeHolidayRecord.RegularPharmacy_Hours_Mon_Fri_Close = drow["RegularStore_Hours_Mon_Fri_Close"].ToString();
                    acmeHolidayRecord.RegularPharmacy_Hours_Sat_Open = drow["RegularStore_Hours_Sat_Open"].ToString();
                    acmeHolidayRecord.RegularPharmacy_Hours_Sat_Close = drow["RegularStore_Hours_Sat_Close"].ToString();
                    acmeHolidayRecord.RegularPharmacy_Hours_Sun_Open = drow["RegularStore_Hours_Sun_Open"].ToString();
                    acmeHolidayRecord.RegularPharmacy_Hours_Sun_Close = drow["RegularStore_Hours_Sun_Close"].ToString();
                }
                if (division != 999)
                {
                    drow["Division"] = Convert.ToString(row[division]);
                    acmeHolidayRecord.Division = drow["Division"].ToString();
                }
                //dtRawData.Rows.Add(drow);
                acmeHolidayRecordList.Add(acmeHolidayRecord);
            }

        }

        /// <summary>
        ///Process input record for Orders and Signs
        /// </summary>
        /// <param name="inputRecord"></param>
        public void CreateOrdersAndSigns(AcmeHolidayRecord lbl)
        {
            ReadHeaderAndSignLayFile(lbl);

            lbl.O_LAYOUT_NO = lbl.SL_LAYOUT_NO;
            lbl.O_PAPER_TYPE = lbl.SL_PAPER_TYPE;
            lbl.O_SIGN_SIZE = lbl.SL_SIGN_SIZE;
            lbl.O_SIGN_HEAD = lbl.SL_SIGN_HEADING;

            lbl.OS_KEY = lbl.DATA_NUM_IN.ToString();
            lbl.OS_STORE_NUMBER = lbl.OS_STORE_NUMBER.PadLeft(5, '0');
            lbl.D_Begin_Date = string.Format("{0:MM/dd/yyyy}", Helper.ConvertStringToDateTime(lbl.D_Begin_Date));
            lbl.D_End_Date = string.Format("{0:MM/dd/yyyy}", Helper.ConvertStringToDateTime(lbl.D_End_Date));

            if (!string.IsNullOrEmpty(lbl.Store_Hours_Open))
            {
                if (new string[] { "24hrs", "24 hrs" }.Contains(lbl.Store_Hours_Open))
                {
                    lbl.D_Store_Hours = "Open 24 Hours";
                }
                else
                {
                    lbl.D_Store_Hours = GetTime(lbl.Store_Hours_Open) + " to " + GetTime(lbl.Store_Hours_Close);
                }
            }

            if (lbl.RX_Hours_Open.EmptyNull().ToUpper() == "CLOSED")
            {
                lbl.D_RX_Hours = "CLOSED";
            }
            else if (!string.IsNullOrEmpty(lbl.RX_Hours_Open))
            {
                lbl.D_RX_Hours = GetTime(lbl.RX_Hours_Open) + " - " + GetTime(lbl.RX_Hours_Close);
            }

            if (!string.IsNullOrEmpty(lbl.RX_Hours_Open))
            {
                lbl.D_RX_Image_Call = Path.Combine(Constants.ImageFolderpath, ("pharmacy" + Constants.ImageFileExtension));
            }

            GetVestcomTagType(lbl);
            if (!lbl.RECORD_SKIP)
            {
                WriteRecordtoDownBackFile(lbl);
            }
            else
            {
                GenerateReportFile(lbl, strReportPath);
            }
        }

        /// <summary>
        /// Reading SignHeader and SignLay Fillers
        /// </summary>
        /// <param name="lbl"></param>
        public void ReadHeaderAndSignLayFile(AcmeHolidayRecord lbl)
        {
            lbl.SL_SIGN_SIZE = Constants.holidaySignCode;
            var sh = AcmeHoliday.SignHeadingFillers.FirstOrDefault(s => s.SignHeaderDesc == lbl.D_Holiday);

            if (sh != null)
            {
                lbl.SL_SIGN_HEADING = sh.HEADING_NO;
            }
            if (string.IsNullOrEmpty(lbl.SL_SIGN_HEADING))
            {
                lbl.SL_SIGN_HEADING = string.Empty;
            }

            SignLayout signLay = AcmeHoliday.SignLayFillers.FirstOrDefault(s => s.SL_SIGN_SIZE.PadLeft(4, '0') == lbl.SL_SIGN_SIZE.PadLeft(4, '0') && s.SL_SIGN_HEADING.PadLeft(4, '0') == lbl.SL_SIGN_HEADING.PadLeft(4, '0'));
            if (signLay != null)
            {
                lbl.SL_LAYOUT_NO = signLay.SL_LAYOUT_NO;
                lbl.SL_PAPER_TYPE = signLay.SL_PAPER_TYPE;
            }
            else
            {
                lbl.MSG_MESSAGE = string.Empty;
                lbl.MSG_MESSAGE = "No Layout-Stock record for Size = " + lbl.SL_SIGN_SIZE + ", Heading = " + lbl.SL_SIGN_HEADING + " - stopping.";
                lbl.RECORD_SKIP = true;
            }

        }

        /// <summary>
        /// Chnages the time in the required format
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private string GetTime(string time)
        {
            time = time.Replace(":", ".");

            if(!string.IsNullOrEmpty(time))
            {
                if (time == "2400")
                {
                    return "Midnight";
                }
                else
                {
                    int svalue = Helper.ConvertStringToInteger(time);
                    string slimit = string.Empty;
                    int loopCount = 0;

                    loopCount = time.Contains("000") ? (time.Length - 2) : (time.Length - 1);
                    for (int i = 0; i < loopCount; i++)
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
            }
            
            return time;
        }

        /// <summary>
        /// This function sets the Vestcom Tag based on Substitute Stock
        /// </summary>
        /// <param name="lbl"></param>
        public void GetVestcomTagType(AcmeHolidayRecord lbl)
        {
            SubstituteStock record;
            record = AcmeHoliday.SubstituteStockFillers.FirstOrDefault(s => s.MAIN_STOCK == lbl.SL_PAPER_TYPE);

            if (record != null)
            {
                //Set Default image if TYPESET_BACKGROUND_NAME is empty.
                if (string.IsNullOrEmpty(lbl.D_BACKGROUND_IMAGE) && !string.IsNullOrEmpty(record.BG_IMG))
                {
                    lbl.D_BACKGROUND_IMAGE = Path.Combine(Constants.ImageFolderpath, (record.BG_IMG + Constants.ImageFileExtension));
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
            if (lbl.D_BACKGROUND_IMAGE.Length > Constants.Max_Path)
            {
                lbl.MSG_MESSAGE = string.Empty;
                lbl.MSG_MESSAGE = "Total Charachers in File Path is longer than " + Constants.Max_Path + " characters ...." + lbl.D_BACKGROUND_IMAGE;
                lbl.RECORD_SKIP = true;
            }
            else if (!File.Exists(lbl.D_BACKGROUND_IMAGE))
            {
                lbl.MSG_MESSAGE = string.Empty;
                lbl.MSG_MESSAGE = lbl.MSG_MESSAGE + "ART IS MISSING!!! IMAGE NAME: " + lbl.D_BACKGROUND_IMAGE + " PLEASE CREATE AND ADD TO IMAGE LIBRARY ASAP.";
                lbl.RECORD_SKIP = true;
            }

        }

        /// <summary>
        /// Creating the DownBack File Header
        /// </summary>
        private void CreateDownBackFileHeader()
        {
            IEnumerable<PropertyInfo> Props = typeof(AcmeHolidayRecord).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(m => m.GetCustomAttributes((false)).Any(a => a.GetType() == typeof(HeaderAttribute)));
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
        /// Filling the Holiday Records in the List
        /// </summary>
        /// <returns></returns>
        private List<AcmeHolidayRecord> GetHolidayRecords()
        {
            return acmeHolidayRecordList.Select(x => new AcmeHolidayRecord()
            {
                SignDataId = x.SignDataId,
                DATA_NUM_IN = x.DATA_NUM_IN,
                OS_ORDER_NUMBER = x.OrderNumber,
                OS_QUANTITY = "2",
                SIGNHEAD_IN = x.SIGNHEAD_IN,
                SIGNSIZE_IN = x.SIGNSIZE_IN,
                D_Holiday = x.Holiday,
                Store_Hours_Open = x.Store_Open,
                Store_Hours_Close = x.Store_Close,
                RX_Hours_Open = x.RX_Open,
                RX_Hours_Close = x.RX_Close,
                D_Begin_Date = x.Begin_Date,
                D_End_Date = x.End_Date,
                OS_STORE_NUMBER = x.Store_Num,
                ARTWORK = x.ARTWORK
            }).ToList();

        }

        /// <summary>
        /// Writes the record in the Downback File
        /// </summary>
        /// <param name="lbl"></param>
        private void WriteRecordtoDownBackFile(AcmeHolidayRecord lbl)
        {

            if (File.Exists(strFilePath))
            {
                StringBuilder content = new StringBuilder();
                content.Append(Environment.NewLine);
                for (int col = 0; col < dtDownBackColumnList.Columns.Count; col++)
                {
                    string d = Convert.ToString(lbl[dtDownBackColumnList.Columns[col].ColumnName.Replace('-', '_')]);

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
        ///Write data into Report File
        /// </summary>
        public static void GenerateReportFile(AcmeHolidayRecord skiprecords, string reportFilePath)
        {
            StringBuilder content = new StringBuilder();
            content.Append(Environment.NewLine);
            content.Append(skiprecords.MSG_MESSAGE + Environment.NewLine);
            File.AppendAllText(reportFilePath + "\\Reports.txt", content.ToString());
        }
    }
}