using Microsoft.Win32;
using ScrazonNative;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace Scrazon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static SolidColorBrush redColor;
        static SolidColorBrush greenColor;
        static SolidColorBrush yellowColor;
        static SaveFileDialog saveFileDialog;
        public static ObservableCollection<string> ViewedItem;
        static StreamWriter streamWriter;
        static bool locationset = false;
        static bool condition = false;
        static bool isdataavailable = false;
        static bool isHeadless = true;
        static AmazonProduct data;
        public MainWindow()
        {
            InitializeComponent();
            ViewedItem = new ObservableCollection<string>();
            viewdItemListBox.ItemsSource = ViewedItem;
            redColor = new SolidColorBrush();
            redColor.Color = Color.FromRgb(237, 36, 73);
            greenColor = new SolidColorBrush();
            greenColor.Color = Color.FromRgb(36, 237, 120);
            yellowColor = new SolidColorBrush();
            yellowColor.Color = Color.FromRgb(237, 173, 36);
            existsInViedIndicator.Fill = greenColor;
        }

        private async void createSession_Click(object sender, RoutedEventArgs e)
        {
            bool status = await Amazon.OpenDriverAsync(isHeadless);
            if (status)
            {
                indicatorCircle.Fill = greenColor;
                if (string.IsNullOrWhiteSpace(code.Text))
                {
                    searchButton.IsEnabled = false;
                }
                else
                {
                    searchButton.IsEnabled = true;
                }
                closeSession.IsEnabled = true;
                createSession.IsEnabled = false;
                condition = true;
                restartSession.IsEnabled = true;
            }
            else
            {
                indicatorCircle.Fill = redColor;
            }
        }

        private async void closeSession_Click(object sender, RoutedEventArgs e)
        {
            bool status = await Amazon.CloseConnectionAsync();
            if (status)
            {
                indicatorCircle.Fill = redColor;
                restartSession.IsEnabled = false;
            }
            else
            {
                indicatorCircle.Fill = greenColor;
            }
            closeSession.IsEnabled = false;
            searchButton.IsEnabled = false;
            createSession.IsEnabled = true;
            condition = false;
        }

        private async void searchButton_Click(object sender, RoutedEventArgs e)
        {
            var tempcolor = indicatorCircle.Fill;
            indicatorCircle.Fill = yellowColor;
            searchButton.IsEnabled = false;
            try
            {
                data = await Amazon.GetProductInfoAsync(code.Text);
                title.Text = data.Title;
                rating.Text = data.Rating.ToString();
                totalreview.Text = data.TotalReviews.ToString();
                recent.Text = data.RecentReviews.ToString();
                isdataavailable = true;
                if (locationset)
                {
                    saveButton.IsEnabled = true;
                }
                if (!ViewedItem.Contains(code.Text))
                {
                    ViewedItem.Add(code.Text);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Please enter a valid product code or restart the session.",
                    "A small problem",
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
            }
            if (string.IsNullOrWhiteSpace(code.Text) || condition == false)
            {
                searchButton.IsEnabled = false;
            }
            else
            {
                searchButton.IsEnabled = true;
            }

            indicatorCircle.Fill = tempcolor;
        }

        private void code_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(code.Text) || condition == false)
            {
                searchButton.IsEnabled = false;
            }
            else
            {
                searchButton.IsEnabled = true;
            }
            if (ViewedItem.Contains(code.Text))
            {
                existsInViedIndicator.Fill = yellowColor;
            }
            else
            {
                existsInViedIndicator.Fill = greenColor;
            }
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (condition)
            {
                await Amazon.CloseConnectionAsync();
            }
        }

        private async void saveButton_Click(object sender, RoutedEventArgs e)
        {
            var tempcolor = indicatorCircle.Fill;
            indicatorCircle.Fill = yellowColor;
            List<string> writable = new List<string>()
            {
                data.Title.Replace(","," - "),
                data.URL,
                data.Rating.ToString(),
                data.TotalReviews.ToString(),
                data.RecentReviews.ToString()
            };
            using (streamWriter = File.AppendText(saveFileDialog.FileName))
            {
                await streamWriter.WriteLineAsync(string.Join(",", writable));
            }
            indicatorCircle.Fill = tempcolor;
        }

        private void setSavelocation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV (*.csv)|*.csv|All files (*.*)|*.*";
                saveFileDialog.AddExtension = true;
                saveFileDialog.FileName = "output";
                saveFileDialog.ShowDialog();
                savingLocationText.Text = saveFileDialog.FileName;
                locationset = true;
                if (isdataavailable)
                {
                    saveButton.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message, 
                    ex.HResult.ToString(), 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        private void mahmudx_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
        }

        private void mahmudnotes_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
        }

        private void infourlbutton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://mahmudx.azurewebsites.net/product/scrazon");
        }

        private void headlessCheck_Checked(object sender, RoutedEventArgs e)
        {
            isHeadless = true;
        }

        private void headlessCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            isHeadless = false;
        }

        private void restartSession_Click(object sender, RoutedEventArgs e)
        {
            closeSession_Click(sender, e);
            createSession_Click(sender, e);
        }

        private void resetSearchList_Click(object sender, RoutedEventArgs e)
        {
            ViewedItem.Clear();
        }
    }
}
