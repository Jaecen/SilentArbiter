using System;
using System.Collections.Generic;
using System.Linq;
using ExchangeService.Market;
using HtmlAgilityPack;

namespace ExchangeService.Exchange
{
	public class Mintpal : IExchange
	{
		readonly ApiUrlBuilder ApiUrl;
		readonly HtmlWeb HtmlWeb;

		public Mintpal()
		{
			ApiUrl = new ApiUrlBuilder();
			HtmlWeb = new HtmlWeb();
		}

		// MintPal doesn't have an order book API, so we just scrape a page.
		public OrderDepth GetMarketOrderDepth(CurrencyPair pair)
		{
			if(pair == null)
				throw new ArgumentNullException("pair");

			var requestUrl = ApiUrl.OrderBook(pair);

			var doc = HtmlWeb.Load(requestUrl.ToString());

			var buyTable = doc.GetElementbyId("buyTable");
			var buys = ParseOrderDepthFromResult(buyTable);

			var sellTable = doc.GetElementbyId("sellTable");
			var sells = ParseOrderDepthFromResult(sellTable);

			return new OrderDepth(pair, buys, sells);
		}

		Dictionary<decimal, decimal> ParseOrderDepthFromResult(HtmlNode table)
		{
			return table
				.SelectNodes("tr[position() > 1]")	// Skip the header row
				.ToDictionary(
					tr => Decimal.Parse(tr.ChildNodes[0].InnerText),
					tr => Decimal.Parse(tr.ChildNodes[1].InnerText)
				);
		}

		class ApiUrlBuilder
		{
			readonly Uri ApiBase;

			public ApiUrlBuilder()
			{
				ApiBase = new Uri("https://api.mintpal.com/market/");
			}

			public Uri OrderBook(CurrencyPair pair)
			{
				return new Uri(String.Format("http://www.mintpal.com/market/{0}/{1}", pair.Base, pair.Counter));
			}

			public Uri RecentTrades(CurrencyPair pair)
			{
				return new Uri(ApiBase, String.Format("trades/{0}/{1}", pair.Base, pair.Counter));
			}
		}
	}
}