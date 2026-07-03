<%@ Page Title="Quản lý kho" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="admin_quan_ly_kho_Default" %>
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <asp:UpdatePanel ID="up_nhaphang" runat="server" UpdateMode="Conditional">

        <ContentTemplate>
            <asp:Panel ID="pn_nhaphang" runat="server" Visible="false" DefaultButton="but_nhaphang">
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
                            <%--pl-4 pl-8-md pr-8-md pr-4--%>
                            <div class="row">
                                <div class="cell-lg-12">
                                    <div class="mt-3">
                                        <label class="fw-600">Tên sản phẩm</label>
                                        <div>
                                            <asp:Label ID="Label3" runat="server" Text="Label"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Tồn hiện tại</label>
                                        <div>
                                            <asp:Label ID="Label4" runat="server" Text="Label"></asp:Label>
                                            sản phẩm
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fg-red fw-600">Số lượng nhập</label>
                                        <asp:TextBox ID="txt_soluong_nhap" onfocus="AutoSelect(this)" MaxLength="14" oninput="format_sotien_new(this)" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                    <%--<div class="mt-3">
                                        <label class="fg-red fw-600">Giá nhập mỗi sản phẩm</label>
                                        <asp:TextBox ID="txt_gianhaphang" onfocus="AutoSelect(this)" MaxLength="14" oninput="format_sotien_new(this)" runat="server" data-role="input"></asp:TextBox>
                                    </div>--%>
                                    <%-- <div class="mt-3">
                                        <label class="fw-600">Tổng tiền nhập hàng</label>
                                        <div class="fg-red text-bold">
                                            <asp:Label ID="Label5" runat="server" Text="Label"></asp:Label>
                                        </div>
                                    </div>--%>
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
            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                <div style="padding-top: 45vh;">
                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="up_add" runat="server" UpdateMode="Conditional">

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
                                        <label class="fg-red fw-600">Số seri</label>
                                        <asp:TextBox ID="txt_so_seri" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fg-red fw-600">Tên sản phẩm</label>
                                        <asp:TextBox ID="txt_name" runat="server" data-role="input" MaxLength="100"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Ảnh sản phẩm</label>
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
                                    <div class="mt-3">
                                        <asp:RadioButton ID="rbCoHoaDon" runat="server" GroupName="HoaDon" Text="Có hóa đơn" Checked="true" />
                                        <asp:RadioButton ID="rbKhongCoHoaDon" runat="server" GroupName="HoaDon" Text="Không có hóa đơn" />
                                    </div>
                                    <div class="mt-3">
                                        <asp:CheckBox runat="server" ID="check_hangthanhly" Text="Hàng thanh lý"></asp:CheckBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Hãng sản phẩm</label>
                                        <div>
                                            <asp:DropDownList ID="DropDownList1" runat="server" data-role="select"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Nhóm sản phẩm</label>
                                        <div>
                                            <asp:DropDownList ID="DropDownList2" runat="server" data-role="select"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Đơn vị tính</label>
                                        <div>
                                            <asp:DropDownList ID="DropDownList3" runat="server" data-role="select"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Model</label>
                                        <asp:TextBox ID="txt_model" runat="server" data-role="input" MaxLength="100"></asp:TextBox>
                                    </div>



                                </div>
                                <div class="cell-lg-6 pl-4-lg">
                                    <div class="mt-3">
                                        <label class="fw-600">Thông số kỹ thuật</label>
                                        <CKEditor:CKEditorControl ID="txt_thongso" runat="server" Height="100px" Width="100%" CustomConfig="/ckeditor/config-basic.js"></CKEditor:CKEditorControl>
                                        <%--<asp:TextBox ID="txt_thongso" data-role="textarea" runat="server" TextMode="MultiLine"></asp:TextBox>--%>
                                    </div>
                                    <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible="false">
                                        <div class="mt-3">
                                            <label class="fw-600">Giá nhập</label>
                                            <asp:TextBox ID="txt_gianhap" onfocus="AutoSelect(this)" MaxLength="14" oninput="format_sotien_new(this)" runat="server" data-role="input" Text="0"></asp:TextBox>
                                        </div>
                                    </asp:PlaceHolder>
                                    <div class="mt-3">
                                        <label class="fw-600">Giá bán</label>
                                        <asp:TextBox ID="txt_giaban" onfocus="AutoSelect(this)" MaxLength="14" oninput="format_sotien_new(this)" runat="server" data-role="input" Text="0"></asp:TextBox>
                                    </div>

                                    <div class="mt-3">
                                        <label class="fw-600">Ghi chú</label>
                                        <asp:TextBox ID="txt_ghichu" runat="server" data-role="input"></asp:TextBox>
                                    </div>
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
                                    <div class="mt-3">
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
                                            <asp:ListItem Text="Dựa vào ngày tạo" Value="1"></asp:ListItem>
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
                            <div class="mb-20">
                                <div class="mt-3">
                                    <div class="fw-600 fg-red"><i>Lọc theo nhu cầu của bạn. Liên hệ: 0842 359 155</i></div>
                                </div>
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
                                            <asp:ListItem Text="ID" Value="id" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Tên sản phẩm" Value="TenSP" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Hàng thanh lý" Value="hangthanhly" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="VAT" Value="cohoadon" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Đơn vị tính" Value="DVT" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Số lượng tồn" Value="soluong_hientai" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Giá bán lẻ" Value="giabanle" Selected="true"></asp:ListItem>

                                            <asp:ListItem Text="Hãng" Value="Hang" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Model" Value="model" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Nhóm" Value="Nhom" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Ghi chú" Value="ghichu" Selected="true"></asp:ListItem>
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

        <ContentTemplate>

            <div class="pos-relative pb-11">
                <div id="menutop-tool-bc" style="position: fixed; top: 52px; width: 100%; z-index: 4">
                    <ul class="h-menu bg-white">

                        <li data-role="hint" data-hint-position="top" data-hint-text="Thêm sản phẩm">
                            <asp:LinkButton ID="but_show_form_add" OnClick="but_show_form_add_Click" runat="server"><span class="mif-plus"></span></asp:LinkButton>
                        </li>
                        <%--<li data-role="hint" data-hint-position="top" data-hint-text="Lưu">
                            <asp:LinkButton ID="but_save" OnClick="but_save_Click" runat="server"><span class="mif-floppy-disk"></span></asp:LinkButton>
                        </li>--%>

                        <li data-role="hint" data-hint-position="top" data-hint-text="Xóa">
                            <asp:LinkButton ID="but_xoa" OnClick="but_xoa_Click" runat="server"><span class="mif-bin"></span></asp:LinkButton>
                        </li>

                        <%--<li data-role="hint" data-hint-position="top" data-hint-text="Lọc">
                            <asp:LinkButton ID="but_show_form_loc" runat="server" OnClick="but_show_form_loc_Click"><span class="mif-filter"></span></asp:LinkButton>
                        </li>--%>
                        <li data-role="hint" data-hint-position="top" data-hint-text="Xuất excel">
                            <asp:LinkButton ID="but_show_form_xuat" runat="server" OnClick="but_show_form_xuat_Click"><span class="mif-file-excel"></span></asp:LinkButton>
                        </li>


                        <li class="bd-gray border bd-default mt-2 d-block-lg d-none" style="height: 24px"></li>

                        <li class="d-block-lg d-none">
                            <a data-role="hint" data-hint-position="top" data-hint-text="Hiển thị">
                                <small>
                                    <asp:Label ID="lb_show" runat="server" Text=""></asp:Label>
                                </small></a>
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
                            <asp:Label ID="lb_show_md" runat="server" Text=""></asp:Label>
                        </small>
                        <asp:LinkButton data-role="hint" data-hint-position="top" data-hint-text="Lùi" ID="but_quaylai1" OnClick="but_quaylai_Click" runat="server" CssClass="button small light"><span class="mif-chevron-left"></span></asp:LinkButton>
                        <asp:LinkButton data-role="hint" data-hint-position="top" data-hint-text="Tới" ID="but_xemtiep1" OnClick="but_xemtiep_Click" runat="server" CssClass="button small light"><span class="mif-chevron-right"></span></asp:LinkButton>
                    </div>
                    <div class="clr-bc"></div>
                </div>
                <div class="row">
                    <div class="cell-lg-12">
                        <div class="bcorn-fix-title-table-container">
                            <table class="bcorn-fix-title-table">
                                <thead>
                                    <tr class="">
                                        <th style="width: 1px;">ID</th>
                                        <th style="width: 1px;">
                                            <%--data-role="checkbox" data-style="2"--%>
                                            <input data-role="hint" data-hint-position="top" data-hint-text="Chọn/Bỏ chọn" type="checkbox" onkeypress="if (event.keyCode==13) return false;" onclick="$('.checkbox-table input[type=checkbox]').prop('checked', this.checked)">
                                        </th>
                                        <th style="width: 50px; min-width: 50px;">Ảnh</th>
                                        <th style="width: 160px; min-width: 160px;">Sản phẩm</th>
                                        <th style="width: 120px; min-width: 120px;">Số seri</th>
                                        <th style="width: 1px; min-width: 1px;">VAT</th>
                                        <th style="width: 1px; min-width: 1px;">ĐVT</th>
                                        
                                        <th style="width: 1px; min-width: 1px;">Tồn</th>
                                        <th style="width: 1px; min-width: 1px;">Giá lẻ</th>
                                        <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible="false">
                                            <th style="width: 1px; min-width: 1px;">Giá nhập</th>
                                        </asp:PlaceHolder>


                                        <th style="width: 1px; min-width: 1px;">Hãng</th>
                                        <th style="width: 1px; min-width: 1px;">Model</th>
                                        <th style="width: 1px; min-width: 1px;">Nhóm</th>


                                        <%--<th style="width: 300px; min-width: 300px;">Thông số</th>--%>

                                        <th style="width: 100px; min-width: 100px;">Ghi chú</th>
                                        <th style="width: 1px; min-width: 1px;"></th>
                                    </tr>
                                </thead>

                                <tbody>
                                    <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound">
                                        <ItemTemplate>
                                            <span style="display: none">
                                                <asp:Label ID="lbID" runat="server" Text='<%#Eval("id") %>'></asp:Label>
                                            </span>
                                            <tr>
                                                <td class="text-center">
                                                    <asp:LinkButton CssClass="fg-white" OnClick="but_show_chinhsua_Click" data-role="hint" data-hint-position="top" data-hint-text="Chỉnh sửa" ID="but_name_1" CommandArgument='<%# Eval("id") %>' runat="server">
                                                        <%#Eval("id") %>
                                                    </asp:LinkButton>
                                                </td>
                                                <%--<td class="text-center"><%# Container.ItemIndex + 1 %></td>--%>
                                                <td class="checkbox-table">
                                                    <%--data-role="checkbox" data-style="2"--%>
                                                    <%--<input type="checkbox" onkeypress="if (event.keyCode==13) return false;" name="check_<%#Eval("id").ToString() %>">--%>
                                                    <asp:CheckBox ID="checkID" runat="server" onkeypress="if (event.keyCode==13) return false;" />
                                                </td>
                                                <td>
                                                    <div data-role="lightbox" class="c-pointer">
                                                        <img src='<%#Eval("anh") %>' class="img-cover-vuong" width="50" height="50" />
                                                    </div>
                                                </td>
                                                <td style="text-align: left!important">
                                                    <asp:LinkButton CssClass="fg-cobalt" OnClick="but_show_chitiet_Click" data-role="hint" data-hint-position="top" data-hint-text="Chi tiết" ID="but_show_chitiet" CommandArgument='<%# Eval("id") %>' runat="server">
    <%#Eval("TenSP") %>
                                                    </asp:LinkButton>

                                                    <div>
                                                        <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%#Eval("hangthanhly").ToString()=="True" %>'>
                                                            <span class="button mini warning rounded">Hàng thanh lý</span>
                                                        </asp:PlaceHolder>
                                                    </div>
                                                </td>
                                                <td><%#Eval("so_seri") %></td>
                                                <td>
                                                    <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("cohoadon").ToString()=="True" %>'>
                                                        <span class="mif mif-checkmark fg-green"></span>
                                                    </asp:PlaceHolder>
                                                </td>
                                                <td><%#Eval("DVT").ToString().ToUpper() %></td>
                                                <td><b><%#Eval("soluong_hientai","{0:#,##0}") %></b></td>
                                                <td class="text-right"><%#Eval("giabanle","{0:#,##0}") %>
                                                    <div><small>x <%#Eval("soluong_hientai") %></small></div>
                                                    <div><small>=<%#Eval("TongBanLe","{0:#,##0}") %></small></div>
                                                </td>

                                                <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible="false">
                                                    <td class="text-right"><%#Eval("gianhap","{0:#,##0}") %>
                                                        <div><small>x <%#Eval("soluong_hientai") %></small></div>
                                                        <div><small>=<%#Eval("TongGiaNhap","{0:#,##0}") %></small></div>
                                                    </td>
                                                </asp:PlaceHolder>

                                                <td><%#Eval("Hang").ToString().ToUpper() %></td>
                                                <td><%#Eval("model").ToString().ToUpper() %></td>
                                                <td><%#Eval("Nhom").ToString().ToUpper() %></td>


                                                <%--<td style="text-align:left!important"><small><%#Eval("thongso_kythuat") %></small></td>--%>


                                                <td><%#Eval("ghichu") %></td>
                                                <td style="vertical-align: middle">
                                                    <div class="dropdown-button place-right">
                                                        <button class="button small bg-transparent">
                                                            <span class="mif mif-more-horiz"></span>
                                                        </button>
                                                        <ul class="d-menu place-right" data-role="dropdown">
                                                            <%--<li><a href="#">Chỉnh sửa</a></li>
             <li><a href="#">Đổi mật khẩu</a></li>--%>
                                                            <li>
                                                                <asp:LinkButton ID="but_show_form_nhaphang" OnClick="but_show_form_nhaphang_Click" CommandArgument='<%#Eval("id") %>' runat="server">Nhập hàng</asp:LinkButton>
                                                            </li>
                                                            <li>
                                                                <a href="javascript:void(0)" onclick="showQRCode('<%#Eval("so_seri") %>')">Mã QR</a>
                                                            </li>
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
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <tr>
                                        <td class=" bg-white"></td>
                                        <td colspan="4" class="text-bold text-right">TỔNG TÀI SẢN</td>
                                        <td class="text-center text-bold"><%=ViewState["tong_ton"] %></td>
                                        <td class="text-right text-bold"><%=ViewState["tong_giale"] %></td>
                                        <td class="text-right text-bold">
                                            <asp:PlaceHolder ID="PlaceHolder6" runat="server" Visible="false">
                                                <%=ViewState["tong_gianhap"] %>
                                            </asp:PlaceHolder>
                                        </td>
                                        <td colspan="7"></td>
                                    </tr>
                                    <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible="false">
                                        <tr>
                                            <td class=" bg-white"></td>
                                            <td colspan="4" class="text-bold text-right">LÃI GỘP (NẾU BÁN HẾT)</td>
                                            <td colspan="3" class="text-right text-bold"><%=ViewState["tong_laigop"] %></td>
                                            <td colspan="7"></td>
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
    </script>

    <!-- Modal QR Code -->
    <div id="qrModal" style="display:none; position: fixed; width: 100%; height: 100%; top: 0; left: 0; z-index: 1050; background: rgba(0,0,0,0.5);">
        <div style="position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); background: #fff; padding: 20px; border-radius: 8px; text-align: center; min-width: 300px; box-shadow: 0 5px 15px rgba(0,0,0,0.3);">
            <h3 class="mt-0 mb-3 text-upper text-bold fg-red">Mã QR Sản Phẩm</h3>
            <img id="qrImage" src="" alt="QR Code" style="width: 200px; height: 200px; display: block; margin: 0 auto; border: 1px solid #ddd; padding: 5px;" />
            <div class="mt-3 text-bold" id="qrSeriLabel"></div>
            <div class="mt-4">
                <button type="button" class="button alert" onclick="document.getElementById('qrModal').style.display='none'">Đóng</button>
            </div>
        </div>
    </div>

    <script>
        function showQRCode(seri) {
            if (!seri) {
                alert('Sản phẩm này chưa có số seri!');
                return;
            }
            var domain = window.location.origin;
            var qrUrl = domain + '/admin/quan-ly-kho/qr_sanpham.aspx?so_seri=' + seri;
            // Dùng api tạo qr
            var qrSrc = 'https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=' + encodeURIComponent(qrUrl);
            
            document.getElementById('qrImage').src = qrSrc;
            document.getElementById('qrSeriLabel').innerText = 'Seri: ' + seri;
            document.getElementById('qrModal').style.display = 'block';
        }
    </script>
</asp:Content>
