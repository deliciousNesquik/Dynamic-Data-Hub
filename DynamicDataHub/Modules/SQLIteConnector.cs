using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace DynamicDataHub.Modules{
    public class SQLIteConnector {

        private string pathFileDB;
        private IDbConnection GetConnection;

        public static string NameDBManagementSystem { get; private set; } = "SQLite";

        public SQLIteConnector(string pathFileDB) 
        {
            this.pathFileDB = pathFileDB;

        }
        public DataTable GetColumnTable(string TableName)
        {
            return CreateQuery($"pragma table_info('{TableName}')");
        }


        public DataTable DeleteRow(string TableName, string NameColumnTable, string ColumnValue)
        {
            return CreateQuery($"delete from {TableName} where {NameColumnTable} = {ColumnValue}");
        }

        public DataTable CreateQuery(string query){

            GetConnection = new SQLiteConnection($"Data Source={this.pathFileDB};Version=3;");
            DataTable databases = new DataTable("Databases");

            try
            {
                using (IDbConnection connection = this.GetConnection)
                {
                    connection.Open();
                    IDbCommand command = connection.CreateCommand();
                    command.CommandText = query;
                    databases.Load(command.ExecuteReader(CommandBehavior.CloseConnection));
                    connection.Close();
                }
                return databases;
            }
            catch (SqlException)
            {
                MessageBox.Show("SqlException");
                return null;
            }
            catch (Exception)
            {
                MessageBox.Show("Exception");
                return null;
            }
 
        }

        public DataTable UpdateRow(string tableName, string editingColumn, string editingElement, string basedColumn, string valueBasedColumn)
        {
            return CreateQuery($"update [{tableName}] set [{editingColumn}] = '{editingElement}' where [{basedColumn}] = '{valueBasedColumn}'");
        }

        public DataTable AddRow(string TableName, Dictionary<string, string> keyValuePairs)
        {
            string query = $"INSERT INTO {TableName}(";

            foreach (var key in keyValuePairs.Keys)
            {
                query += $"{key},";
            }

            int lastIndexColumns = query.LastIndexOf(',');
            if (lastIndexColumns != -1)
            {
                query = query.Remove(lastIndexColumns, 1);
            }

            query += ") VALUES (";

            foreach (var value in keyValuePairs.Values)
            {
                query += $"'{value}',";
            }

            int lastIndexValues = query.LastIndexOf(',');
            if (lastIndexValues != -1)
            {
                query = query.Remove(lastIndexValues, 1);
            }

            query += ")";

            return CreateQuery(query);
        }

        public async Task<bool> GetInfoConnection()
        {
            bool isConnected = false;
            GetConnection = new SQLiteConnection($"Data Source={this.pathFileDB};Version=3;");
            try
            {
                await Task.Run(() => {
                    using (IDbConnection connection = GetConnection)
                    {
                        try
                        {
                            connection.Open();
                            isConnected = true;
                            connection.Close();
                        }
                        catch (SqlException sqlEx)
                        {
                            Console.WriteLine($"SQL Exception при подключении к базе данных: {sqlEx.Message}");
                            isConnected = false;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Общая ошибка при подключении к базе данных: {ex.Message}");
                isConnected = false;
            }

            return isConnected;
        }

        public List<string> GetDBTables()
        {
            List<string> tables = new List<string>();
            var databases = CreateQuery("SELECT name FROM sqlite_schema WHERE type='table';");

            foreach (DataRow row in databases.Rows)
            {
                tables.Add(row[0].ToString());
            }

            return tables;
        }
    }
}
