using PISBusinessLogic.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PISBusinessLogic.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string FIO { get; set; }
        public string Salary { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Email { get; set; }
        public string Comission { get; set; }
        public string ComissionPercent { get; set; }
        public Roles Role { get; set; }
    }
}
