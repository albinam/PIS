using Microsoft.AspNetCore.Mvc;
using PISBusinessLogic;
using PISBusinessLogic.BindingModels;
using PISBusinessLogic.BusinessLogic;
using PISBusinessLogic.Interfaces;
using PISBusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PISCourseworkARMReader.Controllers.Reader
{
    public class BookingsController : Controller
    {
        private readonly IBookingLogic _booking;
        private readonly IBookLogic _book;
        private readonly IUserLogic _user;
        private readonly ILibraryCardLogic _libraryCard;
        private readonly IContractLogic _contract;
        private readonly IGenreLogic _genre;
        private readonly Validation validation;
        public BookingsController(IBookingLogic booking, IBookLogic book, IUserLogic user, ILibraryCardLogic libraryCard, IContractLogic contract, IGenreLogic genre)
        {
            _booking = booking;
            _book = book;
            _user = user;
            _libraryCard = libraryCard;
            _contract = contract;
            _genre = genre;
            validation = new Validation();
        }
        public ActionResult ListOfBookings()
        {
            ViewBag.Genres = _genre.Read(null);
            List<BookViewModel> books = new List<BookViewModel>();
            List<BookingViewModel> bookings = new List<BookingViewModel>();
            var Books = _book.Read(null);
            var Bookings = _booking.Read(null);
            var card = _libraryCard.Read(new LibraryCardBindingModel
            {
                UserId = Program.Reader.Id
            }).FirstOrDefault();
            foreach (var booking in Bookings)
            {
                if (booking.DateTo >= DateTime.Now)
                {
                    if (booking.LibraryCardId == card.Id)
                    {
                        bookings.Add(booking);
                    }
                }
                else
                {
                    _booking.Delete(new BookingBindingModel
                    {
                        Id = booking.Id
                    });
                }
            }
            foreach (var book in Books)
            {
                foreach (var b in bookings)
                {
                    if (book.Id == b.BookId)
                    {
                        books.Add(book);
                    }
                }
            }        
            ViewBag.Books = books;
            return View("Views/Reader/ListOfBookings.cshtml");
        }
        public IActionResult AddBooking(int id)
        {
            ViewBag.BookId = id;
            return View("Views/Reader/AddBooking.cshtml");
        }

        [HttpPost]
        public ActionResult AddBooking( BookingBindingModel model)
        {
            ViewBag.BookId = model.Id;
            if (validation.bookingValidation(model)!="")
            {
                ViewBag.Booking = _booking.Read(null);
                ModelState.AddModelError("", validation.bookingValidation(model));
                return View("Views/Reader/AddBooking.cshtml");
            }
           
            var libraryCard = _libraryCard.Read(new LibraryCardBindingModel
            {
                UserId = Program.Reader.Id
            }).FirstOrDefault();
            if (Convert.ToInt32(libraryCard.Year) < DateTime.Now.Year)
            {
                ViewBag.Booking = _booking.Read(null);
                ModelState.AddModelError("","Ваш читательский билет просрочен, обратитесь в библиотеку");
                return View("Views/Reader/AddBooking.cshtml");
            }
            _booking.CreateOrUpdate(new BookingBindingModel
            {
                DateFrom = model.DateFrom,
                DateTo = model.DateTo,
                BookId = model.BookId,
                LibraryCardId = libraryCard.Id
            });
            var book = _book.Read(new BookBindingModel
            {
                Id = model.BookId
            }).FirstOrDefault();
            if (book.Status == Status.Свободна)
            {
                _book.CreateOrUpdate(new BookBindingModel
                {
                    Id = book.Id,
                    PublishingHouse = book.PublishingHouse,
                    Author = book.Author,
                    Year = book.Year,
                    GenreId = book.GenreId,
                    Name = book.Name,
                    Status = Status.Забронирована
                });
            }
            return RedirectToAction("ListOfBookings");
        }
    }
}
