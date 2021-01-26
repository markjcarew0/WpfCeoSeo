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
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace CeoSeoViewModels
{
    public class MainWindowViewModel : ReactiveObject
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
        /// constructor for MainWindowViewModel
        /// </summary>
        public MainWindowViewModel()
        {
            this.Source = new ObservableCollectionExtended<GoogleSearchData>();

            this.Source.ToObservableChangeSet()
                .Bind(out this.details)
                .Subscribe();

            this.synchronisationContext = TaskScheduler.FromCurrentSynchronizationContext();

            this.QuerySearchString = "conveyancing software";

            var scheduler = RxApp.MainThreadScheduler;

            // turn off the spinner wait control by default
            this.SearchSpinnerOn = false;

            this.WhenAnyValue(vm => vm.QuerySearchString)
                .Where(x => x != null)
                .Throttle(TimeSpan.FromMilliseconds(350), scheduler)
                .ObserveOn(scheduler)
                .Subscribe(this.GetQueryResultData);
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
                        this.Source.Clear();
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
                        this.SearchSpinnerOn = false;
                        this.Source.AddRange(returnNodesData);
                    },
                    this.synchronisationContext)
                .ConfigureAwait(false);
        }

        /// <summary>
        ///     Gets the source.
        /// </summary>
        public ObservableCollectionExtended<GoogleSearchData> Source { get; }

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
                this.RaiseAndSetIfChanged(ref this.querySearchString, value);
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
                this.RaiseAndSetIfChanged(ref this.searchSpinnerOn, value);
            }
        }

        private List<GoogleSearchData> googleReturnedData;

        public List<GoogleSearchData> GoogleReturnedData
        {
            get { return googleReturnedData; }
            set 
            {
                this.RaiseAndSetIfChanged(ref this.googleReturnedData, value);
            }
        }

        /// <summary>
        ///     The details.
        /// </summary>
        private ReadOnlyObservableCollection<GoogleSearchData> details;

        /// <summary>
        ///     Gets or sets the details.
        /// </summary>
        public ReadOnlyObservableCollection<GoogleSearchData> Details
        {
            get => this.details;

            set => this.RaiseAndSetIfChanged(ref this.details, value);
        }


    }
}
