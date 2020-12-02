using Microsoft.AspNetCore.Mvc;
using PISBusinessLogic;
using PISBusinessLogic.BindingModels;
using PISBusinessLogic.Enums;
using PISBusinessLogic.Interfaces;
using PISBusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PISCoursework.Controllers
{
    public class LibrarianController : Controller
    {
        private readonly IBookLogic _book;
        private readonly IGenreLogic _genre;
        private readonly IUserLogic _user;
        private readonly ILibraryCardLogic _libraryCard;
        public LibrarianController(IBookLogic book, IGenreLogic genre, IUserLogic user, ILibraryCardLogic libraryCard)
        {
            _book = book;
            _genre = genre;
            _user = user;
            _libraryCard = libraryCard;

        }
        public IActionResult AddBook()
        {
            ViewBag.Genres = _genre.Read(null);
            return View();
        }
        [HttpPost]
        public ActionResult AddBook(BookBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Genres = _genre.Read(null);
                return View(model);
            }
            if (model.Name == null)
            {
                ViewBag.Genres = _genre.Read(null);
                ModelState.AddModelError("", "Введите название");
                return View(model);
            }
            if (model.Author == null)
            {
                ViewBag.Genres = _genre.Read(null);
                ModelState.AddModelError("", "Введите автора");
                return View(model);
            }
            if (model.PublishingHouse == null)
            {
                ViewBag.Genres = _genre.Read(null);
                ModelState.AddModelError("", "Введите издательство");
                return View(model);
            }
            if (model.Year == null)
            {
                ViewBag.Genres = _genre.Read(null);
                ModelState.AddModelError("", "Введите год издания");
                return View(model);
            }
            if (model.Name != null && model.PublishingHouse != null && model.Year != null && model.Author != null)
            {
                ModelState.AddModelError("", "Книга успешно добавлена");
            }
            _book.CreateOrUpdate(new BookBindingModel
            {
                Name = model.Name,
                Author = model.Author,
                PublishingHouse = model.PublishingHouse,
                Year = model.Year,
                GenreId = model.GenreId,
                Status = Status.Свободна
            });

            return RedirectToAction("ListOfBooks");
        }
        public IActionResult ListOfBooks()
        {
            ViewBag.Genres = _genre.Read(null);
            ViewBag.Books = _book.Read(null);
            return View();
        }
        public IActionResult Readers()
        {
            ViewBag.Users = _user.Read(null);
            return View();
        }
        [HttpGet]
        public ActionResult Readers(UserBindingModel model)
        {
            if (model.FIO == null)
            {
                var Users = _user.Read(null);
                List<UserViewModel> users = new List<UserViewModel>();
                foreach (var user in Users)
                {
                    if (user.Role == Roles.Читатель)
                        users.Add(user);
                }
                ViewBag.Users = users;
                return View();
            }
            var Users2 = _user.Read(new UserBindingModel
            {
                FIO = model.FIO
            });
            List<UserViewModel> users2 = new List<UserViewModel>();
            foreach (var user in Users2)
            {
                if (user.Role == Roles.Читатель)
                    users2.Add(user);
            }
            ViewBag.Users = users2;
            return View();
        }
        [HttpGet]
        public ActionResult AddLibraryCard(int id)
        {
            ViewBag.UserId = id;
            ViewBag.User = _user.Read(new UserBindingModel
            {
                Id = id
            }).FirstOrDefault();
            ViewBag.Exists = _libraryCard.Read(new LibraryCardBindingModel
            {
                UserId = id
            });  
            return View();
        }
        [HttpPost]
        public ActionResult AddLibraryCard(LibraryCardBindingModel model)
        {
            if (model.DateOfBirth == null)
            {
                ModelState.AddModelError("", "Введите дату рождения");
                return View(model);
            }
            if (model.Year == null)
            {
                ModelState.AddModelError("", "Введите год");
                return View(model);
            }
            if (model.PlaceOfWork == null)
            {
                ModelState.AddModelError("", "Введите место работы");
                return View(model);
            }
            _libraryCard.CreateOrUpdate(new LibraryCardBindingModel
            {
                DateOfBirth = model.DateOfBirth,
                Year = model.Year,
                PlaceOfWork = model.PlaceOfWork,
                UserId = model.UserId
            });
            return RedirectToAction("Readers");
        }
    }
}
