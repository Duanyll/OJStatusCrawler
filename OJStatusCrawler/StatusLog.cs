using System;
using System.Collections.Generic;
using System.Text;

namespace OJStatusCrawler
{
    public enum OJStatus
    {
        UKE,
        WA,
        AC,
        TLE,
        MLE,
        CE,
        OLE,
        PE,
        RE
    }

    public class StatusLog
    {
        public string UserName { get; set; }
        public DateTime Time { get; set; }
        public string ProblemID { get; set; }
        public OJStatus Status { get; set; }
        public string URL { get; set; }
    }
}
