using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Vestcom.PaperSign.ACME.Entities
{
    public class AcmeRecord : ICloneable
    {

        public string name { get; set; }

        #region for Cloning
        object ICloneable.Clone()
        {
            return this.Clone();
        }

        public AcmeRecord Clone()
        {
            return (AcmeRecord)this.MemberwiseClone();
        }
        #endregion Cloning

        public object this[string propertyName]
        {
            get
            {
                Type myType = typeof(AcmeRecord);
                PropertyInfo myPropertyInfo = myType.GetProperty(propertyName);
                return myPropertyInfo.GetValue(this, null);
            }
            set
            {
                Type myType = typeof(AcmeRecord);
                PropertyInfo myPropertyInfo = myType.GetProperty(propertyName);
                myPropertyInfo.SetValue(this, value, null);
            }
        }

        #region OUTPUT HEADER
        //OrderSigns 
        [Header("OS-KEY")]
        public string OS_KEY { get; set; }

        [Header("OS-ORDER-NUMBER")]
        public string OS_ORDER_NUMBER { get; set; }

        [Header("OS-ZONES")]
        public string OS_ZONES { get; set; }

        [Header("OS-STORES")]
        public string OS_STORE_NUMBER { get; set; }

        [Header("OS-PULL-DATE")]
        public string OS_PULL_DATE { get; set; }

        [Header("OS-REGULAR-PRICE-FOR")]
        public string OS_REGULAR_PRICE_FOR { get; set; }

        [Header("OS-QUANTITY")]
        public string OS_QUANTITY { get; set; }

        [Header("OS-BP-COMMENT")]
        public string OS_BP_COMMENT { get; set; }

        [Header("OS-BRAND-NAME")]
        public string OS_BRAND_NAME { get; set; }

        [Header("OS-PRODUCT-NAME")]
        public string OS_PRODUCT_NAME { get; set; }

        [Header("OS-EMPHASIS")]
        public string OS_EMPHASIS { get; set; }

        [Header("OS-BRAND-NAME2")]
        public string OS_BRAND_NAME2 { get; set; }

        [Header("OS-PACKAGE-TYPE")]
        public string OS_PACKAGE_TYPE { get; set; }

        [Header("OS-PACKAGE-SIZE")]
        public string OS_PACKAGE_SIZE { get; set; }

        [Header("OS-SPECIAL-PRICE")]
        public string OS_SPECIAL_PRICE { get; set; }

        [Header("OS-SPECIAL-UNITS")]
        public string OS_SPECIAL_UNITS { get; set; }

        [Header("OS-SPECIAL-FOR")]
        public string OS_SPECIAL_FOR { get; set; }

        [Header("OS-SAVINGS")]
        public string OS_SAVINGS { get; set; }

        [Header("OS-UNIT-PRICE")]
        public string OS_UNIT_PRICE { get; set; }

        [Header("OS-COMMENT1")]
        public string OS_COMMENT1 { get; set; }

        [Header("OS-COMMENT2")]
        public string OS_COMMENT2 { get; set; }

        [Header("OS-COMMENT3")]
        public string OS_COMMENT3 { get; set; }

        [Header("OS-COMMENT4")]
        public string OS_COMMENT4 { get; set; }

        [Header("OS-COMMENT5")]
        public string OS_COMMENT5 { get; set; }

        [Header("OS-COMMENT6")]
        public string OS_COMMENT6 { get; set; }

        [Header("OS-COMMENT7")]
        public string OS_COMMENT7 { get; set; }

        [Header("OS-COMMENT8")]
        public string OS_COMMENT8 { get; set; }

        [Header("OS-COMMENT9")]
        public string OS_COMMENT9 { get; set; }

        [Header("OS-COMMENT10")]
        public string OS_COMMENT10 { get; set; }

        [Header(" RESTRICT-CODE")]
        public string RESTRICT_CODE { get; set; }

        [Header("UPC_CODE")]
        public string UPC_CODE { get; set; }

        [Header("OS-CATEGORY")]
        public string OS_CATEGORY { get; set; }

        //ORDERS
        [Header("O-REQUEST-NUMBER")]
        public string O_REQUEST_NUMBER { get; set; }

        [Header("O-SIGN-SIZE")]
        public string O_SIGN_SIZE { get; set; }

        [Header("O-SIGN-HEAD")]
        public string O_SIGN_HEAD { get; set; }

        [Header("O-DEPARTMENT-NO")]
        public string O_DEPARTMENT_NO { get; set; }

        [Header("O-PAPER-TYPE")]
        public string O_PAPER_TYPE { get; set; }

        [Header("O-LAYOUT-NO")]
        public string O_LAYOUT_NO { get; set; }

        [Header("O-IMAGE-CODE")]
        public string O_IMAGE_CODE { get; set; }

        [Header("O-DATE-REQUIRED")]
        public string O_DATE_REQUIRED { get; set; }

        [Header("O-DATE-ORDERED")]
        public string O_DATE_ORDERED { get; set; }

        [Header("O-JOB-NUMBER")]
        public string O_JOB_NUMBER { get; set; }

        [Header("D-Vestcom-Tag-Type")]
        public string D_Vestcom_Tag_Type { get; set; }

        [Header("D-UPC-NUMBER")]
        public string D_UPC_NUMBER { get; set; }

        [Header("D-BACKGROUND-IMAGE")]
        public string D_BACKGROUND_IMAGE { get; set; }

        [Header("D-DEPT-DESC")]
        public string D_DEPT_DESC { get; set; }

        [Header("D-PAPER-SIMP-DUP")]
        public string D_PAPER_SIMP_DUP { get; set; }

        [Header("D-PROMO-STARTDATE")]
        public string D_PROMO_STARTDATE { get; set; }

        [Header("D-LaminationType")]
        public string D_LaminationType { get; set; }

        [Header("D-RigidVinyl")]
        public string D_RigidVinyl { get; set; }
    #endregion



    #region Input
    public string SIGNSIZE_IN { get; set; }
        public string SIGNHEAD_IN { get; set; }
        public string ZonePriceId { get; set; }
        public string StoreNo { get; set; }
        public int SignDataId { get; set; }
        public string OrderNumber { get; set; }
        public string SIGN_QTY_IN { get; set; }
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

        public string Extreme { get; set; }
        #endregion Input

        #region WorkVariables
        public bool RECORD_SKIP = false;
        public string IMAGE_FILE_SIGN_SIZE { get; set; }
        public string IMAGE_FILE_DEPT_NUMBER { get; set; }
        public string IMAGE_FILE_SIGN_HEADING { get; set; }
        public int OS_SPECIAL_FOR_NUMERIC { get; set; }
        public string DEPT_NAME_HOLD { get; set; }
        public string FROM_DATE_HOLD { get; set; }
        public string END_DATE_HOLD { get; set; }
        public string MSG_MESSAGE { get; set; }
        public string MSG_CR { get; set; }
        public string PASS_WORK_AREA { get; set; }
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
        public string ZONE_DATA_IN { get; set; }
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
        public string WORK_ORD_NUMBER { get; set; }
        public string PASS_BATCH_DATE { get; set; }
        public string PASS_JOB_NUMBER { get; set; }
        public string UP_FIELD1 { get; set; }
        public string UP_FIELD2 { get; set; }
        public string UP_FIELD3 { get; set; }
        public string UP_FIELD4 { get; set; }
        public int DASH_COUNT { get; set; }
        public string WORK_FIELD { get; set; }
        public string UP_CODE_IN { get; set; }
        public string UP_TYPE { get; set; }
        public string PASS_PRICE { get; set; }
        public string PASS_FOR { get; set; }
        public string SL_PAPER_TYPE { get; set; }
        public string SL_LAYOUT_NO { get; set; }

        public string HOLD_SL_SIGN_SIZE { get; set; }
        public string HOLD_SL_SIGN_HEADING { get; set; }
        public string HOLD_SL_PAPER_TYPE { get; set; }
        public string HOLD_SL_LAYOUT_NO { get; set; }
        public string ARTWORK { get; set; }

        #endregion WorkVariables


        #region Holiday
        public string Holiday { get; set; }
        public string Store_Hours { get; set; }
        public string RX_Hours { get; set; }
        public string Begin_Date { get; set; }
        public string End_Date { get; set; }

        public string Store_Hours_Open { get; set; }
        public string RX_Hours_Open { get; set; }
        public string Store_Hours_Close { get; set; }
        public string RX_Hours_Close { get; set; }

        #endregion Holiday


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
        public string S_I_REDISPLAY { get; set; }
        public int S_I_DISP_FLD { get; set; }
        public int S_I_INIT_FIELD { get; set; }


        #endregion
    }
}
