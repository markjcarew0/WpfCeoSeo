using CeoSeoDataAccess;
using DataTransferObjects;
using System.Collections.Generic;

namespace CeoSeoCommon
{
    public static class GoogleDataBroker
    {
        public static string GetSearchResponse(string searchString)
        {
            return GoogleData.GetGoogleSearchData(searchString);
        }

    }
}
