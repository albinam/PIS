using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PIS.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public double Sum { get; set; }
        public DateTime Date {get;set;}

    }
}