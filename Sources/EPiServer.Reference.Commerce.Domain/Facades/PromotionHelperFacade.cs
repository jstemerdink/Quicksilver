using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Website.Helpers;

namespace EPiServer.Reference.Commerce.Domain.Facades
{
    public class PromotionHelperFacade
    {
        private PromotionHelper _helper;

        public virtual PromotionContext PromotionContext
        {
            get { return this.GetPromotionHelper().PromotionContext; }
        }

        public virtual void Evaluate(PromotionFilter filter, bool checkEntryLevelLimit)
        {
            this.GetPromotionHelper().Eval(filter, checkEntryLevelLimit);
        }

        public virtual void Reset()
        {
            this._helper = new PromotionHelper();
            this._helper.PromotionContext.TargetGroup = PromotionGroup.GetPromotionGroup(PromotionGroup.PromotionGroupKey.Entry).Key;
        }

        private PromotionHelper GetPromotionHelper()
        {
            if (this._helper == null)
            {
                this.Reset();
            }
            return this._helper;
        }
    }
}