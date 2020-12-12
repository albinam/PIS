using Microsoft.AspNetCore.Mvc;
using PISBusinessLogic.BindingModels;
using PISBusinessLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PISCoursework.Controllers.Reader
{
    public class BookingsController : Controller
    {
        private readonly IBookingLogic _booking;
        public BookingsController(IBookingLogic booking)
        {
            _booking = booking;
        }

        /*   public IActionResult AddBooking()
           {
               ViewBag.Booking = _booking.CreateOrUpdate(_booking);
               return View("Views/Reader/AddBooking.cshtml");
           }
        */
        [HttpPost]
        public ActionResult AddBooking(BookingBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Booking = _booking.Read(null);
                return View(model);
            }
            if (model.DateFrom == null)
            {
                ViewBag.Booking = _booking.Read(null);
                ModelState.AddModelError("", "Введите дату начала бронирования");
                return View("Views/Reader/AddBooking.cshtml");
            }
            if (model.DateTo == null)
            {
                ViewBag.Booking = _booking.Read(null);
                ModelState.AddModelError("", "Введите дату окончания бронирования");
                return View("Views/Reader/AddBooking.cshtml");
            }
            _booking.CreateOrUpdate(new BookingBindingModel
            {
                DateFrom = model.DateFrom,
                DateTo = model.DateTo,
                BookId = model.BookId,
                LibraryCardId = model.LibraryCardId
            });

            return RedirectToAction("ListOfBookings");
        }
    }
}
