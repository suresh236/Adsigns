using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Vestcom.PaperSign.ACME.Entities
{
    public class ACMEHolidayRecord : ICloneable
    {

        public string name { get; set; }

        #region for Cloning
        object ICloneable.Clone()
        {
            return this.Clone();
        }

        public ACMEHolidayRecord Clone()
        {
            return (ACMEHolidayRecord)this.MemberwiseClone();
        }
        #endregion Cloning

        public object this[string propertyName]
        {
            get
            {
                Type myType = typeof(ACMEHolidayRecord);
                PropertyInfo myPropertyInfo = myType.GetProperty(propertyName);
                return myPropertyInfo.GetValue(this, null);
            }
            set
            {
                Type myType = typeof(ACMEHolidayRecord);
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
        //
        public string D_DEPT_DESC { get; set; }


        #endregion OUTPUT HEADER


        public string MSG_MESSAGE { get; set; }
        public string MSG_CR { get; set; }
        public bool RECORD_SKIP { get; set; }

        public string SL_SIGN_SIZE { get; set; }
        public string SL_SIGN_HEADING { get; set; }
        public string SL_LAYOUT_NO { get; set; }
        public string SL_PAPER_TYPE { get; set; }
        public int DATA_NUM_IN { get; set; }
        public string  ARTWORK { get; set; }
        public int SignDataId { get; set; }
        public string SIGNHEAD_IN { get; set; }
        public string SIGNSIZE_IN { get; set; }

        public string Store_Hours_Open { get; set; }
        public string RX_Hours_Open { get; set; }
        public string Store_Hours_Close { get; set; }
        public string RX_Hours_Close { get; set; }



    }
}
