using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace DynamicDataHub.Modules{
    internal class SQLIteConnector{
        private string pathFileDB;
        public SQLIteConnector(string pathFileDB){
            this.pathFileDB = pathFileDB;
        }

        public bool GetInfoConnection() {
            try{
                using (SQLiteConnection conn = new SQLiteConnection($"Data Source={this.pathFileDB};Version=3;")){
                    conn.Open();

                    Console.WriteLine("Подключено к базе данных SQLite");

                    conn.Close();
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
