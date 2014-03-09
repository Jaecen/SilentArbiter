using System.Collections.Generic;

namespace ExchangeService.Market
{
	public class OrderDepth
	{
		public readonly CurrencyPair Pair;
		public readonly Dictionary<decimal, decimal> Buys;
		public readonly Dictionary<decimal, decimal> Sells;

		public OrderDepth(CurrencyPair pair, Dictionary<decimal, decimal> buys, Dictionary<decimal, decimal> sells)
		{
			Pair = pair;
			Buys = buys;
			Sells = sells;
		}
	}
}