using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PIS.Models
{
    public class Contract
    {
        public int Id { get; set; }
        public DateTime Date { get;set;}
        public double Sum { get; set; }
        [ForeignKey("ContractId")]
        public virtual List<BookContract> BookContracts { get; set; }
        public LibraryCard LibraryCard { get; set; }
        public ApplicationUser Librarian { get; set; }
    }
}