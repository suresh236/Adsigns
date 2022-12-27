using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vestcom.PaperSign.ACME.DataAccessLayer
{
    public class Constants  
    {
        public const string storedProc = "usp_GetACMEDownBackFileData";

        public struct GetAcmeInput
        {
            public const string Name = "usp_GetACMEInputSigns";
            public struct Columns
            {
                public const string ClientId = "@ClientId";
                public const string OrderId = "@OrderId";
            }
        }

        public struct GetAcmeHolidayInput
        {
            public const string Name = "usp_GetACMEHolidayInputSigns";
            public struct Columns
            {
                public const string ClientId = "@ClientId";
                public const string OrderId = "@OrderId";
            }
        }
        
        public struct GetAcmeDownBackInput
        {
            public const string Name = "usp_GetACMEDownBackFileData";
            public struct Columns
            {
                public const string ClientId = "@ClientId";
                public const string BatchNumber = "@BatchNumber";
                public const string Action = "@Action";
                public const string SignId = "@SignId";
            }
        }

        public struct AcmeSaveInputStoredProcedure
        {
            public const string Name = "usp_SaveACMEInput";
            public const string HolidayName = "usp_SaveACMEHolidayInput";
            public struct Columns
            {
                public const string ACMEInputTypes = "@ACMEInputTypes";
                public const string KVATExceptions = "@GlobalExceptionTypes";
                public const string ACMEHolidayInputTypes = "@ACMEHolidayInputTypes";
            }

        }

        public struct UpdateFileLogStatus
        {
            public const string Name = "usp_UpdateFileLogStatusPreProc";
            public struct Columns
            {
                public const string FileLogKey = "@FileLogKey";
                public const string Status = "@Status";
                public const string ErrorMesssage = "ErrorMesssage";
                public const string OrderNumber = "@OrderNumber";
            }
        }
        public struct AddFileLogData
        {
            public const string Name = "usp_AddFileLogDataPreProc";
            public struct Columns
            {
                public const string ClientId = "@ClientId";
                public const string FileName = "@FileName";
                public const string FilePath = "@FilePath";
                public const string SubClientId = "@SubClientId";
                public const string FileLogKey = "@FileLogKey";
            }
        }
        public struct CreateOrder
        {
            public const string Name = "usp_CreateOrder";
            public struct Columns
            {
                public const string ClientId = "@ClientId";
                public const string fileName = "@FileName";
                public const string WorkOrder = "@WorkOrder";
            }
        }
        public struct ProcessData
        {
            public const string ISMProcessSPName = "usp_ProcessACMEISMOrder";
            public const string AcmeProcessSPName = "usp_ProcessAcmeOrder";
            public const string HolidayProcessSPName = "usp_ProcessAcmeHolidayOrder";
            public const string AcmeKeHeProcessSPName = "usp_ProcessAcmeKeHeOrder";
            public struct Columns
            {
                public const string OrderNumber = "@OrderNumber";
            }
        }
        public const string AcmeRawTable = "adSignAcme_Raw";
        public const string ISMRawTable = "adSignAcmeISM_Raw";
        public const string HolidayRawTable = "adSignAcmeHoliday_Raw";
    }
}
