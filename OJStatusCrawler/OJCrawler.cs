using System;
using System.Collections.Generic;
using System.Text;

namespace OJStatusCrawler
{
    abstract class OJCrawler
    {
        public abstract List<StatusLog> SearchForACStatus(string UserName);
    }
}
