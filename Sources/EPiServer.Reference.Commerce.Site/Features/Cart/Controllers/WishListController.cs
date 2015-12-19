using System;

using EPiServer.Core;
using EPiServer.Framework.Localization;
using EPiServer.Reference.Commerce.Site.Features.Cart.Models;
using EPiServer.Reference.Commerce.Site.Features.Cart.Pages;
using EPiServer.Reference.Commerce.Site.Features.Cart.Services;
using EPiServer.Reference.Commerce.Site.Features.Start.Pages;
using EPiServer.Web.Mvc;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

using EPiServer.Commerce.Catalog.Linking;
using EPiServer.Reference.Commerce.Domain.Contracts.Services;
using EPiServer.Reference.Commerce.Domain.Facades;
using EPiServer.Reference.Commerce.Site.Features.Cart.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using EPiServer.Web.Routing;

using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;

namespace EPiServer.Reference.Commerce.Site.Features.Cart.Controllers
{
    [Authorize]
    public class WishListController : PageController<WishListPage>
    {
        private readonly IContentLoader _contentLoader;
        private readonly ICartService _cartService;
        private readonly LocalizationService _localizationService;
        private readonly IProductService _productService;

        private readonly LinksRepository _linksRepository;
        private readonly IRelationRepository _relationRepository;
        private readonly CultureInfo _preferredCulture;
        private readonly ReferenceConverter _referenceConverter;

        private readonly CartHelper _cartHelper;

        public WishListController(
            IContentLoader contentLoader,
            ICartService cartService,
            LocalizationService localizationService,
            IProductService productService,
            LinksRepository linksRepository,
            IRelationRepository relationRepository,
            Func<CultureInfo> preferredCulture,
            ReferenceConverter referenceConverter)
        {
            _contentLoader = contentLoader;
            _localizationService = localizationService;
            _cartService = cartService;
            _productService = productService;
            _cartService.InitializeAsWishList();
            _linksRepository = linksRepository;
            _relationRepository = relationRepository;
            _preferredCulture = preferredCulture();
            _referenceConverter = referenceConverter;

            _cartHelper = new CartHelper(contentLoader, linksRepository, relationRepository, preferredCulture, referenceConverter);
        }

        [HttpGet]
        public ActionResult Index(WishListPage currentPage)
        {
            WishListViewModel viewModel = new WishListViewModel
            {
                ItemCount = _cartService.GetLineItemsTotalQuantity(),
                CurrentPage = currentPage,
                CartItems = _cartService.GetCartItems().Cast<CartItem>(),
                Total = _cartService.GetSubTotal()
            };

            return View(viewModel);
        }

        [AcceptVerbs(HttpVerbs.Get|HttpVerbs.Post)]
        public ActionResult WishListMiniCartDetails()
        {
            WishListMiniCartViewModel viewModel = new WishListMiniCartViewModel
            {
                ItemCount = _cartService.GetLineItemsTotalQuantity(),
                WishListPage = _contentLoader.Get<StartPage>(ContentReference.StartPage).WishListPage,
                CartItems = _cartService.GetCartItems().Cast<CartItem>(),
                Total = _cartService.GetTotal()
            };

            return PartialView("_WishListMiniCartDetails", viewModel);
        }

        [HttpPost]
        public ActionResult AddToCart(string code)
        {
            ModelState.Clear();
            string warningMessage = null;

            if (_cartService.AddToCart(code, out warningMessage))
            {
                return WishListMiniCartDetails();
            }

            // HttpStatusMessage can't be longer than 512 characters.
            warningMessage = warningMessage.Length < 512 ? warningMessage : warningMessage.Substring(512);
            return new HttpStatusCodeResult(500, warningMessage);
        }

        [HttpPost]
        public ActionResult ChangeCartItem(string code, decimal quantity, string size, string newSize)
        {
            ModelState.Clear();

            if (quantity > 0)
            {
                if (size == newSize)
                {
                    _cartService.ChangeQuantity(code, quantity);
                }
                else
                {
                    var newCode = _cartHelper.GetSiblingVariantCodeBySize(code, newSize);
                    _cartService.UpdateLineItemSku(code, newCode, quantity);
                }
            }
            else
            {
                _cartService.RemoveLineItem(code);
            }

            return WishListMiniCartDetails();
        }

        [HttpPost]
        public ActionResult DeleteWishList()
        {
            _cartService.DeleteCart();
            var startPage = _contentLoader.Get<StartPage>(ContentReference.StartPage);

            return RedirectToAction("Index", new {Node = startPage.WishListPage});
        }

        
    }


}