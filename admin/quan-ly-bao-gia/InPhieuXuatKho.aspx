<%@ Page Language="C#" AutoEventWireup="true" CodeFile="InPhieuXuatKho.aspx.cs" Inherits="admin_quan_ly_bao_gia_InPhieuXuatKho" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Phiếu xuất kho</title>
    <style>
        * { box-sizing: border-box; }
        body { margin: 0; background: #eee; color: #111; font: 14px Arial, sans-serif; line-height: 1.5; }
        .toolbar { max-width: 210mm; margin: 20px auto; padding: 0 12px; }
        button { padding: 10px 18px; cursor: pointer; border: 1px solid #222; background: #222; color: white; font: inherit; }
        .toolbar p { margin: 8px 0; }
        .sheet { width: 210mm; max-width: 100%; min-height: 270mm; padding: 16mm; margin: 0 auto 24px; background: white; }
        h1 { font-size: 25px; text-align: center; margin: 18px 0 4px; }
        .subtitle { text-align: center; margin-bottom: 24px; }
        .info p { margin: 7px 0; overflow-wrap: anywhere; }
        table { width: 100%; border-collapse: collapse; table-layout: fixed; margin-top: 22px; }
        th, td { border: 1px solid #444; padding: 8px 6px; vertical-align: top; overflow-wrap: anywhere; }
        th { font-size: 13px; }
        .center { text-align: center; } .number { text-align: right; }
        .signatures { display: flex; margin-top: 32px; text-align: center; break-inside: avoid; }
        .signature { width: 33.333%; } .signature small { display: block; font-style: italic; }
        .signature .name { margin-top: 80px; }
        .error { padding: 32px; background: white; max-width: 700px; margin: 40px auto; }
        @page { size: A4; margin: 14mm; }
        @media print {
            body { background: white; }
            .toolbar { display: none; }
            .sheet { width: 100%; max-width: none; min-height: 0; padding: 0; margin: 0; }
            thead { display: table-header-group; }
            tr { break-inside: avoid; page-break-inside: avoid; }
        }
    </style>
</head>
<body>
<% if (CanPrint) { %>
    <div class="toolbar">
        <button type="button" onclick="window.print()">In / Lưu PDF</button>
        <p>Phiếu lấy dữ liệu báo giá đã lưu. Nếu vừa chỉnh sửa, hãy lưu báo giá rồi mở lại phiếu.</p>
        <p>In hoặc in lại phiếu không trừ tồn kho.</p>
    </div>
    <main class="sheet">
        <div><strong>Đơn vị: ................................................................................</strong></div>
        <h1>PHIẾU XUẤT KHO</h1>
        <div class="subtitle">Theo báo giá số <strong><%: QuoteNumber %></strong> &middot; Ngày báo giá: <%: QuoteDate %><br />Ngày lập phiếu: <%: PrintedDate %></div>
        <div class="info">
            <p><strong>Khách hàng / Người nhận:</strong> <%: Customer %></p>
            <p><strong>Điện thoại:</strong> <%: Phone %></p>
            <p><strong>Địa chỉ:</strong> <%: Address %></p>
            <p><strong>Lý do xuất:</strong> Giao hàng theo báo giá số <%: QuoteNumber %>.</p>
            <p><strong>Xuất tại kho:</strong> ................................................................................</p>
        </div>
        <table>
            <colgroup><col style="width:7%" /><col style="width:32%" /><col style="width:23%" /><col style="width:9%" /><col style="width:11%" /><col style="width:18%" /></colgroup>
            <thead><tr><th>STT</th><th>Tên hàng hóa</th><th>Số seri</th><th>ĐVT</th><th>Số lượng</th><th>Ghi chú</th></tr></thead>
            <tbody><%= RowsHtml %></tbody>
            <tbody><tr><td colspan="4"><strong>Tổng số lượng</strong></td><td class="number"><strong><%: TotalQuantity %></strong></td><td></td></tr></tbody>
        </table>
        <div class="signatures">
            <div class="signature"><strong>Người lập phiếu</strong><small>(Ký, họ tên)</small><div class="name"><%: PreparedBy %></div></div>
            <div class="signature"><strong>Người nhận hàng</strong><small>(Ký, họ tên)</small></div>
            <div class="signature"><strong>Thủ kho</strong><small>(Ký, họ tên)</small></div>
        </div>
    </main>
<% } else { %>
    <div class="error"><h1>Chưa thể in phiếu xuất kho</h1><p><%: ErrorMessage %></p></div>
<% } %>
</body>
</html>
