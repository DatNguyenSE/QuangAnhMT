<%@ Page Title="Quản lý nhân viên" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="admin_Default" %>

<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>
<%@ Register Namespace="CKFinder" Assembly="CKFinder" TagPrefix="CKFinder" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <asp:UpdatePanel ID="up_phanquyen" runat="server" UpdateMode="Conditional">
        <%--<Triggers>
         <asp:AsyncPostBackTrigger ControlID="but_show_form_xuat" EventName="Click" />
     </Triggers>--%>
        <ContentTemplate>
            <asp:Panel ID="pn_phanquyen" runat="server" Visible="false" DefaultButton="but_phanquyen">
                <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                    <div style='top: 0; left: 0px; margin: 0 auto; max-width: 1550px; opacity: 1;'>
                        <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                            <a href='#' class='fg-white d-inline' runat="server" id="A1" onserverclick="but_close_form_phanquyen_Click" title='Đóng'>
                                <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                            </a>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                PHÂN QUYỀN NHÂN VIÊN
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>
                <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                    <div style='top: 0; left: 0; margin: 0 auto; max-width: 1556px; opacity: 1;'>
                        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px">
                            <div class="row">
                                <div class="cell-lg-4 pl-2-lg pr-2-lg">
                                    <div class="mt-3">
                                        <div class="mt-1">
                                            <asp:CheckBox ID="check_all_quyen_quanlyhethong" runat="server" CssClass="text-bold" Text="QUẢN LÝ HỆ THỐNG" OnCheckedChanged="check_all_quyen_quanlyhethong_CheckedChanged" AutoPostBack="true" />
                                        </div>
                                        <asp:CheckBoxList ID="check_list_quyen_quanlyhethong" runat="server" AutoPostBack="true" OnSelectedIndexChanged="check_list_quyen_quanlyhethong_SelectedIndexChanged">
                                            <asp:ListItem Text="Cài đặt hệ thống" Value="14" Selected="false"></asp:ListItem>
                                            <asp:ListItem Text="Quản lý dữ liệu nguồn" Value="6" Selected="false"></asp:ListItem>
                                            <asp:ListItem Text="Xem thống kê bán hàng" Value="40" Selected="false"></asp:ListItem>
                                        </asp:CheckBoxList>
                                    </div>
                                    <div class="mt-3">
                                        <div class="mt-1">
                                            <asp:CheckBox ID="check_all_quyen_quanlynhanvien" runat="server" CssClass="text-bold" Text="QUẢN LÝ NHÂN VIÊN" OnCheckedChanged="check_all_quyen_quanlynhanvien_CheckedChanged" AutoPostBack="true" />
                                        </div>
                                        <asp:CheckBoxList ID="check_list_quyen_quanlynhanvien" runat="server" AutoPostBack="true" OnSelectedIndexChanged="check_list_quyen_quanlynhanvien_SelectedIndexChanged">
                                            <asp:ListItem Text="Xem danh sách nhân viên" Value="1" Selected="false"></asp:ListItem>
                                            <asp:ListItem Text="Xem lương cơ bản" Value="2" Selected="false"></asp:ListItem>
                                            <asp:ListItem Text="Thêm nhân viên mới" Value="3" Selected="false"></asp:ListItem>
                                            <asp:ListItem Text="Chỉnh sửa thông tin nhân viên" Value="4" Selected="false"></asp:ListItem>
                                            <asp:ListItem Text="Phân quyền" Value="5" Selected="false"></asp:ListItem>
                                            <asp:ListItem Text="Xem bảng chấm công (toàn bộ)" Value="15" Selected="false"></asp:ListItem>
                                            <asp:ListItem Text="Xem bảng chấm công (riêng tư)" Value="28" Selected="false"></asp:ListItem>
                                        </asp:CheckBoxList>
                                    </div>
                                    <div class="mt-3">
                                        <div class="mt-1">
                                            <asp:CheckBox ID="check_all_quyen_quanlykho" runat="server" CssClass="text-bold" Text="QUẢN LÝ KHO" OnCheckedChanged="check_all_quyen_quanlykho_CheckedChanged" AutoPostBack="true" />
                                        </div>
                                        <asp:CheckBoxList ID="check_list_quyen_quanlykho" runat="server" AutoPostBack="true" OnSelectedIndexChanged="check_list_quyen_quanlykho_SelectedIndexChanged">
                                            <asp:ListItem Text="Xem hàng trong kho" Value="7" Selected="false"></asp:ListItem>
                                            <asp:ListItem Text="Xem giá nhập" Value="8" Selected="false"></asp:ListItem>
                                            <asp:ListItem Text="Thêm sản phẩm mới" Value="9" Selected="false"></asp:ListItem>
                                            <asp:ListItem Text="Chỉnh sửa sản phẩm" Value="10" Selected="false"></asp:ListItem>
                                            <asp:ListItem Text="Xóa sản phẩm" Value="11" Selected="false"></asp:ListItem>
                                            <asp:ListItem Text="Nhập hàng" Value="12" Selected="false"></asp:ListItem>
                                            <asp:ListItem Text="Xem lịch sử nhập xuất" Value="13" Selected="false"></asp:ListItem>
                                        </asp:CheckBoxList>
                                    </div>


                                </div>
                                <div class="cell-lg-4 pl-2-lg pr-2-lg">
                                    <div class="mt-3">
                                        <div class="mt-1">
                                            <asp:CheckBox ID="check_all_quyen_quanlybaogia" runat="server" CssClass="text-bold" Text="QUẢN LÝ BÁO GIÁ" OnCheckedChanged="check_all_quyen_quanlybaogia_CheckedChanged" AutoPostBack="true" />
                                        </div>
                                        <asp:CheckBoxList ID="check_list_quyen_quanlybaogia" runat="server" AutoPostBack="true" OnSelectedIndexChanged="check_list_quyen_quanlybaogia_SelectedIndexChanged">
                                            <asp:ListItem Text="Xem báo giá (toàn bộ)" Value="16" Selected="false"></asp:ListItem>
                                            <asp:ListItem Text="Xem báo giá (riêng tư)" Value="17" Selected="false"></asp:ListItem>
                                            <asp:ListItem Text="Tạo báo giá" Value="29" Selected="false"></asp:ListItem>
                                            <asp:ListItem Text="Sửa - Xóa (toàn bộ)" Value="18" Selected="false"></asp:ListItem>
                                            <asp:ListItem Text="Sửa - Xóa (riêng tư)" Value="19" Selected="false"></asp:ListItem>
                                        </asp:CheckBoxList>
                                    </div>
                                    <div class="mt-3">
                                        <div class="mt-1">
                                            <asp:CheckBox ID="check_all_quyen_datakhachhang" runat="server" CssClass="text-bold" Text="QUẢN LÝ DATA KHÁCH HÀNG" OnCheckedChanged="check_all_quyen_datakhachhang_CheckedChanged" AutoPostBack="true" />
                                        </div>
                                        <asp:CheckBoxList ID="check_list_quyen_datakhachhang" runat="server" AutoPostBack="true" OnSelectedIndexChanged="check_list_quyen_datakhachhang_SelectedIndexChanged">
                                            <asp:ListItem Text="Xem data khách hàng (toàn bộ)" Value="20" Selected="false"></asp:ListItem>
                                            <asp:ListItem Text="Xem data khách hàng (riêng tư)" Value="21" Selected="false"></asp:ListItem>
                                            <asp:ListItem Text="Thêm - Sửa - Xóa (toàn bộ)" Value="22" Selected="false"></asp:ListItem>
                                            <asp:ListItem Text="Thêm - Sửa - Xóa (riêng tư)" Value="23" Selected="false"></asp:ListItem>
                                        </asp:CheckBoxList>
                                    </div>
                                </div>
                                <div class="cell-lg-4 pl-2-lg pr-2-lg">
                                    <div class="mt-3">
                                        <div class="mt-1">
                                            <asp:CheckBox ID="check_all_quyen_quanlyhopdong" runat="server" CssClass="text-bold" Text="BÁN HÀNG" OnCheckedChanged="check_all_quyen_quanlyhopdong_CheckedChanged" AutoPostBack="true" />
                                        </div>
                                        <asp:CheckBoxList ID="check_list_quyen_quanlyhopdong" runat="server" AutoPostBack="true" OnSelectedIndexChanged="check_list_quyen_quanlyhopdong_SelectedIndexChanged">
                                            <asp:ListItem Text="Bán hàng từ báo giá (toàn bộ)" Value="26" Selected="false"></asp:ListItem>
                                            <asp:ListItem Text="Bán hàng từ báo giá (riêng tư)" Value="27" Selected="false"></asp:ListItem>
                                            <asp:ListItem Text="Xem lợi nhuận bán hàng" Value="39" Selected="false"></asp:ListItem>
                                        </asp:CheckBoxList>
                                    </div>
                                    <div class="mt-3">
                                        <div class="mt-1">
                                            <asp:CheckBox ID="check_all_quyen_congviec" runat="server" CssClass="text-bold" Text="CÔNG VIỆC" OnCheckedChanged="check_all_quyen_congviec_CheckedChanged" AutoPostBack="true" />
                                        </div>
                                        <asp:CheckBoxList ID="check_list_quyen_congviec" runat="server" AutoPostBack="true" OnSelectedIndexChanged="check_list_quyen_congviec_SelectedIndexChanged">
                                            <asp:ListItem Text="Xem công việc (toàn bộ)" Value="30" Selected="false"></asp:ListItem>
                                            <asp:ListItem Text="Xem công việc (được giao)" Value="31" Selected="false"></asp:ListItem>
                                            <asp:ListItem Text="Giao việc" Value="32" Selected="false"></asp:ListItem>
                                            <asp:ListItem Text="Chỉnh sửa công việc" Value="33" Selected="false"></asp:ListItem>
                                            <asp:ListItem Text="Xóa công việc" Value="34" Selected="false"></asp:ListItem>
                                        </asp:CheckBoxList>
                                    </div>
                                    <div class="mt-3">
                                        <div class="mt-1">
                                            <asp:CheckBox ID="check_all_quyen_baohanh" runat="server" CssClass="text-bold" Text="HÀNG BẢO HÀNH" OnCheckedChanged="check_all_quyen_baohanh_CheckedChanged" AutoPostBack="true" />
                                        </div>
                                        <asp:CheckBoxList ID="check_list_quyen_baohanh" runat="server" AutoPostBack="true" OnSelectedIndexChanged="check_list_quyen_baohanh_SelectedIndexChanged">
                                            <asp:ListItem Text="Xem hàng bảo hành" Value="35" Selected="false"></asp:ListItem>
                                            <asp:ListItem Text="Thêm hàng bảo hành" Value="36" Selected="false"></asp:ListItem>
                                            <asp:ListItem Text="Sửa hàng bảo hành" Value="37" Selected="false"></asp:ListItem>
                                            <asp:ListItem Text="Xóa hàng bảo hành" Value="38" Selected="false"></asp:ListItem>
                                        </asp:CheckBoxList>
                                    </div>
                                </div>
                            </div>



                            <div class="mt-6 mb-20 text-right">
                                <asp:Button ID="but_phanquyen" runat="server" CssClass="success" Text="Phân quyền" OnClick="but_phanquyen_Click" />
                            </div>
                            <div class="mb-20"></div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress4" runat="server" AssociatedUpdatePanelID="up_phanquyen">
        <ProgressTemplate>
            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                <div style="padding-top: 45vh;">
                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="up_add" runat="server" UpdateMode="Conditional">
        <%--<Triggers>
            <asp:AsyncPostBackTrigger ControlID="but_show_form_add" EventName="Click" />
        </Triggers>--%>
        <ContentTemplate>
            <asp:Panel ID="pn_add" runat="server" Visible="false" DefaultButton="but_add_edit">
                <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                    <div style='top: 0; left: 0px; margin: 0 auto; max-width: 1100px; opacity: 1;'>
                        <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                            <a href='#' class='fg-white d-inline' id="close_add" runat="server" onserverclick="but_close_form_add_Click" title='Đóng'>
                                <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                            </a>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>
                <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                    <div style='top: 0; left: 0; margin: 0 auto; max-width: 1106px; opacity: 1;'>
                        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px">
                            <%--pl-4 pl-8-md pr-8-md pr-4--%>
                            <div class="row">
                                <div class="cell-lg-6 pr-4-lg">
                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Tài khoản</label>
                                        <div>
                                            <asp:TextBox ID="txt_taikhoan" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>
                                    <asp:PlaceHolder ID="PlaceHolder1" runat="server">
                                        <div class="mt-3">
                                            <label class="fw-600 fg-red">Mật khẩu</label>
                                            <div>
                                                <asp:TextBox ID="txt_matkhau" TextMode="Password" runat="server" data-role="input"></asp:TextBox>
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>

                                    <div class="mt-3">
                                        <label class="fw-600">Ảnh đại diện</label>
                                        <input type="file" id="fileInput" onchange="uploadFile()" data-role="file" data-button-title="<span class='mif-file-upload'></span>" />
                                        <div id="message" runat="server"></div>
                                        <div id="uploadedFilePath"></div>
                                        <div style="display: none">
                                            <asp:TextBox ID="txt_link_fileupload" runat="server"></asp:TextBox>
                                        </div>
                                        <div>
                                            <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                                        </div>
                                        <div style='position: absolute; bottom: 0px; left: 100px'>
                                            <asp:Button ID="Button2" runat="server" Text="Xóa ảnh cũ" CssClass="alert small" Visible="false" OnClick="Button2_Click" />
                                        </div>
                                    </div>

                                    <div class="mt-3" style="display:none;">
                                        <label class="fw-600 fg-red">CCCD mặt trước</label>
                                        <input type="file" id="fileInput1" onchange="uploadFile1()" data-role="file" data-button-title="<span class='mif-file-upload'></span>" />
                                        <div id="message1" runat="server"></div>
                                        <div id="uploadedFilePath1"></div>
                                        <div style="display: none">
                                            <asp:TextBox ID="txt_link_fileupload1" runat="server"></asp:TextBox>
                                        </div>
                                        <div>
                                            <asp:Label ID="Label3" runat="server" Text=""></asp:Label>
                                        </div>
                                        <div style='position: absolute; bottom: 0px; left: 100px'>
                                            <asp:Button ID="Button1" runat="server" Text="Xóa ảnh cũ" CssClass="alert small" Visible="false" OnClick="Button1_Click" />
                                        </div>
                                    </div>

                                    <div class="mt-3" style="display:none;">
                                        <label class="fw-600 fg-red">CCCD mặt sau</label>
                                        <input type="file" id="fileInput2" onchange="uploadFile2()" data-role="file" data-button-title="<span class='mif-file-upload'></span>" />
                                        <div id="message2" runat="server"></div>
                                        <div id="uploadedFilePath2"></div>
                                        <div style="display: none">
                                            <asp:TextBox ID="txt_link_fileupload2" runat="server"></asp:TextBox>
                                        </div>
                                        <div>
                                            <asp:Label ID="Label4" runat="server" Text=""></asp:Label>
                                        </div>
                                        <div style='position: absolute; bottom: 0px; left: 100px'>
                                            <asp:Button ID="Button3" runat="server" Text="Xóa ảnh cũ" CssClass="alert small" Visible="false" OnClick="Button3_Click" />
                                        </div>
                                    </div>

                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Số CCCD</label>
                                        <div>
                                            <asp:TextBox ID="txt_so_cccd" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Họ và tên</label>
                                        <div>
                                            <asp:TextBox ID="txt_hoten" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Ngày sinh</label>
                                        <div>
                                            <asp:TextBox ID="txt_ngaysinh" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Điện thoại</label>
                                        <div>
                                            <asp:TextBox ID="txt_dienthoai" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="mt-3">
                                        <label class="fw-600">Tên ngân hàng</label>
                                        <div>
                                            <asp:TextBox ID="txt_tennganhang" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="mt-3">
                                        <label class="fw-600">Số TK ngân hàng</label>
                                        <div>
                                            <asp:TextBox ID="txt_so_tknganhang" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="mt-3">
                                        <label class="fw-600">Tên chủ TK ngân hàng</label>
                                        <div>
                                            <asp:TextBox ID="txt_tenchu_tknganhang" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Ngày vào làm</label>
                                        <div>
                                            <asp:TextBox ID="txt_ngayvaolam" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="mt-3">
                                        <label class="fw-600">SĐT người thân</label>
                                        <div>
                                            <asp:TextBox ID="txt_sdt_nguoithan" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Tên người thân</label>
                                        <div>
                                            <asp:TextBox ID="txt_tennguoithan" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="cell-lg-6 pl-4-lg">
                                    <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible="false">
                                        <div class="mt-3">
                                            <label class="fw-600">Lương cơ bản</label>
                                            <div>
                                                <asp:TextBox ID="txt_luongcoban" runat="server" data-role="input" MaxLength="14" oninput="format_sotien_new(this)"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="mt-3">
                                            <label class="fw-600">Phụ cấp xăng xe</label>
                                            <div>
                                                <asp:TextBox ID="txt_phucap_xangxe" runat="server" data-role="input" MaxLength="14" oninput="format_sotien_new(this)"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="mt-3">
                                            <label class="fw-600">Phụ cấp ăn uống</label>
                                            <div>
                                                <asp:TextBox ID="txt_phucap_anuong" runat="server" data-role="input" MaxLength="14" oninput="format_sotien_new(this)"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="mt-3">
                                            <label class="fw-600">Phụ cấp nhà trọ</label>
                                            <div>
                                                <asp:TextBox ID="txt_phucap_nhatro" runat="server" data-role="input" MaxLength="14" oninput="format_sotien_new(this)"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="mt-3">
                                            <label class="fw-600">Phụ cấp trách nhiệm</label>
                                            <div>
                                                <asp:TextBox ID="txt_phucap_trachniem" runat="server" data-role="input" MaxLength="14" oninput="format_sotien_new(this)"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="mt-3">
                                            <label class="fw-600">Phụ cấp BHYT</label>
                                            <div>
                                                <asp:TextBox ID="txt_phucap_bhyt" runat="server" data-role="input" MaxLength="14" oninput="format_sotien_new(this)"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="mt-3">
                                            <label class="fw-600">% thưởng doanh số bán hàng</label>
                                            <div>
                                                <asp:TextBox ID="txt_phantram_doanhso" runat="server" data-role="input" MaxLength="3" oninput="format_sotien_new(this)"></asp:TextBox>
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                </div>
                            </div>
                            <div class="mt-6 mb-20 text-right">
                                <asp:Button ID="but_add_edit" runat="server" Text="" CssClass="button success" OnClick="but_add_edit_Click" />
                            </div>
                            <div class="mb-20"></div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress5" runat="server" AssociatedUpdatePanelID="up_add">
        <ProgressTemplate>
            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                <div style="padding-top: 45vh;">
                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="up_loc" runat="server" UpdateMode="Conditional">
        <%--<Triggers>
        <asp:AsyncPostBackTrigger ControlID="but_show_form_loc" EventName="Click" />
    </Triggers>--%>
        <ContentTemplate>
            <asp:Panel ID="pn_loc" runat="server" Visible="false" DefaultButton="but_loc">
                <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                    <div style='top: 0; left: 0px; margin: 0 auto; max-width: 900px; opacity: 1;'>
                        <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                            <a href='#' class='fg-white d-inline' runat="server" id="close_loc" onserverclick="but_show_form_loc_Click" title='Đóng'>
                                <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                            </a>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                LỌC DỮ LIỆU
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>
                <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                    <div style='top: 0; left: 0; margin: 0 auto; max-width: 906px; opacity: 1;'>
                        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px">
                            <div class="row">
                                <div class="cell-lg-6 pr-4-lg">
                                    <div class="fw-600 mt-3">Số lượng hiển thị mỗi trang</div>
                                    <asp:TextBox ID="txt_show" MaxLength="7" runat="server" data-role="input" data-clear-button="true"></asp:TextBox>
                                    <div class="mt-3" style="display: none">
                                        <div class="fw-600">Phân loại bài viết</div>
                                        <%--value của ListBox1 k nên đặt unicode để tránh lưu cookie k được--%>
                                        <asp:ListBox ID="ListBox1" runat="server" SelectionMode="Multiple" data-role="select">
                                            <asp:ListItem Value="" Text="Tất cả"></asp:ListItem>
                                            <asp:ListItem Value="TinTuc" Text="Tin tức"></asp:ListItem>
                                            <asp:ListItem Value="SanPham" Text="Sản phẩm"></asp:ListItem>
                                            <asp:ListItem Value="DichVu" Text="Dịch vụ"></asp:ListItem>
                                        </asp:ListBox>
                                    </div>
                                    <%--<div class=" mt-3">
                                    <label class="fw-600">Lọc ra menu con của</label></div>
                                    <asp:ListBox ID="multiSelectList" runat="server" SelectionMode="Multiple" data-role="select">
                                        <asp:ListItem Text="Không chọn" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="Dịch vụ" Value="96"></asp:ListItem>
                                        <asp:ListItem Text="Sản phẩm" Value="97"></asp:ListItem>
                                    </asp:ListBox>--%>
                                </div>
                                <div class="cell-lg-6 pl-4-lg">
                                    <div class="mt-3">
                                        <label class="fw-600 mt-3">Lọc theo thời gian</label>
                                        <asp:DropDownList ID="ddl_thoigian" runat="server" data-role="select">
                                            <asp:ListItem Text="Dựa vào ngày vào làm" Value="1"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600 mt-3">Từ ngày</label>
                                        <asp:TextBox ID="txt_tungay" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                                    </div>
                                    <div class=" mt-3">
                                        <label class="fw-600 mt-3">Đến ngày</label>
                                        <asp:TextBox ID="txt_denngay" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                                    </div>
                                    <div class="mt-1">
                                        <asp:Button ID="but_homqua" runat="server" Text="Hôm qua" Width="92" OnClick="but_homqua_Click" />
                                        <asp:Button ID="but_homnay" runat="server" Text="Hôm nay" Width="92" OnClick="but_homnay_Click" />
                                        <asp:Button ID="but_tuantruoc" runat="server" Text="Tuần trước" Width="92" OnClick="but_tuantruoc_Click" />
                                        <asp:Button ID="but_tuannay" runat="server" Text="Tuần này" Width="92" OnClick="but_tuannay_Click" />
                                        <asp:Button ID="but_thangtruoc" runat="server" Text="Tháng trước" Width="92" OnClick="but_thangtruoc_Click" />
                                        <asp:Button ID="but_thangnay" runat="server" Text="Tháng này" Width="92" OnClick="but_thangnay_Click" />
                                        <asp:Button ID="but_quytruoc" runat="server" Text="Quý trước" Width="92" OnClick="but_quytruoc_Click" />
                                        <asp:Button ID="but_quynay" runat="server" Text="Quý này" Width="92" OnClick="but_quynay_Click" />
                                        <asp:Button ID="but_namtruoc" runat="server" Text="Năm trước" Width="92" OnClick="but_namtruoc_Click" />
                                        <asp:Button ID="but_namnay" runat="server" Text="Năm này" Width="92" OnClick="but_namnay_Click" />
                                    </div>

                                </div>
                            </div>
                            <div class="mt-6 mb-20">
                                <div style="float: left">
                                    <asp:Button ID="but_huy_loc" OnClick="but_huy_loc_Click" runat="server" Text="Đặt lại mặc định" CssClass="button warning small" />
                                </div>
                                <div style="float: right">
                                    <asp:Button ID="but_loc" OnClick="but_loc_Click" runat="server" Text="THỰC HIỆN LỌC" CssClass="button success" />
                                </div>
                                <div style="clear: both"></div>
                            </div>

                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress3" runat="server" AssociatedUpdatePanelID="up_loc">
        <ProgressTemplate>
            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                <div style="padding-top: 45vh;">
                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="up_xuat" runat="server" UpdateMode="Conditional">
        <%--<Triggers>
            <asp:AsyncPostBackTrigger ControlID="but_show_form_xuat" EventName="Click" />
        </Triggers>--%>
        <ContentTemplate>
            <asp:Panel ID="pn_xuat" runat="server" Visible="false" DefaultButton="but_xuat_excel">
                <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                    <div style='top: 0; left: 0px; margin: 0 auto; max-width: 700px; opacity: 1;'>
                        <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                            <a href='#' class='fg-white d-inline' runat="server" id="close_xuat" onserverclick="but_show_form_xuat_Click" title='Đóng'>
                                <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                            </a>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                XUẤT EXCEL
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>
                <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                    <div style='top: 0; left: 0; margin: 0 auto; max-width: 706px; opacity: 1;'>
                        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px">
                            <div class="row">
                                <div class="cell-lg-6 pr-4-lg">
                                    <div class="mt-3">
                                        <div class="fw-600">Chọn mục muốn xuất</div>
                                        <div class="mt-1">
                                            <asp:CheckBox Checked="true" ID="check_all_excel" runat="server" CssClass="text-bold" Text="Tất cả các mục" OnCheckedChanged="check_all_CheckedChanged" AutoPostBack="true" />
                                        </div>
                                        <asp:CheckBoxList ID="check_list_excel" runat="server" AutoPostBack="true" OnSelectedIndexChanged="check_list_excel_SelectedIndexChanged">
                                            <asp:ListItem Text="Tài khoản" Value="taikhoan" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Họ tên" Value="hoten" Selected="true"></asp:ListItem>
                                        </asp:CheckBoxList>
                                    </div>
                                </div>
                                <div class="cell-lg-6 pl-4-lg">
                                    <div class="mt-3">
                                        <div class="fw-600">Chọn trang</div>
                                        <div class="mt-1">
                                            <asp:CheckBox ID="check_all_page" Checked="true" runat="server" CssClass="text-bold" Text="Tất cả các trang" OnCheckedChanged="check_all_page_CheckedChanged" AutoPostBack="true" />
                                        </div>
                                        <asp:CheckBoxList ID="check_list_page" runat="server" AutoPostBack="true" OnSelectedIndexChanged="check_list_page_SelectedIndexChanged"></asp:CheckBoxList>
                                    </div>
                                </div>
                            </div>

                            <div class="cell-12">
                                <div class="mt-3">
                                    <small><b>Lưu ý:</b> Nhấn nút <b class="fg-green">"Xuất Excel"</b> 1 lần và chờ cho đến khi File được tải xuống.</small>
                                </div>
                            </div>

                            <div class="mt-6 mb-20 text-right">
                                <asp:Button ID="but_xuat_excel" runat="server" CssClass="success" Text="Xuất Excel" OnClick="but_xuat_excel_Click" />
                            </div>
                            <div class="mb-20"></div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <%--<asp:UpdateProgress ID="UpdateProgress4" runat="server" AssociatedUpdatePanelID="up_xuat">
        <ProgressTemplate>
            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                <div style="padding-top: 45vh;">
                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>--%>


    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <Triggers>
            <%--<asp:AsyncPostBackTrigger ControlID="but_add" EventName="Click" />--%>
        </Triggers>
        <ContentTemplate>

            <div class="pos-relative pb-11">
                <div id="menutop-tool-bc" style="position: fixed; top: 52px; width: 100%; z-index: 4">
                    <ul class="h-menu bg-white">

                        <li data-role="hint" data-hint-position="top" data-hint-text="Thêm">
                            <asp:LinkButton ID="but_show_form_add" OnClick="but_show_form_add_Click" runat="server"><span class="mif-plus"></span></asp:LinkButton>
                        </li>

                        <li data-role="hint" data-hint-position="top" data-hint-text="Lọc">
                            <asp:LinkButton ID="but_show_form_loc" runat="server" OnClick="but_show_form_loc_Click"><span class="mif-filter"></span></asp:LinkButton>
                        </li>
                        <%--<li data-role="hint" data-hint-position="top" data-hint-text="Xuất excel">
                            <asp:LinkButton ID="but_show_form_xuat" runat="server" OnClick="but_show_form_xuat_Click"><span class="mif-file-excel"></span></asp:LinkButton>
                        </li>--%>


                        <li class="bd-gray border bd-default mt-2 d-block-lg d-none" style="height: 24px"></li>

                        <li class="d-block-lg d-none">
                            <a data-role="hint" data-hint-position="top" data-hint-text="Hiển thị">
                                <small>
                                    <asp:Label ID="lb_show" runat="server" Text=""></asp:Label></small></a>
                        </li>
                        <li class="d-block-lg d-none" data-role="hint" data-hint-position="top" data-hint-text="Lùi">
                            <asp:LinkButton ID="but_quaylai" OnClick="but_quaylai_Click" runat="server"><span class="mif-chevron-left"></span></asp:LinkButton>
                        </li>
                        <li class="d-block-lg d-none" data-role="hint" data-hint-position="top" data-hint-text="Tới">
                            <asp:LinkButton ID="but_xemtiep" OnClick="but_xemtiep_Click" runat="server"><span class="mif-chevron-right"></span></asp:LinkButton>
                        </li>
                    </ul>
                </div>
                <div id="timkiem-fixtop-bc" style="position: fixed; right: 10px; top: 58px; width: 240px; z-index: 4" class="d-none d-block-sm">
                    <asp:TextBox MaxLength="50" data-prepend="<span class='mif mif-search'></span>" ID="txt_timkiem" runat="server" placeholder="Nhập từ khóa" data-role="input" CssClass="input-small" AutoPostBack="true" OnTextChanged="txt_timkiem_TextChanged"></asp:TextBox>
                </div>
            </div>

            <div class="<%--border-top bd-lightGray--%> <%--pt-3 pl-3-lg pl-0 pr-3-lg pr-0 pb-3--%>p-3">
                <div class="d-none-sm d-block">
                    <asp:TextBox MaxLength="50" data-prepend="<span class='mif mif-search'></span>" ID="txt_timkiem1" runat="server" placeholder="Nhập từ khóa" data-role="input" AutoPostBack="true" OnTextChanged="txt_timkiem_TextChanged"></asp:TextBox>
                </div>
                <div class="d-none-lg d-block mb-3 mt-0-lg mt-3">
                    <div class="place-left">
                        <%--<b><%=ViewState["title"] %></b> Nó k kịp lưu vì nó tải trang này trước khi load menu-left--%>
                    </div>
                    <div class="place-right text-right">

                        <small class="pr-1">
                            <asp:Label ID="lb_show_md" runat="server" Text=""></asp:Label></small>
                        <asp:LinkButton data-role="hint" data-hint-position="top" data-hint-text="Lùi" ID="but_quaylai1" OnClick="but_quaylai_Click" runat="server" CssClass="button small light"><span class="mif-chevron-left"></span></asp:LinkButton>
                        <asp:LinkButton data-role="hint" data-hint-position="top" data-hint-text="Tới" ID="but_xemtiep1" OnClick="but_xemtiep_Click" runat="server" CssClass="button small light"><span class="mif-chevron-right"></span></asp:LinkButton>
                    </div>
                    <div class="clr-bc"></div>
                </div>

                <div class="row">
                    <div class="cell-lg-12">
                        <div class="bcorn-fix-title-table-container">
                            <%--style="padding-bottom: 300px"--%>
                            <table class="bcorn-fix-title-table">
                                <thead>
                                    <tr class="">
                                        <th style="width: 1px;">ID</th>
                                        <th style="width: 1px;">
                                            <%--data-role="checkbox" data-style="2"--%>
                                            <input data-role="hint" data-hint-position="top" data-hint-text="Chọn/Bỏ chọn" type="checkbox" onkeypress="if (event.keyCode==13) return false;" onclick="$('.checkbox-table input[type=checkbox]').prop('checked', this.checked)">
                                        </th>

                                        <th class="text-center" style="width: 60px; min-width: 60px;">Ảnh</th>
                                        <th class="text-center" style="min-width: 140px;">Họ tên</th>
                                        <%-- <th class="text-center" style="width: 60px; min-width: 60px;">Ngày sinh</th>--%>
                                        <%--<th class="text-center" style="width: 60px; min-width: 60px;">Điện thoại</th>--%>
                                        <th class="text-center" style="width: 60px; min-width: 60px;">Vào làm</th>
                                        <th class="text-center" style="width: 100px; min-width: 100px; display:none;">CCCD</th>
                                        <th class="text-center" style="width: 100px; min-width: 100px;">Số CCCD</th>
                                        <th class="text-center" style="width: 150px; min-width: 150px;">Thông tin Ngân hàng</th>

                                        <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible="false">
                                            <th class="text-center" style="width: 60px; min-width: 60px;">Lương CB</th>
                                            <th class="text-center" style="width: 150px; min-width: 150px;">Phụ cấp</th>
                                            <th class="text-center" style="width: 120px; min-width: 120px;">TN tháng</th>
                                        </asp:PlaceHolder>
                                        <th class="text-center" style="width: 100px; min-width: 100px;">Người thân</th>
                                        <th class="text-center" style="width: 1px; min-width: 1px;"></th>
                                    </tr>
                                </thead>

                                <tbody>
                                    <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound">
                                        <%--OnItemDataBound="Repeater1_ItemDataBound"--%>
                                        <ItemTemplate>
                                            <span style="display: none">
                                                <asp:Label ID="lbID" runat="server" Text='<%#Eval("taikhoan") %>'></asp:Label>
                                            </span>
                                            <tr>
                                                <td class="text-center">
                                                    <asp:LinkButton CssClass="fg-white" data-role="hint" data-hint-position="top" data-hint-text="Chỉnh sửa" CommandArgument='<%# Eval("taikhoan") %>' OnClick="but_show_chinhsua_Click" ID="but_show_chinhsua" runat="server"> <%#Eval("taikhoan") %></asp:LinkButton>

                                                </td>
                                                <%--<td class="text-center"><%# Container.ItemIndex + 1 %></td>--%>
                                                <td class="checkbox-table text-center">
                                                    <asp:CheckBox ID="checkID" runat="server" onkeypress="if (event.keyCode==13) return false;" />
                                                </td>
                                                <td class="text-center">
                                                    <div data-role="lightbox" class="c-pointer">
                                                        <img src='<%#Eval("anhdaidien") %>' class="img-cover-vuongtron" width="60" height="60" />
                                                    </div>
                                                </td>
                                                <td class="text-left" style="vertical-align: middle">
                                                    <div class="fw-600"><%#Eval("hoten") %></div>
                                                    <div><a title="Gọi" href="tel:<%#Eval("dienthoai") %>"><span class="mif-phone pr-1"></span><%#Eval("dienthoai") %></a></div>
                                                    <div data-role="hint" data-hint-position="top" data-hint-text="Ngày sinh"><span class="mif-calendar pr-1"></span><%#Eval("ngaysinh","{0:dd/MM/yyyy}") %></div>
                                                    <div>
                                                        <asp:PlaceHolder ID="PlaceHolder19" runat="server" Visible='<%#Eval("trangthai_lamviec").ToString()=="Đang làm việc" %>'>
                                                            <div class="button mini success rounded">Đang làm việc</div>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%#Eval("trangthai_lamviec").ToString()=="Đã nghỉ việc" %>'>
                                                            <div class="button mini alert rounded">Đã nghỉ việc</div>
                                                        </asp:PlaceHolder>
                                                    </div>
                                                </td>
                                                <%--<td class="text-center"><%#Eval("ngaysinh","{0:dd/MM/yyyy}") %></td>--%>
                                                <%--<td class="text-center"><a title="Gọi" href="tel:<%#Eval("dienthoai") %>"><%#Eval("dienthoai") %></a></td>--%>
                                                <td class="text-center"><%#Eval("ngayvaolam","{0:dd/MM/yyyy}") %></td>
                                                <td class="text-center" style="display:none;">
                                                    <div data-role="lightbox" class="c-pointer">
                                                        <div class="row">
                                                            <div class="cell-6">
                                                                <img src='<%#Eval("cccd_mattruoc") %>' class="img-cover-vuong" width="50" height="30" />
                                                            </div>
                                                            <div class="cell-6">
                                                                <img src='<%#Eval("cccd_matsau") %>' class="img-cover-vuong" width="50" height="30" />
                                                            </div>
                                                        </div>
                                                    </div>
                                                </td>
                                                <td class="text-center"><%#Eval("so_cccd") %></td>
                                                <td class="text-left" style="line-height: 1.5;">
                                                    <small>Ngân hàng: <b><%#Eval("tennganhang") %></b></small><br />
                                                    <small>Số TK: <b><%#Eval("so_tknganhang") %></b></small><br />
                                                    <small>Chủ TK: <b><%#Eval("tenchu_tknganhang") %></b></small>
                                                </td>

                                                <asp:PlaceHolder ID="lblLuongCoBan" runat="server" Visible="false">
                                                    <td class="text-right">
                                                        <%# Eval("LuongCoBan", "{0:#,##0}") %>
                                                    </td>
                                                    <td class="text-normal">
                                                        <div style="float: left; width"><small>Xăng xe</small></div>
                                                        <div style="float: right"><small><%# Eval("PhuCap_Xangxe", "{0:#,##0}") %></small></div>
                                                        <div style="clear: both"></div>
                                                        <div style="float: left; width"><small>Ăn uống</small></div>
                                                        <div style="float: right"><small><%# Eval("PhuCap_AnUong", "{0:#,##0}") %></small></div>
                                                        <div style="clear: both"></div>
                                                        <div style="float: left; width"><small>Nhà trọ</small></div>
                                                        <div style="float: right"><small><%# Eval("PhuCap_NhaTro", "{0:#,##0}") %></small></div>
                                                        <div style="clear: both"></div>
                                                        <div style="float: left; width"><small>Trách nhiệm</small></div>
                                                        <div style="float: right"><small><%# Eval("PhuCap_TrachNhiem", "{0:#,##0}") %></small></div>
                                                        <div style="clear: both"></div>
                                                        <div style="float: left; width"><small>BHYT (năm)</small></div>
                                                        <div style="float: right"><small><%# Eval("PhuCap_BHYT", "{0:#,##0}") %></small></div>
                                                        <div style="clear: both"></div>

                                                    </td>
                                                    <td class="text-right">
                                                        <b><%# Eval("TongThuNhapThang", "{0:#,##0}") %></b>
                                                        <div><small>+ <%# Eval("phantram_doanhso_banhang") %>% DS bán hàng</small></div>
                                                        <div><small>+ công tác phí</small></div>
                                                    </td>
                                                </asp:PlaceHolder>
                                                <td class="text-center">
                                                    <%#Eval("ten_nguoithan") %>
                                                    <div><a title="Gọi" href="tel:<%#Eval("sdt_nguoithan") %>"><%#Eval("sdt_nguoithan") %></a></div>
                                                </td>
                                                <td style="vertical-align: middle">
                                                    <div class="dropdown-button place-right">
                                                        <button class="button small bg-transparent">
                                                            <span class="mif mif-more-horiz"></span>
                                                        </button>
                                                        <ul class="d-menu place-right" data-role="dropdown">
                                                            <%--<li><a href="#">Chỉnh sửa</a></li>
                                                            <li><a href="#">Đổi mật khẩu</a></li>--%>
                                                            <li>
                                                                <asp:LinkButton ID="but_show_form_phanquyen" OnClick="but_show_form_phanquyen_Click" CommandArgument='<%#Eval("taikhoan") %>' runat="server">Phân quyền</asp:LinkButton></li>
                                                            <li>
                                                                <asp:LinkButton ID="LinkButton1" OnClick="LinkButton1_Click" CommandArgument='<%#Eval("taikhoan") %>' runat="server">Đã nghỉ việc</asp:LinkButton></li>
                                                            <li>
                                                                <asp:LinkButton ID="LinkButton2" OnClick="LinkButton2_Click" CommandArgument='<%#Eval("taikhoan") %>' runat="server">Đang làm việc</asp:LinkButton></li>
                                                            <%-- <li class="divider"></li>
                                                            <li><a href="#">Sao chép thông tin đăng nhập</a></li>
                                                            <li class="divider"></li>
                                                            <li><a href="#">Đang làm việc</a></li>
                                                            <li><a href="#">Đang nghỉ phép</a></li>
                                                            <li><a href="#">Đã nghỉ việc</a></li>--%>
                                                            <%--<li class="divider"></li>--%>
                                                        </ul>
                                                    </div>
                                                </td>

                                                <%-- <td>

                                                <div class="dropdown-button place-right">
                                                    <button class="button small bg-transparent">
                                                        <span class="mif mif-more-horiz"></span>
                                                    </button>
                                                    <ul class="d-menu place-right" data-role="dropdown">
                                                        <li>
                                                            <asp:LinkButton CommandArgument='<%# Eval("taikhoan") %>' ID="LinkButton1" runat="server" Text='Chỉnh sửa' />
                                                        </li>
                                                        <li class="divider"></li>
                                                        <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("bin").ToString()=="False" %>'>
                                                            <li>
                                                                <asp:LinkButton CommandArgument='<%# Eval("taikhoan") %>' ID="LinkButton3" OnClick="but_remove_bin_only_Click" runat="server" Text='Di chuyển vào thùng rác' />
                                                            </li>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%#Eval("bin").ToString()=="True" %>'>
                                                            <li class="divider"></li>
                                                            <li>
                                                                <asp:LinkButton CommandArgument='<%# Eval("taikhoan") %>' ID="LinkButton4" OnClick="but_khoi_phuc_only_Click" runat="server" Text='Khôi phục' />
                                                            </li>
                                                            <li>
                                                                <asp:LinkButton CommandArgument='<%# Eval("taikhoan") %>' ID="LinkButton5" OnClick="but_xoa_vinh_vien_only_Click" runat="server" Text='Xóa vĩnh viễn' CssClass="fg-red" OnClientClick="return confirm('Dữ liệu sẽ không thể khôi phục! Bạn đã chắc chắn chưa?');" />
                                                            </li>
                                                        </asp:PlaceHolder>
                                                    </ul>
                                                </div>
                                            </td>--%>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible="false">
                                        <tr>
                                            <td class=" bg-white"></td>
                                            <td colspan="6" class="text-right text-bold bg-white">TỔNG</td>
                                            <td class="text-right text-bold"><%=ViewState["tongLCB"] %></td>
                                            <td class="text-right text-bold"><%=ViewState["tongPhuCap"] %></td>
                                            <td class="text-right text-bold"><%=ViewState["tongThuNhap"] %></td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                    </asp:PlaceHolder>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>

            </div>
            <%-- <div class="pos-relative">
        <div id="menu-tool-bc" style="position: fixed; bottom: 0px; width: 100%">
            <ul class="h-menu bg-orange fg-white">
                <li>
                    <asp:LinkButton ID="LinkButton2" runat="server"><span class="mif-plus"></span> Thêm</asp:LinkButton>
                </li>
                <li>
                    <asp:LinkButton ID="but_luu" OnClick="but_luu_Click" runat="server"><span class="mif-floppy-disk"></span> Lưu</asp:LinkButton>
                </li>

                <li>
                    <a href="#" class="dropdown-toggle">Products</a>
                    <ul class="d-menu" data-role="dropdown" style="top: auto; bottom: 100%">
                        <li>
                            <a href="#" class="dropdown-toggle">Windows</a>
                            <ul class="d-menu" data-role="dropdown">
                                <li><a href="#">Windows 10</a></li>
                                <li><a href="#">Windows Server</a></li>
                                <li class="divider"></li>
                                <li><a href="#">MS-DOS</a></li>
                            </ul>
                        </li>
                        <li><a href="#">Skype</a></li>
                        <li class="divider"></li>
                        <li><a href="#">Office</a></li>
                    </ul>
                </li>

                <li><a href="#">Thanh công cụ</a></li>
                <li><a href="#">Thanh công cụ</a></li>
            </ul>
        </div>
    </div>--%>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="up_main">
        <ProgressTemplate>
            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                <div style="padding-top: 45vh;">
                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <%--ảnh opengraph của menu--%>
    <script>
        function uploadFile() {
            var fileInput = document.getElementById("fileInput");
            var messageDiv = document.getElementById("message");
            var uploadedFilePathDiv = document.getElementById("uploadedFilePath");

            if (fileInput.files.length > 0) {
                var file = fileInput.files[0];

                // Kiểm tra loại tệp
                var allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".heic"];
                var fileExtension = file.name.substr(file.name.lastIndexOf(".")).toLowerCase();
                if (allowedExtensions.indexOf(fileExtension) === -1) {
                    messageDiv.innerHTML = "Định dạng ảnh không hợp lệ.";
                    return;
                }

                // Kiểm tra kích thước tệp
                var maxFileSize = 10 * 1024 * 1024; // MB
                if (file.size > maxFileSize) {
                    messageDiv.innerHTML = "Vui lòng chọn file có kích thước nhỏ hơn 10 MB.";
                    return;
                }

                var formData = new FormData();
                formData.append("file", file);

                var xhr = new XMLHttpRequest();
                xhr.open("POST", "/uploads/Upload_Handler_Style1.ashx", true);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        //messageDiv.innerHTML = "File uploaded successfully!";
                        uploadedFilePathDiv.innerHTML = "<div><small>Ảnh mới chọn<small></div><img width='100' src='" + xhr.responseText + "' />"; // Hiển thị ảnh
                        document.getElementById('<%= txt_link_fileupload.ClientID %>').value = xhr.responseText;// Hiển thị đường dẫn
                    } else {
                        messageDiv.innerHTML = "Lỗi upload.";
                    }
                };
                xhr.send(formData);
            } else {
                messageDiv.innerHTML = "Vui lòng chọn file.";
            }
        }

        function uploadFile1() {
            var fileInput = document.getElementById("fileInput1");
            var messageDiv = document.getElementById("message1");
            var uploadedFilePathDiv = document.getElementById("uploadedFilePath1");

            if (fileInput.files.length > 0) {
                var file = fileInput.files[0];

                // Kiểm tra loại tệp
                var allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".heic"];
                var fileExtension = file.name.substr(file.name.lastIndexOf(".")).toLowerCase();
                if (allowedExtensions.indexOf(fileExtension) === -1) {
                    messageDiv.innerHTML = "Định dạng ảnh không hợp lệ.";
                    return;
                }

                // Kiểm tra kích thước tệp
                var maxFileSize = 10 * 1024 * 1024; // MB
                if (file.size > maxFileSize) {
                    messageDiv.innerHTML = "Vui lòng chọn file có kích thước nhỏ hơn 10 MB.";
                    return;
                }

                var formData = new FormData();
                formData.append("file", file);

                var xhr = new XMLHttpRequest();
                xhr.open("POST", "/uploads/Upload_Handler_Style1.ashx", true);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        //messageDiv.innerHTML = "File uploaded successfully!";
                        uploadedFilePathDiv.innerHTML = "<div><small>Ảnh mới chọn<small></div><img width='100' src='" + xhr.responseText + "' />"; // Hiển thị ảnh
                        document.getElementById('<%= txt_link_fileupload1.ClientID %>').value = xhr.responseText;// Hiển thị đường dẫn
                    } else {
                        messageDiv.innerHTML = "Lỗi upload.";
                    }
                };
                xhr.send(formData);
            } else {
                messageDiv.innerHTML = "Vui lòng chọn file.";
            }
        }

        function uploadFile2() {
            var fileInput = document.getElementById("fileInput2");
            var messageDiv = document.getElementById("message2");
            var uploadedFilePathDiv = document.getElementById("uploadedFilePath2");

            if (fileInput.files.length > 0) {
                var file = fileInput.files[0];

                // Kiểm tra loại tệp
                var allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".heic"];
                var fileExtension = file.name.substr(file.name.lastIndexOf(".")).toLowerCase();
                if (allowedExtensions.indexOf(fileExtension) === -1) {
                    messageDiv.innerHTML = "Định dạng ảnh không hợp lệ.";
                    return;
                }

                // Kiểm tra kích thước tệp
                var maxFileSize = 10 * 1024 * 1024; // MB
                if (file.size > maxFileSize) {
                    messageDiv.innerHTML = "Vui lòng chọn file có kích thước nhỏ hơn 10 MB.";
                    return;
                }

                var formData = new FormData();
                formData.append("file", file);

                var xhr = new XMLHttpRequest();
                xhr.open("POST", "/uploads/Upload_Handler_Style1.ashx", true);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        //messageDiv.innerHTML = "File uploaded successfully!";
                        uploadedFilePathDiv.innerHTML = "<div><small>Ảnh mới chọn<small></div><img width='100' src='" + xhr.responseText + "' />"; // Hiển thị ảnh
                        document.getElementById('<%= txt_link_fileupload2.ClientID %>').value = xhr.responseText;// Hiển thị đường dẫn
                    } else {
                        messageDiv.innerHTML = "Lỗi upload.";
                    }
                };
                xhr.send(formData);
            } else {
                messageDiv.innerHTML = "Vui lòng chọn file.";
            }
        }
    </script>

</asp:Content>

