using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PIS.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public Book Book { get; set; }
        public LibraryCard LibraryCard { get; set; }
    }
}