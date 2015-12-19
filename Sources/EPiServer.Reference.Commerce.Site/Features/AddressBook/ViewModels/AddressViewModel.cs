using EPiServer.Reference.Commerce.Domain.Contracts.Models;
using EPiServer.Reference.Commerce.Domain.Models;
using EPiServer.Reference.Commerce.Site.Features.AddressBook.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;

namespace EPiServer.Reference.Commerce.Site.Features.AddressBook.ViewModels
{
    public class AddressViewModel : PageViewModel<AddressBookPage>
    {
        public IAddress Address { get; set; }
    }
}