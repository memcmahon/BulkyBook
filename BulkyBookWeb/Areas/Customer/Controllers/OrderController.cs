using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.Facades;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CartFacade cartFacade)
        {
            cartFacade.CartItems = _unitOfWork.ShoppingCart
                .GetAllWhere(ci => ci.ApplicationUserId == cartFacade.OrderHeader.ApplicationUserId, "Product");

            cartFacade.OrderHeader.OrderStatus = SD.StatusPending;
            cartFacade.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            cartFacade.OrderHeader.OrderDate = DateTime.Now;
            cartFacade.OrderHeader.OrderTotal = cartFacade.Total();

            _unitOfWork.OrderHeader.Add(cartFacade.OrderHeader);
            _unitOfWork.Save();

            foreach (var item in cartFacade.CartItems)
            {
                var orderDetail = new OrderDetail
                {
                    ProductId = item.ProductId,
                    OrderId = cartFacade.OrderHeader.Id,
                    Count = item.Count,
                    Price = item.cartPrice()
                };

                _unitOfWork.OrderDetail.Add(orderDetail);
            }

            var domain = "https://localhost:44364/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"customer/order/OrderConfirmation?id={cartFacade.OrderHeader.Id}",
                CancelUrl = domain + $"customer/cart/index",
            };

            foreach (var item in cartFacade.CartItems)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (int)(item.cartPrice() * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title,
                        },

                    },
                    Quantity = item.Count,
                };

                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);

            _unitOfWork.OrderHeader.UpdateStripePaymentId(cartFacade.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

            //_unitOfWork.ShoppingCart.RemoveRange(cartFacade.CartItems);
            //_unitOfWork.Save();

            //return RedirectToAction("Index", "Home");
        }

        public IActionResult OrderConfirmation(int id)
        {
            var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(oh => oh.Id == id);
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);

            // check the stripe status
            if(session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                _unitOfWork.Save();
            }

            var cartItems = _unitOfWork.ShoppingCart.GetAllWhere(c => c.ApplicationUserId == orderHeader.ApplicationUserId);
            _unitOfWork.ShoppingCart.RemoveRange(cartItems);
            _unitOfWork.Save();

            return View(id);
        }
    }
}
