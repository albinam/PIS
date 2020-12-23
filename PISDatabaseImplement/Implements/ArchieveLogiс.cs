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
    public class ArchieveLogiс { 
        protected Assembly GetAssembly()
        {
            return typeof(ArchieveLogiс).Assembly;
        }
        protected List<PropertyInfo> GetFullList()
        {
            using (var context = new DatabaseContext())
            {
                Type type = context.GetType();
                List <PropertyInfo> list = new List < PropertyInfo >  ();
                list.AddRange(type.GetProperties().Where(x =>
                x.PropertyType.FullName.StartsWith("Microsoft.EntityFrameworkCore.DbSet")).ToList());
                list.RemoveRange(1, 4);
                list.RemoveRange(2, 2);
                return list;
               
            }
        }
        public void CreateArchive(string folderName)
        {
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
                string fileName = $"{folderName}.zip";
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                // берем сборку, чтобы от нее создавать объекты
                Assembly assem = GetAssembly();
                // вытаскиваем список классов для сохранения
                var dbsets = GetFullList();
                // берем метод для сохранения (из базвого абстрактного класса)
               
                foreach (var set in dbsets)
                {
                    if (set.Name == "Books")
                    {
                        MethodInfo method =
                  GetType().GetTypeInfo().GetDeclaredMethod("SaveToFileBooks");
                        // создаем объект из класса для сохранения
                        var elem =
                        assem.CreateInstance(set.PropertyType.GenericTypeArguments[0].FullName);
                        // генерируем метод, исходя из класса
                        MethodInfo generic = method.MakeGenericMethod(elem.GetType());
                        // вызываем метод на выполнение
                        generic.Invoke(this, new object[] { folderName });
                    }
                    if (set.Name == "LibraryCards")
                    {
                        MethodInfo method =
                  GetType().GetTypeInfo().GetDeclaredMethod("SaveToFileCards");
                        // создаем объект из класса для сохранения
                        var elem =
                        assem.CreateInstance(set.PropertyType.GenericTypeArguments[0].FullName);
                        // генерируем метод, исходя из класса
                        MethodInfo generic = method.MakeGenericMethod(elem.GetType());
                        // вызываем метод на выполнение
                        generic.Invoke(this, new object[] { folderName });
                    }
                   
                }
                // архивируем
                ZipFile.CreateFromDirectory(folderName, fileName);
                // удаляем папку
                dirInfo.Delete(true);
            }
            catch (Exception)
            {
                // делаем проброс
                throw;
            }
        }
        private void SaveToFileBooks<T>(string folderName) where T : Book, new()
        {
            var records = GetList<Book>();
            List<Book> records2 = new List<Book>();
            T obj = new T();

            foreach (var el in records)
            {
                if (Convert.ToInt32(el.Year)>2000)
                    records2.Add(el);
            }

            DataContractJsonSerializer jsonFormatter = new
           DataContractJsonSerializer(typeof(List<T>));
            using (FileStream fs = new FileStream(string.Format("{0}/{1}.json",
           folderName, obj.GetType().Name), FileMode.OpenOrCreate))
            {
                jsonFormatter.WriteObject(fs, records2);
            }
        }
        private void SaveToFileCards<T>(string folderName) where T : LibraryCard, new()
        {
            var records = GetList<LibraryCard>();
            List<LibraryCard> records2 = new List<LibraryCard>();
            T obj = new T();

            foreach (var el in records)
            {
                if (Convert.ToInt32(el.Year) > 2000)
                    records2.Add(el);
            }

            DataContractJsonSerializer jsonFormatter = new
           DataContractJsonSerializer(typeof(List<T>));
            using (FileStream fs = new FileStream(string.Format("{0}/{1}.json",
           folderName, obj.GetType().Name), FileMode.OpenOrCreate))
            {
                jsonFormatter.WriteObject(fs, records2);
            }
        }
        protected List<T> GetList<T>() where T : class, new()
        {
            using (var context = new DatabaseContext())
            {
                return context.Set<T>().ToList();
            }
        }
    }
}
