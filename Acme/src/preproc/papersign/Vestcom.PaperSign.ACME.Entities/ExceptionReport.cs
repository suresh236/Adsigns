using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vestcom.PaperSign.ACME.Entities
{
    public class ExceptionReport
    {
        [Header("OrderId")]
        public int OrderId { get; set; }
        [Header("SignId")]
        public int SignId { get; set; }
        [Header("ExceptionStatus")]
        public string ExceptionStatus { get; set; }
        [Header("ExceptionDescription")]
        public string ExceptionDescription { get; set; }
        [Header("FieldName")]
        public string FieldName { get; set; }
        [Header("RowNum")]
        public int RowNum { get; set; }
    }
}
