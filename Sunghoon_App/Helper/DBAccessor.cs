using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunghoon_App.Helper
{
    public static class DBAccessor
    {
        public static string tableName = 
            DateTime.Now.ToLocalTime().ToString("Tyyyy_mm_dd");
        public static string dbFileName = "mysql.db";

        public static void InitializeDatabase()
        {
            using (SqliteConnection db = new SqliteConnection("Filename=mysql.db"))
            {
                db.Open();
                String tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS " + DateTime.Now.ToLocalTime().ToString("Tyyyy_mm_dd") + 
                    " (Primary_Key INTEGER PRIMARY KEY AUTO_INCREMENT, " +
                    " Time String, " +
                    " Sole Integer, " +
                    " Sole_gap Integer," +
                    " Foreigner Integer, "+
                    " Foreigner_gap Integer, "+
                    " Inst Integer, "+
                    " Inst_gap Integer)";

                SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteReader();
            }
        }
    }
}
