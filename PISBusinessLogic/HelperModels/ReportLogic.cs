using PISBusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PISBusinessLogic.HelperModels
{
    public class ReportLogic
    {
        public void SaveLibraryCardToWordFile(string fileName, LibraryCardViewModel model)
        {
            string title = "Читательский билет №" + model.Id;
            SaveToWord.CreateDoc(new WordInfo
            {
                FileName = fileName,
                Title = title,
                libraryCard = model,
                book = null

            });
        }
        public void SaveBookToWordFile(string fileName, BookViewModel model)
        {
            string title = "Справка о наличии книги № " + model.Id;
            SaveToWord.CreateDoc(new WordInfo
            {
                FileName = fileName,
                Title = title,
                book = model,
                libraryCard = null

            });
        }
    }
}
