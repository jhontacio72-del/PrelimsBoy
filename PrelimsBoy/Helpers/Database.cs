using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrelimsBoy.Helpers
{
    internal class Database
    {
        private static string connString = "server=localhost;database=user_db;uid=root;pwd=;";

       
        public static MySqlConnection GetConnection()
        {
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                return connection;
            }
            catch (MySqlException ex)
            {
               
                Console.WriteLine($"Database Connection Error: {ex.Message}");
                
                return null; 
            }
        }
    }
}
