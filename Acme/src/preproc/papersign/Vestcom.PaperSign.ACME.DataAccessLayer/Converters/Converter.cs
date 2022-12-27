using System.Collections.Generic;
using System.Data;
using Vestcom.PaperSign.ACME.Entities;

namespace Vestcom.PaperSign.ACME.DataAccessLayer.Converters
{
    public class Converter : IConverter
    {
        public void Convert(IDataReader reader, IList<Department> items)
        {
            while (reader.Read())
            {
                items.Add(new Department
                {
                    DepartmentName = System.Convert.ToString(reader["Name"]),
                    DepartmentNumber = System.Convert.ToString(reader["Code"]),
                    DepartmentId = System.Convert.ToInt32(reader["Id"]),
                    DEPARTMENT_NAME = System.Convert.ToString(reader["Name"]),
                    DEPARTMENT_NO = System.Convert.ToString(reader["Code"])
                });
            }
        }

        /// <summary>
        /// Converts the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="items">The items.</param>
        public void Convert(IDataReader reader, IList<Heading> items)
        {
            while (reader.Read())
            {
                items.Add(new Heading
                {
                    ID = System.Convert.ToInt32(reader["SignHeaderId"]),
                    HEADING_NO = System.Convert.ToString(reader["SignHeaderCode"]),
                    SignHeaderDesc = System.Convert.ToString(reader["SignHeaderDesc"]),
                    HEADING_SALE_FLAG = System.Convert.ToString(reader["SALEFlAG"]),
                    HEADING_BOGO_FLAG = System.Convert.ToString(reader["BOGOFLAG"]),
                    HEADING_PCNTOFF_FLAG = System.Convert.ToString(reader["PCNTOFFFLAG"]),
                    HEADING_10FOR10_FLAG = System.Convert.ToString(reader["FOR10FLAG"]),
                    HEADING_GROCERY_FLAG = System.Convert.ToString(reader["GROCERYFLAG"]),
                    HEADING_BAKERY_FLAG = System.Convert.ToString(reader["BAKERYFLAG"]),
                    HEADING_FRESH_MEAT_FLAG = System.Convert.ToString(reader["FRESHMEATFLAG"]),
                    HEADING_SRVC_DELI_FLAG = System.Convert.ToString(reader["SRVCDELIFLAG"]),
                    HEADING_PKGD_MEAT_FLAG = System.Convert.ToString(reader["PKGDMEATFLAG"]),
                    HEADING_PRODUCE_FLAG = System.Convert.ToString(reader["PRODUCEFLAG"]),
                    HEADING_FLORAL_FLAG = System.Convert.ToString(reader["FLORALFLAG"]),
                    HEADING_GM_FLAG = System.Convert.ToString(reader["GMFLAG"]),
                    HEADING_BEER_WINE_FLAG = System.Convert.ToString(reader["BEERWINEFLAG"]),
                    HEADING_FROZEN_FLAG = System.Convert.ToString(reader["FROZENFLAG"]),
                    HEADING_SEAFOOD_FLAG = System.Convert.ToString(reader["SEAFOODFLAG"]),
                    HEADING_SMFL_FLAG = System.Convert.ToString(reader["SMFLFLAG"]),
                    HEADING_EDLP_FLAG = System.Convert.ToString(reader["EDLPFLAG"]),
                    HEADING_OTHER_FLAG = System.Convert.ToString(reader["OTHERFLAG"]),
                    HEADING_DUPLEX_FLAG = System.Convert.ToString(reader["DUPLEXFLAG"])
                });
            }
        }

        /// <summary>
        /// Converts the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="items">The items.</param>
        public void Convert(IDataReader reader, IList<Image> items)
        {
            while (reader.Read())
            {
                items.Add(new Image
                {
                    ID = System.Convert.ToInt32(reader["Id"]),
                    IMAGE_FILE_SIGN_SIZE = System.Convert.ToString(reader["SignSize"]),
                    IMAGE_FILE_SIGN_HEADING = System.Convert.ToString(reader["SignHead"]),
                    IMAGE_FILE_DEPT_NUMBER = System.Convert.ToString(reader["DepartmentNumber"]),
                    IMAGE_FILE_IMAGE_CODE = System.Convert.ToString(reader["ImageCode"])
                });
            }
        }

        /// <summary>
        /// Converts the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="items">The items.</param>
        public void Convert(IDataReader reader, IList<PromoPGM> items)
        {
            while (reader.Read())
            {
                items.Add(new PromoPGM
                {
                    ID = System.Convert.ToInt32(reader["Id"]),
                    PROGRAM_PROMO_CODE = System.Convert.ToString(reader["PromoCode"]),
                    PROGRAM_SIGN_SIZE = System.Convert.ToString(reader["SignSize"]),
                    PROGRAM_SIGN_HEAD = System.Convert.ToString(reader["SignHead"])
                });
            }
        }

        /// <summary>
        /// Converts the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="items">The items.</param>
        public void Convert(IDataReader reader, IList<UnitPriceEntry> items)
        {
            while (reader.Read())
            {
                items.Add(new UnitPriceEntry
                {
                    ID = System.Convert.ToInt32(reader["UOMId"]),
                    UPE_KEY = System.Convert.ToString(reader["UPE-KEY"]),
                    UPE_UNIT = System.Convert.ToString(reader["UPE-UNIT"]),
                    UPE_NUMERATOR = System.Convert.ToString(reader["UPE-NUMERATOR"]),
                    UPE_DENOMINATOR = System.Convert.ToString(reader["UPE-DENOMINATOR"])
                });
            }

        }

        /// <summary>
        /// Converts the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="items">The items.</param>
        public void Convert(IDataReader reader, IList<SignLayout> items)
        {
            while (reader.Read())
            {
                items.Add(new SignLayout
                {
                    SIGN_LAYOUT_ID = System.Convert.ToInt32(reader["SignLayId"]),
                    SignSizeId = System.Convert.ToInt32(reader["SignSizeId"]),
                    SignHeaderId = System.Convert.ToInt32(reader["SignHeaderId"]),
                    SL_SIGN_SIZE = System.Convert.ToString(reader["SignLaySize"]),
                    SL_SIGN_HEADING = System.Convert.ToString(reader["SignLayHeading"]),
                    SL_LAYOUT_NO = System.Convert.ToString(reader["SignLayLayoutNo"]),
                    SL_PAPER_TYPE = System.Convert.ToString(reader["SignLayPaperType"])
                });
            }
        }

        /// <summary>
        /// Converts the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="items">The items.</param>
        public void Convert(IDataReader reader, IList<InputFile> items)
        {
            while (reader.Read())
            {
                items.Add(new InputFile
                {
                    SignDataId = Helper.ConvertStringToInteger(reader["SignDataId"].ToString()),
                    DepartmentId = Helper.ConvertStringToInteger(reader["DepartmentId"].ToString()),
                    ZonePriceId = Helper.ConvertStringToInteger(reader["ZonePriceId"].ToString()),
                    DATA_NUM_IN = Helper.ConvertStringToInteger(reader["DATA-NUM-IN"].ToString()),
                    OrderNumber = System.Convert.ToString(reader["OrderNumber"]),
                    DEPT_NAME_IN = System.Convert.ToString(reader["DEPT-NAME-IN"]),
                    FROM_DATE_IN = System.Convert.ToString(reader["FROM-DATE-IN"]),
                    END_DATE_IN = System.Convert.ToString(reader["END-DATE-IN"]),
                    BOARS_HEAD_IN = System.Convert.ToString(reader["BOARS-HEAD-IN"]),
                    PROD_BRAND_IN = System.Convert.ToString(reader["PROD-BRAND-IN"]),
                    ITEM_DESC_IN = System.Convert.ToString(reader["ITEM-DESC-IN"]),
                    SIZE_IN = System.Convert.ToString(reader["SIZE-IN"]),
                    SECOND_MSG_IN = System.Convert.ToString(reader["SECOND-MSG-IN"]),
                    COOL_IN = System.Convert.ToString(reader["COOL-IN"]),
                    BOGO_TYPE_INFO_IN = System.Convert.ToString(reader["BOGO-TYPE-INFO-IN"]),
                    PERCENT_IN = System.Convert.ToString(reader["PERCENT-IN"]),
                    Ten_FOR_10_IN = System.Convert.ToString(reader["Ten-FOR-10-IN"]),
                    UPC_IN = System.Convert.ToString(reader["UPC-IN"]),
                    DISCLAIMER_IN = System.Convert.ToString(reader["DISCLAIMER-IN"]),
                    REG_MULTI_IN = Helper.ConvertStringToInteger(reader["REG-MULTI-IN"].ToString()),
                    REG_RETAIL_IN = System.Convert.ToString(reader["REG-RETAIL-IN"]),
                    SALE_MULTI_IN = Helper.ConvertStringToInteger(reader["SALE-MULTI-IN"].ToString()),
                    SALE_PR_IN = System.Convert.ToString(reader["SALE-PR-IN"]),
                    UNIT_PR_IN = System.Convert.ToString(reader["UNIT-PR-IN"]),
                    UOM_IN = System.Convert.ToString(reader["UOM-IN"]),
                    LIMIT_IN = System.Convert.ToString(reader["LIMIT-IN"]),
                    MUST_BUY_IN = System.Convert.ToString(reader["MUST-BUY-IN"]),
                    SAVINGS_IN = System.Convert.ToString(reader["SAVINGS-IN"]),
                    IMAGE_IN = System.Convert.ToString(reader["IMAGE-IN"]).Trim(),
                    MFL_SIZE1_IN = System.Convert.ToString(reader["MFL-SIZE1-IN"]).Trim(),
                    MFL_SIZE2_IN = System.Convert.ToString(reader["MFL-SIZE2-IN"]).Trim(),
                    MFL_SIZE3_IN = System.Convert.ToString(reader["MFL-SIZE3-IN"]).Trim(),
                    MFL_SIZE6_IN = System.Convert.ToString(reader["MFL-SIZE6-IN"]).Trim(),
                    EDLP_SIZE1_IN = System.Convert.ToString(reader["EDLP-SIZE1-IN"]).Trim(),
                    EDLP_SIZE2_IN = System.Convert.ToString(reader["EDLP-SIZE2-IN"]).Trim(),
                    EDLP_SIZE3_IN = System.Convert.ToString(reader["EDLP-SIZE3-IN"]).Trim(),
                    EDLP_SIZE6_IN = System.Convert.ToString(reader["EDLP-SIZE6-IN"]).Trim(),
                    DUPLEX_IN = System.Convert.ToString(reader["DUPLEX-IN"]).Trim(),
                    OS_QUANTITY = Helper.ConvertStringToInteger(reader["SignQty"].ToString()),
                    RESTRICT_CODE = System.Convert.ToString(reader["RESTRICT-CODE"]).Trim(),
                    ORDER_ID = System.Convert.ToString(reader["ORDER-ID"]),
                    IsISM = System.Convert.ToString(reader["ISMType"]) == "ISM",
                    ImageName = System.Convert.ToString(reader["ImageName"]),
                    Side = System.Convert.ToString(reader["SignType"]),
                    LaminationType = System.Convert.ToString(reader["LaminationType"]),
                    rigidVinyl = System.Convert.ToString(reader["RigidVinyl"]),
                });
            }
        }

        /// <summary>
        /// Converts the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="items">The items.</param>
        public void Convert(IDataReader reader, IList<AcmeRecord> items)
        {
            while (reader.Read())
            {
                items.Add(new AcmeRecord
                {
                    ZonePriceId = System.Convert.ToString(reader["AdSignDataZonePriceId"]),
                    SignDataId = System.Convert.ToInt32(reader["AdSignDataId"]),
                    DATA_NUM_IN = System.Convert.ToInt32(reader["DATA-NUM-IN"]),
                    StoreNo = System.Convert.ToString(reader["OS_STORE_NUMBER"]),
                    OrderNumber = System.Convert.ToString(reader["OS_ORDER_NUMBER"]),
                    SIGN_QTY_IN = System.Convert.ToString(reader["SIGNQTY-IN"]),
                    DEPT_NAME_IN = System.Convert.ToString(reader["DEPT-NAME-IN"]),
                    FROM_DATE_IN = System.Convert.ToString(reader["FROM-DATE-IN"]),
                    END_DATE_IN = System.Convert.ToString(reader["END-DATE-IN"]),
                    ZONE_DATA_IN = System.Convert.ToString(reader["ZONE-DATA-IN"]),
                    BOARS_HEAD_IN = System.Convert.ToString(reader["BOARS-HEAD-IN"]),
                    PROD_BRAND_IN = System.Convert.ToString(reader["PROD-BRAND-IN"]),
                    ITEM_DESC_IN = System.Convert.ToString(reader["ITEM-DESC-IN"]),
                    SIZE_IN = System.Convert.ToString(reader["SIZE-IN"]),
                    SECOND_MSG_IN = System.Convert.ToString(reader["SECOND-MSG-IN"]),
                    COOL_IN = System.Convert.ToString(reader["COOL-IN"]),
                    BOGO_TYPE_INFO_IN = System.Convert.ToString(reader["BOGO-TYPE-INFO-IN"]),
                    PERCENT_IN = System.Convert.ToString(reader["PERCENT-IN"]),
                    Ten_FOR_10_IN = System.Convert.ToString(reader["Ten-FOR-10-IN"]),
                    UPC_IN = System.Convert.ToString(reader["UPC-IN"]),
                    DISCLAIMER_IN = System.Convert.ToString(reader["DISCLAIMER-IN"]),
                    REG_MULTI_IN = string.IsNullOrEmpty(System.Convert.ToString(reader["REG-MULTI-IN"])) ? 0 : System.Convert.ToInt32(reader["REG-MULTI-IN"]),
                    REG_RETAIL_IN = System.Convert.ToString(reader["REG-RETAIL-IN"]),
                    SALE_MULTI_IN = string.IsNullOrEmpty(System.Convert.ToString(reader["SALE-MULTI-IN"])) ? 0 : System.Convert.ToInt32(reader["SALE-MULTI-IN"]),
                    SALE_PR_IN = System.Convert.ToString(reader["SALE-PR-IN"]),
                    UNIT_PR_IN = System.Convert.ToString(reader["UNIT-PR-IN"]),
                    UOM_IN = System.Convert.ToString(reader["UOM-IN"]),
                    LIMIT_IN = System.Convert.ToString(reader["LIMIT-IN"]),
                    MUST_BUY_IN = System.Convert.ToString(reader["MUST-BUY-IN"]),
                    SAVINGS_IN = System.Convert.ToString(reader["SAVINGS-IN"]),
                    IMAGE_IN = System.Convert.ToString(reader["IMAGE-IN"]),
                    MFL_SIZE1_IN = System.Convert.ToString(reader["MFL-SIZE1-IN"]),
                    MFL_SIZE2_IN = System.Convert.ToString(reader["MFL-SIZE2-IN"]),
                    MFL_SIZE3_IN = System.Convert.ToString(reader["MFL-SIZE3-IN"]),
                    MFL_SIZE6_IN = System.Convert.ToString(reader["MFL-SIZE6-IN"]),
                    EDLP_SIZE1_IN = System.Convert.ToString(reader["EDLP-SIZE1-IN"]),
                    EDLP_SIZE2_IN = System.Convert.ToString(reader["EDLP-SIZE2-IN"]),
                    EDLP_SIZE3_IN = System.Convert.ToString(reader["EDLP-SIZE3-IN"]),
                    EDLP_SIZE6_IN = System.Convert.ToString(reader["EDLP-SIZE6-IN"]),
                    Extreme = System.Convert.ToString(reader["Extreme"]),
                    DUPLEX_IN = System.Convert.ToString(reader["DUPLEX-IN"]),
                    SIGNHEAD_IN = System.Convert.ToString(reader["O_SIGN_HEAD"]),
                    SIGNSIZE_IN = System.Convert.ToString(reader["O_SIGN_SIZE"]),
                    SL_SIGN_HEADING = System.Convert.ToString(reader["O_SIGN_HEAD"]),
                    SL_SIGN_SIZE = System.Convert.ToString(reader["O_SIGN_SIZE"]),
                    ARTWORK = System.Convert.ToString(reader["ARTWORK"]),
                    OS_COMMENT3 = System.Convert.ToString(reader["IMAGE-IN"]),
                    OS_COMMENT8 = System.Convert.ToString(reader["DUPLEX-IN"]),
                    OS_COMMENT9 = System.Convert.ToString(reader["COOL-IN"]),
                    D_LaminationType = System.Convert.ToString(reader["LaminationType"]),
                    OS_QUANTITY = System.Convert.ToString(reader["SIGNQTY-IN"]),
                    Holiday = System.Convert.ToString(reader["Holiday"]),
                    Store_Hours_Open = System.Convert.ToString(reader["Store_Hours_Open"]),
                    Store_Hours_Close = System.Convert.ToString(reader["Store_Hours_Close"]),
                    RX_Hours_Open = System.Convert.ToString(reader["RX_Hours_Open"]),
                    RX_Hours_Close = System.Convert.ToString(reader["RX_Hours_Close"]),
                    Begin_Date = System.Convert.ToString(reader["Begin_Date"]),
                    End_Date = System.Convert.ToString(reader["End_Date"]),


                });
            }
        }

        /// <summary>
        /// Converts the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="items">The items.</param>
        public void Convert(IDataReader reader, IList<SubstituteStock> items)
        {
            while (reader.Read())
            {
                items.Add(new SubstituteStock
                {
                    MAIN_STOCK = System.Convert.ToString(reader["MainStock"]),
                    SUBSTITUTE_STOCK = System.Convert.ToString(reader["SubstituteStock"]),
                    BG_IMG = System.Convert.ToString(reader["BGImage"]),
                    SIMPDUP = System.Convert.ToString(reader["SIMPDUP"])

                });
            }
        }


        public void Convert(IDataReader reader, IList<HolidayInput> items)
        {
            while (reader.Read())
            {
                items.Add(new HolidayInput
                {
                    SignDataId = Helper.ConvertStringToInteger(reader["SignDataId"].ToString()),
                    DATA_NUM_IN = Helper.ConvertStringToInteger(reader["DATA-NUM-IN"].ToString()),
                    OrderID = System.Convert.ToString(reader["OrderId"]),
                    Holiday = System.Convert.ToString(reader["Holiday"]),
                });
            }
        }

    }
}
