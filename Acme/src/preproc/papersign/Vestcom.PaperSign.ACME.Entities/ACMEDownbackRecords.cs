using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vestcom.PaperSign.ACME.Entities
{
    public class ACMEDownbackRecords
    {
        public IList<SignLayout> SignLayouts { get; set; }
        public IList<Department> Departments { get; set; }
        public IList<Heading> Headings { get; set; }
        public IList<Image> Images { get; set; }
        public IList<UnitPriceEntry> UnitPriceEntrys { get; set; }
        public IList<AcmeRecord> DownBackInputRecords { get; set; }
        public IList<SubstituteStock> SubstituteStock { get; set; }
    }
}
