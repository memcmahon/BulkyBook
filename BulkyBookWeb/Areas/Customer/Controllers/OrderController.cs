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
                _unitOfWork.Save();
            }

            var user = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == cartFacade.OrderHeader.ApplicationUserId);
            if (user.CompanyId.GetValueOrDefault() == 0)
            {
                ProcessPayment(cartFacade);
                return new StatusCodeResult(303);
            } 
            else
            {
                cartFacade.OrderHeader.OrderStatus = SD.StatusApproved;
                cartFacade.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                _unitOfWork.Save();
                return RedirectToAction("OrderConfirmation", "Order", new { id = cartFacade.OrderHeader.Id });
            }
        }

        public IActionResult OrderConfirmation(int id)
        {
            var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(oh => oh.Id == id);
            
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                // check the stripe status
                if(session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }

            var cartItems = _unitOfWork.ShoppingCart.GetAllWhere(c => c.ApplicationUserId == orderHeader.ApplicationUserId);
            _unitOfWork.ShoppingCart.RemoveRange(cartItems);
            _unitOfWork.Save();

            return View(id);
        }

        private void ProcessPayment(CartFacade cartFacade)
        {
            cartFacade.OrderHeader.OrderStatus = SD.StatusPending;
            cartFacade.OrderHeader.PaymentStatus = SD.PaymentStatusPending;

            //Stripe Payment
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
        }
    }
}
