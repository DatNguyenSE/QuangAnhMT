<%@ Page Title="Quản lý kho" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="admin_quan_ly_kho_Default" %>
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .quick-entry-panel > div:last-of-type {
            background: rgba(20, 28, 38, .72) !important;
            backdrop-filter: blur(5px);
        }

        .quick-entry-panel > div:last-of-type > div {
            max-width: 1080px !important;
            padding: 26px 16px 46px;
        }

        .quick-entry-panel > div:first-of-type > div,
        .quick-entry-panel > div:last-of-type > div {
            width: 100%;
            max-width: 1106px !important;
            box-sizing: border-box;
            margin-left: auto !important;
            margin-right: auto !important;
        }

        .quick-entry-panel > div:last-of-type > div > div {
            border-radius: 18px;
            box-shadow: 0 24px 70px rgba(0, 0, 0, .22);
            overflow: hidden;
        }

        .quick-entry-panel .quick-entry-title {
            color: #1f2937;
            letter-spacing: .03em;
        }

        .quick-entry-panel .quick-entry-subtitle {
            color: #64748b;
            font-size: 13px;
            font-weight: 400;
            letter-spacing: 0;
            text-transform: none;
        }

        .quick-entry-panel .quick-entry-section {
            border: 1px solid #e2e8f0;
            border-radius: 12px;
            background: #f8fafc;
            padding: 14px 16px 18px;
            margin-bottom: 18px;
        }

        .quick-entry-panel .quick-entry-section-title {
            color: #334155;
            font-size: 12px;
            font-weight: 700;
            letter-spacing: .08em;
            margin-bottom: 8px;
            text-transform: uppercase;
        }

        .quick-entry-panel .quick-entry-section-help {
            color: #64748b;
            font-size: 12px;
            margin-bottom: 8px;
        }

        .quick-entry-panel .quick-entry-serial {
            background: #fff7ed;
            border-color: #fdba74;
            color: #9a3412;
            font-family: Consolas, monospace;
            font-weight: 700;
        }

        .dung-chung-text {
            display: none;
        }

        .quick-entry-panel .dung-chung-text {
            display: inline;
        }

        .quick-entry-panel .seri-label-toggle {
            color: #ce352c !important;
        }

        @media (max-width: 639px) {
            .quick-entry-panel > div:last-of-type > div {
                padding: 10px 8px 28px;
            }
        }
    </style>
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

    <asp:UpdatePanel ID="up_chinhsuasoluong" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_chinhsuasoluong" runat="server" Visible="false" DefaultButton="but_luu_chinhsuasoluong">
                <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                    <div style='top: 0; left: 0px; margin: 0 auto; max-width: 550px; opacity: 1;'>
                        <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                            <a href='#' class='fg-white d-inline' id="A_close_chinhsuasoluong" runat="server" onserverclick="but_close_form_chinhsuasoluong_Click" title='Đóng'>
                                <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                            </a>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                CHỈNH SỬA SỐ LƯỢNG
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
                                            <asp:Label ID="Label_ten_chinhsuasoluong" runat="server" Text=""></asp:Label>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fg-red fw-600">Số lượng tồn kho</label>
                                        <asp:TextBox ID="txt_chinhsuasoluong" onfocus="AutoSelect(this)" MaxLength="14" oninput="format_sotien_new(this)" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="mt-6 mb-20 text-right">
                                <asp:Button ID="but_luu_chinhsuasoluong" runat="server" Text="Lưu thay đổi" CssClass="button success" OnClick="but_luu_chinhsuasoluong_Click" />
                            </div>
                            <div class="mb-20"></div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress_chinhsuasoluong" runat="server" AssociatedUpdatePanelID="up_chinhsuasoluong">
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
            <asp:TextBox ID="txt_quick_barcode" runat="server" style="display:none" />
            <asp:Button ID="but_open_quick_entry" runat="server" Text="" style="display:none" OnClick="but_open_quick_entry_Click" CausesValidation="false" />
            <asp:Panel ID="pn_add" runat="server" Visible="false" DefaultButton="but_add_edit">
                <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                    <div style='top: 0; left: 0; margin: 0 auto; max-width: 1106px; opacity: 1;'>
                        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 20px">
                            <asp:Label ID="Label1" runat="server" style="display:none"></asp:Label>
                            <%--pl-4 pl-8-md pr-8-md pr-4--%>
                            <div class="row">
                                <div class="cell-lg-6 pr-4-lg">
                                    <asp:PlaceHolder ID="ph_quick_entry_info" runat="server" Visible="false">
                                        <div class="quick-entry-section">
                                            <div class="quick-entry-section-title">Thông tin từ barcode</div>
                                            <div class="quick-entry-section-help">Seri gốc được lấy trực tiếp từ máy quét.</div>
                                        </div>
                                    </asp:PlaceHolder>
                                    <div class="mt-3">
                                        <label class="fw-600 seri-label-toggle">Số seri</label>
                                        <asp:TextBox ID="txt_so_seri" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                    <asp:PlaceHolder ID="ph_quick_entry_quantity" runat="server" Visible="false">
                                        <div class="quick-entry-section mt-4">
                                            <div class="quick-entry-section-title">Số lượng sản phẩm muốn tạo</div>
                                            <div class="quick-entry-section-help">Mỗi sản phẩm sẽ tạo một record riêng với số lượng tồn là 1.</div>
                                            <label class="fg-red fw-600">Số lượng</label>
                                            <asp:TextBox ID="txt_quick_quantity" runat="server" Text="1" MaxLength="5" TextMode="Number" data-role="input"></asp:TextBox>
                                            <div id="quickSerialPreview" class="mt-3" style="display:none"></div>
                                            <div class="mt-3">
                                                <label class="fw-600">Ngày nhập</label>
                                                <asp:TextBox ID="txt_quick_date" runat="server" TextMode="Date" data-role="input"></asp:TextBox>
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="ph_quick_entry_common_start" runat="server" Visible="false">
                                        <div class="quick-entry-section-title">Thông tin dùng chung cho tất cả sản phẩm</div>
                                        <div class="quick-entry-section-help">Các trường bên dưới sẽ được copy vào từng record được tạo.</div>
                                    </asp:PlaceHolder>
                                    <div class="mt-3">
                                        <label class="fg-red fw-600">Tên sản phẩm <small class="dung-chung-text">(dùng chung)</small></label>
                                        <asp:TextBox ID="txt_name" runat="server" data-role="input" MaxLength="100"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Ảnh sản phẩm <small class="dung-chung-text">(dùng chung)</small></label>
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
                                        <label class="fw-600">Hãng sản phẩm <small class="dung-chung-text">(dùng chung)</small></label>
                                        <div>
                                            <asp:DropDownList ID="DropDownList1" runat="server" data-role="select"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Nhóm sản phẩm <small class="dung-chung-text">(dùng chung)</small></label>
                                        <div>
                                            <asp:DropDownList ID="DropDownList2" runat="server" data-role="select"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Đơn vị tính <small class="dung-chung-text">(dùng chung)</small></label>
                                        <div>
                                            <asp:DropDownList ID="DropDownList3" runat="server" data-role="select"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Model <small class="dung-chung-text">(dùng chung)</small></label>
                                        <asp:TextBox ID="txt_model" runat="server" data-role="input" MaxLength="100"></asp:TextBox>
                                    </div>



                                </div>
                                <div class="cell-lg-6 pl-4-lg">
                                    <div class="mt-3">
                                         <label class="fw-600">Thông số kỹ thuật <small class="dung-chung-text">(dùng chung)</small></label>
                                        <CKEditor:CKEditorControl ID="txt_thongso" runat="server" Height="100px" Width="100%" CustomConfig="/ckeditor/config-basic.js"></CKEditor:CKEditorControl>
                                        <%--<asp:TextBox ID="txt_thongso" data-role="textarea" runat="server" TextMode="MultiLine"></asp:TextBox>--%>
                                    </div>
                                    <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible="false">
                                        <div class="mt-3">
                                             <label class="fw-600">Giá nhập <small class="dung-chung-text">(dùng chung)</small></label>
                                            <asp:TextBox ID="txt_gianhap" onfocus="AutoSelect(this)" MaxLength="14" oninput="format_sotien_new(this)" runat="server" data-role="input" Text="0"></asp:TextBox>
                                        </div>
                                    </asp:PlaceHolder>
                                    <div class="mt-3">
                                         <label class="fw-600">Giá bán <small class="dung-chung-text">(dùng chung)</small></label>
                                        <asp:TextBox ID="txt_giaban" onfocus="AutoSelect(this)" MaxLength="14" oninput="format_sotien_new(this)" runat="server" data-role="input" Text="0"></asp:TextBox>
                                    </div>

                                    <div class="mt-3">
                                         <label class="fw-600">Ghi chú <small class="dung-chung-text">(dùng chung)</small></label>
                                        <asp:TextBox ID="txt_ghichu" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="mt-6 mb-20 text-right">
                                <asp:Button ID="but_add_edit" runat="server" Text="" CssClass="button success" OnClick="but_add_edit_Click" />
                                <a href="#" class="button alert ml-2" id="close_add" runat="server" onserverclick="but_close_form_add_Click" title="Đóng">
                                    <span class="mif mif-cross"></span> Đóng
                                </a>
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

    <asp:UpdatePanel ID="up_import_excel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_import_excel" runat="server" Visible="false">
                <div style="position: fixed; width: 100%; height: 52px; top: 0; left: 0; z-index: 1041!important;">
                    <div style="top: 0; left: 0; margin: 0 auto; max-width: 650px;">
                        <div style="position: absolute; right: 18px; top: 14px; z-index: 1040!important;">
                            <a href="#" class="fg-white d-inline" runat="server" id="close_import_excel" onserverclick="but_close_import_excel_Click" title="Đóng">
                                <span class="mif mif-cross mif-2x fg-red fg-lightRed-hover"></span>
                            </a>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">NHẬP SẢN PHẨM TỪ EXCEL</div>
                            <hr />
                        </div>
                    </div>
                </div>
                <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                    <div style="top: 0; left: 0; margin: 0 auto; max-width: 656px;">
                        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px;">
                            <div class="mt-3">
                                <label class="fw-600 fg-red">File Excel</label>
                                <asp:FileUpload ID="fu_import_excel" runat="server" CssClass="mt-1" accept=".xlsx,.xlsm" />
                                <small class="fg-gray">Dữ liệu bắt đầu từ dòng 4: cột C = tên, D = Cái/Cặp/Bộ, E = số lượng.</small>
                            </div>
                            <div class="mt-6 mb-20 text-right">
                                <asp:Button ID="but_confirm_import_excel" runat="server" Text="XÁC NHẬN NHẬP" CssClass="button success" OnClick="but_confirm_import_excel_Click" OnClientClick="return showImportExcelLoading(this);" />
                            </div>
                            <div id="importExcelLoading" class="bg-dark fixed-top h-100 w-100" style="display:none; opacity:0.9; z-index:99999!important;">
                                <div style="padding-top:45vh; text-align:center; color:#fff;">
                                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true">
                                        <span class="electron"></span><span class="electron"></span><span class="electron"></span>
                                    </div>
                                    <div class="mt-3">Đang nhập dữ liệu, vui lòng chờ...</div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="but_confirm_import_excel" />
        </Triggers>
    </asp:UpdatePanel>
    <script type="text/javascript">
        function showImportExcelLoading(button) {
            var input = document.getElementById('<%= fu_import_excel.ClientID %>');
            if (!input || !input.value) {
                alert('Vui lòng chọn file Excel.');
                return false;
            }
            var loading = document.getElementById('importExcelLoading');
            if (loading) loading.style.display = 'block';
            if (button) button.disabled = true;

            // FileUpload cần full postback. Không dùng __doPostBack vì
            // PageRequestManager sẽ biến request thành AJAX và làm mất stream log.
            var form = button && button.form ? button.form : document.forms[0];
            var eventTarget = document.getElementById('__EVENTTARGET');
            var eventArgument = document.getElementById('__EVENTARGUMENT');
            if (form && eventTarget && eventArgument) {
                eventTarget.value = button.name;
                eventArgument.value = '';
                window.setTimeout(function () { form.submit(); }, 0);
                return false;
            }

            return true;
        }
    </script>



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
                        <li data-role="hint" data-hint-position="top" data-hint-text="Quét barcode bằng camera">
                            <a href="javascript:openCameraScanner();"><span class="mif-camera"></span></a>
                        </li>

                        <%--<li data-role="hint" data-hint-position="top" data-hint-text="Lọc">
                            <asp:LinkButton ID="but_show_form_loc" runat="server" OnClick="but_show_form_loc_Click"><span class="mif-filter"></span></asp:LinkButton>
                        </li>--%>
                        <li data-role="hint" data-hint-position="top" data-hint-text="Xuất excel">
                            <asp:LinkButton ID="but_show_form_xuat" runat="server" OnClick="but_show_form_xuat_Click"><span class="mif-file-excel"></span></asp:LinkButton>
                        </li>
                        <li data-role="hint" data-hint-position="top" data-hint-text="Xem sản phẩm đã bán">
                            <asp:LinkButton ID="but_toggle_sold_products" runat="server" OnClick="but_toggle_sold_products_Click">
                                <span class="mif-checkmark"></span>
                                <asp:Label ID="lbl_toggle_sold_products" runat="server" Text="Xem sản phẩm đã bán"></asp:Label>
                            </asp:LinkButton>
                        </li>
                        <li data-role="hint" data-hint-position="top" data-hint-text="Nhập sản phẩm từ Excel">
                            <asp:LinkButton ID="but_show_import_excel" runat="server" Visible="false" OnClick="but_show_import_excel_Click"><span class="mif mif-file-excel"></span><span class="mif-plus"></span></asp:LinkButton>
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
                    <asp:TextBox MaxLength="50" data-prepend="<span class='mif mif-search'></span>" ID="txt_timkiem" runat="server" placeholder="Tìm tên sản phẩm, số seri..." data-role="input" CssClass="input-small" AutoPostBack="true" OnTextChanged="txt_timkiem_TextChanged"></asp:TextBox>
                </div>
            </div>

            <div class="<%--border-top bd-lightGray--%> <%--pt-3 pl-3-lg pl-0 pr-3-lg pr-0 pb-3--%>p-3">
                <div class="d-none-sm d-block">
                        <asp:TextBox MaxLength="50" data-prepend="<span class='mif mif-search'></span>" ID="txt_timkiem1" runat="server" placeholder="Tìm tên sản phẩm, số seri..." data-role="input" AutoPostBack="true" OnTextChanged="txt_timkiem_TextChanged"></asp:TextBox>
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
                        <a data-role="hint" data-hint-position="top" data-hint-text="Quét barcode bằng camera" href="javascript:openCameraScanner();" class="button small light"><span class="mif-camera"></span></a>
                    </div>
                    <div class="clr-bc"></div>
                </div>
                <div class="row">
                    <div class="cell-lg-12">
                        <div class="bcorn-fix-title-table-container">
                            <table class="bcorn-fix-title-table">
                                <thead>
                                    <tr class="">
                                        <%--<th style="width: 1px;">ID</th>--%>
                                        <th style="width: 1px;">Số Seri</th>
                                        <th style="width: 1px;">
                                            <%--data-role="checkbox" data-style="2"--%>
                                            <input data-role="hint" data-hint-position="top" data-hint-text="Chọn/Bỏ chọn" type="checkbox" onkeypress="if (event.keyCode==13) return false;" onclick="$('.checkbox-table input[type=checkbox]').prop('checked', this.checked)">
                                        </th>
                                        <th style="width: 50px; min-width: 50px;">Ảnh</th>
                                        <th style="width: 50px; min-width: 50px;">Mã QR</th>
                                        <th style="width: 160px; min-width: 160px;">Sản phẩm</th>
                                      <%--  <th style="width: 120px; min-width: 120px;">Số seri</th>--%>
                                        <th style="width: 1px; min-width: 1px;">VAT</th>
                                        <th style="width: 180px; min-width: 180px; white-space: nowrap;">Tồn , tổng tồn: <%=ViewState["tong_ton"] %></th>
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
                                                <%--<td class="text-center">
                                                    <asp:LinkButton CssClass="fg-white" OnClick="but_show_chinhsua_Click" data-role="hint" data-hint-position="top" data-hint-text="Chỉnh sửa" ID="but_name_1" CommandArgument='<%# Eval("id") %>' runat="server">
                                                        <%#Eval("id") %>
                                                    </asp:LinkButton>
                                                </td>--%>
                                                <td class="text-center" style="cursor: pointer;" onclick="var sp = this.querySelector('.seri-span'); var inp = this.querySelector('.seri-input'); if(sp) { sp.style.display = 'none'; inp.style.display = 'inline-block'; inp.focus(); }">
                                                    <span class="seri-span" style="display: inline-block; min-width: 50px; min-height: 20px;"><%# string.IsNullOrEmpty(Convert.ToString(Eval("so_seri"))) ? "<i style='color:#ccc'>Click để nhập</i>" : Convert.ToString(Eval("so_seri")) %></span>
                                                    <asp:TextBox ID="txt_so_seri" runat="server" Text='<%#Eval("so_seri") %>' CssClass="input-small seri-input" style="display:none;" onblur="this.style.display='none'; var sp = this.parentElement.querySelector('.seri-span'); if(sp) sp.style.display='inline-block';" onkeydown="if(event.keyCode==13){ if(confirm('Bạn có muốn chỉnh số seri này không?')){ __doPostBack(this.name, ''); return false; } else { return false; } }" OnTextChanged="txt_so_seri_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                </td>
                                                <%--<td class="text-center"><%# Container.ItemIndex + 1 %></td>--%>
                                                <td class="checkbox-table">
                                                    <%--data-role="checkbox" data-style="2"--%>
                                                    <%--<input type="checkbox" onkeypress="if (event.keyCode==13) return false;" name="check_<%#Eval("id").ToString() %>">--%>
                                                    <asp:CheckBox ID="checkID" runat="server" onkeypress="if (event.keyCode==13) return false;" />
                                                </td>
                                                <td>
                                                    <div data-role="lightbox" class="c-pointer">
                                                        <img loading='lazy' decoding='async' src='<%#Eval("anh") %>' class="img-cover-vuong" width="50" height="50" />
                                                    </div>
                                                </td>
                                                <td>
                                                    <div data-role="lightbox" class="c-pointer">
                                                        <img src='<%# "https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=" + HttpUtility.UrlEncode(Request.Url.GetLeftPart(UriPartial.Authority) + "/admin/quan-ly-kho/qr_sanpham.aspx?id=" + Eval("id")) %>' class="img-cover-vuong" width="50" height="50" />
                                                    </div>
                                                </td>
                                                <td style="text-align: left!important">
                                                    <asp:LinkButton CssClass="fg-cobalt" OnClick="but_show_chinhsua_Click" data-role="hint" data-hint-position="top" data-hint-text="Chỉnh sửa" ID="but_show_chitiet" CommandArgument='<%# Eval("id") %>' runat="server">
    <%#Eval("TenSP") %>
                                                    </asp:LinkButton>

                                                    <div>
                                                        <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%# Convert.ToBoolean(Eval("hangthanhly")) %>'>
                                                            <span class="button mini warning rounded">Hàng thanh lý</span>
                                                        </asp:PlaceHolder>
                                                    </div>
                                                </td>
                                                
                                                <td>
                                                    <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%# Convert.ToBoolean(Eval("cohoadon")) %>'>
                                                        <span class="mif mif-checkmark fg-green"></span>
                                                    </asp:PlaceHolder>
                                                </td>
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

                                                <td><%#Convert.ToString(Eval("Hang")).ToUpper() %></td>
                                                <td><%#Convert.ToString(Eval("model")).ToUpper() %></td>
                                                <td><%#Convert.ToString(Eval("Nhom")).ToUpper() %></td>


                                                <%--<td style="text-align:left!important"><small><%#Eval("thongso_kythuat") %></small></td>--%>


                                                <td><%#Eval("ghichu") %></td>
                                                <td style="vertical-align: middle">
                                                    <div class="dropdown-button place-right">
                                                        <button class="button small bg-transparent">
                                                            <span class="mif mif-more-horiz"></span>
                                                        </button>
                                                        <ul class="d-menu place-right" data-role="dropdown">
                                                            <%--<li><a href="#">Chỉnh sửa</a></li>
             <li><a href="#">Đổi mật khẩu</a></li>--%><li>
    <asp:LinkButton ID="LinkButton2" OnClick="but_show_chinhsua_Click" CommandArgument='<%#Eval("id") %>' runat="server">Chỉnh sửa</asp:LinkButton>
</li>
<li>
    <asp:LinkButton ID="but_sao_chep" OnClientClick="return confirm('Bạn có chắc chắn muốn sao chép sản phẩm này?');" OnClick="but_sao_chep_Click" CommandArgument='<%#Eval("id") %>' runat="server">Sao chép</asp:LinkButton>
</li>
                                                            
                                                             <li>
                                                                 <asp:LinkButton ID="but_show_form_nhaphang" OnClick="but_show_form_nhaphang_Click" CommandArgument='<%#Eval("id") %>' runat="server">Nhập hàng</asp:LinkButton>
                                                             </li>
                                                             <li>
                                                                 <asp:LinkButton ID="but_show_form_chinhsuasoluong" OnClick="but_show_form_chinhsuasoluong_Click" CommandArgument='<%#Eval("id") %>' runat="server">Chỉnh sửa số lượng</asp:LinkButton>
                                                             </li>
                                                             <li>
                                                                 <a href='<%# ResolveUrl("~/admin/quan-ly-bao-gia/Default.aspx?tao-bao-gia=" + Eval("id")) %>'>Tạo báo giá cho sản phẩm này</a>
                                                             </li>
                                                             <li>
                                                                 <a href="javascript:void(0)" onclick="showQRCode('<%#Eval("id") %>', '<%#Eval("so_seri") %>')">Mã QR</a>
                                                             </li>
                                                            <li>
                                                                <asp:LinkButton ID="but_xoa_item" OnClientClick="return confirm('Bạn có chắc chắn muốn xóa sản phẩm này?');" OnClick="but_xoa_item_Click" CommandArgument='<%#Eval("id") %>' runat="server" CssClass="fg-red">Xóa</asp:LinkButton>
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
                                        <td colspan="6" class="text-bold text-right">TỔNG TÀI SẢN</td>
                                        <td class="text-center text-bold"><%=ViewState["tong_ton"] %></td>
                                        <td class="text-right text-bold"><%=ViewState["tong_giale"] %></td>
                                        <td class="text-right text-bold">
                                            <asp:PlaceHolder ID="PlaceHolder6" runat="server" Visible="false">
                                                <%=ViewState["tong_gianhap"] %>
                                            </asp:PlaceHolder>
                                        </td>
                                        <td colspan="5"></td>
                                    </tr>
                                    <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible="false">
                                        <tr>
                                            <td class=" bg-white"></td>
                                            <td colspan="6" class="text-bold text-right">LÃI GỘP (NẾU BÁN HẾT)</td>
                                            <td colspan="3" class="text-right text-bold"><%=ViewState["tong_laigop"] %></td>
                                            <td colspan="5"></td>
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

    <div id="cameraBarcodeModal" style="display:none; position:fixed; inset:0; z-index:1060; background:rgba(15,23,42,.78);">
        <div style="position:absolute; top:50%; left:50%; transform:translate(-50%,-50%); width:min(92vw,520px); background:#fff; border-radius:16px; padding:20px; box-shadow:0 24px 70px rgba(0,0,0,.3);">
            <div class="d-flex flex-justify-between flex-align-center mb-3">
                <div class="text-bold text-upper">Quét barcode bằng camera</div>
                <button type="button" class="button alert small" onclick="closeCameraScanner()"><span class="mif-cross"></span></button>
            </div>
            <div style="position:relative; overflow:hidden; background:#0f172a; border-radius:10px; aspect-ratio:4/3;">
                <video id="cameraBarcodeVideo" autoplay muted playsinline style="width:100%; height:100%; object-fit:cover;"></video>
                <div style="position:absolute; left:12%; right:12%; top:35%; height:30%; border:2px solid #22c55e; border-radius:8px; box-shadow:0 0 0 999px rgba(15,23,42,.2);"></div>
            </div>
            <div id="cameraBarcodeStatus" class="mt-3 text-muted">Đưa một mã vạch vào gần khung xanh, để phần vạch chiếm phần lớn khung hình.</div>
        </div>
    </div>

    <script>
        function showQRCode(id, seri) {
            if (!id) {
                alert('Không lấy được ID sản phẩm!');
                return;
            }
            var domain = window.location.origin;
            var qrUrl = domain + '/admin/quan-ly-kho/qr_sanpham.aspx?id=' + id;
            // Dùng api tạo qr
            var qrSrc = 'https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=' + encodeURIComponent(qrUrl);
            
            document.getElementById('qrImage').src = qrSrc;
            document.getElementById('qrSeriLabel').innerText = 'Seri: ' + (seri ? seri : 'Không có');
            document.getElementById('qrModal').style.display = 'block';
        }
    </script>
    <script src="https://unpkg.com/@zxing/browser@0.1.5/umd/zxing-browser.min.js"></script>
    <script type="text/javascript">
        (function () {
            var scanBuffer = "";
            var scanTimer = null;
            var scanStartedAt = 0;
            var scanMinLength = 4;
            var scanGap = 70;
            var cameraStream = null;
            var cameraDetector = null;
            var cameraReader = null;
            var cameraControls = null;
            var cameraScanActive = false;

            function getQuickElement(id) {
                return document.getElementById(id);
            }

            function getQuickElementBySuffix(id) {
                return document.querySelector('[id$="_' + id + '"]') || document.getElementById(id);
            }

            window.updateQuickSerialPreview = function () {
                var serialInput = getQuickElement('<%= txt_so_seri.ClientID %>');
                var quantityInput = getQuickElementBySuffix('txt_quick_quantity');
                var preview = document.getElementById('quickSerialPreview');
                if (!serialInput || !quantityInput || !preview) return;

                var baseSerial = serialInput.value.trim();
                var quantity = parseInt(quantityInput.value, 10);
                if (!baseSerial || !quantity || quantity < 1) {
                    preview.style.display = 'none';
                    preview.innerHTML = '';
                    return;
                }

                quantity = Math.min(quantity, 1000);
                var html = '<div class="fg-green fw-600">Danh sách seri sẽ tạo (' + quantity + ' sản phẩm)</div>';
                html += '<div style="max-height:150px; overflow:auto; background:#fff; border:1px solid #e2e8f0; border-radius:8px; padding:8px 12px; margin-top:7px; font-family:Consolas,monospace; font-size:12px;">';
                var baseNumber = parseInt(baseSerial, 10);
                if (isNaN(baseNumber) || String(baseNumber) !== baseSerial.replace(/^0+(?=\d)/, '')) {
                    preview.innerHTML = '<div class="fg-red">Mã seri phải là chuỗi số.</div>';
                    preview.style.display = 'block';
                    return;
                }
                var serialWidth = baseSerial.length;
                for (var i = 1; i <= quantity; i++) {
                    var serial = String(baseNumber + i - 1);
                    while (serial.length < serialWidth) serial = '0' + serial;
                    html += '<div>' + i + '. ' + serial + '</div>';
                }
                html += '</div>';
                preview.innerHTML = html;
                preview.style.display = 'block';
            };

            function submitScannedBarcode(value) {
                var hiddenBarcode = getQuickElementBySuffix('txt_quick_barcode');
                var openButton = getQuickElementBySuffix('but_open_quick_entry');
                if (!hiddenBarcode || !openButton) return;
                hiddenBarcode.value = value;
                __doPostBack(openButton.name, '');
            }

            function stopCameraStream() {
                cameraScanActive = false;
                if (cameraControls && cameraControls.stop) {
                    cameraControls.stop();
                    cameraControls = null;
                }
                cameraReader = null;
                if (cameraStream) {
                    cameraStream.getTracks().forEach(function (track) { track.stop(); });
                    cameraStream = null;
                }
                var video = document.getElementById('cameraBarcodeVideo');
                if (video) video.srcObject = null;
            }

            function optimizeCamera(video) {
                var track = video && video.srcObject && video.srcObject.getVideoTracks ? video.srcObject.getVideoTracks()[0] : null;
                if (!track || !track.getCapabilities || !track.applyConstraints) return;
                var capabilities = track.getCapabilities();
                var advanced = {};
                if (capabilities.focusMode && capabilities.focusMode.indexOf('continuous') !== -1)
                    advanced.focusMode = 'continuous';
                if (capabilities.zoom && capabilities.zoom.max > capabilities.zoom.min)
                    advanced.zoom = Math.min(capabilities.zoom.min + 1, capabilities.zoom.max);
                if (Object.keys(advanced).length > 0)
                    track.applyConstraints({ advanced: [advanced] }).catch(function () { });
            }

            window.closeCameraScanner = function () {
                stopCameraStream();
                var modal = document.getElementById('cameraBarcodeModal');
                if (modal) modal.style.display = 'none';
            };

            function detectCameraBarcode(video, status) {
                if (!cameraScanActive || !cameraDetector) return;
                cameraDetector.detect(video).then(function (barcodes) {
                    if (!cameraScanActive) return;
                    if (barcodes.length > 0 && barcodes[0].rawValue) {
                        var value = barcodes[0].rawValue.trim();
                        status.innerText = 'Đã nhận barcode: ' + value;
                        stopCameraStream();
                        document.getElementById('cameraBarcodeModal').style.display = 'none';
                        submitScannedBarcode(value);
                        return;
                    }
                    window.setTimeout(function () { detectCameraBarcode(video, status); }, 120);
                }).catch(function () {
                    if (cameraScanActive)
                        window.setTimeout(function () { detectCameraBarcode(video, status); }, 250);
                });
            }

            window.openCameraScanner = function () {
                var modal = document.getElementById('cameraBarcodeModal');
                var video = document.getElementById('cameraBarcodeVideo');
                var status = document.getElementById('cameraBarcodeStatus');
                if (!modal || !video || !status) return;
                modal.style.display = 'block';
                status.innerText = 'Đang khởi động camera...';

                if (typeof window.BarcodeDetector !== 'function' && !window.ZXingBrowser) {
                    status.innerText = 'Trình duyệt này chưa hỗ trợ quét barcode bằng camera. Hãy dùng Chrome mới hoặc máy quét USB.';
                    return;
                }
                if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
                    status.innerText = 'Trình duyệt không hỗ trợ truy cập camera.';
                    return;
                }

                stopCameraStream();
                if (typeof window.BarcodeDetector !== 'function') {
                    cameraScanActive = true;
                    status.innerText = 'Đang khởi động bộ đọc barcode...';
                    cameraReader = new ZXingBrowser.BrowserMultiFormatReader();
                    Promise.resolve(cameraReader.decodeFromConstraints({
                        video: {
                            facingMode: { ideal: 'environment' },
                            width: { min: 640, ideal: 1280, max: 1920 },
                            height: { min: 480, ideal: 720, max: 1080 },
                            frameRate: { ideal: 30 },
                            focusMode: { ideal: 'continuous' }
                        },
                        audio: false
                    }, video, function (result, error, controls) {
                        if (controls) cameraControls = controls;
                        optimizeCamera(video);
                        if (!cameraScanActive || !result) return;
                        var value = result.getText().trim();
                        status.innerText = 'Đã nhận barcode: ' + value;
                        stopCameraStream();
                        modal.style.display = 'none';
                        submitScannedBarcode(value);
                    })).catch(function () {
                        status.innerText = 'Không thể khởi động bộ đọc barcode bằng camera.';
                    });
                    return;
                }

                cameraDetector = new BarcodeDetector();
                navigator.mediaDevices.getUserMedia({
                    video: {
                        facingMode: { ideal: 'environment' },
                            width: { min: 640, ideal: 1280, max: 1920 },
                            height: { min: 480, ideal: 720, max: 1080 },
                            frameRate: { ideal: 30 },
                            focusMode: { ideal: 'continuous' }
                    },
                    audio: false
                })
                    .then(function (stream) {
                        cameraStream = stream;
                    video.srcObject = stream;
                    video.play().catch(function () { });
                    optimizeCamera(video);
                        cameraScanActive = true;
                        status.innerText = 'Đưa một mã vạch vào gần khung xanh, để phần vạch chiếm phần lớn khung hình...';
                        if (cameraDetector) {
                            detectCameraBarcode(video, status);
                        }
                    })
                    .catch(function () {
                        status.innerText = 'Không thể mở camera. Hãy cấp quyền camera cho localhost.';
                    });
            };

            document.addEventListener('keydown', function (event) {
                if (event.key === 'Enter') {
                    var elapsed = scanStartedAt ? Date.now() - scanStartedAt : 99999;
                    if (scanBuffer.length >= scanMinLength && elapsed <= 1200) {
                        event.preventDefault();
                        submitScannedBarcode(scanBuffer);
                    }
                    scanBuffer = '';
                    scanStartedAt = 0;
                    return;
                }

                if (event.key.length !== 1 || event.ctrlKey || event.altKey || event.metaKey) return;
                if (!scanStartedAt) scanStartedAt = Date.now();
                scanBuffer += event.key;
                if (scanTimer) clearTimeout(scanTimer);
                scanTimer = setTimeout(function () {
                    scanBuffer = '';
                    scanStartedAt = 0;
                }, scanGap);
            });

            document.addEventListener('input', function (event) {
                if (event.target && (event.target.id.indexOf('txt_quick_quantity') !== -1 || event.target.id === '<%= txt_so_seri.ClientID %>')) {
                    window.updateQuickSerialPreview();
                }
            });
        })();
    </script>
</asp:Content>
