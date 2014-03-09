using System;
using System.Collections.Generic;
using System.Linq;
using ExchangeService.Market;
using HtmlAgilityPack;

namespace ExchangeService.Exchange
{
	public class CryptoRush : IExchange
	{
		readonly ApiUrlBuilder ApiUrl;
		readonly HtmlWeb HtmlWeb;

		public CryptoRush()
		{
			ApiUrl = new ApiUrlBuilder();
			HtmlWeb = new HtmlWeb();
		}

		// CryptoRush doesn't have an order book API, so we just scrape a page.
		public OrderDepth GetMarketOrderDepth(CurrencyPair pair)
		{
			if(pair == null)
				throw new ArgumentNullException("pair");

			var requestUrl = ApiUrl.OrderBook(pair);

			var doc = HtmlWeb.Load(requestUrl.ToString());
			var buys = ParseOrderDepthFromResult(doc, "buyOrder");
			var sells = ParseOrderDepthFromResult(doc, "sellOrder");

			return new OrderDepth(pair, buys, sells);
		}

		Dictionary<decimal, decimal> ParseOrderDepthFromResult(HtmlDocument doc, string orderType)
		{
			return doc
				.DocumentNode
				.SelectNodes(String.Format("//tr[@class='orderitem' and starts-with(@onclick, '{0}')]", orderType))
				.Select(tr => tr
					.ChildNodes
					.Where(node => node.Name == "td")
					.Select(td => td.InnerText.Trim())
					.ToArray()
				)
				.ToDictionary(
					values => Decimal.Parse(values[0]),
					values => Decimal.Parse(values[1])
				);
		}

		class ApiUrlBuilder
		{
			public Uri OrderBook(CurrencyPair pair)
			{
				return new Uri(String.Format("https://www.cryptorush.in/index.php?p=trading&m={0}&b={1}", pair.Base, pair.Counter));
			}
		}
	}
}