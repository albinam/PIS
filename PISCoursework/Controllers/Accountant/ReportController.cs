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
using PISBusinessLogic.BindingModels;
using System.IO;
using PISDatabaseImplement.Implements;
using PISBusinessLogic.BusinessLogic;

namespace PISCoursework.Controllers.Accountant
{
    public class ReportController : Controller
    {
        private readonly IUserLogic _user;
        private readonly IContractLogic _contract;
        private readonly IPaymentLogic _payment;
        private readonly ReportLogic _report;
        private Validation validation;
        private readonly ArchiveLogic _archive;
        string exportDirectory = Directory.GetCurrentDirectory() + "\\wwwroot\\Export";
        public ReportController(IUserLogic user, IContractLogic contract, ReportLogic report, IPaymentLogic payment, ArchiveLogic archive)
        {
            _user = user;
            _contract = contract;
            _report = report;
            _payment = payment;
            validation = new Validation();
            _archive = archive;
        }
        public IActionResult Report()
        {
            ViewBag.Users = _user.Read(null);
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
        public ActionResult DistributionSalary(DateTime date)
        {
            if (date.Year != 0001)
            {
                var payments = _payment.Read(null);
                payments.OrderBy(x => x.Sum);
                payments.Add(new PaymentViewModel());
                List<Dictionary<double, (double, double, double, double, double, double, Tuple<double, double, double, double, double, double>)>> dict = new List<Dictionary<double, (double, double, double, double, double, double, Tuple<double, double, double, double, double, double>)>>();
                var librarians = _user.Read(null);
                List<UserViewModel> list = new List<UserViewModel>();
                foreach (var librarian in librarians)
                {

                    if (librarian.Role == Roles.Библиотекарь)
                    {
                        list.Add(librarian);
                    }
                }
                foreach (var librarian in list)
                {
                    Dictionary<double, (double, double, double, double, double, double, Tuple<double, double, double, double, double, double>)> count = new Dictionary<double, (double, double, double, double, double, double, Tuple<double, double, double, double, double, double>)>();
                    double sum1 = 0;
                    double sum2 = 0;
                    double sum3 = 0;
                    double sum4 = 0;
                    double sum5 = 0;
                    double sum6 = 0;
                    double sum7 = 0;
                    double sum8 = 0;
                    double sum9 = 0;
                    double sum10 = 0;
                    double sum11 = 0;
                    double sum12 = 0;
                    var payment = _payment.Read(new PaymentBindingModel
                    {
                        UserId = librarian.Id
                    });
                    foreach (var c in payment)
                    {
                        if (c.Date.Year == date.Year)
                        {
                            if (c.Date.Month == 1)
                            {
                                sum1 += Convert.ToDouble(c.Sum);
                            }
                            if (c.Date.Month == 2)
                            {
                                sum2 += Convert.ToDouble(c.Sum);
                            }
                            if (c.Date.Month == 3)
                            {
                                sum3 += Convert.ToDouble(c.Sum);
                            }
                            if (c.Date.Month == 4)
                            {
                                sum4 += Convert.ToDouble(c.Sum);
                            }
                            if (c.Date.Month == 5)
                            {
                                sum5 += Convert.ToDouble(c.Sum);
                            }
                            if (c.Date.Month == 6)
                            {
                                sum6 += Convert.ToDouble(c.Sum);
                            }
                            if (c.Date.Month == 7)
                            {
                                sum7 += Convert.ToDouble(c.Sum);
                            }
                            if (c.Date.Month == 8)
                            {
                                sum8 += Convert.ToDouble(c.Sum);
                            }
                            if (c.Date.Month == 9)
                            {
                                sum9 += Convert.ToDouble(c.Sum);
                            }
                            if (c.Date.Month == 10)
                            {
                                sum10 += Convert.ToDouble(c.Sum);
                            }
                            if (c.Date.Month == 11)
                            {
                                sum11 += Convert.ToDouble(c.Sum);
                            }
                            if (c.Date.Month == 12)
                            {
                                sum12 += Convert.ToDouble(c.Sum);
                            }
                        }
                    }
                    Tuple<double, double, double, double, double, double> tuple = Tuple.Create(sum7, sum8, sum9, sum10, sum11, sum12);
                    count.Add(librarian.Id, (sum1, sum2, sum3, sum4, sum5, sum6, tuple));
                    dict.Add(count);
                }
                ViewBag.Sum = dict;
                ViewBag.Librarians = list;
            }
            return View("Views/Accountant/DistributionSalary.cshtml");
        }
        public ActionResult LeadSalary(DateTime month)
        {
            ViewBag.Users = _user.Read(null);

            int month1 = month.Month;
            if (!validation.leadSalary(month))
            {
                ViewBag.Month = month;
                var Payment = _payment.Read(null);
                List<PaymentViewModel> pays = new List<PaymentViewModel>();

                foreach (var pay in Payment)
                {
                    if (pay.Date.Month == month1)
                    {
                        pays.Add(pay);
                    }
                }
                ViewBag.Payment = pays;
                return View("Views/Accountant/LeadSalary.cshtml");
            }
            else
            {
                ModelState.AddModelError("", "Выберите месяц");
                return View("Views/Accountant/LeadSalary.cshtml");
            }
        }
        public ActionResult CheckLibrarian(int CountReport, int Id)
        {
            if (validation.checkLibrarian(CountReport, Id))
            {
                var cont = _contract.Read(new ContractBindingModel
                {
                    LibrarianId = Id
                });
                int count = 0;
                foreach (var p in cont)
                {
                    count++;
                }
                if (count == CountReport)
                {
                    ModelState.AddModelError("", "Верно!");
                }
                else
                {
                    ModelState.AddModelError("", "Не верно!");
                }
            }
            else
            {
                ModelState.AddModelError("", "Выберете библиотекаря или введите количество ");
            }
            
            ViewBag.Users = _user.Read(null);
            return View("Views/Accountant/CheckLibrarian.cshtml");
        }
        public ActionResult List()
        {
            _report.SaveListToWordFile(exportDirectory + "\\" + "список.docx");
            var fileName = Path.GetFileName(exportDirectory + "\\" + "список.docx");
            return File("Export/" + fileName, "application/xml", fileName);

        }
        public ActionResult ContractLibrarian(int id)
        {
            UserViewModel model = _user.Read(new UserBindingModel
            {
                Id = id
            }).FirstOrDefault();
            _report.SaveContractToWordFile(exportDirectory + "\\" + "Контракт c " + model.FIO + ".docx", model);
            var fileName = Path.GetFileName(exportDirectory + "\\" + "Контракт c " + model.FIO + ".docx");
            return File("Export/" + fileName, "application/xml", fileName);

        }
        public IActionResult BackUpToJsonAsync()
        {
            var payments = _payment.Read(null);
            foreach (var el in payments)
            {
                if (Convert.ToInt32(el.Date.Year) < (DateTime.Now.Year))
                {
                    _payment.Delete(new PaymentBindingModel
                    {
                        Id = el.Id
                    });
                }
            }
            var path = _archive.ArchiveOutdated(2);
            var fileName = Path.GetFileName(path);
            return File("Export/" + fileName, "text/json", fileName);
        }
    }
}
