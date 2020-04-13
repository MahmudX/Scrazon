using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace Scrazon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Amazon.OpenDriver();
        }       

        private async void startButton_Click(object sender, RoutedEventArgs e)
        {
            string d = code.Text;
            if (string.IsNullOrEmpty(d))
            {
                title.Text = "Hola";
            }
            else
            {
                var data = await Amazon.GetProductInfoAsync(code.Text);
                closeButton.IsEnabled = true;
                startButton.IsEnabled = false;
                title.Text = data.Title;
                totalreview.Text = data.TotalReviews.ToString();
                rating.Text = data.Rating.ToString();
                recent.Text = data.RecentReviews.ToString();
                percentage.Text = data.Percentage.ToString() + "%";
            }
        }
        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Amazon.CloseConnection();
            closeButton.IsEnabled = false;
            startButton.IsEnabled = true;

        }
    }
}
