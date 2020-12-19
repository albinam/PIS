using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PISBusinessLogic.BindingModels;
using PISBusinessLogic.Interfaces;
using PISBusinessLogic.Enums;
using PISBusinessLogic.ViewModels;

namespace PISCoursework.Controllers
{
    public class AccountantController : Controller
    {
        private readonly IContractLogic _contract;
        private readonly IUserLogic _user;
        public AccountantController(IUserLogic user, IContractLogic contract)
        {
            _user = user;
            _contract = contract;
        }
        public ActionResult SalaryLibrarian(int id)
        {
            UserViewModel users = new UserViewModel();
            var contract = _contract.Read(null);
            int count = 0;
            foreach(var cont in contract)
            {
                if(cont.LibrarianId == id)
                {
                    count++;
                }
            }
            if(count == 0)
            {
                
            }
            return ViewBag.users;
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
    }
}
