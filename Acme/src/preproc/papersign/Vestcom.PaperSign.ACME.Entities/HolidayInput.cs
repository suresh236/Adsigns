
using System;
using System.Reflection;

namespace Vestcom.PaperSign.ACME.Entities
{
    public class HolidayInput : ICloneable
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
        public string OrderNumber { get; set; }

        public string OrderID { get; set; }
        public int DATA_NUM_IN { get; set; }
        public string SIZE_IN { get; set; }
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
        public string SL_SIGN_HEADING { get; set; }
        public string SL_SIGN_SIZE { get; set; }
        public int? SignSizeId { get; set; }
        public int? SignHeaderId { get; set; }

        public string MSG_MESSAGE { get; set; }
        #endregion




    }
}
