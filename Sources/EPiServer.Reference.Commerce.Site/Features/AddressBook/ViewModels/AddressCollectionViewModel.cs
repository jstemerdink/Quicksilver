using EPiServer.Reference.Commerce.Site.Features.AddressBook.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using System.Collections.Generic;

using EPiServer.Reference.Commerce.Domain.Contracts.Models;

namespace EPiServer.Reference.Commerce.Site.Features.AddressBook.ViewModels
{
    public class AddressCollectionViewModel : PageViewModel<AddressBookPage>
    {
        public IEnumerable<IAddress> Addresses { get; set; }
    }
}