using EPiServer.Reference.Commerce.Site.Features.OrderHistory.Pages;
using System.Collections.Generic;

using EPiServer.Reference.Commerce.Domain.Models.ViewModels;

namespace EPiServer.Reference.Commerce.Site.Features.OrderHistory.Models
{
    public class OrderHistoryViewModel : PageViewModel<OrderHistoryPage>
    {
        public List<Order> Orders { get; set; }
    }
}