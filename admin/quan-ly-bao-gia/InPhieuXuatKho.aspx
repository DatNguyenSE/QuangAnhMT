<%@ Page Language="C#" AutoEventWireup="true" CodeFile="InPhieuXuatKho.aspx.cs" Inherits="admin_quan_ly_bao_gia_InPhieuXuatKho" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Xuất Excel phiếu xuất kho</title>
    <style>
        body { margin: 0; background: #eee; color: #111; font: 14px Arial, sans-serif; line-height: 1.5; }
        .error { padding: 32px; background: white; max-width: 700px; margin: 40px auto; }
        h1 { font-size: 24px; }
    </style>
</head>
<body>
    <div class="error">
        <h1>Chưa thể xuất Excel phiếu xuất kho</h1>
        <p><%: ErrorMessage %></p>
        <a href="Default.aspx">Quay lại báo giá</a>
    </div>
</body>
</html>
