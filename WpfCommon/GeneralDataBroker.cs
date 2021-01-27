// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneralDataBroker.cs" company="MarkJC">
//   Author Mark Carew
//   Date Sunday 24-01-2020
// </copyright>
// <summary>
// a data broker that is the entrance point to the datalayer
// currently it only retrieves data from google queries
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using CeoSeoDataAccess;
using HtmlAgilityPack;

namespace CeoSeoCommon
{
    public static class GeneralDataBroker
    {
        public static HtmlNodeCollection GetSearchResponse(string searchString)
        {
            return GoogleData.GetGoogleSearchData(searchString);
        }
    }
}
