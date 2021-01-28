// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GoogleDataService.cs" company="MarkJC">
//   Author Mark Carew 
// </copyright>
// <summary>
//   google data returned from query
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace DataTransferObjects
{
    public class GoogleDataService : IGoogleDataService
    {
        public GoogleDataService()
        {
        }

        /// <summary>
        /// returns a new instance of GoogleSearchData
        /// which is used to make up a list of lines 
        /// to be displayed in the UI
        /// </summary>
        /// <returns></returns>
        public IGoogleSearchData GetGoogleSearchData()
        {
            return new GoogleSearchData();
        }
    }
}
