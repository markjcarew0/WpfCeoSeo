// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GoogleSearchData.cs" company="MarkJC">
//   Author Mark Carew 
// </copyright>
// <summary>
//   google data returned from query interface definition
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DataTransferObjects
{
    /// <summary>
    /// interface for implimentation by GoogleSearchData
    /// </summary>
    public interface IGoogleSearchData
    {
        string FoundData { get; set; }
        bool IsSmokeBall { get; set; }
        int QueryPosition { get; set; }
    }
}