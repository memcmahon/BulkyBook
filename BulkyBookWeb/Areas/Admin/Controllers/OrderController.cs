using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        //API Calls
        [HttpGet]
        public IActionResult GetAll(string? status)
        {
			//var orderList = _unitOfWork.OrderDetail.GetAll(includeProperties: "OrderHeader");
			//var status = HttpContext.Request.Query["status"].ToString();
			IEnumerable<OrderHeader> orderList;
			if (status != "all")
			{
                orderList = _unitOfWork.OrderHeader.GetAllWhere(o => o.OrderStatus == status ,includeProperties: "User");
			}
			else
			{
                orderList = _unitOfWork.OrderHeader.GetAll(includeProperties: "User");
			}
            return Json(new { data = orderList });
        }
    }
}
