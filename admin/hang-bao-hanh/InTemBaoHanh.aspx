<%@ Page Language="C#" AutoEventWireup="true" CodeFile="InTemBaoHanh.aspx.cs" Inherits="admin_hang_bao_hanh_InTemBaoHanh" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>In Tem Bảo Hành</title>
    <style>
        body {
            margin: 0;
            padding: 0;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            font-size: 13px;
            color: #000;
            background-color: #fff;
        }
        .ticket {
            width: 78mm;
            max-width: 100%;
            margin: 0 auto;
            padding: 5px;
        }
        .center {
            text-align: center;
        }
        .bold {
            font-weight: bold;
        }
        .store-name {
            font-size: 16px;
            text-transform: uppercase;
            margin-bottom: 5px;
        }
        .ticket-title {
            font-size: 15px;
            margin-bottom: 10px;
            border-bottom: 1px dashed #000;
            padding-bottom: 5px;
        }
        .info-row {
            margin-bottom: 5px;
            line-height: 1.4;
        }
        .item-list {
            margin-top: 10px;
            border-top: 1px dashed #000;
            padding-top: 10px;
        }
        .item {
            margin-bottom: 8px;
            padding-bottom: 8px;
            border-bottom: 1px dotted #ccc;
        }
        .item:last-child {
            border-bottom: none;
        }
        .item-name {
            font-weight: bold;
            font-size: 14px;
            margin-bottom: 3px;
        }
        .item-detail {
            margin-left: 10px;
        }
        .footer {
            margin-top: 15px;
            text-align: center;
            font-size: 11px;
            border-top: 1px dashed #000;
            padding-top: 5px;
        }
        /* Chỉ hiển thị khung khi xem trên màn hình, khi in sẽ bỏ margin */
        @media print {
            @page {
                margin: 0;
            }
            body {
                margin: 0;
            }
            .ticket {
                width: 100%;
                padding: 2mm;
            }
        }
    </style>
</head>
<body onload="window.print();">
    <form id="form1" runat="server">
        <div class="ticket">
            <div class="center store-name bold">
                QUANG ANH
            </div>
            <div class="center ticket-title bold">
                TEM BIÊN NHẬN BẢO HÀNH
            </div>

            <div class="info-row">
                <span class="bold">Mã phiếu:</span> <asp:Label ID="lblMaPhieu" runat="server" Text=""></asp:Label>
            </div>
            <div class="info-row">
                <span class="bold">Khách hàng:</span> <asp:Label ID="lblKhachHang" runat="server" Text=""></asp:Label>
            </div>
            <div class="info-row">
                <span class="bold">SĐT:</span> <asp:Label ID="lblSDT" runat="server" Text=""></asp:Label>
            </div>
            <div class="info-row">
                <span class="bold">Ngày nhận:</span> <asp:Label ID="lblNgayNhan" runat="server" Text=""></asp:Label>
            </div>

            <div class="item-list">
                <asp:Repeater ID="rptChiTiet" runat="server">
                    <ItemTemplate>
                        <div class="item">
                            <div class="item-name"><%# Eval("ten") %></div>
                            <div class="item-detail">
                                <span class="bold">Seri:</span> <%# Eval("seri") %>
                            </div>
                            <div class="item-detail">
                                <span class="bold">Lỗi/Ghi chú:</span> <%# Eval("ghichu_sanpham") %>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            
            <div class="footer">
                Vui lòng giữ tem này trên thiết bị<br />
                để thuận tiện đối chiếu bảo hành.
            </div>
        </div>
    </form>
</body>
</html>
