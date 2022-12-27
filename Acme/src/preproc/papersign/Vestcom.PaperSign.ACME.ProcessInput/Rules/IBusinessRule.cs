using Vestcom.PaperSign.ACME.Entities;

namespace Vestcom.PaperSign.ACME.ProcessInput.Rules
{
    /// <summary>
    /// IBusinessRule interface
    /// </summary>
    public interface IBusinessRule
    {
        /// <summary>
        /// Applies the rules.
        /// </summary>
        /// <param name="lbl">The label.</param>
        /// <param name="clientid">The clientid.</param>
        void ApplyRules(InputFile lbl, int clientid, string packagepath);

        /// <summary>
        /// Downbaks the execute rules.
        /// </summary>
        /// <param name="inputFileRecord">The input file record.</param>
        /// <param name="records">The records.</param>
        /// <returns></returns>
        InputFile DownbakExecuteRules(InputFile inputFileRecord, InputRecords records,bool IsKeHeFile);
    }
}
