using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T9Project.Models
{
    public class Database
    {
        public static SQLiteConnection db = new SQLiteConnection("Data Source=MyData.db;");

        public Database()
        {

        }
    }
}
