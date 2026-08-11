using System;
using System.Data.SqlClient;

class Program
{
    static void Main()
    {
        string connStr = "Data Source=112.78.2.146;Initial Catalog=qua93172_quanganh_db;Persist Security Info=True;User ID=qua93172_quanganh_db;Password=2Zftv32_PVif~vpz;Encrypt=False";
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES", conn);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["TABLE_SCHEMA"]}.{reader["TABLE_NAME"]}");
                }
            }
        }
    }
}
