﻿using EPiServer;
using EPiServer.Commerce.Catalog.Linking;
using EPiServer.Commerce.Order;
using EPiServer.Core;
using EPiServer.Editor;
using EPiServer.Filters;
using EPiServer.Globalization;
using EPiServer.Web.Mvc.Html;
using EPiServer.Web.Routing;
using Foundation.Commerce.Customer.Services;
using Foundation.Features.Checkout.Services;
using Foundation.Features.MyAccount.AddressBook;
using Foundation.Infrastructure.Services;
using Mediachase.Commerce;
using System.Web.Mvc;

namespace Foundation.Features.MyAccount.OrderConfirmation
{
    public class OrderConfirmationController : OrderConfirmationControllerBase<OrderConfirmationPage>
    {
        private readonly ICampaignService _campaignService;
        public OrderConfirmationController(
            ICampaignService campaignService,
            ConfirmationService confirmationService,
            IAddressBookService addressBookService,
            IOrderGroupCalculator orderGroupCalculator,
            UrlResolver urlResolver, ICustomerService customerService,
            IContentLoader contentLoader,
            FilterPublished filterPublished,
            ICurrentMarket currentMarket,
            LanguageResolver languageResolver,
            IRelationRepository relationRepository) :
            base(confirmationService, addressBookService, orderGroupCalculator, urlResolver, customerService, contentLoader, filterPublished, currentMarket, languageResolver, relationRepository)
        {
            _campaignService = campaignService;
        }
        public ActionResult Index(OrderConfirmationPage currentPage, string notificationMessage, int? orderNumber)
        {
            IPurchaseOrder order = null;
            if (PageEditing.PageIsInEditMode)
            {
                order = _confirmationService.CreateFakePurchaseOrder();
            }
            else if (orderNumber.HasValue)
            {
                order = _confirmationService.GetOrder(orderNumber.Value);
            }

            if (order != null && order.CustomerId == _customerService.CurrentContactId)
            {
                var viewModel = CreateViewModel(currentPage, order);
                viewModel.NotificationMessage = notificationMessage;

                _campaignService.UpdateLastOrderDate();
                _campaignService.UpdatePoint(decimal.ToInt16(viewModel.SubTotal.Amount));

                return View(viewModel);
            }

            return Redirect(Url.ContentUrl(ContentReference.StartPage));
        }
    }
}