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

namespace DynamicDataHub.Modules
{
    public class SQLServerConnector
    {
        public string ServerName;
        public string DBName;
        private IDbConnection GetConnection;

        public static string NameDBManagementSystem = "SQL Server Management Studio";


        public SQLServerConnector(string ServerName, string DBName)
        {
            this.ServerName = ServerName;
            this.DBName = DBName;
            this.GetConnection = new SqlConnection("Data Source=" + this.ServerName + ";Initial Catalog='" + this.DBName + "';Integrated Security=True;trustservercertificate=True");
        }

        public DataTable GetColumnTable(ListBox TableList)
        {
            return CreateQuery("SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'" + TableList.SelectedItem.ToString() + "'");
        }

        

        public DataTable CreateQuery(string query){

            DataTable databases = new DataTable("Databases");

            try
            {
                using (IDbConnection connection = this.GetConnection){
                    IDbCommand command = connection.CreateCommand();
                    command.CommandText = query;
                    connection.Open();
                    databases.Load(command.ExecuteReader(CommandBehavior.CloseConnection));
                }
                this.GetConnection.Close();
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

        public  async Task<bool> GetInfoConnection()
        {
            bool isConnected = false;
            try
            {
                await Task.Run(() => {
                    using (IDbConnection connection = this.GetConnection)
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

        public void GetDBTables(ListBox TableList) {

            var databases = CreateQuery("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'");

            foreach (DataRow row in databases.Rows){
                if (row[0].ToString() == "sysdiagrams"){
                    continue;
                }
                TableList.Items.Add(row[0].ToString());
            }
        }
    }

}

