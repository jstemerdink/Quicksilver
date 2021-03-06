﻿using System.Web.Mvc;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Start.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Start.Pages;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;

namespace EPiServer.Reference.Commerce.Site.Features.Start.Controllers
{
    public class HeadController : ActionControllerBase
    {
        private readonly IContentLoader _contentLoader;
        private readonly ContentRouteHelper _contentRouteHelper;
        private const string FormatPlaceholder = "{title}";

        public HeadController(IContentLoader contentLoader, ContentRouteHelper contentRouteHelper)
        {
            _contentLoader = contentLoader;
            _contentRouteHelper = contentRouteHelper;
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Title()
        {
            var content = _contentRouteHelper.Content;
            if (content == null)
            {
                return Content("");
            }

            var product = content as EntryContentBase;
            if (product != null)
            {
                // Note: If this product is placed in more than one category, we might pick the wrong category here
                var parentContent = _contentLoader.Get<CatalogContentBase>(content.ParentLink);
                
                var node = parentContent as NodeContent;
                string title = null;
                if (node != null)
                {
                    title = node.SeoInformation.Title.NullIfEmpty() ?? node.DisplayName;
                }
                else
                {
                    title = parentContent.Name;
                }
                return Content(FormatTitle(string.Format("{0} - {1}", product.SeoInformation.Title.NullIfEmpty() ?? product.DisplayName, title)));
            }

            var category = content as NodeContent;
            if (category != null)
            {
                return Content(FormatTitle(category.SeoInformation.Title.NullIfEmpty() ?? category.DisplayName));
            }

            var startPage = content as StartPage;
            if (startPage != null)
            {
                return Content(startPage.Title.NullIfEmpty() ?? startPage.Name);
            }

            return Content(content.Name);
        }

        private string FormatTitle(string title)
        {
            var format = _contentLoader.Get<StartPage>(ContentReference.StartPage).TitleFormat;
            if (string.IsNullOrWhiteSpace(format) || !format.Contains(FormatPlaceholder))
            {
                return title;
            }
            return format.Replace(FormatPlaceholder, title);
        }
    }
}