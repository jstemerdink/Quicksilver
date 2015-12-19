using EPiServer.Reference.Commerce.Domain.Contracts.Models;
using Mediachase.Commerce;

namespace EPiServer.Reference.Commerce.Site.Features.Product.Models
{
    public class ProductViewModel : Domain.Models.ViewModels.ProductViewModel
    {
        public string Brand { get; set; }
    }
}