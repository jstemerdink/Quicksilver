using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

using EPiServer.Commerce.Catalog.Linking;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;

using Mediachase.Commerce.Catalog;

namespace EPiServer.Reference.Commerce.Site.Features.Cart.Extensions
{
    public class CartHelper
    {
        private readonly IContentLoader _contentLoader;
        private readonly LinksRepository _linksRepository;
        private readonly IRelationRepository _relationRepository;
        private readonly CultureInfo _preferredCulture;
        private readonly ReferenceConverter _referenceConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public CartHelper(IContentLoader contentLoader, LinksRepository linksRepository, IRelationRepository relationRepository, Func<CultureInfo> preferredCulture, ReferenceConverter referenceConverter)
        {
            this._contentLoader = contentLoader;
            this._linksRepository = linksRepository;
            this._relationRepository = relationRepository;
            this._preferredCulture = preferredCulture();
            this._referenceConverter = referenceConverter;
        }

        public string GetSiblingVariantCodeBySize(string siblingCode, string size)
        {
            ContentReference variationReference = _referenceConverter.GetContentLink(siblingCode);
            IEnumerable<Relation> productRelations = _linksRepository.GetRelationsByTarget(variationReference).ToList();
            IEnumerable<ProductVariation> siblingsRelations = _relationRepository.GetRelationsBySource<ProductVariation>(productRelations.First().Source);
            IEnumerable<ContentReference> siblingsReferences = siblingsRelations.Select(x => x.Target);
            IEnumerable<IContent> siblingVariations = _contentLoader.GetItems(siblingsReferences, _preferredCulture);

            var siblingVariant = siblingVariations.OfType<FashionVariant>().FirstOrDefault(x => x.Code == siblingCode);

            foreach (var variant in siblingVariations.OfType<FashionVariant>())
            {
                if (variant.Size == size && variant.Code != siblingCode && variant.Color == siblingVariant.Color)
                {
                    return variant.Code;
                }
            }

            return null;
        }
    }
}