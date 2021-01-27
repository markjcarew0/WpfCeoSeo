// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="MarkJC">
//   Author Mark Carew
//   Date Sunday 24-01-2020
// </copyright>
// <summary>
// View Model for the Ceo Seo UI 
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using CeoSeoCommon;
using DataTransferObjects;
using HtmlAgilityPack;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CeoSeoViewModels
{
    //public class MainWindowViewModel : ReactiveObject, IDisposable
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        /// <summary>
        /// if this was really a disposable class 
        /// being instantiated and disposed 
        /// </summary>
        private readonly IDisposable cleanUp;

        /// <summary>
        /// the data that is shown in the UI
        /// filtered by
        /// </summary>
        private List<GoogleSearchData> listData;

        /// <summary>
        /// private backing field 
        /// for public Property QuerySearchString 
        /// </summary>
        private string querySearchString;

        private List<int> rankList;

        /// <summary>
        /// private backing field
        /// for public Property SearchSpinnerOn 
        /// </summary>
        private bool searchSpinnerOn;

        /// <summary>
        /// show only data that contains smokeball
        /// </summary>
        private bool smokeBallOnly;

        /// <summary>
        ///     syncro context
        /// </summary>
        private readonly TaskScheduler synchronisationContext;

        /// <summary>
        /// incomplete implimentation 
        /// </summary>
        private CancellationTokenSource cancelTokenSource;

        /// <summary>
        /// constructor for MainWindowViewModel
        /// </summary>
        public MainWindowViewModel()
        {
            this.SearchSpinnerOn = false;
            this.SmokeBallOnly = false;

            var scheduler = RxApp.MainThreadScheduler;

            // build original data set
            this.WhenAnyValue(vm => vm.QuerySearchString)
            .Where(x => x != null)
            .Throttle(TimeSpan.FromMilliseconds(350), scheduler)
            .ObserveOn(scheduler)
            .Subscribe(this.GetQueryResultData);

            this.WhenAnyValue(vm => vm.SmokeBallOnly)
                 .ObserveOn(scheduler)
                 .Subscribe(CreateFilteredListData);

            // clear all collections
            this.SourceData = new List<GoogleSearchData>();
            this.ListData = new List<GoogleSearchData>();
            this.RankList = new List<int>();

            this.synchronisationContext = TaskScheduler.FromCurrentSynchronizationContext();

            this.QuerySearchString = "conveyancing software";

            // cleanUp = new CompositeDisposable(disposableSource);
        }


        /// <summary>
        ///     Gets the source.
        /// </summary>
        public List<GoogleSearchData> SourceData
        {
            get;
        }

        /// <summary>
        ///     Gets the source.
        /// </summary>
        public List<GoogleSearchData> ListData
        {
            get => listData;
            set
            {
                listData = value;
                this.OnPropertyChanged(nameof(this.ListData));
            }
        }

        /// <summary>
        /// contains the query string UI entered for the google search 
        /// </summary>
        public string QuerySearchString
        {
            get
            {
                return this.querySearchString;
            }

            set
            {
                this.querySearchString = value;
                this.OnPropertyChanged(nameof(this.QuerySearchString));
            }
        }

        /// <summary>
        /// property that toggles on and off the visibility of 
        /// the spinner control 
        /// that indicates that processing activity is going on
        /// </summary>
        public bool SearchSpinnerOn
        {
            get
            {
                return searchSpinnerOn;
            }

            set
            {
                this.searchSpinnerOn = value;
                this.OnPropertyChanged(nameof(this.SearchSpinnerOn));
            }
        }

        /// <summary>
        /// filter to show only smoke ball data
        /// </summary>
        public bool SmokeBallOnly
        {
            get => smokeBallOnly;

            set
            {
                this.smokeBallOnly = value;
                this.OnPropertyChanged(nameof(this.SmokeBallOnly));
            }
        }

        /// <summary>
        /// a list showing the rank positions in seo order that the
        /// query returnde results that contained SmokeBall
        /// </summary>
        public List<int> RankList
        {
            get => rankList;
            set
            {
                rankList = value;
                this.OnPropertyChanged(nameof(this.RankList));
            }
        }

        /// <summary>
        /// minimum implementation of IDisposable
        /// </summary>
        public void Dispose()
        {
            cleanUp.Dispose();
        }

        /// <summary>
        /// create the list of data that is to shown
        /// which is data from SourceData property
        /// filtered by 
        /// </summary>
        /// <param name="flterOrNotFilter"></param>
        private void CreateFilteredListData(bool flterOrNotFilter)
        {
            if (flterOrNotFilter)
            {
                this.ListData =
                        this.SourceData
                        .Where(x => x.IsSmokeBall == flterOrNotFilter)
                        .ToList();
            }
            else
            {
                this.ListData =
                       this.SourceData
                       .ToList();
            }
        }

        /// <summary>
        ///  get the raw data from the google query
        /// </summary>
        /// <param name="query"></param>
        private void GetQueryResultData(string query)
        {
            this.CancelCreateProcess();

            // turn on the spinner wait control showing that earch action is being executed
            this.SearchSpinnerOn = true;

            // run the search in a task so that we can set the ui 
            var CancelTask = Task.Run(
                    () =>
                    {
                        return GoogleDataBroker.GetSearchResponse(query);
                    })
                .ContinueWith(
                    x =>
                    {
                        if (x.Result != null)
                        {
                            this.SourceData.Clear();

                            CreateSourceData(x);

                            CreateFilteredListData(this.SmokeBallOnly);

                            this.SearchSpinnerOn = false;
                        }
                    },
                    this.synchronisationContext)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// create the source data from the google query returned results
        /// </summary>
        /// <param name="x"></param>
        private void CreateSourceData(Task<HtmlNodeCollection> x)
        {
            var returnNodesData = new List<GoogleSearchData>();

            var positionInList = 0;

            foreach (HtmlNode oneNode in x.Result)
            {
                var nodeContent = oneNode.InnerText;
                var newLine = new GoogleSearchData
                {
                    FoundData = nodeContent,
                    IsSmokeBall = nodeContent.Contains("smokeball", StringComparison.OrdinalIgnoreCase),
                    QueryPosition = ++positionInList
                };

                returnNodesData.Add(newLine);
            }
            // turn off the spinner wait control showing that search action is completed
            this.SourceData.AddRange(returnNodesData);

            // build up the rank list for smokeball appearances
            MakeRankList();

        }

        /// <summary>
        ///  build up the rank list for smokeball appearances
        /// </summary>
        private void MakeRankList()
        {
            this.RankList.Clear();
            this.RankList =
                this.SourceData
                    .Where(x => x.IsSmokeBall)
                    .Select(x => x.QueryPosition)
                    .ToList<int>();
        }
    }
}
