using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace PISBusinessLogic.BindingModels
{
    [DataContract]
    public class BookContractBindingModel
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public DateTime DateReturn { get; set; }
        [DataMember]
        public double Price { get; set; }
        [DataMember]
        public double Fine { get; set; }
    }
}