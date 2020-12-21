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

namespace PISCoursework.Controllers
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
        public ActionResult ChangeCommission(int Id, string ComissionPercent)
        {

            if (validation.сhangeCommission(Id,ComissionPercent))
            {
                ViewBag.Users = _user.Read(null);
                var user = _user.Read(new UserBindingModel
                {
                    Id = Id
                }).FirstOrDefault();
                ComissionPercent = ComissionPercent.Replace(".", ",");
                double percent = Convert.ToDouble(ComissionPercent);
                double salary = Convert.ToDouble(user.Salary);
                _user.CreateOrUpdate(new UserBindingModel
                {
                    Id = Id,
                    FIO = user.FIO,
                    Salary = user.Salary,
                    Comission = (Math.Round(salary * (percent / 100), 2)).ToString(),
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
                        ComissionPercentAll = ComissionPercentAll.Replace(".", ",");
                        double percent = Convert.ToDouble(ComissionPercentAll);
                        double salary = Convert.ToDouble(us.Salary);
                        double com = Math.Round(salary *(percent / 100),2);
                        _user.CreateOrUpdate(new UserBindingModel
                        {
                            Id = us.Id,
                            FIO = us.FIO,
                            Salary = us.Salary,
                            Comission = (com).ToString(),
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
                foreach(var User in Users)
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
            if (model.FIO == null && model.Id == null)
            {
                ModelState.AddModelError("", "Выберите хотя бы один параметр поиска");
                return View("Views/Accountant/ListOfLibrarian.cshtml");
            }
            return View(model);
        }
        public ActionResult List()
        {
            _report.SaveListToWordFile("C://Users//marin.LAPTOP-0TUFHPTU//Рабочий стол//универ//3 курс//пис//отч//список.docx");
            // Путь к файлу
            string file_path = Path.Combine("C://Users//marin.LAPTOP-0TUFHPTU//Рабочий стол//универ//3 курс//пис//отч//список.docx");
            // Тип файла - content-type
            string file_type = "application/docx";
            // Имя файла - необязательно
            string file_name = "Список библиотекарей.docx";
            return PhysicalFile(file_path, file_type, file_name);

        }
    }
}
