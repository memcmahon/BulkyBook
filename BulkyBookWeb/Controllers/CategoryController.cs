using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _db;

        public CategoryController(ICategoryRepository db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var objCategoryList = _db.GetAll();
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
                _db.Add(obj);
                _db.Save();
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

            var category = _db.GetFirstOrDefault(c => c.Id == id);
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
                _db.Update(obj);
                _db.Save();
                TempData["success"] = "Category Updated Successfully";
                return RedirectToAction("Index");
			}
            return View(obj);
		}

        public IActionResult Destroy(int? id)
		{
            var category = _db.GetFirstOrDefault(c => c.Id == id);
            if (category == null)
			{
                return NotFound();
			}
            _db.Remove(category);
            _db.Save();
            TempData["success"] = "Category Deleted Successfully";
            return RedirectToAction("Index");
		}


    }
}
