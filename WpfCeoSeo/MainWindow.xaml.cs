// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.cs" company="MarkJC">
//   Author Mark Carew
// </copyright>
// <summary>
// the main and only window in this small application
// it is where the data from the google query is displayed
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WpfCeoSeo
{
    using CeoSeoViewModels;
    using DataTransferObjects;
    using Serilog;
    using System;
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// get the google data transient instance 
        /// not really required to keep this as a private field.
        /// It is a pass through parameter for the view model
        /// </summary>
        private IGoogleDataService googleDataService;

        /// <summary>
        /// logger is used for logging 
        /// </summary>
        private ILogger logger;

        /// <summary>
        /// constructor
        /// </summary>
        public MainWindow(ILogger _logger, IGoogleDataService _googleDataService)
        {
            try
            {
                this.logger = _logger;
                this.googleDataService = _googleDataService;

                InitializeComponent();

                this.DataContext = new MainWindowViewModel(this.logger, this.googleDataService);

                logger.Write(Serilog.Events.LogEventLevel.Information, "Program started");
                logger.Information("Just testing");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Application crashed on startup");
            }
        }

        /// <summary>
        ///  when pressed close the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShutDownButtonClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// this is supposed to set the focus to the first row of the data grid
        /// TODO: nedds attention
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataDatagrid_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate ()
            {
                UiUtils.SetFocusToDataGrid(this.DataDatagrid);
                UiUtils.SetFocusToDataGridFirstRow(DataDatagrid);
               //  UiUtils.SelectRowByIndex(DataDatagrid, 0);
            }), System.Windows.Threading.DispatcherPriority.Background);
        }
    }
}
