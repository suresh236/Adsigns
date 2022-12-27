using System;
using System.Data;
using System.IO;
using System.Linq;
using Vestcom.PreProcSupport;
using Vestcom.PaperSign.ACME.Entities;
using Vestcom.PaperSign.ACME.DataAccessLayer.Manager;
using Vestcom.Core.Utilities.ExcelReader;

namespace Vestcom.PaperSign.ACME.ProcessInput.Rules
{
    [Preprocessor(Constants.CustomerId, Constants.PreprocessorKey, "None")]
    public class ProcessSigns
    {
        string strOrderNumber = string.Empty;

        internal string ImportAcmeData(string filePath, string fileName, int filelogKey, IManager manager,bool IsKeHeFile=false)
        {
            string[] strValues = fileName.Split(' ').Select(sValue => sValue.Trim()).ToArray();

            string result = "";
            DataSet dataset = ReadExcelFile.Read(Path.Combine(filePath, fileName), false);

            if (dataset == null || dataset.Tables.Count == 0)
            {
                throw new Exception(Constants.ISMErrorMessages.NoValidDataOrFormatException);
            }
            else if (dataset.Tables[0].Rows.Count <= 1)
            {
                throw new Exception(Constants.ISMErrorMessages.NoDataFoundException);
            }
            
            //if (dataset.Tables[0].Columns.Count > 45)
            //{
            //    throw new Exception(" Number of columns available in data sheet are greater than expected. Expected number of columns are: 45. Current Number of columns are: " + dataset.Tables[0].Columns.Count.ToString());
            //}
            manager.CreateOrder(Constants.ClientId, fileName, out strOrderNumber);

            FileLog fileLog = new FileLog() { OrderNumber = strOrderNumber, FileLogKey = filelogKey, Status = FileStatus.ProcessPreProcInProgress };
            manager.UpdateFileLog(fileLog);
            if (dataset.Tables[0].Columns.Count <= 6)
            {
                SaveISMData(dataset.Tables[0], manager);
                manager.ProcessData(strOrderNumber, true);
            }
            else
            {
                if (dataset.Tables[0].Columns.Count < 44)
                {
                    throw new Exception(" Number of columns available in data sheet are less than expected. Expected number of columns are: greater then 43. Current Number of columns are: " + dataset.Tables[0].Columns.Count.ToString());
                }
                for (int tableIterator = 0; tableIterator < dataset.Tables.Count; tableIterator++)
                {
                    if (dataset.Tables[tableIterator].Rows.Count > 1)
                    {
                        ExportExcelData(dataset.Tables[tableIterator], manager);
                    }
                }
                manager.ProcessData(strOrderNumber, false, IsKeHeFile:IsKeHeFile);
            }
           
            fileLog.Status = FileStatus.ProcessPreProcCompleted;
            manager.UpdateFileLog(fileLog);

            return result = strOrderNumber;
        }
        private void SaveISMData(DataTable dtData, IManager manager)
        {
            #region Create DataRow
            DataTable dtRawData = new DataTable();
            dtRawData.Columns.Add("OrderNumber");
            dtRawData.Columns.Add("SignInfo");
            dtRawData.Columns.Add("ImageName");
            dtRawData.Columns.Add("SignDescription");
            dtRawData.Columns.Add("Qty");
            dtRawData.Columns.Add("StoreSpecific");
            dtRawData.Columns.Add("SignType");
            dtRawData.Columns.Add("Laminated");
            dtRawData.Columns.Add("GroupNumber", typeof(int));
            dtRawData.Columns.Add("IsProcessed", typeof(bool));
            dtRawData.Columns.Add("SignId", typeof(int));
            #endregion
            bool headerFound = false;

            foreach (DataRow row in dtData.Rows)
            {
                if (row[0].ToString() != "Image Name" && !headerFound)
                {
                    continue;
                }
                headerFound = true;
                int colIndex = 0;
                if (string.IsNullOrEmpty(row[colIndex].ToString()) || row[colIndex].ToString().Equals("Image Name"))
                {
                    continue;
                }

                int drItemArrayCount = row.ItemArray.Count();
                int iCounter = 0;

                if (string.IsNullOrEmpty(Convert.ToString(row[0])) && string.IsNullOrEmpty(Convert.ToString(row[0])))
                {
                    continue;
                }

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
                drow["StoreSpecific"] = Convert.ToString(row[iCounter]);
                iCounter++;
                drow["SignType"] = Convert.ToString(row[iCounter]);
                iCounter++;
                drow["Laminated"] = Convert.ToString(row[iCounter]);
                iCounter++;

                drow["GroupNumber"] = DBNull.Value;
                drow["IsProcessed"] = DBNull.Value;
                drow["SignId"] = DBNull.Value;
                dtRawData.Rows.Add(drow);
            }
            
            manager.SaveRawData(dtRawData, true);
        }
        private void ExportExcelData(DataTable dtData, IManager manager)
        {
            int iCounter = 0;

            #region Create DataRow
            DataTable dtRawData = new DataTable();
            dtRawData.Columns.Add("OrderNumber");
            dtRawData.Columns.Add("Department");
            dtRawData.Columns.Add("DateFrom");
            dtRawData.Columns.Add("DateTO");
            dtRawData.Columns.Add("Zone2");
            dtRawData.Columns.Add("Zone3");
            dtRawData.Columns.Add("Zone4");
            dtRawData.Columns.Add("Zone5");
            dtRawData.Columns.Add("Zone6");
            dtRawData.Columns.Add("Zone7");
            dtRawData.Columns.Add("Zone8");
            dtRawData.Columns.Add("Zone9");
            dtRawData.Columns.Add("Zone7849");
            dtRawData.Columns.Add("Zone87");
            dtRawData.Columns.Add("Zone88");
            dtRawData.Columns.Add("Zone89");
            dtRawData.Columns.Add("Zone896");
            dtRawData.Columns.Add("BoarsHead");
            dtRawData.Columns.Add("Zone779");
            dtRawData.Columns.Add("Brand");
            dtRawData.Columns.Add("Item");
            dtRawData.Columns.Add("Description");
            dtRawData.Columns.Add("FreeFormField");
            dtRawData.Columns.Add("COOL");
            dtRawData.Columns.Add("BOGOType");
            dtRawData.Columns.Add("Percent");
            dtRawData.Columns.Add("UPC");
            dtRawData.Columns.Add("Disclaimer");
            dtRawData.Columns.Add("RegularPriceQty");
            dtRawData.Columns.Add("RegularPrice");
            dtRawData.Columns.Add("SalePriceQty");
            dtRawData.Columns.Add("SalePrice");
            dtRawData.Columns.Add("UnitPrice");
            dtRawData.Columns.Add("UnitOfMeasure");
            dtRawData.Columns.Add("Limit");
            dtRawData.Columns.Add("MustBuy");
            dtRawData.Columns.Add("Savings");
            dtRawData.Columns.Add("Image");
            dtRawData.Columns.Add("SML11X7");
            dtRawData.Columns.Add("SML8X3");
            dtRawData.Columns.Add("SML5X3");
            dtRawData.Columns.Add("SML6X2");
            dtRawData.Columns.Add("ELP11X7");
            dtRawData.Columns.Add("ELP8X3");
            dtRawData.Columns.Add("ELP5X3");
            dtRawData.Columns.Add("ELP6X2");
            dtRawData.Columns.Add("Sided2");
            dtRawData.Columns.Add("GroupNumber", typeof(int));
            dtRawData.Columns.Add("IsProcessed", typeof(bool));
            dtRawData.Columns.Add("SignId", typeof(int));


            int Zone779=0;

            for (int i = 0; i < dtData.Rows[0].ItemArray.Count(); i++)
            {
                if (Convert.ToString(dtData.Rows[0].ItemArray[i]).EmptyNull().Trim().ToUpper() == "ZONE 779")
                {
                    Zone779 = i;
                }
            }


            #endregion
            int blankRowCounter = 1;
            for (int rCnt = 1; rCnt < dtData.Rows.Count; rCnt++)
            {

                int drItemArrayCount = dtData.Rows[rCnt].ItemArray.Count();
                iCounter = 0;
                DataRow drow = dtRawData.NewRow();
                drow["OrderNumber"] = strOrderNumber;
                if (iCounter < drItemArrayCount)
                {
                    if (Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim() == "")
                    {
                        blankRowCounter++;
                    }
                    drow["Department"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]);
                    iCounter++;
                }
                else
                {
                    continue;
                }
                #region assign data to to data row
                if (string.IsNullOrEmpty(Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter])))
                {
                    drow["DateFrom"] = DBNull.Value;
                }
                else
                {
                    DateTime fromXlsxDate;
                    if (DateTime.TryParse(Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]), out fromXlsxDate))
                    {
                        drow["DateFrom"] = fromXlsxDate;
                    }
                    else
                    {
                        DateTime fromDate = DateTime.FromOADate(double.Parse(Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]))).Date;
                        drow["DateFrom"] = fromDate;
                    }
                        
                }
                iCounter++;
                if (string.IsNullOrEmpty(Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter])))
                {
                    drow["DateTo"] = DBNull.Value;
                }
                else
                {
                    DateTime toXlsxDate;
                    if (DateTime.TryParse(Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]), out toXlsxDate))
                    {
                        drow["DateTo"] = toXlsxDate;
                    }
                    else
                    {
                        DateTime toDate = DateTime.FromOADate(double.Parse(Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]))).Date;
                        drow["DateTo"] = toDate;
                    }
                }
                iCounter++;
                drow["Zone2"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                iCounter++;
                drow["Zone3"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                iCounter++;
                drow["Zone4"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                iCounter++;
                drow["Zone5"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                iCounter++;
                drow["Zone6"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                iCounter++;
                drow["Zone7"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                iCounter++;
                drow["Zone8"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                iCounter++;
                drow["Zone9"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                iCounter++;
                drow["Zone7849"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                iCounter++;
                drow["Zone87"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                iCounter++;
                drow["Zone88"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                iCounter++;
                drow["Zone89"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                iCounter++;
                drow["Zone896"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                iCounter++;

                drow["BoarsHead"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                iCounter++;

                if (Zone779 > 0)
                {
                    drow["Zone779"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[Zone779]).Trim();
                    iCounter++;
                }

                drow["Brand"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                iCounter++;
                drow["Item"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                iCounter++;
                drow["Description"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                iCounter++;
                drow["FreeFormField"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                iCounter++;

                if (iCounter < drItemArrayCount)
                {
                    drow["COOL"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                    iCounter++;
                }
                if (iCounter < drItemArrayCount)
                {
                    drow["BOGOType"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                    iCounter++;
                }
                if (iCounter < drItemArrayCount)
                {
                    drow["Percent"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                    iCounter++;
                }
                if (iCounter < drItemArrayCount)
                {
                    drow["UPC"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                    iCounter++;
                }
                if (iCounter < drItemArrayCount)
                {
                    drow["Disclaimer"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                    iCounter++;
                }
                if (iCounter < drItemArrayCount)
                {
                    drow["RegularPriceQty"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                    iCounter++;
                }
                if (iCounter < drItemArrayCount)
                {
                    drow["RegularPrice"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                    iCounter++;
                }
                if (iCounter < drItemArrayCount)
                {
                    drow["SalePriceQty"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                    iCounter++;
                }
                if (iCounter < drItemArrayCount)
                {
                    drow["SalePrice"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                    iCounter++;
                }
                if (iCounter < drItemArrayCount)
                {
                    drow["UnitPrice"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                    iCounter++;
                }
                if (iCounter < drItemArrayCount)
                {
                    drow["UnitOfMeasure"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                    iCounter++;
                }
                if (iCounter < drItemArrayCount)
                {
                    drow["Limit"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                    iCounter++;
                }
                if (iCounter < drItemArrayCount)
                {
                    drow["MustBuy"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                    iCounter++;
                }
                if (iCounter < drItemArrayCount)
                {
                    if (Helper.ConvertStringToDouble(Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter])) > 0)
                    {
                        drow["Savings"] = string.Format("{0:0.00}", Helper.ConvertStringToDouble(Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim()), 2);
                        iCounter++;
                    }
                    else
                    {
                        drow["Savings"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                        iCounter++;
                    }

                }
                if (iCounter < drItemArrayCount)
                {
                    drow["Image"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                    iCounter++;
                }
                if (iCounter < drItemArrayCount)
                {
                    drow["SML11X7"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                    iCounter++;
                }
                if (iCounter < drItemArrayCount)
                {
                    drow["SML8X3"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                    iCounter++;
                }
                if (iCounter < drItemArrayCount)
                {
                    drow["SML5X3"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                    iCounter++;
                }
                if (iCounter < drItemArrayCount)
                {
                    drow["SML6X2"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                    iCounter++;
                }
                if (iCounter < drItemArrayCount)
                {
                    drow["ELP11X7"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                    iCounter++;
                }
                if (iCounter < drItemArrayCount)
                {
                    drow["ELP8X3"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                    iCounter++;
                }
                if (iCounter < drItemArrayCount)
                {
                    drow["ELP5X3"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                    iCounter++;
                }
                if (iCounter < drItemArrayCount)
                {
                    drow["ELP6X2"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                    iCounter++;
                }
                if (iCounter < drItemArrayCount)
                {
                    drow["Sided2"] = Convert.ToString(dtData.Rows[rCnt].ItemArray[iCounter]).Trim();
                    iCounter++;
                }


                drow["GroupNumber"] = DBNull.Value;
                drow["IsProcessed"] = DBNull.Value;
                drow["SignId"] = DBNull.Value;
                dtRawData.Rows.Add(drow);
                #endregion
            }
            if (dtData.Rows.Count == blankRowCounter)
            {
                iCounter = 0;
                throw new Exception(Constants.ISMErrorMessages.NoValidDataOrFormatException);
            }
            manager.SaveRawData(dtRawData,false);
        }
    }
}
