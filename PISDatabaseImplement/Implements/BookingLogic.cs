using PISBusinessLogic.BindingModels;
using PISBusinessLogic.Interfaces;
using PISBusinessLogic.ViewModels;
using PISDatabaseimplements.Models;
using PISDatabaseImplements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PISDatabaseImplement.Implements
{
    public class BookingLogic : IBookingLogic
    {
        public void CreateOrUpdate(BookingBindingModel model)
        {
            using (var context = new DatabaseContext())
            {
                Booking element = model.Id.HasValue ? null : new Booking();
                if (model.Id.HasValue)
                {
                    element = context.Bookings.FirstOrDefault(rec => rec.Id == model.Id);
                    if (element == null)
                    {
                        throw new Exception("Элемент не найден");
                    }
                }
                else
                {
                    element = new Booking();
                    context.Bookings.Add(element);
                }
                element.DateFrom = model.DateFrom;
                element.DateTo = model.DateTo;
                element.BookId = model.BookId;
                element.LibraryCardId = model.LibraryCardId;
                context.SaveChanges();
            }
        }

        public void Delete(BookingBindingModel model)
        {
            using (var context = new DatabaseContext())
            {
                Booking element = context.Bookings.FirstOrDefault(rec => rec.Id == model.Id);

                if (element != null)
                {
                    context.Bookings.Remove(element);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Элемент не найден");
                }
            }

        }
        public List<BookingViewModel> Read(BookingBindingModel model)
        {
            using (var context = new DatabaseContext())
            {
                return context.Bookings
                 .Where(rec => model == null
                   || rec.Id == model.Id || (rec.BookId == model.BookId) || (rec.LibraryCardId == model.LibraryCardId))
               .Select(rec => new BookingViewModel
               {
                   Id = rec.Id,
                   DateFrom = rec.DateFrom,
                   BookId=rec.BookId,
                   LibraryCardId=rec.LibraryCardId,
                   DateTo = rec.DateTo
               })
                .ToList();
            }
        }
    }
}