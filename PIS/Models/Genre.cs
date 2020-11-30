using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PIS.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        [ForeignKey("GenreId")]
        public virtual List<Book> Books { get; set; }
    }
}