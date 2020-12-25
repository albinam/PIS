using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace PISCourseworkARMLibrarian.Models
{
    public class Chart
    {
        public string GenreName { get; set; }
        [DisplayName("Количество")]
        public int Count { get; set; }
    }
}
