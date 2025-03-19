using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using DotNetEnv;
using Microsoft.Data.SqlClient;

namespace DataAccessLayer
{
    public class DataBaseConnection
    {

        SqlConnection sqlConnection;
        private string connectionString;

        public DataBaseConnection()
        {

            string envPath = Path.Combine(AppContext.BaseDirectory, "login.env");
            Env.Load(envPath);


            //DATABASE_URL=Server=servername;Database=MarketPlace;User Id=user;Password=password;TrustServerCertificate=True
            this.connectionString = Env.GetString("DATABASE_URL");
            Console.WriteLine($"Current Directory: {Directory.GetCurrentDirectory()}");
            Debug.WriteLine($"DATABASE_URL: {Env.GetString("DATABASE_URL")}");
            Console.WriteLine(connectionString);
            
            this.sqlConnection = new SqlConnection(connectionString);
        }

        public SqlConnection GetConnection()
        {
            return this.sqlConnection;
        }

        public void OpenConnection()
        {
            this.sqlConnection.Open();
        }

        public void CloseConnection()
        {
            this.sqlConnection.Close();
        }

    }
}
