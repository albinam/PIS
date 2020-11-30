using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace PISDatabaseimplements.Models
{
    public class BookContract
    {
        public int Id { get; set; }
        public DateTime DateReturn { get; set; }
        public double Price { get; set; }
        public double Fine { get; set; }
        public Book Book { get; set; }
        public Contract Contract { get; set; }
    }
}