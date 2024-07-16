using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDataHub.Modules
{
    internal class SQLServerConnector
    {
        public string ServerName;
        public string DBName;
        public string connectionString;

        public SQLServerConnector(string ServerName, string DBName) {
            this.ServerName = ServerName;
            this.DBName = DBName;
            connectionString = "Data Source=" + this.ServerName + ";Initial Catalog='" + this.DBName + "';Integrated Security=True;trustservercertificate=True";
    }
        
        public bool GetInfoConnection(){
            try{
                using (SqlConnection connection = new SqlConnection(connectionString)){
                    connection.Open();

                    connection.Close(); 
                    return true;
                }
            }
            catch (Exception ex){
                Console.WriteLine($"Ошибка при подключении к базе данных: {ex.Message}");
                return false;
            }
}
    }
}
