using Microsoft.AspNetCore.Mvc;
using PISBusinessLogic;
using PISBusinessLogic.BindingModels;
using PISBusinessLogic.Interfaces;
using PISBusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PISCoursework.Controllers.Librarian
{
    public class BooksController : Controller
    {
        private readonly IBookLogic _book;
        private readonly IGenreLogic _genre;
        private readonly IUserLogic _user;
        private readonly ILibraryCardLogic _libraryCard;
        private readonly IContractLogic _contract;
        private Validation validation;
        public BooksController(IBookLogic book, IGenreLogic genre, IUserLogic user, ILibraryCardLogic libraryCard, IContractLogic contract)
        {
            _book = book;
            _genre = genre;
            _user = user;
            _libraryCard = libraryCard;
            _contract = contract;
            validation = new Validation();
        }
        public IActionResult AddBook()
        {
            ViewBag.Genres = _genre.Read(null);
            return View("Views/Librarian/AddBook.cshtml");
        }
        [HttpPost]
        public ActionResult AddBook(BookBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Genres = _genre.Read(null);
                return View(model);
            }
            if (validation.addBook(model) != "")
            {
                ViewBag.Genres = _genre.Read(null);
                ModelState.AddModelError("", validation.addBook(model));
                return View("Views/Librarian/AddBook.cshtml");
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
            ModelState.AddModelError("", "Книга успешно добавлена");
            return RedirectToAction("ListOfBooks");
        }
        [HttpGet]
        public ActionResult ListOfBooks(BookBindingModel model)
        {
            ViewBag.Genres = _genre.Read(null);
            ViewBag.Books = _book.Read(new BookBindingModel
            {
                Status = Status.Свободна
            });
            return BookSearch(model);

        }
        public ActionResult BookPrice(int GenreId, string Percent)
        {
            if (validation.bookPrice(GenreId,Percent))
            {
                ViewBag.Genres = _genre.Read(null);
                var genre = _genre.Read(new GenreBindingModel
                {
                    Id = GenreId
                }).FirstOrDefault();
                Percent = Percent.Replace(".", ",");
                double percent = Convert.ToDouble(Percent);
                _genre.CreateOrUpdate(new GenreBindingModel
                {
                    Id = GenreId,
                    Name = genre.Name,
                    Price = genre.Price * percent
                });
                ModelState.AddModelError("", "Цена успешно изменена");
                return View("Views/Librarian/BookPrice.cshtml");
            }
            else
            {
                ViewBag.Genres = _genre.Read(null);
                ModelState.AddModelError("", "Выберите жанр и/или введите коэффициент изменения");
                return View("Views/Librarian/BookPrice.cshtml");
            }
        }
        public ActionResult BookSearch(BookBindingModel model)
        {
            ViewBag.Genres = _genre.Read(null);
            var freebooks = _book.Read(null);
            List<BookViewModel> freeBooks = new List<BookViewModel>();
            foreach (var book in freebooks)
            {
                if (book.Status == Status.Свободна)
                    freeBooks.Add(book);
            }
            ViewBag.Books = freeBooks;
            //по названию
            if (model.Name != null && model.GenreId == 0 && model.Author == null)
            {
                ViewBag.Books = _book.Read(new BookBindingModel
                {
                    Name = model.Name,
                    Status = Status.Свободна
                });
                return View("Views/Librarian/ListOfBooks.cshtml");
            }
            //по жанру
            if (model.GenreId != 0 && model.Name == null && model.Author == null)
            {
                ViewBag.Books = _book.Read(new BookBindingModel
                {
                    GenreId = model.GenreId,
                    Status = Status.Свободна
                });
                return View("Views/Librarian/ListOfBooks.cshtml");
            }
            //по автору
            if (model.GenreId == 0 && model.Name == null && model.Author != null)
            {
                ViewBag.Books = _book.Read(new BookBindingModel
                {
                    Author = model.Author,
                    Status = Status.Свободна
                });
                return View("Views/Librarian/ListOfBooks.cshtml");
            }
            //по жанру и названию
            if (model.GenreId != 0 && model.Name != null && model.Author == null)
            {
                var books = _book.Read(new BookBindingModel
                {
                    Name = model.Name,
                    Status = Status.Свободна
                });
                List<BookViewModel> Books = new List<BookViewModel>();
                foreach (var book in books)
                {
                    if (book.GenreId == model.GenreId)
                        Books.Add(book);
                }
                ViewBag.Books = Books;
                return View("Views/Librarian/ListOfBooks.cshtml");
            }
            // по всем трем
            if (model.GenreId != 0 && model.Name != null && model.Author != null)
            {
                var books = _book.Read(new BookBindingModel
                {
                    Name = model.Name,
                    Status = Status.Свободна
                });
                List<BookViewModel> Books = new List<BookViewModel>();
                foreach (var book in books)
                {
                    if (book.GenreId == model.GenreId && book.Author == model.Author)
                        Books.Add(book);
                }
                ViewBag.Books = Books;
                return View("Views/Librarian/ListOfBooks.cshtml");
            }
            //по жанру и автору
            if (model.GenreId != 0 && model.Name == null && model.Author != null)
            {
                var books = _book.Read(new BookBindingModel
                {
                    Author = model.Author,
                    Status = Status.Свободна
                });
                List<BookViewModel> Books = new List<BookViewModel>();
                foreach (var book in books)
                {
                    if (book.GenreId == model.GenreId)
                        Books.Add(book);
                }
                ViewBag.Books = Books;
                return View("Views/Librarian/ListOfBooks.cshtml");
            }
            //по автору и названию
            if (model.GenreId == 0 && model.Name != null && model.Author != null)
            {
                var books = _book.Read(new BookBindingModel
                {
                    Name = model.Name,
                    Status = Status.Свободна
                });
                List<BookViewModel> Books = new List<BookViewModel>();
                foreach (var book in books)
                {
                    if (book.Author == model.Author)
                        Books.Add(book);
                }
                ViewBag.Books = Books;
                return View("Views/Librarian/ListOfBooks.cshtml");
            }
            if (validation.bookSearch(model))
            {
                ModelState.AddModelError("", "Выберите хотя бы один параметр поиска");
                return View("Views/Librarian/ListOfBooks.cshtml");
            }
            return View("Views/Librarian/ListOfBooks.cshtml");
        }
    }
}
