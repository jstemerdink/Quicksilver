using EPiServer.Core;

namespace EPiServer.Reference.Commerce.Domain.Contracts.Models
{
    public interface IPageViewModel<T>
        where T : IContent
    {
        T CurrentPage { get; set; }
    }
}