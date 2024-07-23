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

namespace DynamicDataHub.Modules{
    public class SQLIteConnector {
        private string pathFileDB;
        private IDbConnection GetConnection;

        public static string NameDBManagementSystem = "SQLite";

        public SQLIteConnector(string pathFileDB) 
        {
            this.pathFileDB = pathFileDB;
            this.GetConnection = new SQLiteConnection($"Data Source={this.pathFileDB};Version=3;");
        }

        public DataTable GetColumnTable(ListBox TableList)
        {
            return CreateQuery("pragma table_info('" + TableList.SelectedItem.ToString() + "')");
        }

        public DataTable CreateQuery(string query){
            DataTable databases = new DataTable("Databases");

            try
            {
                using (IDbConnection connection = this.GetConnection)
                {
                    IDbCommand command = connection.CreateCommand();
                    command.CommandText = query;
                    connection.Open();
                    databases.Load(command.ExecuteReader(CommandBehavior.CloseConnection));
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

        public bool GetInfoConnection() {
            try{
                using (IDbConnection connection = this.GetConnection)
                {
                    connection.Open();

                    Console.WriteLine("Подключено к базе данных SQLite");

                    connection.Close();
                    return true;
                }
            }
            catch (Exception ex){
                Console.WriteLine($"Ошибка при подключении к базе данных: {ex.Message}");
                return false;
            }
        }

        public void GetDBTables(ListBox TableList)
        {

            var databases = CreateQuery("SELECT name FROM sqlite_schema WHERE type='table';");

            foreach (DataRow row in databases.Rows)
            {
                TableList.Items.Add(row[0].ToString());
            }
        }
    }
}
