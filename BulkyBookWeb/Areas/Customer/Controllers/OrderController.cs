using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.Facades;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Mvc;

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

            _unitOfWork.ShoppingCart.RemoveRange(cartFacade.CartItems);
            _unitOfWork.Save();
            
            return View();
        }
    }
}
