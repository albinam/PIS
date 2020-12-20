using PISBusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PISBusinessLogic.HelperModels
{
    public class WordInfoList
    {
        public string FileName { get; set; }
        public string Title { get; set; }
        public List<UserViewModel> UserFIO { get; set; }
    }
}
