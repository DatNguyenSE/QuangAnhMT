<%@ Page Title="Trang chủ" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="admin_Default" %>
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <%--title & meta--%>
    <asp:Literal ID="literal_meta" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
  
    <asp:UpdatePanel ID="up_baohoanthanh" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_baohoanthanh" runat="server" Visible="false" DefaultButton="but_hoanthanh">
                <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                    <div style='top: 0; left: 0px; margin: 0 auto; max-width: 600px; opacity: 1;'>
                        <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                            <a href='#' class='fg-white d-inline' id="A1" runat="server" onserverclick="but_close_form_baohoanthanh_Click" title='Đóng'>
                                <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                            </a>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                BÁO CÁO HOÀN THÀNH CÔNG VIỆC
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>
                <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                    <div style='top: 0; left: 0; margin: 0 auto; max-width: 606px; opacity: 1;'>
                        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px">
                            <%--pl-4 pl-8-md pr-8-md pr-4--%>
                            <div>
                                <div class="mt-1">
                                    <label class="fw-600 fg-red">Ghi chú báo cáo</label>
                                    <asp:TextBox ID="TextBox3" Text="" runat="server" data-role="textarea" TextMode="MultiLine"></asp:TextBox>
                                </div>
                                <div class="mt-3">
                                    <label class="fw-600">Ảnh báo cáo</label>
                                    <input type="file" id="fileInput" onchange="uploadFile()" data-role="file" data-button-title="<span class='mif-file-upload'></span>" />
                                    <div id="message" runat="server"></div>
                                    <div id="uploadedFilePath"></div>
                                    <div style="display: none">
                                        <asp:TextBox ID="txt_link_fileupload" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="mt-6 text-right">
                                <asp:Button ID="but_hoanthanh" runat="server" Text="HOÀN THÀNH CÔNG VIỆC" CssClass="button success small" OnClick="but_hoanthanh_Click" />
                            </div>
                            <div class="mb-20"></div>

                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="up_baohoanthanh">
        <ProgressTemplate>
            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                <div style="padding-top: 45vh;">
                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>


    <div class=" p-3">
        <div class="row">
            <div class="bg-white cell-lg-5 p-3">
                <div><b>Chấm công ngày <%=DateTime.Now.ToShortDateString() %></b></div>
                <div>
                    <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                </div>
                <div class="">
                    <asp:Button ID="but_diemdanh" runat="server" Text="Báo vào ca" CssClass="small success rounded" OnClick="but_diemdanh_Click" OnClientClick="getLocationAndSubmit(); return false;" />
                </div>

                <div style="overflow: auto;" class="mt-2 mb-3">
                    <table class="table striped row-hover table-border cell-border compact cell-hover bg-white ">
                        <thead>
                            <tr>
                                <td class="fw-600 bg-white " style="width: 1px; min-width: 1px">TT</td>
                                <td class="fw-600 bg-white text-center" style="width: 40px; min-width: 40px">Ảnh</td>
                                <td class="fw-600 bg-white " style="min-width: 130px">Nhân viên</td>
                                <td class="fw-600 bg-white text-center" style="width: 60px; min-width: 60px">Vào ca</td>
                                <td class="fw-600 bg-white  text-center" style="width: 60px; min-width: 60px">Ra ca</td>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="Repeater1" runat="server">
                                <ItemTemplate>
                                    <span style="display: none">
                                        <asp:Label ID="lbID" runat="server" Text='<%#Eval("taikhoan") %>'></asp:Label>
                                    </span>
                                    <tr>
                                        <td class="text-center"><%# Container.ItemIndex+1 %></td>
                                        <td>
                                            <div data-role="lightbox" class="c-pointer">
                                                <img src="<%#Eval("anhdaidien") %>" class="img-cover-vuongtron" width="40" height="40" />
                                            </div>
                                        </td>
                                        <td class="text-left" style="vertical-align: middle">
                                            <div class="fw-600"><%#Eval("hoten") %></div>
                                            <div><small><%#Eval("taikhoan") %></small></div>
                                        </td>
                                        <td class="text-center">
                                            <div class="fw-600"><%#Eval("ngaychamcong","{0:HH:mm}") %></div>
                                            <div><small><a data-role="hint" data-hint-position="top" data-hint-text="Xem vị trí" target="_blank" href="https://google.com/maps?q=<%#Eval("vido") %>,<%#Eval("kinhdo") %>" class="fg-cobalt"><%# Eval("khoangcach", "{0:#,##0}") %>m</a></small></div>
                                        </td>
                                        <td class="text-center">
                                            <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%#Eval("baoraca")!=null %>'>
                                                <div class="fw-600"><%#Eval("baoraca","{0:HH:mm}") %></div>
                                                <div><small><a data-role="hint" data-hint-position="top" data-hint-text="Xem vị trí" target="_blank" href="https://google.com/maps?q=<%#Eval("vido_raca") %>,<%#Eval("kinhdo_raca") %>" class="fg-cobalt"><%# Eval("khoangcach_raca", "{0:#,##0}") %>m</a></small></div>
                                            </asp:PlaceHolder>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </div>

        </div>


        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="row mt-3">
                    <div class="bg-white cell-lg-12 p-3">
                        <div><b>Công việc chưa hoàn thành</b></div>
                        <asp:PlaceHolder ID="PlaceHolder1" runat="server">
                            <div class="bcorn-fix-title-table-container">
                                <table class="bcorn-fix-title-table">
                                    <thead>
                                        <tr class="">
                                            <th style="width: 1px;">ID</th>

                                            <th style="min-width: 200px;" class="text-left">Công việc</th>
                                            <th style="width: 110px; min-width: 110px;" class="text-left">Người giao</th>
                                            <th style="width: 1px; min-width: 1px;">Ngày giao</th>
                                            <th style="width: 140px; min-width: 140px;" class="text-left">Người nhận</th>

                                            <th style="width: 1px; min-width: 1px;">Thời hạn</th>

                                            <th style="width: 80px; min-width: 80px;" class="text-left">Trạng thái</th>
                                            <%--<th style="width: 300px; min-width: 300px;">Thông số</th>--%>
                                        </tr>
                                    </thead>

                                    <tbody>
                                        <asp:Repeater ID="Repeater2" runat="server">
                                            <ItemTemplate>
                                                <span style="display: none">
                                                    <asp:Label ID="lbID" runat="server" Text='<%#Eval("id") %>'></asp:Label>
                                                </span>
                                                <tr>
                                                    <td class="text-center">
                                                        <%#Eval("id") %>
                                                    </td>


                                                    <td style="text-align: left!important">
                                                        <div><%#Eval("TenCongViec") %></div>
                                                        <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%#Eval("Gap_KhongGap").ToString()=="True" %>'>
                                                            <asp:PlaceHolder ID="PlaceHolder11" runat="server" Visible='<%#Eval("trangthai").ToString()!="Hoàn thành" %>'>
                                                                <div class="button mini rounded alert ani-flash">Việc gấp</div>
                                                            </asp:PlaceHolder>
                                                            <asp:PlaceHolder ID="PlaceHolder12" runat="server" Visible='<%#Eval("trangthai").ToString()=="Hoàn thành" %>'>
                                                                <div class="button mini rounded alert">Việc gấp</div>
                                                            </asp:PlaceHolder>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("Gap_KhongGap").ToString()=="False" %>'>
                                                            <div class="button mini rounded dark">Không gấp</div>
                                                        </asp:PlaceHolder>
                                                    </td>
                                                    <td style="text-align: left!important">
                                                        <div><%#Eval("TenNguoiGiao") %></div>
                                                        <div><small class="fw-600"><%#Eval("nguoigiao") %></small></div>
                                                    </td>
                                                    <td>
                                                        <div><%#Eval("ngaygiao","{0:dd/MM/yyyy}") %></div>
                                                        <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%#Eval("tunhan_chidinh").ToString()=="True" %>'>
                                                            <div class="button mini rounded bg-lightBlue">Tự nhận</div>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%#Eval("tunhan_chidinh").ToString()=="False" %>'>
                                                            <div class="button mini rounded bg-lightPink">Chỉ định</div>
                                                        </asp:PlaceHolder>
                                                    </td>
                                                    <td class="text-left">
                                                        <%#Eval("HoTen_NguoiNhan") %>
                                                    </td>
                                                    <td>
                                                        <asp:PlaceHolder ID="PlaceHolder9" runat="server" Visible='<%#Eval("ThoiHan")!=null %>'>
                                                            <div><%#Eval("ThoiHan","{0:dd/MM/yyyy}") %></div>
                                                            <asp:PlaceHolder ID="PlaceHolder13" runat="server" Visible='<%#Eval("trehan").ToString()=="True" %>'>
                                                                <div class="button mini rounded alert ani-flash">Trễ hạn</div>
                                                            </asp:PlaceHolder>
                                                        </asp:PlaceHolder>
                                                    </td>

                                                    <td class="text-left">
                                                        <div>
                                                            <asp:LinkButton ID="but_show_form_baohoanthanh" OnClick="but_show_form_baohoanthanh_Click" CommandArgument='<%#Eval("id") %>' runat="server">Báo đã hoàn thành</asp:LinkButton>
                                                        </div>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>

                        </asp:PlaceHolder>
                    </div>
                </div>

                <div class="row mt-3">
                    <div class="bg-white cell-lg-12 p-3">
                        <div><b>Công việc chưa ai nhận</b></div>
                        <asp:PlaceHolder ID="PlaceHolder6" runat="server">
                            <div class="bcorn-fix-title-table-container">
                                <table class="bcorn-fix-title-table">
                                    <thead>
                                        <tr class="">
                                            <th style="width: 1px;">ID</th>

                                            <th style="min-width: 200px;" class="text-left">Công việc</th>
                                            <th style="width: 110px; min-width: 110px;" class="text-left">Người giao</th>
                                            <th style="width: 1px; min-width: 1px;">Ngày giao</th>


                                            <th style="width: 1px; min-width: 1px;">Thời hạn</th>

                                            <th style="width: 80px; min-width: 80px;" class="text-left">Trạng thái</th>
                                            <%--<th style="width: 300px; min-width: 300px;">Thông số</th>--%>
                                        </tr>
                                    </thead>

                                    <tbody>
                                        <asp:Repeater ID="Repeater3" runat="server">
                                            <ItemTemplate>
                                                <span style="display: none">
                                                    <asp:Label ID="lbID" runat="server" Text='<%#Eval("id") %>'></asp:Label>
                                                </span>
                                                <tr>
                                                    <td class="text-center">
                                                        <%#Eval("id") %>
                                                    </td>


                                                    <td style="text-align: left!important">
                                                        <div><%#Eval("TenCongViec") %></div>
                                                        <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%#Eval("Gap_KhongGap").ToString()=="True" %>'>
                                                            <asp:PlaceHolder ID="PlaceHolder11" runat="server" Visible='<%#Eval("trangthai").ToString()!="Hoàn thành" %>'>
                                                                <div class="button mini rounded alert ani-flash">Việc gấp</div>
                                                            </asp:PlaceHolder>
                                                            <asp:PlaceHolder ID="PlaceHolder12" runat="server" Visible='<%#Eval("trangthai").ToString()=="Hoàn thành" %>'>
                                                                <div class="button mini rounded alert">Việc gấp</div>
                                                            </asp:PlaceHolder>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("Gap_KhongGap").ToString()=="False" %>'>
                                                            <div class="button mini rounded dark">Không gấp</div>
                                                        </asp:PlaceHolder>
                                                    </td>
                                                    <td style="text-align: left!important">
                                                        <div><%#Eval("TenNguoiGiao") %></div>
                                                        <div><small class="fw-600"><%#Eval("nguoigiao") %></small></div>
                                                    </td>
                                                    <td>
                                                        <div><%#Eval("ngaygiao","{0:dd/MM/yyyy}") %></div>
                                                        <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%#Eval("tunhan_chidinh").ToString()=="True" %>'>
                                                            <div class="button mini rounded bg-lightBlue">Tự nhận</div>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%#Eval("tunhan_chidinh").ToString()=="False" %>'>
                                                            <div class="button mini rounded bg-lightPink">Chỉ định</div>
                                                        </asp:PlaceHolder>
                                                    </td>

                                                    <td>
                                                        <asp:PlaceHolder ID="PlaceHolder9" runat="server" Visible='<%#Eval("ThoiHan")!=null %>'>
                                                            <div><%#Eval("ThoiHan","{0:dd/MM/yyyy}") %></div>
                                                            <asp:PlaceHolder ID="PlaceHolder13" runat="server" Visible='<%#Eval("trehan").ToString()=="True" %>'>
                                                                <div class="button mini rounded alert ani-flash">Trễ hạn</div>
                                                            </asp:PlaceHolder>
                                                        </asp:PlaceHolder>
                                                    </td>

                                                    <td class="text-left">
                                                        <div>
                                                            <asp:LinkButton ID="but_nhanviecngay" OnClick="but_nhanviecngay_Click" CommandArgument='<%#Eval("id") %>' runat="server">Nhận việc ngay</asp:LinkButton>
                                                        </div>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                        </asp:PlaceHolder>
                    </div>
                </div>

                <div class="row mt-3">
                    <div class="bg-white cell-lg-12 p-3">
                        <div><b>Hàng bảo hành chưa trả</b></div>
                        <asp:PlaceHolder ID="PlaceHolder7" runat="server">
                            <div class="bcorn-fix-title-table-container">
                                <table class="bcorn-fix-title-table">
                                    <thead>
                                        <tr class="">
                                            <th style="width: 1px;">ID</th>
 
                                            <th style="width: 108px; min-width: 108px;">Ngày tạo</th>
                                            <th style="width: 150px; min-width: 150px;">Khách hàng</th>
                                            <th style="width: 150px; min-width: 150px;">Địa chỉ</th>
                                            <th style="width: 1px; min-width: 1px;">Hạn trả</th>
                                            <th style="width: 1px; min-width: 1px;">Tổng tiền</th>
                                            <th style="width: 1px; min-width: 1px;">Tổng giảm</th>
                                            <th style="width: 1px; min-width: 1px;">Tổng sau giảm</th>
                                            <th style="width: 100px; min-width: 100px;">Ghi chú</th>
                                            <th style="width: 80px; min-width: 80px;"></th>
                                        </tr>
                                    </thead>

                                    <tbody>
                                        <asp:Repeater ID="Repeater4" runat="server">
                                            <ItemTemplate>
                                                <span style="display: none">
                                                    <asp:Label ID="lbID" runat="server" Text='<%#Eval("id") %>'></asp:Label>
                                                </span>
                                                <tr>
                                                    <td class="text-center">
                                                       <%#Eval("id") %>
                                                    </td>
                                              
                                                    <td class="text-left"><small><%#Eval("ngaytao","{0:dd/MM/yyyy HH:mm}") %></small>
                                                        <div class="fw-600"><%#Eval("HoTenNhanVien") %></div>
                                                    </td>
                                                    <td class="text-left"><%#Eval("ten_khachhang") %>
                                                        <div><a class="fw-600" title="Nhấn để gọi" href="tel:<%#Eval("sdt_khachhang") %>"><span class="mif-phone pr-1"></span><%#Eval("sdt_khachhang") %></a></div>

                                                    </td>

                                                    <td class="text-left">
                                                        <%#Eval("diachi_khachhang") %></td>

                                                    <td>
                                                        <div><%#Eval("NgayHenKhachTra","{0:dd/MM/yyyy}") %></div>
                                                        <asp:PlaceHolder ID="PlaceHolder13" runat="server" Visible='<%#Eval("trehen").ToString()=="True" %>'>
                                                            <div class="button mini rounded alert ani-flash">Trễ hẹn</div>
                                                        </asp:PlaceHolder>
                                                    </td>

                            

                                                    <td class="text-right"><%#Eval("TongTien","{0:#,##0}") %>

                                        
                                                    </td>
                                                    <td class="text-right">
                                                        <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%#Eval("TongGiam").ToString()!="0" %>'>
                                                            <div class="fg-orange"><%#Eval("TongGiam","{0:#,##0}") %></div>
                                                        </asp:PlaceHolder>
                                                    </td>
                                                    <td class="text-right">
                                                        <div><%#Eval("TongSauGiam","{0:#,##0}") %></div>
                                                        <asp:PlaceHolder ID="PlaceHolder6" runat="server" Visible='<%#Eval("congno").ToString()!="0" %>'>
                                                            <%# Eval("congno","{0:#,##0}") %>
                                                        </asp:PlaceHolder>
                                                    </td>
                                                    <td><%#Eval("ghichu") %></td>

                                                    <td style="vertical-align: middle">
                                                        <a href="/admin/hang-bao-hanh/default.aspx?id=<%#Eval("id") %>">Xác nhận đã trả hàng</a>
                                                      
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>

                                    </tbody>
                                 
                                </table>
                            </div>
                        </asp:PlaceHolder>
                    </div>
                </div>

            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
            <ProgressTemplate>
                <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                    <div style="padding-top: 45vh;">
                        <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                    </div>
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>

    </div>




</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <%=notifi %>
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
        }</script>
</asp:Content>

