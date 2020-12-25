using Microsoft.AspNetCore.Mvc;
using PISBusinessLogic.BindingModels;
using PISBusinessLogic.BusinessLogic;
using PISBusinessLogic.Enums;
using PISBusinessLogic.HelperModels;
using PISBusinessLogic.Interfaces;
using PISBusinessLogic.ViewModels;
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
        private readonly EncryptionLogic _enc;
        private readonly Validation validation;

        public UserController(IUserLogic user, EncryptionLogic enc)
        {
            _user = user;
            _enc = enc;
            validation = new Validation();
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(UserBindingModel user)
        {
            if (validation.userCheck(user, null) != "")
            {
                ModelState.AddModelError("", validation.userCheck(user, null));
                return View();
            }
            else
            {
                var userView = _user.Read(new UserBindingModel
                {
                    Email = user.Email,
                }).FirstOrDefault();
                if (validation.userCheck(user, userView) != "")
                {
                    ModelState.AddModelError("", validation.userCheck(user, userView));
                    return View();
                }
                if (userView == null)
                {
                    ModelState.AddModelError("", "Почта или пароль не верны, попробуйте еще раз");
                    return View();
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
        [HttpPost]
        public ActionResult Registration(UserBindingModel user)
        {
            if (validation.registrationCheck(user) != "")
            {
                ModelState.AddModelError("", validation.registrationCheck(user));
                return View();
            }
            var userView = _user.Read(new UserBindingModel
            {
                Email = user.Email
            }).FirstOrDefault();
            if (userView != null)
            {
                ModelState.AddModelError("", "Данный Email уже занят");
                return View(user);
            }
            _user.CreateOrUpdate(new UserBindingModel
            {
                FIO = user.FIO,
                Password = _enc.Encrypt(user.Password, user.Email),
                Email = user.Email,
                Role = Roles.Читатель
            });
            return RedirectToAction("Login", "User");
        }
        public IActionResult Logout()
        {
            if (Program.Librarian != null)
            {
                Program.Librarian = null;
            }
            if (Program.Accountant != null)
            {
                Program.Accountant = null;
            }
            if (Program.Reader != null)
            {
                Program.Reader = null;
            }
            return RedirectToAction("Index", "Home");
        }
    }
}

