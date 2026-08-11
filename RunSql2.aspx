<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Configuration" %>
<%
    string connStr = ConfigurationManager.ConnectionStrings["bco86512_quanganh_dbConnectionString"].ConnectionString;
    using (SqlConnection conn = new SqlConnection(connStr))
    {
        conn.Open();
        string sql = @"
            SELECT sm.definition
            FROM sys.sql_modules sm
            JOIN sys.triggers t ON sm.object_id = t.object_id
        ";
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Response.Write("<pre>" + Server.HtmlEncode(reader["definition"].ToString()) + "</pre><hr/>");
                }
            }
        }
    }
%>
