using PISBusinessLogic.Enums;
using PISBusinessLogic.HelperModels;
using PISBusinessLogic.Interfaces;
using PISBusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PISBusinessLogic.BusinessLogic
{
    public class ReportLogic
    {
        private readonly IUserLogic _user;
        public ReportLogic(IUserLogic user)
        {
            this._user = user;
        }
        public void SaveLibraryCardToWordFile(string fileName, LibraryCardViewModel model)
        {
            string title = "Читательский билет №" + model.Id;
            SaveToWord.CreateDoc(new WordInfo
            {
                FileName = fileName,
                Title = title,
                libraryCard = model,
                book = null

            });
        }
        public void SaveBookToWordFile(string fileName, BookViewModel model)
        {
            string title = "Справка о наличии книги № " + model.Id;
            SaveToWord.CreateDoc(new WordInfo
            {
                FileName = fileName,
                Title = title,
                book = model,
                libraryCard = null

            });
        }
        public void SaveContractReaderToWordFile(string fileName,ContractViewModel model)
        {
            string title = "Договор №  " + model.Id;
            SaveToWord.CreateDoc(new WordInfo
            {
                FileName = fileName,
                Title = title,
                contract = model
            });
        }
        public void SaveContractToWordFile(string fileName, UserViewModel model)
        {
            string title = "Контракт с  " + model.FIO;
            SaveToWord.CreateDoc(new WordInfo
            {
                FileName = fileName,
                Title = title,
                user =  model

            });
        }
        public List<UserViewModel> GetUsers()
        {
            var list = new List<UserViewModel>();
            var users = _user.Read(null);
            foreach (var us in users)
            {
                if (us.Role == Roles.Библиотекарь)
                {
                    list.Add(us);
                }
            }
            return list;
        }
        public void SaveListToWordFile(string fileName)
        {
            string title = "Список библиотекарей ";
            SaveToWord.CreateDoc(new WordInfo
            {
                FileName = fileName,
                Title = title,
                UserFIO = GetUsers()
            });
        }
    }
}
