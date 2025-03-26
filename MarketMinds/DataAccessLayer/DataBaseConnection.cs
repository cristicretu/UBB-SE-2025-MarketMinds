using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccessLayer
{
    public class DataBaseConnection
    {
        SqlConnection sqlConnection;

        // TONI: Data Source=LAPTOP-TONI\SQLEXPRESS;Initial Catalog= MarketPlace;Integrated Security=True; TrustServerCertificate=True;
        // MALOS  "Server=GHASTERLAPTOP;Database=MarketPlace;User Id=sa;Password=1808;TrustServerCertificate=True";
        //COSTIN: Data Source = COSTIN\SQLEXPRESS; Initial Catalog = MarketPlace; Integrated Security = True; TrustServerCertificate=True;
        //private string connectionString = "Server=GHASTERLAPTOP;Database=MarketPlace;User Id=sa;Password=1808;TrustServerCertificate=True";
        //ROBERT private string connectionString = "Data Source = TESTHPLEL\\SQLEXPRESS; Initial Catalog = MarketPlace; Integrated Security = True; TrustServerCertificate=True";
        private string connectionString = "Data Source = DESKTOP-HS89V7M; Initial Catalog = MarketPlace; Integrated Security = True; TrustServerCertificate=True;";

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
            if (this.sqlConnection.State != System.Data.ConnectionState.Open)
            {
                this.sqlConnection.Open();
            }
        }

        public void CloseConnection()
        {
            if (this.sqlConnection.State == System.Data.ConnectionState.Open)
            {
                this.sqlConnection.Close();
            }
        }

    }
}

