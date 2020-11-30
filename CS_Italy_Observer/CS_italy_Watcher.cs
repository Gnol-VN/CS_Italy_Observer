using Expo.Server.Client;
using Expo.Server.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;


namespace CS_Italy_Observer
{
    class CS_italy_Watcher
    {
        static void Main(string[] args)
        {
            string mapName = "cs_italy";
            string expoPushToken = "ExponentPushToken[AICngyBJmFOCmWFqQlemgu]";

            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument document = htmlWeb.Load("https://cs-online.club/en/servers");
            //List<HtmlNode>  cssSelectorNodes = document.DocumentNode.QuerySelectorAll("div").ToList();
            List <HtmlNode> hyperlinkList = document.DocumentNode.SelectNodes("(//text()[contains(., '"+ mapName + "')]/../../../td[@class='num_cl']/*[@class='online'])").ToList();
            foreach (var item in hyperlinkList)
            {
                if (Int32.Parse(item.InnerText) > 10)
                {
                    SendPush(expoPushToken, item.InnerText);
                    Console.WriteLine(Int32.Parse(item.InnerText));
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
