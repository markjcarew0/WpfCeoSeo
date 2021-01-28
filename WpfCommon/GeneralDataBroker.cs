// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneralDataBroker.cs" company="MarkJC">
//   Author Mark Carew
//   Date Sunday 24-01-2020
// </copyright>
// <summary>
// contains a data broker that is the entrance point to the datalayer
// currently it only retrieves data from google queries
// would contain any other common routines available to the viewmodel layer 
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace CeoSeoCommon
{
    using CeoSeoDataAccess;
    using HtmlAgilityPack;

    /// <summary>
    /// all data update and retrieval processing passes through this static class
    /// </summary>
    public static class GeneralDataBroker
    {
        /// <summary>
        /// return the data that is requested from google.com
        /// using the entered search string from the UI
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
        public static HtmlNodeCollection GetSearchResponse(string searchString)
        {
            return GoogleData.GetGoogleSearchData(searchString);
        }
    }
}
