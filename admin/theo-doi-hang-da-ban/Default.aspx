<%@ Page Title="Theo dõi hàng đã bán" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="admin_theo_doi_hang_da_ban_Default" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .product-img {
            width: 50px;
            height: 50px;
            object-fit: cover;
            border-radius: 4px;
            border: 1px solid #ddd;
        }
        .modal-label {
            font-weight: 600;
            color: #555;
            margin-top: 10px;
            display: block;
            font-size: 13px;
        }
        .modal-value {
            font-size: 14px;
            color: #000;
            padding: 5px 0;
            border-bottom: 1px solid #f5f5f5;
        }
        .section-title {
            font-size: 15px;
            font-weight: bold;
            color: #1d4ed8;
            margin-top: 20px;
            margin-bottom: 10px;
            border-left: 4px solid #1d4ed8;
            padding-left: 8px;
        }
        .warranty-cell {
            line-height: 1.5;
            white-space: nowrap;
        }
        .warranty-months {
            font-size: 15px;
            font-weight: 700;
            color: #1d4ed8;
        }
        .warranty-expiry {
            display: block;
            margin-top: 3px;
            font-size: 13px;
            color: #555;
        }
        .warranty-status {
            display: block;
            margin-top: 3px;
            font-size: 13px;
            font-weight: 700;
            color: #dc2626;
        }
        .warranty-unknown {
            font-size: 13px;
            color: #000;
        }
        .warranty-filter-button {
            margin: 2px 3px;
            padding: 8px 12px;
            font-size: 14px;
            font-weight: 600;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <!-- Detail Modal Popup -->
            <asp:Panel ID="pn_detail" runat="server" Visible="false">
                <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                    <div style='top: 0; left: 0px; margin: 0 auto; max-width: 1150px; opacity: 1;'>
                        <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                            <a href='#' class='fg-white d-inline' runat="server" id="close_detail" onserverclick="but_close_detail_Click" title='Đóng'>
                                <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                            </a>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                CHI TIẾT SẢN PHẨM ĐÃ BÁN
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>
                <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                    <div style='top: 0; left: 0; margin: 0 auto; max-width: 1156px; opacity: 1;'>
                        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px">
                            <div class="row">
                                <!-- Left side: Product & Salesperson Info -->
                                <div class="cell-md-5 pr-2-md">
                                    <!-- Product Card -->
                                    <div class="border bd-default p-4 mb-4 bg-light" style="border-radius: 8px;">
                                        <div class="text-bold fg-darkBlue mb-3" style="font-size: 15px; border-bottom: 2px solid #1d4ed8; padding-bottom: 5px;">
                                            <span class="mif-open-book mr-1"></span> THÔNG TIN SẢN PHẨM
                                        </div>
                                        <div class="text-center mb-3">
                                            <asp:Image ID="img_detail_sp" runat="server" CssClass="img-fluid border" style="max-height: 150px; object-fit: contain; border-radius: 4px;" />
                                        </div>
                                        <div class="mt-2">
                                            <small class="modal-label">Tên sản phẩm</small>
                                            <div class="modal-value text-bold"><asp:Label ID="lbl_detail_tensp" runat="server"></asp:Label></div>
                                        </div>
                                         <div class="mt-1" style="border-bottom: 1px solid #f5f5f5; padding-bottom: 4px;">
                                             <small class="fg-dark fw-600" style="display: inline-block; width: 130px;">Model:</small>
                                             <span class="ml-1"><asp:Label ID="lbl_detail_model" runat="server"></asp:Label></span>
                                         </div>
                                         <div class="mt-1" style="border-bottom: 1px solid #f5f5f5; padding-bottom: 4px;">
                                             <small class="fg-dark fw-600" style="display: inline-block; width: 130px;">Số Seri (Serial):</small>
                                             <span class="ml-1"><asp:Label ID="lbl_detail_seri" runat="server"></asp:Label></span>
                                         </div>
                                         <div class="mt-1" style="border-bottom: 1px solid #f5f5f5; padding-bottom: 4px;">
                                             <small class="fg-dark fw-600" style="display: inline-block; width: 130px;">Mã KH (Kích hoạt):</small>
                                             <span class="ml-1"><asp:Label ID="lbl_detail_makichhoat" runat="server"></asp:Label></span>
                                         </div>
                                         <div class="mt-1" style="border-bottom: 1px solid #f5f5f5; padding-bottom: 4px;">
                                             <small class="fg-dark fw-600" style="display: inline-block; width: 130px;">Bảo hành (tháng):</small>
                                             <span class="ml-1"><asp:Label ID="lbl_detail_thangbaohanh" runat="server"></asp:Label></span>
                                         </div>
                                         <div class="mt-1" style="border-bottom: 1px solid #f5f5f5; padding-bottom: 4px;">
                                             <small class="fg-dark fw-600" style="display: inline-block; width: 130px;">Seri đổi lần 1:</small>
                                             <span class="ml-1"><asp:Label ID="lbl_detail_seri_do_l1" runat="server"></asp:Label></span>
                                         </div>
                                         <div class="mt-1" style="border-bottom: 1px solid #f5f5f5; padding-bottom: 4px;">
                                             <small class="fg-dark fw-600" style="display: inline-block; width: 130px;">Mã KH đổi lần 1:</small>
                                             <span class="ml-1"><asp:Label ID="lbl_detail_id_khachhang_do_l1" runat="server"></asp:Label></span>
                                         </div>
                                         <div class="mt-1" style="border-bottom: 1px solid #f5f5f5; padding-bottom: 4px;">
                                             <small class="fg-dark fw-600" style="display: inline-block; width: 130px;">Ngày đổi lần 1:</small>
                                             <span class="ml-1"><asp:Label ID="lbl_detail_ngaydo_l1" runat="server"></asp:Label></span>
                                         </div>
                                         <div class="mt-1" style="border-bottom: 1px solid #f5f5f5; padding-bottom: 4px;">
                                             <small class="fg-dark fw-600" style="display: inline-block; width: 130px;">Seri đổi lần 2:</small>
                                             <span class="ml-1"><asp:Label ID="lbl_detail_seri_do_l2" runat="server"></asp:Label></span>
                                         </div>
                                         <div class="mt-1" style="border-bottom: 1px solid #f5f5f5; padding-bottom: 4px;">
                                             <small class="fg-dark fw-600" style="display: inline-block; width: 130px;">Mã KH đổi lần 2:</small>
                                             <span class="ml-1"><asp:Label ID="lbl_detail_id_khachhang_do_l2" runat="server"></asp:Label></span>
                                         </div>
                                         <div class="mt-1" style="border-bottom: 1px solid #f5f5f5; padding-bottom: 4px;">
                                             <small class="fg-dark fw-600" style="display: inline-block; width: 130px;">Ngày đổi lần 2:</small>
                                             <span class="ml-1"><asp:Label ID="lbl_detail_ngaydo_l2" runat="server"></asp:Label></span>
                                         </div>
                                         <div class="mt-2">
                                             <small class="fg-dark fw-600 d-block mb-1">Mô tả</small>
                                             <div class="modal-value text-muted" style="font-size: 12px;"><asp:Label ID="lbl_detail_mota" runat="server"></asp:Label></div>
                                         </div>
                                         <div class="mt-2">
                                             <small class="fg-dark fw-600 d-block mb-1">Thông số kỹ thuật</small>
                                             <div class="modal-value text-muted" style="font-size: 12px; border-bottom: none;"><asp:Label ID="lbl_detail_thongso" runat="server"></asp:Label></div>
                                        </div>
                                    </div>

                                    <!-- Salesperson Card -->
                                    <div class="border bd-default p-4 bg-light" style="border-radius: 8px;">
                                        <div class="text-bold fg-darkBlue mb-3" style="font-size: 15px; border-bottom: 2px solid #1d4ed8; padding-bottom: 5px;">
                                            <span class="mif-user mr-1"></span> NHÂN VIÊN & DOANH SỐ
                                        </div>
                                        <div class="mt-2">
                                            <small class="modal-label">Người bán (Họ tên)</small>
                                            <div class="modal-value text-bold"><asp:Label ID="lbl_detail_nguoiban" runat="server"></asp:Label></div>
                                        </div>
                                        <div class="mt-2">
                                            <small class="modal-label">Phần trăm doanh số (%)</small>
                                            <div class="modal-value"><asp:Label ID="lbl_detail_phantramdoanhso" runat="server"></asp:Label>%</div>
                                        </div>
                                        <div class="mt-2">
                                            <small class="modal-label">Thưởng doanh số tương ứng</small>
                                            <div class="modal-value text-bold fg-green" style="font-size: 16px; border-bottom: none;"><asp:Label ID="lbl_detail_thuongdoanhso" runat="server"></asp:Label> VNĐ</div>
                                        </div>
                                    </div>
                                </div>

                                <!-- Right side: Customer & Financial details -->
                                <div class="cell-md-7 pl-2-md">
                                    <!-- Customer & Order Header Info -->
                                    <div class="border bd-default p-4 mb-4 bg-light" style="border-radius: 8px;">
                                        <div class="text-bold fg-darkBlue mb-3" style="font-size: 15px; border-bottom: 2px solid #1d4ed8; padding-bottom: 5px;">
                                            <span class="mif-assignment mr-1"></span> GIAO DỊCH & KHÁCH HÀNG
                                        </div>
                                        <div class="row">
                                            <div class="cell-sm-6">
                                                <small class="modal-label">Khách hàng</small>
                                                <div class="modal-value text-bold"><asp:Label ID="lbl_detail_tenkh" runat="server"></asp:Label></div>
                                            </div>
                                            <div class="cell-sm-6">
                                                <small class="modal-label">Số điện thoại</small>
                                                <div class="modal-value"><asp:Label ID="lbl_detail_sdtkh" runat="server"></asp:Label></div>
                                            </div>
                                            <div class="cell-12 mt-2">
                                                <small class="modal-label">Địa chỉ nhận hàng</small>
                                                <div class="modal-value"><asp:Label ID="lbl_detail_diachikh" runat="server"></asp:Label></div>
                                            </div>
                                            <div class="cell-sm-4 mt-2">
                                                <small class="modal-label">Mã Báo Giá</small>
                                                <div class="modal-value" style="border-bottom: none;">
                                                    <a href='/admin/quan-ly-bao-gia/default.aspx' target="_blank" class="fg-blue text-bold text-underline">
                                                        #<asp:Label ID="lbl_detail_mabg" runat="server"></asp:Label>
                                                    </a>
                                                </div>
                                            </div>
                                            <div class="cell-sm-5 mt-2">
                                                <small class="modal-label">Ngày bán (Ký HĐ)</small>
                                                <div class="modal-value" style="border-bottom: none;"><asp:Label ID="lbl_detail_ngayban" runat="server"></asp:Label></div>
                                            </div>
                                            <div class="cell-sm-3 mt-2">
                                                <small class="modal-label">Số lượng</small>
                                                <div class="modal-value text-bold" style="border-bottom: none;"><asp:Label ID="lbl_detail_soluong" runat="server"></asp:Label> cái</div>
                                            </div>
                                        </div>
                                    </div>

                                    <!-- Financial Breakdown Card -->
                                    <div class="border bd-default p-4 bg-light" style="border-radius: 8px;">
                                        <div class="text-bold fg-darkBlue mb-3" style="font-size: 15px; border-bottom: 2px solid #1d4ed8; padding-bottom: 5px;">
                                            <span class="mif-coins mr-1"></span> CHI TIẾT TÀI CHÍNH & PHÂN BỔ
                                        </div>
                                        
                                        <div class="bg-white p-3 border bd-default mb-3" style="border-radius: 4px;">
                                            <div class="text-bold fg-emerald mb-2" style="font-size: 13px;">1. Tính toán cho sản phẩm này</div>
                                            <table class="table table-border cell-border compact">
                                                <tbody>
                                                    <tr>
                                                        <td>Thành tiền gốc</td>
                                                        <td class="text-right"><asp:Label ID="lbl_detail_thanhtiengoc_sp" runat="server"></asp:Label> đ</td>
                                                    </tr>
                                                    <tr>
                                                        <td>Giảm giá sản phẩm (<asp:Label ID="lbl_detail_giamgia_phantram_sp" runat="server"></asp:Label>%)</td>
                                                        <td class="text-right fg-orange">- <asp:Label ID="lbl_detail_giamgia_sp" runat="server"></asp:Label> đ</td>
                                                    </tr>
                                                    <tr>
                                                        <td class="text-bold">Thành tiền sau giảm</td>
                                                        <td class="text-right text-bold"><asp:Label ID="lbl_detail_saugiam_sp" runat="server"></asp:Label> đ</td>
                                                    </tr>
                                                    <tr>
                                                        <td>Giảm đặc biệt đơn (<asp:Label ID="lbl_detail_giamgiadacbiet_phantram_sp" runat="server"></asp:Label>%)</td>
                                                        <td class="text-right fg-orange">- <asp:Label ID="lbl_detail_giamgiadacbiet_phanbo" runat="server"></asp:Label> đ</td>
                                                    </tr>
                                                    <tr>
                                                        <td>Thuế VAT phân bổ (<asp:Label ID="lbl_detail_vat_phantram" runat="server"></asp:Label>%)</td>
                                                        <td class="text-right fg-cobalt">+ <asp:Label ID="lbl_detail_vat_phanbo" runat="server"></asp:Label> đ</td>
                                                    </tr>
                                                    <tr class="bg-lightInfo">
                                                        <td class="text-bold fg-red">TỔNG TIỀN CUỐI SẢN PHẨM</td>
                                                        <td class="text-right text-bold fg-red" style="font-size: 15px;"><asp:Label ID="lbl_detail_tongtiencuoi_sp" runat="server"></asp:Label> đ</td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </div>

                                        <div class="bg-white p-3 border bd-default" style="border-radius: 4px;">
                                            <div class="text-bold fg-cobalt mb-2" style="font-size: 13px;">2. Đối chiếu toàn bộ đơn hàng (Báo giá)</div>
                                            <table class="table table-border cell-border compact">
                                                <tbody>
                                                    <tr>
                                                        <td>Tổng sau giảm toàn đơn</td>
                                                        <td class="text-right"><asp:Label ID="lbl_detail_tongdonhang_saugiam" runat="server"></asp:Label> đ</td>
                                                    </tr>
                                                    <tr>
                                                        <td>Giảm đặc biệt đơn (<asp:Label ID="lbl_detail_tongdonhang_pt_giamgiadacbiet" runat="server"></asp:Label>%)</td>
                                                        <td class="text-right fg-orange">- <asp:Label ID="lbl_detail_tongdonhang_giamgiadacbiet" runat="server"></asp:Label> đ</td>
                                                    </tr>
                                                    <tr>
                                                        <td>Tổng thuế VAT đơn (<asp:Label ID="lbl_detail_tongdonhang_vat_phantram" runat="server"></asp:Label>%)</td>
                                                        <td class="text-right fg-cobalt">+ <asp:Label ID="lbl_detail_tongdonhang_vat" runat="server"></asp:Label> đ</td>
                                                    </tr>
                                                    <tr class="bg-lightSuccess">
                                                        <td class="text-bold fg-dark">GIÁ TRỊ THỰC CỦA ĐƠN HÀNG</td>
                                                        <td class="text-right text-bold fg-dark" style="font-size: 15px;"><asp:Label ID="lbl_detail_tongdonhang_giatrithuc" runat="server"></asp:Label> đ</td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                            
                                            <div class="text-bold fg-dark mb-1 mt-3" style="font-size: 12px; border-top: 1px dashed #ccc; padding-top: 10px;">
                                                 <span class="mif-list mr-1"></span> Các sản phẩm trong báo giá:
                                            </div>
                                            <table class="table table-border cell-border compact mb-0" style="font-size: 12px; width: 100%;">
                                                <thead>
                                                    <tr class="bg-grayLighter">
                                                        <th>Tên sản phẩm</th>
                                                        <th style="width: 60px; text-align: center;">SL</th>
                                                        <th style="width: 120px; text-align: right;">Tiền cuối</th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <asp:Repeater ID="rpt_order_items" runat="server">
                                                        <ItemTemplate>
                                                            <tr>
                                                                <td><%# Eval("productName") %></td>
                                                                <td class="text-center font-bold"><%# Eval("quantity") %></td>
                                                                <td class="text-right text-bold fg-red"><%# Convert.ToInt64(Eval("totalPrice")).ToString("#,##0") %> đ</td>
                                                            </tr>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </tbody>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="mb-20 mt-8 text-right">
                                <asp:Button ID="btn_close_modal" runat="server" Text="Đóng" CssClass="button secondary" OnClick="but_close_detail_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- Main Page Layout -->
            <div class="pos-relative pb-11">
                <div id="menutop-tool-bc" style="position: fixed; top: 52px; width: 100%; z-index: 4">
                    <ul class="h-menu bg-white">
                        <li style="display:none;">
                            <div class="p-2 d-flex flex-align-center">
                                <small class="pr-2">Hiển thị:</small>
                                <asp:TextBox ID="txt_show" Width="50" MaxLength="5" runat="server" data-role="input" AutoPostBack="true" OnTextChanged="txt_show_TextChanged" Text="30" style="display:inline-block; height: 32px;"></asp:TextBox>
                            </div>
                        </li>
                        <li class="bd-gray border bd-default mt-2" style="height: 24px; display:none;"></li>
                        <li>
                            <asp:LinkButton ID="btn_warranty" runat="server" CssClass="button success warranty-filter-button" OnClick="btn_warranty_Click">Hàng còn bảo hành</asp:LinkButton>
                        </li>
                        <li>
                            <asp:LinkButton ID="btn_all" runat="server" CssClass="button secondary warranty-filter-button" OnClick="btn_all_Click">Tất cả</asp:LinkButton>
                        </li>
                        <li>
                            <a>
                                <small>
                                    <asp:Label ID="lbl_page_info" runat="server" Text=""></asp:Label>
                                </small>
                            </a>
                        </li>
                        <li data-role="hint" data-hint-position="top" data-hint-text="Lùi">
                            <asp:LinkButton ID="btn_prev" OnClick="btn_prev_Click" runat="server"><span class="mif-chevron-left"></span></asp:LinkButton>
                        </li>
                        <li data-role="hint" data-hint-position="top" data-hint-text="Tới">
                            <asp:LinkButton ID="btn_next" OnClick="btn_next_Click" runat="server"><span class="mif-chevron-right"></span></asp:LinkButton>
                        </li>
                    </ul>
                </div>
                <div id="timkiem-fixtop-bc" style="position: fixed; right: 10px; top: 58px; width: 240px; z-index: 4" class="d-none d-block-sm">
                    <asp:TextBox MaxLength="50" data-prepend="<span class='mif mif-search'></span>" ID="txt_timkiem" runat="server" placeholder="Tìm sản phẩm, seri, khách..." data-role="input" CssClass="input-small" AutoPostBack="true" OnTextChanged="txt_timkiem_TextChanged"></asp:TextBox>
                </div>

                <div style="padding-top: 60px;"></div>

                <div class="p-3">
                    <div class="d-none-sm d-block mb-3">
                        <asp:TextBox MaxLength="50" data-prepend="<span class='mif mif-search'></span>" ID="txt_timkiem1" runat="server" placeholder="Tìm sản phẩm, seri, khách..." data-role="input" AutoPostBack="true" OnTextChanged="txt_timkiem_TextChanged"></asp:TextBox>
                    </div>
                    <div class="row">
                        <div class="cell-lg-12">
                            <div class="bcorn-fix-title-table-container">
                                <table class="bcorn-fix-title-table">
                                    <thead>
                                        <tr>
                                            <th style="width: 120px;">Ngày bán</th>
                                            <th style="width: 80px;">Ảnh</th>
                                            <th style="width: 130px;">Khách hàng</th>
                                            <th style="width: 230px; min-width: 230px;">Sản phẩm</th>
                                            <th style="width: 120px;">Số Seri</th>
                                            <th style="width: 100px; text-align: center;">Mã KH</th>
                                            <th style="width: 90px; text-align: center;">SL Bán</th>
                                            <th style="width: 120px;">Giá bán</th>
                                            <th style="width: 120px;">Tổng tiền cuối</th>
                                            <th style="width: 150px;">Bảo hành</th>
                                            <th style="width: 70px;">ID<br />Báo giá</th>
                                            <th style="width: 100px; text-align: center;">Hành động</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound">
                                            <ItemTemplate>
                                                <tr>
                                                    <td><%# Eval("ngayban", "{0:dd/MM/yyyy}") %></td>
                                                    <td class="text-center">
                                                        <img src='<%# string.IsNullOrEmpty(Convert.ToString(Eval("productImage"))) ? "/uploads/images/no-image.png" : Eval("productImage") %>' class="product-img" />
                                                    </td>
                                                     <td style="min-width: 130px;">
                                                        <div class="fw-600"><%# Eval("tenKhachHang") %></div>
                                                        <small class="text-muted"><%# Eval("sdtKhachHang") %></small>
                                                     </td>
                                                    <td style="min-width: 230px;">
                                                        <div class="fw-600"><%# Eval("productName") %></div>
                                                    </td>
                                                    <td><%# Eval("productSerial") %></td>
                                                    <td class="text-center"><%# Eval("maKH") %></td>
                                                    <td class="text-center font-bold"><%# Eval("quantity") %></td>
                                                    <td><%# Convert.ToInt64(Eval("price")).ToString("#,##0") %></td>
                                                    <td class="font-bold fg-red"><%# Convert.ToInt64(Eval("totalPrice")).ToString("#,##0") %></td>
                                                     <td class="warranty-cell">
                                                         <asp:Label ID="lbl_warranty_unknown" runat="server" CssClass="warranty-unknown" Text="Không rõ" Visible='<%# string.IsNullOrWhiteSpace(Convert.ToString(Eval("thangBaoHanh"))) %>'></asp:Label>
                                                         <asp:Panel ID="pn_warranty_details" runat="server" Visible='<%# !string.IsNullOrWhiteSpace(Convert.ToString(Eval("thangBaoHanh"))) %>'>
                                                             <div class="warranty-months"><%# Eval("thangBaoHanh") + " tháng" %></div>
                                                             <div class="warranty-expiry">Hạn ngày: <%# Eval("warrantyExpiry") == null ? "Không rõ" : Convert.ToDateTime(Eval("warrantyExpiry")).ToString("dd/MM/yyyy") %></div>
                                                             <asp:Label ID="lbl_warranty_expired" runat="server" CssClass="warranty-status" Text="Hết hạn bảo hành" Visible='<%# Convert.ToBoolean(Eval("warrantyExpired")) %>'></asp:Label>
                                                         </asp:Panel>
                                                     </td>
                                                     <td class="text-center"><%# Eval("baogiaId") %></td>
                                                    <td class="text-center">
                                                        <asp:LinkButton ID="btn_view" runat="server" CssClass="button mini primary rounded" CommandArgument='<%# Eval("baogiaId") + "|" + Eval("productId") %>' OnClick="btn_view_Click">
                                                            Xem chi tiết
                                                        </asp:LinkButton>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                        <tr ID="tr_empty" runat="server" visible="false">
                                            <td colspan="11" class="text-center p-4 fg-gray">
                                                Không tìm thấy sản phẩm đã bán nào.
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>


                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="up_main">
        <ProgressTemplate>
            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                <div style="padding-top: 45vh;">
                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>
