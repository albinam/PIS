using Microsoft.AspNetCore.Mvc;
using PISBusinessLogic.BindingModels;
using PISBusinessLogic.Enums;
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

        public UserController(IUserLogic user)
        {
            _user = user;
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Registration()
        {
            //ViewBag.User = Program.Reader;
            return View();
        }
        [HttpPost]
        public ActionResult Login(UserBindingModel user)
        {
            if (user.Password == null)
            {
                ModelState.AddModelError("", "Вы ввели неверный пароль, либо пользователь не найден");
                return View();
            }

            if (user.Email == null)
            {
                ModelState.AddModelError("", "Вы ввели неверный пароль, либо пользователь не найден");
                return View();
            }
            else
            {
                var userView = _user.Read(new UserBindingModel
                {
                    Email = user.Email,
                    Password = user.Password
                }).FirstOrDefault();
                if (userView == null)
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
        [HttpPost]
        public ViewResult Registration(RegistrationModel user)
        {
            if (String.IsNullOrEmpty(user.Email))
            {
                ModelState.AddModelError("", "Введите электронную почту");
                return View(user);
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
            if (!Regex.IsMatch(user.Email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
            {
                ModelState.AddModelError("", "Email введен некорректно");
                return View(user);
            }
            if (String.IsNullOrEmpty(user.FIO))
            {
                ModelState.AddModelError("", "Введите ФИО");
                return View(user);
            }
            if (String.IsNullOrEmpty(user.Password))
            {
                ModelState.AddModelError("", "Введите пароль");
                return View(user);
            }
            _user.CreateOrUpdate(new UserBindingModel
            {
                FIO = user.FIO,
                Password = user.Password,
                Email = user.Email,
                Role = Roles.Читатель
            });
            ModelState.AddModelError("", "Вы успешно зарегистрированы");
            return View("Registration", user);
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

