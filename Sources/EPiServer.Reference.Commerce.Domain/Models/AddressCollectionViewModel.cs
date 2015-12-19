using System.Collections.Generic;

using EPiServer.Core;
using EPiServer.Reference.Commerce.Domain.Contracts.Models;

namespace EPiServer.Reference.Commerce.Domain.Models
{
    public class AddressCollectionViewModel : IPageViewModel<IContent>
    {
        public IEnumerable<IAddress> Addresses { get; set; }

        public IContent CurrentPage { get; set; }
    }
}