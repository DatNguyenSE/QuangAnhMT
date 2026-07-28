<%@ Page Title="" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="cong-no-khach-hang.aspx.cs" Inherits="admin_thong_ke_cong_no_khach_hang" ResponseEncoding="utf-8" Culture="vi-VN" UICulture="vi-VN" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <meta charset="utf-8" />
    <style>
        .table-wrap { overflow-x: auto; }
        .btn-action { margin-right: 5px; }
    </style>
    <script>
        function copyToClipboard(text) {
            var input = document.createElement('textarea');
            input.innerHTML = text;
            document.body.appendChild(input);
            input.select();
            var result = document.execCommand('copy');
            document.body.removeChild(input);
            if (result) {
                Metro.notify.create("Đã copy link thành công!", "Thông báo", { cls: "success" });
            } else {
                Metro.notify.create("Copy thất bại!", "Lỗi", { cls: "alert" });
            }
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" Runat="Server">
    <div class="bg-white p-4">
        <h4 class="mt-0 mb-4">
            <span class="mif-users"></span>
            CÔNG NỢ KHÁCH HÀNG
        </h4>

        <div class="row mb-4">
            <div class="cell-lg-3 cell-md-4 cell-sm-12">
                <label class="fw-600">Từ ngày</label>
                <asp:TextBox ID="txt_tungay" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
            </div>
            <div class="cell-lg-3 cell-md-4 cell-sm-12">
                <label class="fw-600">Đến ngày</label>
                <asp:TextBox ID="txt_denngay" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
            </div>
            <div class="cell-lg-4 cell-md-4 cell-sm-12">
                <label class="fw-600">Tìm kiếm (Tên, SĐT)</label>
                <div class="input">
                    <asp:TextBox ID="txt_timkiem" runat="server" placeholder="Nhập tên, SĐT..."></asp:TextBox>
                </div>
            </div>
            <div class="cell-lg-3 cell-md-12 cell-sm-12 pt-6">
                <asp:Button ID="but_thongke" runat="server" Text="TÌM KIẾM" CssClass="button primary" OnClick="but_thongke_Click" />
                <asp:Button ID="but_reset" runat="server" Text="THÁNG NÀY" CssClass="button secondary" OnClick="but_reset_Click" />
                <asp:Button ID="but_export" runat="server" Text="XUẤT EXCEL" CssClass="button success" OnClick="but_export_Click" />
            </div>
        </div>

        <asp:Panel ID="pn_thongbao" runat="server" Visible="false" CssClass="remark warning mb-4">
            <asp:Label ID="lb_thongbao" runat="server"></asp:Label>
        </asp:Panel>

        <div class="row mb-4">
            <div class="cell-lg-3 cell-md-6 cell-sm-6">
                <div class="remark alert">
                    <div>TỔNG CÔNG NỢ</div>
                    <h3><asp:Literal ID="ltr_tongno" runat="server" /></h3>
                </div>
            </div>
            <div class="cell-lg-3 cell-md-6 cell-sm-6">
                <div class="remark warning">
                    <div>CÔNG NỢ BÁN HÀNG</div>
                    <h3><asp:Literal ID="ltr_nobanhang" runat="server" /></h3>
                </div>
            </div>
            <div class="cell-lg-3 cell-md-6 cell-sm-6">
                <div class="remark info" style="border-left-color: #00bcd4;">
                    <div>CÔNG NỢ BẢO HÀNH</div>
                    <h3><asp:Literal ID="ltr_nobaohanh" runat="server" /></h3>
                </div>
            </div>
            <div class="cell-lg-3 cell-md-6 cell-sm-6">
                <div class="remark success">
                    <div>SỐ KHÁCH ĐANG NỢ</div>
                    <h3><asp:Literal ID="ltr_sokhach" runat="server" /></h3>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="cell-12">
                <div class="panel">
                    <div class="heading bg-dark fg-white"><span class="title">Danh sách khách hàng đang nợ</span></div>
                    <div class="content p-2 table-wrap">
                        <asp:UpdatePanel ID="up_grid" runat="server">
                            <ContentTemplate>
                                <asp:GridView ID="grv_khachhang" runat="server" AutoGenerateColumns="false" GridLines="None" CssClass="table striped hovered cell-border compact" OnRowCommand="grv_khachhang_RowCommand">
                                    <Columns>
                                        <asp:BoundField DataField="TenKhachHang" HeaderText="Tên Khách Hàng" />
                                        <asp:BoundField DataField="SoDienThoai" HeaderText="Số Điện Thoại" />
                                        <asp:BoundField DataField="NoBanHangText" HeaderText="Nợ Mua Hàng" ItemStyle-CssClass="text-right" />
                                        <asp:BoundField DataField="NoBaoHanhText" HeaderText="Nợ Bảo Hành" ItemStyle-CssClass="text-right" />
                                        <asp:BoundField DataField="TongNoText" HeaderText="Tổng Nợ" ItemStyle-CssClass="text-right fw-bold text-danger" />
                                        <asp:TemplateField HeaderText="Hành Động" ItemStyle-CssClass="text-center">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnDetail" runat="server" CssClass="button small info btn-action" CommandName="ViewDetail" CommandArgument='<%# Eval("Token") %>'>
                                                    <span class="mif-eye"></span> Chi tiết
                                                </asp:LinkButton>
                                                <button type="button" class="button small secondary btn-action" onclick="copyToClipboard('<%# Eval("PublicLink") %>')">
                                                    <span class="mif-copy"></span> Copy Link
                                                </button>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Modal Chi tiết -->
    <div class="dialog" data-role="dialog" id="modal_chitiet" data-width="800" data-close-button="true">
        <div class="dialog-title">Chi tiết công nợ khách hàng</div>
        <div class="dialog-content">
            <asp:UpdatePanel ID="up_modal" runat="server">
                <ContentTemplate>
                    <div class="mb-2"><strong>Khách hàng:</strong> <asp:Literal ID="ltr_modal_ten" runat="server"></asp:Literal></div>
                    <div class="mb-4"><strong>Số điện thoại:</strong> <asp:Literal ID="ltr_modal_sdt" runat="server"></asp:Literal></div>
                    
                    <h5>1. Công nợ Mua Hàng</h5>
                    <div class="table-wrap mb-4">
                        <asp:GridView ID="grv_modal_banhang" runat="server" AutoGenerateColumns="false" CssClass="table striped cell-border compact" GridLines="None" ShowHeaderWhenEmpty="true">
                            <Columns>
                                <asp:BoundField DataField="Ngay" HeaderText="Ngày" DataFormatString="{0:dd/MM/yyyy}" />
                                <asp:BoundField DataField="MaDon" HeaderText="Mã đơn" />
                                <asp:BoundField DataField="TongTienText" HeaderText="Tổng tiền" ItemStyle-CssClass="text-right" />
                                <asp:BoundField DataField="CongNoText" HeaderText="Còn nợ" ItemStyle-CssClass="text-right fw-bold text-danger" />
                            </Columns>
                            <EmptyDataTemplate>Không có công nợ mua hàng.</EmptyDataTemplate>
                        </asp:GridView>
                    </div>

                    <h5>2. Công nợ Dịch Vụ / Bảo Hành</h5>
                    <div class="table-wrap">
                        <asp:GridView ID="grv_modal_baohanh" runat="server" AutoGenerateColumns="false" CssClass="table striped cell-border compact" GridLines="None" ShowHeaderWhenEmpty="true">
                            <Columns>
                                <asp:BoundField DataField="Ngay" HeaderText="Ngày trả" DataFormatString="{0:dd/MM/yyyy}" />
                                <asp:BoundField DataField="MaDon" HeaderText="Mã phiếu" />
                                <asp:BoundField DataField="TongTienText" HeaderText="Tổng tiền" ItemStyle-CssClass="text-right" />
                                <asp:BoundField DataField="CongNoText" HeaderText="Còn nợ" ItemStyle-CssClass="text-right fw-bold text-danger" />
                            </Columns>
                            <EmptyDataTemplate>Không có công nợ bảo hành.</EmptyDataTemplate>
                        </asp:GridView>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div class="dialog-actions">
            <button class="button js-dialog-close">Đóng</button>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="foot" Runat="Server">
</asp:Content>
