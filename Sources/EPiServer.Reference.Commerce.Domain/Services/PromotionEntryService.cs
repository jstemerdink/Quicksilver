﻿using System;
using System.Linq;

using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.Linking;
using EPiServer.Reference.Commerce.Domain.Contracts.Services;
using EPiServer.Reference.Commerce.Domain.Facades;
using EPiServer.ServiceLocation;

using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Inventory;
using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Marketing.Objects;
using Mediachase.Commerce.Pricing;

namespace EPiServer.Reference.Commerce.Domain.Services
{
   public abstract class PromotionEntryService : IPromotionEntryService
    {
        protected readonly ILinksRepository _linksRepository;
        protected readonly ICatalogSystem _catalogSystem;
        protected readonly IContentLoader _contentLoader;
        protected readonly IWarehouseInventoryService _inventoryService;
        protected readonly IWarehouseRepository _warehouseRepository;

       protected PromotionEntryService(
            ILinksRepository linksRepository,
            ICatalogSystem catalogSystem,
            IContentLoader contentLoader,
            IWarehouseInventoryService inventoryService,
            IWarehouseRepository warehouseRepository)
        {
            this._contentLoader = contentLoader;
            this._linksRepository = linksRepository;
            this._catalogSystem = catalogSystem;
            this._inventoryService = inventoryService;
            this._warehouseRepository = warehouseRepository;
        }

        public virtual IPriceValue GetDiscountPrice(IPriceValue price, EntryContentBase entry, Currency currency, PromotionHelperFacade promotionHelper)
        {
            var promotionEntry = this.CreatePromotionEntry(entry, price);
            var filter = new PromotionFilter
            {
                IgnoreConditions = false,
                IgnorePolicy = false,
                IgnoreSegments = false,
                IncludeCoupons = false
            };

            var sourceSet = new PromotionEntriesSet();
            sourceSet.Entries.Add(promotionEntry);
            promotionHelper.PromotionContext.SourceEntriesSet = sourceSet;
            promotionHelper.PromotionContext.TargetEntriesSet = sourceSet;
            promotionHelper.Evaluate(filter, false);

            if (promotionHelper.PromotionContext.PromotionResult.PromotionRecords.Count > 0)
            {
                return new PriceValue
                {
                    CatalogKey = price.CatalogKey,
                    CustomerPricing = CustomerPricing.AllCustomers,
                    MarketId = price.MarketId,
                    MinQuantity = 1,
                    UnitPrice = new Money(price.UnitPrice.Amount - GetDiscountPrice(promotionHelper), currency),
                    ValidFrom = DateTime.UtcNow,
                    ValidUntil = null
                };
            }
            return price;
        }

        private PromotionEntry CreatePromotionEntry(EntryContentBase entry, IPriceValue price)
        {
            var catalogNodes = string.Empty;
            var catalogs = string.Empty;
            foreach (var node in entry.GetNodeRelations(this._linksRepository).Select(x => this._contentLoader.Get<NodeContent>(x.Target)))
            {
                var entryCatalogName = this._catalogSystem.GetCatalogDto(node.CatalogId).Catalog[0].Name;
                catalogs = string.IsNullOrEmpty(catalogs) ? entryCatalogName : catalogs + ";" + entryCatalogName;
                catalogNodes = string.IsNullOrEmpty(catalogNodes) ? node.Code : catalogNodes + ";" + node.Code;
            }
            var promotionEntry = new PromotionEntry(catalogs, catalogNodes, entry.Code, price.UnitPrice.Amount);
            this.Populate(promotionEntry, entry, price);
            return promotionEntry;
        }

        private static decimal GetDiscountPrice(PromotionHelperFacade promotionHelper)
        {
            var result = promotionHelper.PromotionContext.PromotionResult;
            return result.PromotionRecords.Sum(record => GetDiscountAmount(record, record.PromotionReward));
        }

        private static decimal GetDiscountAmount(PromotionItemRecord record, PromotionReward reward)
        {
            decimal discountAmount = 0;
            if (reward.RewardType != PromotionRewardType.EachAffectedEntry && reward.RewardType != PromotionRewardType.AllAffectedEntries)
            {
                return Math.Round(discountAmount, 2);
            }
            if (reward.AmountType == PromotionRewardAmountType.Percentage)
            {
                discountAmount = record.AffectedEntriesSet.TotalCost * reward.AmountOff / 100;
            }
            else
            {
                discountAmount += reward.AmountOff;
            }
            return Math.Round(discountAmount, 2);
        }

        private void Populate(PromotionEntry entry, EntryContentBase catalogEntry, IPriceValue price)
        {
            entry.Quantity = 1;
            entry.Owner = catalogEntry;
            entry["Id"] = catalogEntry.Code;

            if (catalogEntry.Property != null)
            {
                foreach (var prop in catalogEntry.Property.Where(x => x.IsPropertyData))
                {
                    entry[prop.Name] = prop.Value;
                }
            }

            entry["ExtendedPrice"] = price.UnitPrice.Amount;
            var inventories = this._inventoryService.List(price.CatalogKey, this._warehouseRepository.List()).ToList();
            if (!inventories.Any())
            {
                return;
            }

            entry["AllowBackordersAndPreorders"] = inventories.Any(i => i.AllowBackorder) && inventories.Any(i => i.AllowPreorder);
            entry["InStockQuantity"] = inventories.Sum(i => i.InStockQuantity - i.ReservedQuantity);
            entry["PreorderQuantity"] = inventories.Sum(i => i.PreorderQuantity);
            entry["BackorderQuantity"] = inventories.Sum(i => i.BackorderQuantity);
            entry["InventoryStatus"] = inventories.First().InventoryStatus;
        }
    }
}