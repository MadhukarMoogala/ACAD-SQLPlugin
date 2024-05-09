using Autodesk.AutoCAD.EditorInput;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace AutoCAD.SQL.Plugin
{
    //Example DB from https://www.sqlservertutorial.net/wp-content/uploads/SQL-Server-Sample-Database.zip
    /// <summary>
    /// An utility class to access SQL Server database
    /// </summary>
    public class DatabaseManager : IDisposable
    {
        private readonly string? _connectionString;
        private bool disposedValue;

        public DatabaseManager()
        {
            // Target this class for user secrets
            var builder = new ConfigurationBuilder()               
                             .AddUserSecrets<DatabaseManager>(); 

             IConfiguration configuration = builder.Build();
            _connectionString = configuration.GetConnectionString("mssqlserver");
        }
        /// <summary>
        /// To test the connection to the SQL Server database
        /// </summary>
        /// <returns>bool</returns>

        public bool TestSqlServerConnection()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Cannot connect to Database server: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Run a query to get the number of customers in each city in California with more than 10 customers
        /// </summary>
        /// <param name="ed"></param>

        public void RunQueryAndWriteToEditor(Editor ed)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string queryString = @"
                        USE BikesStores;
                        SELECT
                          city,
                          COUNT(*)
                        FROM
                          sales.customers
                        WHERE
                          state = 'CA'
                        GROUP BY
                          city
                        HAVING
                          COUNT(*) > 10
                        ORDER BY
                          city;
                    ";

                    using (var cmd = new SqlCommand(queryString, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    string city = reader.GetString(0); // assuming city is the first column (index 0)
                                    int customerCount = reader.GetInt32(1); // assuming number of customers is the second column (index 1)
                                    ed.WriteMessage($"\nCity: {city}, Customer Count: {customerCount}");
                                }
                            }
                            else
                            {
                                ed.WriteMessage("\nNo results found.");
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage($"\nError executing query: {ex.Message}");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //nothing to dispose, connection is disposed in using block
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
