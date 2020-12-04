using Microsoft.AspNetCore.Mvc;
using PISBusinessLogic;
using PISBusinessLogic.BindingModels;
using PISBusinessLogic.Enums;
using PISBusinessLogic.Interfaces;
using PISBusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PISCoursework.Controllers
{
    public class LibrarianController : Controller
    {
        private readonly IBookLogic _book;
        private readonly IGenreLogic _genre;
        private readonly IUserLogic _user;
        private readonly ILibraryCardLogic _libraryCard;
        private readonly IContractLogic _contract;
        public LibrarianController(IBookLogic book, IGenreLogic genre, IUserLogic user, ILibraryCardLogic libraryCard, IContractLogic contract)
        {
            _book = book;
            _genre = genre;
            _user = user;
            _libraryCard = libraryCard;
            _contract = contract;
        }
        public IActionResult AddBook()
        {
            ViewBag.Genres = _genre.Read(null);
            return View();
        }
        [HttpPost]
        public ActionResult AddBook(BookBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Genres = _genre.Read(null);
                return View(model);
            }
            if (model.Name == null)
            {
                ViewBag.Genres = _genre.Read(null);
                ModelState.AddModelError("", "Введите название");
                return View(model);
            }
            if (model.Author == null)
            {
                ViewBag.Genres = _genre.Read(null);
                ModelState.AddModelError("", "Введите автора");
                return View(model);
            }
            if (model.PublishingHouse == null)
            {
                ViewBag.Genres = _genre.Read(null);
                ModelState.AddModelError("", "Введите издательство");
                return View(model);
            }
            if (model.Year == null)
            {
                ViewBag.Genres = _genre.Read(null);
                ModelState.AddModelError("", "Введите год издания");
                return View(model);
            }
            if (model.Name != null && model.PublishingHouse != null && model.Year != null && model.Author != null)
            {
                ModelState.AddModelError("", "Книга успешно добавлена");
            }
            _book.CreateOrUpdate(new BookBindingModel
            {
                Name = model.Name,
                Author = model.Author,
                PublishingHouse = model.PublishingHouse,
                Year = model.Year,
                GenreId = model.GenreId,
                Status = Status.Свободна
            });

            return RedirectToAction("ListOfBooks");
        }
        public IActionResult Readers()
        {
            ViewBag.Users = _user.Read(null);
            ViewBag.Create = -1;
            return View();
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
                return View();
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
            return View();
        }
        [HttpGet]
        public ActionResult AddLibraryCard(int id)
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
            return View();
        }
        [HttpPost]
        public ActionResult AddLibraryCard(LibraryCardBindingModel model)
        {
            if (model.DateOfBirth == null)
            {
                ModelState.AddModelError("", "Введите дату рождения");
                return View(model);
            }
            if (model.Year == null)
            {
                ModelState.AddModelError("", "Введите год");
                return View(model);
            }
            if (model.PlaceOfWork == null)
            {
                ModelState.AddModelError("", "Введите место работы");
                return View(model);
            }
            _libraryCard.CreateOrUpdate(new LibraryCardBindingModel
            {
                DateOfBirth = model.DateOfBirth,
                Year = model.Year,
                PlaceOfWork = model.PlaceOfWork,
                UserId = model.UserId
            });
            return RedirectToAction("Readers");
        }
        [HttpGet]
        public ActionResult ListOfBooks(BookBindingModel model)
        {
            ViewBag.Genres = _genre.Read(null);
            ViewBag.Books = _book.Read(new BookBindingModel
            {
                Status = Status.Свободна
            });
            return BookSearch(model);

        }
       
        [HttpGet]
        public ActionResult AddContractBooks(string Email, int id,  BookBindingModel model)
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
            int libraryCard=0;
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
                    Status = Status.Занята
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
                    Sum = contract.Sum+getSum(contractBooks),
                    Fine = 0,
                    LibraryCardId = contract.LibrarianId,
                    LibrarianId = Program.Librarian.Id,
                    ContractBooks = contractBooks
                }) ;
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
               
                return View();
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
            return View(model);
        }
        [HttpGet]
        public ActionResult ListOfContracts(int id,string FIO)
        {
            ViewBag.Genres = _genre.Read(null);
            ViewBag.Contracts = _contract.Read(null);
            var Users = _user.Read(null);
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
                if(card!=null)
                    cards.Add(card);
            }
            ViewBag.LibraryCards = cards;
            ViewBag.Users = users;
            if (id == 0 && FIO == null )
            {
                return View();
            }
            else
            {
                
                if (id != 0 && FIO == null)
                {
                    ViewBag.Contracts = _contract.Read(new ContractBindingModel
                    {
                        Id = id
                    });
                    return View(id);
                }
                if (id == 0 && FIO != null)
                {
                    var readers = _user.Read(new UserBindingModel
                    {
                        FIO = FIO
                    });
                    foreach(var reader in readers)
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
                    return View();
                }
                ModelState.AddModelError("", "Введите хотя бы один параметр поиска");
                return View();
            }
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
                return View(model);
            }
            //по жанру
            if (model.GenreId != 0 && model.Name == null && model.Author == null)
            {
                ViewBag.Books = _book.Read(new BookBindingModel
                {
                    GenreId = model.GenreId,
                    Status = Status.Свободна
                });
                return View(model);
            }
            //по автору
            if (model.GenreId == 0 && model.Name == null && model.Author != null)
            {
                ViewBag.Books = _book.Read(new BookBindingModel
                {
                    Author = model.Author,
                    Status = Status.Свободна
                });
                return View(model);
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
                return View(model);
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
                return View(model);
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
                return View(model);
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
                return View(model);
            }
            if (model.GenreId == 0 && model.Name == null && model.Author == null)
            {
                ModelState.AddModelError("", "Выберите хотя бы один параметр поиска");
                return View();
            }
            return View(model);
        }
    }
}
