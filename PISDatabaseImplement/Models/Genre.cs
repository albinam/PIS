using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace PISDatabaseimplements.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }

    }
}