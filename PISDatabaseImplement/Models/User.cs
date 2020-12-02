using PISBusinessLogic.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace PISDatabaseimplements.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FIO { get; set; }
        public string Salary { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Comission { get; set; }
        public string ComissionPercent { get; set; }
        public Roles Role { get; set; }
    }
}
