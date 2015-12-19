using System;

using Mediachase.Commerce.Core;

namespace EPiServer.Reference.Commerce.Domain.Facades
{
    public class AppContextFacade
    {
        public virtual Guid ApplicationId
        {
            get { return AppContext.Current.ApplicationId; }
        }
    }
}