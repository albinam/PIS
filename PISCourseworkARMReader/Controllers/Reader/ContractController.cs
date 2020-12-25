using Microsoft.AspNetCore.Mvc;
using PISBusinessLogic.BindingModels;
using PISBusinessLogic.BusinessLogic;
using PISBusinessLogic.Enums;
using PISBusinessLogic.HelperModels;
using PISBusinessLogic.Interfaces;
using PISBusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PISCourseworkARMReader.Controllers.Reader
{
    public class ContractController : Controller
    {
        private readonly IBookLogic _book;
        private readonly IGenreLogic _genre;
        private readonly IUserLogic _user;
        private readonly ILibraryCardLogic _libraryCard;
        private readonly IContractLogic _contract;
        private readonly IBookingLogic _booking;
        private Validation validation;
        private readonly ReportLogic _report;
        string exportDirectory = Directory.GetCurrentDirectory() + "\\wwwroot\\Export1";
        public ContractController(IBookLogic book, IGenreLogic genre, IUserLogic user, ILibraryCardLogic libraryCard, IContractLogic contract, ReportLogic report, IBookingLogic booking)
        {
            _book = book;
            _genre = genre;
            _user = user;
            _libraryCard = libraryCard;
            _contract = contract;
            _report = report;
            _booking = booking;
            validation = new Validation();
        }
        [HttpGet]
        public ActionResult ChangeDate(int id, DateTime date)
        {
            if (id != 0)
            {
                var contract = _contract.Read(new ContractBindingModel
                {
                    Id = id
                }).FirstOrDefault();
                ViewBag.ContractBooks = contract;
                ViewBag.Genres = _genre.Read(null);
                if (date != new DateTime())
                {
                    if (date < contract.Date)
                    {
                        ModelState.AddModelError("", "Дата возврата не может быть меньше даты заключения");
                        return View("Views/Reader/ChangeDate.cshtml");
                    }
                    TimeSpan period = date - contract.Date;
                    _contract.CreateOrUpdate(new ContractBindingModel
                    {
                        Id = contract.Id,
                        Date = contract.Date,
                        DateReturn = date,
                        Sum = getSum(ConvertModels(contract.ContractBooks), period.Days),
                        Fine = contract.Fine,
                        LibraryCardId = contract.LibraryCardId,
                        ContractStatus = contract.ContractStatus,
                        LibrarianId = contract.LibrarianId,
                        ContractBooks = ConvertModels(contract.ContractBooks)
                    });
                    var contract2 = _contract.Read(new ContractBindingModel
                    {
                        Id = id
                    }).FirstOrDefault();
                    ViewBag.ContractBooks = contract2;
                    ViewBag.Genres = _genre.Read(null);
                }
                return View("Views/Reader/ChangeDate.cshtml");
            }
            return View("Views/Reader/ChangeDate.cshtml");
        }
        [HttpGet]
        public ActionResult ListOfContractsReader()
        {
            ViewBag.Genres = _genre.Read(null);
            var Contracts = _contract.Read(null);
            List<ContractViewModel> contracs = new List<ContractViewModel>();
            var card = _libraryCard.Read(new LibraryCardBindingModel
            {
                UserId = Program.Reader.Id
            }).FirstOrDefault();
            foreach (var cont in Contracts)
            {
                if (cont.LibraryCardId == card.Id)
                {
                    contracs.Add(cont);
                }
            }
            ViewBag.Contracts = contracs;
            ViewBag.Users = Program.Reader;
            return View("Views/Reader/ListOfContractsReader.cshtml");

        }
        public List<ContractBookBindingModel> ConvertModels(List<ContractBookViewModel> list)
        {
            if (list == null) return null;
            List<ContractBookBindingModel> list2 = new List<ContractBookBindingModel>();
            foreach (var l in list)
            {
                list2.Add(new ContractBookBindingModel
                {
                    Id = l.Id,
                    BookId = l.BookId,
                    ContractId = l.ContractId

                });
            }
            return list2;
        }
        public double getSum(List<ContractBookBindingModel> contractBooks, int period)
        {
            double sum = 0;
            if (contractBooks != null)
            {
                foreach (var book in contractBooks)
                {
                    var b = _book.Read(new BookBindingModel
                    {
                        Id = book.BookId
                    }).FirstOrDefault();
                    var g = _genre.Read(new GenreBindingModel
                    {
                        Id = b.GenreId
                    }).FirstOrDefault();
                    sum += g.Price * period;
                }
                return sum;
            }
            else
            {
                return 0;
            }
        }
        public ActionResult PrintContract(int id)
        {
           
            ContractViewModel model = _contract.Read(new ContractBindingModel
            {
                Id = id
            }).FirstOrDefault();
            _report.SaveContractReaderToWordFile(exportDirectory + "\\" + "договор" + id + ".docx", model);
            var fileName = Path.GetFileName(exportDirectory + "\\" + "договор" + id + ".docx");
            return File("Export1/" + fileName, "text/json", fileName);
        }
    }
}
