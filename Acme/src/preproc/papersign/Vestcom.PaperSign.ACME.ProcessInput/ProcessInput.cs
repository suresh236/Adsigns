using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Vestcom.AuditData;
using Vestcom.Core.Utilities;
using Vestcom.PaperSign.ACME.DataAccessLayer.Manager;
using Vestcom.PaperSign.ACME.Entities;
using Vestcom.PaperSign.ACME.ProcessInput.Rules;
using Vestcom.PreProcSupport;
using Vestcom.Core.Utilities.EmailClient.Base;
using Vestcom.Core.Utilities.ExcelReader;

namespace Vestcom.PaperSign.ACME.ProcessInput
{
    /// <summary>
    /// ProcessInput Class for Acme
    /// </summary>
    /// <seealso cref="Vestcom.PreProcSupport.IRunnableExtended" />
    [Preprocessor(Constants.CustomerId, Constants.PreprocessorKey, "None")]
    public class ProcessInput : IRunnableExtended
    {

        #region Protected Members

        protected readonly IManager manager;
        protected readonly IBusinessRule processInputRule;
        Vestcom.Connection.RealmTypes currentRealmType;
        IProcessing RecordAuditor;
        string AuditDataEmailAddresses;
        bool IsFakeAuditData;
        string packagepath;
        string strOrderNumber = "";
        int filelogKey = 0;
        #endregion

        #region Public Variables

        public IBusinessRule BusinessLogicObj { get; private set; }
        public static InputRecords records = new InputRecords();
        public DataTable dtRecordsToUpdate_Insert;
        public static int recordNumber = 0;
        #endregion

        #region Constructor

        public ProcessInput()
        {
            VestcomConfiguration.LoadConfiguration();
            new Vestcom.Connection.RegisteredCustomerContext(Constants.CustomerId);
            manager = new Manager();
            BusinessLogicObj = new ProcessInputRule();
        }

        #endregion

        #region Public Methods

        #region IRunnableExtended Implementation

        /// <summary>
        /// Runs the specified settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        public Response Run(RunSettings settings)
        {
            //This is done for Local debuging 
            //IsFakeAuditData = true;

            Response response = new Response(ResponseType.Success);
            currentRealmType = GetRealmType(settings.Realm);

            packagepath = settings.PackagePath;

            SetAuditDataEmailAddresses();

            if (!IsFakeAuditData)
            {
                RecordAuditor = new Processing((Vestcom.Connection.RealmTypes)currentRealmType);
            }
            else
            {
                RecordAuditor = new FakeProcessing();
            }

            int clientId = Constants.ClientId;
            List<string> files = settings.Files;
            bool holiday = false;
            Exception exRunStatus = null;
            bool IsKeHeFile=false;
            for (int i = 0; i < files.Count; i++)
            {
                try
                {                  
                    string[] strValues = files[i].Split('_').Select(sValue => sValue.Trim()).ToArray();
                    FileLog fileLog = new FileLog();
                    if (strValues != null && strValues.Length > 2 && strValues[0] == Convert.ToString(clientId) && strValues[2].ToLower().Contains("processdata"))
                    {
                        strOrderNumber = strValues.ElementAtOrDefault(1);
                    }
                    else
                    {
                        string fileName = files[i];
                        string filePath = settings.PackagePath;
                        manager.AddFileLogData(clientId, fileName, filePath, 0, out filelogKey);
                        fileLog.FileLogKey = filelogKey;                        
                        string fileExt = Path.GetExtension(Path.Combine(filePath, fileName)).ToLower();
                        bool isValidExt = false;

                        var filedata = ReadExcelFile.Read(Path.Combine(filePath, fileName), true);
                        int columnCount = filedata.Tables[0].Columns.Count;
                        if (string.Equals(fileExt, ".xls") || string.Equals(fileExt, ".xlsx"))
                        {
                            if (filedata.Tables.Contains("ACME"))
                            {
                                ProcessHolidayRawData obj = new ProcessHolidayRawData();
                                strOrderNumber = obj.ImportAcmeHolidayData(filePath, fileName, filelogKey, manager);
                                isValidExt = true;
                                holiday = true;
                            }
                            else if (columnCount == 6 || columnCount == 7)
                            {
                                ProcessISMSigns obj = new ProcessISMSigns();
                                strOrderNumber = obj.ImportISMData(filePath, fileName, filelogKey, manager);
                                isValidExt = true;
                            }
                            else if(Path.GetFileNameWithoutExtension(Path.Combine(filePath, fileName)).ToUpper().Contains("KEHE"))
                            {
                                IsKeHeFile = true;
                                ProcessSigns obj = new ProcessSigns();
                                strOrderNumber = obj.ImportAcmeData(filePath, fileName, filelogKey, manager, IsKeHeFile);
                                isValidExt = true;
                            }
                            else 
                            {
                                ProcessSigns obj = new ProcessSigns();
                                strOrderNumber = obj.ImportAcmeData(filePath, fileName, filelogKey, manager);
                                isValidExt = true;
                            }
                           
                        }

                        if (!isValidExt)
                        {
                            throw new Exception(Constants.ISMErrorMessages.NoValidExtension);
                        }
                    }

                    fileLog.OrderNumber = strOrderNumber;
                    fileLog.Status = FileStatus.ProcessPreProcInProgress;
                    manager.UpdateFileLog(fileLog);

                    if (holiday)
                    {
                        var holidayRecords = manager.GetAcmeHolidaySigns(clientId, strOrderNumber);
                        ExecuteRules(clientId, strOrderNumber, holidayRecords.HolidayInputFileRecords.ToList(), holidayRecords);

                        List<HolidayInputTypes> holidayinputtypes = holidayRecords.HolidayInputFileRecords.Select(a =>new HolidayInputTypes()
                                                                            { Id = a.DATA_NUM_IN,SignSizeId = a.SignSizeId, SignHeaderId = a.SignHeaderId }).ToList();

                        manager.SaveHolidayInput(holidayinputtypes, holidayRecords.Exceptions);
                    }
                    else
                    {
                        records = manager.GetAcmeSigns(clientId, strOrderNumber);
                        ExecuteRules(clientId, strOrderNumber, records.InputFileRecords.ToList(), IsKeHeFile);
                        manager.SaveInput(dtRecordsToUpdate_Insert, records.Exceptions);
                    }
                    fileLog.Status = FileStatus.ProcessPreProcCompleted;
                    manager.UpdateFileLog(fileLog);
                }
                catch (Exception ex)
                {
                    FileLog fileLog = new FileLog()
                    {
                        OrderNumber = strOrderNumber,Status = FileStatus.ProcessPreProcFailed, ErrorMesssage = ex.Message,FileLogKey = filelogKey
                    };

                    manager.UpdateFileLog(fileLog);
                    exRunStatus = ex;
                    SendExceptionEmail(ex.Message);
                }
            }

            if (exRunStatus == null)
            {
                RecordAuditor.SaveAuditData(GetProgramName(), GetProgramVersion(), "Acme", Processing.RunStatus.NoErrors, true, AuditDataEmailAddresses);
            }
            else
            {
                response.ResponseState = ResponseType.Failure;
                response.Exceptions.Add(exRunStatus);
                RecordAuditor.SaveAuditData(GetProgramName(), GetProgramVersion(), "Acme", Processing.RunStatus.WithErrors, true, AuditDataEmailAddresses, runTimeException: exRunStatus);
            }

            return response;
        }


        /// <summary>
        /// Runs the specified base path.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        /// <param name="files">The files.</param>
        /// <param name="realmType">Type of the realm.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Response Run(string basePath, List<string> files, int realmType)
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// Executes the rules.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="orderNumber">The order number.</param>
        /// <param name="inputFileRecords">The input file records.</param>
        public void ExecuteRules(int clientId, string orderNumber, List<InputFile> inputFileRecords,bool IsKeHeFile=false)
        {
            recordNumber = 0;
            List<int> departments = new List<int>();
            CreateTableForRecordsToUpdate_Insert();
            ProcessISMInputRule inputRuleISM = new ProcessISMInputRule();
            for (int i = 0; i < inputFileRecords.Count(); i++)
            {              

                InputFile inputFile = inputFileRecords.ElementAtOrDefault(i);

                int keHeqty = Helper.ConvertStringToInteger(inputFile.MFL_SIZE1_IN);

                if (inputFile.IsISM)
                {
                    inputRuleISM.ApplyRules(inputFile, records);
                }
                else
                {
                    // Apply Business Rule on Input File Record
                    BusinessLogicObj.ApplyRules(inputFile, clientId, packagepath);
                    // Add SignSize & SignHeader Logic
                    inputFile = BusinessLogicObj.DownbakExecuteRules(inputFile, records, IsKeHeFile);
                    if (IsKeHeFile)
                    {
                        inputFile.OS_QUANTITY = keHeqty;
                    }


                    // Update Deparment Record
                    if (!departments.Contains(inputFile.DepartmentId) && inputFile.DepartmentId > 0)
                    {
                        departments.Add(inputFile.DepartmentId);
                    }
                    if (ProcessInputRule.InputFileList.Count > 0)
                    {
                        foreach (InputFile file in ProcessInputRule.InputFileList)
                        {
                            file.SignDataId = -1;
                            AddInputRecordToDataTable(file, inputFile.SignDataId);

                        }
                    }
                }

                AddInputRecordToDataTable(inputFile, -1);
                manager.UpdateDepartmentId(orderNumber);

            }
        }


        /// <summary>
        /// Executes the rules.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="orderNumber">The order number.</param>
        /// <param name="inputFileRecords">The input file records.</param>
        public void ExecuteRules(int clientId, string orderNumber, List<HolidayInput> inputFileRecords, HolidayInputRecords records)
        {
            recordNumber = 0;
            List<int> departments = new List<int>();
            ProcessHolidayInputRule processholidayinputrule = new ProcessHolidayInputRule();
            foreach (HolidayInput inputFile in inputFileRecords)
            {
                processholidayinputrule.ApplyRules(inputFile, records, clientId, packagepath);
            }

        }


        /// <summary>
        /// Creates the table for records to update insert.
        /// </summary>
        public void CreateTableForRecordsToUpdate_Insert()
        {

            dtRecordsToUpdate_Insert = new DataTable("InputRecords");

            dtRecordsToUpdate_Insert.Columns.Add("ID", typeof(int));
            dtRecordsToUpdate_Insert.Columns.Add("DepartmentId", typeof(int));
            dtRecordsToUpdate_Insert.Columns.Add("adSignZoneData_ID", typeof(int));
            dtRecordsToUpdate_Insert.Columns.Add("OrderNumber", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("DATA_NUM_IN", typeof(int));
            dtRecordsToUpdate_Insert.Columns.Add("DepartmentName", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("EffectiveDate", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("ExpiryDate", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("SectionId", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("AdOverLine", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("AdBlowLine", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("AdSizeDescription", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("SubDescription", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("SplInstruction", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("WeightedItem", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("NuvalScore", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("UPC", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("Attribute1", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("Retail1PRC", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("Retail2PRC", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("UnitPrice", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("PPCText", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("Limit", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("LB", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("Savings", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("Attribute2", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("Attribute4", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("Attribute3", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("Attribute5", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("Attribute6", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("Attribute8", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("Attribute7", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("Attribute9", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("Attribute10", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("Format", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("PrivateLblCd", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("Retail1For", typeof(int));
            dtRecordsToUpdate_Insert.Columns.Add("Retail2For", typeof(int));
            dtRecordsToUpdate_Insert.Columns.Add("SignQty", typeof(int));
            dtRecordsToUpdate_Insert.Columns.Add("MasteradsignID", typeof(int));
            dtRecordsToUpdate_Insert.Columns.Add("SignSizeId", typeof(int));
            dtRecordsToUpdate_Insert.Columns.Add("SignHeaderId", typeof(int));
            dtRecordsToUpdate_Insert.Columns.Add("Ten_FOR_10_IN", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("SL_SIGN_HEADING", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("SL_SIGN_SIZE", typeof(string));
            dtRecordsToUpdate_Insert.Columns.Add("RowNum", typeof(int));

        }

        /// <summary>
        /// Adds the input record to data table.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="MasteradsignID">The masteradsign identifier.</param>
        public void AddInputRecordToDataTable(InputFile record, int MasteradsignID)
        {

            DataRow newRow = dtRecordsToUpdate_Insert.NewRow();
            newRow["ID"] = record.SignDataId;
            newRow["DepartmentId"] = record.DepartmentId;
            newRow["adSignZoneData_ID"] = record.ZonePriceId;
            newRow["OrderNumber"] = record.OrderNumber;
            newRow["DATA_NUM_IN"] = record.DATA_NUM_IN;
            newRow["DepartmentName"] = record.DEPT_NAME_IN;
            newRow["EffectiveDate"] = record.FROM_DATE_IN;
            newRow["ExpiryDate"] = record.END_DATE_IN;
            newRow["Retail1For"] = record.REG_MULTI_IN;
            newRow["Retail2For"] = record.SALE_MULTI_IN;
            newRow["SignQty"] = record.OS_QUANTITY;
            newRow["SectionId"] = record.BOARS_HEAD_IN;
            newRow["AdOverLine"] = record.PROD_BRAND_IN;
            newRow["AdBlowLine"] = record.ITEM_DESC_IN;
            newRow["AdSizeDescription"] = record.SIZE_IN;
            newRow["SubDescription"] = record.SECOND_MSG_IN;
            newRow["SplInstruction"] = record.COOL_IN;
            newRow["WeightedItem"] = record.BOGO_TYPE_INFO_IN;
            newRow["NuvalScore"] = record.PERCENT_IN;
            newRow["Ten_FOR_10_IN"] = record.Ten_FOR_10_IN;
            newRow["UPC"] = record.UPC_IN;
            newRow["Attribute1"] = record.DISCLAIMER_IN;
            newRow["Retail1PRC"] = record.REG_RETAIL_IN;
            newRow["Retail2PRC"] = record.SALE_PR_IN;
            newRow["UnitPrice"] = record.UNIT_PR_IN;
            newRow["PPCText"] = record.UOM_IN;
            newRow["Limit"] = record.LIMIT_IN;
            newRow["LB"] = record.MUST_BUY_IN;
            newRow["Savings"] = record.SAVINGS_IN;
            newRow["Attribute2"] = record.IMAGE_IN;
            newRow["Attribute4"] = record.MFL_SIZE1_IN;
            newRow["Attribute3"] = record.MFL_SIZE2_IN;
            newRow["Attribute5"] = record.MFL_SIZE3_IN;
            newRow["Attribute6"] = record.MFL_SIZE6_IN;
            newRow["Attribute8"] = record.EDLP_SIZE1_IN;
            newRow["Attribute7"] = record.EDLP_SIZE2_IN;
            newRow["Attribute9"] = record.EDLP_SIZE3_IN;
            newRow["Attribute10"] = record.EDLP_SIZE6_IN;
            newRow["Format"] = record.DUPLEX_IN;
            newRow["PrivateLblCd"] = record.RESTRICT_CODE;
            newRow["SL_SIGN_HEADING"] = record.SL_SIGN_HEADING;
            newRow["SL_SIGN_SIZE"] = record.SL_SIGN_SIZE;
            if (record.SignSizeId == null)
            {
                newRow["SignSizeId"] = DBNull.Value;
            }
            else
            {
                newRow["SignSizeId"] = record.SignSizeId;
            }

            if (record.SignHeaderId == null)
            {
                newRow["SignHeaderId"] = DBNull.Value;
            }
            else
            {
                newRow["SignHeaderId"] = record.SignHeaderId;
            }


            if (MasteradsignID != -1)
            {
                newRow["MasteradsignID"] = MasteradsignID;
            }
            newRow["RowNum"] = record.RowNum;
            dtRecordsToUpdate_Insert.Rows.Add(newRow);

        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the program version.
        /// </summary>
        /// <returns></returns>
        private string GetProgramVersion()
        {
            return System.Windows.Forms.Application.ProductVersion.ToString();
        }
        /// <summary>
        /// Gets the name of the program.
        /// </summary>
        /// <returns></returns>
        private string GetProgramName()
        {
            return Assembly.GetExecutingAssembly().FullName;
        }

        /// <summary>
        /// Gets the type of the realm.
        /// </summary>
        /// <param name="realmType">Type of the realm.</param>
        /// <returns></returns>
        private static Vestcom.Connection.RealmTypes GetRealmType(int realmType)
        {
            Vestcom.Connection.RealmTypes activeRealm = Vestcom.Connection.RealmTypes.None;
            switch (realmType)
            {
                case 2:
                    activeRealm = Vestcom.Connection.RealmTypes.Production;
                    break;
                case 1:
                    activeRealm = Vestcom.Connection.RealmTypes.Test;
                    break;
            }
            return activeRealm;
        }

        /// <summary>
        /// Sets the audit data email addresses.
        /// </summary>
        /// <exception cref="System.ApplicationException">Invalid Realm Type: " + currentRealmType.ToString()</exception>
        private void SetAuditDataEmailAddresses()
        {
            if (currentRealmType == Vestcom.Connection.RealmTypes.Test)
            {
                AuditDataEmailAddresses = Constants.TestAuditDataEmailAddresses;
            }
            else if (currentRealmType == Vestcom.Connection.RealmTypes.Production)
            {
                AuditDataEmailAddresses = Constants.ProdAuditDataEmailAddresses;
            }
            else
            {
                throw new ApplicationException("Invalid Realm Type: " + currentRealmType.ToString());
            }

        }
        #endregion
        private void SendExceptionEmail(string message)
        {
            string mailTo = VestcomConfiguration.Config.AppSettings.Settings["ExceptionEmailAddresses"].Value;
            string subject = "Acme adSignDataImport - Failed";
            string bodyContent = Environment.MachineName + Environment.NewLine + "AcmeDataImport" + Environment.NewLine + DateTime.Now + Environment.NewLine;
            bodyContent += "Failed Work Order Numbers: " + strOrderNumber + Environment.NewLine + "Acme adSignDataImport  -   Failed. Please see the log or contact Administrator" + Environment.NewLine;
            bodyContent += "Failure Message:" + message;
            SMTPSender obj = new SMTPSender();
            obj.SendEmail(mailTo, subject, bodyContent);
        }
    }
}
