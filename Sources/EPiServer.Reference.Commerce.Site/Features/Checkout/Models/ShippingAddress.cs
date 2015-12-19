using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using EPiServer.Reference.Commerce.Domain.Models;

namespace EPiServer.Reference.Commerce.Site.Features.Checkout.Models
{
    public class ShippingAddress : Address
    {
        public Guid ShippingMethodId { get; set; }
    }
}