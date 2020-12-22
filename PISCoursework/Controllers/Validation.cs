using PISBusinessLogic.BindingModels;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public bool сhangeCommissionAll( string ComissionPercentAll)
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
        public bool addSalar(PaymentBindingModel model, int Id )
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
    }
}
