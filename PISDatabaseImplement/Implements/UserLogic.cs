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
    public class UserLogic:IUserLogic
    {
        public void CreateOrUpdate(UserBindingModel model)
        {
            using (var context = new DatabaseContext())
            {
                User element = model.Id.HasValue ? null : new User();
                if (model.Id.HasValue)
                {
                    element = context.Users.FirstOrDefault(rec => rec.Id == model.Id);
                    if (element == null)
                    {
                        throw new Exception("Элемент не найден");
                    }
                }
                else
                {
                    element = new User();
                    context.Users.Add(element);
                }
                element.Email = model.Email;
                element.Role = model.Role;
                element.FIO = model.FIO;
                element.Password = model.Password;
                element.ComissionPercent = model.ComissionPercent;
                element.Comission = model.Comission;
                element.Salary = model.Salary;
                context.SaveChanges();
            }
        }
        public void Delete(UserBindingModel model)
        {
            using (var context = new DatabaseContext())
            {
                User element = context.Users.FirstOrDefault(rec => rec.Id == model.Id);

                if (element != null)
                {
                    context.Users.Remove(element);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Элемент не найден");
                }
            }
        }
        public List<UserViewModel> Read(UserBindingModel model)
        {
            using (var context = new DatabaseContext())
            {
                return context.Users
                 .Where(rec => model == null
                   || (rec.Id == model.Id) ||(model.FIO == rec.FIO)
                   || (rec.Email == model.Email)
                        && (model.Password == null || rec.Password == model.Password))
               .Select(rec => new UserViewModel
               {
                   Id = rec.Id,
                   FIO = rec.FIO,
                   Email = rec.Email,
                   Role=rec.Role,
                   Password = rec.Password,
                   Comission = rec.Comission,
                   ComissionPercent = rec.ComissionPercent,
                   Salary = rec.Salary
               })
                .ToList();
            }
        }
    }
}
