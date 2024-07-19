using System;
using System.Collections.Generic;
using System.Data;
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
        private IDbConnection GetConnection;

        public SQLServerConnector(string ServerName, string DBName)
        {
            this.ServerName = ServerName;
            this.DBName = DBName;
            this.GetConnection = new SqlConnection("Data Source=" + this.ServerName + ";Initial Catalog='" + this.DBName + "';Integrated Security=True;trustservercertificate=True");
        }

        public async Task<bool> GetInfoConnection()
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
    }

}

