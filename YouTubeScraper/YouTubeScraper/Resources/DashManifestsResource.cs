using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeScraper.Resources
{
    public class DashManifestsResource
    {
        public class ListRequest { }

        public ListRequest List(string id)
        {
            return new ListRequest();
        }
    }
}
