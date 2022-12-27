using Vestcom.Core.Utilities;

namespace Vestcom.PaperSign.ACME.ProcessInput
{
    /// <summary>
    /// Class Which Define Constant Values
    /// </summary>
    public static class Constants
    {
        public const int ClientId = 2;
        public const int CustomerId = 142;
        public const string PreprocessorKey = "71F9C65B-CACA-4B69-B0C6-2FF00D0D3AB0";

        public const int Accent_E_AsciiValue = 232;
        public const string EValue = "e";

        public const string Space = " ";
        public const string DoubleQuote = "\"";
        public const string SpaceAnddoubleQuote = " \"";
        public const string doubleQuoteAndSpace = "\" ";

        public const string pipeSeparator = "|";
        public const string Y = "Y";
        public const string N = "N";
        public const string B = "B";
        public const string RejectFileName = "Reject.txt";

        public const string holidaySignCode = "0020";

        public const string Dot = ".";
        public const string BAKERY = "BAKERY";
        public const string FLORAL = "FLORAL";
        public const string G_M_AND_H_B_C = "G M & H B C";
        public const string GROCERY = "GROCERY";
        public const string GR = "GR";
        public const string MEAT_BUTCHER_BLOCK = "MEAT & BUTCHER BLOCK";
        public const string MEAT_DELI = "MEAT DELI";
        public const string PRODUCE = "PRODUCE";
        public const string SERVICE_DELI = "SERVICE DELI";
        public const string Grocery_KeHe = "GROCERY-KEHE";
        public const string SEAFOOD = "SEAFOOD";
        public const string Monopoly = "Monopoly";
        public static readonly string[] ChkList_SL_SIGN_HEADING = new string[] { "0101", "0102", "0103", "0107", "0151", "0152", "0153", "0157" };
        public static readonly string[] ChkList_Duplex_SL_SIGN_SIZE = new string[] { "0001", "0002", "0003", "0006", "0151", "0152", "0153", "0156" };
        public static readonly string[] ChkList_PS_TYPE_CHAR2 = new string[] { "L", "D", "C", "F", "E", "A", "P", "G" };
        public const string _0002 = "0002";
        public const string _0001 = "0001";
        public const string _0003 = "0003";
        public const string _0006 = "0006";
        public const string OS_ZONES = "z 2 3 4 5 6 7 8 9 86 87 88 89";
        public const string ZwithSpace = "z ";
        public const string _2withSpace = "2 ";
        public const string _3withSpace = "3 ";
        public const string _4withSpace = "4 ";
        public const string _5withSpace = "5 ";
        public const string _6withSpace = "6 ";
        public const string _7withSpace = "7 ";
        public const string _8withSpace = "8 ";
        public const string _9withSpace = "9 ";
        public const string _86withSpace = "86 ";
        public const string _87withSpace = "87 ";
        public const string _88withSpace = "88 ";
        public const string _89withSpace = "89 ";

        public const string Coke = "Coke";
        public const string Pepsi = "Pepsi";
        public const string Mtn_Dew = "Mtn Dew";
        public const string Lancaster_Brand_Beef = "Lancaster Brand Beef";
        public const string Hatfield_Simply_Tender = "Hatfield Simply Tender";
        public const string MONOPOLY = "MONOPOLY";
        public const string AA = "AA";
        public const string Limit = "Limit";
        public const string Must_buy = "Must buy";
        public const string BOGO = "BOGO";
        public const string Buy1Get1 = "Buy 1 Get 1";
        public const string FREE = "FREE";
        public const string When_you_buy = "When you buy ";
        public const bool IsDBConnection = true;
        public const string ProdAuditDataEmailAddresses = "amishra@vestcom.com,cjones@vestcom.com,zjafri@vestcom.com,tanveer.siddiqui@globallogic.com,pparmar@vestcom.com";
        public const string TestAuditDataEmailAddresses = "amishra@vestcom.com,zjafri@vestcom.com,tanveer.siddiqui@globallogic.com,pparmar@vestcom.com";
        public const string IMAGEFILEEXTENSION = ".eps";

        public static string IMAGEFOLDERPATH = VestcomConfiguration.Config != null && VestcomConfiguration.Config.AppSettings.Settings["ImageFolderPath"] != null ? VestcomConfiguration.Config.AppSettings.Settings["ImageFolderPath"].Value : string.Empty;
        public struct ISMErrorMessages
        {
            public const string NoValidDataOrFormatException = "Uploaded File Layout is not matching existing file layout of Acme Adsigns Import. Please upload the correct file format with acceptable data layout. File needs to be a valid XLS file having one sheet of 45 columns.";
            public const string NoDataFoundException = "No data found in data sheet. Please upload valid xls file having one sheet of 45 columns with valid data.";
            public const string NoValidExtension = "File format is not correct. File need to a valid XLS file having one sheet of 45 columns.";

        }
    }
}
