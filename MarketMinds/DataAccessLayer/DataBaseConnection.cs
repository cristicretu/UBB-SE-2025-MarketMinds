using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace DataAccessLayer
{
    public class DataBaseConnection
    {
        SqlConnection sqlConnection;
        //replace with own
        private string connectionString = "Server=GHASTERLAPTOP;Database=MarketPlace;User Id=sa;Password=1808;TrustServerCertificate=True";
        

        public DataBaseConnection()
        {
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

