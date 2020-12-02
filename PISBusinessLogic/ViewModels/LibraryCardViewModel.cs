
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PISBusinessLogic.ViewModels
{
    public class LibraryCardViewModel
    {
        public int Id { get; set; }
        public string Year { get; set; }
        public string PlaceOfWork { get; set; }
        public int? UserId { get; set; }
        public string ReaderFIO { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }
    }
}