using PISBusinessLogic.BindingModels;
using PISBusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PISBusinessLogic.Interfaces
{
    public interface IBookingLogic
    {
        List<BookingViewModel> Read(BookingBindingModel model);
        void CreateOrUpdate(BookingBindingModel model);
        void Delete(BookingBindingModel model);
    }
}
