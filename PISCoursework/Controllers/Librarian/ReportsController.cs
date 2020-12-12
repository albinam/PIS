using Microsoft.AspNetCore.Mvc;
using PISBusinessLogic;
using PISBusinessLogic.BindingModels;
using PISBusinessLogic.Interfaces;
using PISBusinessLogic.ViewModels;
using PISCoursework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PISCoursework.Controllers.Librarian
{
    public class ReportsController : Controller
    {
        private readonly IBookLogic _book;
        private readonly IGenreLogic _genre;
        private readonly IUserLogic _user;
        private readonly ILibraryCardLogic _libraryCard;
        private readonly IContractLogic _contract;
        public ReportsController(IBookLogic book, IGenreLogic genre, IUserLogic user, ILibraryCardLogic libraryCard, IContractLogic contract)
        {
            _book = book;
            _genre = genre;
            _user = user;
            _libraryCard = libraryCard;
            _contract = contract;
        }
        public IActionResult Reports()
        {
            return View("Views/Librarian/Reports.cshtml");
        }
        public IActionResult Charts()
        {
            return View("Views/Librarian/Charts.cshtml");
        }
        public IActionResult DateReport()
        {
            return View("Views/Librarian/DateReport.cshtml");
        }
        [HttpPost]
        public ActionResult DateReport(DateTime month)
        {
            DateTime date = new DateTime();
            if (month == date)
            {
                return View("Views/Librarian/DateReport.cshtml");
            }
            int month2 = month.Month;
            var Contracts = _contract.Read(null);
            List<ContractViewModel> contracts = new List<ContractViewModel>();
            foreach (var contract in Contracts)
            {
                if (contract.Date.Month == month2)
                {
                    contracts.Add(contract);
                }
            }
            List<DateTime> dates = new List<DateTime>();
            List<double> sums = new List<double>();
            List<DateTime> dates2 = new List<DateTime>();
            List<double> sums2 = new List<double>();
            var last = _contract.Read(null).LastOrDefault();
            double sum = 0;
            foreach (var c in contracts)
            {          
                    dates.Add(c.Date.Date);              
                    sums.Add(c.Sum+c.Fine);                   
            }
            dates.Add(date);
            for (int i = 0; i < dates.Count-1; i++)
            {
                if (dates[i] != dates[i + 1])
                {
                    sum += sums[i];
                    sums2.Add(sum);
                    sum = 0;
                }
                else
                {
                    sum += sums[i];
                }
            }
            dates.LastOrDefault();
            dates.Remove(dates.LastOrDefault());
            dates = dates.Distinct().ToList();
            ViewBag.Dates = dates;
            ViewBag.Sum = sums2;
            return View("Views/Librarian/DateReport.cshtml");
        }
        [HttpGet]
        public JsonResult Chart()
        {
            var Genres = _genre.Read(null);
            List<Chart> chart = new List<Chart>();
            foreach (var genre in Genres)
            {
                int count = 0;
                var Books = _book.Read(null);
                foreach (var book in Books)
                {
                    if (book.Status == Status.Выдана && book.GenreId == genre.Id)
                    {
                        count++;
                    }
                }
                chart.Add(new Chart { GenreName = genre.Name, Count = count });
            }
            return Json(chart);
        }      
    }
}
