<%@ Page Title="Thông tin sản phẩm" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="qr_sanpham.aspx.cs" Inherits="admin_quan_ly_kho_qr_sanpham" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .qr-product-container {
            max-width: 600px;
            margin: 0 auto;
            background: #fff;
            border-radius: 8px;
            box-shadow: 0 4px 15px rgba(0,0,0,0.1);
            overflow: hidden;
            padding-bottom: 20px;
        }
        .qr-header {
            background: #d32f2f;
            color: #fff;
            padding: 15px;
            text-align: center;
            font-size: 1.2rem;
            font-weight: bold;
            text-transform: uppercase;
        }
        .qr-image-wrapper {
            width: 100%;
            height: 250px;
            background: #f5f5f5;
            display: flex;
            align-items: center;
            justify-content: center;
            overflow: hidden;
        }
        .qr-image-wrapper img {
            max-width: 100%;
            max-height: 100%;
            object-fit: cover;
        }
        .qr-details {
            padding: 20px;
        }
        .qr-title {
            font-size: 1.5rem;
            font-weight: 700;
            color: #333;
            margin-bottom: 5px;
        }
        .qr-seri {
            font-size: 0.9rem;
            color: #777;
            margin-bottom: 15px;
            border-bottom: 1px dashed #ddd;
            padding-bottom: 10px;
        }
        .qr-info-row {
            display: flex;
            justify-content: space-between;
            margin-bottom: 10px;
            font-size: 1rem;
        }
        .qr-info-label {
            font-weight: 600;
            color: #555;
        }
        .qr-info-value {
            font-weight: bold;
            color: #000;
            text-align: right;
        }
        .qr-stock {
            background: #e8f5e9;
            color: #2e7d32;
            padding: 10px;
            border-radius: 5px;
            text-align: center;
            font-size: 1.1rem;
            font-weight: bold;
            margin-top: 15px;
            border: 1px solid #c8e6c9;
        }
        .qr-actions {
            display: flex;
            gap: 10px;
            padding: 0 20px;
            margin-top: 20px;
        }
        .qr-btn {
            flex: 1;
            padding: 12px;
            font-size: 1.1rem;
            font-weight: bold;
            text-align: center;
            border-radius: 5px;
            cursor: pointer;
            transition: all 0.2s;
            border: none;
        }
        .qr-btn-nhap {
            background-color: #28a745;
            color: white;
        }
        .qr-btn-nhap:hover { background-color: #218838; }
        .qr-btn-xuat {
            background-color: #dc3545;
            color: white;
        }
        .qr-btn-xuat:hover { background-color: #c82333; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div class="p-3">
        <asp:Panel ID="pnError" runat="server" Visible="false" CssClass="text-center mt-10">
            <h3 class="fg-red">Sản phẩm không tồn tại hoặc lỗi hiển thị!</h3>
            <p>Vui lòng kiểm tra lại mã QR.</p>
        </asp:Panel>

        <asp:Panel ID="pnContent" runat="server">
            <div class="qr-product-container">
                <div class="qr-header">
                    Thông Tin Sản Phẩm
                </div>
                <div class="qr-image-wrapper">
                    <asp:Image ID="imgProduct" runat="server" />
                </div>
                
                <div class="qr-details">
                    <div class="qr-title"><asp:Literal ID="litTenSP" runat="server"></asp:Literal></div>
                    <div class="qr-seri">Seri: <asp:Literal ID="litSeri" runat="server"></asp:Literal></div>

                    <div class="qr-info-row">
                        <div class="qr-info-label">Hãng:</div>
                        <div class="qr-info-value"><asp:Literal ID="litHang" runat="server"></asp:Literal></div>
                    </div>
                    <div class="qr-info-row">
                        <div class="qr-info-label">Nhóm:</div>
                        <div class="qr-info-value"><asp:Literal ID="litNhom" runat="server"></asp:Literal></div>
                    </div>
                    <div class="qr-info-row">
                        <div class="qr-info-label">Model:</div>
                        <div class="qr-info-value"><asp:Literal ID="litModel" runat="server"></asp:Literal></div>
                    </div>
                    <div class="qr-info-row">
                        <div class="qr-info-label">Đơn vị tính:</div>
                        <div class="qr-info-value"><asp:Literal ID="litDVT" runat="server"></asp:Literal></div>
                    </div>
                    
                    <div class="qr-stock">
                        Tồn kho: <asp:Literal ID="litTonKho" runat="server"></asp:Literal>
                    </div>
                </div>

                <div class="qr-actions">
                    <button type="button" class="qr-btn qr-btn-nhap" onclick="alert('Tính năng Nhập Hàng đang được phát triển')"><span class="mif-download"></span> Nhập hàng</button>
                    <button type="button" class="qr-btn qr-btn-xuat" onclick="alert('Tính năng Xuất Kho đang được phát triển')"><span class="mif-upload"></span> Xuất kho</button>
                </div>
                
                <div class="text-center mt-4 mb-2">
                    <a href="/admin/quan-ly-kho/default.aspx" class="button light"><span class="mif-arrow-left"></span> Quay lại Quản lý kho</a>
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>
