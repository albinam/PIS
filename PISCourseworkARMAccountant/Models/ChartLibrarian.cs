using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace PISCourseworkARMAccountant.Models
{
    public class ChartLibrarian
    {
        public string UserFIO { get; set; }
        [DisplayName("Библиотекарь")]
        public int Sum { get; set; }
    }
}
