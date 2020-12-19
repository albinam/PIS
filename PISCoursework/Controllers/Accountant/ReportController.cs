using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PISCoursework.Models;
using PISBusinessLogic.HelperModels;
using PISBusinessLogic.Interfaces;
using PISBusinessLogic.ViewModels;
using PISBusinessLogic.Enums;

namespace PISCoursework.Controllers.Accountant
{
    public class ReportController : Controller
    {
        private readonly IUserLogic _user;
        private readonly IContractLogic _contract;
        private readonly ReportLogic _report;
        public ReportController(IUserLogic user, IContractLogic contract, ReportLogic report)
        {
            _user = user;
            _contract = contract;
            _report = report;
        }
        public IActionResult Report()
        {
            return View("Views/Accountant/Report.cshtml");
        }
        public IActionResult Chart()
        {
            return View("Views/Accountant/Chart.cshtml");
        }
        public JsonResult Charts()
        {
            var Users = _user.Read(null);
            List<ChartLibrarian> chart = new List<ChartLibrarian>();
            foreach (var us in Users)
            {
                if (us.Role == Roles.Библиотекарь)
                {
                    int sum = 0;
                    var Contract = _contract.Read(null);
                    foreach (var contr in Contract)
                    {
                        if (contr.LibrarianId == us.Id)
                        {
                            sum += Convert.ToInt32(contr.Sum);
                        }
                    }
                    chart.Add(new ChartLibrarian { UserFIO = us.FIO, Sum = sum });
                }
            }
            return Json(chart);
        }
    }
}
