using System;

namespace ExchangeService.Market
{
	public class CurrencyPair : IEquatable<CurrencyPair>
	{
		public static CurrencyPair DrkBtc = new CurrencyPair(Currency.Darkcoin, Currency.Bitcoin);
		public static CurrencyPair UtcBtc = new CurrencyPair(Currency.Ultracoin, Currency.Bitcoin);

		public readonly Currency Base;
		public readonly Currency Counter;

		public CurrencyPair(Currency @base, Currency counter)
		{
			if(@base == null)
				throw new ArgumentNullException("base");

			if(counter == null)
				throw new ArgumentNullException("counter");

			Base = @base;
			Counter = counter;
		}

		public bool Equals(CurrencyPair other)
		{
			if(other == null)
				return false;

			return Base == other.Base && Counter == other.Counter;
		}

		public override bool Equals(object obj)
		{
			if(obj is CurrencyPair)
				return Equals((CurrencyPair)obj);

			return false;
		}

		public override int GetHashCode()
		{
			return Base.GetHashCode() ^ Counter.GetHashCode();
		}

		public static bool operator ==(CurrencyPair x, CurrencyPair y)
		{
			if(Object.ReferenceEquals(x, null))
				return Object.ReferenceEquals(y, null);

			return x.Equals(y);
		}

		public static bool operator !=(CurrencyPair x, CurrencyPair y)
		{
			return !(x == y);
		}

		public override string ToString()
		{
			return String.Format("{0}/{1}", Base, Counter);
		}
	}
}