using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace PISDatabaseimplements.Models
{
    public class ContractBook
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int ContractId { get; set; }
        public virtual Book Book { get; set; }
        public virtual Contract Contract { get; set; }
    }
}