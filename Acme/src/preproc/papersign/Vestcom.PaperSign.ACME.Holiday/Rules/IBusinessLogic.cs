using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vestcom.PaperSign.ACME.Entities;

namespace Vestcom.PaperSign.ACME.Holiday.Rules
{
    public interface IBusinessLogic
    {
        void ApplyRules(string filePath, string DownLoadFilePath, string reportPath);
    }
}
