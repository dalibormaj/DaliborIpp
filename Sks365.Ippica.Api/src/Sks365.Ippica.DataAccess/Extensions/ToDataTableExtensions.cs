using Sks365.Ippica.Common.Exceptions;
using Sks365.Ippica.Common.Utility;
using Sks365.Ippica.Domain.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Sks365.Ippica.Repository.Extensions
{
    public static class ToDataTableExtensions
    {
        /// <summary>
        /// Convert list to DataTable. Conversion will be create a table with the same names as the fields in the class. 
        /// Note: The type of elements of the list must not be a ValueType
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="dataTableName"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this List<T> list, string dataTableName = null) where T : BaseDomainModel
        {
            //get the type of the list items
            var itemType = list.GetType().GetGenericArguments().FirstOrDefault();

            //If the type of the item is value type throw an exception. Not covered at the moment.
            if (itemType.IsValueType) throw new IppicaException(ReturnCodeEnum.Unknown, "Cannot convert to DataTable");

            var table = new DataTable(dataTableName ?? itemType.Name);
            var propertyInfos = itemType.GetProperties().Where(x => CanBeMapped(GetUnderlyingType(x.PropertyType)));

            //DataTable columns
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                var propertyType = GetUnderlyingType(propertyInfo.PropertyType);
                table.Columns.Add(propertyInfo.Name, propertyType);
            }

            //DataTable rows
            foreach (T item in list)
            {
                var row = table.NewRow();
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    row[propertyInfo.Name] = itemType.GetProperty(propertyInfo.Name).GetValue(item, null) ?? DBNull.Value; ;
                }
                table.Rows.Add(row);
            }
            return table;
        }

        public static DataTable ToDataTable<T>(this T model, string dataTableName = null) where T : BaseDomainModel
        {
            var table = new DataTable(dataTableName ?? typeof(T).Name);
            var row = table.NewRow();

            var propertyInfos = typeof(T).GetProperties().Where(x => CanBeMapped(GetUnderlyingType(x.PropertyType)));

            //DataTable
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                var propertyType = GetUnderlyingType(propertyInfo.PropertyType);
                table.Columns.Add(propertyInfo.Name, propertyType);
                row[propertyInfo.Name] = typeof(T).GetProperty(propertyInfo.Name).GetValue(model, null) ?? DBNull.Value;
            }
            return table;
        }

        private static Type GetUnderlyingType(Type x)
        {
            var type = Nullable.GetUnderlyingType(x);

            if (type == null && x.IsEnum)
            {
                type = Enum.GetUnderlyingType(x);
            }
            else if (type != null && type.IsEnum)
            {
                type = Enum.GetUnderlyingType(type);
            }

            return type ?? x;
        }

        private static bool CanBeMapped(Type x)
        {
            var type = GetUnderlyingType(x);
            return (type.IsValueType || type == typeof(string));
        }
    }
}
