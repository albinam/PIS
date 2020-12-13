using PISBusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PISBusinessLogic.HelperModels
{
    public class ReportLogic
    {
        public void SaveTravelToursToWordFile(string fileName, LibraryCardViewModel model)
        {
            string title = "Читательский билет №" + model.Id;
            SaveToWord.CreateDoc(new WordInfo
            {
                FileName = fileName,
                Title = title,
                libraryCard=model

            }) ;
        }
    }
}
