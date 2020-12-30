using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PISBusinessLogic.BindingModels;
using PISBusinessLogic.Interfaces;
using PISBusinessLogic.Enums;
using PISBusinessLogic.ViewModels;
using PISBusinessLogic.HelperModels;
using System.IO;
using PISBusinessLogic.BusinessLogic;

namespace PISCourseworkARMAccountant.Controllers
{
    public class AccountantController : Controller
    {
        private readonly IContractLogic _contract;
        private readonly IUserLogic _user;
        private readonly ReportLogic _report;
        private Validation validation;
        public AccountantController(IUserLogic user, IContractLogic contract, ReportLogic report)
        {
            _user = user;
            _contract = contract;
            _report = report;
            validation = new Validation();
        }
        public ActionResult ListContract(int Id, DateTime month)
        {
            if (validation.listContract(Id, month))
            {
                int month1 = month.Month;
                List<ContractViewModel> contracts = new List<ContractViewModel>();
                var contract = _contract.Read(new ContractBindingModel
                {
                    LibrarianId = Id
                });
                foreach (var cont in contract)
                {
                    if (cont.Date.Month == month1)
                    {
                        contracts.Add(cont);
                    }
                }
                contracts.OrderBy(x => x.Date);
                ViewBag.Contract = contracts;
                ViewBag.Users = _user.Read(null);
                return View("Views/Accountant/ListContract.cshtml");
            }
            else
            {
                ViewBag.Users = _user.Read(null);
                ModelState.AddModelError("", "Выберите библиотекаря и дату");
                return View("Views/Accountant/ListContract.cshtml");
            }
            
        }
        
        public ActionResult ChangeCommission(int Id, string ComissionPercent)
        {

            if (validation.сhangeCommission(Id, ComissionPercent))
            {
                ViewBag.Users = _user.Read(null);
                var user = _user.Read(new UserBindingModel
                {
                    Id = Id
                }).FirstOrDefault();
                decimal percent = decimal.Parse(ComissionPercent)/10000;           
                decimal salary = decimal.Parse(user.Salary.ToString());
                _user.CreateOrUpdate(new UserBindingModel
                {
                    Id = Id,
                    FIO = user.FIO,
                    Password=user.Password,
                    Email = user.Email,
                    Salary = user.Salary,
                    Comission = (Math.Round(Convert.ToDouble(salary * percent), 2)).ToString(),
                    ComissionPercent = ComissionPercent,
                });
                ModelState.AddModelError("", "Процент успешно изменен");
                return View("Views/Accountant/ChangeCommission.cshtml");
            }
            else
            {
                ViewBag.Users = _user.Read(null);
                ModelState.AddModelError("", "Выберите библиотекаря или введите процент");
                return View("Views/Accountant/ChangeCommission.cshtml");
            }
        }
        public ActionResult ChangeCommissionAll(string ComissionPercentAll)
        {
            if (validation.сhangeCommissionAll(ComissionPercentAll))
            {
                var user = _user.Read(null);
                ViewBag.Users = _user.Read(null);
                List<UserViewModel> users = new List<UserViewModel>();
                foreach (var us in user)
                {
                    if (us.Role == Roles.Библиотекарь)
                    {
                        decimal percent = decimal.Parse(ComissionPercentAll) /10000 ;
                        decimal salary = decimal.Parse(us.Salary.ToString());
                        _user.CreateOrUpdate(new UserBindingModel
                        {
                            Id = us.Id,
                            FIO = us.FIO,
                            Password = us.Password,
                            Email = us.Email,
                            Salary = us.Salary,
                            Comission = (Math.Round(Convert.ToDouble(salary * percent), 2)).ToString(),
                            ComissionPercent = ComissionPercentAll,
                        });
                    }
                }
                ModelState.AddModelError("", "Процент успешно изменен");
                return View("Views/Accountant/ChangeCommission.cshtml");
            }
            else
            {
                ViewBag.Users = _user.Read(null);
                ModelState.AddModelError("", "Введите процент");
                return View("Views/Accountant/ChangeCommission.cshtml");
            }
        }
        public ActionResult ListOfLibrarian(UserBindingModel model)
        {        
            var user = _user.Read(null);
            List<UserViewModel> users = new List<UserViewModel>();
            foreach (var us in user)
            {
                if (us.Role == Roles.Библиотекарь)
                    users.Add(us);
            }
            ViewBag.Users = users;
            return LibrarianSearch(model);
        }
        public ActionResult LibrarianSearch(UserBindingModel model)
        {
            var user = _user.Read(null);
            List<UserViewModel> users = new List<UserViewModel>();
            foreach (var us in user)
            {
                if (us.Role == Roles.Библиотекарь)
                    users.Add(us);
            }
            ViewBag.Users = users;

            //по ФИО
            if (model.FIO != null && model.Id == null)
            {
                var Users = _user.Read(new UserBindingModel
                {
                    FIO = model.FIO,
                    Id = model.Id
                });
                List<UserViewModel> librarians = new List<UserViewModel>();
                foreach (var User in Users)
                {
                    if (User.Role == Roles.Библиотекарь)
                    {
                        librarians.Add(User);
                    }
                }
                ViewBag.Users = librarians;
                return View("Views/Accountant/ListOfLibrarian.cshtml");
            }
            //по личному коду
            if (model.FIO == null && model.Id != null)
            {
                var Users = _user.Read(new UserBindingModel
                {
                    FIO = model.FIO,
                    Id = model.Id
                });
                List<UserViewModel> librarians = new List<UserViewModel>();
                foreach (var User in Users)
                {
                    if (User.Role == Roles.Библиотекарь)
                    {
                        librarians.Add(User);
                    }
                }
                ViewBag.Users = librarians;
                return View("Views/Accountant/ListOfLibrarian.cshtml");
            }
            //по ФИО и личному коду
            if (model.FIO != null && model.Id != null)
            {
                var Users = _user.Read(new UserBindingModel
                {
                    FIO = model.FIO
                });
                List<UserViewModel> librarians = new List<UserViewModel>();
                foreach (var User in Users)
                {
                    if (User.Role == Roles.Библиотекарь)
                    {
                        if (User.Id == model.Id)
                        {
                            librarians.Add(User);
                        }
                    }
                }
                ViewBag.Users = librarians;
                return View("Views/Accountant/ListOfLibrarian.cshtml");
            }

            if (model.FIO == null && model.Id == null)
            {
                ModelState.AddModelError("", "Выберите хотя бы один параметр поиска");
                return View("Views/Accountant/ListOfLibrarian.cshtml");
            }
            return View(model);
        }
    }
}
