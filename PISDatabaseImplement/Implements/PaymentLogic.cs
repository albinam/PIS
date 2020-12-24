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
    public class PaymentLogic: IPaymentLogic
    {
        public void CreateOrUpdate(PaymentBindingModel model)
        {
            using (var context = new DatabaseContext())
            {
                Payment element = model.Id.HasValue ? null : new Payment();
                if (model.Id.HasValue)
                {
                    element = context.Payments.FirstOrDefault(rec => rec.Id == model.Id);
                    if (element == null)
                    {
                        throw new Exception("Элемент не найден");
                    }
                }
                else
                {
                    element = new Payment();
                    context.Payments.Add(element);
                }
                element.Sum = model.Sum;
                element.Date = model.Date;
                element.UserId = model.UserId;
                context.SaveChanges();
            }
        }
        public void Delete(PaymentBindingModel model)
        {
            using (var context = new DatabaseContext())
            {
                Payment element = context.Payments.FirstOrDefault(rec => rec.Id == model.Id);

                if (element != null)
                {
                    context.Payments.Remove(element);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Элемент не найден");
                }
            }
        }
        public List<PaymentViewModel> Read(PaymentBindingModel model)
        {
            using (var context = new DatabaseContext())
            {
                return context.Payments
                 .Where(rec => model == null
                   || (rec.Id == model.Id) || (rec.UserId == model.UserId) || (rec.Date.Month == model.Date.Month))

               .Select(rec => new PaymentViewModel
               {
                   Id = rec.Id,
                   Date = rec.Date,
                   Sum = rec.Sum,
                   UserId = rec.UserId,
                   LibrarianFIO = rec.Librarian.FIO
               })
                .ToList();
            }
        }
    }
}
