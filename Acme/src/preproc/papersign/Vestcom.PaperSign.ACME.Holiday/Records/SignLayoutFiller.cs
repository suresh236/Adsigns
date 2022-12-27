using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vestcom.PaperSign.ACME.Holiday.Entities;

namespace Vestcom.PaperSign.ACME.Holiday.Records
{
    public class SignLayoutFiller
    {
        public static IEnumerable<SignLayout> AddSignLayFiller()
        {
            return new List<SignLayout>()
            {
                new SignLayout(){SL_SIGN_SIZE="0020",SL_SIGN_HEADING="0060",SL_LAYOUT_NO="0020",SL_PAPER_TYPE="2000"},
                new SignLayout(){SL_SIGN_SIZE="0020",SL_SIGN_HEADING="0061",SL_LAYOUT_NO="0020",SL_PAPER_TYPE="2001"},
                new SignLayout(){SL_SIGN_SIZE="0020",SL_SIGN_HEADING="0062",SL_LAYOUT_NO="0020",SL_PAPER_TYPE="2002"},
                new SignLayout(){SL_SIGN_SIZE="0020",SL_SIGN_HEADING="0063",SL_LAYOUT_NO="0020",SL_PAPER_TYPE="2003"},
                new SignLayout(){SL_SIGN_SIZE="0020",SL_SIGN_HEADING="0064",SL_LAYOUT_NO="0020",SL_PAPER_TYPE="2004"},
                new SignLayout(){SL_SIGN_SIZE="0020",SL_SIGN_HEADING="0065",SL_LAYOUT_NO="0020",SL_PAPER_TYPE="2005"},
                new SignLayout(){SL_SIGN_SIZE="0020",SL_SIGN_HEADING="0066",SL_LAYOUT_NO="0020",SL_PAPER_TYPE="2006"},
            };
        }
    }
}

