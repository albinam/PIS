
using PISBusinessLogic.BindingModels;
using PISBusinessLogic.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PISBusinessLogic.ViewModels
{
    public class ContractViewModel
    {
        public int? Id { get; set; }
        public int LibraryCardId { get; set; }
        public int LibrarianId { get; set; }
        public DateTime Date { get;set;}
        public DateTime DateReturn { get; set; }
        public double Fine { get; set; }
        public double Sum { get; set; }
        public ContractStatus ContractStatus { get; set; }
        public string ReaderFIO { get; set; }
        public string LibrarianFIO { get; set; }
        public List<ContractBookViewModel> ContractBooks { get; set; }
    }
}