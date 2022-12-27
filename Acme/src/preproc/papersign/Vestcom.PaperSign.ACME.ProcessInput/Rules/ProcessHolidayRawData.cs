using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Vestcom.Core.Utilities.ExcelReader;
using Vestcom.PaperSign.ACME.DataAccessLayer.Manager;
using Vestcom.PaperSign.ACME.Entities;
using Vestcom.PreProcSupport;

namespace Vestcom.PaperSign.ACME.ProcessInput.Rules
{
    [Preprocessor(Constants.CustomerId, Constants.PreprocessorKey, "None")]
    public class ProcessHolidayRawData
    {
        string strOrderNumber = string.Empty;

        internal string ImportAcmeHolidayData(string filePath, string fileName, int filelogKey, IManager manager)
        {
            string[] strValues = fileName.Split(' ').Select(sValue => sValue.Trim()).ToArray();

            string result = "";
            DataSet dataset = ReadExcelFile.Read(Path.Combine(filePath, fileName), false);

            manager.CreateOrder(Constants.ClientId, fileName, out strOrderNumber);

            FileLog fileLog = new FileLog() { OrderNumber = strOrderNumber, FileLogKey = filelogKey, Status = FileStatus.ProcessPreProcInProgress };

            manager.UpdateFileLog(fileLog);

            SaveData(dataset.Tables[0], manager);
            manager.ProcessData(strOrderNumber, false, true);

            fileLog.Status = FileStatus.ProcessPreProcCompleted;
            manager.UpdateFileLog(fileLog);

            return result = strOrderNumber;
        }
        private void SaveData(DataTable dtData, IManager manager)
        {
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

                //To get the excl fils first cols header
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

                if (iCounter < drItemArrayCount)
                {
                    drow["OrderNumber"] = strOrderNumber;
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
                }
                if (descriptionloc != 999)
                {
                    drow["Description"] = Convert.ToString(row[descriptionloc]);
                }
                if (districtloc != 999)
                {
                    drow["District"] = Convert.ToString(row[districtloc]);
                }
                if (storephonenumberloc != 999)
                {
                    drow["Store_Phone_Number"] = Convert.ToString(row[storephonenumberloc]);
                }
                if (pharmacyphone != 999)
                {
                    drow["Pharmacy_Phone"] = Convert.ToString(row[pharmacyphone]);
                }
                if (begindate != 999)
                {
                    drow["Holiday"] = Convert.ToString(dtData.Rows[holidayrow - 1][begindate]);
                    drow["Begin_Date"] = Convert.ToString(row[begindate]);
                }
                if (enddate != 999)
                {
                    drow["End_Date"] = Convert.ToString(row[enddate]);
                }
                if (storetime != 999)
                {
                    drow["Store_Open"] = Convert.ToString(row[storetime]);
                    drow["Store_Close"] = Convert.ToString(row[storetime + 1]);
                }

                if (RXtime != 999)
                {
                    drow["RX_Open"] = Convert.ToString(row[RXtime]);
                    drow["RX_Close"] = Convert.ToString(row[RXtime + 1]);
                }

                if (regularstorehours != 999)
                {
                    drow["RegularStore_Hours_Mon_Fri_Open"] = Convert.ToString(row[regularstorehours]);
                    drow["RegularStore_Hours_Mon_Fri_Close"] = Convert.ToString(row[regularstorehours + 1]);
                    drow["RegularStore_Hours_Sat_Open"] = Convert.ToString(row[regularstorehours + 2]);
                    drow["RegularStore_Hours_Sat_Close"] = Convert.ToString(row[regularstorehours + 3]);
                    drow["RegularStore_Hours_Sun_Open"] = Convert.ToString(row[regularstorehours + 4]);
                    drow["RegularStore_Hours_Sun_Close"] = Convert.ToString(row[regularstorehours + 5]);
                }


                if (regularpharmacyhours != 999)
                {
                    drow["RegularPharmacy_Hours_Mon_Fri_Open"] = Convert.ToString(row[regularpharmacyhours]);
                    drow["RegularPharmacy_Hours_Mon_Fri_Close"] = Convert.ToString(row[regularpharmacyhours + 1]);
                    drow["RegularPharmacy_Hours_Sat_Open"] = Convert.ToString(row[regularpharmacyhours + 2]);
                    drow["RegularPharmacy_Hours_Sat_Close"] = Convert.ToString(row[regularpharmacyhours + 3]);
                    drow["RegularPharmacy_Hours_Sun_Open"] = Convert.ToString(row[regularpharmacyhours + 4]);
                    drow["RegularPharmacy_Hours_Sun_Close"] = Convert.ToString(row[regularpharmacyhours + 5]);
                }
                if (division != 999)
                {
                    drow["Division"] = Convert.ToString(row[division]);
                }
                dtRawData.Rows.Add(drow);
            }

            manager.SaveRawData(dtRawData, false, true);
        }
    }

}
