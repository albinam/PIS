using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace PISBusinessLogic.BindingModels
{
    [DataContract]
    public class BookBindingModel
    {
        [DataMember]
        public int? Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Author { get; set; }
        [DataMember]
        public string PublishingHouse { get; set; }
        [DataMember]
        public string Year { get; set; }
        [DataMember]
        public Status Status { get; set; }
        [DataMember]
        public int GenreId { get; set; }
    }
}