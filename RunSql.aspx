<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Configuration" %>
<%
    string connStr = ConfigurationManager.ConnectionStrings["bco86512_quanganh_dbConnectionString"].ConnectionString;
    using (SqlConnection conn = new SqlConnection(connStr))
    {
        conn.Open();
        string sql = @"
ALTER TRIGGER [let99665_thaianaudio].[trg_MuonTra_AutoCapNhatKho] 
ON [let99665_thaianaudio].[PhieuMuonHang_ChiTiet_tb] 
AFTER INSERT, UPDATE, DELETE AS 
BEGIN 
    SET NOCOUNT ON; 
    
    -- Xử lý INSERT 
    IF EXISTS (SELECT * FROM inserted) AND NOT EXISTS (SELECT * FROM deleted) 
    BEGIN 
        UPDATE k 
        SET k.soluong_hientai = k.soluong_hientai - ISNULL(i.SoLuongMuon, 0) 
        FROM [let99665_thaianaudio].[KhoSanPham_tb] k 
        JOIN inserted i ON k.Id = i.id_sanpham; 
        
        IF EXISTS ( SELECT 1 FROM [let99665_thaianaudio].[KhoSanPham_tb] WHERE soluong_hientai < 0 ) 
        BEGIN 
            RAISERROR(N'Trừ kho dẫn đến tồn kho âm!', 16, 1); 
            ROLLBACK TRANSACTION; 
            RETURN; 
        END 
    END 
    
    -- Xử lý DELETE 
    IF EXISTS (SELECT * FROM deleted) AND NOT EXISTS (SELECT * FROM inserted) 
    BEGIN 
        UPDATE k 
        SET k.soluong_hientai = k.soluong_hientai + ISNULL(d.SoLuongMuon, 0) - ISNULL(d.SoLuongTra, 0) 
        FROM [let99665_thaianaudio].[KhoSanPham_tb] k 
        JOIN deleted d ON k.Id = d.id_sanpham; 
    END 
    
    -- Xử lý UPDATE 
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted) 
    BEGIN 
        UPDATE k 
        SET k.soluong_hientai = k.soluong_hientai + ISNULL(d.SoLuongMuon, 0) - ISNULL(i.SoLuongMuon, 0) - ISNULL(d.SoLuongTra, 0) + ISNULL(i.SoLuongTra, 0) 
        FROM [let99665_thaianaudio].[KhoSanPham_tb] k 
        JOIN inserted i ON k.Id = i.id_sanpham 
        JOIN deleted d ON k.Id = d.id_sanpham; 
        
        IF EXISTS ( SELECT 1 FROM [let99665_thaianaudio].[KhoSanPham_tb] WHERE soluong_hientai < 0 ) 
        BEGIN 
            RAISERROR(N'Cập nhật làm tồn kho âm!', 16, 1); 
            ROLLBACK TRANSACTION; 
            RETURN; 
        END 
    END 
END
        ";
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            cmd.ExecuteNonQuery();
            Response.Write("ALTER TRIGGER SUCCESSFUL");
        }
    }
%>
