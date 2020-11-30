using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Expo.Server.Client;
using Expo.Server.Models;
using Newtonsoft.Json.Linq;

namespace CS_Italy_Observer
{
    public class XboxSmythWatcher
    {
        public static void Main(string[] args)
        {
            var handler = new HttpClientHandler();

            string expoPushToken = "ExponentPushToken[AICngyBJmFOCmWFqQlemgu]";

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

                    request.Content = new StringContent("cartPage=false&entryNumber=0&latitude=&longitude=&searchThroughGeoPointFirst=false&xaaLandingStores=false&CSRFToken=c370cb1e-19b3-4082-82c0-0d85d88bbc0a");
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded; charset=UTF-8"); 
                    
                    var response = httpClient.SendAsync(request).Result;
             
                    JObject s = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    var stockString = "";
                    foreach (var jToken in s.First.First)
                    {
                        if (Int32.Parse(jToken["stockLevel"].ToString()) > 0)
                        {
                            stockString += jToken["displayName"] + ": " + jToken["stockLevel"] + "\n";
                        }
                    }

                    Console.WriteLine(stockString);
                    SendPush(expoPushToken, stockString);

                }
            }

		
        }

        private static void SendPush(string expoPushToken, string innerText)
        {
            var expoSDKClient = new PushApiClient();
            var pushTicketReq = new PushTicketRequest()
            {
                PushTo = new List<string>() { expoPushToken },
                //PushBadgeCount = 7,
                PushTitle = "Playable map detected",
                PushBody = "cs_italy: " + innerText
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