using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace ScrazonNative
{
    class Amazon
    {
        public static ChromeDriver driver;
        private static async Task<int> GetRecentReviewsAsync(int pagerange, string code)
        {
            int result = 0;
            TimeSpan acceptableTime = new TimeSpan(120, 12, 5, 3);
            for (int i = 1; i < pagerange + 1; i++)
            {
                string url = string.Format(
                    "https://www.amazon.com/product-reviews/{0}/ref=cm_cr_arp_d_viewopt_srt?ie=UTF8&reviewerType=all_reviews&sortBy=recent&pageNumber={1}", code, i);
                await Task.Run(() => driver.Navigate().GoToUrl(url));
                var dates = await Task.Run(() =>
                    driver.FindElements(By.XPath("//span[@data-hook=\"review-date\"]")));
                var lastDateElement = dates[dates.Count - 1].Text.Split();
                int l = lastDateElement.Length;
                string dateString = lastDateElement[l - 3] + " " + lastDateElement[l - 2] + " " + lastDateElement[l - 1];
                DateTime lastDate = DateTime.Parse(dateString);
                TimeSpan reviewAge = DateTime.Now - lastDate;
                if (acceptableTime < reviewAge)
                {
                    int temp = 0;
                    foreach (var item in dates)
                    {
                        var dateElement = item.Text.Split();
                        dateString = dateElement[l - 3] + " " + dateElement[l - 2] + " " + dateElement[l - 1];
                        DateTime date = DateTime.Parse(dateString);
                        if (DateTime.Now - date <= acceptableTime)
                        {
                            temp++;
                        }
                    }
                    result = ((i - 1) * 10) + temp;
                    break;
                }
            }
            return result;
        }
        public async static Task<AmazonProduct> GetProductInfoAsync(string code)
        {
            AmazonProduct amazonProduct = new AmazonProduct();
            string url = string.Format("https://www.amazon.com/dp/{0}", code);
            amazonProduct.URL = url;
            await Task.Run(() => driver.Navigate().GoToUrl(url));
            amazonProduct.Title =
                driver.FindElementByXPath("//span[@id=\"productTitle\"]").Text.Trim();
            amazonProduct.TotalReviews =
                int.Parse(driver.FindElementByXPath("//span[@id=\"acrCustomerReviewText\"]")
                .Text.Split()[0].Replace(",", "").Trim());
            amazonProduct.Rating =
                float.Parse(driver.FindElementByXPath("//span[@data-hook=\"rating-out-of-text\"]")
                .Text.Split()[0]);
            int pagerange = amazonProduct.TotalReviews / 10;
            amazonProduct.RecentReviews = await GetRecentReviewsAsync(pagerange, code);
            
            return amazonProduct;

        }
        public async static Task<bool> OpenDriverAsync(bool isHeadless)
        {
            try
            {
                var options = new ChromeOptions();
                var ffds = ChromeDriverService.CreateDefaultService();
                ffds.HideCommandPromptWindow = true;
                if (isHeadless)
                {
                    options.AddArguments("--headless");
                }
                await Task.Run(() => driver = new ChromeDriver(ffds, options));
                return true;
            }
            catch (Exception e)
            {
                string title = e.HResult.ToString();
                MessageBox.Show(e.Message, title);
                return false;
            }
        }
        public async static Task<bool> CloseConnectionAsync()
        {
            try
            {
                await Task.Run(() => driver.Quit());
                return true;
            }
            catch (Exception e)
            {
                string title = e.HResult.ToString();
                MessageBox.Show(e.Message, title);
                return false;
            }
        }

    }
}
