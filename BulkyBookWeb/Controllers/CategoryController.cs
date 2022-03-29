using BulkyBookWeb.Data;
using BulkyBookWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var objCategoryList = _db.Categories.ToList();
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
                _db.Categories.Add(obj);
			    _db.SaveChanges();
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

            var category = _db.Categories.Find(id);
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
                _db.Categories.Update(obj);
                _db.SaveChanges();
                TempData["success"] = "Category Updated Successfully";
                return RedirectToAction("Index");
			}
            return View(obj);
		}

        public IActionResult Destroy(int? id)
		{
            var category = _db.Categories.Find(id);
            if (category == null)
			{
                return NotFound();
			}
            _db.Categories.Remove(category);
            _db.SaveChanges();
            TempData["success"] = "Category Deleted Successfully";
            return RedirectToAction("Index");
		}


    }
}
