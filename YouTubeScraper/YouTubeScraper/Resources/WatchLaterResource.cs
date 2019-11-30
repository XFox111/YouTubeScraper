using System;
using System.Collections.Generic;
using System.Text;

namespace YouTube.Resources
{
    public class WatchLaterResource
    {
        public class ListRequest { }
        public class InsertRequest { }
        public class DeleteRequest { }

        public ListRequest List()
        {
            return new ListRequest();
        }

        public InsertRequest Insert(string videoId)
        {
            return new InsertRequest();
        }

        public DeleteRequest Delete(string videoId)
        {
            return new DeleteRequest();
        }
    }
}
