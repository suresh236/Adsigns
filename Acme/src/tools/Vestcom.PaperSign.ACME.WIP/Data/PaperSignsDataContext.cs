using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vestcom.Core.Utilities;
using Vestcom.PaperSign.ACME.DataAccessLayer.Manager;

namespace Vestcom.PaperSign.ACME.WIP.Data
{
    class PaperSignsDataContext : PaperSignsDataContextDataContext
    {
        public const int CustomerId = 142;
        protected readonly IManager manager;

        public PaperSignsDataContext()
        {
            Connection.ConnectionString = ConfigurationManager.AppSettings["SqlDatabaseConnection"];
            VestcomConfiguration.LoadConfiguration();
            new Connection.RegisteredCustomerContext(CustomerId);
            manager = new Manager();
        }

        public Dictionary<string, string> GetACMERecord(int clientId, int signId)
        {
            Sign sign = Signs.FirstOrDefault(x => x.Id == signId);

            if (sign == null)
            {
                return null;
            }

            return new Dictionary<string, string>
            {
                {"DEPT_NAME_IN", sign.DepartmentId.ToString()}, //May not need this or need to go about it differently
                {"SIZE_IN", sign.AdSizeDescription},
                {"ITEM_DESC_IN", sign.AdBlowLine},
                {"PROD_BRAND_IN", sign.AdOverLine},
                {"SECOND_MSG_IN", sign.SubDescription},
                {"COOL_IN", sign.SplInstruction},
                {"BOGO_TYPE_INFO_IN", sign.WeightedItem},
                {"PERCENT_IN", sign.NuvalScore},
                {"DISCLAIMER_IN", sign.Attribute1},
                {"IMAGE_IN", sign.Attribute2},
                {"MFL_SIZE1_IN", sign.Attribute3},
                {"MFL_SIZE2_IN", sign.Attribute4},
                {"MFL_SIZE3_IN", sign.Attribute5},
                {"MFL_SIZE6_IN", sign.Attribute6},
                {"SIGNHEAD_IN", sign.SignHeaderId.ToString()},
                {"OS_QUANTITY", sign.Attribute20},
                {"Begin_Date", sign.EffectiveDate.ToString()},
                {"End_Date", sign.ExpiryDate.ToString()},
                {"Extreme", sign.Format},
                {"ZONE_DATA_IN", sign.SectionId}
            };
        }
    }
}
