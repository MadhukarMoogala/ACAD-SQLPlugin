using System;
using System.Data.SqlClient;
using System.Text;

namespace DbAccessUtil
{
    public class DataAccessor
    {
        private readonly string _connectionString;
        private readonly StringBuilder _errorMsg = new StringBuilder();

        public DataAccessor(string sqlConnectionString)
        {
            _connectionString = sqlConnectionString;
        }

        public string ErrorMsg => _errorMsg.ToString();

        public bool TestSqlServerConnection()
        {
            _errorMsg.Length = 0;

            var connectionOk = false;
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    // Do something

                    connectionOk = true;
                    connection.Close();
                }
            }
            catch(System.Exception ex)
            {
                _errorMsg.Append($"Cannot connect to Database server:\n{ex.Message}");
            }

            return connectionOk;
        }
    }
}
