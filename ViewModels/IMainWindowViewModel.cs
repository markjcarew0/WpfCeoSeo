// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="MarkJC">
//   Author Mark Carew
//   Date Sunday 24-01-2020
// </copyright>
// <summary>
// View Model for the Ceo Seo UI MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using DataTransferObjects;
using System.Collections.Generic;

namespace CeoSeoViewModels
{
    public interface IMainWindowViewModel
    {
        List<IGoogleSearchData> ListData { get; set; }
        string QuerySearchString { get; set; }
        List<int> RankList { get; set; }
        bool SearchSpinnerOn { get; set; }
        bool SmokeBallOnly { get; set; }
        List<IGoogleSearchData> SourceData { get; }

        void Dispose();
    }
}