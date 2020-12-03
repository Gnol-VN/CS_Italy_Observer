using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Expo.Server.Client;
using Expo.Server.Models;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace CS_Italy_Observer
{
    public class XboxWatcher
    {
        public static string CSRFToken = "324d6ed0-cc7a-4af7-9020-3710af23e680";
        static string expoPushToken = "ExponentPushToken[AICngyBJmFOCmWFqQlemgu]";

        public static void Main(string[] args)
        {
            ExecuteAmazonWatch();
            ExecuteArgosWatch();
            ExecuteSmythWatch();
        }

        private static void ExecuteAmazonWatch()
        {
            var chromeOptions = new ChromeOptions();
            // chromeOptions.AddArguments("--headless", "--no-sandbox", "--disable-gpu", "--whitelisted-ips");

            var service = ChromeDriverService.CreateDefaultService("D:\\CS_Italy_Observer\\Selenium", "chromedriver.exe");
            using IWebDriver driver = new ChromeDriver(service, chromeOptions);
            driver.Navigate().GoToUrl(new Uri("https://www.amazon.co.uk/gp/product/B08H93GKNJ"));
            Task.Delay(5000).Wait();

            Console.Write(driver.FindElement(By.XPath("//div[@id='availability']/span")).Text);
        }

        private static void ExecuteArgosWatch()
        {
            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument document = htmlWeb.Load("http://www.argos.ie/static/Product/partNumber/8448262/Trail/searchtext%3EXBOX+SERIES+X.htm");
            //List<HtmlNode>  cssSelectorNodes = document.DocumentNode.QuerySelectorAll("div").ToList();
            HtmlNode h1 = document.DocumentNode.SelectSingleNode("//h1");
            if (h1.InnerText != "Sorry, Xbox is currently unavailable.")
            {
                SendPush(expoPushToken, "Argos detected", "AVAILABLE");
            }
        }

        private static void ExecuteSmythWatch()
        {
            var handler = new HttpClientHandler();


            // If you are using .NET Core 3.0+ you can replace `~DecompressionMethods.None` to `DecompressionMethods.All`
            handler.AutomaticDecompression = ~DecompressionMethods.All;

            using (var httpClient = new HttpClient(handler))
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://www.smythstoys.com/ie/en-ie/store-pickup/192012/pointOfServices"))
                {
                    request.Headers.TryAddWithoutValidation("authority", "www.smythstoys.com");
                    request.Headers.TryAddWithoutValidation("accept", "*/*");
                    request.Headers.TryAddWithoutValidation("x-requested-with", "XMLHttpRequest");
                    request.Headers.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.67 Safari/537.36 Edg/87.0.664.47");
                    request.Headers.TryAddWithoutValidation("origin", "https://www.smythstoys.com");
                    request.Headers.TryAddWithoutValidation("sec-fetch-site", "same-origin");
                    request.Headers.TryAddWithoutValidation("sec-fetch-mode", "cors");
                    request.Headers.TryAddWithoutValidation("sec-fetch-dest", "empty");
                    request.Headers.TryAddWithoutValidation("referer", "https://www.smythstoys.com/ie/en-ie/video-games-and-tablets/xbox-gaming/xbox-series-x-%7C-s/xbox-series-x-%7C-s-consoles/xbox-series-x-1tb-console/p/192012");
                    request.Headers.TryAddWithoutValidation("accept-language", "en-US,en;q=0.9");
                    request.Headers.TryAddWithoutValidation("cookie", "siteVisited=false; _gcl_au=1.1.259245703.1605042265; _fbp=fb.1.1605042265138.496314202; _hjid=fa75d4a8-a39f-44a1-9e3d-f0b5070d86f3; _gid=GA1.2.2089821112.1606748987; locationCookie=_; BVBRANDID=735759e1-745b-4a16-9d6a-5a56042e2677; recentlyBrowsedProducts=192012; GCLB=CKOYtqahrKGmSg; flixgvid=flix5f0f38c9000000.63592246; inptime0_1764_en=0; _hjTLDTest=1; JSESSIONID=92A338BC9AB1EAC748F338363DEB20E8.app8; stc111380=env:1606756445%7C20201231171405%7C20201130174405%7C1%7C1011422:20211130171405|uid:1605042266533.1842698228.938602.111380.536569347.6:20211130171405|srchist:1011421%3A1%3A20201211210426%7C1011420%3A1606748987%3A20201231150947%7C1011422%3A1606756445%3A20201231171405:20211130171405|tsa:1606756445722.1623865629.4061399.4480709603445041.:20201130174405; _dc_gtm_UA-74244151-1=1; _ga_FHGLQ54MHW=GS1.1.1606756444.4.1.1606756445.0; _ga=GA1.1.291993754.1605042265; _gat_UA-74244151-1=1; _hjAbsoluteSessionInProgress=0; _hjIncludedInSessionSample=0");

                    request.Content = new StringContent("cartPage=false&entryNumber=0&latitude=&longitude=&searchThroughGeoPointFirst=false&xaaLandingStores=false&CSRFToken=" + CSRFToken);
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded; charset=UTF-8");
                    try
                    {
                        var response = httpClient.SendAsync(request).Result;

                        JObject s = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                        var returnString = "";
                        foreach (var jToken in s.First.First)
                        {
                            if (Int32.Parse(jToken["stockLevel"].ToString()) > 0)
                            {
                                returnString += jToken["displayName"] + ": " + jToken["stockLevel"] + "\n";
                            }
                        }
                        if (!string.IsNullOrEmpty(returnString))
                        {
                            SendPush(expoPushToken, returnString, "AVAILABLE");
                        }
                    }
                    catch
                    {
                        SendPush(expoPushToken, "Change CSRF", "Change CSRF");
                    }
                }
            }

        }

        private static void SendPush(string expoPushToken, string innerText, string label)
        {
            var expoSDKClient = new PushApiClient();
            var pushTicketReq = new PushTicketRequest()
            {
                PushTo = new List<string>() { expoPushToken },
                //PushBadgeCount = 7,
                PushTitle = label,
                PushBody = innerText
            };
            var result = expoSDKClient.PushSendAsync(pushTicketReq).GetAwaiter().GetResult();

            if (result?.PushTicketErrors?.Count() > 0)
            {
                foreach (var error in result.PushTicketErrors)
                {
                    Console.WriteLine($"Error: {error.ErrorCode} - {error.ErrorMessage}");
                }
            }
        }
    }
}