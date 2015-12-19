using Mediachase.Commerce;

namespace EPiServer.Reference.Commerce.Domain.Contracts.Models
{
    public interface IProductModel
    {
        string Code { get; set; }
        string DisplayName { get; set; }
        Money ExtendedPrice { get; set; }
        string ImageUrl { get; set; }
        Money PlacedPrice { get; set; }
        string Url { get; set; }
    }
}
