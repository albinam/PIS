using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PISBusinessLogic.ViewModels
{
    public class BookingViewModel
    {
        public int Id { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int? BookId { get; set; }
        public int LibraryCardId { get; set; }
    }
}