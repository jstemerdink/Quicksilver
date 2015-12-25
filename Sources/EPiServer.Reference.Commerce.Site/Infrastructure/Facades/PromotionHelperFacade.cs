using System.Linq;

using EPiServer.ServiceLocation;
using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Website.Helpers;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Facades
{
    [ServiceConfiguration(typeof(PromotionHelperFacade), Lifecycle = ServiceInstanceScope.HybridHttpSession)]
    public class PromotionHelperFacade
    {
        private PromotionHelper _helper;

        public virtual PromotionContext PromotionContext
        {
            get { return GetPromotionHelper().PromotionContext; }
        }

        public virtual void Evaluate(PromotionFilter filter, bool checkEntryLevelLimit)
        {
            GetPromotionHelper().Eval(filter, checkEntryLevelLimit);
        }

        public virtual void Reset()
        {
            _helper = new PromotionHelper();
            _helper.PromotionContext.TargetGroup = PromotionGroup.GetPromotionGroup(PromotionGroup.PromotionGroupKey.Entry).Key;
        }

        private PromotionHelper GetPromotionHelper()
        {
            if (_helper == null)
            {
                Reset();
            }
            return _helper;
        }

        public virtual void AddCouponsToMarketingContext()
        {
            // Add coupons in the promotion context to the marketing context 
            foreach (string couponCode in this.PromotionContext.Coupons.Where(couponCode => !string.IsNullOrEmpty(couponCode)))
            {
                MarketingContext.Current.AddCouponToMarketingContext(couponCode);
            }
        }
    }
}