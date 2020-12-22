using PISBusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PISBusinessLogic.HelperModels
{
    public class WordInfoContract
    {
        public string FileName { get; set; }
        public string Title { get; set; }
        public UserViewModel user { get; set; }
    }
}
