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
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static IServiceProvider serviceProvider;

        public App()
        {
            IServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// add the required classes to be resolved from the dependency injection container
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureServices(IServiceCollection services)
        {
            // allows the creation of the main window by Dependency Inversion
            services.AddSingleton<MainWindow>();

            // allows the creation of a logger for the application by Dependency Inversion
            services.AddSingleton(CreateLogger());

            // allows the creation of instances of the only Data Transfer Object used by the application by Dependency Inversion
            services.AddTransient<IGoogleDataService>(provider => new GoogleDataService());
        }

        /// <summary>
        /// retrieve the startup point for the program from the container
        /// and execute it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStartup(object sender, StartupEventArgs e)
        {
            // raise the main windo 
            var mainWindow = serviceProvider.GetService<MainWindow>();
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
    }
 }
