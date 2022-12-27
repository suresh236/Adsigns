using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vestcom.PaperSign.ACME.Holiday.Entities;

namespace Vestcom.PaperSign.ACME.Holiday.Records
{
    public class SignHeadingFiller
    {
        public static IEnumerable<SignHeading> AddSignHeadingFiller()
        {
            return new List<SignHeading>()
            {
                new SignHeading(){ HEADING_NO ="0060", SignHeaderDesc ="Christmas Day"},
                new SignHeading(){ HEADING_NO ="0061", SignHeaderDesc ="Independence Day"},
                new SignHeading(){ HEADING_NO ="0062", SignHeaderDesc ="Labor Day"},
                new SignHeading(){ HEADING_NO ="0063", SignHeaderDesc ="New Year's Day"},
                new SignHeading(){ HEADING_NO ="0064", SignHeaderDesc ="Memorial Day"},
                new SignHeading(){ HEADING_NO ="0065", SignHeaderDesc ="Thanksgiving"},
                new SignHeading(){ HEADING_NO ="0066", SignHeaderDesc = "Easter Sunday" },
            };
        }
    }
}

