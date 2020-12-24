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
    public class LibraryCardLogic : ILibraryCardLogic
    {
        public void CreateOrUpdate(LibraryCardBindingModel model)
        {
            using (var context = new DatabaseContext())
            {
                LibraryCard element = model.Id.HasValue ? null : new LibraryCard();
                if (model.Id.HasValue)
                {
                    element = context.LibraryCards.FirstOrDefault(rec => rec.Id == model.Id);
                    if (element == null)
                    {
                        throw new Exception("Элемент не найден");
                    }
                }
                else
                {
                    element = new LibraryCard();
                    context.LibraryCards.Add(element);
                }
                element.DateOfBirth = model.DateOfBirth;
                element.Year = model.Year;
                element.PlaceOfWork = model.PlaceOfWork;
                element.UserId = model.UserId.Value;
                context.SaveChanges();
            }
        }
        public void Delete(LibraryCardBindingModel model)
        {
            using (var context = new DatabaseContext())
            {
                LibraryCard element = context.LibraryCards.FirstOrDefault(rec => model == null || rec.Id == model.Id);

                if (element != null)
                {
                    context.LibraryCards.Remove(element);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Элемент не найден");
                }
            }
        }
        public List<LibraryCardViewModel> Read(LibraryCardBindingModel model)
        {
            using (var context = new DatabaseContext())
            {
                return context.LibraryCards
                 .Where(rec => model == null || rec.Id == model.Id || (rec.UserId == model.UserId && model.UserId.HasValue))
               .Select(rec => new LibraryCardViewModel
               {
                   Id = rec.Id,
                   DateOfBirth = rec.DateOfBirth,
                   Year = rec.Year,
                   PlaceOfWork = rec.PlaceOfWork,
                   UserId = rec.UserId,
                   ReaderFIO=rec.Reader.FIO,
               })
                .ToList();
            }
        }
    }
}
