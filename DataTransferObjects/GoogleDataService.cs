using System;
using System.Collections.Generic;
using System.Text;

namespace DataTransferObjects
{
    public class GoogleDataService : IGoogleDataService
    {
        public GoogleDataService()
        {
        }

        public IGoogleSearchData GetGoogleSearchData()
        {
            return new GoogleSearchData();
        }
    }
}
