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

        public const string nameDBManagementSystem = "SQLite";

        public SQLIteConnector(string pathFileDB) 
        {
            this.pathFileDB = pathFileDB;

        }
        public DataTable GetColumnTable(string TableName)
        {
            return CreateQuery($"pragma table_info('{TableName}')");
        }

        public List<string> GetColumnNames(string tableName)
        {
            var columns = new List<string>();
            var databases = CreateQuery($"SELECT c.name FROM pragma_table_info('{tableName}') c;");

            foreach (DataRow row in databases.Rows)
            {
                columns.Add(row[0].ToString());
            }

            return columns;
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
                    var t = command.ExecuteReader(CommandBehavior.CloseConnection);
                    int rowsAffected = t.RecordsAffected;
                    DatabaseConfiguration.countRowsAffected = rowsAffected;
                    databases.Load(t);
                    connection.Close();
                }
                return databases;
            }
            catch (SqlException ex)
            {
                DatabaseConfiguration.messageOfError = ex.Message;
                //switch (ex.Number)
                //{
                //    case 18456:
                //        MessageBox.Show("Неверный логин или пароль");
                //        break;
                //    case 102:
                //        MessageBox.Show("Некорректный синтаксис запроса");
                //        break;
                //    case 18451:
                //        MessageBox.Show("Не удалось установить соединение с сервером");
                //        break;
                //    case 156:
                //        MessageBox.Show("Недопустимый параметр в функции или процедуре");
                //        break;
                //    case 229:
                //        MessageBox.Show("Недостаточно прав для выполнения операции");
                //        break;
                //    case 2601:
                //        MessageBox.Show("Запись в таблицу запрещена");
                //        break;
                //    case 262:
                //        MessageBox.Show("Недостаточно прав для выполнения операции");
                //        break;
                //    case 2627:
                //        MessageBox.Show("Конфликт конкуренции (два процесса пытаются изменить одну и ту же строку)");
                //        break;
                //    case 2629:
                //        MessageBox.Show("Попытка вставить дублирующуюся запись в столбец, ограниченный UNIQUE");
                //        break;
                //    case 266:
                //        MessageBox.Show("Попытка вставить дубликат ключевого значения в столбце, ограниченном PRIMARY KEY или UNIQUE");
                //        break;
                //    case 2720:
                //        MessageBox.Show("Попытка вставить значение, которое превышает максимально допустимый размер для столбца");
                //        break;
                //    case 2746:
                //        MessageBox.Show("Попытка вставить значение NULL в столбец, который не позволяет NULL");
                //        break;
                //    case 245:
                //        MessageBox.Show("Ошибка преобразования типов");
                //        break;
                //    case 547:
                //        MessageBox.Show("Конфликт инструкции DELETE с ограничением REFERENCE в базе данных");
                //        break;
                //    default:
                //        MessageBox.Show($"Неизвестная ошибка {ex.Number} - {ex.Message}");
                //        Clipboard.SetText(ex.Message);
                //        break;

                //}
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
            string query = $"INSERT INTO [{TableName}](";

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
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Общая ошибка при подключении к базе данных: {ex.Message}");
                        isConnected = false;
                    }

                }
            });

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
