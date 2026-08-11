<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Configuration" %>
<%
    string connStr = ConfigurationManager.ConnectionStrings["bco86512_quanganh_dbConnectionString"].ConnectionString;
    using (SqlConnection conn = new SqlConnection(connStr))
    {
        conn.Open();
        string sql = @"
            SELECT OBJECT_NAME(object_id) AS obj_name
            FROM sys.sql_modules
            WHERE definition LIKE '%bco86512_quanganh_db%'
        ";
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                bool hasRows = false;
                while (reader.Read())
                {
                    hasRows = true;
                    Response.Write("FOUND: " + Server.HtmlEncode(reader["obj_name"].ToString()) + "<br/>");
                }
                if (!hasRows) Response.Write("NO OBJECTS FOUND");
            }
        }
    }
%>
