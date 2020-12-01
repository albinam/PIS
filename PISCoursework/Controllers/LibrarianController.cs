using Microsoft.AspNetCore.Mvc;
using PISBusinessLogic;
using PISBusinessLogic.BindingModels;
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
        public LibrarianController(IBookLogic book, IGenreLogic genre)
        {
            _book = book;
            _genre = genre;
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

    }
}
