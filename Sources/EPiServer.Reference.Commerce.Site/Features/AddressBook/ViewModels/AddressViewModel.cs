using EPiServer.Reference.Commerce.Domain.Contracts.Models;
using EPiServer.Reference.Commerce.Domain.Models;
using EPiServer.Reference.Commerce.Domain.Models.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.AddressBook.Pages;

namespace EPiServer.Reference.Commerce.Site.Features.AddressBook.ViewModels
{
    public class AddressViewModel : PageViewModel<AddressBookPage>
    {
        public IAddress Address { get; set; }
    }
}