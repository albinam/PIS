using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PISBusinessLogic.BindingModels
{
    public class BookBindingModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string PublishingHouse { get; set; }
        public string Year { get; set; }
        public int GenreId { get; set; }
    }
}