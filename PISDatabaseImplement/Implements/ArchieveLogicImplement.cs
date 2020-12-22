using PISBusinessLogic.HelperModels;
using PISDatabaseImplements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PISDatabaseImplement.Implements
{
    public class ArchieveLogicImplement : ArchieveLogic
    {
        protected override Assembly GetAssembly()
        {
            return typeof(ArchieveLogicImplement).Assembly;
        }
        protected override List<PropertyInfo> GetFullList()
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
        protected override List<T> GetList<T>()
        {
            using (var context = new DatabaseContext())
            {
                return context.Set<T>().ToList();
            }
        }
    }
}
