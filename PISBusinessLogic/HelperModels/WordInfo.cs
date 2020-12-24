using PISBusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PISBusinessLogic.HelperModels
{
    public class WordInfo
    {
        public string FileName { get; set; }
        public string Title { get; set; }
        public LibraryCardViewModel libraryCard { get; set; }
        public BookViewModel book { get; set; }
        public List<UserViewModel> UserFIO { get; set; }
        public UserViewModel user { get; set; }
        public ContractViewModel contract { get; set; }
    }
}
