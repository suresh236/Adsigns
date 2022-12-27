using System;
using System.Collections.Generic;
using Vestcom.PreProcSupport;
using System.IO;
using System.Linq;
using Vestcom.PaperSign.ACME.ProcessDownback.Rules;
using Vestcom.PaperSign.ACME.Entities;
using Vestcom.PaperSign.ACME.DataAccessLayer.Manager;
using Vestcom.AuditData;
using Vestcom.Core.Utilities;
using System.Reflection;
using System.Configuration;
using Newtonsoft.Json;
using Vestcom.Core.Utilities.EmailClient.Base;
using Vestcom.Processors.Common.Interfaces;

namespace Vestcom.PaperSign.ACME.ProcessDownback
{
    /// <summary>
    /// ProcessDownback class for Acme
    /// </summary>
    /// <seealso cref="Vestcom.PreProcSupport.IRunnableExtended" />
    [Preprocessor(Constants.CustomerId, Constants.PreprocessorKey, "None")]
    public class ProcessDownback : IRunnableExtended, IPortalDataProcessor
    {
        #region Protected Members
        protected readonly IManager manager;
        protected readonly IBusinessRule processDownbackRule;
        protected readonly IHolidayBusinessRule processHolidayDownbackRule;
        Vestcom.Connection.RealmTypes currentRealmType;
        IProcessing RecordAuditor;
        bool IsKeepLocal;
        public static bool IsFakeAuditData;
        string AuditDataEmailAddresses;
        #endregion

        #region Pubic Members
        public static ACMEDownbackRecords records = new ACMEDownbackRecords();
        public static ACMEHolidayDownbackRecords holidayrecords = new ACMEHolidayDownbackRecords();

        public static int ORDERS_CREATED = 0;
        public static int ORDER_SIGNS_CREATED = 0;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessDownback"/> class.
        /// </summary>
        public ProcessDownback()
        {
            VestcomConfiguration.LoadConfiguration();
            new Vestcom.Connection.RegisteredCustomerContext(Constants.CustomerId);
            manager = new Manager();
            processDownbackRule = new ProcessDownbackRule();
            processHolidayDownbackRule = new ProcessHolidayDownbackRule();
        }

        #endregion

        #region IRunnableExtended Implementation

        /// <summary>
        /// This is first method which is call by PreprocEngine.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public Response Run(RunSettings settings)
        {
            //This is done for Local debuging 
            //IsFakeAuditData = IsKeepLocal = true;
            Response response = new Response(ResponseType.Success);
            currentRealmType = GetRealmType(settings.Realm);
            SetAuditDataEmailAddresses();
            if (!IsFakeAuditData)
            {
                RecordAuditor = new Processing((Vestcom.Connection.RealmTypes)currentRealmType);
            }
            else
            {
                RecordAuditor = new FakeProcessing();
            }

            try
            {
                string strClientId = "", strBatchNumber = "", strProofType = "";
                string packagePath = settings.PackagePath;
                string reportPath = Path.Combine(packagePath, Constants.REPORTFILEFOLDER);
                if (!Directory.Exists(reportPath))
                {
                    Directory.CreateDirectory(reportPath);
                }

                List<string> files = settings.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    string[] strValues = Path.GetFileNameWithoutExtension(files[i]).Split('_').Select(sValue => sValue.Trim()).ToArray();
                    if (strValues.Length > 2)
                    {
                        strClientId = strValues[0];
                        strBatchNumber = strValues[1];
                        strProofType = strValues[2];
                    }
                    
                    records = manager.GetDownbackSigns(Convert.ToInt32(strClientId), strBatchNumber, strProofType, 0);
                    GetHolidayRecords();

                    bool isHolidayRecordsExits = holidayrecords.DownBackInputRecords.Any();
                    string fileName = Convert.ToString(strClientId) + "_" +
                                            Convert.ToString(strBatchNumber) + "_" +
                                            Convert.ToString(strProofType) + "_" +
                                            Convert.ToString(DateTime.Now.ToString("yyyyMMddTHHmmss")) + (isHolidayRecordsExits ? "=ACMEHolidayHourSign" : "=ACMEPaperSign");
                    string strFilePath = Path.Combine(packagePath, fileName);

                    if (isHolidayRecordsExits)
                    {
                        processHolidayDownbackRule.ExecuteRules(Convert.ToInt32(strClientId),holidayrecords, strFilePath, strBatchNumber, strProofType, reportPath);
                    }

                    records.DownBackInputRecords = records.DownBackInputRecords.Where(s => s.SIGNSIZE_IN != Constants.holidaySignCode).ToList();

                    if (records.DownBackInputRecords.Any())
                    {
                        if (records.DownBackInputRecords.FirstOrDefault().DEPT_NAME_IN.ToUpper() == "GROCERY-KEHE")
                        {
                            records.DownBackInputRecords=records.DownBackInputRecords.OrderBy(x => x.DATA_NUM_IN).ToList();
                        }

                        processDownbackRule.ExecuteRules(Convert.ToInt32(strClientId), strFilePath, strBatchNumber, strProofType, reportPath);
                    }
                    
                    //update tagtype in db
                    if (strProofType.ToUpperInvariant().Trim() == "PROOF")
                    {
                        manager.UpdateTagType(records);
                    }
                    if (!IsKeepLocal)
                    {
                        File.Copy(strFilePath, Path.Combine(GetOutputPath(strProofType), fileName));
                    }
                    //Send Email
                    SendMail(packagePath, files[i]);
                }
                RecordAuditor.SaveAuditData(GetProgramName(), GetProgramVersion(), "Acme", Processing.RunStatus.NoErrors, true, AuditDataEmailAddresses);
            }
            catch (Exception ex)
            {
                response.ResponseState = ResponseType.Failure;
                response.Exceptions.Add(ex);
                if (RecordAuditor != null)
                {
                    RecordAuditor.SaveAuditData(GetProgramName(), GetProgramVersion(), "Acme", Processing.RunStatus.WithErrors, true, AuditDataEmailAddresses, runTimeException: ex);
                }
            }

            return response;
        }

        /// <summary>
        /// Email downback reports
        /// </summary>
        private void SendMail(string packagePath, string fileName)
        {
            if (records.DownBackInputRecords.Count > 0)
            {
                string orderNumber = records.DownBackInputRecords.FirstOrDefault().OS_ORDER_NUMBER;
                int RECORDS_SKIP = records.DownBackInputRecords.Where(x => x.RECORD_SKIP == true).ToList().Count;
                List<FileInfo> attachments = null;
                string reportName = string.Empty;
                if (RECORDS_SKIP > 0)
                {
                    string reportsFileName = Path.Combine(packagePath, Constants.REPORTFILEFOLDER) + "\\Reports.txt";
                    FileInfo file = new FileInfo(reportsFileName);
                    attachments = new List<FileInfo>();
                    attachments.Add(file);
                    reportName = "Reports.txt";
                }
                string toAddress = VestcomConfiguration.Config.AppSettings.Settings["MailTo"].Value;
                string subject = VestcomConfiguration.Config.AppSettings.Settings["Subject"].Value;
                subject = subject.Replace("{ClientName}", "Acme");
                subject = subject.Replace("{OrderNumber}", orderNumber);
                string htmlMessage = VestcomConfiguration.Config.AppSettings.Settings["Message"].Value;
                htmlMessage = string.IsNullOrEmpty(reportName) ? "There is no record in exception for " + orderNumber + " and data file " + fileName + "." : htmlMessage.Replace("{ReportName}", reportName) + ".";
                htmlMessage += (RECORDS_SKIP > 0) ? Environment.NewLine + "No. of exception records - " + RECORDS_SKIP + "." : "";
                SMTPSender sender = new SMTPSender();
                sender.SendEmail(toAddress, subject, htmlMessage, attachments);
            }
        }

        /// <summary>
        /// Runs the specified client identifier.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="signId">The sign identifier.</param>
        /// <returns></returns>
        public Dictionary<string, string> Run(int clientId, int signId)
        {
            Dictionary<string, string> headerAndValues = ProcessDownbackRule.GetDownBackFileColumns();
            records = manager.GetDownbackSigns(clientId, string.Empty, "PREVIEW", signId);
            (processDownbackRule as ProcessDownbackRule)?.ProcessOrderAndSign(records.DownBackInputRecords.FirstOrDefault(), headerAndValues);

            // Mimic Mapping Functoid to concatenate these two fields to get a Sign Size...
            if (headerAndValues.ContainsKey("O-SIGN-SIZE") && headerAndValues.ContainsKey("D-PAPER-SIMP-DUP"))
            {
                headerAndValues["O-SIGN-SIZE"] = $"{headerAndValues["O-SIGN-SIZE"]}-{headerAndValues["D-PAPER-SIMP-DUP"]}";
            }
            return headerAndValues;
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


        private void GetHolidayRecords()
        {
            holidayrecords.Departments = records.Departments;
            holidayrecords.Headings = records.Headings;
            holidayrecords.SignLayouts = records.SignLayouts;
            holidayrecords.SubstituteStock = records.SubstituteStock;            

            holidayrecords.DownBackInputRecords = records.DownBackInputRecords.Where(s=>s.SIGNSIZE_IN == Constants.holidaySignCode).Select(x => new ACMEHolidayRecord()
                            {
                                SignDataId =x.SignDataId,
                                DATA_NUM_IN =x.DATA_NUM_IN,
                                OS_ORDER_NUMBER = x.OrderNumber,
                                OS_QUANTITY = x.SIGN_QTY_IN,
                                SIGNHEAD_IN = x.SIGNHEAD_IN,
                                SIGNSIZE_IN = x.SIGNSIZE_IN,
                                D_Holiday = x.Holiday,
                                Store_Hours_Open = x.Store_Hours_Open,
                                Store_Hours_Close =x.Store_Hours_Close,
                                RX_Hours_Open = x.RX_Hours_Open,
                                RX_Hours_Close =x.RX_Hours_Close,
                                D_Begin_Date =x.Begin_Date,
                                D_End_Date =x.End_Date,
                                OS_STORE_NUMBER =x.StoreNo,
                                ARTWORK = x.ARTWORK
                            }).ToList();
          
        }

        #endregion

        #region IPortalDataProcessor Implementation
   
        public IEnumerable<string> Process(int clientId, int signId)
        {           
            Dictionary<string, string> headerAndValues = ProcessDownbackRule.GetDownBackFileColumns();
            records = manager.GetDownbackSigns(clientId, string.Empty, "PREVIEW", signId);
            (processDownbackRule as ProcessDownbackRule)?.ProcessOrderAndSign(records.DownBackInputRecords.FirstOrDefault(), headerAndValues);

            return headerAndValues.Values;
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
        /// Gets the output path.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        private string GetOutputPath(string action)
        {
            string OutPath = string.Empty;
            if (action.Trim().Equals("PROOF", StringComparison.InvariantCultureIgnoreCase))
            {
                OutPath = JobDefinitionProxy.GetWatchFolder(new Guid(Constants.ProofJobDefinitionKey));
            }
            else if (action.Trim().Equals("PRINT", StringComparison.InvariantCultureIgnoreCase))
            {
                OutPath = VestcomConfiguration.Config != null && VestcomConfiguration.Config.AppSettings.Settings["KameleonPath"] != null ? VestcomConfiguration.Config.AppSettings.Settings["KameleonPath"].Value : string.Empty;
            }
            return OutPath;
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

        public IEnumerable<string> Process(int clientId, int signId, int? signZonePriceId = null)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}







