<%@ Page Title="Lịch sử nhập xuất" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="lich-su-nhap-xuat.aspx.cs" Inherits="admin_quan_ly_kho_lich_su_nhap_xuat" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">


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
                                </div>
                                <div class="cell-lg-6 pl-4-lg">

                                    <div class="mt-3">
                                        <label class="fw-600 mt-3">Từ ngày</label>
                                        <asp:TextBox ID="txt_tungay" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false"></asp:TextBox>
                                    </div>
                                    <div class=" mt-3">
                                        <label class="fw-600 mt-3">Đến ngày</label>
                                        <asp:TextBox ID="txt_denngay" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false"></asp:TextBox>
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
        <Triggers>
            <%--<asp:AsyncPostBackTrigger ControlID="but_add" EventName="Click" />--%>
        </Triggers>
        <ContentTemplate>

            <div class="pos-relative pb-11">
                <div id="menutop-tool-bc" style="position: fixed; top: 52px; width: 100%; z-index: 4">
                    <ul class="h-menu bg-white">

                        <%--<li data-role="hint" data-hint-position="top" data-hint-text="Thêm sản phẩm">
                         <asp:LinkButton ID="but_show_form_add" OnClick="but_show_form_add_Click" runat="server"><span class="mif-plus"></span></asp:LinkButton>
                     </li>--%>
                        <%--<li data-role="hint" data-hint-position="top" data-hint-text="Lưu">
                         <asp:LinkButton ID="but_save" OnClick="but_save_Click" runat="server"><span class="mif-floppy-disk"></span></asp:LinkButton>
                     </li>--%>



                        <li data-role="hint" data-hint-position="top" data-hint-text="Lọc">
                            <asp:LinkButton ID="but_show_form_loc" runat="server" OnClick="but_show_form_loc_Click"><span class="mif-filter"></span></asp:LinkButton>
                        </li>
                        <li data-role="hint" data-hint-position="top" data-hint-text="Xóa">
                            <asp:LinkButton ID="but_xoa" OnClick="but_xoa_Click" runat="server"><span class="mif-bin"></span></asp:LinkButton>
                        </li>
                        <%--<li data-role="hint" data-hint-position="top" data-hint-text="Xuất excel">
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
                                        <th style="width: 140px; min-width: 140px;">Ngày<br />
                                            Nhân viên</th>
                                        <th style="width: 50px; min-width: 50px;">Ảnh</th>
                                        <th style="width: 160px; min-width: 160px;">Sản phẩm</th>
                                        <th style="width: 1px; min-width: 1px;">VAT</th>
                                        <th style="width: 1px; min-width: 1px;">ĐVT</th>
                                        <th style="width: 1px; min-width: 1px;">Tồn đầu</th>
                                        <th style="width: 130px; min-width: 130px;">SL</th>
                                        <th style="width: 1px; min-width: 1px;">Tồn cuối</th>
                                        <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible="false">
                                            <th style="width: 90px; min-width: 90px;">Giá<br />
                                                Thành tiền</th>
                                        </asp:PlaceHolder>


                                        <th style="width: 1px; min-width: 1px;">Hãng</th>
                                        <th style="width: 1px; min-width: 1px;">Model</th>
                                        <th style="width: 1px; min-width: 1px;">Nhóm</th>


                                        <%--<th style="width: 300px; min-width: 300px;">Thông số</th>--%>
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
                                                    <%#Eval("id") %>
                                                </td>
                                                <%--<td class="text-center"><%# Container.ItemIndex + 1 %></td>--%>
                                                <td class="checkbox-table">
                                                    <%--data-role="checkbox" data-style="2"--%>
                                                    <%--<input type="checkbox" onkeypress="if (event.keyCode==13) return false;" name="check_<%#Eval("id").ToString() %>">--%>
                                                    <asp:CheckBox ID="checkID" runat="server" onkeypress="if (event.keyCode==13) return false;" />
                                                </td>
                                                <td class="text-left"><small><%#Eval("ngaynhap","{0:dd/MM/yyyy HH:mm}") %></small>

                                                    <div class="fw-600"><%#Eval("HoTenNhanVien") %></div>
                                                </td>
                                                <td>
                                                    <div data-role="lightbox" class="c-pointer">
                                                        <img src='<%#Eval("anh") %>' class="img-cover-vuong" width="50" height="50" />
                                                    </div>
                                                </td>
                                                <td style="text-align: left!important">
                                                    <%#Eval("TenSP") %>
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
                                                <td><%#Eval("DVT").ToString().ToUpper() %></td>
                                                <td><b><%#Eval("ton_hientai","{0:#,##0}") %></b></td>

                                                <asp:PlaceHolder ID="PlaceHolder7" runat="server" Visible='<%#Eval("nhap_hay_xuat").ToString()=="True" %>'>
                                                    <td>
                                                        <div data-role="hint" data-hint-position="top" data-hint-text="Nhập" class="button mini rounded success"><b>+ <%#Eval("soluong_nhap","{0:#,##0}") %></b></div>
                                                    </td>
                                                    <td><b><%#Eval("TonCuoi_SauNhap","{0:#,##0}") %></b></td>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="PlaceHolder8" runat="server" Visible='<%#Eval("nhap_hay_xuat").ToString()=="False" %>'>
                                                    <td>
                                                        <div data-role="hint" data-hint-position="top" data-hint-text="Xuất" class="button mini rounded warning">- <%#Eval("soluong_nhap","{0:#,##0}") %></div>
                                                      <div><small>Xuất đơn hàng số <b><%#Eval("id_baogia") %></b></small></div>
                                                        <div><small><%#Eval("TenKhach") %></small></div>
                                                        <div><small><%#Eval("SDT_Khach") %></small></div>
                                                    </td>
                                                    <td><b><%#Eval("TonCuoi_SauXuat","{0:#,##0}") %></b></td>
                                                </asp:PlaceHolder>


                                                <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible="false">
                                                    <td class="text-right"><%#Eval("GiaNhapXuat","{0:#,##0}") %>
                                                        <div><small>x <%#Eval("soluong_nhap") %></small></div>
                                                        <div><small>=<%#Eval("TongGia","{0:#,##0}") %></small></div>
                                                    </td>
                                                </asp:PlaceHolder>

                                                <td><%#Eval("Hang").ToString().ToUpper() %></td>
                                                <td><%#Eval("model").ToString().ToUpper() %></td>
                                                <td><%#Eval("Nhom").ToString().ToUpper() %></td>


                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </div>

                        <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible="false">
                            <div class="mt-10"><b>Thống kê</b></div>
                            <div><small>Từ <%=ViewState["tungay"] %> đến <%=ViewState["denngay"] %></small></div>
                            <div class="row mt-3">
                                <div class="cell-lg-4">
                                    <div class="bg-green p-4 fg-white">
                                        <div class="text-center"><b>NHẬP HÀNG</b></div>
                                        <small>
                                            <div class="place-left">Tổng SL nhập</div>
                                            <div class="place-right"><b><%= Convert.ToInt64(ViewState["TongSLNhap"]).ToString("#,##0") %></b></div>
                                            <div class="clr-bc"></div>
                                            <div class="place-left">Tiền nhập hàng</div>
                                            <div class="place-right"><b><%= Convert.ToInt64(ViewState["TongTienNhap"]).ToString("#,##0") %></b></div>
                                            <div class="clr-bc"></div>
                                        </small>
                                    </div>
                                </div>
                                <div class="cell-lg-4">
                                    <div class="bg-orange p-4 fg-white">
                                        <div class="text-center"><b>BÁN HÀNG</b></div>
                                        <small>
                                            <div class="place-left">Tổng SL xuất</div>
                                            <div class="place-right"><b><%= Convert.ToInt64(ViewState["TongSLBan"]).ToString("#,##0") %></b></div>
                                            <div class="clr-bc"></div>
                                            <div class="place-left">Tiền bán hàng</div>
                                            <div class="place-right"><b><%= Convert.ToInt64(ViewState["TongTienBan"]).ToString("#,##0") %></b></div>
                                            <div class="clr-bc"></div>
                                        </small>
                                    </div>
                                </div>
                                <div class="cell-lg-4">
                                    <div class="bg-cyan p-4 fg-white text-center">
                                        <div><b>CHÊNH LỆCH</b></div>
                                        <small>
                                            <div><b><%=ViewState["ChenhLech"] %></b></div>
                                            <div><b>VNĐ</b></div>
                                        </small>
                                    </div>
                                </div>
                            </div>
                        </asp:PlaceHolder>


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
</asp:Content>

