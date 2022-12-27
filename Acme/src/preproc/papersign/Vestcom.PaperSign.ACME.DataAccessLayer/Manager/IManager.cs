using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Vestcom.PaperSign.ACME.Entities;

namespace Vestcom.PaperSign.ACME.DataAccessLayer.Manager
{
    /// <summary>
    /// IManager interface
    /// </summary>
    public interface IManager
    {
        /// <summary>
        /// Gets the acme signs.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <returns></returns>
        InputRecords GetAcmeSigns(int clientId, string orderId);
        /// <summary>
        /// Gets the downback signs.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="BatchNumber">The batch number.</param>
        /// <param name="Action">The action.</param>
        /// <returns></returns>
        ACMEDownbackRecords GetDownbackSigns(int clientId, string BatchNumber, string Action, int signId);
        /// <summary>
        /// Saves the input.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="exceptions">The exceptions.</param>
        /// <returns></returns>
        bool SaveInput(DataTable dt,IList<ExceptionReport> exceptions);
        /// <summary>
        /// Updates the department identifier.
        /// </summary>
        /// <param name="orderNumber">The order number.</param>
        /// <returns></returns>
        bool UpdateDepartmentId(string orderNumber);

        /// <summary>
        /// Updates the type of the tag.
        /// </summary>
        /// <param name="records">The records.</param>
        void UpdateTagType(ACMEDownbackRecords records);

        /// <summary>
        /// Update FileLog
        /// </summary>
        /// <param name="fileLog"></param>
        /// <returns></returns>
        bool UpdateFileLog(FileLog fileLog);

        bool AddFileLogData(int ClientId, string FileName, string FilePath, int SubClientId, out int FileLogKey);
        bool CreateOrder(int ClientId, string FileName, out string OrderNumber);
        bool ProcessData(string OrderNumber, bool IsISM, bool IsHoliday = false, bool IsKeHeFile = false);
        bool SaveRawData(DataTable dt, bool IsISM, bool IsHoliday=false);

        bool SaveISMRawData(DataTable dt);

        HolidayInputRecords GetAcmeHolidaySigns(int clientId, string orderId);

        bool SaveHolidayInput(List<HolidayInputTypes> holidayinputtypes, IList<ExceptionReport> exceptions);
        void UpdateTagType(ACMEHolidayDownbackRecords records);

    }
}
