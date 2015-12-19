using EPiServer.Core;
using EPiServer.Reference.Commerce.Domain.Contracts.Models;

namespace EPiServer.Reference.Commerce.Domain.Models.ViewModels
{
    public class PageViewModel<T> : IPageViewModel<T>
        where T : IContent
    {
        public T CurrentPage { get; set; }
    }
}