using PISBusinessLogic.BindingModels;
using PISBusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PISBusinessLogic.Interfaces
{
    public interface IGenreLogic
    {
        List<GenreViewModel> Read(GenreBindingModel model);
        void CreateOrUpdate(GenreBindingModel model);
        void Delete(GenreBindingModel model);
    }
}
