<%@ Page Title="" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="ban-hang.aspx.cs" Inherits="admin_thong_ke_ban_hang" ResponseEncoding="utf-8" Culture="vi-VN" UICulture="vi-VN" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <meta charset="utf-8" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" Runat="Server">
    <div class="bg-white p-4">
        <h4 class="mt-0 mb-4">
            <span class="mif-chart-bars"></span>
            THỐNG KÊ BÁN HÀNG
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
            <div class="cell-lg-6 cell-md-4 cell-sm-12 pt-6">
                <asp:Button ID="but_thongke" runat="server" Text="XEM THỐNG KÊ" CssClass="button primary" OnClick="but_thongke_Click" />
                <asp:Button ID="but_reset" runat="server" Text="THÁNG NÀY" CssClass="button secondary" OnClick="but_reset_Click" />
                <asp:Button ID="but_xuat_excel" runat="server" Text="XUẤT EXCEL" CssClass="button success" OnClick="but_xuat_excel_Click" />
            </div>
        </div>

        <div style="display:none">
            <asp:DropDownList ID="ddl_loai_ngay" runat="server">
                <asp:ListItem Text="Ngày bán/ký HĐ" Value="ban" Selected="True"></asp:ListItem>
                <asp:ListItem Text="Ngày báo giá" Value="baogia"></asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList ID="ddl_trangthai" runat="server">
                <asp:ListItem Text="Đã bán" Value="daban" Selected="True"></asp:ListItem>
                <asp:ListItem Text="Tất cả" Value="all"></asp:ListItem>
                <asp:ListItem Text="Chưa bán" Value="chuaban"></asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList ID="ddl_thanhtoan" runat="server">
                <asp:ListItem Text="Tất cả" Value="all" Selected="True"></asp:ListItem>
                <asp:ListItem Text="Đã thanh toán" Value="done"></asp:ListItem>
                <asp:ListItem Text="Còn nợ" Value="debt"></asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList ID="ddl_nhanvien" runat="server"></asp:DropDownList>
            <asp:TextBox ID="txt_timkiem" runat="server"></asp:TextBox>
            <asp:TextBox ID="txt_sanpham" runat="server"></asp:TextBox>
        </div>

        <asp:Panel ID="pn_thongbao" runat="server" Visible="false" CssClass="remark warning mb-4">
            <asp:Label ID="lb_thongbao" runat="server"></asp:Label>
        </asp:Panel>

        <div class="row mb-4">
            <div class="cell-lg-2 cell-md-4 cell-sm-6">
                <div class="remark success">
                    <div>ĐƠN ĐÃ BÁN</div>
                    <h3><asp:Literal ID="ltr_don_ban" runat="server" /></h3>
                    <div style="font-size: 13px; margin-top: 5px;"><asp:Literal ID="ltr_trend_don" runat="server" /></div>
                </div>
            </div>
            <div class="cell-lg-2 cell-md-4 cell-sm-6">
                <div class="remark alert">
                    <div>TỔNG DOANH THU</div>
                    <h3><asp:Literal ID="ltr_doanhthu" runat="server" /></h3>
                    <div style="font-size: 13px; margin-top: 5px;"><asp:Literal ID="ltr_trend_doanhthu" runat="server" /></div>
                </div>
            </div>
            <div class="cell-lg-2 cell-md-4 cell-sm-6">
                <div class="remark info" style="border-left-color: #00bcd4;">
                    <div>DT BẢO HÀNH</div>
                    <h3><asp:Literal ID="ltr_doanhthubaohanh" runat="server" /></h3>
                    <div style="font-size: 13px; margin-top: 5px;"><asp:Literal ID="ltr_trend_baohanh" runat="server" /></div>
                </div>
            </div>
            <div class="cell-lg-2 cell-md-4 cell-sm-6">
                <div class="remark" style="border-left-color: #9c27b0;">
                    <div>LỢI NHUẬN</div>
                    <h3><asp:Literal ID="ltr_loinhuan" runat="server" /></h3>
                    <div style="font-size: 13px; margin-top: 5px;"><asp:Literal ID="ltr_trend_loinhuan" runat="server" /></div>
                </div>
            </div>
            <div class="cell-lg-2 cell-md-4 cell-sm-6">
                <div class="remark primary">
                    <div>ĐÃ THANH TOÁN</div>
                    <h3><asp:Literal ID="ltr_dathanhtoan" runat="server" /></h3>
                    <div style="font-size: 13px; margin-top: 5px;"><asp:Literal ID="ltr_trend_dathanhtoan" runat="server" /></div>
                </div>
            </div>
            <div class="cell-lg-2 cell-md-4 cell-sm-6">
                <div class="remark warning" style="cursor: pointer;" onclick="window.location.href='/admin/thong-ke/cong-no-khach-hang.aspx'" title="Chuyển đến trang Công nợ khách hàng">
                    <div>CÔNG NỢ <span style="font-size: 11px; margin-left: 10px; font-weight: normal; opacity: 0.8;">(bấm vào để xem chi tiết)</span></div>
                    <h3><asp:Literal ID="ltr_congno" runat="server" /></h3>
                    <div style="font-size: 13px; margin-top: 5px;"><asp:Literal ID="ltr_trend_congno" runat="server" /></div>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="cell-lg-6 cell-md-12">
                <div class="panel">
                    <div class="heading bg-cyan fg-white"><span class="title">Thống kê theo nhân viên</span></div>
                    <div class="content p-2">
                        <asp:GridView ID="grv_nhanvien" runat="server" AutoGenerateColumns="false" GridLines="None" CssClass="table striped hovered cell-border compact">
                            <Columns>
                                <asp:BoundField DataField="NhanVien" HeaderText="Nhân viên" />
                                <asp:BoundField DataField="SoDon" HeaderText="Số đơn" ItemStyle-CssClass="text-center" />
                                <asp:BoundField DataField="DoanhThuText" HeaderText="Doanh thu" ItemStyle-CssClass="text-right" />
                                <asp:BoundField DataField="CongNoText" HeaderText="Công nợ" ItemStyle-CssClass="text-right" />
                                <asp:BoundField DataField="ThuongText" HeaderText="Thưởng DS" ItemStyle-CssClass="text-right" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
            <div class="cell-lg-6 cell-md-12">
                <div class="panel">
                    <div class="heading bg-orange fg-white"><span class="title">Thống kê theo hãng</span></div>
                    <div class="content p-2">
                        <asp:GridView ID="grv_hang" runat="server" AutoGenerateColumns="false" GridLines="None" CssClass="table striped hovered cell-border compact">
                            <Columns>
                                <asp:BoundField DataField="TenHang" HeaderText="Hãng" />
                                <asp:BoundField DataField="SoLuong" HeaderText="SL bán" ItemStyle-CssClass="text-center" />
                                <asp:BoundField DataField="DoanhThuText" HeaderText="Doanh thu" ItemStyle-CssClass="text-right" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
            <div class="cell-lg-6 cell-md-12">
                <div class="panel">
                    <div class="heading bg-violet fg-white"><span class="title">Top sản phẩm</span></div>
                    <div class="content p-2">
                        <asp:GridView ID="grv_sanpham" runat="server" AutoGenerateColumns="false" GridLines="None" CssClass="table striped hovered cell-border compact">
                            <Columns>
                                <asp:BoundField DataField="TenSanPham" HeaderText="Sản phẩm" />
                                <asp:BoundField DataField="SoLuong" HeaderText="SL bán" ItemStyle-CssClass="text-center" />
                                <asp:BoundField DataField="DoanhThuText" HeaderText="Doanh thu" ItemStyle-CssClass="text-right" />
                                <asp:BoundField DataField="LoiNhuanText" HeaderText="Lợi nhuận" ItemStyle-CssClass="text-right" />
                                <asp:BoundField DataField="TonKhoText" HeaderText="Tồn kho" ItemStyle-CssClass="text-center text-bold fg-red" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
            <div class="cell-lg-6 cell-md-12">
                <div class="panel">
                    <div class="heading bg-blue fg-white"><span class="title">Top khách hàng</span></div>
                    <div class="content p-2">
                        <asp:GridView ID="grv_khachhang" runat="server" AutoGenerateColumns="false" GridLines="None" CssClass="table striped hovered cell-border compact">
                            <Columns>
                                <asp:BoundField DataField="KhachHang" HeaderText="Khách hàng" />
                                <asp:BoundField DataField="SoDon" HeaderText="Số đơn" ItemStyle-CssClass="text-center" />
                                <asp:BoundField DataField="DoanhThuText" HeaderText="Doanh thu" ItemStyle-CssClass="text-right" />
                                <asp:BoundField DataField="CongNoText" HeaderText="Công nợ" ItemStyle-CssClass="text-right" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="foot" Runat="Server">
</asp:Content>
