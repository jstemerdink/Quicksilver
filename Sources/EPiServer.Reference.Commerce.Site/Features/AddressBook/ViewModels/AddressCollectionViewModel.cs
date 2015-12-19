using EPiServer.Reference.Commerce.Site.Features.AddressBook.Pages;
using System.Collections.Generic;

using EPiServer.Reference.Commerce.Domain.Contracts.Models;
using EPiServer.Reference.Commerce.Domain.Models.ViewModels;

namespace EPiServer.Reference.Commerce.Site.Features.AddressBook.ViewModels
{
    public class AddressCollectionViewModel : PageViewModel<AddressBookPage>
    {
        public IEnumerable<IAddress> Addresses { get; set; }
    }
}