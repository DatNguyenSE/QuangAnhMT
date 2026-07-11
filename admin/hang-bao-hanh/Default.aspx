<%@ Page Title="Hàng bảo hành" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="admin_hang_bao_hanh_Default" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <%--<asp:UpdatePanel ID="up_daban" runat="server" UpdateMode="Conditional">
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
                                XÁC NHẬN ĐÃ TRẢ HÀNG
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
                                        <label class="fw-600">Ghi chú thêm</label>
                                        <asp:TextBox ID="txt_ghichu_chuagiao" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="mt-6 mb-20">
                                <div style="float: left">
                                </div>
                                <div style="float: right">
                                    <asp:Button ID="but_daban" OnClick="but_daban_Click" runat="server" Text="XÁC NHẬN ĐÃ TRẢ" CssClass="button success" />
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
    </asp:UpdateProgress>--%>

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
                                        <small class="fg-red fw-600">Ngày nhận</small>
                                        <div>
                                            <asp:TextBox ID="txt_ngaynhan" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="cell-lg-4  pl-2-lg pr-2-lg">
                                    <div class="mt-2">
                                        <small class="fg-red fw-600">Ngày hẹn trả</small>
                                        <div>
                                            <asp:TextBox ID="txt_ngayhentra" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="cell-lg-4 pl-2-lg pr-2-lg">
                                    <div class="mt-2">
                                        <small class="fg-red fw-600">Trạng thái phiếu</small>
                                        <asp:DropDownList ID="ddl_trangthai" runat="server" data-role="select">
                                            <asp:ListItem Value="Đang xử lý">Đang xử lý</asp:ListItem>
                                            <asp:ListItem Value="Đã nhận">Đã nhận</asp:ListItem>
                                            <asp:ListItem Value="Đã trả">Đã trả</asp:ListItem>
                                            <asp:ListItem Value="Đã hủy">Đã hủy</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="cell-lg-4 pl-2-lg pr-2-lg">
                                    <div class="mt-2">
                                        <small class="fw-600">Ngày trả thực tế</small>
                                        <div>
                                            <asp:TextBox ID="txt_ngaytrathucte" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="cell-lg-4 pl-2-lg pr-2-lg">
                                    <div class="mt-2">
                                        <small class="fw-600">Công nợ khách</small>
                                        <asp:TextBox ID="txt_congno" Text="0" CssClass="input-small" onfocus="AutoSelect(this)" MaxLength="14" oninput="format_sotien_new(this)" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="cell-lg-4 pl-2-lg pr-2-lg">
                                    <div class="mt-2">
                                        <small class="fw-600">Ghi chú</small>
                                        <asp:TextBox ID="txt_ghichu" CssClass="" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="cell-lg-4 pl-2-lg pr-2-lg">
                                    <div class="mt-4 pt-1">
                                        <asp:CheckBox ID="chk_trehen" runat="server" Text="Trễ hẹn" CssClass="fg-red text-bold" />
                                    </div>
                                </div>
                                <div class="cell-lg-4  pl-2-lg pr-2-lg">
                                     <div class="mt-2">
                                         <small class="fw-600">File Excel Bảo Hành</small>
                                         <input type="file" id="excelFileInput" onchange="uploadExcelFile()" data-role="file" data-button-title="Chọn file" />
                                         <asp:TextBox ID="txt_excel_filepath" runat="server" style="display:none;"></asp:TextBox>
                                         <div id="message_excel" class="fg-red"></div>
                                         <div id="uploadedFilePath_excel" class="fg-green"></div>
                                         <asp:PlaceHolder ID="ph_current_file" runat="server" Visible="false">
                                             <div class="mt-2" style="display: flex; gap: 10px;">
                                                 <asp:HyperLink ID="lnk_download_file" runat="server" Target="_blank" CssClass="button mini success rounded">
                                                     <span class="mif-file-excel pr-1"></span> Tải file Excel hiện tại
                                                 </asp:HyperLink>
                                                 <asp:LinkButton ID="btn_delete_excel" runat="server" CssClass="button mini alert rounded" OnClick="btn_delete_excel_Click" OnClientClick="return confirm('Bạn có chắc chắn muốn xóa file Excel này không?');">
                                                     <span class="mif-bin pr-1"></span> Xóa file
                                                 </asp:LinkButton>
                                             </div>
                                         </asp:PlaceHolder>
                                     </div>
                                 </div>


                            </div>
                            <div class="mt-6 text-right">

                                <asp:Button ID="but_add_edit" runat="server" Text="" CssClass="button success small" OnClick="but_add_edit_Click" />
                            </div>
                            <hr />
                            <asp:PlaceHolder ID="PlaceHolder1" runat="server">
                                <div class="row mt-3">
                                    <div class="cell-lg-12 p-3 bg-light">
                                        <b>Thêm sản phẩm bảo hành vào phiếu</b>

                                        <asp:Panel ID="Panel1" runat="server" DefaultButton="but_add_sp_chitiet">
                                            <div class="row">
                                                <div class="cell-lg-6 pl-2-lg pr-2-lg">
                                                    <div class="mt-2">
                                                        <small class="fg-red fw-600">Tên sản phẩm</small>
                                                        <asp:TextBox ID="txt_name" runat="server" data-role="input" MaxLength="100"></asp:TextBox>
                                                    </div>
                                                    <div class="mt-2">
                                                        <small class="fw-600 fg-red">Ảnh sản phẩm</small>
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
                                                    <div class="mt-2">
                                                        <small class="fw-600 fg-red">Hãng sản phẩm</small>
                                                        <div>
                                                            <asp:DropDownList ID="ddl_hang_add" runat="server" data-role="select" data-filter="true"></asp:DropDownList>
                                                        </div>
                                                    </div>
   
                                                    <div class="mt-2">
                                                        <small class="fw-600 fg-red">Đơn vị tính</small>
                                                        <div>
                                                            <asp:DropDownList ID="ddl_dvt_add" runat="server" data-role="select" data-filter="true"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="mt-2">
                                                        <small class="fg-red fw-600">Model</small>
                                                        <asp:TextBox ID="txt_model" runat="server" data-role="input" MaxLength="100"></asp:TextBox>
                                                    </div>
                                                    <div class="mt-2">
                                                        <small class="fw-600">Thông số kỹ thuật</small>
                                                        <CKEditor:CKEditorControl ID="txt_thongso" runat="server" Height="100px" Width="100%" CustomConfig="/ckeditor/config-basic.js"></CKEditor:CKEditorControl>
                                                        <%--<asp:TextBox ID="txt_thongso" data-role="textarea" runat="server" TextMode="MultiLine"></asp:TextBox>--%>
                                                    </div>
                                                </div>
                                                <div class="cell-lg-3 pl-2-lg pr-2-lg">
                                                    <div class="mt-2">
                                                        <small class="fg-red fw-600">Số lượng</small>
                                                        <asp:TextBox ID="txt_soluong" data-role="spinner" data-buttons-position="right" Text="1" data-min-value="1" data-max-value="999" onfocus="AutoSelect(this)" oninput="format_sotien_new(this)" runat="server"></asp:TextBox>
                                                    </div>
                                                    <div class="mt-2">
                                                        <small class="fg-red fw-600">Số tiền bảo hành</small>
                                                        <asp:TextBox ID="txt_sotien_baohanhg" onfocus="AutoSelect(this)" MaxLength="14" oninput="format_sotien_new(this)" runat="server" data-role="input" Text="0"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="cell-lg-3 pl-2-lg pr-2-lg">
                                                    <div class="mt-2">
                                                        <small class="fw-600">Giảm giá (%)</small>
                                                        <asp:TextBox ID="txt_giamgia_phantram" Text="0" onfocus="AutoSelect(this)" MaxLength="4" runat="server" data-role="input"></asp:TextBox>
                                                    </div>
                                                    <div class="mt-2">
                                                        <small class="fw-600">Seri</small>
                                                        <asp:TextBox ID="txt_seri_add" runat="server" data-role="input"></asp:TextBox>
                                                    </div>
                                                    <div class="mt-2">
                                                        <small class="fw-600">Thời hạn BH</small>
                                                        <asp:TextBox ID="txt_thoihan_baohanh_add" runat="server" data-role="input"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="cell-lg-12 text-right pr-2-lg mt-3 mb-3">
                                                    <asp:Button ID="but_add_sp_chitiet" OnClick="but_add_sp_chitiet_Click" runat="server" CssClass="info small" Text="Thêm vào phiếu bảo hành" />
                                                </div>
                                            </div>
                                        </asp:Panel>
                                    </div>

                                    <div class="cell-lg-12">
                                        <div style="overflow: auto" class=" mt-3">
                                            <div class="row">
                                                <asp:Repeater ID="Repeater2" runat="server">
                                                    <ItemTemplate>
                                                        <span style="display: none">
                                                            <asp:Label ID="lbID_chitiet" runat="server" Text='<%#Eval("id") %>'></asp:Label>
                                                        </span>
                                                        <div class="cell-lg-12 mb-4">
                                                            <div class="border bd-gray p-4 bg-white" style="box-shadow: 0 2px 5px rgba(0,0,0,0.1); border-radius: 4px;">
                                                                <div class="row">
                                                                    <!-- Cột 1: Thông tin Sản phẩm -->
                                                                    <div class="cell-lg-4 border-right bd-lightGray pr-4">
                                                                        <div class="d-flex flex-align-center mb-3">
                                                                            <div class="mr-3" data-role="lightbox" class="c-pointer">
                                                                                <img src='<%#Eval("anh") %>' class="img-cover-vuong border" width="60" height="60" onerror="this.src='/uploads/images/no-image.png'" />
                                                                            </div>
                                                                            <div>
                                                                                <div class="text-bold text-leader"><%#Eval("ten") %></div>

                                                                            </div>
                                                                        </div>
                                                                        
                                                                        <div class="row">
                                                                            <div class="cell-6 mt-2">
                                                                                <small class="fw-600 fg-red">Số lượng nhận</small>
                                                                                <asp:TextBox data-role="input" CssClass="input-small" ID="txt_sl_chitiet" MaxLength="4" runat="server" Text='<%#Eval("soluong","{0:#,##0}") %>'></asp:TextBox>
                                                                            </div>
                                                                            <div class="cell-6 mt-2">
                                                                                <small class="fw-600 fg-red">Giá tiền</small>
                                                                                <asp:TextBox data-role="input" CssClass="input-small" oninput="format_sotien_new(this)" ID="txt_sotien_baohanh1" runat="server" Text='<%#Eval("sotien_baohanh","{0:#,##0}") %>'></asp:TextBox>
                                                                            </div>
                                                                            <div class="cell-6 mt-2">
                                                                                <small class="fw-600 fg-red">Seri</small>
                                                                                <asp:TextBox data-role="input" CssClass="input-small" ID="txt_seri" runat="server" Text='<%#Eval("seri") %>'></asp:TextBox>
                                                                            </div>
                                                                            <div class="cell-6 mt-2">
                                                                                <small class="fw-600">Thời hạn BH</small>
                                                                                <asp:TextBox data-role="input" CssClass="input-small" ID="txt_thoihan_baohanh" runat="server" Text='<%#Eval("thoi_han_baohanh") %>'></asp:TextBox>
                                                                            </div>
                                                                            <div class="cell-12 mt-2">
                                                                                <small class="fw-600">Ghi chú SP (Tình trạng sp)</small>
                                                                                <asp:TextBox data-role="input" CssClass="input-small" ID="txt_ghichu_sanpham" runat="server" Text='<%#Eval("ghichu_sanpham") %>'></asp:TextBox>
                                                                            </div>
                                                                            <div class="cell-6 mt-2">
                                                                                <small class="fw-600">Hãng</small>
                                                                                <asp:TextBox data-role="input" CssClass="input-small" ID="txt_hang_chitiet" runat="server" Text='<%#Eval("TenHang") %>'></asp:TextBox>
                                                                            </div>
                                                                            <div class="cell-6 mt-2">
                                                                                <small class="fw-600">Đơn vị tính</small>
                                                                                <asp:TextBox data-role="input" CssClass="input-small" ID="txt_dvt_chitiet" runat="server" Text='<%#Eval("DVT") %>'></asp:TextBox>
                                                                            </div>
                                                                            <div class="cell-6 mt-2">
                                                                                <small class="fw-600">Model</small>
                                                                                <asp:TextBox data-role="input" CssClass="input-small" ID="txt_model_chitiet" runat="server" Text='<%#Eval("model") %>'></asp:TextBox>
                                                                            </div>
                                                                            <div class="cell-12 mt-2">
                                                                                <small class="fw-600">Giảm giá (%)</small>
                                                                                <asp:TextBox data-role="input" CssClass="input-small" onfocus="AutoSelect(this)" MaxLength="4" ID="txt_giamgia_phantram_chitiet" runat="server" Text='<%#Eval("giamgia_phantram") %>'></asp:TextBox>
                                                                            </div>
                                                                            <div class="cell-12 mt-2">
                                                                                <small class="fw-600">Thông số kỹ thuật</small>
                                                                                <asp:TextBox data-role="textarea" CssClass="input-small" ID="txt_thongso_chitiet" runat="server" TextMode="MultiLine" Text='<%#Eval("thongso_kythuat") %>'></asp:TextBox>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    
                                                                    <!-- Cột 2: Đối tác sửa chữa -->
                                                                    <div class="cell-lg-4 border-right bd-lightGray pr-4 pl-4">
                                                                        <div class="text-bold mb-2">Quy trình Sửa chữa</div>
                                                                        <div class="row">
                                                                            <div class="cell-6 mt-2">
                                                                                <small class="fw-600">Nơi sửa</small>
                                                                                <asp:TextBox data-role="input" CssClass="input-small" ID="txt_noisua" runat="server" Text='<%#Eval("noi_sua") %>'></asp:TextBox>
                                                                            </div>
                                                                            <div class="cell-6 mt-2">
                                                                                <small class="fw-600">Mã ĐT Sửa</small>
                                                                                <asp:TextBox data-role="input" CssClass="input-small" ID="txt_madoitac" runat="server" Text='<%#Eval("ma_doitac_sua") %>'></asp:TextBox>
                                                                            </div>
                                                                            
                                                                            <div class="cell-6 mt-2">
                                                                                <small class="fw-600">Ngày mang đi</small>
                                                                                <asp:TextBox ID="txt_ngaymangsua" runat="server" CssClass="input-small" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true" Text='<%#Eval("ngay_mang_sua", "{0:dd/MM/yyyy}") %>'></asp:TextBox>
                                                                            </div>
                                                                            <div class="cell-6 mt-2">
                                                                                <small class="fw-600">SL mang đi</small>
                                                                                <asp:TextBox data-role="input" CssClass="input-small" ID="txt_slmangsua" runat="server" Text='<%#Eval("sl_mang_sua") %>'></asp:TextBox>
                                                                            </div>
                                                                            
                                                                            <div class="cell-6 mt-2">
                                                                                <small class="fw-600">Ngày sửa về</small>
                                                                                <asp:TextBox ID="txt_ngaysuave" runat="server" CssClass="input-small" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true" Text='<%#Eval("ngay_sua_ve", "{0:dd/MM/yyyy}") %>'></asp:TextBox>
                                                                            </div>
                                                                            <div class="cell-6 mt-2">
                                                                                <small class="fw-600">SL sửa về</small>
                                                                                <asp:TextBox data-role="input" CssClass="input-small" ID="txt_slsuave" runat="server" Text='<%#Eval("sl_sua_ve") %>'></asp:TextBox>
                                                                            </div>
                                                                            <div class="cell-12 mt-2">
                                                                                <small class="fw-600">Chi phí sửa chữa (Công nợ ĐT)</small>
                                                                                <asp:TextBox data-role="input" CssClass="input-small" oninput="format_sotien_new(this)" ID="txt_congnodoitac" runat="server" Text='<%#Eval("congno_doitac","{0:#,##0}") %>'></asp:TextBox>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    
                                                                    <!-- Cột 3: Trả khách hàng -->
                                                                    <div class="cell-lg-4 pl-4">
                                                                        <div class="text-bold mb-2">Bàn giao Khách hàng</div>
                                                                        <div class="row">
                                                                            <div class="cell-6 mt-2">
                                                                                <small class="fw-600">Số phiếu trả</small>
                                                                                <asp:TextBox data-role="input" CssClass="input-small" ID="txt_sophieutra" runat="server" Text='<%#Eval("so_phieu_tra") %>'></asp:TextBox>
                                                                            </div>
                                                                            <div class="cell-6 mt-2">
                                                                                <small class="fw-600">SL trả khách</small>
                                                                                <asp:TextBox data-role="input" CssClass="input-small" ID="txt_sltrakhach" runat="server" Text='<%#Eval("sl_tra_khach") %>'></asp:TextBox>
                                                                            </div>
                                                                            <div class="cell-12 mt-2">
                                                                                <small class="fw-600">Công nợ khách</small>
                                                                                <asp:TextBox data-role="input" CssClass="input-small" oninput="format_sotien_new(this)" ID="txt_congnotrakhach" runat="server" Text='<%#Eval("congno_trakhach","{0:#,##0}") %>'></asp:TextBox>
                                                                            </div>
                                                                            <div class="cell-12 mt-2">
                                                                                <small class="fw-600">Ghi chú trả khách</small>
                                                                                <asp:TextBox data-role="input" CssClass="input-small" ID="txt_ghichutrakhach" runat="server" Text='<%#Eval("ghichu_trakhach") %>'></asp:TextBox>
                                                                            </div>
                                                                            <div class="cell-12 mt-2 text-right">
                                                                                <asp:Button ToolTip="Xóa SP này khỏi phiếu" ID="but_xoachitiet" OnClick="but_xoachitiet_Click" CommandArgument='<%#Eval("id") %>' runat="server" Text="Xóa Sản Phẩm Này" CssClass="button alert small outline" />
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                                
                                                <!-- Bảng TỔNG CỘNG -->
                                                <div class="cell-lg-12">
                                                    <table class="table row-hover table-border cell-border compact striped bg-white">
                                                        <tfoot>
                                                            <tr class="bg-light text-bold">
                                                                <td class="text-right">TỔNG CỘNG</td>
                                                                <td class="text-right text-bold" style="width: 200px;"><%=ViewState["TongSauGiam_ChiTiet"] %></td>
                                                            </tr>
                                                            <%if ((ViewState["pt_giamgiadacbiet"] != null && ViewState["pt_giamgiadacbiet"].ToString() != "0") || (ViewState["giamgia_dacbiet"] != null && ViewState["giamgia_dacbiet"].ToString() != "0"))
                                                                {  %>
                                                            <%if (ViewState["pt_giamgiadacbiet"] != null && ViewState["pt_giamgiadacbiet"].ToString() != "0")
                                                                {  %>
                                                            <tr class="bg-yellow fg-red text-bold">
                                                                <td class="text-right">GIẢM GIÁ ĐẶC BIỆT (%)
                                                                    <div><small><%= ViewState["pt_giamgiadacbiet"]%>%</small></div>
                                                                </td>
                                                                <td class="text-right text-bold">
                                                                   <%= ViewState["giamgia_dacbiet"] != null ? Convert.ToInt64(ViewState["giamgia_dacbiet"]).ToString("#,##0") : "0" %>
                                                                </td>
                                                            </tr>
                                                            <%}
                                                            else
                                                                {  %>
                                                            <tr class="bg-yellow fg-red text-bold">
                                                                <td class="text-right">GIẢM GIÁ ĐẶC BIỆT (số tiền)</td>
                                                                <td class="text-right text-bold">
                                                                    <%= ViewState["giamgia_dacbiet"] != null ? Convert.ToInt64(ViewState["giamgia_dacbiet"]).ToString("#,##0") : "0" %></td>
                                                            </tr>
                                                            <%} %>
                                                            <%if (ViewState["vat_chitiet"] != null && ViewState["vat_chitiet"].ToString() != "0")
                                                                {  %>
                                                            <tr class="text-bold">
                                                                <td class="text-right">VAT (<%= ViewState["vat_chitiet"]%>%)</td>
                                                                <td class="text-right text-bold">
                                                                   <%= ViewState["thanhtien_vat_chitiet"]%>
                                                                </td>
                                                            </tr>
                                                            <%} %>
        
                                                            <tr class="bg-cobalt fg-white text-bold">
                                                                <td class="text-right">GIÁ TRỊ THỰC CỦA ĐƠN HÀNG</td>
                                                                <td class="text-right text-bold">
                                                                    <%= ViewState["donhang_saugiamgia"] != null ? Convert.ToInt64(ViewState["donhang_saugiamgia"]).ToString("#,##0") : "0" %></td>
                                                            </tr>
                                                            <%}
                                                                else
                                                                {  %>
                                                            <%if (ViewState["vat_chitiet"] != null && ViewState["vat_chitiet"].ToString() != "0")
                                                                {  %>
                                                            <tr class="text-bold">
                                                                <td class="text-right">VAT (<%= ViewState["vat_chitiet"]%>%)</td>
                                                                <td class="text-right text-bold">
                                                                     <%= ViewState["thanhtien_vat_chitiet"]%>
                                                                </td>
                                                            </tr>
                                                            <%} %>
        
                                                            <tr class="bg-cobalt fg-white text-bold">
                                                                <td class="text-right">GIÁ TRỊ THỰC CỦA ĐƠN HÀNG</td>
                                                                <td class="text-right text-bold">
                                                                    <%= ViewState["donhang_saugiamgia"] != null ? Convert.ToInt64(ViewState["donhang_saugiamgia"]).ToString("#,##0") : "0" %></td>
                                                            </tr>
                                                            <%} %>
                                                        </tfoot>
                                                    </table>
                                                </div>
                                            </div>
                                        </div>
                                        <%--<div>
                                             <asp:TextBox ID="txt_ghichu_chuagiao" runat="server" data-role="input"></asp:TextBox>
                                        </div>--%>
                                        <div class="text-right">
                                             <asp:Button ID="Button1" runat="server" Text="XÁC NHẬN ĐÃ TRẢ HÀNG" CssClass="button primary small" OnClick="but_daban_Click" />
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

    <asp:Panel ID="pn_import" ClientIDMode="Static" runat="server" style="display:none">
        <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
            <div style='top: 0; left: 0px; margin: 0 auto; max-width: 600px; opacity: 1;'>
                <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                    <a href='#' class='fg-white d-inline' onclick="document.getElementById('pn_import').style.display='none'; return false;" title='Đóng'>
                        <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                    </a>
                </div>
                <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                    <div class="pt-4 text-upper text-bold">Nhập file Excel</div>
                    <hr />
                </div>
            </div>
        </div>
        <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
            <div style='top: 0; left: 0; margin: 0 auto; max-width: 606px; opacity: 1;'>
                <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px; min-height: 300px;">
                    <div class="row">
                        <div class="cell-lg-12">
                            <div class="mt-3">
                                <label class="fw-600 mb-2 d-block">Chọn file Excel (.xls, .xlsx)</label>
                                <input type="file" data-role="file" data-button-title="<span class='mif-folder'></span> Chọn File" data-prepend="<span class='mif-file-excel'></span>" id="fileUploadAjax" accept=".xls,.xlsx" />
                            </div>
                        </div>
                    </div>
                    <div class="mt-6 mb-20 text-right">
                        <button type="button" class="button success rounded" onclick="ajaxImportExcelFile()">
                            Xác nhận
                        </button>
                    </div>
                    
                    <div id="import_loading" style="display: none; position: absolute; top: 0; left: 0; width: 100%; height: 100%; background: rgba(255,255,255,0.9); z-index: 1050; text-align: center; padding-top: 100px;">
                        <div class="activity-atom mx-auto color-style" data-role="activity" data-type="atom" data-style="color"></div>
                        <div class="mt-4 fw-bold fg-dark">Đang xử lý dữ liệu, vui lòng chờ...</div>
                    </div>
                    
                    <script>
                        function ajaxImportExcelFile() {
                            var fileInput = document.getElementById('fileUploadAjax');
                            var files = fileInput ? fileInput.files : null;
                            
                            // Fallback cho file input của Metro UI nếu id='fileUploadAjax' ko lấy dc files do bị thay đổi DOM
                            if (!files || files.length === 0) {
                                // Tìm input type file thật trong panel này
                                var realInput = document.querySelector('#pn_import input[type="file"]');
                                if (realInput && realInput.files) files = realInput.files;
                            }

                            if (!files || files.length === 0) {
                                Metro.dialog.create({
                                    title: "<span class='mif-warning fg-red'></span> Cảnh báo",
                                    content: "Vui lòng chọn một file Excel trước khi Xác nhận!",
                                    clsDialog: "alert"
                                });
                                return;
                            }
                            
                            var formData = new FormData();
                            formData.append("file", files[0]);
                            
                            document.getElementById('import_loading').style.display = 'block';
                            
                            fetch('Default.aspx?action=importExcel', {
                                method: 'POST',
                                body: formData
                            })
                            .then(response => response.json())
                            .then(data => {
                                document.getElementById('import_loading').style.display = 'none';
                                
                                if (data.success) {
                                    Metro.dialog.create({
                                        title: "<span class='mif-checkmark fg-green'></span> Thành công",
                                        content: data.message,
                                        clsDialog: "success",
                                        onClose: function() {
                                            // Refresh data grid
                                            var btnRefresh = document.getElementById('<%= btnRefreshGrid.ClientID %>');
                                            if (btnRefresh) {
                                                btnRefresh.click();
                                            }
                                        }
                                    });
                                } else {
                                    Metro.dialog.create({
                                        title: "<span class='mif-cancel fg-red'></span> Lỗi",
                                        content: data.message,
                                        clsDialog: "alert"
                                    });
                                }
                            })
                            .catch(error => {
                                document.getElementById('import_loading').style.display = 'none';
                                Metro.dialog.create({
                                    title: "Lỗi hệ thống",
                                    content: "Đã xảy ra lỗi kết nối: " + error,
                                    clsDialog: "alert"
                                });
                            });
                        }
                    </script>
                </div>
            </div>
        </div>
    </asp:Panel>


    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Button ID="btnRefreshGrid" runat="server" OnClick="btnRefreshGrid_Click" style="display:none;" />

            <div class="pos-relative pb-11">
                <div id="menutop-tool-bc" style="position: fixed; top: 52px; width: 100%; z-index: 4">
                    <ul class="h-menu bg-white">

                        <li data-role="hint" data-hint-position="top" data-hint-text="Tạo phiếu">
                            <asp:LinkButton ID="but_show_form_add" OnClick="but_show_form_add_Click" runat="server"><span class="mif-plus"></span></asp:LinkButton>
                        </li>
                        <li data-role="hint" data-hint-position="top" data-hint-text="Nhập file Excel">
                            <asp:LinkButton ID="but_show_form_import" OnClientClick="document.getElementById('pn_import').style.display='block'; return false;" runat="server"><span class="mif-file-excel"></span></asp:LinkButton>
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
                        <%-- <li data-role="hint" data-hint-position="top" data-hint-text="Xuất excel">
                            <asp:LinkButton ID="but_show_form_xuat" runat="server" OnClick="but_show_form_xuat_Click"><span class="mif-file-excel"></span></asp:LinkButton>
                        </li>--%>


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
                                        <th style="width: 108px; min-width: 108px;">Ngày</th>
                                        <th style="width: 150px; min-width: 150px;">Khách hàng</th>
                                        <th style="width: 150px; min-width: 150px;">Sản phẩm</th>
                                        <th style="width: 1px; min-width: 1px;">Hạn trả</th>
                                        <th style="width: 1px; min-width: 1px;">Trạng thái</th>
                                        <th style="width: 1px; min-width: 1px;">Tổng tiền</th>
                                        <th style="width: 1px; min-width: 1px;">Tổng giảm</th>
                                        <th style="width: 1px; min-width: 1px;">Tổng sau giảm</th>
                                        <th style="width: 1px; min-width: 1px;">VAT (%)</th>
                                        <th style="width: 1px; min-width: 1px;">Sau thuế</th>
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
                                                    <asp:LinkButton CssClass="fg-white" OnClick="but_show_chinhsua_Click" data-role="hint" data-hint-position="top" data-hint-text="Chi tiết" ID="but_name_1" CommandArgument='<%# Eval("id") %>' runat="server">
                                                        <%#Eval("id") %>
                                                    </asp:LinkButton>
                                                    <asp:PlaceHolder ID="ph_excel_list" runat="server" Visible="false">
                                                        <div>
                                                            <asp:HyperLink ID="lnk_excel_list" Target="_blank" ToolTip="Tải file excel bảo hành" runat="server">
                                                                <span class="mif-file-excel fg-green mif-lg"></span>
                                                            </asp:HyperLink>
                                                        </div>
                                                    </asp:PlaceHolder>
                                                </td>
                                                <td class="checkbox-table">
                                                    <asp:CheckBox ID="checkID" runat="server" onkeypress="if (event.keyCode==13) return false;" />
                                                </td>

                                                <td class="text-left">
                                                    <div><small>Tạo: <%#Eval("ngaytao","{0:dd/MM/yy HH:mm}") %></small></div>
                                                    <div><small class="fg-green">Nhận: <%#Eval("ngaynhan","{0:dd/MM/yy}") %></small></div>
                                                    <div class="fw-600"><%#Eval("HoTenNhanVien") %></div>
                                                </td>
                                                <td class="text-left">
                                                    <div class="fw-600 text-upper"><%#Eval("ten_khachhang") %></div>
                                                    <div><a class="fw-600" title="Nhấn để gọi" href="tel:<%#Eval("sdt_khachhang") %>"><span class="mif-phone pr-1"></span><%#Eval("sdt_khachhang") %></a></div>
                                                    <div class="text-small text-muted"><%#Eval("diachi_khachhang") %></div>
                                                </td>
                                                <td class="text-left">
                                                    <%# GetChiTietSanPham(Eval("id")) %>
                                                </td>

                                                <td>
                                                    <div><%#Eval("NgayHenKhachTra","{0:dd/MM/yyyy}") %></div>
                                                    <asp:PlaceHolder ID="PlaceHolder13" runat="server" Visible='<%# Convert.ToBoolean(Eval("trehen")) %>'>
                                                        <div class="button mini rounded alert ani-flash">Trễ hẹn</div>
                                                    </asp:PlaceHolder>
                                                </td>

                                                <td>
                                                    <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Convert.ToString(Eval("trangthai")).Trim()=="Đang xử lý" || Convert.ToString(Eval("trangthai")).Trim()=="Đã nhận" %>'>
                                                        <div class="button mini rounded warning">Đang xử lý</div>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%#Convert.ToString(Eval("trangthai")).Trim()=="Đã trả" %>'>
                                                        <div class="button mini rounded success">Đã trả</div>
                                                         <div><%#Eval("NgayTra_ThucTe","{0:dd/MM/yyyy}") %></div>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolderSP" runat="server" Visible='<%# !string.IsNullOrEmpty(Convert.ToString(Eval("SoPhieuTra"))) %>'>
                                                        <div class="text-small mt-1 fg-green">Phiếu: <%#Eval("SoPhieuTra") %></div>
                                                    </asp:PlaceHolder>
                                                </td>

                                                <td class="text-right"><%#Eval("TongTien","{0:#,##0}") %>

                                                   
                                                </td>
                                                <td class="text-right">
                                                    <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%#Convert.ToString(Eval("TongGiam"))!="0" %>'>
                                                        <div class="fg-orange"><%#Eval("TongGiam","{0:#,##0}") %></div>
                                                    </asp:PlaceHolder>
                                                </td>
                                                <td class="text-right">
                                                    <%#Eval("TongSauGiam","{0:#,##0}") %>
                                                    <asp:PlaceHolder ID="PlaceHolder6" runat="server" Visible="false">

                                                        <asp:LinkButton CssClass="button mini alert rounded ani-flash" OnClick="but_show_chinhsua_Click" data-role="hint" data-hint-position="top" data-hint-text="Thanh toán công nợ" ID="LinkButton1" CommandArgument='<%# Eval("id") %>' runat="server">
           <%# Eval("congno","{0:#,##0}") %>
                                                        </asp:LinkButton>
                                                    </asp:PlaceHolder>
                                                </td>
                                                 <td class="text-center fg-green"><%#Eval("vat") %>%
                                                      <asp:PlaceHolder ID="PlaceHolder7" runat="server" Visible='<%#Convert.ToString(Eval("vat"))!="0" %>'>
                                                          <div><small><%#Eval("TongTien_VAT","{0:#,##0}") %></small></div>
                                                      </asp:PlaceHolder>
                                                 </td>
                                                 <td class="text-right text-bold"><%#Eval("TongSauThue","{0:#,##0}") %>
                                                     <asp:PlaceHolder ID="PlaceHolder6_new" runat="server" Visible='<%#Convert.ToString(Eval("congno"))!="0" %>'>
                                                         <asp:LinkButton CssClass="button mini alert rounded ani-flash" OnClick="but_show_chinhsua_Click" data-role="hint" data-hint-position="top" data-hint-text="Thanh toán công nợ" ID="LinkButton1_new" CommandArgument='<%# Eval("id") %>' runat="server">
                                                                <%# Eval("congno","{0:#,##0}") %>
                                                         </asp:LinkButton>
                                                     </asp:PlaceHolder>
                                                 </td>
                                                 <td> <%#Eval("ghichu") %></td>

                                                <td style="vertical-align: middle">
                                                    <div class="dropdown-button place-right">
                                                        <button class="button small bg-transparent">
                                                            <span class="mif mif-more-horiz"></span>
                                                        </button>
                                                        <ul class="d-menu place-right" data-role="dropdown">
                                                            <li>
                                                                <asp:LinkButton ID="lnkEdit" OnClick="but_show_chinhsua_Click" CommandArgument='<%# Eval("id") %>' runat="server">Chỉnh sửa</asp:LinkButton>
                                                            </li>
                                                            <li>
                                                                <asp:LinkButton ID="lnkDelete" OnClick="lnk_xoadong_Click" CommandArgument='<%# Eval("id") %>' OnClientClick="return confirm('Bạn có chắc chắn muốn xóa phiếu bảo hành này?');" runat="server">Xóa</asp:LinkButton>
                                                            </li>
                                                        </ul>
                                                    </div>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>

                                </tbody>
                                <tfoot>
                                    <tr class="bg-white  text-bold">
                                        <td></td>
                                        <td colspan="6" class="text-right text-bold">TỔNG</td>
                                        <td class="text-right text-bold"><%=ViewState["TongThanhTien"] %>
                                          
                                        </td>
                                        <td><%if (Convert.ToString(ViewState["TongGiam"]) != "0")
                                                {  %>
                                            <div class="fg-orange"><%=ViewState["TongGiam"] %></div>
                                            <%} %></td>
                                        <td class="text-right"><%=ViewState["TongSauGiam"] %></td>
                                         <td class="fg-green text-right">
                                             <%=ViewState["TongTien_VAT"] %>
                                         </td>
                                         <td class="text-bold text-right"><%=ViewState["TongSauThue"] %></td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                </tfoot>
                            </table>
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
         function uploadExcelFile() {
             var fileInput = document.getElementById("excelFileInput");
             var messageDiv = document.getElementById("message_excel");
             var uploadedFilePathDiv = document.getElementById("uploadedFilePath_excel");

             if (fileInput.files.length > 0) {
                 var file = fileInput.files[0];

                 var allowedExtensions = [".xlsx", ".xls"];
                 var fileExtension = file.name.substr(file.name.lastIndexOf(".")).toLowerCase();
                 if (allowedExtensions.indexOf(fileExtension) === -1) {
                     messageDiv.innerHTML = "Định dạng file không hợp lệ. Chỉ chấp nhận .xlsx, .xls";
                     return;
                 }

                 var maxFileSize = 100 * 1024 * 1024; // 100 MB
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
                         messageDiv.innerHTML = "";
                         uploadedFilePathDiv.innerHTML = "<div><small>Đã chọn: " + file.name + "</small></div>";
                         document.getElementById('<%= txt_excel_filepath.ClientID %>').value = xhr.responseText;
                     } else {
                         messageDiv.innerHTML = xhr.responseText || "Lỗi upload.";
                     }
                 };
                 xhr.send(formData);
             } else {
                 messageDiv.innerHTML = "Vui lòng chọn file.";
             }
         }
     </script>


</asp:Content>
