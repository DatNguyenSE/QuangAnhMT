<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Configuration" %>
<%
    string connStr = ConfigurationManager.ConnectionStrings["bco86512_quanganh_dbConnectionString"].ConnectionString;
    using (SqlConnection conn = new SqlConnection(connStr))
    {
        conn.Open();
        string sql = @"
ALTER proc [let99665_thaianaudio].[spLoad_ExportPhieuMuonHang] as 
SELECT NguoiMuon,C.ten AS TenSanPham, Isnull(tenchuongtrinh,'') tenchuongtrinh, NgayMuon,NgayTra, Isnull(SoLuongMuon,0) SoLuongMuon,Isnull(SoLuongTra,0) SoLuongTra 
FROM [let99665_thaianaudio].[PhieuMuonHang_ChiTiet_tb] A 
inner join [let99665_thaianaudio].[PhieuMuonHang_tb] B on A.id_PhieuMuon = B.id 
left join [let99665_thaianaudio].KhoSanPham_tb C on A.id_sanpham = C.id
        ";
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            cmd.ExecuteNonQuery();
            Response.Write("ALTER PROCEDURE SUCCESSFUL");
        }
    }
%>
