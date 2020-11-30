using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PIS.Models
{
    public class Status
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string BookStatus { get; set; }
        public Book Book { get; set; }
    }
}