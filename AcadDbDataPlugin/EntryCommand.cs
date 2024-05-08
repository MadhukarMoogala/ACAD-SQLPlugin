

using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System.Data;
using System.Data.SqlClient;
using System.Text;

[assembly: CommandClass(typeof(AutoCAD.SQL.Plugin.EntryCommand))]

namespace AutoCAD.SQL.Plugin
{
    //Example DB from https://www.sqlservertutorial.net/wp-content/uploads/SQL-Server-Sample-Database.zip
    public class DataAccessor
    {
        private readonly string _connectionString;

        public DataAccessor(string connectionString)
        {
            _connectionString = connectionString;
        }

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
    }
    public class EntryCommand
    {

        [CommandMethod("ConnectDb")]
        public static void ConnectDb()
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            if(doc is null)
            {
                return;
            }   
            var ed = doc.Editor;
            // Consider storing connection string in a configuration file for security
            string connectionString = "data source=LAPTOP-F00D91HG;initial catalog=BikesStores;trusted_connection=true";
            var data = new DataAccessor(connectionString);
            try
            {
                data.TestSqlServerConnection();
                ed.WriteMessage("\nConnected to SQL Server database successfully!");
                data.RunQueryAndWriteToEditor(ed);
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage($"\nConnecting to SQL Server database failed!\n{ex.Message}");
            }
           
        }
    }
}
