using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;

namespace DASHBOARD.DAL
{
    class TransformFormat
    {
        public static List<T> ConvertTableToList<T>(DataTable dt) {
            List<T> data = new List<T>();
            foreach (DataRow row in transformatTable(dt).Rows)
            {
                T item = GetITem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetITem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();
            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties()) {
                    if (pro.Name == column.ColumnName)
                    {
                        pro.SetValue(obj,dr[column.ColumnName], null);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return obj;
        }

        public static DataTable transformatTable(DataTable dt)
        {
            Debug.WriteLine("N° COL: " + dt.Columns.Count + "N° ROWS: " + dt.Rows.Count);
            DataTable dataTable = new DataTable();
            foreach (DataColumn column in dt.Columns)
            {
                DataColumn dataColumn = new DataColumn(column.ColumnName);
                Debug.WriteLine("---------NOMNRE COL: "+ column.ColumnName);
                Debug.WriteLine("TIPO COL AN: " + column.DataType);
                switch (Type.GetTypeCode(column.DataType))
                {
                    case TypeCode.DateTime:
                        dataColumn.DataType = typeof(String);
                        break;
                    default:
                        dataColumn.DataType = column.DataType;
                        break;
                }
                Debug.WriteLine("TIPO COL NU: " + dataColumn.DataType);
                dataTable.Columns.Add(dataColumn);
            }
            foreach (DataRow row in dt.Rows) {
                DataRow dr = dataTable.NewRow();
                int index = 0; 
                foreach (DataColumn co in dataTable.Columns )
                {
                    switch (Type.GetTypeCode(co.DataType))
                    {
                        case TypeCode.String:
                            dr[index] = row[co.ColumnName].ToString();
                            break;
                        case TypeCode.Int32:
                            dr[index] = int.Parse(row[co.ColumnName].ToString());
                            break;
                        case TypeCode.Decimal:
                            dr[index] = decimal.Parse(row[co.ColumnName].ToString());
                            break;
                    }
                    index++;
                }
                //0800 760 107
                dataTable.Rows.Add(dr);
            }
            return dataTable;
        }
    

        public Type ConvertiTipo(SqlDbType sqlDbType)
        {
            var typeMap = new Dictionary<SqlDbType, Type>();

            typeMap[SqlDbType.NVarChar] = typeof(String);
            typeMap[SqlDbType.Int] = typeof(int);
            typeMap[SqlDbType.SmallInt] = typeof(Int16);
            typeMap[SqlDbType.BigInt] = typeof(Int64);
            typeMap[SqlDbType.VarBinary] = typeof(Byte[]);
            typeMap[SqlDbType.Bit] = typeof(Boolean);
            typeMap[SqlDbType.DateTime2] = typeof(DateTime);
            typeMap[SqlDbType.DateTimeOffset] = typeof(DateTimeOffset);
            typeMap[SqlDbType.Decimal] = typeof(Decimal);
            typeMap[SqlDbType.Float] = typeof(Double);
            typeMap[SqlDbType.Money] = typeof(Decimal);
            typeMap[SqlDbType.TinyInt] = typeof(Byte);
            typeMap[SqlDbType.Time] = typeof(TimeSpan);

            return typeMap[(sqlDbType)];
        }
    }
}
