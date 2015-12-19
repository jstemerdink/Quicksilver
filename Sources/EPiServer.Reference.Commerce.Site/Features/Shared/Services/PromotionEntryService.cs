using System;
using System.Linq;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.Linking;
using EPiServer.Reference.Commerce.Domain.Contracts.Services;
using EPiServer.Reference.Commerce.Domain.Facades;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using EPiServer.ServiceLocation;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Inventory;
using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Marketing.Objects;
using Mediachase.Commerce.Pricing;

namespace EPiServer.Reference.Commerce.Site.Features.Shared.Services
{
    [ServiceConfiguration(typeof(IPromotionEntryService), Lifecycle = ServiceInstanceScope.Singleton)]
    public class PromotionEntryService : Domain.Services.PromotionEntryService
    {
        public PromotionEntryService(ILinksRepository linksRepository, ICatalogSystem catalogSystem, IContentLoader contentLoader, IWarehouseInventoryService inventoryService, IWarehouseRepository warehouseRepository)
            : base(linksRepository, catalogSystem, contentLoader, inventoryService, warehouseRepository)
        {
        }
    }
}