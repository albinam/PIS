using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PISBusinessLogic.BindingModels;
using PISBusinessLogic.Interfaces;

namespace PISCoursework.Controllers.Accountant
{
    public class SalaryController : Controller
    {
        private readonly IUserLogic _user;
        private readonly IPaymentLogic _payment;
        private Validation validation;
        public SalaryController(IUserLogic user, IPaymentLogic payment)
        {
            _user = user;
            _payment = payment;
            validation = new Validation();
        }
        public ActionResult AddSalary(PaymentBindingModel model, int Id)
        {
            var pay = _payment.Read(new PaymentBindingModel
            {
                UserId = Id
            });
            if (pay.Count == 0)
            {
                return AddSalaries(model, Id);
            }
            foreach (var p in pay)
            {
                if (p.Date.Month == model.Date.Month && p.Date.Year == model.Date.Year)
                {
                    ViewBag.Users = _user.Read(null);
                    ModelState.AddModelError("", "В этом месяце уже начислена зарплата");
                    return View("Views/Accountant/Salary.cshtml");
                }
            }
            return AddSalaries(model, Id);
        }

        public ActionResult AddSalaries(PaymentBindingModel model, int Id)
        {
            if (validation.addSalar(model, Id))
            {
                ViewBag.Users = _user.Read(null);
                var user = _user.Read(new UserBindingModel
                {
                    Id = Id
                }).FirstOrDefault();
                double salary = Convert.ToDouble(user.Salary);
                double com = Convert.ToDouble(user.Comission);
                _payment.CreateOrUpdate(new PaymentBindingModel
                {
                    Date = model.Date,
                    Sum = Math.Round((salary + com),2),
                    UserId = Id,
                });
                ModelState.AddModelError("", "Зарплата начислена");
                return View("Views/Accountant/Salary.cshtml");
            }
            else
            {
                ViewBag.Users = _user.Read(null);
                ModelState.AddModelError("", "Выберите библиотекаря или поставьте дату");
                return View("Views/Accountant/Salary.cshtml");
            }
        }
        public ActionResult CheckSalary(PaymentBindingModel model, string sum)
        {
            ViewBag.Users = _user.Read(null);
            sum=sum.Replace(".", ",");
            if (validation.checkSalary(model,sum))
            {
                var pay = _payment.Read(new PaymentBindingModel
                {
                    UserId = model.UserId
                });
                foreach (var p in pay)
                {
             
                    if (p.Date.Month == model.Date.Month && p.Date.Year == model.Date.Year)
                    {
                        if (p.Sum < Convert.ToDouble(sum))
                        {
                            ModelState.AddModelError("", "Сумма превышает суммы платежа");
                            return View("Views/Accountant/LeadSalary.cshtml");
                        }
                        _payment.CreateOrUpdate(new PaymentBindingModel
                        {
                            Id = p.Id,
                            Date = p.Date,
                            Sum = p.Sum - Convert.ToDouble(sum),
                            UserId = p.UserId
                        });
                        ModelState.AddModelError("", "Изменено");
                        return View("Views/Accountant/LeadSalary.cshtml");
                    }
                }
                ModelState.AddModelError("", "Нет платежа по такому параметру");
                return View("Views/Accountant/LeadSalary.cshtml");
            }
            else
            {
                ModelState.AddModelError("", "Выберите библиотекаря, дату и введите сумму");
                return View("Views/Accountant/LeadSalary.cshtml");
            }
        }

        public ActionResult SalaryAll(DateTime date)
        {
            var pay = _payment.Read(null);
            double sum = 0;
            if (!validation.salaryAll(date))
            {
                foreach (var p in pay)
                {
                    int year = (p.Date).Year;
                    int year1 = date.Year;
                    if (year == year1)
                    {
                        sum += Math.Round(Convert.ToDouble(p.Sum), 2);
                    }
                }
            }
            else
            {
                ViewBag.Users = _user.Read(null);
                ModelState.AddModelError("", "Выберите дату");
                return View("Views/Accountant/Salary.cshtml");
            }
            ViewBag.Sum = Math.Round(sum, 2);
            ViewBag.Users = _user.Read(null);
            return View("Views/Accountant/Salary.cshtml");
        }
    }
}
