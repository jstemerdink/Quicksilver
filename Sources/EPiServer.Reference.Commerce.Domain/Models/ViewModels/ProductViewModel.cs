using EPiServer.Reference.Commerce.Domain.Contracts.Models;

using Mediachase.Commerce;

namespace EPiServer.Reference.Commerce.Domain.Models.ViewModels
{
    public class ProductViewModel : IProductModel
    {
        public string Code { get; set; }

        public string DisplayName { get; set; }

        public Money ExtendedPrice { get; set; }

        public string ImageUrl { get; set; }

        public Money PlacedPrice { get; set; }

        public string Url { get; set; }
    }
}
