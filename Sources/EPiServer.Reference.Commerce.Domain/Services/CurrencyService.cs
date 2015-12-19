using System.Collections.Generic;
using System.Linq;

using EPiServer.Reference.Commerce.Domain.Contracts.Services;
using EPiServer.ServiceLocation;

using Mediachase.Commerce;

namespace EPiServer.Reference.Commerce.Domain.Services
{
    public abstract class CurrencyService : ICurrencyService
    {
        protected const string CurrencyCookie = "Currency";
        protected readonly ICurrentMarket _currentMarket;
        protected readonly CookieService _cookieService;

        protected CurrencyService(ICurrentMarket currentMarket, CookieService cookieService)
        {
            this._currentMarket = currentMarket;
            this._cookieService = cookieService;
        }

        public virtual IEnumerable<Currency> GetAvailableCurrencies()
        {
            return this.CurrentMarket.Currencies;
        }

        public virtual Currency GetCurrentCurrency()
        {
            Currency currency;
            return this.TryGetCurrency(this._cookieService.Get(CurrencyCookie), out currency) ? 
                currency : 
                this.CurrentMarket.DefaultCurrency;
        }

        public virtual bool SetCurrentCurrency(string currencyCode)
        {
            Currency currency;
            
            if (!this.TryGetCurrency(currencyCode, out currency))
            {
                return false;
            }
                
            this._cookieService.Set(CurrencyCookie, currencyCode);

            return true;
        }

        protected bool TryGetCurrency(string currencyCode, out Currency currency)
        {
            var result = this.GetAvailableCurrencies()
                .Where(x => x.CurrencyCode == currencyCode)
                .Cast<Currency?>()
                .FirstOrDefault();

            if (result.HasValue)
            {
                currency = result.Value;
                return true;
            }

            currency = null;
            return false;
        }

        protected IMarket CurrentMarket
        {
            get { return this._currentMarket.GetCurrentMarket(); }
        }
    }
}