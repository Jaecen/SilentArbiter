using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExchangeService.Market
{
	[TypeConverter(typeof(CurrencyTypeConverter))]
	public class Currency : IEquatable<Currency>
	{
		public static readonly Currency Bitcoin;
		public static readonly Currency Darkcoin;
		public static readonly Currency Ultracoin;
		public static readonly IEnumerable<Currency> Known;

		static Currency()
		{
			Bitcoin = new Currency("Bitcoin", "BTC");
			Darkcoin = new Currency("Darkcoin", "DRK");
			Ultracoin = new Currency("Ultracoin", "UTC");

			Known = new[]
			{
				Bitcoin,
				Darkcoin,
				Ultracoin,
			};
		}

		public readonly string Name;
		public readonly string Code;

		public Currency(string name, string code)
		{
			if(name == null)
				throw new ArgumentNullException("name");

			if(code == null)
				throw new ArgumentNullException("code");

			Name = name;
			Code = code;
		}

		public bool Equals(Currency other)
		{
			if(other == null)
				return false;

			if(Object.ReferenceEquals(this, other))
				return true;

			return StringComparer.OrdinalIgnoreCase.Equals(Code, other.Code);
		}

		public override bool Equals(object obj)
		{
			if(obj is Currency)
				return Equals((Currency)obj);

			return false;
		}

		public override int GetHashCode()
		{
			return Code.GetHashCode();
		}

		public static bool operator ==(Currency x, Currency y)
		{
			if(Object.ReferenceEquals(x, null))
				return Object.ReferenceEquals(y, null);

			return x.Equals(y);
		}

		public static bool operator !=(Currency x, Currency y)
		{
			return !(x == y);
		}

		public override string ToString()
		{
			return Code;
		}
	}

	public class CurrencyTypeConverter : TypeConverter
	{
		readonly Dictionary<string, Currency> CurrencyCodeMappings;

		public CurrencyTypeConverter()
		{
			CurrencyCodeMappings = Currency
				.Known
				.ToDictionary(currency => currency.Code, StringComparer.OrdinalIgnoreCase);
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if(sourceType == typeof(String))
				return true;

			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if(value is String && CurrencyCodeMappings.ContainsKey((string)value))
				return CurrencyCodeMappings[(string)value];

			return base.ConvertFrom(context, culture, value);
		}
	}
}