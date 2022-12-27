using System;
using System.Reflection;

namespace Vestcom.PaperSign.ACME.Entities
{
    public class InputFile : ICloneable
    {

        public object this[string propertyName]
        {
            get
            {
                Type myType = typeof(InputFile);
                PropertyInfo myPropertyInfo = myType.GetProperty(propertyName);
                return myPropertyInfo.GetValue(this, null);
            }
            set
            {
                Type myType = typeof(InputFile);
                PropertyInfo myPropertyInfo = myType.GetProperty(propertyName);
                myPropertyInfo.SetValue(this, value, null);
            }
        }

        #region for Cloning
        object ICloneable.Clone()
        {
            return this.Clone();
        }

        public InputFile Clone()
        {
            return (InputFile)this.MemberwiseClone();
        }
        #endregion Cloning


        #region Input File Structure

        public int SignDataId { get; set; }
        public int DepartmentId { get; set; }
        public int ZonePriceId { get; set; }

        public string OrderNumber { get; set; }
        public int DATA_NUM_IN { get; set; }
        public string DEPT_NAME_IN { get; set; }
        public string FROM_DATE_IN { get; set; }
        public string END_DATE_IN { get; set; }
        public string BOARS_HEAD_IN { get; set; }
        public string PROD_BRAND_IN { get; set; }
        public string ITEM_DESC_IN { get; set; }
        public string SIZE_IN { get; set; }
        public string SECOND_MSG_IN { get; set; }
        public string COOL_IN { get; set; }
        public string BOGO_TYPE_INFO_IN { get; set; }
        public string PERCENT_IN { get; set; }
        public string Ten_FOR_10_IN { get; set; }
        public string UPC_IN { get; set; }
        public string DISCLAIMER_IN { get; set; }
        public int REG_MULTI_IN { get; set; }
        public string REG_RETAIL_IN { get; set; }
        public int SALE_MULTI_IN { get; set; }
        public string SALE_PR_IN { get; set; }
        public string UNIT_PR_IN { get; set; }
        public string UOM_IN { get; set; }
        public string LIMIT_IN { get; set; }
        public string MUST_BUY_IN { get; set; }
        public string SAVINGS_IN { get; set; }
        public string IMAGE_IN { get; set; }
        public string MFL_SIZE1_IN { get; set; }
        public string MFL_SIZE2_IN { get; set; }
        public string MFL_SIZE3_IN { get; set; }
        public string MFL_SIZE6_IN { get; set; }
        public string EDLP_SIZE1_IN { get; set; }
        public string EDLP_SIZE2_IN { get; set; }
        public string EDLP_SIZE3_IN { get; set; }
        public string EDLP_SIZE6_IN { get; set; }
        public string DUPLEX_IN { get; set; }
        public string ORDER_ID { get; set; }
        public bool RECORD_SKIP { get; set; }


        #endregion


        #region Constructor

        public InputFile()
        {
            Common = new CommonProperties();
        }

        #endregion



        #region Common Properties

        public CommonProperties Common { get; set; }

        #endregion





        public string OS_UNIT_PRICE { get; set; }
        public string O_DEPARTMENT_NO { get; set; }
        public int? SignSizeId { get; set; }
        public int? SignHeaderId { get; set; }

        public string RESTRICT_CODE { get; set; }

        #region PaperSign Downbak

        #region WorkVariables
        public string IMAGE_FILE_SIGN_SIZE { get; set; }
        public string IMAGE_FILE_DEPT_NUMBER { get; set; }
        public string IMAGE_FILE_SIGN_HEADING { get; set; }
        public int OS_QUANTITY_NUMERIC { get; set; }
        public int OS_QUANTITY { get; set; }
        public int OS_SPECIAL_FOR_NUMERIC { get; set; }
        public string DEPT_NAME_HOLD { get; set; }
        public string FROM_DATE_HOLD { get; set; }
        public string END_DATE_HOLD { get; set; }
        public string MSG_MESSAGE { get; set; }
        public string MSG_CR { get; set; }
        public string HEADING_BAKERY_FLAG { get; set; }
        public string HEADING_FLORAL_FLAG { get; set; }
        public string HEADING_GM_FLAG { get; set; }
        public string HEADING_GROCERY_FLAG { get; set; }
        public string HEADING_FRESH_MEAT_FLAG { get; set; }
        public string HEADING_PKGD_MEAT_FLAG { get; set; }
        public string HEADING_PRODUCE_FLAG { get; set; }
        public string HEADING_SRVC_DELI_FLAG { get; set; }
        public string HEADING_SEAFOOD_FLAG { get; set; }
        public string MFL_SIZE { get; set; }
        public string HEADING_SMFL_FLAG { get; set; }
        public string EDLP_SIZE { get; set; }
        public string HEADING_EDLP_FLAG { get; set; }
        public string SL_SIGN_HEADING { get; set; }
        public string SL_SIGN_SIZE { get; set; }
        public string HEADING_BOGO_FLAG { get; set; }
        public string HEADING_PCNTOFF_FLAG { get; set; }
        public string HEADING_SALE_FLAG { get; set; }
        public string HEADING_DUPLEX_FLAG { get; set; }
        public string HEADING_10FOR10_FLAG { get; set; }
        public string HEADING_BEER_WINE_FLAG { get; set; }
        public string HEADING_FROZEN_FLAG { get; set; }
        public string HEADING_OTHER_FLAG { get; set; }
        public string WORK_DATE { get; set; }
        public string WORK_DATE_MONTH { get; set; }
        public string WORK_DATE_DAY { get; set; }
        public string WORK_DATE_YEAR { get; set; }
        public string BOARS_HEAD_DAT { get; set; }
        public int LIMIT_COUNT { get; set; }
        public int BOGO_BUY_COUNT_DISPLAY { get; set; }
        public string BOGO_WORK_AREA { get; set; }
        public int BOGO_BUY_COUNT { get; set; }
        public int BOGO_GET_COUNT { get; set; }
        public string DUMMY { get; set; }
        public int BOGO_GET_COUNT_DISPLAY { get; set; }
        public string PRICE_IN { get; set; }
        public string WORK_PRICE { get; set; }
        public int SALE_MULTI_DISPLAY { get; set; }
        public string WORK_SAVINGS { get; set; }
        public string UPC_ANALYSIS1 { get; set; }
        public string UPC_ANAL5_PART1 { get; set; }
        public string UPC_ANAL5_PART2 { get; set; }
        public string UPC_ANAL3_PART1 { get; set; }
        public string UPC_ANAL3_PART2 { get; set; }
        public string UPC_ANAL3_PART3 { get; set; }
        public string UPC_ANAL6_PART1 { get; set; }
        public string UPC_ANAL6_PART2 { get; set; }
        public string UPC_ANAL6_PART3 { get; set; }
        public string UPC_ANAL6_PART4 { get; set; }
        public string UPC_ANAL1_PART1 { get; set; }
        public string UPC_ANAL1_PART2 { get; set; }
        public string UPC_ANAL1_PART3 { get; set; }
        public string UPC_ANAL7_PART1 { get; set; }
        public string UPC_ANAL7_PART2 { get; set; }
        public string UPC_ANAL7_PART3 { get; set; }
        public string UPC_ANAL7_PART4 { get; set; }
        public string UPC_ANAL8_PART1 { get; set; }
        public string UPC_ANAL8_PART2 { get; set; }
        public string UPC_ANAL4_DASH1 { get; set; }
        public string UPC_ANAL4_DASH2 { get; set; }
        public string UPC_ANAL4_PART1 { get; set; }
        public string UPC_ANAL4_PART2 { get; set; }
        public string UPC_ANAL4_PART3 { get; set; }
        public string UPC_ANAL2_PART1 { get; set; }
        public string UPC_ANAL2_PART2 { get; set; }
        public string UPC_ANAL2_PART3 { get; set; }
        public string UPC_ANAL2_MIDDLE { get; set; }
        public string WORK_DOLLARS { get; set; }
        public string WORK_CENTS { get; set; }
        public int SIGN_NUMBER { get; set; }
        public string WORK_WH_NUMBER { get; set; }
        public string UP_FIELD1 { get; set; }
        public string UP_FIELD2 { get; set; }
        public string UP_FIELD3 { get; set; }
        public string UP_FIELD4 { get; set; }
        public int DASH_COUNT { get; set; }
        public string WORK_FIELD { get; set; }
        public string UP_CODE_IN { get; set; }
        public string UP_TYPE { get; set; }
        public string SL_PAPER_TYPE { get; set; }
        public string SL_LAYOUT_NO { get; set; }
        public string HOLD_SL_SIGN_SIZE { get; set; }
        public string HOLD_SL_SIGN_HEADING { get; set; }
        public string HOLD_SL_PAPER_TYPE { get; set; }
        public string HOLD_SL_LAYOUT_NO { get; set; }

        #endregion WorkVariables

        #region UNITPRIC.CBL
        public string WORK_UNITS_PART1 { get; set; }
        public string WORK_UNITS_PART2 { get; set; }
        public int WORK_UNITS_PART2_COUNT { get; set; }
        public int DECIMAL_POINT_COUNT { get; set; }
        public int ASTERISK_COUNT { get; set; }
        public string PACK_QTY { get; set; }
        public string PS_SIZE { get; set; }
        public string TEMP_SIZE { get; set; }
        public string WORK_UNITS_COMPUTATIONAL { get; set; }
        public string WORK_UNITS { get; set; }
        public string SPECIAL_PRICE_COMPUTATIONAL { get; set; }
        public string UNIT_PRICE_COMPUTATIONAL { get; set; }
        public string RE_SEARCH_FLAG { get; set; }
        public string PS_KEY { get; set; }
        public string WK_UNIT_PRICE { get; set; }
        public string UNIT_PRICE_DISPLAY_DOLLARS { get; set; }
        public string DISPLAY_UNITS { get; set; }
        public string UNIT_PRICE_DISPLAY_CENTS { get; set; }
        public string WORK_STRING { get; set; }
        public string UPE_DENOMINATOR { get; set; }
        public string UPE_NUMERATOR { get; set; }
        public string UPE_UNIT { get; set; }
        public string PS_TYPE_CHAR2 { get; set; }
        public string PACKAGE_SIZE_DATA { get; set; }
        public string PS_UNITS { get; set; }
        public string PS_UNITS2 { get; set; }
        public string COMPUTE_UNIT_PRICE_FLAG { get; set; }
        public string PS_TYPE_CHAR1 { get; set; }
        public string PASS_LOWER { get; set; }
        public int PASS_LEN { get; set; }
        public string S_I_REDISPLAY { get; set; }
        public int S_I_DISP_FLD { get; set; }
        public int S_I_INIT_FIELD { get; set; }
        public string D_BACKGROUND_IMAGE { get; set; }
        public int RowNum { get; set; }


        #endregion

        #endregion

        public bool IsISM { get; set; }
        public string ImageName { get; set; }
        public string Side { get; set; }
        public string LaminationType { get; set; }
        public string rigidVinyl { get; set; }

    }
}
