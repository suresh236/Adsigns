using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vestcom.PaperSign.ACME.Entities;

namespace Vestcom.PaperSign.ACME.ProcessDownback.Rules
{
 
    /// <summary>
    ///  IBusinessRule interface
    /// </summary>
    public interface IHolidayBusinessRule
    {
        /// <summary>
        /// Executes the rules.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="inputFilePath">The input file path.</param>
        /// <param name="strBatchID">The string batch identifier.</param>
        /// <param name="Action">The action.</param>
        /// <param name="reportPath">The report path.</param>
        void ExecuteRules(int clientId, ACMEHolidayDownbackRecords records, string inputFilePath, string strBatchID, string Action, string reportPath);
    }
}
