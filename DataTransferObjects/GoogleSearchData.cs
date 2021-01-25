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
    public class GoogleSearchData
    {
        private string foundData;

        public string FoundData
        {
            get { return foundData; }
            set { foundData = value; }
        }

    }
}