using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PISBusinessLogic.ViewModels
{
    public class BookContractViewModel
    {
        public int Id { get; set; }
        public DateTime DateReturn { get; set; }
        public double Price { get; set; }
        public double Fine { get; set; }
    }
}