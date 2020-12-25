using Microsoft.AspNetCore.Mvc;
using PISBusinessLogic;
using PISBusinessLogic.BindingModels;
using PISBusinessLogic.BusinessLogic;
using PISBusinessLogic.Enums;
using PISBusinessLogic.HelperModels;
using PISBusinessLogic.Interfaces;
using PISBusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PISCourseworkARMReader.Controllers.Reader
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
        public ActionResult BookSearch(BookBindingModel model)
        {
            ViewBag.Genres = _genre.Read(null);
            var freebooks = _book.Read(null);
            var Bookings = _booking.Read(null);
            List<BookingViewModel> bookings = new List<BookingViewModel>();
            List<BookViewModel> books = new List<BookViewModel>();
            var card = _libraryCard.Read(new LibraryCardBindingModel
            {
                UserId = Program.Reader.Id
            }).FirstOrDefault();
            List<BookViewModel> freeBooks = new List<BookViewModel>();
            foreach (var booking in Bookings)
            {
                var b = _book.Read(new BookBindingModel
                {
                    Id = booking.BookId
                }).FirstOrDefault();
                if (booking.DateTo < DateTime.Now)
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
                if (booking.LibraryCardId == card.Id)
                {
                    bookings.Add(booking);
                }
            }
            foreach (var book in freebooks)
            {
                foreach (var b in bookings)
                {
                    if (book.Id == b.BookId)
                    {
                        books.Add(book);
                    }
                }
            }          
            freeBooks = freebooks;
            foreach (var b in books)
            {
                freeBooks.Remove(b);
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
    }
}
