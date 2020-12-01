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
    public  class GenreLogic:IGenreLogic
    {
        public void CreateOrUpdate(GenreBindingModel model)
        {
            using (var context = new DatabaseContext())
            {
                Genre element = model.Id.HasValue ? null : new Genre();
                if (model.Id.HasValue)
                {
                    element = context.Genres.FirstOrDefault(rec => rec.Id == model.Id);
                    if (element == null)
                    {
                        throw new Exception("Элемент не найден");
                    }
                }
                else
                {
                    element = new Genre();
                    context.Genres.Add(element);
                }
                element.Name = model.Name;
                element.Price = model.Price;       
                context.SaveChanges();
            }
        }
        public void Delete(GenreBindingModel model)
        {
            using (var context = new DatabaseContext())
            {
                Genre element = context.Genres.FirstOrDefault(rec => rec.Id == model.Id);

                if (element != null)
                {
                    context.Genres.Remove(element);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Элемент не найден");
                }
            }
        }
        public List<GenreViewModel> Read(GenreBindingModel model)
        {
            using (var context = new DatabaseContext())
            {
                return context.Genres
                 .Where(rec => model == null
                   || rec.Id == model.Id)
               .Select(rec => new GenreViewModel
               {
                   Id = rec.Id,
                   Name = rec.Name,
                   Price = rec.Price,
               })
                .ToList();
            }
        }
    }
}

