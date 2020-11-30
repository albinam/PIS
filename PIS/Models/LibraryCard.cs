using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PIS.Models
{
    public class LibraryCard
    {
        public int Id { get; set; }
        public string Year { get; set; }
        public string PlaceOfWork { get; set; }
        public DateTime DateOfBirth { get; set; }
        public ApplicationUser Reader { get; set; }
    }
}