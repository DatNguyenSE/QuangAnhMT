<%@ Page Language="C#" AutoEventWireup="true" CodeFile="bao-gia.aspx.cs" Inherits="bao_gia" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <%--Basic --%>
    <asp:Literal ID="literal_meta" runat="server"></asp:Literal>
    <meta charset="UTF-8" />
    <meta http-equiv="content-language" content="vi" />
    <%--<meta name="robots" content="index, follow" />
    <meta name="revisit-after" content="1 days" />--%>

    <link rel='icon' href='/uploads/images/logo.png' sizes='16x16' type='image/png' />
    <link rel='icon' href='/uploads/images/logo.png' sizes='32x32' type='image/png' />
    <link rel='icon' href='/uploads/images/logo.png' sizes='48x48' type='image/png' />

    <!-- Apple Touch Icon -->
    <link rel='apple-touch-icon' href='/uploads/images/logo.png' sizes='180x180' />
    <link rel='apple-touch-icon' href='/uploads/images/logo.png' sizes='167x167' />
    <link rel='apple-touch-icon' href='/uploads/images/logo.png' sizes='152x152' />
    <link rel='apple-touch-icon' href='/uploads/images/logo.png' sizes='120x120' />

    <!-- Android Icons -->
    <link rel='icon' href='/uploads/images/logo.png' sizes='192x192' />
    <link rel='icon' href='/uploads/images/logo.png' sizes='512x512' />



    <%--Favicon & icon Mobile & meta--%>
    <asp:Literal ID="literal_fav_icon" runat="server"></asp:Literal>

    <%--css nguồn--%>
    <link href="/Metro-UI-CSS-master/css/metro-all.min.css" rel="stylesheet" />
    <%--dành riêng cho trang login--%>
    <link href="/css/login.css" rel="stylesheet" />
    <%--viết thêm dựa vào metro--%>
    <link href="/css/bcorn-with-metro.css" rel="stylesheet" />
    <%--sửa lại css metro theo ý mình--%>
    <link href="/css/fix-metro.css" rel="stylesheet" />
    <!-- jquery nên để trước các js khác -->
    <%--<script src="/js/jquery-3.7.1.min.js"></script>--%>

    <style>
        a[href^="tel"] {
            color: inherit !important; /* Giữ nguyên màu chữ của parent */
            text-decoration: none !important; /* Xóa gạch chân nếu cần */
        }

        .style1-table .style1-table1 .style1-table2 {
            width: 100%; /* Chiều rộng 100% */
            border-collapse: collapse; /* Loại bỏ khoảng cách giữa các viền */
        }

        .style1-table td, .style1-table th {
            padding: 4px;
            border: 1px solid #000;
            border-left: none;
            border-right: none;
        }

            /* Cột giữa: có border phải */
            .style1-table td:not(:last-child),
            .style1-table th:not(:last-child) {
                border-right: 1px solid #000;
            }

            /* Cột đầu tiên: có thể có border trái nếu cần */
            .style1-table td:not(:first-child),
            .style1-table th:not(:first-child) {
                border-left: 1px solid #000;
            }

        .style1-table1 td, th {
            padding: 4px; /* Padding trong các ô */
            /* border: 1px solid #000; Viền 1px màu đen */
            border-bottom: 1px solid #000;
            border-top: 1px solid #000;
            border-right: 1px solid #000;
        }

        .style1-table2 td, th {
            padding: 4px; /* Padding trong các ô */
            /* border: 1px solid #000; Viền 1px màu đen */
            border-bottom: 1px solid #000;
            border-top: 1px solid #000;
            border-left: 1px solid #000;
        }

        table td, th {
            vertical-align: middle; /* Canh giữa theo chiều dọc */
            /*text-align: center; /* Canh giữa theo chiều ngang (nếu cần) */ */
        }

        .canhgiua-td-bc {
            vertical-align: middle !important;
        }

        @media print {
            @page {
                margin: 20mm;
            }

            body {
                margin: 0;
                padding: 0;
            }

            thead {
                display: table-header-group;
            }


            .page-break {
                page-break-before: always; /* Luôn bắt đầu trang mới trước phần tử có class này */
            }

            .avoid-break {
                page-break-inside: avoid; /* Tránh ngắt trang giữa phần tử này */
            }

            .page-number {
                position: fixed;
                bottom: 0;
                left: 0;
                width: 100%;
                text-align: center;
                font-size: 12px;
            }

                .page-number:after {
                    content: "Trang " counter(page);
                }
        }
    </style>

</head>
<body style="margin: 0!important; padding: 0!important">
    <%--Times New Roman--%>
    <form id="form1" runat="server">
        <div style="/*height: 148mm; */ width: 210mm!important; font-family: Arial!important; overflow: auto; margin: 0 auto; overflow: hidden" class="bg-white">
            <div style="border-left: 1px solid #000; border-right: 1px solid #000;">
                <div id="head" class="p-2 fg-yellow" style="background: linear-gradient(#0036a3,#3d7eff)">
                    <div style="float: left; width: 90px">
                        <img src="/uploads/images/avt.png" width="90" />
                    </div>
                    <div style="float: right; width: calc(100% - 100px)">
                        <div class="text-bold mt-2 " style="font-size: 18px">CÔNG TY TNHH THƯƠNG MẠI DỊCH VỤ ĐẦU TƯ HHM AUDIO</div>
                        <div><small>Mã số thuế: 3703072451</small></div>
                        <div><small><span class="mif-location"></span>Số 77, Đường số 8, Khu dân cư Hiệp Thành 3, Khu phố 7, Hiệp Thành, Thủ Dầu Một, Bình Dương, Việt Nam</small></div>
                        <div><small><span class="mif-phone"></span>0978 308 345 (Mr. Thái An) <span class="mif-earth pl-5"></span>www.hhmaudio.com</small></div>
                    </div>
                    <div class="clr-bc"></div>
                </div>
                <div class="text-center text-bold mt-5 mb-5" style="font-size: 26px">BÁO GIÁ SẢN PHẨM</div>
                <div class="row">
                    <div class="cell-7 pr-5">
                        <table class="style1-table1" style="width: 100%;">
                            <tr>
                                <td colspan="2" class="text-bold text-center bg-light">THÔNG TIN KHÁCH HÀNG</td>
                            </tr>
                            <tr>
                                <td style="width: 130px">Tên khách hàng</td>
                                <td class="fw-600"><%=tenkh %></td>
                            </tr>
                            <tr>
                                <td>Điện thoại</td>
                                <td class="fw-600"><%=sdtkh %></td>
                            </tr>
                            <tr>
                                <td>Địa chỉ</td>
                                <td class="fw-600"><%=diachikh %></td>
                            </tr>
                        </table>
                    </div>
                    <div class="cell-5 pl-5">
                        <table class="style1-table2" style="width: 100%">
                            <tr>
                                <td colspan="2" class="text-bold text-center bg-light">BÁO GIÁ SỐ <%=sobg %></td>
                            </tr>
                            <tr>
                                <td style="width: 116px">Ngày báo giá</td>
                                <td class="fw-600"><%=ngaybg %></td>
                            </tr>
                            <tr>
                                <td>Thời hạn đến</td>
                                <td class="fw-600"><%=hanbg %></td>
                            </tr>
                            <tr>
                                <td>Nhân viên BG</td>
                                <td class="fw-600"><%=nhanvienbg %></td>
                            </tr>
                            <tr>
                                <td>Điện thoại</td>
                                <td class="fw-600"><%=sdtnv %></td>
                            </tr>
                        </table>
                    </div>
                </div>

                <%--   <div style="overflow: auto">--%>
                <table class="style1-table mt-5" style="font-size: 14px !important;">
                    <thead>
                        <tr class="text-bold fg-white bg-darkCobalt">
                            <th>TT</th>
                            <th>ẢNH</th>
                            <th>SẢN PHẨM</th>
                            <th>HÃNG</th>
                            <th>THÔNG SỐ KỸ THUẬT</th>
                            <th>GIÁ</th>
                            <th>ĐVT</th>
                            <th>SL</th>
                            <th>THÀNH TIỀN</th>
                            <th>TỔNG SAU GIẢM</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="Repeater2" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td class="text-center"><%# Container.ItemIndex + 1 %></td>
                                    <td>
                                        <div data-role="lightbox" class="c-pointer">
                                            <img src='<%#Eval("anh") %>' class="img-cover-vuong" width="50" height="50" />
                                        </div>
                                    </td>
                                    <td class="text-left">
                                        <%#Eval("ten_sanpham") %>
                                        <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%#Eval("hangthanhly")=="True" %>'>
                                            <div class="fg-orange">Hàng thanh lý</div>
                                        </asp:PlaceHolder>
                                    </td>
                                    <td class="text-center">
                                        <%#Eval("TenHang") %>
                                    </td>
                                    <td class="text-left">
                                        <small><%#Eval("thongso_kythuat") %></small>
                                    </td>
                                    <td class="text-right"><%#Eval("giaban_taithoidiemnay","{0:#,##0}") %></td>
                                    <td class="text-center">
                                        <%#Eval("DVT") %>
                                    </td>
                                    <td class="text-center text-bold">
                                        <%#Eval("soluong","{0:#,##0}") %>
                                    </td>
                                    <td class="text-right"><%#Eval("thanhtien","{0:#,##0}") %>
                                        <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%#Eval("giamgia_phantram").ToString()!="0" %>'>
                                            <div><small>Giảm <%#Eval("giamgia_phantram","{0:#,##0}") %>%</small></div>
                                        </asp:PlaceHolder>
                                    </td>
                                    <td class="text-right text-bold"><%#Eval("TongSauGiam","{0:#,##0}") %></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        <tr class="bg-light text-bold">
                            <td></td>
                            <td colspan="8" class="text-right">TỔNG CỘNG</td>
                            <td class="text-right text-bold"><%=ViewState["TongSauGiam_ChiTiet"] %></td>
                        </tr>
                        <% if (ViewState["giamgia_dacbiet"] != null && ViewState["giamgia_dacbiet"].ToString() != "0")
                            { %>
                        <tr class="bg-yellow fg-red text-bold">
                            <td></td>
                            <td colspan="8" class="text-right">GIẢM GIÁ ĐẶC BIỆT</td>
                            <td class="text-right text-bold">
                                <%= Convert.ToInt64(ViewState["giamgia_dacbiet"]).ToString("#,##0") %>
                            </td>
                        </tr>
                        <% if (ViewState["vat_chitiet"] != null && ViewState["vat_chitiet"].ToString() != "0")
                            { %>
                        <tr class="text-bold">
                            <td></td>
                            <td colspan="8" class="text-right">VAT</td>
                            <td class="text-right text-bold">
                                <%= ViewState["vat_chitiet"] %>%>
                             <div><small><%= ViewState["thanhtien_vat_chitiet"] ?? "" %></small></div>
                            </td>
                        </tr>
                        <% } %>

                        <tr class="bg-cobalt fg-white text-bold">
                            <td></td>
                            <td colspan="8" class="text-right">GIÁ TRỊ THỰC CỦA ĐƠN HÀNG</td>
                            <td class="text-right text-bold">
                                <%= ViewState["donhang_saugiamgia"] != null ? Convert.ToInt64(ViewState["donhang_saugiamgia"]).ToString("#,##0") : "0" %>
                            </td>
                        </tr>
                        <% }
                            else
                            { %>

                        <% if (ViewState["vat_chitiet"] != null && ViewState["vat_chitiet"].ToString() != "0")
                            { %>
                        <tr class="text-bold">
                            <td></td>
                            <td colspan="8" class="text-right">VAT</td>
                            <td class="text-right text-bold">
                                <%= ViewState["vat_chitiet"] %>%
                         <div><small><%= ViewState["thanhtien_vat_chitiet"] ?? "" %></small></div>
                            </td>
                        </tr>
                        <% } %>

                        <tr class="bg-cobalt fg-white text-bold">
                            <td></td>
                            <td colspan="8" class="text-right">GIÁ TRỊ THỰC CỦA ĐƠN HÀNG</td>
                            <td class="text-right text-bold">
                                <%= ViewState["donhang_saugiamgia"] != null ? Convert.ToInt64(ViewState["donhang_saugiamgia"]).ToString("#,##0") : "0" %>
                            </td>
                        </tr>
                        <% } %>
                    </tbody>
                </table>
            </div>
            <div class="row mt-6 mb-10">
                <div class="cell-8">
                    <%if (ViewState["vat_chitiet"] != null && ViewState["vat_chitiet"].ToString() == "0")
                        {

                    %>
                    <div class="text-bold">CÔNG TY TNHH THƯƠNG MẠI DỊCH VỤ ĐẦU TƯ HHM AUDIO</div>
                    <div><small><b>Địa chỉ:</b> Số 77, Đường số 8, Khu dân cư Hiệp Thành 3,Khu phố 7, Phường Hiệp Thành, Thành phố Thủ Dầu Một, Tỉnh Bình Dương, Việt Nam</small></div>
                    <div class="info-ck" style="display: flex; align-items: center; gap: 20px;">
                        <div style="flex: 1;">
                            <div class="mt-3 text-bold">STK: 0421000502463 | Vietcombank | Thái Đình An</div>
                            <div class="mt-3">
                                <i>Rất mong được sự hợp tác của Quý khách hàng.</i>
                            </div>
                            <div class="text-bold">Trân trọng!</div>
                        </div>
                        <div style="flex: 0 0 auto; width: 110px; height: 110px">
                            <asp:Image ID="imgQRCode" runat="server" Visible="false" CssClass="qr-image" />
                        </div>
                    </div>



                    <%}
                        else
                        {  %>
                    <div class="text-bold">CÔNG TY TNHH THƯƠNG MẠI DỊCH VỤ ĐẦU TƯ HHM AUDIO</div>
                    <div><small><b>Địa chỉ:</b> Số 77, Đường số 8, Khu dân cư Hiệp Thành 3,Khu phố 7, Phường Hiệp Thành, Thành phố Thủ Dầu Một, Tỉnh Bình Dương, Việt Nam</small></div>
                    <div class="info-ck" style="display: flex; align-items: center; gap: 20px;">
                        <div style="flex: 1;">
                            <div class="mt-3 text-bold">STK CÔNG TY: 1030308345 | Vietcombank</div>
                            <div class="mt-3">
                                <i>Rất mong được sự hợp tác của Quý khách hàng.</i>
                            </div>
                            <div class="text-bold">Trân trọng!</div>
                        </div>
                        <div style="flex: 0 0 auto; width: 110px; height: 110px">
                            <asp:Image ID="imgQRCode1" runat="server" Visible="false" CssClass="qr-image" />
                        </div>
                    </div>
                    <%} %>
                </div>
                <div class="cell-4 text-center text-bold">
                    GIÁM ĐỐC
                    <div>
                        <img src="/uploads/images/CON-DAU-HHM-AUDIO.PNG" width="120" />
                    </div>
                    THÁI ĐÌNH AN
                </div>
            </div>

        </div>

    </form>
    <script src="/Metro-UI-CSS-master/js/metro.min.js"></script>
    <%=notifi %>
</body>
</html>
