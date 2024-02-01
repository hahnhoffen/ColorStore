using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolProjectColorStore
{
    public class Databaseconnection
    {
        string server = "localhost";
        string database = "SCHOOL_STORE";
        string username = "root";
       // string password = "Ditt Lösenord";

        string connectionstring = "";
        public Databaseconnection(string server, string database, string username, string password)
        {
            connectionstring =
                "SERVER=" + server + ";" +
                "DATABASE=" + database + ";" +
                "UID=" + username + ";" +
                "PASSWORD=" + password + ";";
        }
        public string GetConnectionString()
        {
            return connectionstring;
        }
    }
}
