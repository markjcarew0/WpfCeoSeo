using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using System.Windows;
using Microsoft.Extensions.Logging;
using DataTransferObjects;

namespace WpfCeoSeo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IServiceProvider serviceProvider;

        public App()
        {
            IServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            services.AddSingleton(CreateLogger());
            services.AddTransient<IGoogleSearchData, GoogleSearchData>();
        }


        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }

        private Serilog.ILogger CreateLogger()
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
