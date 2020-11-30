using PISBusinessLogic.BindingModels;
using PISBusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PISBusinessLogic.Interfaces
{
    public interface IPaymentLogic
    {
        List<PaymentViewModel> Read(PaymentBindingModel model);
        void CreateOrUpdate(PaymentBindingModel model);
        void Delete(PaymentBindingModel model);
    }
}
