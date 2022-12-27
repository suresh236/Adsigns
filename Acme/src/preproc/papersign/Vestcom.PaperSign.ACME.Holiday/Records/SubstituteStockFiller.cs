using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vestcom.PaperSign.ACME.Holiday.Entities;

namespace Vestcom.PaperSign.ACME.Holiday.Records
{
    public class SubstituteStockFiller
    {
        public static IEnumerable<SubstituteStock> AddSubstituteStockFiller()
        {
            return new List<SubstituteStock>()
            {
                new SubstituteStock(){MAIN_STOCK="2000",SUBSTITUTE_STOCK="1007",BG_IMG="xmashours"},
                new SubstituteStock(){MAIN_STOCK="2001",SUBSTITUTE_STOCK="1007",BG_IMG="july4thhours"},
                new SubstituteStock(){MAIN_STOCK="2002",SUBSTITUTE_STOCK="1007",BG_IMG="labordayhours"},
                new SubstituteStock(){MAIN_STOCK="2003",SUBSTITUTE_STOCK="1007",BG_IMG="newyearhours"},
                new SubstituteStock(){MAIN_STOCK="2004",SUBSTITUTE_STOCK="1007",BG_IMG="memhours"},
                new SubstituteStock(){MAIN_STOCK="2005",SUBSTITUTE_STOCK="1007",BG_IMG="thankshours"},
                new SubstituteStock(){MAIN_STOCK="2006",SUBSTITUTE_STOCK="1007",BG_IMG="easterhours"},
            };
        }
    }
}

