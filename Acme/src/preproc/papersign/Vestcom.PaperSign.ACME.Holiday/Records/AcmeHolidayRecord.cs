using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Vestcom.Core.Database.Entities;

namespace Vestcom.PaperSign.ACME.Holiday.Records
{
    public class AcmeHolidayRecord : ICloneable
    {
        #region for Cloning
        object ICloneable.Clone()
        {
            return this.Clone();
        }

        public AcmeHolidayRecord Clone()
        {
            return (AcmeHolidayRecord)this.MemberwiseClone();
        }

        #endregion Cloning
        public object this[string propertyName]
        {
            get
            {
                Type myType = typeof(AcmeHolidayRecord);
                PropertyInfo myPropertyInfo = myType.GetProperty(propertyName);
                return myPropertyInfo.GetValue(this, null);
            }
            set
            {
                Type myType = typeof(AcmeHolidayRecord);
                PropertyInfo myPropertyInfo = myType.GetProperty(propertyName);
                myPropertyInfo.SetValue(this, value, null);
            }
        }

        #region Input Fields
        public string SIGNSIZE_IN { get; set; }
        public string SIGNHEAD_IN { get; set; }
        public int SignDataId { get; set; }
        public string OrderNumber { get; set; }
        public string SIGN_QTY_IN { get; set; }
        public int DATA_NUM_IN { get; set; }

        #endregion

        #region OUTPUT HEADER
        //OrderSigns 
        [Header("OS-KEY")]
        public string OS_KEY { get; set; }

        [Header("OS-ORDER-NUMBER")]
        public string OS_ORDER_NUMBER { get; set; }

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

        [Header("OS-STORES")]
        public string OS_STORE_NUMBER { get; set; }

        [Header("OS-QUANTITY")]
        public string OS_QUANTITY { get; set; }

        [Header("D-Holiday")]
        public string D_Holiday { get; set; }

        [Header("D-Store-Hours")]
        public string D_Store_Hours { get; set; }

        [Header("D-RX-Hours")]
        public string D_RX_Hours { get; set; }

        [Header("D-Begin-Date")]
        public string D_Begin_Date { get; set; }

        [Header("D-End-Date")]
        public string D_End_Date { get; set; }

        [Header("D-Vestcom-Tag-Type")]
        public string D_Vestcom_Tag_Type { get; set; }

        [Header("D-BACKGROUND-IMAGE")]
        public string D_BACKGROUND_IMAGE { get; set; }

        [Header("D-RX-Image-Call")]
        public string D_RX_Image_Call { get; set; }

        #endregion OUTPUT HEADER

        #region WorkVariables

        public string SL_SIGN_HEADING { get; set; }
        public string SL_SIGN_SIZE { get; set; }
        public string SL_PAPER_TYPE { get; set; }
        public string SL_LAYOUT_NO { get; set; }
        public string ARTWORK { get; set; }
        public string MSG_MESSAGE { get; set; }
        public bool RECORD_SKIP { get; set; }
        public string Store_Hours_Open { get; set; }
        public string RX_Hours_Open { get; set; }
        public string Store_Hours_Close { get; set; }
        public string RX_Hours_Close { get; set; }

        #endregion WorkVariables

        #region "adSignAcmeHoliday_Raw"
        public string Store_Num { get; set; }
        public string Description { get; set; }
        public string District { get; set; }
        public string Store_Phone_Number { get; set; }
        public string Pharmacy_Phone { get; set; }
        public string Holiday { get; set; }
        public string Begin_Date { get; set; }
        public string End_Date { get; set; }
        public string Store_Open { get; set; }
        public string Store_Close { get; set; }
        public string RX_Open { get; set; }
        public string RX_Close { get; set; }
        public string RegularStore_Hours_Mon_Fri_Open { get; set; }
        public string RegularStore_Hours_Mon_Fri_Close { get; set; }
        public string RegularStore_Hours_Sat_Open { get; set; }
        public string RegularStore_Hours_Sat_Close { get; set; }
        public string RegularStore_Hours_Sun_Open { get; set; }
        public string RegularStore_Hours_Sun_Close { get; set; }
        public string RegularPharmacy_Hours_Mon_Fri_Open { get; set; }
        public string RegularPharmacy_Hours_Mon_Fri_Close { get; set; }
        public string RegularPharmacy_Hours_Sat_Open { get; set; }
        public string RegularPharmacy_Hours_Sat_Close { get; set; }
        public string RegularPharmacy_Hours_Sun_Open { get; set; }
        public string RegularPharmacy_Hours_Sun_Close { get; set; }
        public string Division { get; set; }
        #endregion

    }
}

