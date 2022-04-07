using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {
        private IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            var company = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);
            if (company == null)
                return View(new Company());

            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                {
                    _unitOfWork.Company.Add(company);
                    TempData["success"] = "Company Created";
                }
                else
                {
                    _unitOfWork.Company.Update(company);
                    TempData["success"] = "Company Updated";
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(company);
        }

        #region
        // API Calls

        [HttpGet]
        public IActionResult GetAll()
        {
            var companies = _unitOfWork.Company.GetAll();
            return Json(new {data = companies});
        }

        [HttpDelete]
        public IActionResult Destroy(int? id)
        {
            var company = _unitOfWork.Company.GetFirstOrDefault(c => c.Id == id);

            if (company == null)
                return Json(new { success = false, message = "Error Occurred while deleting" });

            _unitOfWork.Company.Remove(company);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Company Deleted" });
        }

        #endregion
    }
}
