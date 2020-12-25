using Microsoft.AspNetCore.Mvc;
using PISBusinessLogic;
using PISBusinessLogic.BindingModels;
using PISBusinessLogic.BusinessLogic;
using PISBusinessLogic.Enums;
using PISBusinessLogic.HelperModels;
using PISBusinessLogic.Interfaces;
using PISBusinessLogic.ViewModels;
using PISCourseworkARMLibrarian.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PISCourseworkARMLibrarian.Controllers
{
    public class ReadersController : Controller
    {
        private readonly IBookLogic _book;
        private readonly IGenreLogic _genre;
        private readonly IUserLogic _user;
        private readonly ILibraryCardLogic _libraryCard;
        private readonly IContractLogic _contract;
        private readonly IBookingLogic _booking;
        private Validation validation;
        private readonly ReportLogic _report;
        public ReadersController(IBookLogic book, IGenreLogic genre, IUserLogic user, ILibraryCardLogic libraryCard, IContractLogic contract, ReportLogic report, IBookingLogic booking)
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
        public ActionResult Readers(UserBindingModel model)
        {
            if (model.FIO == null)
            {
                var Users = _user.Read(null);
                List<UserViewModel> users = new List<UserViewModel>();
                List<LibraryCardViewModel> list = new List<LibraryCardViewModel>();
                foreach (var user in Users)
                {
                    if (user.Role == Roles.Читатель)
                        users.Add(user);
                    var card = _libraryCard.Read(new LibraryCardBindingModel
                    {
                        UserId = user.Id
                    }).FirstOrDefault();
                    if (card != null)
                    {
                        list.Add(card);
                    }

                }
                List<UserViewModel> userWithCard = new List<UserViewModel>();
                List<UserViewModel> userWithCardOverdue = new List<UserViewModel>();
                foreach (var c in list)
                {
                    foreach (var u in users)
                    {
                        if (u.Id == c.UserId && c.Year == DateTime.Now.Year.ToString())
                        {
                            userWithCard.Add(u);
                        }
                        if (u.Id == c.UserId && c.Year != DateTime.Now.Year.ToString())
                        {
                            userWithCardOverdue.Add(u);
                        }
                    }
                }
                foreach (var uc in userWithCard)
                {
                    users.Remove(uc);
                }
                foreach (var uc in userWithCardOverdue)
                {
                    users.Remove(uc);
                }
                ViewBag.Users = userWithCard;
                ViewBag.UsersWithoutCard = users;
                ViewBag.UsersWithCardOverdue = userWithCardOverdue;
                ViewBag.Create = -1;
                return View("Views/Librarian/Readers.cshtml");
            }
            else
            {
                var Users2 = _user.Read(new UserBindingModel
                {
                    FIO = model.FIO
                });
                List<UserViewModel> users2 = new List<UserViewModel>();
                List<LibraryCardViewModel> list2 = new List<LibraryCardViewModel>();
                List<UserViewModel> userWithCard2 = new List<UserViewModel>();
                List<UserViewModel> userWithCardOverdue2 = new List<UserViewModel>();
                foreach (var user in Users2)
                {
                    if (user.Role == Roles.Читатель)
                        users2.Add(user);
                    var card = _libraryCard.Read(new LibraryCardBindingModel
                    {
                        UserId = user.Id
                    }).FirstOrDefault();
                    list2.Add(card);
                }
                foreach (var c in list2)
                {
                    foreach (var u in users2)
                    {
                        if (u.Id == c.UserId && c.Year == DateTime.Now.Year.ToString())
                        {
                            userWithCard2.Add(u);
                        }
                        if (u.Id == c.UserId && c.Year != DateTime.Now.Year.ToString())
                        {
                            userWithCardOverdue2.Add(u);
                        }
                    }
                }
                foreach (var uc in userWithCard2)
                {
                    users2.Remove(uc);
                }
                foreach (var uc in userWithCardOverdue2)
                {
                    users2.Remove(uc);
                }
                ViewBag.Users = userWithCard2;
                ViewBag.UsersWithoutCard = users2;
                ViewBag.UsersWithCardOverdue = userWithCardOverdue2;
                ViewBag.Create = -1;
                return View("Views/Librarian/Readers.cshtml");
            }
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
                        Id = exist.Id,
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

            return RedirectToAction("Readers", "Readers");
        }

        [HttpGet]
        public ActionResult AddContractBooks(string Email, int id, BookBindingModel model, DateTime date)
        {
            int libraryCard = 0;
            ViewBag.Email = Email;
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
            ViewBag.Genres = _genre.Read(null);
            ViewBag.FIO = Email;
            ViewBag.Create = -1;
            var freebooks = _book.Read(null);
            var bookings = _booking.Read(new BookingBindingModel
            {
                LibraryCardId = libraryCard
            });
            List<BookViewModel> freeBooks = new List<BookViewModel>();
            foreach (var booking in bookings)
            {
                var b = _book.Read(new BookBindingModel
                {
                    Id = booking.BookId
                }).LastOrDefault();
                if (booking.DateTo > DateTime.Now)
                {
                    if (b.Status == Status.Забронирована)
                    {
                        freeBooks.Add(b);
                    }
                }
                else if (booking.DateTo < DateTime.Now)
                {
                    _book.CreateOrUpdate(new BookBindingModel
                    {
                        Id = b.Id,
                        Name = b.Name,
                        PublishingHouse = b.PublishingHouse,
                        Author = b.Author,
                        Year = b.Year,
                        GenreId = b.GenreId,
                        Status = Status.Свободна
                    });
                }
            }
            foreach (var book in freebooks)
            {
                if (book.Status == Status.Свободна)
                    freeBooks.Add(book);
            }
            ViewBag.Books = freeBooks;
            if ((model.Author != null || model.GenreId != 0 || model.Name != null) && (id == 0 || id == -1))
            {
                return BookSearch(model, libraryCard);
            }
            if ((model.Author != null || model.GenreId != 0 || model.Name != null) && (id != 0 && id != -1))
            {
                var contract = _contract.Read(null).OrderBy(x => x.Id).Last();
                var listOfBooks = _contract.Read(new ContractBindingModel
                {
                    Id = contract.Id
                }).FirstOrDefault();
                if (listOfBooks.ContractBooks.Count != 0)
                {
                    ViewBag.ContractBooks = listOfBooks;
                }
                return BookSearch(model, libraryCard);
            }
            var contractBooks = new List<ContractBookBindingModel>();
            if (id != 0 && id != -1)
            {
                var book = _book.Read(new BookBindingModel
                {
                    Id = id
                }).FirstOrDefault();
                var booking2 = _booking.Read(new BookingBindingModel
                {
                    LibraryCardId = libraryCard
                });
                List<BookingViewModel> list = new List<BookingViewModel>();
                foreach (var b in booking2)
                {
                    if (b.BookId == id)
                    {
                        if (b.DateTo > DateTime.Now)
                        {
                            list.Add(b);
                        }
                    }
                }
                if (booking2 != null && list.Count != 0)
                {
                    _booking.Delete(new BookingBindingModel
                    {
                        Id = list.First().Id,
                    });
                }
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
                var contract = _contract.Read(null).OrderBy(x => x.Id).Last();

                contractBooks.Add(new ContractBookBindingModel
                {
                    BookId = id,
                });
                if (contract.ContractBooks.Count != 0)
                {
                    foreach (var cb in ConvertModels(contract.ContractBooks))
                    {
                        if (cb.BookId != id)
                        {
                            contractBooks.Add(cb);
                        }
                    }
                }

                TimeSpan period = contract.DateReturn.AddMinutes(10) - DateTime.Now;
                _contract.CreateOrUpdate(new ContractBindingModel
                {
                    Id = contract.Id,
                    Date = contract.Date,
                    DateReturn = contract.DateReturn,
                    Sum = getSum(contractBooks, period.Days),
                    Fine = 0,
                    LibraryCardId = contract.LibraryCardId,
                    ContractStatus = ContractStatus.Активен,
                    LibrarianId = Program.Librarian.Id,
                    ContractBooks = contractBooks
                });
                if (date != new DateTime())
                {
                    if (validation.periodCheck(date) != "")
                    {
                        ModelState.AddModelError("", (validation.periodCheck(date)));
                        return View("Views/Librarian/AddContractBooks.cshtml");
                    }
                    period = date.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute + 10) - DateTime.Now;
                    _contract.CreateOrUpdate(new ContractBindingModel
                    {
                        Id = contract.Id,
                        Date = contract.Date,
                        DateReturn = date,
                        Sum = getSum(contractBooks, period.Days),
                        Fine = 0,
                        LibraryCardId = contract.LibraryCardId,
                        ContractStatus = ContractStatus.Активен,
                        LibrarianId = Program.Librarian.Id,
                        ContractBooks = contractBooks
                    });
                    var listOfBooks1 = _contract.Read(new ContractBindingModel
                    {
                        Id = contract.Id
                    }).FirstOrDefault();
                    if (listOfBooks1.ContractBooks.Count != 0)
                    {
                        ViewBag.ContractBooks = listOfBooks1;
                    }
                    return View("Views/Librarian/AddContractBooks.cshtml");
                }
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
                freeBooks.Clear();
                freebooks = _book.Read(null);
                bookings = _booking.Read(new BookingBindingModel
                {
                    LibraryCardId = libraryCard
                });
                foreach (var book1 in freebooks)
                {
                    if (book1.Status == Status.Свободна)
                        freeBooks.Add(book1);
                }
                foreach (var booking in bookings)
                {
                    var b = _book.Read(new BookBindingModel
                    {
                        Id = booking.BookId
                    }).FirstOrDefault();
                    if (b.Status == Status.Забронирована)
                    {
                        freeBooks.Add(b);
                    }
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
                    ContractStatus = ContractStatus.Активен,
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
            double fine = 5;

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
                        Fine = ((fine * date) / 100) * contract.Sum
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
                if (id != 0 && FIO != null)
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
                        var c = _contract.Read(new ContractBindingModel
                        {
                            LibraryCardId = card.Id
                        });
                        List<ContractViewModel> list = new List<ContractViewModel>();
                        foreach (var contract in c)
                        {
                            if (contract.Id == id)
                            {
                                list.Add(contract);
                            }
                        }
                        ViewBag.Contracts = list;
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
        [HttpGet]
        public ActionResult ReadersWithOverdue(DateTime date)
        {
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
        public ActionResult EndContract(int id)
        {
            if (id != 0)
            {
                var model = _contract.Read(new ContractBindingModel
                {
                    Id = id
                }).FirstOrDefault();

                _contract.CreateOrUpdate(new ContractBindingModel
                {
                    Id = model.Id,
                    Sum = model.Sum,
                    Fine = model.Fine,
                    DateReturn = model.DateReturn,
                    Date = model.Date,
                    ContractStatus = ContractStatus.Завершен,
                    LibrarianId = model.LibrarianId,
                    LibraryCardId = model.LibraryCardId,
                    ContractBooks = ConvertModels(model.ContractBooks)

                });
                foreach (var b in ConvertModels(model.ContractBooks))
                {
                    var book = _book.Read(new BookBindingModel
                    {
                        Id = b.BookId
                    }).FirstOrDefault();
                    var bookings = _booking.Read(new BookingBindingModel
                    {
                        BookId = book.Id
                    });
                    if (bookings.Count == 0)
                    {
                        _book.CreateOrUpdate(new BookBindingModel
                        {
                            Id = book.Id,
                            Name = book.Name,
                            GenreId = book.GenreId,
                            Author = book.Author,
                            Year = book.Year,
                            PublishingHouse = book.PublishingHouse,
                            Status = Status.Свободна
                        });
                    }
                    else
                    {
                        _book.CreateOrUpdate(new BookBindingModel
                        {
                            Id = book.Id,
                            Name = book.Name,
                            GenreId = book.GenreId,
                            Author = book.Author,
                            Year = book.Year,
                            PublishingHouse = book.PublishingHouse,
                            Status = Status.Забронирована
                        });
                    }
                }
            }
            ViewBag.Genres = _genre.Read(null);
            var Users = _user.Read(null);
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
            return View("Views/Librarian/ListOfContracts.cshtml");
        }
        public ActionResult BookSearch(BookBindingModel model, int libraryCard)
        {
            ViewBag.Genres = _genre.Read(null);
            List<BookViewModel> freebooks = getBooks(libraryCard);
            List<BookViewModel> removebooks = new List<BookViewModel>();
            ViewBag.Books = freebooks;
            //по названию
            if (model.Name != null && model.GenreId == 0 && model.Author == null)
            {
                foreach (var el in freebooks)
                {
                    if (el.Name != model.Name)
                    {
                        removebooks.Add(el);
                    }
                }
                foreach (var book in removebooks)
                {
                    freebooks.Remove(book);
                }
                ViewBag.Books = freebooks;
                return View("Views/Librarian/AddContractBooks.cshtml");
            }
            //по жанру
            if (model.GenreId != 0 && model.Name == null && model.Author == null)
            {
                foreach (var el in freebooks)
                {
                    if (el.GenreId != model.GenreId)
                    {
                        removebooks.Add(el);
                    }
                }
                foreach (var book in removebooks)
                {
                    freebooks.Remove(book);
                }
                ViewBag.Books = freebooks;
                return View("Views/Librarian/AddContractBooks.cshtml");
            }
            //по автору
            if (model.GenreId == 0 && model.Name == null && model.Author != null)
            {
                foreach (var el in freebooks)
                {
                    if (el.Author != model.Author)
                    {
                        removebooks.Add(el);
                    }
                }
                foreach (var book in removebooks)
                {
                    freebooks.Remove(book);
                }
                ViewBag.Books = freebooks;
                return View("Views/Librarian/AddContractBooks.cshtml");
            }
            //по жанру и названию
            if (model.GenreId != 0 && model.Name != null && model.Author == null)
            {
                foreach (var el in freebooks)
                {
                    if (el.GenreId != model.GenreId || el.Name != model.Name)
                    {
                        removebooks.Add(el);
                    }
                }
                foreach (var book in removebooks)
                {
                    freebooks.Remove(book);
                }
                ViewBag.Books = freebooks;
                return View("Views/Librarian/AddContractBooks.cshtml");
            }
            // по всем трем
            if (model.GenreId != 0 && model.Name != null && model.Author != null)
            {
                foreach (var el in freebooks)
                {
                    if (el.GenreId != model.GenreId || el.Name != model.Name || el.Author != model.Author)
                    {
                        removebooks.Add(el);
                    }
                }
                foreach (var book in removebooks)
                {
                    freebooks.Remove(book);
                }
                ViewBag.Books = freebooks;
                return View("Views/Librarian/AddContractBooks.cshtml");
            }
            //по жанру и автору
            if (model.GenreId != 0 && model.Name == null && model.Author != null)
            {
                foreach (var el in freebooks)
                {
                    if (el.GenreId != model.GenreId || el.Author != model.Author)
                    {
                        removebooks.Add(el);
                    }
                }
                foreach (var book in removebooks)
                {
                    freebooks.Remove(book);
                }
                ViewBag.Books = freebooks;
                return View("Views/Librarian/AddContractBooks.cshtml");
            }
            //по автору и названию
            if (model.GenreId == 0 && model.Name != null && model.Author != null)
            {
                foreach (var el in freebooks)
                {
                    if (el.Name != model.Name || el.Author != model.Author)
                    {
                        removebooks.Add(el);
                    }
                }
                foreach (var book in removebooks)
                {
                    freebooks.Remove(book);
                }
                ViewBag.Books = freebooks;
                return View("Views/Librarian/AddContractBooks.cshtml");
            }
            return View("Views/Librarian/AddContractBooks.cshtml");
        }

        public List<BookViewModel> getBooks(int libraryCard)
        {
            var freebooks = _book.Read(null);
            var bookings = _booking.Read(new BookingBindingModel
            {
                LibraryCardId = libraryCard
            });
            List<BookViewModel> freeBooks = new List<BookViewModel>();
            foreach (var book in freebooks)
            {
                if (book.Status == Status.Свободна)
                    freeBooks.Add(book);
            }
            foreach (var booking in bookings)
            {
                var b = _book.Read(new BookBindingModel
                {
                    Id = booking.BookId
                }).FirstOrDefault();
                if (booking.DateTo > DateTime.Now)
                {
                    if (b.Status == Status.Забронирована)
                    {
                        freeBooks.Add(b);
                    }
                }
                else
                {
                    _book.CreateOrUpdate(new BookBindingModel
                    {
                        Id = b.Id,
                        Name = b.Name,
                        PublishingHouse = b.PublishingHouse,
                        Author = b.Author,
                        Year = b.Year,
                        GenreId = b.GenreId,
                        Status = Status.Свободна
                    });
                    _booking.Delete(new BookingBindingModel
                    {
                        Id = booking.Id
                    });
                }
            }
            return freeBooks;
        }
    }
}
