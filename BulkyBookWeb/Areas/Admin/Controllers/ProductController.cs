using System.Collections.Generic;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.Facades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Controllers
{
    public class ProductController : Controller
    {
        private IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var coverTypes = _unitOfWork.Product.GetAll();
            return View(coverTypes);
        }

        public IActionResult Upsert(int? id)
        {
            ProductFacade productF = new ProductFacade
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetSelectList(),
                CoverTypeList = _unitOfWork.CoverType.GetSelectList()
            };

            if (id == null || id == 0)
            {
                //Create Product
                return View(productF);
            } else
            {
                //update Product
            }

            return View(productF);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Product obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Product Updated";
                return RedirectToAction("Index");
            }

            return View(obj);
        }

        public IActionResult Destroy(int? id)
        {
            var product = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);

            if (product == null)
                return View();

            _unitOfWork.Product.Remove(product);
            _unitOfWork.Save();
            TempData["success"] = "Product Destroyed";

            return RedirectToAction("Index");
        }
    }
}
