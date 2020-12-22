using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace PISBusinessLogic.BindingModels
{
    public class GenreBindingModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }

    }
}