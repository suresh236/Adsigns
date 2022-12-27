using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vestcom.PaperSign.ACME.Holiday
{
    public class Constants
    {
        public const int CustomerId = 142;
        public const string PreprocessorKey = "F3BA268E-B2F9-4163-ACBD-D7302473D5BC";
        public const string pipeSeparator = "|";
        public const string ReportFileFolder = "Report";

        public const string ImageFileExtension = ".eps";
        public const int Max_Path = 260;
        public const string holidaySignCode = "0020";

        public const string ProdAuditDataEmailAddresses = "tsiddiqui@vestcom.com,cjones@vestcom.com,zjafri@vestcom.com,IShafer@vestcom.com,rhaygood@vestcom.com";
        public const string TestAuditDataEmailAddresses = "tsiddiqui@vestcom.com,zjafri@vestcom.com,IShafer@vestcom.com,rhaygood@vestcom.com";

        public static string ImageFolderpath = @"\\websvc-lrar-04\PaperSigns\InBoundImages\ACME";
        public static string TestJobDefinitionKey = "e6a097e6-8208-4f33-9bb4-5f39cdc835a9";
        public static string ProdJobDefinitionKey = "14006e12-f98d-4729-8da9-0b56975290be";

    }
}
