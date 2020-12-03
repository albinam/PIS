using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace PISBusinessLogic.BindingModels
{
    [DataContract]
    public class ContractBookBindingModel
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int BookId { get; set; }
        [DataMember]
        public int ContractId { get; set; }
        [DataMember]
        public double Fine { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string PublishingHouse { get; set; }
        public string Year { get; set; }
        public Status Status { get; set; }
        public int GenreId { get; set; }
    }
}