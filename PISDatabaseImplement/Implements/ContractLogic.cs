using Microsoft.EntityFrameworkCore;
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
    public class ContractLogic : IContractLogic
    {
        public void CreateOrUpdate(ContractBindingModel model)
        {
            using (var context = new DatabaseContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        Contract element = model.Id.HasValue ? null : new Contract();
                        if (model.Id.HasValue)
                        {
                            element = context.Contracts.FirstOrDefault(rec => rec.Id ==
                           model.Id);
                            if (element == null)
                            {
                                throw new Exception("Элемент не найден");
                            }
                            element.LibraryCardId = model.LibraryCardId;
                            element.Date = model.Date;
                            element.Sum = model.Sum;
                            element.LibrarianId = model.LibrarianId;
                            element.DateReturn = model.DateReturn;
                            element.ContractStatus = model.ContractStatus;
                            element.Fine = model.Fine;
                            var ContractBooks = context.ContractBooks.Where(rec
                          => rec.ContractId == model.Id.Value).ToList();
                            // удалили те, которых нет в модели
                            List<ContractBook> list = new List<ContractBook>();
                            foreach (var cb in model.ContractBooks)
                            {
                                foreach (var cb2 in ContractBooks)
                                {
                                    if (cb.BookId == cb2.BookId)
                                    {
                                        list.Add(cb2);
                                    }
                                }
                            }
                            context.ContractBooks.RemoveRange(list);                        
                            context.SaveChanges();                           
                            var groupBooks = model.ContractBooks
                              .GroupBy(rec => rec.BookId)
                              .Select(rec => new
                              {
                                  BookId = rec.Key
                              });

                            foreach (var groupBook in groupBooks)
                            {
                                context.ContractBooks.Add(new ContractBook
                                {
                                    ContractId = element.Id,
                                    BookId = groupBook.BookId
                                });
                                context.SaveChanges();
                            }
                            context.SaveChanges();
                        }

                        else
                        {
                            element.LibraryCardId = model.LibraryCardId;
                            element.Date = model.Date;
                            element.Sum = model.Sum;
                            element.LibrarianId = model.LibrarianId;
                            element.DateReturn = model.DateReturn;
                            element.Fine = model.Fine;
                            element.ContractStatus = model.ContractStatus;
                            context.Contracts.Add(element);
                            context.SaveChanges();
                            var groupBooks = model.ContractBooks
                               .GroupBy(rec => rec.BookId)
                               .Select(rec => new
                               {
                                   BookId = rec.Key
                               });

                            foreach (var groupBook in groupBooks)
                            {
                                context.ContractBooks.Add(new ContractBook
                                {
                                    ContractId = element.Id,
                                    BookId = groupBook.BookId
                                });
                                context.SaveChanges();
                            }
                        }
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        public void Delete(ContractBindingModel model)
        {
            using (var context = new DatabaseContext())
            {
                Contract element = context.Contracts.FirstOrDefault(rec => rec.Id == model.Id.Value);

                if (element != null)
                {
                    context.Contracts.Remove(element);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Элемент не найден");
                }
            }
        }
        public List<ContractViewModel> Read(ContractBindingModel model)
        {
            using (var context = new DatabaseContext())
            {
                return context.Contracts.Where(rec => model == null
                   || rec.Id == model.Id || (rec.LibraryCardId == model.LibraryCardId) || (rec.LibrarianId == model.LibrarianId))
                .Select(rec => new ContractViewModel
                {
                    Id = rec.Id,
                    LibraryCardId = rec.LibraryCardId,
                    ReaderFIO = rec.LibraryCard.Reader.FIO,
                    LibrarianId = rec.LibrarianId,
                    LibrarianFIO = rec.Librarian.FIO,
                    DateReturn = rec.DateReturn,
                    ContractStatus=rec.ContractStatus,
                    Fine = rec.Fine,
                    Sum = rec.Sum,
                    Date = rec.Date,
                    ContractBooks = GetContractBookViewModel(rec)
                })
            .ToList();
            }
        }
        public static List<ContractBookViewModel> GetContractBookViewModel(Contract Contract)
        {
            using (var context = new DatabaseContext())
            {
                var ContractBooks = context.ContractBooks
                    .Where(rec => rec.ContractId == Contract.Id)
                    .Include(rec => rec.Book)
                    .Select(rec => new ContractBookViewModel
                    {
                        Id = rec.Id,
                        ContractId = rec.ContractId,
                        BookId = rec.BookId,
                    }).ToList();
                foreach (var Book in ContractBooks)
                {
                    var BookData = context.Books.Where(rec => rec.Id == Book.BookId).FirstOrDefault();
                    Book.Author = BookData.Author;
                    Book.Name = BookData.Name;
                    Book.PublishingHouse = BookData.PublishingHouse;
                    Book.Year = BookData.Year;
                    Book.GenreId = BookData.GenreId;
                    Book.Status = BookData.Status;
                }
                return ContractBooks;
            }
        }
    }
}