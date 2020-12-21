using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace PISBusinessLogic.BindingModels
{
    [DataContract]
    public class PaymentBindingModel
    {
        [DataMember]
        public int? Id { get; set; }
        [DataMember]
        public double Sum { get; set; }
        [DataMember]
        public DateTime Date {get;set;}
        [DataMember]
        public int UserId { get; set; }
    }
}