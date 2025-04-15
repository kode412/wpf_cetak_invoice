using System.Data.SQLite;
using System.IO;


namespace Invoice
{
    public static class DatabaseConn
    {
        private static SQLiteConnection _connection;
        public static SQLiteConnection GetConnection()
        {
            if(_connection == null)
            {
                _connection = new SQLiteConnection("Data Source=InvoiceDB.sqlite;Version=3;");
                InitializeDatabase();
            }
            
            return _connection;
        }
        public static void InitializeDatabase() 
        {
            if (!File.Exists("InvoiceDB.sqlite"))
            {
                SQLiteConnection.CreateFile("InvoiceDB.sqlite");
                using (var cmd = new SQLiteCommand(_connection))
                {
                    _connection.Open();
                    cmd.CommandText = @"CREATE TABLE IF NOT EXITS iNVOICES (
                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        Struk TEXT,
                                        Pembayaran TEXT,
                                        Tanggal TEXT)";
                    cmd.ExecuteNonQuery();
                }

            }
            else 
            {
                _connection.Open();
            }
        }
        public static void CloseConnection()
        {
            if (_connection != null && _connection.State != System.Data.ConnectionState.Closed)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }
        }

    }
}
