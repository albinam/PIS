using PISBusinessLogic.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace PISBusinessLogic.BindingModels
{
    [DataContract]
    public class UserBindingModel
    {
        [DataMember]
        public int? Id { get; set; }
        [DataMember]
        public string FIO { get; set; }
        [DataMember]
        public string Salary { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Comission { get; set; }
        [DataMember]
        public string ComissionPercent { get; set; }
        [DataMember]
        public Roles Role { get; set; }
    }
}
