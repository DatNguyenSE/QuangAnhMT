<%@ Page Title="Quản lý báo giá" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="admin_quan_ly_bao_gia_Default" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .quote-expiry-cell {
            white-space: nowrap;
        }
        .quote-expired {
            display: inline-block;
            margin-top: 3px;
            padding: 1px 4px;
            font-size: 11px;
            font-weight: 700;
            color: #dc2626;
            background-color: #fee2e2;
            border: 1px solid #fecaca;
            border-radius: 2px;
        }
        .quote-date {
            line-height: 1.45;
            white-space: nowrap;
        }
        .quote-date-user {
            font-weight: 600;
            color: #333;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div id="full_page_loading" class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important; display: none;">
        <div style="padding-top: 45vh;">
            <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
            <div class="text-center fg-white mt-4 text-bold" style="font-size: 1.2rem;">Đang xử lý dữ liệu... Vui lòng không đóng trang.</div>
        </div>
    </div>
    
    <script type="text/javascript">
        function generateRandomSerial() {
            var now = new Date();
            var yy = now.getFullYear().toString().substring(2);
            var mm = (now.getMonth() + 1).toString().padStart(2, '0');
            var dd = now.getDate().toString().padStart(2, '0');
            var hh = now.getHours().toString().padStart(2, '0');
            var min = now.getMinutes().toString().padStart(2, '0');
            var ss = now.getSeconds().toString().padStart(2, '0');
            
            var seri = dd + mm + yy + hh + min + ss;
            document.getElementById('<%= txt_so_seri.ClientID %>').value = seri;
        }

        function autoFillSeri(dropdown) {
            var selectedOption = dropdown.options[dropdown.selectedIndex];
            if (selectedOption) {
                var seri = selectedOption.getAttribute('data-seri');
                var txtSoSeri = document.getElementById('<%= txt_so_seri.ClientID %>');
                if (txtSoSeri) {
                    txtSoSeri.value = (seri != null) ? seri : "";
                }
            }
        }
    </script>
    
    <asp:UpdatePanel ID="up_import" runat="server" UpdateMode="Conditional">
        <Triggers>
            <asp:PostBackTrigger ControlID="but_import_excel" />
        </Triggers>
        <ContentTemplate>
            <asp:Panel ID="pn_import" runat="server" Visible="false" DefaultButton="but_import_excel">
                <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                    <div style='top: 0; left: 0px; margin: 0 auto; max-width: 600px; opacity: 1;'>
                        <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                            <a href='#' class='fg-white d-inline' runat="server" id="close_import" onserverclick="but_show_form_import_Click" title='Đóng'>
                                <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                            </a>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                NHẬP DỮ LIỆU TỪ EXCEL
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>
                <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                    <div style='top: 0; left: 0; margin: 0 auto; max-width: 606px; opacity: 1;'>
                        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px">
                            <div class="row">
                                <div class="cell-12">
                                    <div class="mt-3">
                                        <div class="fw-600">Chọn file Excel</div>
                                        <div class="mt-2">
                                            <asp:FileUpload ID="file_import" runat="server" data-role="file" data-button-title="<span class='mif-folder'></span>" />
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <small><b>Lưu ý:</b> Nhấn nút <b class="fg-green">"Xác nhận nhập"</b> 1 lần và vui lòng không đóng trang cho đến khi hoàn tất.</small>
                                    </div>
                                </div>
                            </div>
                            <div class="mt-6 mb-20 text-right">
                                <asp:Button ID="but_import_excel" runat="server" CssClass="button success" Text="Xác nhận nhập" OnClick="but_import_excel_Click" OnClientClick="return showLoadingForImport();" />
                            </div>
                            <div class="mb-20"></div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

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
                                            <asp:ListItem Text="Ngày báo giá" Value="ngaybaogia" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Nhân viên" Value="HoTenNhanVien" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Khách hàng" Value="ten_khachhang" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="SĐT khách" Value="sdt_khachhang" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Địa chỉ" Value="diachi_khachhang" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Hạn báo giá" Value="ngayhethan" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Trạng thái báo giá" Value="trangthai" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Ngày bán" Value="ngayban_kyhopdong" Selected="true"></asp:ListItem>
                                             <asp:ListItem Text="Phần trăm doanh số" Value="phantram_doanhso_now" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Thưởng doanh số" Value="thuongdoanhso" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Tổng tiền" Value="TongTien" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Tổng giảm" Value="TongGiam" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Tổng sau giảm" Value="TongSauGiam" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="VAT (%)" Value="vat" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Sau thuế" Value="TongSauThue" Selected="true"></asp:ListItem>
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

    <asp:UpdatePanel ID="up_daban" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_daban" runat="server" Visible="false" DefaultButton="but_daban">
                <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                    <div style='top: 0; left: 0px; margin: 0 auto; max-width: 600px; opacity: 1;'>
                        <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                            <a href='#' class='fg-white d-inline' runat="server" id="A1" onserverclick="but_close_form_daban_Click" title='Đóng'>
                                <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                            </a>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                XÁC NHẬN ĐÃ BÁN
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>
                <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                    <div style='top: 0; left: 0; margin: 0 auto; max-width: 606px; opacity: 1;'>
                        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px">
                            <div class="row">
                                <div class="cell-lg-12">
                                    <div class="mt-3">
                                        <label class="fw-600">Hóa đơn - Chứng từ - Hợp đồng...</label>
                                        <input type="file" id="fileInput" onchange="uploadFile()" data-role="file" data-button-title="<span class='mif-file-upload'></span>" />
                                        <div id="message" runat="server"></div>
                                        <div id="uploadedFilePath"></div>
                                        <div style="display: none">
                                            <asp:TextBox ID="txt_link_fileupload" runat="server"></asp:TextBox>
                                        </div>
                                        <%--<div>
                                            <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                                        </div>
                                        <div style='position: absolute; bottom: 0px; left: 100px'>
                                            <asp:Button ID="Button2" runat="server" Text="Xóa ảnh cũ" CssClass="alert small" Visible="false" OnClick="Button2_Click" />
                                        </div>--%>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Số tiền đã thanh toán</label>
                                        <asp:TextBox ID="txt_dathanhtoan" Text="" placeholder="Nhập số tiền" onfocus="AutoSelect(this)" MaxLength="14" oninput="format_sotien_new(this)" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Ghi chú giao hàng</label>
                                        <asp:TextBox ID="txt_ghichu_chuagiao" Text="" placeholder="Đã giao hết thì bỏ trống" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="mt-6 mb-20">
                                <div style="float: left">
                                </div>
                                <div style="float: right">
                                    <asp:Button ID="but_daban" OnClick="but_daban_Click" runat="server" Text="XÁC NHẬN ĐÃ BÁN" CssClass="button success" />
                                </div>
                                <div style="clear: both"></div>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="up_daban">
        <ProgressTemplate>
            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                <div style="padding-top: 45vh;">
                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="up_xem_chitiet" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_xem_chitiet" runat="server" Visible="false">
                <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                    <div style='top: 0; left: 0px; margin: 0 auto; max-width: 1200px; opacity: 1;'>
                        <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                            <a href='#' class='fg-white d-inline' id="close_xem_chitiet" runat="server" onserverclick="but_close_xem_chitiet_Click" title='Đóng'>
                                <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                            </a>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                CHI TIẾT BÁO GIÁ
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>
                <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                    <div style='top: 0; left: 0; margin: 0 auto; max-width: 1206px; opacity: 1;'>
                        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px; min-height: 100vh;">
                            <div class="row">
                                <div class="cell-lg-6">
                                    <div class="mt-3">
                                        <b>Khách hàng:</b> <asp:Label ID="lbl_xc_khachhang" runat="server" CssClass="fg-blue fw-600"></asp:Label>
                                    </div>
                                    <div class="mt-1">
                                        <b>SĐT:</b> <asp:Label ID="lbl_xc_sdt" runat="server"></asp:Label>
                                    </div>
                                    <div class="mt-1">
                                        <b>Địa chỉ:</b> <asp:Label ID="lbl_xc_diachi" runat="server"></asp:Label>
                                    </div>
                                </div>
                                <div class="cell-lg-6">
                                    <div class="mt-3">
                                        <b>Ngày báo giá:</b> <asp:Label ID="lbl_xc_ngaybaogia" runat="server"></asp:Label>
                                    </div>
                                    <div class="mt-1">
                                        <b>Ngày hết hạn:</b> <asp:Label ID="lbl_xc_ngayhethan" runat="server"></asp:Label>
                                    </div>
                                    <div class="mt-1">
                                        <b>Trạng thái:</b> <asp:Label ID="lbl_xc_trangthai" runat="server" CssClass="fw-600"></asp:Label>
                                    </div>
                                </div>
                                
                                <div class="cell-lg-12">
                                    <hr />
                                    <div class="fw-600 fg-red mb-2">DANH SÁCH SẢN PHẨM</div>
                                    <div style="overflow: auto">
                                        <table class="bcorn-fix-title-table">
                                            <thead>
                                                <tr>
                                                    <th class="text-left" style="min-width: 250px;">Sản phẩm</th>
                                                    <th class="text-center" style="width: 80px;">SL</th>
                                                    <th class="text-right" style="min-width: 100px;">Đơn giá</th>
                                                    <th class="text-right" style="min-width: 100px;">Giảm giá</th>
                                                    <th class="text-right" style="min-width: 120px;">Thành tiền</th>
                                                    <th class="text-left" style="min-width: 120px;">Số seri</th>
                                                    <th class="text-left" style="min-width: 100px;">Bảo hành</th>
                                                    <th class="text-left" style="min-width: 150px;">Diễn giải</th>
                                                    <th class="text-left" style="min-width: 120px;">Seri Đổ L1</th>
                                                    <th class="text-left" style="min-width: 120px;">KH Đổ L1</th>
                                                    <th class="text-left" style="min-width: 120px;">Ngày Đổ L1</th>
                                                    <th class="text-left" style="min-width: 120px;">Seri Đổ L2</th>
                                                    <th class="text-left" style="min-width: 120px;">KH Đổ L2</th>
                                                    <th class="text-left" style="min-width: 120px;">Ngày Đổ L2</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <asp:Repeater ID="Repeater_ChiTietBaoGia" runat="server">
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td class="text-left"><%# Eval("TenSanPham") %></td>
                                                            <td class="text-center"><%# Eval("soluong") %></td>
                                                            <td class="text-right"><%# Eval("giaban_taithoidiemnay", "{0:#,##0}") %></td>
                                                            <td class="text-right"><%# Eval("GiamGiaHienThi") %></td>
                                                            <td class="text-right fw-600 fg-red"><%# Eval("TongSauGiam", "{0:#,##0}") %></td>
                                                            <td class="text-left"><%# Eval("So_Seri") %></td>
                                                            <td class="text-left"><%# Eval("Thang_BaoHanh") %></td>
                                                            <td class="text-left"><%# Eval("Mota") %></td>
                                                            <td class="text-left"><%# Eval("Seri_Do_L1") %></td>
                                                            <td class="text-left"><%# Eval("Id_khacHang_do_L1") %></td>
                                                            <td class="text-left"><%# Eval("NgayDo_L1", "{0:dd/MM/yyyy}") %></td>
                                                            <td class="text-left"><%# Eval("Seri_Do_L2") %></td>
                                                            <td class="text-left"><%# Eval("Id_khacHang_do_L2") %></td>
                                                            <td class="text-left"><%# Eval("NgayDo_L2", "{0:dd/MM/yyyy}") %></td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                                
                                <div class="cell-lg-12">
                                    <hr />
                                    <div class="row mb-5">
                                        <div class="cell-lg-8"></div>
                                        <div class="cell-lg-4 text-right">
                                            <div class="mb-1">
                                                <b>Tổng tiền:</b> <asp:Label ID="lbl_xc_tongtien" runat="server"></asp:Label>
                                            </div>
                                            <div class="mb-1">
                                                <b>Giảm giá khách hàng:</b> <asp:Label ID="lbl_xc_tonggiam" runat="server" CssClass="fg-orange"></asp:Label>
                                            </div>
                                            <div class="mb-1 text-bold" style="font-size: 1.2rem;">
                                                <b>Tổng sau giảm:</b> <asp:Label ID="lbl_xc_tongsaugiam" runat="server" CssClass="fg-red"></asp:Label>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="mb-20"></div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="up_add" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_add" runat="server" Visible="false" DefaultButton="but_add_edit">
                <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                    <div style='top: 0; left: 0px; margin: 0 auto; max-width: 1200px; opacity: 1;'>
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
                    <div style='top: 0; left: 0; margin: 0 auto; max-width: 1206px; opacity: 1;'>
                        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px">
                            <%--pl-4 pl-8-md pr-8-md pr-4--%>
                            <div class="row">
                                <asp:PlaceHolder ID="PlaceHolder8" runat="server" Visible="false">
                                    <div class="cell-lg-8 pr-2-lg mb-3 mt-3">
                                        <div class="d-flex">
                                            <asp:DropDownList ID="DropDownList2" runat="server" data-role="select"></asp:DropDownList>
                                            <asp:Button ID="but_check" OnClick="but_check_Click" runat="server" Text="Check" />
                                        </div>
                                    </div>
                                </asp:PlaceHolder>

                                <div class="cell-lg-12">
                                    <b>Thông tin khách hàng</b>
                                </div>

                                <div class="cell-lg-4  pl-2-lg pr-2-lg">
                                    <div class="mt-2">
                                        <small class="fg-red fw-600">SĐT khách hàng</small>
                                        <asp:TextBox ID="txt_sdt" CssClass="input-small" runat="server" data-role="input"></asp:TextBox>
                                        <%-- <ajaxToolkit:AutoCompleteExtender
                                            ID="AutoCompleteExtender1"
                                            runat="server"
                                            TargetControlID="txt_sdt"
                                            ServiceMethod="GetPhoneNumbers"
                                            MinimumPrefixLength="3"
                                            CompletionSetCount="10"
                                            EnableCaching="true">
                                        </ajaxToolkit:AutoCompleteExtender>--%>
                                    </div>
                                </div>
                                <div class="cell-lg-4  pl-2-lg pr-2-lg">
                                    <div class="mt-2">
                                        <small class="fg-red fw-600">Tên khách hàng</small>
                                        <asp:TextBox ID="txt_ten_kh" CssClass="input-small" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="cell-lg-4  pl-2-lg pr-2-lg">
                                    <div class="mt-2">
                                        <small class="fg-red fw-600">Địa chỉ khách hàng</small>
                                        <asp:TextBox ID="txt_diachi_kh" CssClass="input-small" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="cell-lg-4  pl-2-lg pr-2-lg">
                                    <div class="mt-2">
                                        <small class="fg-red fw-600">Giảm giá khách hàng</small>
                                        <asp:TextBox ID="txt_giamgia_kh" CssClass="input-small" Text="0" placeholder="Nhập số tiền hoặc %" onfocus="AutoSelect(this)" runat="server" data-role="input"></asp:TextBox>
                                        <asp:RadioButtonList ID="rd_loai_giamgia" runat="server" RepeatDirection="Horizontal" CssClass="mt-1" style="font-size: 14px; display: flex; gap: 10px;">
                                            <asp:ListItem Value="phantram" Selected="True">Phần trăm (%)</asp:ListItem>
                                            <asp:ListItem Value="sotien">Số tiền</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                </div>

                                <div class="cell-lg-4  pl-2-lg pr-2-lg">
                                    <div class="mt-2">
                                        <small class="fg-red fw-600">VAT (%)</small>
                                        <asp:TextBox ID="txt_vat" Text="0" CssClass="input-small" onfocus="AutoSelect(this)" MaxLength="3" oninput="format_sotien_new(this)" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="cell-lg-4  pl-2-lg pr-2-lg">
                                    <div class="mt-2">
                                        <small class="fg-red fw-600">Số ngày hiệu lực</small>
                                        <asp:TextBox ID="txt_songayhieuluc" CssClass="input-small" placeholder="Nhập số ngày. VD: 30" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                </div>
                                <asp:PlaceHolder ID="PlaceHolder10" runat="server" Visible="false">
                                    <div class="cell-lg-8 pl-2-lg pr-2-lg">
                                        <div class="mt-2">
                                            <small class="fg-red fw-600">Ghi chú giao hàng</small>
                                            <asp:TextBox ID="txt_ghichu_giaohang" CssClass="input-small" placeholder="Đã giao hết thì bỏ trống" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>
                                </asp:PlaceHolder>
                            </div>
                            <div class="mt-6 text-right">

                                <asp:Button ID="but_add_edit" runat="server" Text="" CssClass="button success small" OnClick="but_add_edit_Click" />
                            </div>
                            <hr />
                            <asp:PlaceHolder ID="PlaceHolder1" runat="server">
                                <div class="row mt-3">
                                    <div class="cell-lg-12 p-3 bg-light">
                                        <b>Thêm sản phẩm vào báo giá</b>

                                        <asp:Panel ID="Panel1" runat="server" DefaultButton="but_add_sp_chitiet">
                                            <div class="row">
                                                <div class="cell-lg-6 pl-2-lg pr-2-lg">
                                                    <div class="mt-2">
                                                        <small class="fg-red fw-600">Sản phẩm</small>
                                                        <asp:DropDownList ID="DropDownList1" runat="server" data-role="select" onchange="autoFillSeri(this)"></asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="cell-lg-3 pl-2-lg pr-2-lg">
                                                    <div class="mt-2">
                                                        <small class="fg-red fw-600">Số lượng</small>
                                                        <asp:TextBox ID="txt_soluong" data-role="spinner" data-buttons-position="right" Text="1" data-min-value="1" data-max-value="999" onfocus="AutoSelect(this)" oninput="format_sotien_new(this)" runat="server"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="cell-lg-3 pl-2-lg pr-2-lg">
                                                    <div class="mt-2">
                                                        <small class="fg-red fw-600">Giảm giá (%)</small>
                                                        <asp:TextBox ID="txt_giamgia_phantram" Text="0" onfocus="AutoSelect(this)" MaxLength="4" runat="server" data-role="input"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="cell-lg-4 pl-2-lg pr-2-lg">
                                                    <div class="mt-2">
                                                        <small class="fg-red fw-600">Số seri</small>
                                                        <div class="d-flex">
                                                            <asp:TextBox ID="txt_so_seri" runat="server" data-role="input" CssClass="w-100"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="cell-lg-3 pl-2-lg pr-2-lg">
                                                    <div class="mt-2">
                                                        <small class="fg-red fw-600">Bảo hành / tháng</small>
                                                        <asp:TextBox ID="txt_baohanh_thang" runat="server" data-role="input"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="cell-lg-5 pl-2-lg pr-2-lg">
                                                    <div class="mt-2">
                                                        <small class="fg-red fw-600">Diễn giải</small>
                                                        <asp:TextBox ID="txt_diengiai" runat="server" data-role="input"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="cell-lg-12 text-right pr-2-lg mt-3 mb-3">
                                                    <a class="btn-copy button mini rounded dark" onclick="copyToClipboard('https://thaianaudio.vn/bao-gia.aspx?id=<%=ViewState["id_guide_chitiet"] %>')">Copy Link</a>
                                                    <asp:Button ID="but_add_sp_chitiet" OnClick="but_add_sp_chitiet_Click" runat="server" CssClass="info small" Text="THÊM SẢN PHẨM" />
                                                </div>
                                            </div>
                                        </asp:Panel>
                                    </div>

                                    <div class="cell-lg-12">
                                        <div style="overflow: auto" class=" mt-3">
                                            <table class="bcorn-fix-title-table">
                                                <thead>
                                                    <tr class="">
                                                        <th style="width: 1px;">TT</th>
                                                        <%--<th style="width: 1px;">
                                                            <input data-role="hint" data-hint-position="top" data-hint-text="Chọn/Bỏ chọn" type="checkbox" onkeypress="if (event.keyCode==13) return false;" onclick="$('.checkbox-table2 input[type=checkbox]').prop('checked', this.checked)">
                                                        </th>--%>
                                                        <th style="width: 50px; min-width: 50px;">Ảnh</th>
                                                        <th style="width: 160px; min-width: 160px;">Sản phẩm</th>
                                                        <th style="width: 1px; min-width: 1px;">Hãng</th>
                                                        <th style="min-width: 200px;">Thông số kỹ thuật</th>
                                                        <th style="width: 1px; min-width: 1px;">Giá</th>
                                                        <th style="width: 1px; min-width: 1px;">ĐVT</th>
                                                        <th style="width: 1px; min-width: 1px;">SL</th>
                                                        <th style="width: 100px; min-width: 100px;">Thành tiền</th>
                                                        <th style="width: 110px; min-width: 110px;">Tổng sau giảm</th>
                                                        <th style="width: 1px; min-width: 1px;"></th>
                                                    </tr>
                                                </thead>

                                                <tbody>
                                                    <asp:Repeater ID="Repeater2" runat="server">
                                                        <ItemTemplate>
                                                            <span style="display: none">
                                                                <asp:Label ID="lbID_chitiet" runat="server" Text='<%#Eval("id") %>'></asp:Label>
                                                            </span>
                                                            <tr>
                                                                <td class="text-center"><%# Container.ItemIndex + 1 %></td>
                                                                <%--<td class="checkbox-table2">
                                                                    <asp:CheckBox ID="checkID2" runat="server" onkeypress="if (event.keyCode==13) return false;" />
                                                                </td>--%>
                                                                <td>
                                                                    <div data-role="lightbox" class="c-pointer">
                                                                        <img src='<%#Eval("anh") %>' class="img-cover-vuong" width="50" height="50" />
                                                                    </div>
                                                                </td>
                                                                <td class="text-left">
                                                                    <%#Eval("ten_sanpham") %>
                                                                </td>
                                                                <td>
                                                                    <%#Eval("TenHang") %>
                                                                </td>
                                                                <td class="text-left">
                                                                    <small><%#Eval("thongso_kythuat") %></small>
                                                                </td>
                                                                <td class="text-right"><%#Eval("giaban_taithoidiemnay","{0:#,##0}") %></td>
                                                                <td class="text-center">
                                                                    <%#Eval("DVT") %>
                                                                </td>
                                                                <td class="text-center">
                                                                    <%-- <div><%#Eval("soluong","{0:#,##0}") %></div>--%>
                                                                    <asp:TextBox data-role="input" CssClass="input-small" data-clear-button="false" oninput="format_sotien_new(this)" ID="txt_sl_chitiet" Width="40" MaxLength="4" runat="server" onfocus="AutoSelect(this)" Text='<%#Eval("soluong","{0:#,##0}") %>' onkeypress="if (event.keyCode==13) return false;"></asp:TextBox>
                                                                </td>
                                                                <td class="text-right"><%#Eval("thanhtien","{0:#,##0}") %>
                                                                    <%--<asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%#Eval("giamgia_phantram").ToString()!="0" %>'>
                                                                        <div><small class="fg-orange"><span class="mif mif-arrow-drop-down"></span><span data-role="hint" data-hint-position="left" data-hint-text="Giảm giá"><%#Eval("giamgia_phantram","{0:#,##0}") %>%</span></small></div>
                                                                    </asp:PlaceHolder>--%>
                                                                    <div><small>Giảm giá (%)</small></div>
                                                                    <asp:TextBox placeholder="Giảm giá (%)" data-role="input" CssClass="input-small" data-clear-button="false" ID="txt_giamgia_phantram_chitiet" Width="20" MaxLength="4" runat="server" Text='<%#Eval("giamgia_phantram") %>' onkeypress="if (event.keyCode==13) return false;"></asp:TextBox>

                                                                </td>
                                                                <td class="text-right text-bold"><%#Eval("TongSauGiam","{0:#,##0}") %></td>
                                                                <td>
                                                                    <asp:Button ToolTip="Xóa" ID="but_xoachitiet" OnClick="but_xoachitiet_Click" CommandArgument='<%#Eval("id") %>' runat="server" Text="Xóa" CssClass="mini rounded alert" />

                                                                </td>
                                                            </tr>
                                                        </ItemTemplate>
                                                    </asp:Repeater>

                                                </tbody>
                                                <tfoot>
                                                    <tr class="bg-light text-bold">
                                                        <td></td>
                                                        <td colspan="8" class="text-right">TỔNG CỘNG</td>
                                                        <td class="text-right text-bold"><%=ViewState["TongSauGiam_ChiTiet"] %></td>
                                                        <td colspan="1"></td>
                                                    </tr>
                                                    <%if ((ViewState["pt_giamgiadacbiet"] != null && ViewState["pt_giamgiadacbiet"].ToString() != "0") || (ViewState["giamgia_dacbiet"] != null && ViewState["giamgia_dacbiet"].ToString() != "0"))
                                                        {  %>
                                                    <%if (ViewState["pt_giamgiadacbiet"] != null && ViewState["pt_giamgiadacbiet"].ToString() != "0")
                                                        {  %>
                                                    <tr class="bg-yellow fg-red text-bold">
                                                        <td></td>
                                                        <td colspan="8" class="text-right">GIẢM GIÁ ĐẶC BIỆT (%)</td>
                                                        <td class="text-right text-bold">
                                                            <%= ViewState["pt_giamgiadacbiet"]%>%
                                                           <div><small><%= ViewState["giamgia_dacbiet"] != null ? Convert.ToInt64(ViewState["giamgia_dacbiet"]).ToString("#,##0") : "0" %></small></div>
                                                        </td>
                                                        <td colspan="1"></td>
                                                    </tr>
                                                    <%}
                                                    else
                                                        {  %>
                                                    <tr class="bg-yellow fg-red text-bold">
                                                        <td></td>
                                                        <td colspan="8" class="text-right">GIẢM GIÁ ĐẶC BIỆT (số tiền)</td>
                                                        <td class="text-right text-bold">
                                                            <%= ViewState["giamgia_dacbiet"] != null ? Convert.ToInt64(ViewState["giamgia_dacbiet"]).ToString("#,##0") : "0" %></td>
                                                        <td colspan="1"></td>
                                                    </tr>
                                                    <%} %>
                                                    <%if (ViewState["vat_chitiet"].ToString() != "0")
                                                        {  %>
                                                    <tr class="text-bold">
                                                        <td></td>
                                                        <td colspan="8" class="text-right">VAT</td>
                                                        <td class="text-right text-bold">
                                                            <%= ViewState["vat_chitiet"]%>%
                                                           <div><small><%= ViewState["thanhtien_vat_chitiet"]%></small></div>
                                                        </td>
                                                        <td colspan="1"></td>
                                                    </tr>
                                                    <%} %>

                                                    <tr class="bg-cobalt fg-white text-bold">
                                                        <td></td>
                                                        <td colspan="8" class="text-right">GIÁ TRỊ THỰC CỦA ĐƠN HÀNG</td>
                                                        <td class="text-right text-bold">
                                                            <%= ViewState["donhang_saugiamgia"] != null ? Convert.ToInt64(ViewState["donhang_saugiamgia"]).ToString("#,##0") : "0" %></td>
                                                        <td colspan="1"></td>
                                                    </tr>
                                                    <%}
                                                        else
                                                        {  %>
                                                    <%if (ViewState["vat_chitiet"].ToString() != "0")
                                                        {  %>
                                                    <tr class="text-bold">
                                                        <td></td>
                                                        <td colspan="8" class="text-right">VAT</td>
                                                        <td class="text-right text-bold">
                                                            <%= ViewState["vat_chitiet"]%>%
                                                             <div><small><%= ViewState["thanhtien_vat_chitiet"]%></small></div>
                                                        </td>
                                                        <td colspan="1"></td>
                                                    </tr>
                                                    <%} %>

                                                    <tr class="bg-cobalt fg-white text-bold">
                                                        <td></td>
                                                        <td colspan="8" class="text-right">GIÁ TRỊ THỰC CỦA ĐƠN HÀNG</td>
                                                        <td class="text-right text-bold">
                                                            <%= ViewState["donhang_saugiamgia"] != null ? Convert.ToInt64(ViewState["donhang_saugiamgia"]).ToString("#,##0") : "0" %></td>
                                                        <td colspan="1"></td>
                                                    </tr>
                                                    <%} %>
                                                </tfoot>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </asp:PlaceHolder>


                            <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible="false">

                                <div class="row mt-5">
                                    <div class="cell-lg-5 pr-4-lg mt-3">
                                        <div><b>Thanh toán công nợ</b></div>
                                        <asp:Panel ID="Panel2" runat="server" DefaultButton="but_thanhtoan_congno">
                                            <div class="d-flex mt-1">

                                                <asp:TextBox ID="txt_sotien_thanhtoan_congno" Text="" onfocus="AutoSelect(this)" MaxLength="14" oninput="format_sotien_new(this)" runat="server" data-role="input"></asp:TextBox>
                                                <asp:Button ID="but_thanhtoan_congno" runat="server" Text="Xác nhận" CssClass="button warning" OnClick="but_thanhtoan_congno_Click" />

                                            </div>
                                        </asp:Panel>
                                    </div>
                                    <div class="cell-lg-7 pl-4-lg mt-3">
                                        <div><b>Lịch sử thanh toán</b></div>
                                        <div style="overflow: auto" class=" mt-1">
                                            <table class="table row-hover table-border cell-border compact striped">
                                                <thead class="bg-orange">
                                                    <%--<th class=" text-bold fg-white" style="width: 1px;">TT</th>--%>
                                                    <th class=" text-bold fg-white" style="min-width: 80px;">Ngày</th>
                                                    <th class=" text-bold fg-white" style="min-width: 100px;">Xác nhận</th>
                                                    <th class=" text-bold fg-white" style="min-width: 80px;">Số tiền</th>
                                                </thead>
                                                <tbody>
                                                    <asp:Repeater ID="Repeater5" runat="server">
                                                        <ItemTemplate>
                                                            <tr>
                                                                <%-- <td class="text-center">
                                                                    <%# Container.ItemIndex + 1 %>
                                                                </td>--%>
                                                                <td><%#Eval("ngay_thanhtoan","{0:dd/MM/yyyy HH:mm}") %>'</td>
                                                                <td>
                                                                    <div>
                                                                        <%#Eval("hoten") %>
                                                                    </div>
                                                                </td>
                                                                <td class=" text-right"><%#Eval("sotien_thanhtoan","{0:#,##0}") %></td>
                                                            </tr>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </tbody>
                                                <tfoot>
                                                    <tr>
                                                        <td colspan="2" class="text-bold text-right">TỔNG THANH TOÁN</td>
                                                        <td class="text-bold text-right"><%= ViewState["Tong_ThanhToan"] != null ? Convert.ToInt64(ViewState["Tong_ThanhToan"]).ToString("#,##0") : "0" %></td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="3" class="text-right fg-red">
                                                            <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                                                        </td>
                                                    </tr>
                                                </tfoot>
                                            </table>
                                        </div>
                                    </div>


                                </div>
                            </asp:PlaceHolder>

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
                                        <div class="fw-600">Người báo giá</div>
                                        <asp:ListBox ID="ListBox2" runat="server" SelectionMode="Multiple" data-role="select"></asp:ListBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600 mt-3">Đã bán/Chưa bán</label>
                                        <asp:DropDownList ID="DropDownList3" runat="server" data-role="select">
                                            <asp:ListItem Text="Tất cả" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="Đã bán" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="Chưa bán" Value="2"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600 mt-3">Thanh toán/Công nợ</label>
                                        <asp:DropDownList ID="DropDownList4" runat="server" data-role="select">
                                            <asp:ListItem Text="Tất cả" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="Đã thanh toán" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="Còn nợ" Value="2"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600 mt-3">Giao hàng/Chưa giao</label>
                                        <asp:DropDownList ID="DropDownList5" runat="server" data-role="select">
                                            <asp:ListItem Text="Tất cả" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="Đã giao đủ" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="Chưa giao đủ" Value="2"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="mt-3" style="display: none">
                                        <div class="fw-600">Phân loại báo giá</div>
                                        <%--value của ListBox1 k nên đặt unicode để tránh lưu cookie k được--%>
                                        <asp:ListBox ID="ListBox1" runat="server" SelectionMode="Multiple" data-role="select">
                                            <asp:ListItem Value="" Text="Tất cả"></asp:ListItem>
                                            <asp:ListItem Value="DaBan" Text="Đã"></asp:ListItem>
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
                                            <asp:ListItem Text="Dựa vào ngày báo giá" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="Dựa vào ngày bán" Value="2"></asp:ListItem>
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

    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <div class="pos-relative pb-11">
                <div id="menutop-tool-bc" style="position: fixed; top: 52px; width: 100%; z-index: 4">
                    <ul class="h-menu bg-white">

                        <li data-role="hint" data-hint-position="top" data-hint-text="Tạo báo giá">
                            <asp:LinkButton ID="but_show_form_add" OnClick="but_show_form_add_Click" runat="server"><span class="mif-plus"></span></asp:LinkButton>
                        </li>
                        <li style="display: none;" data-role="hint" data-hint-position="top" data-hint-text="Nhập từ Excel">
                            <asp:LinkButton ID="but_show_form_import" OnClick="but_show_form_import_Click" runat="server"><span class="mif-file-excel fg-green"></span></asp:LinkButton>
                        </li>
                        <%--<li data-role="hint" data-hint-position="top" data-hint-text="Lưu">
                            <asp:LinkButton ID="but_save" OnClick="but_save_Click" runat="server"><span class="mif-floppy-disk"></span></asp:LinkButton>
                        </li>--%>

                        <li data-role="hint" data-hint-position="top" data-hint-text="Xóa">
                            <asp:LinkButton ID="but_xoa" OnClick="but_xoa_Click" runat="server"><span class="mif-bin"></span></asp:LinkButton>
                        </li>

                        <li data-role="hint" data-hint-position="top" data-hint-text="Lọc">
                            <asp:LinkButton ID="but_show_form_loc" runat="server" OnClick="but_show_form_loc_Click"><span class="mif-filter"></span></asp:LinkButton>
                        </li>
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
                                        <th style="width: 60px; min-width: 60px;">ID</th>
                                        <th style="width: 1px;">
                                            <%--data-role="checkbox" data-style="2"--%>
                                            <input data-role="hint" data-hint-position="top" data-hint-text="Chọn/Bỏ chọn" type="checkbox" onkeypress="if (event.keyCode==13) return false;" onclick="$('.checkbox-table input[type=checkbox]').prop('checked', this.checked)">
                                        </th>
                                        <th style="width: 130px; min-width: 130px;">Ngày báo giá</th>
                                        <th style="width: 150px; min-width: 150px;">Khách hàng</th>
                                        <th style="width: 220px; min-width: 220px;">Địa chỉ</th>
                                        <th style="width: 150px; min-width: 150px;">Hạn BG</th>
                                        <th style="width: 110px; min-width: 110px;">Đã bán</th>
                                        <th style="width: 1px; min-width: 1px;">Tổng tiền</th>
                                        <th style="width: 1px; min-width: 1px;">Tổng giảm</th>
                                        <th style="width: 1px; min-width: 1px;">Tổng sau giảm</th>
                                        <th style="width: 1px; min-width: 1px;">VAT (%)</th>
                                        <th style="width: 1px; min-width: 1px;">Sau thuế</th>
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
                                                <td class="text-center"><%# Eval("id") %></td>

                                                <td class="checkbox-table">
                                                    <asp:CheckBox ID="checkID" runat="server" onkeypress="if (event.keyCode==13) return false;" />
                                                </td>

                                                <td class="text-left quote-date">
                                                    <div><%# Eval("ngaybaogia", "{0:dd/MM/yyyy}") %></div>
                                                    <div class="quote-date-user"><%# Eval("HoTenNhanVien") %></div>
                                                </td>



                                                <td class="text-left">
                                                    <asp:LinkButton CssClass="fg-black fw-600" OnClick="but_show_chinhsua_Click" data-role="hint" data-hint-position="top" data-hint-text="Chi tiết báo giá" ID="but_name_1" CommandArgument='<%# Eval("id") %>' runat="server">
                                                        <%#Eval("ten_khachhang") %>
                                                    </asp:LinkButton>
                                                    <asp:PlaceHolder runat="server" Visible='<%# !string.IsNullOrEmpty(Eval("sdt_khachhang") as string) %>'>
                                                        <div><a class="fw-600" title="Nhấn để gọi" href="tel:<%#Eval("sdt_khachhang") %>"><span class="mif-phone pr-1"></span><%#Eval("sdt_khachhang") %></a></div>
                                                    </asp:PlaceHolder>
                                                </td>

                                                <td class="text-left" style="max-width: 220px;">
                                                    <%# Eval("diachiKhachHang") %>
                                                </td>
                                                <td class="text-left quote-expiry-cell">
                                                    <div><%# Eval("ngayhethan") == null ? "Không rõ" : Convert.ToDateTime(Eval("ngayhethan")).ToString("dd/MM/yyyy") %></div>
                                                    <asp:Label ID="lbl_quote_expired" runat="server" CssClass="quote-expired" Text="Đã quá hạn" Visible='<%# Convert.ToBoolean(Eval("baoGiaQuaHan")) %>'></asp:Label>
                                                </td>
                                                
                                                <td>
                                                    <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%# Eval("trangthai").ToString() == "Đã ký HĐ" %>'>
                                                        <div class="button mini rounded success"><%# Eval("ngayban_kyhopdong", "{0:dd/MM/yyyy}") %></div>
                                                        <div><a href="<%# Eval("file_hopdong") %>"><span data-role="hint" data-hint-position="top" data-hint-text="Tải file" class="mif-download mif-lg"></span></a></div>
                                                        <asp:PlaceHolder ID="PlaceHolder9" runat="server" Visible='<%# Eval("ghichu_chuagiao").ToString() != "" %>'>
                                                            <asp:LinkButton CssClass="button mini warning rounded ani-flash" OnClick="but_show_chinhsua_Click" data-role="hint" data-hint-position="top" data-hint-text="Sửa ghi chú" ID="LinkButton2" CommandArgument='<%# Eval("id") %>' runat="server">
                                                                <%# Eval("ghichu_chuagiao") %>
                                                            </asp:LinkButton>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="PlaceHolder12" runat="server" Visible='<%# Eval("thuongdoanhso").ToString() != "0" %>'>
                                                            <div>
                                                                <small class="fw-500 fg-green">Thưởng <%# Eval("phantram_doanhso_now") %>% DS<br />
                                                                    = <%# Eval("thuongdoanhso", "{0:#,##0}") %></small>
                                                            </div>
                                                        </asp:PlaceHolder>
                                                    </asp:PlaceHolder>
                                                </td>

                                                <td class="text-right"><%#Eval("TongTien","{0:#,##0}") %>

                                                   
                                                </td>
                                                <td class="text-right">
                                                    <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%#Eval("TongGiam").ToString()!="0" %>'>
                                                        <div class="fg-orange"><%#Eval("TongGiam","{0:#,##0}") %></div>
                                                    </asp:PlaceHolder>
                                                </td>
                                                <td class="text-right"><%#Eval("TongSauGiam","{0:#,##0}") %></td>
                                                <td class="text-center fg-green"><%#Eval("vat") %>%
                                                     <asp:PlaceHolder ID="PlaceHolder7" runat="server" Visible='<%#Eval("vat").ToString()!="0" %>'>
                                                         <div><small><%#Eval("TongTien_VAT","{0:#,##0}") %></small></div>
                                                     </asp:PlaceHolder>
                                                </td>
                                                <td class="text-right text-bold"><%#Eval("TongSauThue","{0:#,##0}") %>
                                                    <asp:PlaceHolder ID="PlaceHolder6" runat="server" Visible='<%#Eval("congno").ToString()!="0" %>'>

                                                        <asp:LinkButton CssClass="button mini alert rounded ani-flash" OnClick="but_show_chinhsua_Click" data-role="hint" data-hint-position="top" data-hint-text="Thanh toán công nợ" ID="LinkButton1" CommandArgument='<%# Eval("id") %>' runat="server">
                                                               <%# Eval("congno","{0:#,##0}") %>
                                                        </asp:LinkButton>
                                                    </asp:PlaceHolder>
                                                </td>
                                                <td style="vertical-align: middle">
                                                    <%--https://thaianaudio.vn/bao-gia.aspx?id=--%>
                                                    <div class="dropdown-button place-right">
                                                        <button class="button small bg-transparent">
                                                            <span class="mif mif-more-horiz"></span>
                                                        </button>
                                                        <ul class="d-menu place-right" data-role="dropdown">
                                                            <li style="display: none;">
                                                                <asp:LinkButton ID="but_xem_chitiet_baogia" OnClick="but_xem_chitiet_baogia_Click" CommandArgument='<%# Eval("id") %>' runat="server">Chi tiết</asp:LinkButton>
                                                            </li>
                                                            <li>
                                                                <asp:LinkButton ID="but_show_chinhsua_dropdown" OnClick="but_show_chinhsua_Click" CommandArgument='<%# Eval("id") %>' runat="server">Chỉnh sửa</asp:LinkButton>
                                                            </li>
                                                            <li>
                                                                <asp:LinkButton ID="but_show_form_daban" OnClick="but_show_form_daban_Click" CommandArgument='<%# Eval("id") + "|" + Eval("TongSauThue","{0:#,##0}") %>' runat="server">Xác nhận đã bán</asp:LinkButton></li>
                                                            <li>
                                                                <a class="btn-copy" onclick="copyToClipboard('https://thaianaudio.vn/bao-gia.aspx?id=<%# Eval("id_guide") %>')">Copy Link
                                                                </a>
                                                            </li>
                                                            <li style="display: none;">
         
                                                                <a href='<%# "/admin/quan-ly-bao-gia/Export.aspx?id=" + Eval("id") %>' target="_blank">Xuất Excel</a>

                                                            </li>
                                                            <li>
                                                                <asp:LinkButton ID="lnkDelete" OnClick="lnk_xoadong_Click" CommandArgument='<%# Eval("id") %>' OnClientClick="return confirm('Bạn có chắc chắn muốn xóa phiếu báo giá này?');" runat="server">
                                                                    <span class="fg-red">Xóa</span>
                                                                </asp:LinkButton>
                                                            </li>

                                                            <%--<li class="divider"></li>--%>
                                                        </ul>
                                                    </div>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>

                                </tbody>
                                <%--<tfoot>
                                    <tr class="bg-white  text-bold">
                                        <td></td>
                                        <td colspan="6" class="text-right text-bold">TỔNG</td>
                                        <td class="text-right text-bold"><%=ViewState["TongThanhTien"] %>
                                          
                                        </td>
                                        <td><%if (ViewState["TongGiam"].ToString() != "0")
                                                {  %>
                                            <div class="fg-orange"><%=ViewState["TongGiam"] %></div>
                                            <%} %></td>
                                        <td class="text-right"><%=ViewState["TongSauGiam"] %></td>
                                        <td class="fg-green text-right"><%=ViewState["TongTien_VAT"] %></td>
                                        <td class=" text-bold text-right"><%=ViewState["TongSauThue"] %></td>
                                        <td></td>
                                    </tr>
                                </tfoot>--%>
                            </table>
                        </div>



                        <div class="mt-10"><b>Thống kê</b></div>
                        <div><small>Từ <%=ViewState["tungay"] %> đến <%=ViewState["denngay"] %></small></div>
                        <div class="row mt-3">
                            <div class="cell-lg-4">
                                <div class="bg-green p-4 fg-white">
                                    <div class="text-center"><b>THỐNG KÊ BÁN HÀNG</b></div>
                                    <small>
                                        <div class="place-left">Đơn đã bán</div>
                                        <div class="place-right"><b><%= Convert.ToInt64(ViewState["TongĐonaBan"]).ToString("#,##0") %></b></div>
                                        <div class="clr-bc"></div>
                                        <div class="place-left">Doanh thu</div>
                                        <div class="place-right"><b><%= Convert.ToInt64(ViewState["TongDoanhThu"]).ToString("#,##0") %></b></div>
                                        <div class="clr-bc"></div>
                                        <div class="place-left">Đã thanh toán</div>
                                        <div class="place-right"><b><%= Convert.ToInt64(ViewState["TongDaThanhToan"]).ToString("#,##0") %></b></div>
                                        <div class="clr-bc"></div>
                                        <div class="place-left">Công nợ</div>
                                        <div class="place-right"><b><%= Convert.ToInt64(ViewState["TongCongNo"]).ToString("#,##0") %></b></div>
                                        <div class="clr-bc"></div>
                                    </small>
                                    <asp:PlaceHolder ID="PlaceHolder11" runat="server" Visible="false">
                                        <hr />
                                        <div class="text-center"><b>LỢI NHUẬN BÁN HÀNG</b></div>
                                        <small>
                                            <div class="place-left">Tổng giá nhập</div>
                                            <div class="place-right"><b><%= Convert.ToInt64(ViewState["TongGiaNhap"]).ToString("#,##0") %></b></div>
                                            <div class="clr-bc"></div>
                                            <div class="place-left">Lợi nhuận</div>
                                            <div class="place-right"><b><%= Convert.ToInt64(ViewState["TongLoiNhuan"]).ToString("#,##0") %></b></div>
                                            <div class="clr-bc"></div>
                                        </small>
                                    </asp:PlaceHolder>
                                </div>
                            </div>
                        </div>



                    </div>
                </div>


            </div>
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
    <script>
        function uploadFile() {
            var fileInput = document.getElementById("fileInput");
            var messageDiv = document.getElementById("message");
            var uploadedFilePathDiv = document.getElementById("uploadedFilePath");

            if (fileInput.files.length > 0) {
                var file = fileInput.files[0];

                // Kiểm tra loại tệp
                var allowedExtensions = [".docx", ".doc", ".pdf", ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".heic"];
                var fileExtension = file.name.substr(file.name.lastIndexOf(".")).toLowerCase();
                if (allowedExtensions.indexOf(fileExtension) === -1) {
                    messageDiv.innerHTML = "Định dạng file không hợp lệ.";
                    return;
                }

                // Kiểm tra kích thước tệp
                var maxFileSize = 100 * 1024 * 1024; // 10 MB
                if (file.size > maxFileSize) {
                    messageDiv.innerHTML = "Vui lòng chọn file có kích thước nhỏ hơn 100 MB.";
                    return;
                }

                var formData = new FormData();
                formData.append("file", file);

                var xhr = new XMLHttpRequest();
                xhr.open("POST", "/uploads/Upload_Handler_File1.ashx", true);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        // Hiển thị đường dẫn file đã tải lên
                        uploadedFilePathDiv.innerHTML =
                            "<div><small>Tải file thành công.</small></div>"
                        // +"<a href='" + xhr.responseText + "' target='_blank'>" + xhr.responseText + "</a>";
                        document.getElementById('<%= txt_link_fileupload.ClientID %>').value = xhr.responseText; // Lưu đường dẫn vào input
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

    <script>
        function copyToClipboard(link) {
            // Tạo một element tạm để lưu link
            var tempInput = document.createElement("input");
            tempInput.style.position = "absolute";
            tempInput.style.left = "-9999px";
            tempInput.value = link;

            document.body.appendChild(tempInput); // Thêm element vào body
            tempInput.select(); // Chọn giá trị trong input
            document.execCommand("copy"); // Sao chép giá trị
            document.body.removeChild(tempInput); // Xóa element tạm

            // Hiển thị thông báo
            Metro.notify.create("Đã sao chép.", "Thông báo", {});
        }


        function show_saochep() {
            Metro.notify.create("Đã sao chép.", "Thông báo", {});
        }

        function showLoadingForImport() {
            var fileInput = document.getElementById('<%= file_import.ClientID %>');
            if(fileInput.files.length === 0) {
                Metro.dialog.create({
                    title: "Lỗi",
                    content: "Vui lòng chọn file Excel trước khi import.",
                    closeButton: true
                });
                return false;
            }
            document.getElementById('full_page_loading').style.display = 'block';
            return true;
        }
    </script>
</asp:Content>
