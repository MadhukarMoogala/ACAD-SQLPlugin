using Autodesk.AutoCAD.Runtime;
using System.Data;
using System.Text;

[assembly: CommandClass(typeof(AutoCAD.SQL.Plugin.EntryCommand))]

namespace AutoCAD.SQL.Plugin
{
    public class EntryCommand
    {
        /// <summary>
        /// AutoCAD Command to connect to SQL Server database and run a query
        /// </summary>
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
            //string connectionString = "data source=LAPTOP-F00D91HG;initial catalog=BikesStores;trusted_connection=true";
            var data = new DatabaseManager();
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
