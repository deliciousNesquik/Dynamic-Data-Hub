using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace DynamicDataHub.Modules
{
    public class SQLServerConnector 
    {
        private string serverName;
        private IDbConnection GetConnection;

        public static string NameDBManagementSystem = "SQL Server Management Studio";


        public SQLServerConnector(string serverName)
        {
            this.serverName = serverName;
        }

        public DataTable GetColumnTable(string TableName, string DBName)
        {
            return CreateQuery($"SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'{TableName}'", DBName);
        }

        public List<string> GetDBNames()
        {
            List<string> DBNames = new List<string>();
            var databases = CreateQuery("SELECT name FROM sys.databases");

            foreach (DataRow row in databases.Rows)
            {
                DBNames.Add(row[0].ToString());
            }

            return DBNames;
        }

        public DataTable CreateQuery(string query, string DBName=""){

            DataTable databases = new DataTable("Databases");
            GetConnection = new SqlConnection($"Data Source={this.serverName};Initial Catalog='{DBName}';Integrated Security=True;trustservercertificate=True");
            try
            {
                using (IDbConnection connection = GetConnection){
                    IDbCommand command = connection.CreateCommand();
                    command.CommandText = query;
                    connection.Open();
                    databases.Load(command.ExecuteReader(CommandBehavior.CloseConnection));
                }
                return databases;
            }
            catch (SqlException){
                MessageBox.Show("SqlException");
                return null;
            }
            catch (Exception){
                MessageBox.Show("Exception");
                return null;
            }
        }

        public DataTable DeleteRow(string TableName, string ColumnName, string ValueColumn ,string DBName)
        {
            return CreateQuery($"delete from [{TableName}] where [{ColumnName}] = N'{ValueColumn}'", DBName);
        }

        public DataTable UpdateRow(string dbName, string tableName, string editingColumn, string editingElement, string basedColumn, string valueBasedColumn)
        {
            return CreateQuery($"update [{tableName}] set [{editingColumn}] = '{editingElement}' where [{basedColumn}] = '{valueBasedColumn}'", dbName);
        }

        public DataTable IsIdentityColumn(string tableName, string dbName)
        {
            return CreateQuery($"SELECT name FROM sys.columns WHERE object_id = object_id('{tableName}') and is_identity = 1", dbName);
        }

        public  async Task<bool> GetInfoConnection()
        {
            bool isConnected = false;
            GetConnection = new SqlConnection("Data Source=" + this.serverName + ";Initial Catalog='';Integrated Security=True;trustservercertificate=True");
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

        public List<string> GetDBTables(string DBName) {
            List<string> tables = new List<string>();

            var databases = CreateQuery($"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'", DBName);

            foreach (DataRow row in databases.Rows){
                if (row[0].ToString() == "sysdiagrams"){
                    continue;
                }
                tables.Add(row[0].ToString());
            }

            return tables;
        }
    }

}

