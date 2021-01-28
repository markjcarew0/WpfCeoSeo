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
    using Messaging;
    using Serilog;
    using System;
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
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
                // this is necessary because we cant set focus to the first line of the datagrid
                // until the data is loaded.
                this.WeakEventSetUp();

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
        ///     Finalizes an instance of the <see cref="MainWindow" /> class.
        /// </summary>
        ~MainWindow()
        {
            this.Dispose(false);
        }

        /// <summary>
        ///     Gets a value indicating whether is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true); // I am calling you from Dispose, it's safe

            GC.SuppressFinalize(this);
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
        ///     The dispose.
        /// </summary>
        /// <param name="disposing">
        ///     The disposing.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                if (disposing)
                {
                    this.WeakEventTearDown();
                }

                this.IsDisposed = true;
            }
        }

        /// <summary>
        /// The set focus to patient medication history data grid.
        /// </summary>
        private void SetFocusToDataDatagrid()
        {
            UiUtils.SetFocusToDataGridFirstRow(this.DataDatagrid);
        }

        /// <summary>
        ///     The singleton get message.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void SingletonGetMessage(object sender, MessageEventArgs e)
        {
            // When this message arrives from the main window viewmodel
            // we know that the data has been loaded into the itemssource of the datagrid
            // by the fact that the process in the viewmodel of loading this data is complete.
            // We cannot use the loaded event for the datagrid
            // because the items.count of the itemssource will be zero
            // until the viewmodel has completed its process.
            if (e.MessageFor == "MainWindow")
            {
                if (e.Message == "SetFocus")
                {
                    var stringContent = e.Content as string;
                    if (stringContent is string)
                    {
                        if (stringContent == "RefreshDataDatagrid")
                        {
                            // DispatcherPriority lower than render should ensure items have rendered before we try to focus them
                            this.Dispatcher?.BeginInvoke(new Action(this.SetFocusToDataDatagrid), DispatcherPriority.ContextIdle, null);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     The weak event set up.
        /// </summary>
        private void WeakEventSetUp()
        {
            WeakEventManager<Messenger, MessageEventArgs>.AddHandler(
                Messenger.Singleton,
                "SendMessage",
                this.SingletonGetMessage);
        }

        /// <summary>
        ///     The weak event set up.
        /// </summary>
        private void WeakEventTearDown()
        {
            WeakEventManager<Messenger, MessageEventArgs>.RemoveHandler(
                Messenger.Singleton,
                "SendMessage",
                this.SingletonGetMessage);
        }
    }
}
