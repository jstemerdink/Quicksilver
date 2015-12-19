﻿using EPiServer.Core;
using EPiServer.Editor;
using EPiServer.Reference.Commerce.Site.Features.AddressBook.Services;
using EPiServer.Reference.Commerce.Site.Features.Checkout.Models;
using EPiServer.Reference.Commerce.Site.Features.Checkout.Pages;
using EPiServer.Reference.Commerce.Site.Features.Checkout.Services;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using EPiServer.Web.Mvc.Html;
using Mediachase.Commerce.Orders;
using System;
using System.Web.Mvc;

using EPiServer.Reference.Commerce.Domain.Facades;

namespace EPiServer.Reference.Commerce.Site.Features.Checkout.Controllers
{
    public class OrderConfirmationController : OrderConfirmationControllerBase<OrderConfirmationPage>
    {

        public OrderConfirmationController(ConfirmationService confirmationService, AddressBookService addressBookService, CustomerContextFacade customerContextFacade)
            : base(confirmationService, addressBookService, customerContextFacade)
        {
        }

        [HttpGet]
        public ActionResult Index(OrderConfirmationPage currentPage, string notificationMessage, Guid? contactId = null, int orderNumber = 0)
        {
            PurchaseOrder order = _confirmationService.GetOrder(orderNumber, PageEditing.PageIsInEditMode);
            
            if (order == null && !PageEditing.PageIsInEditMode)
            {
                return Redirect(Url.ContentUrl(ContentReference.StartPage));
            }
            if (order.CustomerId != _customerContext.CurrentContactId && !PageEditing.PageIsInEditMode)
            {
                return Redirect(Url.ContentUrl(ContentReference.StartPage));
            }

            OrderConfirmationViewModel<OrderConfirmationPage> viewModel = CreateViewModel(currentPage, order);
            viewModel.NotificationMessage = notificationMessage;

            return View(viewModel);
        }
    }
}