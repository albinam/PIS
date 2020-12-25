using PISBusinessLogic.BindingModels;
using PISBusinessLogic.BusinessLogic;
using PISBusinessLogic.Enums;
using PISBusinessLogic.HelperModels;
using PISBusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PISCoursework.Controllers
{
    public class Validation
    {
        public string addBook(BookBindingModel model)
        {
            if (model.Name == null)
            {
                return "Введите название";
            }
            if (model.Author == null)
            {
                return "Введите автора";
            }
            if (model.PublishingHouse == null)
            {
                return "Введите издательство";
            }
            if (model.Year == null)
            {
                return "Введите год издания";
            }
            return "";
        }
        public bool bookPrice(int GenreId, string Percent)
        {
            if (GenreId != 0 && Percent != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool bookSearch(BookBindingModel model)
        {
            if (model.GenreId == 0 && model.Name == null && model.Author == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string addLibraryCard(LibraryCardBindingModel model)
        {
            if (model.DateOfBirth == null)
            {
                return "Введите дату рождения";
            }
            if (model.DateOfBirth > DateTime.Now.Date)
            {
                return "Дата рождения не может быть позднее нынешней даты";
            }
            if (model.PlaceOfWork == null)
            {
                return "Введите место работы";
            }
            return "";
        }
        public bool readersWithOverdue(DateTime date)
        {
            var dat1 = new DateTime();
            if (date == dat1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool сhangeCommission(int Id, string ComissionPercent)
        {
            if (Id != 0 && ComissionPercent != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool сhangeCommissionAll(string ComissionPercentAll)
        {
            if (ComissionPercentAll != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool addSalar(PaymentBindingModel model, int Id)
        {
            if (Id != 0 && model.Date != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool salaryAll(DateTime date)
        {
            var dat1 = new DateTime();
            if (date == dat1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string periodCheck(DateTime date)
        {
            if (date.Date < DateTime.Now.Date)
            {
                return "Дата не может быть меньше или равна нынешней даты";
            }
            return "";
        }
        public string userCheck(UserBindingModel user, UserViewModel userView)
        {
            EncryptionLogic _enc = new EncryptionLogic();
           
            if (user != null)
            {
                if (user.Password == null)
                {
                    return "Введите пароль";

                }
                if (user.Email == null)
                {
                    return "Введите эл.почту";

                }
            }
            if (userView != null)
            {
                if (userView.Role == Roles.Библиотекарь || userView.Role == Roles.Бухгалтер)
                {
                    if (user.Password != userView.Password)
                    {
                        return "Вы ввели неверный пароль";
                    }
                }
                else
                {
                    if (user.Password != _enc.Decrypt(userView.Password, userView.Email))
                    {
                        return "Вы ввели неверный пароль";
                    }
                }
            }
            return "";
        }
        public string registrationCheck(UserBindingModel user)
        {
            if (String.IsNullOrEmpty(user.Email))
            {
                return "Введите электронную почту";
            }
            if (!Regex.IsMatch(user.Email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
            {
                return "Email введен некорректно";
            }
            if (String.IsNullOrEmpty(user.FIO))
            {
                return "Введите ФИО";
            }
            if (String.IsNullOrEmpty(user.Password))
            {
                return "Введите пароль";
            }
            if (!Regex.IsMatch(user.Password, @".{8,15}"))
            {
                return "Пароль должен содержать не менее 8 и не более 15 символов";
            }
            return "";
        }
        public string bookingValidation(BookingBindingModel model)
        {
            if (model.DateFrom == new DateTime())
            {           
                return "Введите дату начала бронирования";
            }
            if (model.DateTo == new DateTime())
            {           
                return "Введите дату окончания бронирования";
            }
            if (model.DateTo < model.DateFrom)
            {
                return "Дата окончания бронирования не может быть меньше даты начала бронирования";
            }
            return "";
        }
        public bool leadSalary(DateTime month)
        {
            var dat1 = new DateTime();
            if (month == dat1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool checkLibrarian(int CountReport, int Id)
        {
            if (Id != 0 && CountReport >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool listContract(int Id, DateTime month)
        {
            var dat1 = new DateTime();
            if (Id != 0 && month != dat1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool distributionSalary(DateTime date)
        {
            var dat1 = new DateTime();
            if (date == dat1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool checkSalary(PaymentBindingModel model, string sum)
        {
            if (model.UserId != 0 && sum !=null && model.Date != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
