using System;
using System.Data;
using System.IO;
using System.Linq;
using Vestcom.PreProcSupport;
using Vestcom.PaperSign.ACME.Entities;
using Vestcom.PaperSign.ACME.DataAccessLayer.Manager;
using Vestcom.Core.Utilities.ExcelReader;
using System.Text.RegularExpressions;

namespace Vestcom.PaperSign.ACME.ProcessInput.Rules
{
    /// <summary>
    /// class ProcessISMSigns
    /// </summary>
    [Preprocessor(Constants.CustomerId, Constants.PreprocessorKey, "None")]
    class ProcessISMSigns
    {
        string strOrderNumber = string.Empty;

        /// <summary>
        /// Imports the ism data.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="filelogKey">The filelog key.</param>
        /// <param name="manager">The manager.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// File format is not correct. File need to a valid XLS file having one sheet of 6 columns.
        /// or
        /// No data found in data sheet. Please upload valid xls file having one sheet of 6 columns with valid data
        /// </exception>
        internal string ImportISMData(string filePath, string fileName, int filelogKey, IManager manager)
        {
            string[] strValues = fileName.Split(' ').Select(sValue => sValue.Trim()).ToArray();
           
            string result = "";
            DataSet dataset = ReadExcelFile.Read(Path.Combine(filePath, fileName),true);

            if (dataset == null || dataset.Tables.Count == 0)
            {
                throw new Exception("File format is not correct. File need to a valid XLS file having one sheet of 6 columns.");
            }
            else if (dataset.Tables[0].Rows.Count <= 1)
            {
                throw new Exception("No data found in data sheet. Please upload valid xls file having one sheet of 6 columns with valid data");
            }

            manager.CreateOrder(Constants.ClientId, fileName, out strOrderNumber);

            FileLog fileLog = new FileLog() { OrderNumber = strOrderNumber, FileLogKey = filelogKey, Status = FileStatus.ProcessPreProcInProgress };
            manager.UpdateFileLog(fileLog);

            for (int tableIterator = 0; tableIterator < dataset.Tables.Count; tableIterator++)
            {
                if (dataset.Tables[tableIterator].Rows.Count > 1)
                {
                    ExportExcelData(dataset.Tables[tableIterator], manager);
                }
            }

            manager.ProcessData(strOrderNumber, true);

            fileLog.Status = FileStatus.ProcessPreProcCompleted;
            manager.UpdateFileLog(fileLog);

            return result = strOrderNumber;
        }

        /// <summary>
        /// Exports the excel data.
        /// </summary>
        /// <param name="dtData">The dt data.</param>
        /// <param name="manager">The manager.</param>
        /// <exception cref="System.Exception">No valid data found in data sheet. Please upload valid xls file having one sheet of 6 columns with valid data</exception>
        public void ExportExcelData(DataTable dtData, IManager manager)
        {

            int iCounter = 0;
            bool qtyFound = false;
            bool isHeaderRow = false;

            #region Create DataRow
            DataTable dtRawData = new DataTable();
            dtRawData.Columns.Add("OrderNumber");
            dtRawData.Columns.Add("SignInfo");
            dtRawData.Columns.Add("ImageName");
            dtRawData.Columns.Add("SignDescription");
            dtRawData.Columns.Add("Qty");
            dtRawData.Columns.Add("StoreSpecific");
            dtRawData.Columns.Add("zones1");
            dtRawData.Columns.Add("ExceptStores");
            dtRawData.Columns.Add("ExceptZone");
            dtRawData.Columns.Add("SignType");
            dtRawData.Columns.Add("Laminated");
            dtRawData.Columns.Add("RigidVinyl");
            dtRawData.Columns.Add("GroupNumber", typeof(int));
            dtRawData.Columns.Add("IsProcessed", typeof(bool));
            dtRawData.Columns.Add("SignId", typeof(int));


            #endregion

            foreach (DataRow row in dtData.Rows)
            {
                qtyFound = true;

                if (isHeaderRow)
                {
                    isHeaderRow = false;
                    continue;
                }

                int drItemArrayCount = row.ItemArray.Count();
                iCounter = 0;

                DataRow drow = dtRawData.NewRow();
                
                if (iCounter < drItemArrayCount)
                {
                    drow["OrderNumber"] = strOrderNumber;
                }
                else
                {
                    continue;
                }

                drow["ImageName"] = Convert.ToString(row[iCounter]);
                iCounter++;
                drow["SignDescription"] = Convert.ToString(row[iCounter]);
                iCounter++;
                drow["Qty"] = Convert.ToString(row[iCounter]);
                iCounter++;

                string storeVal = row[iCounter] == DBNull.Value ? string.Empty : Convert.ToString(row[iCounter]).Trim();

                if (storeVal.ToLower().Contains("all stores"))
                {
                    drow["zones1"] = "All Stores";
                }
                else if (!string.IsNullOrEmpty(storeVal) && storeVal.ToLower().Contains("all except"))
                {
                    drow["zones1"] = "All Stores";

                    storeVal = Regex.Replace(storeVal, "All EXCEPT", "", RegexOptions.IgnoreCase);

                    if (!string.IsNullOrEmpty(storeVal))
                    {
                        SetZoneSoreInRow(storeVal, drow, "ExceptStores", "ExceptZone");
                    }
                }
                else if (storeVal.ToLower().Contains("only"))
                {
                    storeVal = Regex.Replace(storeVal, "Only", "", RegexOptions.IgnoreCase);
                    if (!string.IsNullOrEmpty(storeVal))
                    {
                        SetZoneSoreInRow(storeVal, drow, "StoreSpecific", "zones1");
                    }
                }
                else if (storeVal.ToLower().Contains("except"))
                {
                    string[] exceptList = { "EXCEPT", "except", "Except" };
                    var storeArr = storeVal.Split(exceptList, StringSplitOptions.None);
                    if (!string.IsNullOrEmpty(storeArr[0]))
                    {
                        SetZoneSoreInRow(storeArr[0], drow, "StoreSpecific", "zones1");
                    }
                    if (storeArr.Length>1&&!string.IsNullOrEmpty(storeArr[1]))
                    {
                        SetZoneSoreInRow(storeArr[1], drow, "ExceptStores", "ExceptZone");
                    }
                }

                iCounter++;
                drow["SignType"] = row[iCounter] == DBNull.Value ? string.Empty : Convert.ToString(row[iCounter]).Trim();

                iCounter++;
                drow["Laminated"] = row[iCounter] == DBNull.Value ? string.Empty : Convert.ToString(row[iCounter]).Trim();
                iCounter++;
                drow["RigidVinyl"] = row[iCounter] == DBNull.Value ? string.Empty : Convert.ToString(row[iCounter]).Trim();
                

                drow["GroupNumber"] = DBNull.Value;
                drow["IsProcessed"] = DBNull.Value;
                drow["SignId"] = DBNull.Value;

                dtRawData.Rows.Add(drow);
            }

            if (!qtyFound)
            {
                throw new Exception(" No valid data found in data sheet. Please upload valid xls file having one sheet of 6 columns with valid data");
            }
            manager.SaveISMRawData(dtRawData);
        }

        private void SetZoneSoreInRow(string storeVal, DataRow drow, string storeCol, string zoneCol)
        {
            string[] Zonestorearr = storeVal.Trim().Split('-');
            string ZoneTemp = null;
            string storeTemp = null;
            foreach (string Zonestore in Zonestorearr)
            {
                double storenum;
                if (double.TryParse(Zonestore, out storenum))
                {
                    storeTemp = string.IsNullOrEmpty(storeTemp) ? storenum.ToString() : $"{storeTemp}-{storenum.ToString()}";
                }
                else
                {
                    ZoneTemp = string.IsNullOrEmpty(ZoneTemp) ? Zonestore : $"{ZoneTemp}-{Zonestore}";
                }
            }
            drow[storeCol] = storeTemp;
            drow[zoneCol] = ZoneTemp;
        }

    }
}

