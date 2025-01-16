public class CurrencyService
{
    private readonly Dictionary<string, decimal> _conversionRates = new()
    {
        { "USD", 0.0072m }, // 1 NPR = 0.0072 USD
        { "NPR", 1m }       // NPR is the default base currency
    };

    private readonly Dictionary<string, string> _currencySymbols = new()
    {
        { "NPR", "₨" },
        { "USD", "$" }
    };

    // Default currency set to NPR
    public string SelectedCurrency { get; private set; } = "NPR";
    public string SelectedCurrencySymbol => _currencySymbols.ContainsKey(SelectedCurrency)
        ? _currencySymbols[SelectedCurrency]
        : string.Empty;

    // Conversion logic
    public decimal GetConversionRate(string currency)
    {
        return _conversionRates.ContainsKey(currency) ? _conversionRates[currency] : 1.0m;
    }

    public void SetCurrency(string currency)
    {
        if (_conversionRates.ContainsKey(currency))
        {
            SelectedCurrency = currency;
        }
    }
}
