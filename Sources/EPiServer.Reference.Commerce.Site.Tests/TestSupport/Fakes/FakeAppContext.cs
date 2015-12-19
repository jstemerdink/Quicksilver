using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using System;

using EPiServer.Reference.Commerce.Domain.Facades;

namespace EPiServer.Reference.Commerce.Site.Tests.TestSupport.Fakes
{
    public class FakeAppContext : AppContextFacade
    {
        public override Guid ApplicationId
        {
            get { return Guid.Empty; }
        }
    }
}
