using PISBusinessLogic.HelperModels;
using PISDatabaseimplements.Models;
using PISDatabaseImplements;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;

namespace PISDatabaseImplement.Implements
{
    public class ArchiveLogic
    {
        string exportDirectory = Directory.GetCurrentDirectory() + "\\wwwroot\\Export";
        string exportDirectory2 = Directory.GetCurrentDirectory() + "\\wwwroot\\Export2";
        DatabaseContext context;
        public ArchiveLogic()
        {
            context = new DatabaseContext();
        }
        public string ArchiveOutdated(int role)
        {
            DateTime curTime = DateTime.Now;

            string folderName="";
            string fileName="";
            if (role == 1)
            {
                folderName = exportDirectory + "\\Archive" + curTime.Day.ToString() + curTime.Month.ToString() + curTime.Year.ToString();
                fileName = $"{folderName}.zip";
            }
            if (role == 2)
            {
                folderName = exportDirectory2 + "\\Archive" + curTime.Day.ToString() + curTime.Month.ToString() + curTime.Year.ToString();
                fileName = $"{folderName}.zip";
            }

            Directory.CreateDirectory(folderName);
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(folderName);
                if (dirInfo.Exists)
                {
                    foreach (FileInfo file in dirInfo.GetFiles())
                    {
                        file.Delete();
                    }
                }
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                if (role == 1)
                {
                    var books = context.Books.Where(x => Convert.ToInt32(x.Year) < (DateTime.Now.Year - 10));

                    DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Book>));
                    using (FileStream fs = new FileStream(string.Format("{0}/Books.json", folderName), FileMode.OpenOrCreate))
                    {
                        jsonFormatter.WriteObject(fs, books);
                    }
                    var cards = context.LibraryCards.Where(x => Convert.ToInt32(x.Year) < (DateTime.Now.Year - 5));

                    jsonFormatter = new DataContractJsonSerializer(typeof(List<LibraryCard>));
                    using (FileStream fs = new FileStream(string.Format("{0}/LibraryCards.json", folderName), FileMode.OpenOrCreate))
                    {
                        jsonFormatter.WriteObject(fs, cards);
                    }

                }
                if (role == 2)
                {
                    var payments = context.Payments.Where(x => Convert.ToInt32(x.Date.Year) < (DateTime.Now.Year));

                    DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Payment>));
                    using (FileStream fs = new FileStream(string.Format("{0}/Payments.json", folderName), FileMode.OpenOrCreate))
                    {
                        jsonFormatter.WriteObject(fs, payments);
                    }
                }
                ZipFile.CreateFromDirectory(folderName, fileName);
                dirInfo.Delete(true);
            }
            catch (Exception)
            {
                throw;
            }
            return fileName;
        }
    }
}