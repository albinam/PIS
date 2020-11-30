using PISBusinessLogic.BindingModels;
using PISBusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PISBusinessLogic.Interfaces
{
    public interface IBookLogic
    {
        List<BookViewModel> Read(BookBindingModel model);
        void CreateOrUpdate(BookBindingModel model);
        void Delete(BookBindingModel model);
    }
}
