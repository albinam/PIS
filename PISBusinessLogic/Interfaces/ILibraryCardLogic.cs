using PISBusinessLogic.BindingModels;
using PISBusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PISBusinessLogic.Interfaces
{
    public interface ILibraryCardLogic
    {
        List<LibraryCardViewModel> Read(LibraryCardBindingModel model);
        void CreateOrUpdate(LibraryCardBindingModel model);
        void Delete(LibraryCardBindingModel model);
    }
}
