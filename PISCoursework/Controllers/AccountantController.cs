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

            //по ФИО
            if (model.FIO != null && model.Id == 0)
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
                return View(model);
            }
            /*//по личному коду
            if (model.FIO == null && model.Id != 0)
            {
                ViewBag.Users = _user.Read(new UserBindingModel
                {
                    FIO = model.FIO,
                    Id = model.Id
                });
                return View(model);
            }
            //по ФИО и личному коду
            if (model.FIO != null && model.Id != 0)
            {
                ViewBag.Users = _user.Read(new UserBindingModel
                {
                    FIO = model.FIO,
                    Id = model.Id
                });
                return View(model);
            }*/
            if (model.FIO == null && model.Id == 0)
            {
                ModelState.AddModelError("", "Выберите хотя бы один параметр поиска");
                return View();
            }

            return View(model);

        }
    }
}
