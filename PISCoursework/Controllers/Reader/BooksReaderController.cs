using Microsoft.AspNetCore.Mvc;
using PISBusinessLogic;
using PISBusinessLogic.BindingModels;
using PISBusinessLogic.Enums;
using PISBusinessLogic.HelperModels;
using PISBusinessLogic.Interfaces;
using PISBusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PISCoursework.Controllers.Reader
{
    public class BooksReaderController : Controller
    {
        private readonly IBookLogic _book;
        private readonly IGenreLogic _genre;
        private readonly IUserLogic _user;
        private readonly ILibraryCardLogic _libraryCard;
        private readonly IContractLogic _contract;
        private readonly IBookingLogic _booking;
        private Validation validation;
        private readonly ReportLogic _report;
        public BooksReaderController(IBookLogic book, IGenreLogic genre, IUserLogic user, ILibraryCardLogic libraryCard, IContractLogic contract, ReportLogic report, IBookingLogic booking)
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

        [HttpGet]
        public ActionResult ListOfBooksReader(BookBindingModel model)
        {
            ViewBag.Genres = _genre.Read(null);
            ViewBag.Books = _book.Read(new BookBindingModel
            {
                Status = Status.Свободна
            });
            return BookSearch(model);

        }
        //public ActionResult BookPrice(int GenreId, string Percent)
        //{
        //    if (validation.bookPrice(GenreId, Percent))
        //    {
        //        ViewBag.Genres = _genre.Read(null);
        //        var genre = _genre.Read(new GenreBindingModel
        //        {
        //            Id = GenreId
        //        }).FirstOrDefault();
        //        Percent = Percent.Replace(".", ",");
        //        double percent = Convert.ToDouble(Percent);
        //        _genre.CreateOrUpdate(new GenreBindingModel
        //        {
        //            Id = GenreId,
        //            Name = genre.Name,
        //            Price = genre.Price * percent
        //        });
        //        ModelState.AddModelError("", "Цена успешно изменена");
        //        return View("Views/Librarian/BookPrice.cshtml");
        //    }
        //    else
        //    {
        //        ViewBag.Genres = _genre.Read(null);
        //        ModelState.AddModelError("", "Выберите жанр и/или введите коэффициент изменения");
        //        return View("Views/Librarian/BookPrice.cshtml");
        //    }
        //}
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
                }).FirstOrDefault();
                if (booking.DateTo > DateTime.Now)
                {
                    if (b.Status == Status.Забронирована)
                    {
                        freeBooks.Add(b);
                    }
                }
                else
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
                return View("Views/Reader/ListOfBooksReader.cshtml");
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
                return View("Views/Reader/ListOfBooksReader.cshtml");
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
                return View("Views/Reader/ListOfBooksReader.cshtml");
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
                return View("Views/Reader/ListOfBooksReader.cshtml");
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
                return View("Views/Reader/ListOfBooksReader.cshtml");
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
                return View("Views/Reader/ListOfBooksReader.cshtml");
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
                return View("Views/Reader/ListOfBooksReader.cshtml");
            }
            if (validation.bookSearch(model))
            {
                ModelState.AddModelError("", "Выберите хотя бы один параметр поиска");
                return View("Views/Reader/ListOfBooksReader.cshtml");
            }
            return View("Views/Reader/ListOfBooksReader.cshtml");
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
                return View("Views/Reader/BooksReader.cshtml");
            }
            if (type == 2)
            {
                ViewBag.Genres = _genre.Read(null);
                List<BookViewModel> books = new List<BookViewModel>();
                var Books = _book.Read(null);
                foreach (var book in Books)
                {
                    if (book.Status == Status.Забронирована)
                        books.Add(book);
                }
                ViewBag.Books = books;
                return View("Views/Reader/BooksReader.cshtml");
            }
            if (type == 3)
            {
                ViewBag.Genres = _genre.Read(null);
                var contracts = _contract.Read(null);
                foreach (var c in contracts) { }
                return View("Views/Reader/BooksReader.cshtml");
            }
            return View("Views/Reader/BooksReader.cshtml");
        }
        [HttpGet]
        public ActionResult ListOfContractsReader()
        {
            ViewBag.Genres = _genre.Read(null);
            var Contracts = _contract.Read(null);
            List<ContractViewModel> contracs = new List<ContractViewModel>();

                var card = _libraryCard.Read(new LibraryCardBindingModel
                {
                    UserId = Program.Reader.Id
                }).FirstOrDefault();
            foreach (var cont in Contracts)
            {
                if (cont.LibraryCardId == card.UserId)
                {
                    contracs.Add(cont);
                }
            }
            ViewBag.Contracts = contracs;
            ViewBag.Users = Program.Reader;
            return View("Views/Reader/ListOfContractsReader.cshtml");
        
        }
        public List<ContractBookBindingModel> ConvertModels(List<ContractBookViewModel> list)
        {
            if (list == null) return null;
            List<ContractBookBindingModel> list2 = new List<ContractBookBindingModel>();
            foreach (var l in list)
            {
                list2.Add(new ContractBookBindingModel
                {
                    Id = l.Id,
                    BookId = l.BookId,
                    ContractId = l.ContractId

                });
            }
            return list2;
        }
    }
}
