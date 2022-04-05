using System.Collections.Generic;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
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
            Product product = new();
            var categoryList = _unitOfWork.Category.GetAll()
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
            var coverTypeList = _unitOfWork.CoverType.GetAll()
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });

            if (id == null || id == 0)
            {
                //Create Product
                ViewBag.CategoryList = categoryList;
                ViewData["CoverTypeList"] = coverTypeList;
                return View(product);
            } else
            {
                //update Product
            }

            return View(product);
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
