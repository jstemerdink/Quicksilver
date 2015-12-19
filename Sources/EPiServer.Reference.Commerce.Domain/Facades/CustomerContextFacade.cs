using System;

using EPiServer.ServiceLocation;

using Mediachase.Commerce.Customers;

namespace EPiServer.Reference.Commerce.Domain.Facades
{
    public class CustomerContextFacade
    {
        public Injected<CurrentContactFacade> CurrentContact { get; private set; }
        public virtual Guid CurrentContactId { get { return CustomerContext.Current.CurrentContactId;} }
        public virtual CustomerContact GetContactById(Guid contactId)
        {
            return CustomerContext.Current.GetContactById(contactId);
        }
    }
}