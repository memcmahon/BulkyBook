using System.Collections.Generic;
using System.Security.Claims;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.Facades;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Customer.Controllers
{

    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;

            var cartFacade = new CartFacade
            {
                CartItems = _unitOfWork.ShoppingCart.GetAllWhere(c => c.ApplicationUserId == userId, "Product")
            };

            return View(cartFacade);
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;
            var user = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == userId);

            var cartFacade = new CartFacade
            {
                CartItems = _unitOfWork.ShoppingCart.GetAllWhere(c => c.ApplicationUserId == userId, "Product"),
                OrderHeader = new OrderHeader
                {
                    User = user,
                    ApplicationUserId = userId,
                    Name = user.Name,
                    Address = user.Address,
                    City = user.City,
                    State = user.State,
                    PostalCode = user.PostalCode,
                    PhoneNumber = user.PhoneNumber
                }
            };

            return View(cartFacade);
        }

        public IActionResult IncreaseQuantity(int id)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == id);
            if (cart == null) return NotFound();

            cart.IncreaseQuantity();
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }

        public IActionResult DecreaseQuantity(int id)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == id);
            if (cart == null) return NotFound();

            if (cart.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(cart);
            }
            else
            {
                cart.DecreaseQuantity();
            }
            
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }

        public IActionResult Destroy(int id)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == id);
            if (cart == null) return NotFound();

            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }
    }
}
