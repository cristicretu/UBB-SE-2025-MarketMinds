using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetEnv;
using Microsoft.Data.SqlClient;

namespace DataAccessLayer
{
    internal class DataBaseConnection
    {
        SqlConnection sqlConnection = null;

        static void main(string[] args)
        {
            Env.Load();

            //DATABASE_URL=Server=servername;Database=MarketPlace;User Id=user;Password=password
            string connectionString = Env.GetString("DATABASE_URL");

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    Console.WriteLine("Connected");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
