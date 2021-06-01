using System;
using System.Collections.Generic;
using System.Data;

namespace Sks365.Ippica.Repository.Extensions
{
    public static class DataReaderExtensions
    {
        public static List<T> GetDataObjects<T>(this IDataReader reader) where T : class, new()
        {
            var list = new List<T>();

            if (reader == null)
                return list;

            HashSet<string> tableColumnNames = null;
            while (reader.Read())
            {
                tableColumnNames = tableColumnNames ?? CollectColumnNames(reader);
                var entity = new T();
                foreach (var propertyInfo in typeof(T).GetProperties())
                {
                    object retrievedObject = null;
                    if (tableColumnNames.Contains(propertyInfo.Name) && (retrievedObject = reader[propertyInfo.Name]) != DBNull.Value)
                    {
                        propertyInfo.SetValue(entity, retrievedObject, null);
                    }
                }
                list.Add(entity);
            }
            // reader.Close();

            return list;
        }

        private static HashSet<string> CollectColumnNames(IDataReader reader)
        {
            var collectedColumnInfo = new HashSet<string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                collectedColumnInfo.Add(reader.GetName(i));
            }
            return collectedColumnInfo;
        }

    }
}


