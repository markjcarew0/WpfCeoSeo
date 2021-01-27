// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GoogleSearchData.cs" company="MarkJC">
//   Author Mark Carew 
// </copyright>
// <summary>
//   google data returned from query
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DataTransferObjects
{
    public interface IGoogleSearchData
    {
        string FoundData { get; set; }
        bool IsSmokeBall { get; set; }
        int QueryPosition { get; set; }
    }
}