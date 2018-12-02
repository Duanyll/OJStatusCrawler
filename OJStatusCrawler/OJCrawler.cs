using System;
using System.Collections.Generic;
using System.Text;

namespace OJStatusCrawler
{
    abstract class OJCrawler
    {
        public abstract List<StatusLog> Go(string UserName);
    }
}
