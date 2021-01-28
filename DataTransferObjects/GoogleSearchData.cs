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
    /// <summary>
    /// a data transfer object used to contain each line of 
    /// the data returned from the google query
    /// </summary>
    public class GoogleSearchData : IGoogleSearchData
    {
        private string foundData;

        private bool isSmokeBall;

        private int queryPosition;

        public string FoundData
        {
            get { return foundData; }
            set { foundData = value; }
        }

        public bool IsSmokeBall
        {
            get { return isSmokeBall; }
            set { isSmokeBall = value; }
        }

        public int QueryPosition
        {
            get { return queryPosition; }
            set { queryPosition = value; }
        }
    }
}