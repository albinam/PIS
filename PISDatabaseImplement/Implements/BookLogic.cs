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
    public class BookLogic:IBookLogic
    {
        public void CreateOrUpdate(BookBindingModel model)
        {
            using (var context = new DatabaseContext())
            {
                Book element = model.Id.HasValue ? null : new Book();
                if (model.Id.HasValue)
                {
                    element = context.Books.FirstOrDefault(rec => rec.Id == model.Id);
                    if (element == null)
                    {
                        throw new Exception("Элемент не найден");
                    }
                }
                else
                {
                    element = new Book();
                    context.Books.Add(element);
                }
                element.Name = model.Name;
                element.Author = model.Author;
                element.Year = model.Year;
                element.PublishingHouse = model.PublishingHouse;
                element.GenreId = model.GenreId;
                element.Status = model.Status;
                context.SaveChanges();
            }
        }
        public void Delete(BookBindingModel model)
        {
            using (var context = new DatabaseContext())
            {
                Book element = context.Books.FirstOrDefault(rec => rec.Id == model.Id);

                if (element != null)
                {
                    context.Books.Remove(element);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Элемент не найден");
                }
            }
        }
        public List<BookViewModel> Read(BookBindingModel model)
        {
            using (var context = new DatabaseContext())
            {
                return context.Books
                 .Where(rec => model == null
                   || rec.Id == model.Id ||(rec.Name==model.Name && rec.Status==model.Status) || (rec.GenreId==model.GenreId && rec.Status == model.Status) || (rec.Author == model.Author && rec.Status == model.Status))             
               .Select(rec => new BookViewModel
               {
                   Id = rec.Id,
                   Author = rec.Author,
                   Name = rec.Name,
                   PublishingHouse = rec.PublishingHouse,
                   Year = rec.Year,
                   GenreId=rec.GenreId,
                   Status=rec.Status
               })
                .ToList();
            }
        }
    }
}
