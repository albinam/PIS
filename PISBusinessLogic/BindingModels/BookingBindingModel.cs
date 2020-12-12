using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace PISBusinessLogic.BindingModels
{
    [DataContract]
    public class BookingBindingModel
    {
        [DataMember]
        public int? Id { get; set; }
        [DataMember]
        public DateTime DateFrom { get; set; }
        [DataMember]
        public DateTime DateTo { get; set; }
        public int BookId { get; set; }
        public int LibraryCardId { get; set; }
    }
}