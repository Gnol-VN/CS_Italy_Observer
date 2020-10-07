using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CS_Italy_Observer
{
    class Program
    {
        static void Main(string[] args)
        {
            string mapName = "cs_assault";
            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument document = htmlWeb.Load("https://cs-online.club/en/servers");
            //List<HtmlNode>  cssSelectorNodes = document.DocumentNode.QuerySelectorAll("div").ToList();
            List <HtmlNode> hyperlinkList = document.DocumentNode.SelectNodes("(//text()[contains(., '"+ mapName + "')]/../../../td[@class='num_cl']/*[@class='online'])").ToList();
            foreach (var item in hyperlinkList)
            {
                if (Int32.Parse(item.InnerText) > 10)
                {
                    Console.WriteLine(Int32.Parse(item.InnerText));
                }
            }
        }
    }
}
