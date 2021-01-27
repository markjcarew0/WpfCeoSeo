// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGoogleDataService.cs" company="MarkJC">
//   Author Mark Carew 
// </copyright>
// <summary>
//   interface definition for IGoogleDataService
//  whoose purpose is to create instances of IGoogleSearchData
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataTransferObjects
{
    public interface IGoogleDataService
    {
        // returns an instance of IGoogleSearchData
        IGoogleSearchData GetGoogleSearchData();
    }
}
