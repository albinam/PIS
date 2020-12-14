﻿using Microsoft.AspNetCore.Mvc;
using PISBusinessLogic;
using PISBusinessLogic.BindingModels;
using PISBusinessLogic.HelperModels;
using PISBusinessLogic.Interfaces;
using PISBusinessLogic.ViewModels;
using PISCoursework.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly ReportLogic _report;
        public ReportsController(IBookLogic book, IGenreLogic genre, IUserLogic user, ILibraryCardLogic libraryCard, IContractLogic contract, ReportLogic report)
        {
            _book = book;
            _genre = genre;
            _user = user;
            _libraryCard = libraryCard;
            _contract = contract;
            _report = report;
        }
        public IActionResult Reports()
        {
            var contracts = _contract.Read(null);
            contracts.OrderBy(x => x.LibraryCardId);
            contracts.GroupBy(x => x.LibraryCardId);
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
                sums.Add(c.Sum + c.Fine);
            }
            dates.Add(date);
            for (int i = 0; i < dates.Count - 1; i++)
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
        public ActionResult SumByMonths()
        {
            var contracts = _contract.Read(null);
            contracts.OrderBy(x => x.LibraryCardId);
            contracts.Add(new ContractViewModel());

            List<Dictionary<int, (int, int, int, int, int, int, Tuple<int, int, int, int, int, int>)>> dict = new List<Dictionary<int, (int, int, int, int, int, int, Tuple<int, int, int, int, int, int>)>>();

            var readers = _user.Read(null);
            List<LibraryCardViewModel> list = new List<LibraryCardViewModel>();
            foreach (var reader in readers)
            {
                var card = _libraryCard.Read(new LibraryCardBindingModel
                {
                    UserId = reader.Id
                }).FirstOrDefault();
                if (card != null)
                {
                    list.Add(card);
                }

            }
            foreach (var reader in list)
            {
                Dictionary<int, (int, int, int, int, int, int, Tuple<int, int, int, int, int, int>)> count = new Dictionary<int, (int, int, int, int, int, int, Tuple<int, int, int, int, int, int>)>();
                int count1 = 0;
                int count2 = 0;
                int count3 = 0;
                int count4 = 0;
                int count5 = 0;
                int count6 = 0;
                int count7 = 0;
                int count8 = 0;
                int count9 = 0;
                int count10 = 0;
                int count11 = 0;
                int count12 = 0;
                var contract = _contract.Read(new ContractBindingModel
                {
                    LibraryCardId = reader.Id
                });
                foreach (var c in contract)
                {
                    if (c.Date.Month == 1)
                    {
                        count1 += c.ContractBooks.Count;
                    }
                    if (c.Date.Month == 2)
                    {
                        count2 += c.ContractBooks.Count;
                    }
                    if (c.Date.Month == 3)
                    {
                        count3 += c.ContractBooks.Count;
                    }
                    if (c.Date.Month == 4)
                    {
                        count4 += c.ContractBooks.Count;
                    }
                    if (c.Date.Month == 5)
                    {
                        count5 += c.ContractBooks.Count;
                    }
                    if (c.Date.Month == 6)
                    {
                        count6 += c.ContractBooks.Count;
                    }
                    if (c.Date.Month == 7)
                    {

                        count7 += c.ContractBooks.Count;
                    }
                    if (c.Date.Month == 8)
                    {
                        count8 += c.ContractBooks.Count;
                    }
                    if (c.Date.Month == 9)
                    {
                        count9 += c.ContractBooks.Count;
                    }
                    if (c.Date.Month == 10)
                    {
                        count10 += c.ContractBooks.Count;
                    }
                    if (c.Date.Month == 11)
                    {
                        count11 += c.ContractBooks.Count;
                    }
                    if (c.Date.Month == 12)
                    {
                        count12 += c.ContractBooks.Count;
                    }

                }
                Tuple<int, int, int, int, int, int> tuple = Tuple.Create(count7, count8, count9, count10, count11, count12);
                count.Add(reader.Id, (count1, count2, count3, count4, count5, count6, tuple));
                dict.Add(count);
            }
            ViewBag.Count = dict;
            ViewBag.Readers = list;
            return View("Views/Librarian/SumByMonths.cshtml");

        }
      
    }
}
