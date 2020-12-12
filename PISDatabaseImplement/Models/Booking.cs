using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace PISDatabaseimplements.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int BookId { get; set; }
        public int LibraryCardId { get; set; }
        public virtual Book Book { get; set; }
        public virtual LibraryCard LibraryCard { get; set; }
    }
}