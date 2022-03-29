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
                return RedirectToAction("Index");
			}
            return View();
		}
    }
}
