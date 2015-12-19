using System.Collections.Generic;

using EPiServer.Reference.Commerce.Domain.Contracts.Models;

using Mediachase.Commerce;

namespace EPiServer.Reference.Commerce.Domain.Models.ViewModels
{
    public abstract class CartViewModel
    {
        public decimal ItemCount { get; set; }
        public virtual IEnumerable<ICartItem> CartItems { get; set; }
        public Money Total { get; set; }
    }
}