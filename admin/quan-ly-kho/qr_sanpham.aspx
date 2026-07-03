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

        <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
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
                    <asp:LinkButton ID="but_show_form_nhaphang" OnClick="but_show_form_nhaphang_Click" runat="server" CssClass="qr-btn qr-btn-nhap"><span class="mif-download"></span> Nhập hàng</asp:LinkButton>
                    <button type="button" class="qr-btn qr-btn-xuat" onclick="alert('Tính năng Xuất Kho đang được phát triển')"><span class="mif-upload"></span> Xuất kho</button>
                </div>
                
                <div class="text-center mt-4 mb-2">
                    <a href="/admin/quan-ly-kho/default.aspx" class="button light"><span class="mif-arrow-left"></span> Quay lại Quản lý kho</a>
                </div>
            </div>
        </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>

        <!-- Modal Nhập hàng -->
        <asp:UpdatePanel ID="up_nhaphang" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Panel ID="pn_nhaphang" runat="server" Visible="false">
                    <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                        <div style='top: 0; left: 0px; margin: 0 auto; max-width: 550px; opacity: 1;'>
                            <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                                <a href='#' class='fg-white d-inline' id="A1" runat="server" onserverclick="but_close_form_nhaphang_Click" title='Đóng'>
                                    <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                                </a>
                            </div>
                            <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                                <div class="pt-4 text-upper text-bold">
                                    NHẬP HÀNG
                                </div>
                                <hr />
                            </div>
                        </div>
                    </div>
                    <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                        <div style='top: 0; left: 0; margin: 0 auto; max-width: 556px; opacity: 1;'>
                            <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px">
                                <div class="row">
                                    <div class="cell-lg-12">
                                        <div class="mt-3">
                                            <label class="fw-600">Tên sản phẩm</label>
                                            <div>
                                                <asp:Label ID="Label3" runat="server" Text=""></asp:Label>
                                            </div>
                                        </div>
                                        <div class="mt-3">
                                            <label class="fw-600">Tồn hiện tại</label>
                                            <div>
                                                <asp:Label ID="Label4" runat="server" Text="0"></asp:Label>
                                                sản phẩm
                                            </div>
                                        </div>
                                        <div class="mt-3">
                                            <label class="fg-red fw-600">Số lượng nhập</label>
                                            <asp:TextBox ID="txt_soluong_nhap" onfocus="AutoSelect(this)" MaxLength="14" oninput="format_sotien_new(this)" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="mt-6 mb-20 text-right">
                                    <asp:Button ID="but_nhaphang" runat="server" Text="Nhập hàng" CssClass="button success" OnClick="but_nhaphang_Click" />
                                </div>
                                <div class="mb-20"></div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="up_nhaphang">
            <ProgressTemplate>
                <div style="position: fixed; width: 100%; height: 100%; background-color: none; top: 0; left: 0; z-index: 1000000!important; background-color: #0000001a">
                    <div style='position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); color: white; opacity: 1;'>
                        <div data-role="activity" data-type="atom" data-style="dark"></div>
                    </div>
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
    </div>
</asp:Content>
