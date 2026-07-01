<%@ Control Language="C#" AutoEventWireup="true" CodeFile="menu-left-uc.ascx.cs" Inherits="admin_uc_menu_left_uc" %>
<div class="navview-pane" style="z-index: 6">
    <div class="d-flex flex-align-center bg-navview-left-bc">
        <span class="pull-button m-0 bg-navview-left-bc-hover">
            <span class="mif-menu fg-white"></span>
        </span>
        <div class="app-title h4 text-light m-0 fg-white pl-2" style="line-height: 52px">ThaiAnAudio</div>
    </div>

    <%-- <div class="suggest-box">
        <input id="search" list="suggestions" type="text" data-role="input" data-clear-button="true" data-search-button="false" placeholder="Tìm nhanh..." autocomplete="off" onkeypress="if (event.keyCode==13) return false;">
        <datalist id="suggestions">
            <!-- Các suggestion sẽ được thêm từ JavaScript -->
        </datalist>

        <script>
            // Bao gồm đoạn mã trong hàm tự chạy
            (function () {
                var suggestionsArray = [
                    { label: "Quản lý menu", link: "/admin/quan-ly-menu/default.aspx" },
                    { label: "Quản lý bài viết", link: "/admin/quan-ly-bai-viet/default.aspx" },
                    // Các suggestion khác tương ứng
                ];

                var searchInput = document.getElementById("search");

                // Sự kiện khi người dùng nhập
                searchInput.addEventListener("input", function () {
                    var keyword = searchInput.value.toLowerCase();
                    updateSuggestions(keyword);
                });

                // Sự kiện khi người dùng chọn một suggestion
                searchInput.addEventListener("change", function () {
                    var selectedValue = searchInput.value.toLowerCase();
                    navigateToLink(selectedValue);
                });

                // Hàm cập nhật danh sách gợi ý
                function updateSuggestions(keyword) {
                    var datalist = document.getElementById("suggestions");
                    // Xóa các option hiện tại
                    datalist.innerHTML = "";

                    // Lặp qua mảng và thêm các suggestion thỏa mãn từ khóa
                    suggestionsArray.forEach(function (suggestion) {
                        var lowerSuggestion = suggestion.label.toLowerCase();
                        if (lowerSuggestion.includes(keyword)) {
                            var option = document.createElement("option");
                            option.value = suggestion.label;
                            datalist.appendChild(option);
                        }
                    });
                }

                // Hàm điều hướng đến link tương ứng
                function navigateToLink(selectedValue) {
                    // Tìm suggestion có giá trị trùng khớp
                    var selectedSuggestion = suggestionsArray.find(function (suggestion) {
                        return suggestion.label.toLowerCase() === selectedValue;
                    });

                    // Nếu tìm thấy suggestion, điều hướng đến link
                    if (selectedSuggestion) {
                        window.location.href = selectedSuggestion.link;
                    }
                }
            })();
        </script>

        <button class="holder">
            <span class="mif-search fg-white"></span>
        </button>
    </div>--%>


    <ul class="navview-menu" id="side-menu">
        <li class="<%=a0 %>">
            <a href="/admin/default.aspx">
                <span class="icon"><span class="mif-home"></span></span>
                <span class="caption">Trang chủ</span>
            </a>
        </li>
        <li class="item-header">HỆ THỐNG</li>
        <li class="<%=a5 %>">
            <a href="/admin/quan-ly-he-thong/cai-dat.aspx">
                <span class="icon"><span class="mif-cog"></span></span>
                <span class="caption">Cài đặt hệ thống</span>
            </a>
        </li>

        <li class="<%=a1 %>">
            <a class="dropdown-toggle">
                <span class="icon"><span class="mif-database"></span></span>
                <span class="caption">Dữ liệu nguồn</span>
            </a>
            <ul class="navview-menu" data-role="dropdown">
                <li class="<%=a1_1 %>">
                    <a href="/admin/quan-ly-he-thong/du-lieu-nguon/hang-san-pham.aspx" onclick="Metro.activity.open({type:'cycle',overlayClickClose:true})">
                        <span class="icon"><span class="mif-chevron-right"></span></span>
                        <span class="caption">Hãng sản phẩm</span>
                    </a>
                </li>
                <li class="<%=a1_2 %>">
                    <a href="/admin/quan-ly-he-thong/du-lieu-nguon/nhom-san-pham.aspx" onclick="Metro.activity.open({type:'cycle',overlayClickClose:true})">
                        <span class="icon"><span class="mif-chevron-right"></span></span>
                        <span class="caption">Nhóm sản phẩm</span>
                    </a>
                </li>
                <li class="<%=a1_3 %>">
                    <a href="/admin/quan-ly-he-thong/du-lieu-nguon/don-vi-tinh.aspx" onclick="Metro.activity.open({type:'cycle',overlayClickClose:true})">
                        <span class="icon"><span class="mif-chevron-right"></span></span>
                        <span class="caption">Đơn vị tính</span>
                    </a>
                </li>
            </ul>
        </li>

        <li class="item-header">NHÂN VIÊN</li>
        <li class="<%=a2 %>">
            <a href="/admin/quan-ly-nhan-vien/default.aspx">
                <span class="icon"><span class="mif-users"></span></span>
                <span class="caption">Quản lý nhân viên</span>
            </a>
        </li>
        <li class="<%=a6 %>">
            <a href="/admin/quan-ly-nhan-vien/bang-cham-cong.aspx">
                <span class="icon"><span class="mif-table"></span></span>
                <span class="caption">Bảng chấm công & Thu nhập</span>
            </a>
        </li>

        <li class="item-header">KHO HÀNG</li>
        <li class="<%=a3 %>">
            <a href="/admin/quan-ly-kho/default.aspx">
                <span class="icon"><span class="mif-widgets"></span></span>
                <span class="caption">Quản lý kho</span>
            </a>
        </li>
        <li class="<%=a4 %>">
            <a href="/admin/quan-ly-kho/lich-su-nhap-xuat.aspx">
                <span class="icon"><span class="mif-calendar"></span></span>
                <span class="caption">Lịch sử nhập xuất</span>
            </a>
        </li>
        <li class="<%=muon_hang %>">
            <a href="/admin/quan-ly-kho/muon-hang.aspx">
                <span class="icon"><span class="mif-calendar"></span></span>
                <span class="caption">Mượn hàng</span>
            </a>
        </li>

        <li class="item-header">DANH MỤC KHÁC</li>
        <li class="<%=a8 %>">
            <a href="/admin/data-khach-hang/default.aspx">
                <span class="icon"><span class="mif-user-secret"></span></span>
                <span class="caption">Data khách hàng</span>
            </a>
        </li>
        <li class="<%=a7 %>">
            <a href="/admin/quan-ly-bao-gia/default.aspx">
                <span class="icon"><span class="mif-dollar2"></span></span>
                <span class="caption">Báo giá & Bán hàng</span>
            </a>
        </li>
        <li class="">
            <a href="/admin/thong-ke/ban-hang.aspx">
                <span class="icon"><span class="mif-dollar2"></span></span>
                <span class="caption">Thống kê bán hàng</span>
            </a>
        </li>
        <li class="<%=a9 %>">
            <a href="/admin/quan-ly-cong-viec/default.aspx">
                <span class="icon"><span class="mif-clipboard"></span></span>
                <span class="caption">Công việc</span>
            </a>
        </li>
        <li class="<%=a10 %>">
            <a href="/admin/hang-bao-hanh/default.aspx">
                <span class="icon"><span class="mif-tools"></span></span>
                <span class="caption">Hàng bảo hành</span>
            </a>
        </li>
    </ul>

    <div class="w-100 text-center text-small data-box p-2 border-top bd-darkCobalt" style="position: absolute; bottom: 0">
        <%--bg-navview-foot-bc--%>
        <div>Sản phẩm của <a href="https:/Hotasoft.com" class="text-muted fg-white-hover no-decor">Hotasoft.com</a></div>
    </div>

</div>
