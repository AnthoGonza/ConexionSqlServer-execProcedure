using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;

namespace DASHBOARD.DAL
{
    public class Conexion : IDisposable
    {
        public SqlParameterCollection parameters { get; set; }
        public string catalog { get; set; }
        public string command { get; set; }
        public CommandType commandType { get; set; }

        public Conexion() {
            this.catalog = "";
            this.command = "";
            this.commandType = CommandType.Text;
            this.parameters = (SqlParameterCollection)typeof(SqlParameterCollection).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null).Invoke(null);
        }
        public Conexion(string catalog,string command, CommandType commandType)
        {
            this.catalog = catalog;
            this.command = command;
            this.commandType = commandType;
            this.parameters = (SqlParameterCollection)typeof(SqlParameterCollection).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null).Invoke(null);
        }

        public DataTable executeQuery() {
            try {
                using (SqlConnection sqlCon = new SqlConnection(ConfigurationManager.ConnectionStrings[this.catalog.ToUpper()].ToString()))
                {
                    sqlCon.Open();
                    using (SqlCommand sqlCom = new SqlCommand(this.command, sqlCon)) {
                        if (this.parameters != null) {
                            foreach (SqlParameter parameter in this.parameters) {
                                sqlCom.Parameters.AddWithValue(parameter.ParameterName,parameter.Value);
                            }
                        }
                        sqlCom.CommandType = this.commandType;

                        using (SqlDataAdapter adapter = new SqlDataAdapter(sqlCom)) {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                return dt;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                Debug.WriteLine("Error/executeQuery/Conexion/DASHBOARD.DAL: " + ex);
                return null;
            }
        }

        public List<T> executeQuery<T>() where T: new()
        {
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(ConfigurationManager.ConnectionStrings[this.catalog.ToUpper()].ToString()))
                {
                    sqlCon.Open();
                    using (SqlCommand sqlCom = new SqlCommand(this.command, sqlCon))
                    {
                        if (this.parameters != null)
                        {
                            foreach (SqlParameter parameter in this.parameters)
                            {
                                sqlCom.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
                            }
                        }
                        sqlCom.CommandType = this.commandType;
                        verParamter(sqlCom);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(sqlCom))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill (dt);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                return TransformFormat.ConvertTableToList<T>(dt);
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error/executeQuery/Conexion/DASHBOARD.DAL: " + ex);
                return null;
            }
        }
        public void Dispose() {
        }

    }
}
