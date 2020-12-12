using Microsoft.AspNetCore.Mvc;
using PISBusinessLogic.BindingModels;
using PISBusinessLogic.Enums;
using PISBusinessLogic.Interfaces;
using PISCoursework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PISCoursework.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserLogic _user;

        public UserController(IUserLogic user)
        {
            _user = user;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginModel user)
        {
            var userView = _user.Read(new UserBindingModel
            {
                Email = user.Login,
                Password = user.Password
            }).FirstOrDefault();
            if (userView == null || userView.Password == null)
            {
                ModelState.AddModelError("", "Вы ввели неверный пароль, либо пользователь не найден");
                return View(user);
            }
            if (userView.Role == Roles.Библиотекарь)
            {
                Program.Librarian = userView;
            }
            if (userView.Role == Roles.Читатель)
            {
                Program.Reader = userView;
            }
            if (userView.Role == Roles.Бухгалтер)
            {
                Program.Accountant = userView;
            }

            return RedirectToAction("Index", "Home");
        }
    }
}

