using Microsoft.AspNetCore.Mvc;
using PISBusinessLogic;
using PISBusinessLogic.BindingModels;
using PISBusinessLogic.Enums;
using PISBusinessLogic.Interfaces;
using PISBusinessLogic.ViewModels;
using PISCoursework.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PISCoursework.Controllers
{
    public class ReadersController : Controller
    {
        private readonly IBookLogic _book;
        private readonly IGenreLogic _genre;
        private readonly IUserLogic _user;
        private readonly ILibraryCardLogic _libraryCard;
        private readonly IContractLogic _contract;
        private Validation validation;
        public ReadersController(IBookLogic book, IGenreLogic genre, IUserLogic user, ILibraryCardLogic libraryCard, IContractLogic contract)
        {
            _book = book;
            _genre = genre;
            _user = user;
            _libraryCard = libraryCard;
            _contract = contract;
            validation = new Validation();
        }
        
        public IActionResult Readers()
        {
            ViewBag.Users = _user.Read(null);
            ViewBag.Create = -1;
            return View("Views/Librarian/Readers.cshtml");
        }
        [HttpGet]
        public ActionResult Readers(UserBindingModel model)
        {
            if (model.FIO == null)
            {
                var Users = _user.Read(null);
                List<UserViewModel> users = new List<UserViewModel>();
                foreach (var user in Users)
                {
                    if (user.Role == Roles.Читатель)
                        users.Add(user);
                }
                ViewBag.Users = users;
                ViewBag.Create = -1;
                return View("Views/Librarian/Readers.cshtml");
            }
            var Users2 = _user.Read(new UserBindingModel
            {
                FIO = model.FIO
            });
            List<UserViewModel> users2 = new List<UserViewModel>();
            foreach (var user in Users2)
            {
                if (user.Role == Roles.Читатель)
                    users2.Add(user);
            }
            ViewBag.Create = -1;
            ViewBag.Users = users2;
            return View("Views/Librarian/Readers.cshtml");
        }
        [HttpGet]
        public ActionResult AddLibraryCard(int id, bool prolong)
        {
            if (prolong == false)
            {
                ViewBag.UserId = id;
                ViewBag.User = _user.Read(new UserBindingModel
                {
                    Id = id
                }).FirstOrDefault();
                ViewBag.Exists = _libraryCard.Read(new LibraryCardBindingModel
                {
                    UserId = id
                });
                return View("Views/Librarian/AddLibraryCard.cshtml");
            }
            else
            {
                ViewBag.UserId = id;
                ViewBag.User = _user.Read(new UserBindingModel
                {
                    Id = id
                }).FirstOrDefault();
                var exists = _libraryCard.Read(new LibraryCardBindingModel
                {
                    UserId = id
                });

                foreach (var exist in exists)
                {
                    _libraryCard.CreateOrUpdate(new LibraryCardBindingModel
                    {
                        Id= exist.Id,
                        DateOfBirth = exist.DateOfBirth,
                        Year = DateTime.Now.Year.ToString(),
                        PlaceOfWork = exist.PlaceOfWork,
                        UserId = exist.UserId
                    });
                }
                var reader = _libraryCard.Read(new LibraryCardBindingModel
                {
                    UserId = id
                });
                ViewBag.Exists = reader;
                return View("Views/Librarian/AddLibraryCard.cshtml");
            }
        }
        [HttpPost]
        public ActionResult AddLibraryCard(LibraryCardBindingModel model)
        {
            ViewBag.Exists = _libraryCard.Read(new LibraryCardBindingModel
            {
                UserId = model.UserId
            });
            if (validation.addLibraryCard(model) != "")
            {
                ModelState.AddModelError("", validation.addLibraryCard(model));
                return View("Views/Librarian/AddLibraryCard.cshtml");
            }          
            _libraryCard.CreateOrUpdate(new LibraryCardBindingModel
            {
                DateOfBirth = model.DateOfBirth,
                Year = DateTime.Now.Year.ToString(),
                PlaceOfWork = model.PlaceOfWork,
                UserId = model.UserId
            });

            return RedirectToAction("Readers");
        }
       
        [HttpGet]
        public ActionResult AddContractBooks(string Email, int id, BookBindingModel model)
        {
            ViewBag.Genres = _genre.Read(null);
            ViewBag.FIO = Email;
            ViewBag.Create = -1;
            var freebooks = _book.Read(null);
            List<BookViewModel> freeBooks = new List<BookViewModel>();
            foreach (var book in freebooks)
            {
                if (book.Status == Status.Свободна)
                    freeBooks.Add(book);
            }
            ViewBag.Books = freeBooks;
            if ((model.Author != null || model.GenreId != 0 || model.Name != null) && (id == 0 || id == -1))
            {
                return BookSearch(model);
            }
            if ((model.Author != null || model.GenreId != 0 || model.Name != null) && (id != 0 && id != -1))
            {
                var contract = _contract.Read(null).LastOrDefault();
                var listOfBooks = _contract.Read(new ContractBindingModel
                {
                    Id = contract.Id
                }).FirstOrDefault();
                if (listOfBooks.ContractBooks.Count != 0)
                {
                    ViewBag.ContractBooks = listOfBooks.ContractBooks;
                }
                return BookSearch(model);
            }
            int libraryCard = 0;
            if (Email != null)
            {
                var user = _user.Read(new UserBindingModel
                {
                    Email = Email
                }).FirstOrDefault();
                var libraryCard1 = _libraryCard.Read(new LibraryCardBindingModel
                {
                    UserId = user.Id
                }).FirstOrDefault();
                libraryCard = libraryCard1.Id;
            }

            var contractBooks = new List<ContractBookBindingModel>();
            if (id != 0 && id != -1)
            {
                var book = _book.Read(new BookBindingModel
                {
                    Id = id
                }).FirstOrDefault();
                _book.CreateOrUpdate(new BookBindingModel
                {
                    Id = id,
                    GenreId = book.GenreId,
                    Name = book.Name,
                    Author = book.Author,
                    PublishingHouse = book.PublishingHouse,
                    Year = book.Year,
                    Status = Status.Выдана
                });
                contractBooks.Add(new ContractBookBindingModel
                {
                    BookId = id,
                });
                var contract = _contract.Read(null).LastOrDefault();
                _contract.CreateOrUpdate(new ContractBindingModel
                {
                    Id = contract.Id,
                    Date = contract.Date,
                    DateReturn = contract.DateReturn,
                    Sum = contract.Sum + getSum(contractBooks),
                    Fine = 0,
                    LibraryCardId = contract.LibraryCardId,
                    LibrarianId = Program.Librarian.Id,
                    ContractBooks = contractBooks
                });
                var listOfBooks = _contract.Read(new ContractBindingModel
                {
                    Id = contract.Id
                }).FirstOrDefault();
                if (listOfBooks.ContractBooks.Count != 0)
                {
                    ViewBag.ContractBooks = listOfBooks;
                }
                ViewBag.Genres = _genre.Read(null);
                ViewBag.FIO = Email;
                ViewBag.Create = -1;
                freebooks = _book.Read(null);
                freeBooks = new List<BookViewModel>();
                foreach (var book1 in freebooks)
                {
                    if (book1.Status == Status.Свободна)
                        freeBooks.Add(book1);
                }
                ViewBag.Books = freeBooks;

                return View("Views/Librarian/AddContractBooks.cshtml");
            }
            if (id == -1)
            {
                _contract.CreateOrUpdate(new ContractBindingModel
                {
                    Date = DateTime.Now,
                    DateReturn = DateTime.Now.AddDays(14),
                    Sum = 0,
                    Fine = 0,
                    LibraryCardId = libraryCard,
                    LibrarianId = Program.Librarian.Id,
                    ContractBooks = contractBooks
                });
            }
            return View("Views/Librarian/AddContractBooks.cshtml");
        }
        [HttpGet]
        public ActionResult ListOfContracts(int id, string FIO)
        {
            ViewBag.Genres = _genre.Read(null);
            var Contracts = _contract.Read(null);
            var Users = _user.Read(null);
            double fine = 1;

            foreach (var contract in Contracts)
            {
                int date = (DateTime.Now.Date - contract.DateReturn.Date).Days;
                List<ContractBookBindingModel> list = new List<ContractBookBindingModel>();
                list = ConvertModels(contract.ContractBooks);
                if (date > 0)
                {
                    _contract.CreateOrUpdate(new ContractBindingModel
                    {
                        Id = contract.Id,
                        Date = contract.Date,
                        DateReturn = contract.DateReturn,
                        LibrarianId = contract.LibrarianId,
                        LibraryCardId = contract.LibraryCardId,
                        Sum = contract.Sum,
                        ContractBooks = list,
                        Fine = fine * date
                    });
                }
            }
            var contracts = _contract.Read(null);
            ViewBag.Contracts = contracts;
            List<UserViewModel> users = new List<UserViewModel>();
            List<LibraryCardViewModel> cards = new List<LibraryCardViewModel>();
            foreach (var user in Users)
            {
                if (user.Role == Roles.Читатель)
                    users.Add(user);
                var card = _libraryCard.Read(new LibraryCardBindingModel
                {
                    UserId = user.Id
                }).FirstOrDefault();
                if (card != null)
                    cards.Add(card);
            }
            ViewBag.LibraryCards = cards;
            ViewBag.Users = users;
            if (id == 0 && FIO == null)
            {
                return View("Views/Librarian/ListOfContracts.cshtml");
            }
            else
            {

                if (id != 0 && FIO == null)
                {
                    ViewBag.Contracts = _contract.Read(new ContractBindingModel
                    {
                        Id = id
                    });
                    return View("Views/Librarian/ListOfContracts.cshtml");
                }
                if (id == 0 && FIO != null)
                {
                    var readers = _user.Read(new UserBindingModel
                    {
                        FIO = FIO
                    });
                    foreach (var reader in readers)
                    {
                        var card = _libraryCard.Read(new LibraryCardBindingModel
                        {
                            UserId = reader.Id
                        }).FirstOrDefault();
                        ViewBag.Contracts = _contract.Read(new ContractBindingModel
                        {
                            LibraryCardId = card.Id
                        });
                    }
                    return View("Views/Librarian/ListOfContracts.cshtml");
                }
                ModelState.AddModelError("", "Введите хотя бы один параметр поиска");
                return View("Views/Librarian/ListOfContracts.cshtml");
            }
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
        public double getSum(List<ContractBookBindingModel> contractBooks)
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
                    sum += g.Price;
                }
                return sum;
            }
            else
            {
                return 0;
            }
        }
       
    
        [HttpGet]
        public ActionResult ReadersWithOverdue(DateTime date)
        {
            var dat1 = new DateTime();
            List<ContractViewModel> contracts = new List<ContractViewModel>();
            var Contracts = _contract.Read(null);
            foreach (var contract in Contracts)
            {
                if (contract.DateReturn < date)
                {
                    contracts.Add(contract);
                }
            }
            List<LibraryCardViewModel> libraryCards = new List<LibraryCardViewModel>();
            foreach (var contract in contracts)
            {
                libraryCards = _libraryCard.Read(new LibraryCardBindingModel
                {
                    Id = contract.LibraryCardId
                });
            }
            List<UserViewModel> readers = new List<UserViewModel>();
            foreach (var reader in libraryCards)
            {
                readers = _user.Read(new UserBindingModel
                {
                    Id = reader.UserId
                });
            }
            ViewBag.Contracts = contracts;
            ViewBag.Readers = readers;
            if (validation.readersWithOverdue(date))
            {
                ModelState.AddModelError("", "Выберите дату");
                return View("Views/Librarian/ReadersWithOverdue.cshtml");
            }
            return View("Views/Librarian/ReadersWithOverdue.cshtml");
        }
        public ActionResult BookSearch(BookBindingModel model)
        {
            ViewBag.Genres = _genre.Read(null);
            var freebooks = _book.Read(null);
            List<BookViewModel> freeBooks = new List<BookViewModel>();
            foreach (var book in freebooks)
            {
                if (book.Status == Status.Свободна)
                    freeBooks.Add(book);
            }
            ViewBag.Books = freeBooks;
            //по названию
            if (model.Name != null && model.GenreId == 0 && model.Author == null)
            {
                ViewBag.Books = _book.Read(new BookBindingModel
                {
                    Name = model.Name,
                    Status = Status.Свободна
                });
                return View("Views/Librarian/ListOfBooks.cshtml");
            }
            //по жанру
            if (model.GenreId != 0 && model.Name == null && model.Author == null)
            {
                ViewBag.Books = _book.Read(new BookBindingModel
                {
                    GenreId = model.GenreId,
                    Status = Status.Свободна
                });
                return View("Views/Librarian/ListOfBooks.cshtml");
            }
            //по автору
            if (model.GenreId == 0 && model.Name == null && model.Author != null)
            {
                ViewBag.Books = _book.Read(new BookBindingModel
                {
                    Author = model.Author,
                    Status = Status.Свободна
                });
                return View("Views/Librarian/ListOfBooks.cshtml");
            }
            //по жанру и названию
            if (model.GenreId != 0 && model.Name != null && model.Author == null)
            {
                var books = _book.Read(new BookBindingModel
                {
                    Name = model.Name,
                    Status = Status.Свободна
                });
                List<BookViewModel> Books = new List<BookViewModel>();
                foreach (var book in books)
                {
                    if (book.GenreId == model.GenreId)
                        Books.Add(book);
                }
                ViewBag.Books = Books;
                return View("Views/Librarian/ListOfBooks.cshtml");
            }
            // по всем трем
            if (model.GenreId != 0 && model.Name != null && model.Author != null)
            {
                var books = _book.Read(new BookBindingModel
                {
                    Name = model.Name,
                    Status = Status.Свободна
                });
                List<BookViewModel> Books = new List<BookViewModel>();
                foreach (var book in books)
                {
                    if (book.GenreId == model.GenreId && book.Author == model.Author)
                        Books.Add(book);
                }
                ViewBag.Books = Books;
                return View("Views/Librarian/ListOfBooks.cshtml");
            }
            //по жанру и автору
            if (model.GenreId != 0 && model.Name == null && model.Author != null)
            {
                var books = _book.Read(new BookBindingModel
                {
                    Author = model.Author,
                    Status = Status.Свободна
                });
                List<BookViewModel> Books = new List<BookViewModel>();
                foreach (var book in books)
                {
                    if (book.GenreId == model.GenreId)
                        Books.Add(book);
                }
                ViewBag.Books = Books;
                return View("Views/Librarian/ListOfBooks.cshtml");
            }
            //по автору и названию
            if (model.GenreId == 0 && model.Name != null && model.Author != null)
            {
                var books = _book.Read(new BookBindingModel
                {
                    Name = model.Name,
                    Status = Status.Свободна
                });
                List<BookViewModel> Books = new List<BookViewModel>();
                foreach (var book in books)
                {
                    if (book.Author == model.Author)
                        Books.Add(book);
                }
                ViewBag.Books = Books;
                return View("Views/Librarian/ListOfBooks.cshtml");
            }
            if (validation.bookSearch(model))
            {
                ModelState.AddModelError("", "Выберите хотя бы один параметр поиска");
                return View("Views/Librarian/ListOfBooks.cshtml");
            }
            return View("Views/Librarian/ListOfBooks.cshtml");
        }
    }
}
