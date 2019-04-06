using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;

namespace Conexion.DAL
{
    class TransformFormat
    {
        public static List<T> ConvertTableToList<T>(DataTable dt) 
        {
            var columnName = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
            var properties = typeof(T).GetProperties();
            return dt.AsEnumerable().Select(row=> {
                var objT = Activator.CreateInstance<T>();
                foreach (var pro in properties)
                {
                    if (columnName.Contains(pro.Name))
                    {
                        PropertyInfo property = objT.GetType().GetProperty(pro.Name);
                        pro.SetValue(objT, row[pro.Name] == DBNull.Value ? null : Convert.ChangeType(row[pro.Name], property.GetType()));
                    }
                }
                return objT;
            }).ToList();
        }
       
    }
}
