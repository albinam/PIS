using PISBusinessLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PISDatabaseimplements.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string PublishingHouse { get; set; }
        public string Year { get; set; }
        public Status Status { get; set; }
        public int GenreId { get; set; }
        public virtual Genre Genre { get; set; }

    }
}