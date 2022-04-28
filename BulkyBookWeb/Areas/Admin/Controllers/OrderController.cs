﻿using BulkyBook.DataAccess.Repository.IRepository;
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
        public IActionResult GetAll()
        {
            //var orderList = _unitOfWork.OrderDetail.GetAll(includeProperties: "OrderHeader");
            var orderList = _unitOfWork.OrderHeader.GetAll(includeProperties: "User");
            return Json(new { data = orderList });
        }
    }
}
