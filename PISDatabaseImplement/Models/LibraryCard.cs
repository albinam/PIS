
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PISDatabaseimplements.Models
{
    public class LibraryCard
    {
        public int Id { get; set; }
        public string Year { get; set; }
        public string PlaceOfWork { get; set; }
        public DateTime DateOfBirth { get; set; }
        public User Reader { get; set; }
    }
}