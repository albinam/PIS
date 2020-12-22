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
            foreach( var p in pay)
            {
                int month = (p.Date).Month;
                int year = (p.Date).Year;
                int month1 = (model.Date).Month;
                int year1 = (model.Date).Year;
                if (year < year1 && p.UserId == Id) 
                {
                    if (month1 < month && p.UserId == Id)
                    {
                        return AddSalaries(model, Id);
                    }
                    else
                    {
                        ViewBag.Users = _user.Read(null);
                        ModelState.AddModelError("", "В этом месяце уже начислена зарплата");
                        return View("Views/Accountant/Salary.cshtml");
                    }
                }
                if(year >= year1 && p.UserId == Id)
                {
                    if (month1 > month && p.UserId == Id)
                    {
                        return AddSalaries(model,Id);
                    }
                    else
                    {
                        ViewBag.Users = _user.Read(null);
                        ModelState.AddModelError("", "В этом месяце уже начислена зарплата");
                        return View("Views/Accountant/Salary.cshtml");
                    }
                }
                else
                {
                    ViewBag.Users = _user.Read(null);
                    ModelState.AddModelError("", "Ошибка");
                    return View("Views/Accountant/Salary.cshtml");
                }
               
            }
            ViewBag.Users = _user.Read(null);
            ModelState.AddModelError("", "Ошибочка");
            return View("Views/Accountant/Salary.cshtml");
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
                    Sum = salary + com,
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
