using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

using System.Threading.Tasks;
using Windows.UI.Core;
using System.Net;
using System.IO;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using System.IO.Compression;
using Windows.Storage;
using Windows.Foundation;

namespace BackgroundTasks
{
    public sealed class ForecastData
    {
        ApplicationDataContainer appSettings;

        internal double confidenceDemocrat;
        internal double confidenceRepublican;

        internal double confidenceDifference = 0;

        public ForecastData()
        {
            appSettings = ApplicationData.Current.LocalSettings;

        }


        bool navigationcomplete = false;

        internal async Task GetData(int forecastType)
        {

            //GET CORRECT URL FOR FORECASTTYPE (POLLS PLUS, POLLS ONLY, NOW-CAST)
            string url = "http://projects.fivethirtyeight.com/2016-election-forecast/";
            switch (forecastType)
            {
                case 0:
                    url = url + "/#plus";
                    break;
                case 2:
                    url = url + "/#now";
                    break;
            }
            

            //GET RAW HTML WITH HTTPCLIENT
            string result = "";

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "ISO-8859-1");
            HttpResponseMessage response = await client.GetAsync("http://projects.fivethirtyeight.com/2016-election-forecast/");
            response.EnsureSuccessStatusCode();

            //DECOMPRESS GZIP RESPONSE WITH GZIPSTREAM
            using (var responseStream = await response.Content.ReadAsStreamAsync())
            using (var deflateStream = new GZipStream(responseStream, CompressionMode.Decompress))
            using (var streamReader = new StreamReader(deflateStream))
            {
                result = streamReader.ReadToEnd();
            }
            
            //CONVERT TO HTMLDOCUMENT (HTMLAGILITYPACK)
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(result);
            

            //GET CONFIDENCE SCORES
            var inputs = from input in html.DocumentNode.Descendants("p")
                         where input.Attributes.Count > 0 && input.Attributes["class"].Value == "candidate-val winprob"
                         select input;
            
            confidenceDemocrat = double.Parse(inputs.ToList()[0].InnerText.TrimEnd('%'));
            confidenceRepublican = double.Parse(inputs.ToList()[1].InnerText.TrimEnd('%'));

            Debug.WriteLine(confidenceDemocrat + " HILLARY");
            Debug.WriteLine(confidenceRepublican + " TRUMP");

            //GET LAST FORECAST IF AVAILABLE
            Point lastValues =  new Point(confidenceDemocrat, confidenceRepublican);
            if (appSettings.Values.ContainsKey("lastforecast"))
            {
                lastValues = (Point)appSettings.Values["lastforecast"];
            }
            if (appSettings.Values.ContainsKey("lastdifference"))
            {
                confidenceDifference = (double)appSettings.Values["lastdifference"];
            }

            //COMPARE TO LAST FORECAST
            if (confidenceDemocrat - lastValues.X > 0.01 || confidenceDemocrat - lastValues.X < -0.01)
            {
                //SET DIFFERENCE AND SAVE CURRENT FORECAST AS LASTFORECAST
                confidenceDifference = confidenceDemocrat - lastValues.X;
                appSettings.Values["lastforecast"] = new Point(confidenceDemocrat, confidenceRepublican);
                appSettings.Values["lastdifference"] = confidenceDifference;
            }
        }

        private void Webview_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            navigationcomplete = true;
        }
    }
}
