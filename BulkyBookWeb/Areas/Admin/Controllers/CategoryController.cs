using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var objCategoryList = _unitOfWork.Category.GetAll();
            return View(objCategoryList);
        }

        public IActionResult Create()
		{
            return View();
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
		{
			if (obj.Name == obj.DisplayOrder.ToString())
			{
                //Because the view is bound to the model, this will add an error to the name property of the model
                //ModelState.AddModelError("name", "The DisplayOrder cannot exactly match the name");
                //this will add an error to the "All" error list
                ModelState.AddModelError("Error", "The DisplayOrder cannot exactly match the name");
			}
            if (ModelState.IsValid)
			{
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category Created Successfully";
                return RedirectToAction("Index");
			}
            return View();
		}

        public IActionResult Edit(int? id)
		{
            if (id == null || id == 0)
			{
                return NotFound();
			}

            var category = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == id);
            //var category = _db.Categories.SingleOrDefault(c => c.DisplayOrder == id);
            //var category = _db.Categories.FirstOrDefault(c => c.DisplayOrder == id);

            if (category == null)
            { 
                return NotFound(); 
            }

            return View(category);
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj)
		{
            if (obj.Name == obj.DisplayOrder.ToString())
			{
                ModelState.AddModelError("name", "The DisplayOrder cannot exactly match the Name.");
			}
            if (ModelState.IsValid)
			{
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category Updated Successfully";
                return RedirectToAction("Index");
			}
            return View(obj);
		}

        public IActionResult Destroy(int? id)
		{
            var category = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == id);
            if (category == null)
			{
                return NotFound();
			}
            _unitOfWork.Category.Remove(category);
            _unitOfWork.Save();
            TempData["success"] = "Category Deleted Successfully";
            return RedirectToAction("Index");
		}


    }
}
