using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vestcom.PaperSign.ACME.Entities
{
   public class HolidayInputTypes
    {
        [Header("Id")]
        public int Id { get; set; }

        [Header("SignSizeId")]
        public int? SignSizeId { get; set; }

        [Header("SignHeaderId")]
        public int? SignHeaderId { get; set; }
    }
}
