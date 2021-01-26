using CeoSeoViewModels;
using DataTransferObjects;
using System.Windows;
using System.Windows.Data;

namespace WpfCeoSeo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private MainWindowViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
