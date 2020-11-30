using PISBusinessLogic.BindingModels;
using PISBusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PISBusinessLogic.Interfaces
{
    public interface IUserLogic
    {
        List<UserViewModel> Read(UserBindingModel model);
        void CreateOrUpdate(UserBindingModel model);
        void Delete(UserBindingModel model);
    }
}
