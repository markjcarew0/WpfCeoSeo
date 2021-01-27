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
using DynamicData;
using DynamicData.Binding;
using HtmlAgilityPack;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace CeoSeoViewModels
{
    //public class MainWindowViewModel : ReactiveObject, IDisposable
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        /// <summary>
        /// private backing field 
        /// for public Property QuerySearchString 
        /// </summary>
        private string querySearchString;

        /// <summary>
        ///     sync context tied to mainline
        /// </summary>
        private readonly TaskScheduler synchronisationContext;

        /// <summary>
        /// private backing field
        /// for public Property SearchSpinnerOn 
        /// </summary>
        private bool searchSpinnerOn;

        /// <summary>
        /// show only data that contains smokeball
        /// </summary>
        private bool smokeBallOnly;

        private readonly IDisposable cleanUp;

        /// <summary>
        /// the data that is shown in the UI
        /// filtered by
        /// </summary>
        private List<GoogleSearchData> listData;

        /// <summary>
        /// constructor for MainWindowViewModel
        /// </summary>
        public MainWindowViewModel()
        {
            this.SearchSpinnerOn = false;
            this.SmokeBallOnly = true;

            var scheduler = RxApp.MainThreadScheduler;

            // build original data set
            this.WhenAnyValue(vm => vm.QuerySearchString)
            .Where(x => x != null)
            .Throttle(TimeSpan.FromMilliseconds(350), scheduler)
            .ObserveOn(scheduler)
            .Subscribe(this.GetQueryResultData);

            this.WhenAnyValue(vm => vm.SmokeBallOnly)
                 .ObserveOn(scheduler)
                 .Subscribe(FilterListData);

            // clear all collections
            this.SourceData = new List<GoogleSearchData>();
            this.ListData = new List<GoogleSearchData>();

            this.synchronisationContext = TaskScheduler.FromCurrentSynchronizationContext();

            this.QuerySearchString = "conveyancing software";

            // cleanUp = new CompositeDisposable(disposableSource);
        }

        /// <summary>
        /// create the list of data that is to shown
        /// which is data from SourceData property
        /// filtered by 
        /// </summary>
        /// <param name="flterOrNotFilter"></param>
        private void FilterListData(bool flterOrNotFilter)
        {
            this.ListData =
                    this.SourceData
                    .Where(x => x.IsSmokeBall == flterOrNotFilter)
                    .ToList();
        }

        private void GetQueryResultData(string query)
        {
            // turn on the spinner wait control showing that earch action is being executed
            this.SearchSpinnerOn = true;

            // run the search in a task so that we can set the ui 
            Task.Run(
                    () =>
                    {
                        return GoogleDataBroker.GetSearchResponse(query);
                    })
                .ContinueWith(
                    x =>
                    {
                        this.SourceData.Clear();
                        var returnNodesData = new List<GoogleSearchData>();
                        var positionInList = 0;


                        if (x.Result != null)
                        {
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
                            this.SearchSpinnerOn = false;
                            this.SourceData.AddRange(returnNodesData);

                            FilterListData(this.SmokeBallOnly);
                        }
                    },
                    this.synchronisationContext)
                .ConfigureAwait(false);
        }

        /// <summary>
        ///     Gets the source.
        /// </summary>
        public List<GoogleSearchData> SourceData { get; }

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

        public void Dispose()
        {
            cleanUp.Dispose();
        }
    }
}
