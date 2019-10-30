using System;
using System.Collections.Generic;
using System.Text;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace YouTubeScraper.Resources
{
    public class HistoryResource
    {
        public class ListRequest { }
        public class InsertRequest { }
        public class DeleteRequest { }
        public class ClearRequest { }

        public ListRequest List()
        {
            return new ListRequest();
        }

        public InsertRequest Insert(string videoId, TimeSpan? leftOn)
        {
            return new InsertRequest();
        }

        public DeleteRequest Delete(string videoId)
        {
            return new DeleteRequest();
        }

        public ClearRequest Clear()
        {
            return new ClearRequest();
        }
    }
}
