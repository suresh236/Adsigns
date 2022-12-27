using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Vestcom.AuditData;
using Vestcom.Core.Utilities;
using Vestcom.PaperSign.ACME.DataAccessLayer.Manager;
using Vestcom.PaperSign.ACME.Holiday.Entities;
using Vestcom.PaperSign.ACME.Holiday.Records;
using Vestcom.PaperSign.ACME.Holiday.Rules;
using Vestcom.PreProcSupport;

namespace Vestcom.PaperSign.ACME.Holiday
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Vestcom.PreProcSupport.IRunnableExtended" />
    [Preprocessor(Constants.CustomerId, Constants.PreprocessorKey, "None")]
    public class AcmeHoliday : IRunnableExtended
    {
        #region Protected Members
        protected readonly IManager manager;
        protected readonly IBusinessLogic processHolidayBusinessRule;
        Vestcom.Connection.RealmTypes currentRealmType;
        IProcessing RecordAuditor;
        bool IsKeepLocal;
        public static bool IsFakeAuditData;
        string AuditDataEmailAddresses;
        string filePath = string.Empty;
        string originalFileName = string.Empty;
        #endregion

        #region Pubic Members
        public static IEnumerable<SignHeading> SignHeadingFillers = null;
        public static IEnumerable<SignLayout> SignLayFillers = null;
        public static IEnumerable<SubstituteStock> SubstituteStockFillers = null;

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessDownback"/> class.
        /// </summary>
        public AcmeHoliday()
        {
            new Vestcom.Connection.RegisteredCustomerContext(Constants.CustomerId);
            processHolidayBusinessRule = new BusinessLogic();
        }
        #endregion

        #region BASE CLASS METHODS

        /// <summary>
        /// 
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
                FillLookupData();
                GenerateTempPipeSepertorFile(settings);

                string fileName = Path.GetFileNameWithoutExtension(originalFileName) + "_" +
                                            Convert.ToString(DateTime.Now.ToString("yyyyMMddTHHmmss")) + "=ACMEHolidaySigns";
                string packagePath = settings.PackagePath;
                string strFilePath = Path.Combine(packagePath, fileName);

                string reportPath = Path.Combine(packagePath, Constants.ReportFileFolder);
                if (!Directory.Exists(reportPath))
                {
                    Directory.CreateDirectory(reportPath);
                }

                processHolidayBusinessRule.ApplyRules(filePath, strFilePath, reportPath);

                if (!IsKeepLocal)
                {
                    string outputFolderPath = GetOutputFolderPath();
                    File.Copy(strFilePath, Path.Combine(outputFolderPath, fileName));
                }

            }
            catch (Exception ex)
            {
                response.ResponseState = ResponseType.Failure;
                response.Exceptions.Add(ex);
                if (RecordAuditor != null)
                {
                    RecordAuditor.SaveAuditData(GetProgramName(), GetProgramVersion(), "Acme Holiday Signs", Processing.RunStatus.WithErrors, true, AuditDataEmailAddresses, runTimeException: ex);
                }
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

        /// <summary>
        /// Sets the audit data email addresses.
        /// </summary>
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

        /// <summary>
        /// Generating Temporary Pipe Separator File
        /// </summary>
        /// <param name="settings"></param>
        public void GenerateTempPipeSepertorFile(RunSettings settings)
        {
            string packagePath = settings.PackagePath;
            List<string> files = settings.Files;
            for (int i = 0; i < files.Count; i++)
            {
                filePath = Path.Combine(packagePath, files[i]);
                originalFileName = files[i];
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the name of the program.
        /// </summary>
        /// <returns></returns>
        private string GetProgramName()
        {
            return Assembly.GetExecutingAssembly().FullName;
        }

        /// <summary>
        /// Gets the program version.
        /// </summary>
        /// <returns></returns>
        private string GetProgramVersion()
        {
            return System.Windows.Forms.Application.ProductVersion.ToString();
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
        /// Fill Collection with respect to DB Connection
        /// </summary>
        private void FillLookupData()
        {
                SignHeadingFillers = SignHeadingFiller.AddSignHeadingFiller();
                SignLayFillers = SignLayoutFiller.AddSignLayFiller();
                SubstituteStockFillers = SubstituteStockFiller.AddSubstituteStockFiller();
        }

        /// <summary>
        /// Gets the Output Folder Path
        /// </summary>
        /// <returns></returns>
        private string GetOutputFolderPath()
        {
            if (currentRealmType == Vestcom.Connection.RealmTypes.Test)
            {
                return Vestcom.PreProcSupport.JobDefinitionProxy.GetWatchFolder(new Guid(Constants.TestJobDefinitionKey)).ToString();
            }
            else
            {
                return Vestcom.PreProcSupport.JobDefinitionProxy.GetWatchFolder(new Guid(Constants.ProdJobDefinitionKey)).ToString();
            }
        }


        #endregion
    }

}


