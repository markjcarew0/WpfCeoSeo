// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.cs" company="MarkJC">
//   Author Mark Carew 
// </copyright>
// <summary>
//   the application class which is the entry point of the application
//   where the dependency injection container is instantiated and 
//   initialised
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace WpfCeoSeo
{
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using Serilog;
    using System.Windows;
    using DataTransferObjects;
    using System.Windows.Threading;
    using CeoSeoViewModels;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IServiceProvider serviceProvider;
        ILogger localLogger;

        public static IServiceProvider ServiceProvider { get => serviceProvider; set => serviceProvider = value; }

        public App()
        {
            IServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// add the required classes to be resolved from the dependency injection container
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureServices(IServiceCollection services)
        {
            // services.AddSingleton(provider => MainWindowViewModelFactory());

            // allows the creation of the main window by Dependency Inversion
            services.AddSingleton<MainWindow>();

            services.AddSingleton(provider => MainWindowViewModelFactory());

            // allows the creation of a logger for the application by Dependency Inversion
            localLogger = CreateLogger();
            services.AddSingleton(localLogger);

            // allows the creation of instances of the only Data Transfer Object used by the application by Dependency Inversion
            services.AddTransient<IGoogleDataService>(provider => new GoogleDataService());
        }

        /// <summary>
        /// add a fact
        /// </summary>
        /// <returns></returns>
        private IMainWindowViewModel MainWindowViewModelFactory()
        {
            return new MainWindowViewModel(ServiceProvider.GetService<ILogger>(), ServiceProvider.GetService<IGoogleDataService>());
        }

        /// <summary>
        /// retrieve the startup point for the program from the container
        /// and execute it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStartup(object sender, StartupEventArgs e)
        {
            // raise the main window 
            var mainWindow = ServiceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }

        /// <summary>
        /// for logging we are to use serilog
        /// </summary>
        /// <returns></returns>
        private ILogger CreateLogger()
        {
            var logger = Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               // .WriteTo.Console()
               .WriteTo.File("logs\\myapp.txt", rollingInterval: RollingInterval.Day)
               .CreateLogger();

            return logger;
        }

        /// <summary>
        ///     The app dispatcher unhandled exception.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Process unhandled exception
            // Prevent default unhandled exception processing
            if (this.MainWindow != null)
            {
                MessageBox.Show(this.MainWindow, e.Exception.StackTrace, "Dispatcher Exception");
            }

            localLogger.Error(e.Exception, "DispatcherUnhandledException");
            e.Handled = true;
        }

    }
}
