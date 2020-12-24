using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PISBusinessLogic.ViewModels
{
    public class PaymentViewModel
    {
        public int Id { get; set; }
        public double Sum { get; set; }
        public DateTime Date {get;set;}
        public int UserId { get; set; }
        public string LibrarianFIO { get; set; }
    }
}