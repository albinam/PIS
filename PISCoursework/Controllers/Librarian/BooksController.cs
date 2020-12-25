using Microsoft.AspNetCore.Mvc;
using PISBusinessLogic;
using PISBusinessLogic.BindingModels;
using PISBusinessLogic.BusinessLogic;
using PISBusinessLogic.HelperModels;
using PISBusinessLogic.Interfaces;
using PISBusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly IBookingLogic _booking;
        private Validation validation;
        private readonly ReportLogic _report;
        public BooksController(IBookLogic book, IGenreLogic genre, IUserLogic user, ILibraryCardLogic libraryCard, IContractLogic contract, ReportLogic report, IBookingLogic booking)
        {
            _book = book;
            _genre = genre;
            _user = user;
            _libraryCard = libraryCard;
            _contract = contract;
            _report = report;
            _booking = booking;
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
            if (validation.bookPrice(GenreId, Percent))
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
            var bookings = _booking.Read(null);
            List<BookViewModel> freeBooks = new List<BookViewModel>();
            foreach (var booking in bookings)
            {
                var b = _book.Read(new BookBindingModel
                {
                    Id = booking.BookId
                }).LastOrDefault();
                if (booking.DateTo > DateTime.Now)
                {
                    if (b.Status == Status.Забронирована)
                    {
                        if (!freeBooks.Contains(b))
                        {
                            freeBooks.Add(b);
                        }
                    }
                }
                else if (booking.DateTo < DateTime.Now)
                {
                    _book.CreateOrUpdate(new BookBindingModel
                    {
                        Id = b.Id,
                        Name = b.Name,
                        PublishingHouse = b.PublishingHouse,
                        Author = b.Author,
                        Year = b.Year,
                        GenreId = b.GenreId,
                        Status = Status.Свободна
                    });
                    _booking.Delete(new BookingBindingModel
                    {
                        Id = booking.Id
                    });
                }
            }
            foreach (var book in freebooks)
            {
                if (book.Status == Status.Свободна)
                    freeBooks.Add(book);
            }

            ViewBag.Books = freeBooks;
            List<BookViewModel> removebooks = new List<BookViewModel>();
            //по названию
            if (model.Name != null && model.GenreId == 0 && model.Author == null)
            {
                foreach (var el in freeBooks)
                {
                    if (el.Name != model.Name)
                    {
                        removebooks.Add(el);
                    }
                }
                foreach (var book in removebooks)
                {
                    freeBooks.Remove(book);
                }
                ViewBag.Books = freeBooks;
                return View("Views/Librarian/ListOfBooks.cshtml");
            }
            //по жанру
            if (model.GenreId != 0 && model.Name == null && model.Author == null)
            {
                foreach (var el in freeBooks)
                {
                    if (el.GenreId != model.GenreId)
                    {
                        removebooks.Add(el);
                    }
                }
                foreach (var book in removebooks)
                {
                    freeBooks.Remove(book);
                }
                ViewBag.Books = freeBooks;
                return View("Views/Librarian/ListOfBooks.cshtml");
            }
            //по автору
            if (model.GenreId == 0 && model.Name == null && model.Author != null)
            {
                foreach (var el in freeBooks)
                {
                    if (el.Author != model.Author)
                    {
                        removebooks.Add(el);
                    }
                }
                foreach (var book in removebooks)
                {
                    freeBooks.Remove(book);
                }
                ViewBag.Books = freeBooks;
                return View("Views/Librarian/ListOfBooks.cshtml");
            }
            //по жанру и названию
            if (model.GenreId != 0 && model.Name != null && model.Author == null)
            {
                foreach (var el in freeBooks)
                {
                    if (el.GenreId != model.GenreId || el.Name != model.Name)
                    {
                        removebooks.Add(el);
                    }
                }
                foreach (var book in removebooks)
                {
                    freeBooks.Remove(book);
                }
                ViewBag.Books = freeBooks;
                return View("Views/Librarian/ListOfBooks.cshtml");
            }
            // по всем трем
            if (model.GenreId != 0 && model.Name != null && model.Author != null)
            {
                foreach (var el in freeBooks)
                {
                    if (el.GenreId != model.GenreId || el.Name != model.Name || el.Author != model.Author)
                    {
                        removebooks.Add(el);
                    }
                }
                foreach (var book in removebooks)
                {
                    freeBooks.Remove(book);
                }
                ViewBag.Books = freeBooks;
                return View("Views/Librarian/ListOfBooks.cshtml");
            }
            //по жанру и автору
            if (model.GenreId != 0 && model.Name == null && model.Author != null)
            {
                foreach (var el in freeBooks)
                {
                    if (el.GenreId != model.GenreId || el.Author != model.Author)
                    {
                        removebooks.Add(el);
                    }
                }
                foreach (var book in removebooks)
                {
                    freeBooks.Remove(book);
                }
                ViewBag.Books = freeBooks;
                return View("Views/Librarian/ListOfBooks.cshtml");
            }
            //по автору и названию
            if (model.GenreId == 0 && model.Name != null && model.Author != null)
            {
                foreach (var el in freeBooks)
                {
                    if (el.Name != model.Name || el.Author != model.Author)
                    {
                        removebooks.Add(el);
                    }
                }
                foreach (var book in removebooks)
                {
                    freeBooks.Remove(book);
                }
                ViewBag.Books = freeBooks;
                return View("Views/Librarian/ListOfBooks.cshtml");
            }
            return View("Views/Librarian/ListOfBooks.cshtml");
        }

        public ActionResult Books(int type)
        {
            ViewBag.Type = type;
            //свободные и выданные
            if (type == 1)
            {
                ViewBag.Genres = _genre.Read(null);
                List<BookViewModel> books = new List<BookViewModel>();
                var Books = _book.Read(null);
                foreach (var book in Books)
                {
                    if (book.Status == Status.Выдана || book.Status == Status.Свободна)
                        books.Add(book);
                }
                ViewBag.Books = books;
                return View("Views/Librarian/Books.cshtml");
            }
            if (type == 2)
            {
                ViewBag.Genres = _genre.Read(null);
                List<BookViewModel> books = new List<BookViewModel>();
                var Books = _book.Read(null);
                var Bookings = _booking.Read(null);
                foreach (var book in Books)
                {
                    foreach (var booking in Bookings)
                    {
                        if (book.Id == booking.BookId)
                        {
                            books.Add(book);
                        }
                    }
                }
                ViewBag.Books = books;
                return View("Views/Librarian/Books.cshtml");
            }
            if (type == 3)
            {
                ViewBag.Genres = _genre.Read(null);
                ViewBag.Books = GetFavoriteCategories();
                return View("Views/Librarian/Books.cshtml");
            }
            return View("Views/Librarian/Books.cshtml");
        }
        private List<BookViewModel> GetFavoriteCategories()
        {
            Dictionary<int, int> productCategories = new Dictionary<int, int>();
            foreach (var contract in _contract.Read(null))
            {
                foreach (var cb in contract.ContractBooks)
                {
                    var book = _book.Read(new BookBindingModel
                    {
                        Id = cb.BookId
                    }).FirstOrDefault();
                    if (productCategories.ContainsKey(book.Id))
                    {
                        productCategories[book.Id] += 1;
                    }
                    else
                    {
                        productCategories[book.Id] = 1;
                    }
                }
            }
            var categories = productCategories.ToList();
            categories.Sort((p1, p2) => p2.Value.CompareTo(p1.Value));
            var result = new List<BookViewModel>();
            foreach (var cat in categories)
            {
                var book = _book.Read(new BookBindingModel
                {
                    Id = cat.Key
                }).FirstOrDefault();
                result.Add(book);
            }
            return result;
        }   
    }
}
