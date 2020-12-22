
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace PISBusinessLogic.BindingModels
{
    [DataContract]
    public class LibraryCardBindingModel
    {
        [DataMember]
        public int? Id { get; set; }
        [DataMember]
        public int? UserId { get; set; }
        [DataMember]
        public string Year { get; set; }
        [DataMember]
        public string PlaceOfWork { get; set; }
        [DataMember]
        public DateTime DateOfBirth { get; set; }
    }
}