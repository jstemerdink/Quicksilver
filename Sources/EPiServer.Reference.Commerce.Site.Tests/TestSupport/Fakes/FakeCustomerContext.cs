using EPiServer.Reference.Commerce.Domain.Facades;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;

namespace EPiServer.Reference.Commerce.Site.Tests.TestSupport.Fakes
{
    class FakeCustomerContext : CustomerContextFacade
    {
        private readonly CurrentContactFacade _currentContact;

        public FakeCustomerContext(CurrentContactFacade currentContact)
        {
            _currentContact = currentContact;
        }

        public new CurrentContactFacade CurrentContact 
        {
            get { return _currentContact; }
        }
    }
}