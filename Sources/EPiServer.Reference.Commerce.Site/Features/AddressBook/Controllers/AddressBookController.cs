using EPiServer.Core;
using EPiServer.Framework.Localization;
using EPiServer.Reference.Commerce.Site.Features.AddressBook.Pages;
using EPiServer.Reference.Commerce.Site.Features.AddressBook.Services;
using EPiServer.Reference.Commerce.Site.Features.AddressBook.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.Services;
using EPiServer.Reference.Commerce.Site.Features.Start.Pages;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using System;
using System.Web.Mvc;

using EPiServer.Reference.Commerce.Domain.Contracts.Services;
using EPiServer.Reference.Commerce.Domain.Models;

namespace EPiServer.Reference.Commerce.Site.Features.AddressBook.Controllers
{
    [Authorize]
    public class AddressBookController : PageController<AddressBookPage>
    {
        private readonly IContentLoader _contentLoader;
        private readonly IAddressBookService _addressBookService;
        private readonly LocalizationService _localizationService;
        private readonly ControllerExceptionHandler _controllerExceptionHandler;

        public AddressBookController(
            IContentLoader contentLoader,
            IAddressBookService addressBookService,
            LocalizationService localizationService, 
            ControllerExceptionHandler controllerExceptionHandler)
        {
            _contentLoader = contentLoader;
            _addressBookService = addressBookService;
            _localizationService = localizationService;
            _controllerExceptionHandler = controllerExceptionHandler;
        }

        [HttpGet]
        public ActionResult Index(AddressBookPage currentPage)
        {
            AddressCollectionViewModel viewModel = new AddressCollectionViewModel() { Addresses = _addressBookService.GetAddressBook(), CurrentPage = currentPage};
        

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult EditForm(AddressBookPage currentPage, Guid? addressId)
        {
            AddressViewModel viewModel = new AddressViewModel
            {
                Address = new Address
                {
                    AddressId = addressId,
                    HtmlFieldPrefix = "Address"
                },
                CurrentPage = currentPage 
            };

            _addressBookService.LoadAddress(viewModel.Address);

            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GetRegionsForCountry(string countryCode, string region)
        {
            var address = new Address();
            address.RegionOptions = _addressBookService.GetRegionOptionsByCountryCode(countryCode);
            address.Region = region;

            return PartialView("_AddressRegion", address);
        }

        [HttpPost]
        public ActionResult Save(AddressBookPage currentPage, AddressViewModel viewModel)
        {
            if (String.IsNullOrEmpty(viewModel.Address.Name))
            {
                ModelState.AddModelError("Address.Name", _localizationService.GetString("/Shared/Address/Form/Empty/Name"));
            }

            if (!_addressBookService.CanSave(viewModel.Address))
            {
                ModelState.AddModelError("Address.Name", _localizationService.GetString("/AddressBook/Form/Error/ExistingAddress"));
            }

            if (!ModelState.IsValid)
            {
                _addressBookService.LoadAddress(viewModel.Address);
                viewModel.CurrentPage = currentPage;

                return View("EditForm", viewModel);
            }

            _addressBookService.Save(viewModel.Address);

            return RedirectToAction("Index", new { node = GetStartPage().AddressBookPage });
        }

        [HttpPost]
        public ActionResult Remove(Guid addressId)
        {
            _addressBookService.Delete(addressId);
            return RedirectToAction("Index", new { node = GetStartPage().AddressBookPage });
        }

        [HttpPost]
        public ActionResult SetPreferredShippingAddress(Guid addressId)
        {
            _addressBookService.SetPreferredShippingAddress(addressId);
            return RedirectToAction("Index", new { node = GetStartPage().AddressBookPage });
        }

        [HttpPost]
        public ActionResult SetPreferredBillingAddress(Guid addressId)
        {
            _addressBookService.SetPreferredBillingAddress(addressId);
            return RedirectToAction("Index", new { node = GetStartPage().AddressBookPage });
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            _controllerExceptionHandler.HandleRequestValidationException(filterContext, "save", OnSaveException);
        }

        public ActionResult OnSaveException(ExceptionContext filterContext)
        {
            Guid addressId;
            Guid.TryParse(filterContext.HttpContext.Request.Form["addressId"], out addressId);

            var currentPage = filterContext.RequestContext.GetRoutedData<AddressBookPage>();
            if (currentPage == null)
            {
                return new EmptyResult();
            }

            AddressViewModel viewModel = new AddressViewModel
            {
                Address = new Address
                {
                    AddressId = addressId != Guid.Empty ? (Guid?)addressId : null,
                    ErrorMessage = filterContext.Exception.Message,
                    HtmlFieldPrefix = "Address"
                },
                CurrentPage = currentPage
            };

            _addressBookService.LoadAddress(viewModel.Address);

            return View("EditForm", viewModel);
        }

        private StartPage GetStartPage()
        {
            return _contentLoader.Get<StartPage>(ContentReference.StartPage);
        }
    }
}