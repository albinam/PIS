﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace PISBusinessLogic.BindingModels
{
    [DataContract]
    public class ContractBindingModel
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public DateTime Date { get;set;}
        [DataMember]
        public double Sum { get; set; }
      
    }
}