using CeoSeoDataAccess;
using DataTransferObjects;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Threading;

namespace CeoSeoCommon
{
    public static class GoogleDataBroker
    {
        public static HtmlNodeCollection GetSearchResponse(string searchString)
        {
            return GoogleData.GetGoogleSearchData(searchString);
        }
    }
}
