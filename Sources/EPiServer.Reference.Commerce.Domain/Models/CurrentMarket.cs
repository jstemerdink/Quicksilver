using EPiServer.Reference.Commerce.Domain.Services;

using Mediachase.Commerce;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Markets;

namespace EPiServer.Reference.Commerce.Domain.Models
{
    public class CurrentMarket : ICurrentMarket
    {
        private const string MarketCookie = "MarketId";
        private static readonly MarketId DefaultMarketId = new MarketId("DEFAULT");
        private readonly IMarketService _marketService;
        private readonly CookieService _cookieService;

        public CurrentMarket(IMarketService marketService, CookieService cookieService)
        {
            this._marketService = marketService;
            this._cookieService = cookieService;
        }

        public virtual IMarket GetCurrentMarket()
        {
            var market = this._cookieService.Get(MarketCookie);
            if (string.IsNullOrEmpty(market))
            {
                market = DefaultMarketId.Value;
            }
            return this.GetMarket(new MarketId(market));
        }

        public virtual void SetCurrentMarket(MarketId marketId)
        {
            var market = this.GetMarket(marketId);
            SiteContext.Current.Currency = market.DefaultCurrency;
            this._cookieService.Set(MarketCookie, marketId.Value);
        }

        private IMarket GetMarket(MarketId marketId)
        {
            return this._marketService.GetMarket(marketId) ?? this._marketService.GetMarket(DefaultMarketId);
        }
    }
}