using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vestcom.PaperSign.ACME.Entities
{
    [AttributeUsage(AttributeTargets.Property)]
    public class HeaderAttribute : Attribute
    {

        public HeaderAttribute(string name)
        {
            this.Header = name;
        }
        public string Header { get; set; }
    }
}
