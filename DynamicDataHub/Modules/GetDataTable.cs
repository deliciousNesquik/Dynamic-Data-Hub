using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DynamicDataHub.Modules
{
    internal class GetDataTable
    {
        private int indexOfColumn;
        private int indexOfDataType;
        private int indexOfIsNullable;

        private SQLServerConnector sqlServerDB;
        private SQLIteConnector sqliteDB;

        public List<string> nullableColumns { get; private set; } = new List<string>();

        private string serverName;
        private string filePathDb;

        public GetDataTable()
        {
            this.serverName = DatabaseConfiguration.serverName;
            this.filePathDb = DatabaseConfiguration.filePathDb;
        }

        public DataTable GetDataTableSQLServer(string tableName, string dbName)
        {
            nullableColumns.Clear();
            sqlServerDB = new SQLServerConnector(this.serverName);

            DataTable table = new DataTable(tableName);
            DataTable databases = sqlServerDB.GetColumnTable(tableName, dbName);


            indexOfColumn = 0;
            indexOfDataType = 1;
            indexOfIsNullable = 2;

            int rows_columns = databases.Rows.Count;

            foreach (DataRow _row in databases.Rows)
            {
                if (_row[indexOfIsNullable].ToString() == "YES")
                {
                    nullableColumns.Add(_row[indexOfColumn].ToString());
                }
                switch (_row[indexOfDataType].ToString())
                {
                    case "int":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(Int32));
                        break;
                    case "nvarchar":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(String));
                        break;
                    case "date":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(DateTime));
                        break;
                    case "bit":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(Boolean));
                        break;
                    case "nchar":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(String));
                        break;
                    case "char":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(String));
                        break;
                    case "varchar":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(String));
                        break;
                    case "float":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(float));
                        break;
                    case "decimal":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(decimal));
                        break;
                    case "datetime":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(DateTime));
                        break;
                    case "image":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(string));
                        break;
                    case "uniqueidentifier":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(string));
                        break;
                    case "sysname":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(string));
                        break;
                    case "varbinary":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(string));
                        break;
                    default:
                        break;
                }
            }

            databases = sqlServerDB.IsIdentityColumn(tableName, dbName);

            foreach (DataColumn column in table.Columns)
            {
                string columnName = column.ColumnName.ToString();

                foreach (DataRow row in databases.Rows)
                {
                    if (row["name"].ToString() == columnName)
                    {
                        column.ReadOnly = true;
                    }
                }

            }


            databases = sqlServerDB.CreateQuery($"SELECT * FROM [{tableName}]", dbName);
            int count_columns = databases.Columns.Count;

            List<object> row_values = new List<object>();
            foreach (DataRow _row in databases.Rows)
            {
                for (int i = 0; i < count_columns; i++)
                {
                    row_values.Add(_row[i]);
                }

                table.Rows.Add(row_values.ToArray());

                row_values.Clear();
            }

            return table;
        }

        public DataTable GetDataTableSQLite(string tableName)
        {
            nullableColumns.Clear();

            sqliteDB = new SQLIteConnector(this.filePathDb);

            DataTable table = new DataTable(tableName);
            DataTable dataBases = sqliteDB.GetColumnTable(tableName);

            indexOfColumn = 1;
            indexOfDataType = 2;
            indexOfIsNullable = 3;

            int rows_columns = dataBases.Rows.Count;

            foreach (DataRow _row in dataBases.Rows)
            {
                if (Int32.Parse(_row[indexOfIsNullable].ToString()) == 0)
                {
                    nullableColumns.Add(_row[indexOfColumn].ToString());
                }

                switch (_row[indexOfDataType].ToString())
                {
                    case "INTEGER":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(Int32));
                        break;
                    case "TEXT":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(String));
                        break;
                    case "BLOB":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(byte));
                        break;
                    case "REAL":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(float));
                        break;
                }
            }


            dataBases = sqliteDB.CreateQuery($"SELECT * FROM [{tableName}]");

            int countСolumns = dataBases.Columns.Count;

            List<object> rowValues = new List<object>();
            foreach (DataRow _row in dataBases.Rows)
            {
                for (int i = 0; i < countСolumns; i++)
                {
                    rowValues.Add(_row[i]);
                }
                try
                {
                    table.Rows.Add(rowValues.ToArray());
                }
                catch (ArgumentException)
                {
                    //customMessageBoxBuilder = new CustomMessageBoxBuilder();
                    //customMessageBoxBuilder.ShowError("Ошибка", "У столбца в таблице отсутствует тип данных", "Ok", window);
                }

                rowValues.Clear();
            }

            return table;
        }
    }
}
