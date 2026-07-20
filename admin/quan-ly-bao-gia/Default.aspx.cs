using NPOI.SS.Formula.Functions;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_quan_ly_bao_gia_Default : System.Web.UI.Page
{
    // ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Tài khoản đã bị khóa.", "false", "false", "OK", "alert", ""), true);
    String_cl str_cl = new String_cl();
    DateTime_cl dt_cl = new DateTime_cl();

    public void set_dulieu_macdinh()
    {

        ResetButtonCss();//button chọn ngày nhanh
        txt_show.Text = "30";
        ViewState["current_page_qlbaogia1"] = "1";
        txt_tungay.Text = dt_cl.return_ngaydaunam(DateTime.Now.Year.ToString()).ToShortDateString();
        txt_denngay.Text = dt_cl.return_ngaycuoinam(DateTime.Now.Year.ToString()).ToShortDateString();
        ViewState["tungay"] = txt_tungay.Text;
        ViewState["denngay"] = txt_denngay.Text;




        #region set_get_cookie
        // Lấy cookie "cookie_qlbaogia1" từ Request.Cookies
        HttpCookie cookie = Request.Cookies["cookie_qlbaogia1"];
        if (cookie == null)
        {
            ListBox1.SelectedIndex = 0;//mặc định chọn tất cả phân loại, nếu select=true ngoài html thì k lưu lịch sử đc, kệ mẹ nó cứ làm y vậy đi, đừng quan tâm tới đoạn này
            ListBox2.SelectedIndex = 0;
            // Nếu cookie không tồn tại, tạo cookie mới
            cookie = new HttpCookie("cookie_qlbaogia1");
            cookie["show"] = txt_show.Text;//lưu số dòng hiển thị mỗi trang
            cookie["trang_hientai"] = "1";//lưu trang hiện tại
            cookie["id_loctheothoigian"] = ddl_thoigian.SelectedValue;
            cookie["id_loc3"] = DropDownList3.SelectedValue;
            cookie["id_loc4"] = DropDownList4.SelectedValue;
            cookie["id_loc5"] = DropDownList5.SelectedValue;
            cookie["tungay"] = txt_tungay.Text;
            cookie["denngay"] = txt_denngay.Text;
            cookie["nguoibaogia"] = "";//Tất cả
            cookie["phanloai"] = "";//Tất cả phân loại
                                    // Đặt thời gian hết hạn của cookie là 1 ngày từ thời điểm hiện tại
            cookie.Expires = DateTime.Now.AddDays(1);
            cookie.HttpOnly = true;
            cookie.Secure = true;
            // Thêm cookie vào Response.Cookies
            Response.Cookies.Add(cookie);

        }
        else
        {
            // Nếu cookie đã tồn tại, lấy giá trị từ cookie
            txt_show.Text = cookie["show"];
            ViewState["current_page_qlbaogia1"] = cookie["trang_hientai"];
            ddl_thoigian.SelectedValue = cookie["id_loctheothoigian"];
            DropDownList3.SelectedValue = cookie["id_loc3"];
            DropDownList4.SelectedValue = cookie["id_loc4"];
            DropDownList5.SelectedValue = cookie["id_loc5"];
            txt_tungay.Text = cookie["tungay"];
            txt_denngay.Text = cookie["denngay"];
            if (cookie["nguoibaogia"] == "")//nếu phân loại là Tất cả (value = "")
                ListBox2.SelectedIndex = 0;//Chọn mục Tất cả
            else
            {
                // Chọn các mục tương ứng với giá trị đã lưubu
                string[] _chon = cookie["nguoibaogia"].Split(',');
                foreach (ListItem item in ListBox2.Items)
                {
                    if (_chon.Contains(item.Value))
                        item.Selected = true;
                }
            }
            if (cookie["phanloai"] == "")//nếu phân loại là Tất cả (value = "")
                ListBox1.SelectedIndex = 0;//Chọn mục Tất cả
            else
            {
                // Chọn các mục tương ứng với giá trị đã lưu
                string[] _chon_phanloai = cookie["phanloai"].Split(',');
                foreach (ListItem item in ListBox1.Items)
                {
                    if (_chon_phanloai.Contains(item.Value))
                        item.Selected = true;
                }
            }
            // Thiết lập lại thời gian hết hạn của cookie là 1 ngày từ thời điểm hiện tại
            cookie.Expires = DateTime.Now.AddDays(1);
            // Cập nhật cookie trong Response.Cookies
            Response.Cookies.Set(cookie);
        }
        #endregion


    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            Session["url_back"] = HttpContext.Current.Request.Url.AbsoluteUri;
            check_login_cl.check_login_admin("16", "17");

            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";

            ViewState["taikhoan"] = _tk;
            using (dbDataContext db = new dbDataContext())
            {
                var q_tk = db.taikhoan_tbs
                  .Select(c => new { c.taikhoan, c.hoten })
                  .AsQueryable();
                ListBox2.DataSource = q_tk;
                ListBox2.DataValueField = "taikhoan";
                ListBox2.DataTextField = "hoten";
                ListBox2.DataBind();
                ListBox2.Items.Insert(0, new ListItem("Tất cả", ""));


            }
            set_dulieu_macdinh();
            show_main();

        }
    }
    #region main - phân trang - tìm kiếm

    public void show_main()
    {

        using (dbDataContext db = new dbDataContext())
        {

            #region lấy dữ liệu tối ưu
            var query = db.BaoGia_tbs.AsQueryable();

            string currentUser = ViewState["taikhoan"]?.ToString();
            if (check_login_cl.CheckQuyen(db, currentUser, "17"))
                query = query.Where(p => p.nguoibaogia == currentUser);

            // Gộp tìm kiếm
            string _key = string.IsNullOrWhiteSpace(txt_timkiem.Text) ? txt_timkiem1.Text.Trim() : txt_timkiem.Text.Trim();
            if (!string.IsNullOrEmpty(_key))
            {
                query = query.Where(p =>
                    p.ten_khachhang.Contains(_key) ||
                    p.diachi_khachhang.Contains(_key) ||
                    p.sdt_khachhang == _key ||
                    p.id.ToString() == _key);
            }

            if (string.IsNullOrEmpty(_key))
            {
                // Xử lý lọc theo thời gian
                string _id_locthoigian = ddl_thoigian.SelectedValue;
                if (_id_locthoigian == "1")
                {
                    if (DateTime.TryParse(txt_tungay.Text, out var tungay1))
                    {
                        DateTime fromDate = tungay1.Date;
                        query = query.Where(p => p.ngaybaogia >= fromDate);
                    }
                    if (DateTime.TryParse(txt_denngay.Text, out var denngay1))
                    {
                        DateTime toDateExclusive = denngay1.Date.AddDays(1);
                        query = query.Where(p => p.ngaybaogia < toDateExclusive);
                    }
                }
                else if (_id_locthoigian == "2")
                {
                    if (DateTime.TryParse(txt_tungay.Text, out var tungay2))
                    {
                        DateTime fromDate = tungay2.Date;
                        query = query.Where(p => p.ngayban_kyhopdong >= fromDate);
                    }
                    if (DateTime.TryParse(txt_denngay.Text, out var denngay2))
                    {
                        DateTime toDateExclusive = denngay2.Date.AddDays(1);
                        query = query.Where(p => p.ngayban_kyhopdong < toDateExclusive);
                    }
                }

                // Lọc theo trạng thái: Đã bán / Chưa bán
                switch (DropDownList3.SelectedValue)
                {
                    case "1":
                        query = query.Where(p => p.ngayban_kyhopdong != null);
                        break;
                    case "2":
                        query = query.Where(p => p.ngayban_kyhopdong == null);
                        break;
                }

                // Lọc theo thanh toán (công nợ)
                switch (DropDownList4.SelectedValue)
                {
                    case "1":
                        query = query.Where(p => p.ngayban_kyhopdong != null && p.congno == 0);
                        break;
                    case "2":
                        query = query.Where(p => p.ngayban_kyhopdong != null && p.congno != 0);
                        break;
                }

                // Lọc giao hàng
                switch (DropDownList5.SelectedValue)
                {
                    case "1":
                        query = query.Where(p => p.ngayban_kyhopdong != null && (p.ghichu_chuagiao == null || p.ghichu_chuagiao == ""));
                        break;
                    case "2":
                        query = query.Where(p => p.ngayban_kyhopdong == null && p.ghichu_chuagiao != null && p.ghichu_chuagiao != "");
                        break;
                }

                // Lọc theo người bán
                var list_taikhoan = ListBox2.Items.Cast<ListItem>()
                    .Where(i => i.Selected)
                    .Select(i => i.Value)
                    .ToList();

                if (!list_taikhoan.Contains(""))
                    query = query.Where(p => list_taikhoan.Contains(p.nguoibaogia));
            }

            // Giữ truy vấn đã lọc chưa sắp xếp để các phép thống kê không phải sort toàn bộ dữ liệu.
            var filteredQuery = query;
            int _Tong_Record = filteredQuery.Count();

            if (_Tong_Record != 0)
            {
                // Đơn đã bán
                var tongDonBan = filteredQuery.Count(p => p.ngayban_kyhopdong != null);

                var q_daban = filteredQuery.Where(p => p.ngayban_kyhopdong != null);
                
                // Kéo dữ liệu Báo giá đã bán về RAM (chỉ lấy các cột cần thiết để tính toán)
                var dabanList = q_daban.Select(p => new { p.id, p.vat, p.giamgiadacbiet, p.congno }).ToList();

                // Group-by bảng Chi tiết trước bằng SQL Server (chỉ tính những đơn đã bán để tối ưu)
                // Chỉ group các chi tiết thuộc báo giá đã lọc, không quét toàn bộ bảng chi tiết.
                var chiTietSums = (from ct in db.BaoGia_ChiTiet_tbs
                                   join bg in q_daban on ct.id_baogia equals bg.id.ToString()
                                   group ct by ct.id_baogia into g
                                   select new
                                   {
                                       id_baogia = g.Key,
                                       sum = g.Sum(x => (long?)x.TongSauGiam) ?? 0
                                   })
                                    .ToList();

                // Join in-memory để tính Tổng doanh thu
                var tongDoanhThu = (from ob1 in dabanList
                                    join ct in chiTietSums on ob1.id.ToString() equals ct.id_baogia into chiTietGroup
                                    let tongSauGiamCT = chiTietGroup.Sum(x => x.sum)
                                    let giamGiaDacBiet = (long?)ob1.giamgiadacbiet ?? 0
                                    let tongSauGiam = tongSauGiamCT - giamGiaDacBiet
                                    let tongSauThue = ob1.vat != 0 ? tongSauGiam * (1 + (decimal)ob1.vat / 100M) : tongSauGiam
                                    select (decimal?)tongSauThue).Sum() ?? 0m;

                // Công nợ
                var tongCongNo = dabanList.Select(p => (decimal?)p.congno).Sum() ?? 0m;

                var tongDaThanhToan = tongDoanhThu - tongCongNo;

                ViewState["TongĐonaBan"] = tongDonBan;      // int
                ViewState["TongDoanhThu"] = tongDoanhThu;    // decimal
                ViewState["TongCongNo"] = tongCongNo;      // decimal
                ViewState["TongDaThanhToan"] = tongDaThanhToan; // decimal
            }
            else
            {
                ViewState["TongĐonaBan"] = 0;
                ViewState["TongDoanhThu"] = 0m;
                ViewState["TongCongNo"] = 0m;
                ViewState["TongDaThanhToan"] = 0m;
            }
            #endregion

            if (check_login_cl.CheckQuyen(db, ViewState["taikhoan"].ToString(), "39"))
            {
                PlaceHolder11.Visible = true;
                #region tính lợi nhuận
                DateTime? tuNgayHD = DateTime.TryParse(txt_tungay.Text, out var _tu) ? _tu.Date : (DateTime?)null;
                DateTime? denNgayHD = DateTime.TryParse(txt_denngay.Text, out var _den) ? _den.Date : (DateTime?)null;
                DateTime? denNgayHDExclusive = denNgayHD.HasValue ? denNgayHD.Value.AddDays(1) : (DateTime?)null;

                var list_sanpham_ban =
                     (from ob1 in filteredQuery
                     join ob2 in db.BaoGia_ChiTiet_tbs
                         on ob1.id.ToString() equals ob2.id_baogia
                     where ob1.trangthai == "Đã ký HĐ"
                           && ob1.ngayban_kyhopdong.HasValue
                           && (!tuNgayHD.HasValue || ob1.ngayban_kyhopdong >= tuNgayHD)
                           && (!denNgayHDExclusive.HasValue || ob1.ngayban_kyhopdong < denNgayHDExclusive)
                      group new { soluong = ob2.soluong ?? 0 } by ob2.id_sanpham into g
                      join kho in db.KhoSanPham_tbs on g.Key equals kho.id.ToString()
                      select new
                      {
                          id_sanpham = g.Key,
                          tong_so_luong = g.Sum(x => x.soluong),
                          gianhap = kho.gianhap ?? 0,
                          tong_gia_von = (long)g.Sum(x => x.soluong) * (kho.gianhap ?? 0)
                      }).ToList();

                if (list_sanpham_ban.Any())
                {
                    ViewState["TongGiaNhap"] = list_sanpham_ban.Sum(p => p.tong_gia_von);
                    ViewState["TongLoiNhuan"] =
                        Convert.ToInt64(ViewState["TongDoanhThu"]) - Convert.ToInt64(ViewState["TongGiaNhap"]);
                }
                else
                {
                    ViewState["TongGiaNhap"] = "0";
                    ViewState["TongLoiNhuan"] = "0";
                }
                #endregion
            }
            else
                PlaceHolder11.Visible = false;

            #region phân trang OK, k sửa
            // Xử lý số record mỗi trang
            int show = Number_cl.Check_Int(txt_show.Text.Trim()); if (show <= 0) show = 30;
            int current_page = int.Parse(ViewState["current_page_qlbaogia1"].ToString()); int total_page = number_of_page_class.return_total_page(_Tong_Record, show); if (current_page < 1) current_page = 1; else if (current_page > total_page) current_page = total_page;
            ViewState["total_page"] = total_page;
            if (current_page >= total_page)
            {
                but_xemtiep.Enabled = false;
                but_xemtiep1.Enabled = false;
            }
            else
            {
                but_xemtiep.Enabled = true;
                but_xemtiep1.Enabled = true;
            }
            if (current_page == 1)
            {
                but_quaylai.Enabled = false;
                but_quaylai1.Enabled = false;
            }
            else
            {
                but_quaylai.Enabled = true;
                but_quaylai1.Enabled = true;
            }
            
            // Chỉ sắp xếp và lấy ID của trang hiện tại.
            var pagedIds = filteredQuery
                .OrderByDescending(p => p.ngaybaogia)
                .Skip(current_page * show - show)
                .Take(show)
                .Select(p => p.id)
                .ToList();
            
            // Lấy chi tiết bằng pagedIds
            var list_split = (from ob1 in db.BaoGia_tbs
                               where pagedIds.Contains(ob1.id)
                               join ob2 in db.BaoGia_ChiTiet_tbs on ob1.id.ToString() equals ob2.id_baogia into chiTietGroup
                               join kh in db.Data_KhachHang_tbs on ob1.sdt_khachhang equals kh.sdt into KhachHangGroup
                               from kh in KhachHangGroup.DefaultIfEmpty()
                               join tk in db.taikhoan_tbs on ob1.nguoibaogia equals tk.taikhoan into TaiKhoanGroup
                              from tk in TaiKhoanGroup.DefaultIfEmpty()
                              let tongTien = chiTietGroup.Sum(ct => (long?)ct.thanhtien) ?? 0
                              let tongGiamCT = chiTietGroup.Sum(ct => (long?)ct.giamgia_thanhtien) ?? 0
                              let tongSauGiamCT = chiTietGroup.Sum(ct => (long?)ct.TongSauGiam) ?? 0
                              let giamGiaDacBiet = (long?)ob1.giamgiadacbiet ?? 0
                              let tongGiam = tongGiamCT + giamGiaDacBiet
                              let tongSauGiam = tongSauGiamCT - giamGiaDacBiet
                              let tienVAT = ob1.vat != 0 ? tongSauGiam * ((decimal)ob1.vat / 100) : 0
                              let tongSauThue = ob1.vat != 0 ? tongSauGiam * (1 + (decimal)ob1.vat / 100) : tongSauGiam
                              let tenMatHang = (from ct in chiTietGroup
                                                join sp in db.KhoSanPham_tbs on ct.id_sanpham equals sp.id.ToString()
                                                select sp.ten).FirstOrDefault() ?? ""
                              select new
                              {
                                  ob1.id,
                                  ob1.file_hopdong,
                                  ob1.ngayban_kyhopdong,
                                  congno = ob1.congno ?? 0,
                                  ob1.vat,
                                  ob1.id_guide,
                                  ghichu_chuagiao = ob1.ghichu_chuagiao ?? "",
                                   ob1.sdt_khachhang,
                                   ob1.ten_khachhang,
                                   diachiKhachHang = kh != null ? kh.diachi : "",
                                  ob1.ngayhethan,
                                  ob1.ngaybaogia,
                                  ob1.nguoibaogia,
                                  ob1.phantram_doanhso_now,
                                  ob1.thuongdoanhso,
                                  ob1.trangthai,
                                  ob1.ThoiHan_BaoGia,
                                  TongTien = tongTien,
                                  TongGiam = tongGiam,
                                  TongSauGiam = tongSauGiam,
                                  HoTenNhanVien = tk != null ? tk.hoten : "",
                                   TongTien_VAT = tienVAT,
                                   TongSauThue = tongSauThue,
                                   tenMatHang = tenMatHang,
                                   baoGiaQuaHan = ob1.ngayban_kyhopdong == null && ob1.ngayhethan < DateTime.Now
                              }).AsEnumerable().OrderByDescending(p => p.ngaybaogia).ToList();

            //xử lý thanh thông báo phân trang
            int stt = (show * current_page) - show + 1; int _s1 = stt + list_split.Count() - 1;
            if (_Tong_Record != 0) lb_show.Text = stt + "-" + _s1 + " trong số " + _Tong_Record.ToString("#,##0"); else lb_show.Text = "0-0/0"; lb_show_md.Text = stt + "-" + _s1 + " trong số " + _Tong_Record.ToString("#,##0");
            #endregion
            Repeater1.DataSource = list_split;
            Repeater1.DataBind();
        }

    }
    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            ViewState["current_page_qlbaogia1"] = int.Parse(ViewState["current_page_qlbaogia1"].ToString()) - 1;
            #region LƯU TRANG HIỆN TẠI
            // Lấy cookie "cookie_qlbaogia1" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_qlbaogia1"];
            if (cookie != null)
            {
                cookie["trang_hientai"] = ViewState["current_page_qlbaogia1"].ToString();
                // Thiết lập lại thời gian hết hạn của cookie là 1 ngày từ thời điểm hiện tại
                cookie.Expires = DateTime.Now.AddDays(1);
                // Cập nhật cookie trong Response.Cookies
                Response.Cookies.Set(cookie);
            }
            #endregion
            show_main();
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            ViewState["current_page_qlbaogia1"] = int.Parse(ViewState["current_page_qlbaogia1"].ToString()) + 1;
            #region LƯU TRANG HIỆN TẠI
            // Lấy cookie "cookie_qlbaogia1" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_qlbaogia1"];
            if (cookie != null)
            {
                cookie["trang_hientai"] = ViewState["current_page_qlbaogia1"].ToString();
                // Thiết lập lại thời gian hết hạn của cookie là 1 ngày từ thời điểm hiện tại
                cookie.Expires = DateTime.Now.AddDays(1);
                // Cập nhật cookie trong Response.Cookies
                Response.Cookies.Set(cookie);
            }
            #endregion
            show_main();
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    protected void txt_timkiem_TextChanged(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            ViewState["current_page_qlbaogia1"] = 1;
            show_main();
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    #endregion


    #region Xuất excel
    protected void but_show_form_xuat_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            pn_xuat.Visible = !pn_xuat.Visible;

            // Clear CheckBoxList before adding new items
            check_list_page.Items.Clear();
            // add số trang vào checkboxlist
            for (int i = 1; i <= int.Parse(ViewState["total_page"].ToString()); i++)
            {
                // Tạo một ListItem mới với văn bản và giá trị là số thứ tự
                ListItem item = new ListItem($"Trang {i}", i.ToString());

                // Thêm mục vào CheckBoxList
                check_list_page.Items.Add(item);
                //tích chọn luôn
                item.Selected = true;
            }

            up_xuat.Update();
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    protected void but_xuat_excel_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            bool _chonmuc = false, _chonPage = false;

            foreach (ListItem item in check_list_excel.Items)
            {
                if (item.Selected)
                {
                    _chonmuc = true;
                    break; // Thoát vòng lặp sớm nếu tìm thấy mục được chọn
                }
            }
            if (!_chonmuc)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Không có mục nào được chọn.", "false", "false", "OK", "alert", ""), true);
                return; // Kết thúc sớm nếu không có mục nào được chọn
            }

            // Khởi tạo danh sách để lưu các mục được chọn (nếu cần)
            List<ListItem> selectedPage = new List<ListItem>();//để lưu các trang được chọn. dùng để xuất excel
            foreach (ListItem item in check_list_page.Items)
            {
                if (item.Selected)//nếu có trang đc chọn
                {
                    selectedPage.Add(item);//lưu lại trang đc chọn
                    _chonPage = true;
                    //break; // Thoát vòng lặp sớm nếu tìm thấy mục được chọn. K thoát vòng lặp vì để lưu hết trang đc chọn
                }
            }
            if (!_chonPage)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Không có trang nào được chọn.", "false", "false", "OK", "alert", ""), true);
                return; // Kết thúc sớm nếu không có mục nào được chọn
            }

            if (!Directory.Exists(Server.MapPath("~/uploads/files/")))
                Directory.CreateDirectory(Server.MapPath("~/uploads/files/"));
            using (dbDataContext db = new dbDataContext())
            {
                #region lấy dữ liệu
                var list_all = (from ob1 in db.BaoGia_tbs
                                join ob2 in db.BaoGia_ChiTiet_tbs
                                on ob1.id.ToString() equals ob2.id_baogia into chiTietGroup
                                join tk in db.taikhoan_tbs
                                on ob1.nguoibaogia equals tk.taikhoan into TaiKhoanGroup
                                from tk in TaiKhoanGroup.DefaultIfEmpty()
                                select new
                                {
                                    ob1.id,
                                    ob1.file_hopdong,
                                    ob1.ngayban_kyhopdong,
                                    congno = ob1.congno ?? 0,
                                    ob1.vat,
                                    ob1.id_guide,
                                    ghichu_chuagiao = ob1.ghichu_chuagiao ?? "",
                                    ob1.sdt_khachhang,
                                    ob1.ten_khachhang,
                                    ob1.diachi_khachhang,
                                    ob1.ngayhethan,
                                    ob1.ngaybaogia,
                                    ob1.nguoibaogia,
                                    ob1.phantram_doanhso_now,
                                    ob1.thuongdoanhso,
                                    ob1.trangthai, // Đã ký HĐ, Còn hiệu lực, Hết hiệu lực
                                    ob1.ThoiHan_BaoGia,
                                    TongTien = chiTietGroup.Sum(ct => (Int64?)ct.thanhtien) ?? 0,
                                    TongGiam = (chiTietGroup.Sum(ct => (Int64?)ct.giamgia_thanhtien) ?? 0) + (Int64?)ob1.giamgiadacbiet ?? 0,
                                    TongSauGiam = (chiTietGroup.Sum(ct => (Int64?)ct.TongSauGiam) ?? 0) - (Int64?)ob1.giamgiadacbiet ?? 0,
                                    HoTenNhanVien = tk == null ? "" : tk.hoten,
                                    // Tính TongTien_VAT: TongTien + VAT
                                    TongTien_VAT = ob1.vat != 0
                                        ? ((chiTietGroup.Sum(ct => (Int64?)ct.TongSauGiam) ?? 0) - (Int64?)ob1.giamgiadacbiet ?? 0) * ((decimal)ob1.vat / 100)
                                        : 0,
                                    // Tính TongSauThue: TongSauGiam + VAT
                                    TongSauThue = ob1.vat != 0
                                        ? ((chiTietGroup.Sum(ct => (Int64?)ct.TongSauGiam) ?? 0) - (Int64?)ob1.giamgiadacbiet ?? 0)
                                          * (1 + (decimal)ob1.vat / 100)
                                        : (chiTietGroup.Sum(ct => (Int64?)ct.TongSauGiam) ?? 0) - (Int64?)ob1.giamgiadacbiet ?? 0,
                                }).AsQueryable();


                if (check_login_cl.CheckQuyen(db, ViewState["taikhoan"].ToString(), "17"))
                { list_all = list_all.Where(p => p.nguoibaogia == ViewState["taikhoan"].ToString()); }

                // Kiểm tra xem textbox có dữ liệu tìm kiếm không
                string _key = txt_timkiem.Text.Trim();
                if (!string.IsNullOrEmpty(_key))
                {
                    list_all = list_all.Where(p => p.ten_khachhang.Contains(_key) || p.diachi_khachhang.Contains(_key) || p.sdt_khachhang == _key || p.id.ToString() == _key);
                }
                else
                {
                    string _key1 = txt_timkiem1.Text.Trim();
                    if (!string.IsNullOrEmpty(_key1))
                    {
                        list_all = list_all.Where(p => p.ten_khachhang.Contains(_key1) || p.diachi_khachhang.Contains(_key1) || p.sdt_khachhang == _key1 || p.id.ToString() == _key1);
                    }
                }



                //xử lý theo thời gian
                string _id_locthoigian = ddl_thoigian.SelectedValue;
                if (_id_locthoigian == "1")//lọc theo ngày BG
                {
                    if (txt_tungay.Text != "")
                        list_all = list_all.Where(p => p.ngaybaogia.Value.Date >= DateTime.Parse(txt_tungay.Text).Date);
                    if (txt_denngay.Text != "")
                        list_all = list_all.Where(p => p.ngaybaogia.Value.Date <= DateTime.Parse(txt_denngay.Text).Date);
                }
                else if (_id_locthoigian == "2")//lọc theo ngày BG
                {
                    if (txt_tungay.Text != "")
                        list_all = list_all.Where(p => p.ngayban_kyhopdong.HasValue && p.ngayban_kyhopdong.Value.Date >= DateTime.Parse(txt_tungay.Text).Date);
                    if (txt_denngay.Text != "")
                        list_all = list_all.Where(p => p.ngayban_kyhopdong.HasValue && p.ngayban_kyhopdong.Value.Date <= DateTime.Parse(txt_denngay.Text).Date);
                }
                //xử lý theo lọc khác: đã bán/chưa bán
                string _id_loc3 = DropDownList3.SelectedValue;
                if (_id_loc3 != "0")
                {
                    if (_id_loc3 == "1")//đã bán
                        list_all = list_all.Where(p => p.ngayban_kyhopdong != null);
                    else//chưa bán
                        list_all = list_all.Where(p => p.ngayban_kyhopdong == null);
                }
                //xử lý theo lọc khác: Thanh toán/Công nợ
                string _id_loc4 = DropDownList4.SelectedValue;
                if (_id_loc4 != "0")
                {
                    if (_id_loc4 == "1")//đã thanh toán
                        list_all = list_all.Where(p => p.ngayban_kyhopdong != null && p.congno == 0);
                    else//công nợ
                        list_all = list_all.Where(p => p.ngayban_kyhopdong == null && p.congno != 0);
                }
                //xử lý theo lọc khác: Giao hàng/Chưa giao
                string _id_loc5 = DropDownList5.SelectedValue;
                if (_id_loc5 != "0")
                {
                    if (_id_loc5 == "1")//đã fiao
                        list_all = list_all.Where(p => p.ngayban_kyhopdong != null && p.ghichu_chuagiao == "");
                    else//chưa giao
                        list_all = list_all.Where(p => p.ngayban_kyhopdong == null && p.ghichu_chuagiao != "");
                }
                //lọc theo người bán
                List<string> list_taikhoan = new List<string>();
                foreach (ListItem item in ListBox2.Items)
                {
                    if (item.Selected)
                        list_taikhoan.Add(item.Value);
                }
                if (!list_taikhoan.Contains(""))//nếu tồn tại "": tất cả thì k lọc
                    list_all = list_all.Where(tk => list_taikhoan.Contains(tk.nguoibaogia));


                //lọc theo phân loại bài viết
                //List<string> list_phanloai_baiviet = new List<string>();
                //foreach (ListItem item in ListBox1.Items)
                //{
                //    if (item.Selected)
                //        list_phanloai_baiviet.Add(item.Value);
                //}
                //if (!list_phanloai_baiviet.Contains(""))//nếu tồn tại "": tất cả thì k lọc
                //    list_all = list_all.Where(tk => list_phanloai_baiviet.Contains(tk.phanloai));

                //sắp xếp
                list_all = list_all.OrderByDescending(p => p.ngaybaogia);
                int _Tong_Record = list_all.Count();

                if (_Tong_Record != 0)
                {
                    ViewState["TongThanhTien"] = list_all.Sum(p => p.TongTien).ToString("#,##0");
                    ViewState["TongGiam"] = list_all.Sum(p => p.TongGiam).ToString("#,##0");
                    ViewState["TongSauGiam"] = list_all.Sum(p => p.TongSauGiam).ToString("#,##0");
                    ViewState["TongTien_VAT"] = list_all.Sum(p => p.TongTien_VAT).ToString("#,##0");
                    ViewState["TongSauThue"] = list_all.Sum(p => p.TongSauThue).ToString("#,##0");

                    ViewState["TongĐonaBan"] = list_all.Count(p => p.ngayban_kyhopdong != null);
                    ViewState["TongDoanhThu"] = list_all.Where(p => p.ngayban_kyhopdong != null).Sum(p => p.TongSauThue);
                    ViewState["TongCongNo"] = list_all.Where(p => p.ngayban_kyhopdong != null).Sum(p => p.congno);
                    ViewState["TongDaThanhToan"] = Convert.ToInt64(ViewState["TongDoanhThu"]) - Convert.ToInt64(ViewState["TongCongNo"]);
                }
                else
                {
                    ViewState["TongThanhTien"] = "0";
                    ViewState["TongGiam"] = "0";
                    ViewState["TongSauGiam"] = "0";
                    ViewState["TongTien_VAT"] = "0";
                    ViewState["TongSauThue"] = "0";
                }
                #endregion


                #region xuất vào excel
                // Sử dụng EPPlus để tạo một tệp Excel và ghi dữ liệu vào đó
                using (ExcelPackage package = new ExcelPackage())
                {
                    int _cot = 1;//đánh dấu là cột 1
                                 //đặt tên sheet
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet 1");
                    //ghi tiêu đề
                    foreach (ListItem item in check_list_excel.Items)//duyệt qua hết check list mục cần xuất
                    {
                        if (item.Selected)//nếu mục này có chọn
                        {
                            worksheet.Cells[1, _cot].Value = item.Text;
                            _cot = _cot + 1;
                        }
                    }
                    //hết vòng lặp thì cột bằng 1 lại
                    _cot = 1;
                    // Bắt đầu ghi dữ liệu từ dòng thứ 2
                    int _row = 2;

                    #region xác định dữ liệu chuẩn bị xuất (list_xuat). Là xuất tất cả hay các trang riêng lẻ đc chọn
                    // Ghi dữ liệu từ LINQ Query vào ExcelWorksheet
                    IEnumerable<dynamic> list_xuat;
                    if (check_all_page.Checked == true)//nếu chọn tất cả
                        list_xuat = list_all;
                    else//nếu chọn trang riêng lẻ
                    {
                        // Khởi tạo danh sách để lưu trữ dữ liệu xuất ra từ các trang cụ thể
                        List<dynamic> list_split = new List<dynamic>();
                        // Duyệt qua `selectedItems` để lấy giá trị trang cụ thể
                        foreach (ListItem selectedItem in selectedPage)
                        {
                            // Chuyển đổi giá trị của `ListItem` thành số trang (int)
                            int pageNumber = int.Parse(selectedItem.Value);

                            // Tính toán vị trí bắt đầu và kết thúc của trang cụ thể trong `list_all`
                            int itemsPerPage = Number_cl.Check_Int(txt_show.Text.Trim()); // Số lượng mục trên mỗi trang
                            int startIndex = (pageNumber - 1) * itemsPerPage; // Chỉ số bắt đầu của trang cụ thể trong `list_all`
                            int endIndex = startIndex + itemsPerPage;

                            // Lọc dữ liệu từ `list_all` cho trang cụ thể
                            var pageData = list_all.Skip(startIndex).Take(itemsPerPage);

                            // Thêm dữ liệu đã lọc vào danh sách `list_xuat`
                            list_split.AddRange(pageData);
                        }
                        list_xuat = list_split;
                    }
                    #endregion

                    foreach (var t in list_xuat)
                    {
                        _cot = 1;
                        // Chỉ lặp qua các mục đã được chọn trong `check_list_excel.Items`
                        foreach (ListItem item in check_list_excel.Items.Cast<ListItem>().Where(item => item.Selected))
                        {
                            string _tencot = item.Value;//lấy tên cột
                            switch (_tencot)
                            {
                                case "id":
                                    worksheet.Cells[_row, _cot].Value = t.id; _cot = _cot + 1;
                                    break;

                                case "HoTenNhanVien":
                                    worksheet.Cells[_row, _cot].Value = t.HoTenNhanVien; _cot = _cot + 1;
                                    break;
                                case "ten_khachhang":
                                    worksheet.Cells[_row, _cot].Value = t.ten_khachhang; _cot = _cot + 1;
                                    break;
                                case "sdt_khachhang":
                                    worksheet.Cells[_row, _cot].Value = t.sdt_khachhang; _cot = _cot + 1;
                                    break;
                                case "diachi_khachhang":
                                    worksheet.Cells[_row, _cot].Value = t.diachi_khachhang; _cot = _cot + 1;
                                    break;
                                case "trangthai":
                                    worksheet.Cells[_row, _cot].Value = t.trangthai; _cot = _cot + 1;
                                    break;
                                case "phantram_doanhso_now":
                                    worksheet.Cells[_row, _cot].Value = t.phantram_doanhso_now; _cot = _cot + 1;
                                    break;
                                case "thuongdoanhso":
                                    worksheet.Cells[_row, _cot].Value = t.thuongdoanhso; _cot = _cot + 1;
                                    break;
                                case "TongTien":
                                    worksheet.Cells[_row, _cot].Value = t.TongTien; _cot = _cot + 1;
                                    break;
                                case "TongGiam":
                                    worksheet.Cells[_row, _cot].Value = t.TongGiam; _cot = _cot + 1;
                                    break;
                                case "TongSauGiam":
                                    worksheet.Cells[_row, _cot].Value = t.TongSauGiam; _cot = _cot + 1;
                                    break;
                                case "vat":
                                    worksheet.Cells[_row, _cot].Value = t.vat; _cot = _cot + 1;
                                    break;
                                case "TongSauThue":
                                    worksheet.Cells[_row, _cot].Value = t.TongSauThue; _cot = _cot + 1;
                                    break;

                                case "ngayhethan":
                                    // Giả định t.ngaytao là thuộc tính DateTime hoặc DateTime?
                                    DateTime? ngayhethan = t.ngayhethan;

                                    if (ngayhethan.HasValue)
                                    {
                                        // Chuyển đổi DateTime thành chỉ ngày (ngayTao.Value.Date)
                                        DateTime onlyDate = ngayhethan.Value.Date;

                                        // Đặt giá trị ô là kiểu DateTime chỉ với ngày
                                        worksheet.Cells[_row, _cot].Value = onlyDate;

                                        // Định dạng số cho ô thành "dd/MM/yyyy"
                                        worksheet.Cells[_row, _cot].Style.Numberformat.Format = "dd/MM/yyyy";
                                    }
                                    else
                                    {
                                        // Nếu giá trị ngayTao là null, bạn có thể để trống ô đó hoặc xử lý theo cách khác
                                        worksheet.Cells[_row, _cot].Value = DBNull.Value; // Hoặc để trống, hoặc đặt giá trị mặc định
                                    }
                                    _cot = _cot + 1;
                                    break;
                                case "ngaybaogia":
                                    // Giả định t.ngaytao là thuộc tính DateTime hoặc DateTime?
                                    DateTime? ngaybaogia = t.ngaybaogia;

                                    if (ngaybaogia.HasValue)
                                    {
                                        // Chuyển đổi DateTime thành chỉ ngày (ngayTao.Value.Date)
                                        DateTime onlyDate = ngaybaogia.Value.Date;

                                        // Đặt giá trị ô là kiểu DateTime chỉ với ngày
                                        worksheet.Cells[_row, _cot].Value = onlyDate;

                                        // Định dạng số cho ô thành "dd/MM/yyyy"
                                        worksheet.Cells[_row, _cot].Style.Numberformat.Format = "dd/MM/yyyy";
                                    }
                                    else
                                    {
                                        // Nếu giá trị ngayTao là null, bạn có thể để trống ô đó hoặc xử lý theo cách khác
                                        worksheet.Cells[_row, _cot].Value = DBNull.Value; // Hoặc để trống, hoặc đặt giá trị mặc định
                                    }
                                    _cot = _cot + 1;
                                    break;
                                case "ngayban_kyhopdong":
                                    // Giả định t.ngaytao là thuộc tính DateTime hoặc DateTime?
                                    DateTime? ngayban_kyhopdong = t.ngayban_kyhopdong;

                                    if (ngayban_kyhopdong.HasValue)
                                    {
                                        // Chuyển đổi DateTime thành chỉ ngày (ngayTao.Value.Date)
                                        DateTime onlyDate = ngayban_kyhopdong.Value.Date;

                                        // Đặt giá trị ô là kiểu DateTime chỉ với ngày
                                        worksheet.Cells[_row, _cot].Value = onlyDate;

                                        // Định dạng số cho ô thành "dd/MM/yyyy"
                                        worksheet.Cells[_row, _cot].Style.Numberformat.Format = "dd/MM/yyyy";
                                    }
                                    else
                                    {
                                        // Nếu giá trị ngayTao là null, bạn có thể để trống ô đó hoặc xử lý theo cách khác
                                        worksheet.Cells[_row, _cot].Value = DBNull.Value; // Hoặc để trống, hoặc đặt giá trị mặc định
                                    }
                                    _cot = _cot + 1;
                                    break;
                                default: break;
                            }
                        }
                        _row++;
                    }
                    // Lưu tệp Excel vào đường dẫn đã chỉ định
                    string filePath = "/uploads/files/BanHang-" + str_cl.taoma_theothoigian() + ".xlsx";
                    package.SaveAs(new System.IO.FileInfo(Server.MapPath("~" + filePath)));
                    //Response.Redirect(filePath);

                    // URL bạn muốn chuyển hướng đến
                    string url = filePath;
                    // Script để mở trang mới trong tab mới
                    string script = $"window.open('{url}', '_blank');";
                    // Đăng ký script để thực thi sau khi UpdatePanel postback hoàn thành
                    ScriptManager.RegisterStartupScript(this, GetType(), "OpenNewTab", script, true);


                    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành co", "1000", "warning"), true);

                    #region tắt Update Panel thì dùng được. Nó dùng RAM để xử lý, k lưu file trên ổ cứng
                    // Sử dụng MemoryStream để lưu tệp Excel
                    //using (MemoryStream stream = new MemoryStream())
                    //{
                    //    // Lưu tệp Excel vào MemoryStream
                    //    package.SaveAs(stream);

                    //    // Lưu nội dung MemoryStream vào một tệp tạm thời trên máy chủ
                    //    string filePath = Path.Combine(Server.MapPath("~/uploads/files/"), "DanhMuc-" + str_cl.taoma_theothoigian() + ".xlsx");
                    //    File.WriteAllBytes(filePath, stream.ToArray());

                    //    // Đăng ký script JavaScript để tự động tải xuống tệp Excel
                    //    string script = $"window.location.href = '/uploads/files/DanhMuc-{str_cl.taoma_theothoigian()}.xlsx';";
                    //    ScriptManager.RegisterStartupScript(this, GetType(), "DownloadExcel", script, true);
                    //}
                    #endregion
                }
                #endregion

            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }

    }
    protected void check_all_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            // Kiểm tra trạng thái của checkbox "Chọn tất cả"
            bool isChecked = check_all_excel.Checked;

            // Đặt trạng thái của tất cả các mục trong CheckBoxList theo trạng thái của "Chọn tất cả"
            foreach (ListItem item in check_list_excel.Items)
            {
                item.Selected = isChecked;
            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    protected void check_list_excel_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            // Kiểm tra xem tất cả các mục trong CheckBoxList đã được chọn hay chưa
            bool allSelected = true;

            foreach (ListItem item in check_list_excel.Items)
            {
                if (!item.Selected)
                {
                    allSelected = false;
                    break;
                }
            }

            // Cập nhật trạng thái của check_all_page theo kết quả kiểm tra
            check_all_excel.Checked = allSelected;
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    protected void check_all_page_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            // Kiểm tra trạng thái của checkbox "Chọn tất cả"
            bool isChecked = check_all_page.Checked;

            // Đặt trạng thái của tất cả các mục trong CheckBoxList theo trạng thái của "Chọn tất cả"
            foreach (ListItem item in check_list_page.Items)
            {
                item.Selected = isChecked;
            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    protected void check_list_page_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            // Kiểm tra xem tất cả các mục trong CheckBoxList đã được chọn hay chưa
            bool allSelected = true;

            foreach (ListItem item in check_list_page.Items)
            {
                if (!item.Selected)
                {
                    allSelected = false;
                    break;
                }
            }

            // Cập nhật trạng thái của check_all_page theo kết quả kiểm tra
            check_all_page.Checked = allSelected;
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    #endregion

    #region ADD - EDIT - CHI TIẾT
    public void reset_control_add_edit()
    {
        try
        {
            Label1.Text = null; txt_vat.Text = "0"; txt_giamgia_kh.Text = "0";
        rd_loai_giamgia.SelectedValue = "phantram";
            txt_sdt.Text = ""; txt_ten_kh.Text = ""; txt_diachi_kh.Text = ""; PlaceHolder1.Visible = false; txt_songayhieuluc.Text = "30";
            Repeater2.DataSource = null;
            Repeater2.DataBind();
            ViewState["add_edit"] = null;
            DropDownList1.DataSource = null;
            DropDownList1.DataBind();
            txt_soluong.Text = "1";
            txt_giamgia_phantram.Text = "0";
            txt_so_seri.Text = "";
            txt_baohanh_thang.Text = "";
            txt_diengiai.Text = "";
            DropDownList2.DataSource = null;
            DropDownList2.DataBind();
            Repeater5.DataSource = null;
            Repeater5.DataBind();
            txt_sotien_thanhtoan_congno.Text = "0";
            Label2.Text = null;
            PlaceHolder3.Visible = false;
            PlaceHolder10.Visible = false;
            txt_ghichu_giaohang.Text = "";

            ViewState["TongThanhTien_ChiTiet"] = "0";
            ViewState["TongGiam_ChiTiet"] = "0";
            ViewState["TongSauGiam_ChiTiet"] = "0";
            ViewState["id_guide_chitiet"] = "";
            ViewState["giamgia_dacbiet"] = "0";
            ViewState["vat_chitiet"] = "0";
            ViewState["thanhtien_vat_chitiet"] = "0";
            ViewState["Tong_ThanhToan"] = 0;
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    protected void but_show_form_add_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("29", "29");
            //reset control
            reset_control_add_edit();

            ViewState["add_edit"] = "add";
            Label1.Text = "TẠO BÁO GIÁ";
            but_add_edit.Text = "TẠO BÁO GIÁ";

            PlaceHolder8.Visible = true;
            using (dbDataContext db = new dbDataContext())
            {
                var allCust = db.Data_KhachHang_tbs
                                .OrderBy(x => x.ten)
                                .AsEnumerable()
                                .Select(u => new { 
                                    id = u.id.ToString(), 
                                    CustomField = (string.IsNullOrEmpty(u.sdt) ? "(Không có SĐT)" : u.sdt) + " - " + u.ten 
                                })
                                .ToList();

                DropDownList2.DataSource = allCust;
                DropDownList2.DataValueField = "id";
                DropDownList2.DataTextField = "CustomField";
                DropDownList2.DataBind();
                DropDownList2.Items.Insert(0, new ListItem("Khách hàng đã báo giá", ""));
            }

            //hiện form add_edit trong updatePanel_add
            pn_add.Visible = !pn_add.Visible;
            up_add.Update();
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    protected void but_close_form_add_Click(object sender, EventArgs e)
    {
        try
        {
            //reset control
            reset_control_add_edit();
            //ẩn form
            pn_add.Visible = !pn_add.Visible;
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    //chỉnh sửa
    public void load_edit(dbDataContext db, string _idbg)
    {

        var q = db.KhoSanPham_tbs
      .OrderBy(p => p.ten)
      .Select(p => new { p.id, p.so_seri, DisplayText = p.ten + (p.so_seri != null && p.so_seri != "" ? " - " + p.so_seri : "") + " <span class='fg-red'>(" + (p.soluong_hientai != null ? p.soluong_hientai : 0) + ")</span>" });

        DropDownList1.Items.Clear();
        DropDownList1.Items.Add(new ListItem("Chọn", ""));
        foreach(var item in q.ToList())
        {
            ListItem li = new ListItem(item.DisplayText, item.id.ToString());
            if (!string.IsNullOrEmpty(item.so_seri))
            {
                li.Attributes.Add("data-seri", item.so_seri);
            }
            DropDownList1.Items.Add(li);
        }

        // Lấy danh sách chi tiết bao giá cùng thông tin sản phẩm
        var q_chitiet = from chitiet in db.BaoGia_ChiTiet_tbs
                        join sanpham in db.KhoSanPham_tbs
                        on chitiet.id_sanpham equals sanpham.id.ToString() into sanphamGroup
                        from sanpham in sanphamGroup.DefaultIfEmpty()
                        join ob4 in db.DuLieuNguon_tbs.Where(p => p.kyhieu == "donvitinh") on sanpham.donvitinh equals ob4.id.ToString() into DVTGroup
                        from ob4 in DVTGroup.DefaultIfEmpty()
                        join ob2 in db.DuLieuNguon_tbs
                                    .Where(p => p.kyhieu == "hangsanpham") on sanpham.id_hang equals ob2.id.ToString() into HangGroup
                        from ob2 in HangGroup.DefaultIfEmpty()
                        where chitiet.id_baogia == _idbg
                        select new
                        {
                            chitiet.id,
                            chitiet.id_baogia,
                            chitiet.id_sanpham,
                            chitiet.soluong,
                            chitiet.thanhtien,
                            chitiet.giaban_taithoidiemnay,
                            chitiet.TongSauGiam,
                            chitiet.giamgia_phantram,
                            chitiet.giamgia_thanhtien,
                            ten_sanpham = sanpham == null ? "" : (sanpham.ten + (sanpham.so_seri != null && sanpham.so_seri != "" ? " - " + sanpham.so_seri : "")),
                            anh = sanpham == null ? "" : sanpham.anh,
                            DVT = ob4.ten,
                            sanpham.thongso_kythuat,
                            VAT = sanpham.cohoadon,//true false
                            TenHang = ob2.ten,
                        };
        if (q_chitiet.Any())
        {
            var bg = db.BaoGia_tbs.FirstOrDefault(p => p.id.ToString() == _idbg);
            if (bg != null)
            {
                ViewState["pt_giamgiadacbiet"] = bg.pt_giamgiadacbiet ?? 0;
                ViewState["giamgia_dacbiet"] = bg.giamgiadacbiet ?? 0;
            }
            else
            {
                ViewState["pt_giamgiadacbiet"] = "0";
                ViewState["giamgia_dacbiet"] = "0";
            }

            ViewState["TongThanhTien_ChiTiet"] = (q_chitiet.Sum(p => (long?)p.thanhtien) ?? 0).ToString("#,##0");
            ViewState["TongGiam_ChiTiet"] = (q_chitiet.Sum(p => (long?)p.giamgia_thanhtien) ?? 0).ToString("#,##0");
            Int64 TongSauGiam_ChiTiet = q_chitiet.Sum(p => (long?)p.TongSauGiam) ?? 0;
            ViewState["TongSauGiam_ChiTiet"] = TongSauGiam_ChiTiet.ToString("#,##0");
            //nếu có giảm đặc biệt
            if (ViewState["giamgia_dacbiet"].ToString() != "0")
            {
                if (ViewState["vat_chitiet"].ToString() == "0") // nếu không có VAT
                {
                    ViewState["donhang_saugiamgia"] = TongSauGiam_ChiTiet - Int64.Parse(ViewState["giamgia_dacbiet"].ToString());
                    ViewState["thanhtien_vat_chitiet"] = 0;
                }
                else // nếu có VAT
                {
                    decimal vatRate = decimal.Parse(ViewState["vat_chitiet"].ToString()) / 100; // Tính tỷ lệ VAT
                    long giamGiaDacBiet = Int64.Parse(ViewState["giamgia_dacbiet"].ToString());
                    // Tính giá trị VAT
                    decimal tongTruocVAT = TongSauGiam_ChiTiet - giamGiaDacBiet;
                    decimal tienVAT = tongTruocVAT * vatRate;
                    ViewState["thanhtien_vat_chitiet"] = (Math.Round(tienVAT, 0)).ToString("#,##0"); // Làm tròn giá trị VAT
                    // Tính tổng sau giảm giá và VAT
                    ViewState["donhang_saugiamgia"] = (TongSauGiam_ChiTiet - giamGiaDacBiet) * (1 + vatRate);

                }
            }
            else//nếu k có giảm giá đặc biệt
            {
                if (ViewState["vat_chitiet"].ToString() == "0") // nếu không có VAT
                {
                    ViewState["donhang_saugiamgia"] = TongSauGiam_ChiTiet - Int64.Parse(ViewState["giamgia_dacbiet"].ToString());
                    ViewState["thanhtien_vat_chitiet"] = 0;
                }
                else // nếu có VAT
                {
                    decimal vatRate = decimal.Parse(ViewState["vat_chitiet"].ToString()) / 100; // Tính tỷ lệ VAT
                    // Tính giá trị VAT
                    decimal tongTruocVAT = TongSauGiam_ChiTiet - 0;
                    decimal tienVAT = tongTruocVAT * vatRate;
                    ViewState["thanhtien_vat_chitiet"] = Math.Round(tienVAT, 0).ToString("#,##0"); // Làm tròn giá trị VAT

                    // Tính tổng sau giảm giá và VAT
                    ViewState["donhang_saugiamgia"] = (TongSauGiam_ChiTiet) * (1 + vatRate);
                }
            }
        }
        else
        {

            ViewState["TongThanhTien_ChiTiet"] = "0";
            ViewState["TongGiam_ChiTiet"] = "0";
            ViewState["TongSauGiam_ChiTiet"] = "0";
        }
        Repeater2.DataSource = q_chitiet.OrderBy(p => p.ten_sanpham);
        Repeater2.DataBind();

    }
    public void load_congno(dbDataContext db, string _idbg, Int64 _congno, string _trangthai)
    {
        if (_trangthai == "Đã ký HĐ")
        {
            PlaceHolder3.Visible = true;//hiện lịch sử thanh toán
            var q_thanhtoan = from thanhtoan in db.LichSu_ThanhToan_tbs
                              join taiKhoan in db.taikhoan_tbs
                              on thanhtoan.nguoixacnhan equals taiKhoan.taikhoan
                              where thanhtoan.idbg == _idbg
                              select new
                              {
                                  // Chọn các trường bạn cần từ cả hai bảng
                                  thanhtoan.id,
                                  thanhtoan.ngay_thanhtoan,
                                  thanhtoan.nguoixacnhan,
                                  thanhtoan.sotien_thanhtoan,
                                  taiKhoan.hoten,
                              };
            Repeater5.DataSource = q_thanhtoan.OrderByDescending(p => p.ngay_thanhtoan);
            Repeater5.DataBind();
            txt_sotien_thanhtoan_congno.Text = _congno.ToString("#,##0");
            if (q_thanhtoan.Any())
                ViewState["Tong_ThanhToan"] = q_thanhtoan.Sum(p => p.sotien_thanhtoan);
            else
                ViewState["Tong_ThanhToan"] = 0;
            if (_congno == 0)
                Label2.Text = "Đã thanh toán đủ";
            else
                Label2.Text = "Còn thiếu " + _congno.ToString("#,##0");
        }
        else
        {
            PlaceHolder3.Visible = false;
            Repeater5.DataSource = null;
            Repeater5.DataBind();
            txt_sotien_thanhtoan_congno.Text = "0";
            ViewState["Tong_ThanhToan"] = 0;
        }
    }
    protected void but_show_chinhsua_Click(object sender, EventArgs e)
    {

        check_login_cl.check_login_admin("18", "19");
        ViewState["add_edit"] = "edit";
        Label1.Text = "CHỈNH SỬA BÁO GIÁ";
        but_add_edit.Text = "CẬP NHẬT BÁO GIÁ";
        PlaceHolder1.Visible = true;
        using (dbDataContext db = new dbDataContext())
        {
            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;
            PlaceHolder8.Visible = false;
            //truy vấn dữ liệu để sửa
            var q = db.BaoGia_tbs.FirstOrDefault(p => p.id.ToString() == _id);
            if (q != null)
            {
                ViewState["id_edit"] = _id;
                ViewState["id_guide_chitiet"] = q.id_guide.ToString().ToLower();
                if (q.giamgiadacbiet != 0)
                {
                    ViewState["giamgia_dacbiet"] = q.giamgiadacbiet;
                }
                else
                {
                    ViewState["giamgia_dacbiet"] = "0";
                }
                if (q.vat != 0)
                {
                    ViewState["vat_chitiet"] = q.vat;

                }
                else
                {
                    ViewState["vat_chitiet"] = "0";
                    ViewState["thanhtien_vat_chitiet"] = "0";
                }
                if (q.trangthai == "Đã ký HĐ")
                {
                    PlaceHolder10.Visible = true;
                    txt_ghichu_giaohang.Text = q.ghichu_chuagiao;
                }
                else
                {
                    PlaceHolder10.Visible = false;
                    txt_ghichu_giaohang.Text = "";
                }


                txt_sdt.Text = q.sdt_khachhang;
                txt_ten_kh.Text = q.ten_khachhang;
                txt_diachi_kh.Text = q.diachi_khachhang;
                
                if (q.pt_giamgiadacbiet.HasValue && q.pt_giamgiadacbiet.Value > 0)
                {
                    rd_loai_giamgia.SelectedValue = "phantram";
                    txt_giamgia_kh.Text = q.pt_giamgiadacbiet.Value.ToString();
                }
                else
                {
                    rd_loai_giamgia.SelectedValue = "sotien";
                    txt_giamgia_kh.Text = (q.giamgiadacbiet ?? 0).ToString("#,##0");
                }
                txt_vat.Text = q.vat.ToString();

                // Tính số ngày giữa ngày hết hạn và ngày báo giá
                if (q.ngayhethan.HasValue && q.ngaybaogia.HasValue)
                {
                    TimeSpan difference = q.ngayhethan.Value - q.ngaybaogia.Value;
                    int daysDifference = difference.Days; // Lấy số ngày
                    txt_songayhieuluc.Text = daysDifference.ToString();
                }
                else
                {
                    txt_songayhieuluc.Text = "0";
                }

                load_edit(db, _id);
                Int64 _congno = 0;
                if (q.congno != null)
                    _congno = q.congno.Value;
                load_congno(db, _id, _congno, q.trangthai);

                //hiện form add_edit trong updatePanel_add
                pn_add.Visible = !pn_add.Visible;
                up_add.Update();
            }
            else
                ViewState["id_edit"] = "";

        }

    }
    protected void but_close_chinhsua_Click(object sender, EventArgs e)
    {
        try
        {
            //reset control
            reset_control_add_edit();
            //ẩn form
            pn_add.Visible = !pn_add.Visible;
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    protected void but_add_edit_Click(object sender, EventArgs e)
    {
        try
        {

            #region Chuẩn bị dữ liệu
            string _sdt = str_cl.XuLy_SDT_NhapVao(txt_sdt.Text);
            string _ten_kh = str_cl.VietHoa_ChuCai_DauTien(txt_ten_kh.Text.Trim());
            string _diachi = txt_diachi_kh.Text.Trim();
            int _ngayhieuluc = Number_cl.Check_Int(txt_songayhieuluc.Text.Trim());
            int _vat = Number_cl.Check_Int(txt_vat.Text.Trim());
            Int64 _giamgia_dacbiet = 0;
            decimal? _pt_GiamGia = null;
            if (rd_loai_giamgia.SelectedValue == "phantram")
            {
                if (!string.IsNullOrEmpty(txt_giamgia_kh.Text.Trim()))
                {
                    decimal temp;
                    if (!decimal.TryParse(txt_giamgia_kh.Text.Trim(), out temp))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Phần trăm giảm giá không hợp lệ.", "false", "false", "OK", "alert", ""), true);
                        return;
                    }
                    _pt_GiamGia = temp;
                }
            }
            else
            {
                _giamgia_dacbiet = Number_cl.Check_Int64(txt_giamgia_kh.Text.Trim());
                _pt_GiamGia = 0; // Reset phần trăm để tính theo số tiền
            }
            DateTime _ngayhientai = DateTime.Now;
            #endregion

            using (dbDataContext db = new dbDataContext())
            {
                #region Kiểm tra ngoại lệ.
                if (str_cl.KiemTra_SDT(_sdt) == false)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "SĐT khách hàng không hợp lệ.", "false", "false", "OK", "alert", ""), true);
                    return;
                }
                if (_ten_kh == "")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập tên khách hàng.", "false", "false", "OK", "alert", ""), true);
                    return;
                }
                if (_diachi == "")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập địa chỉ khách hàng.", "false", "false", "OK", "alert", ""), true);
                    return;
                }
                if (_ngayhieuluc <= 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Ngày hiệu lực không hợp lệ.", "false", "false", "OK", "alert", ""), true);
                    return;
                }
                if (_vat < 0 || _vat > 100)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "VAT từ 0 đến 100.", "false", "false", "OK", "alert", ""), true);
                    return;
                }
                if (_pt_GiamGia.HasValue && (_pt_GiamGia.Value < 0 || _pt_GiamGia.Value > 100))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Phần trăm giảm giá phải từ 0 đến 100.", "false", "false", "OK", "alert", ""), true);
                    return;
                }
                #endregion

                if (ViewState["add_edit"].ToString() == "add")
                {
                    check_login_cl.check_login_admin("29", "29");//kiểm tra quyền
                    #region thêm mới báo giá
                    BaoGia_tb _ob = new BaoGia_tb();
                    _ob.id_guide = Guid.NewGuid();
                    _ob.sdt_khachhang = _sdt;
                    _ob.ten_khachhang = _ten_kh;
                    _ob.diachi_khachhang = _diachi;
                    _ob.ngaybaogia = _ngayhientai;
                    if (rd_loai_giamgia.SelectedValue == "phantram")
                    {
                        _ob.pt_giamgiadacbiet = (long)(_pt_GiamGia ?? 0);
                        _ob.giamgiadacbiet = 0;
                    }
                    else
                    {
                        _ob.pt_giamgiadacbiet = 0;
                        _ob.giamgiadacbiet = _giamgia_dacbiet;
                    }
                    _ob.vat = _vat;
                    _ob.ngayhethan = _ngayhientai.AddDays(_ngayhieuluc);
                    _ob.ThoiHan_BaoGia = txt_songayhieuluc.Text.Trim();
                    _ob.nguoibaogia = ViewState["taikhoan"].ToString();
                    _ob.trangthai = "Còn hiệu lực";
                    _ob.tongtien = 0;
                    _ob.giatri_thuc_donhang = 0;
                    _ob.congno = 0;
                    _ob.thuongdoanhso = 0; _ob.phantram_doanhso_now = 0;
                    db.BaoGia_tbs.InsertOnSubmit(_ob);
                    #endregion

                    #region cập nhật thông tin khách hàng (hoặc thêm mới)
                    Data_KhachHang_tb q = null;
                    
                    // Nếu người dùng có chọn khách hàng từ DropDownList (có ID)
                    string _id_khachhang_chon = DropDownList2.SelectedValue;
                    if (!string.IsNullOrEmpty(_id_khachhang_chon))
                    {
                        long _id_kh = Convert.ToInt64(_id_khachhang_chon);
                        q = db.Data_KhachHang_tbs.FirstOrDefault(p => p.id == _id_kh);
                    }

                    // Nếu không chọn từ dropdown, hoặc khách hàng ID không tồn tại, thì thử tìm theo SĐT mới nhập
                    if (q == null && !string.IsNullOrEmpty(_sdt))
                    {
                        q = db.Data_KhachHang_tbs.FirstOrDefault(p => p.sdt == _sdt);
                    }

                    Data_KhachHang_tb _ob1 = null;
                    if (q == null)
                    {
                        _ob1 = new Data_KhachHang_tb();
                        _ob1.sdt = _sdt; _ob1.ten = _ten_kh; _ob1.diachi = _diachi; _ob1.ngay_capnhat = _ngayhientai; _ob1.nhanvien_chamsoc = ViewState["taikhoan"].ToString();
                        if (rd_loai_giamgia.SelectedValue == "phantram") {
                            _ob1.pt_GiamGia = _pt_GiamGia ?? 0;
                        }
                        db.Data_KhachHang_tbs.InsertOnSubmit(_ob1);
                    }
                    else//nếu có rồi mà đổi thông tin thì cập nhật mới
                    {
                        q.sdt = _sdt; // Cập nhật sđt mới (nếu có thay đổi)
                        q.ten = _ten_kh;
                        q.diachi = _diachi;
                        q.nhanvien_chamsoc = ViewState["taikhoan"].ToString();
                        if (rd_loai_giamgia.SelectedValue == "phantram") {
                            q.pt_GiamGia = _pt_GiamGia;
                        }
                    }
                    #endregion

                    db.SubmitChanges();
                    
                    // Gán Id_KhachHnag sau khi SubmitChanges để lấy được ID của khách hàng mới (nếu có)
                    if (q != null)
                    {
                        _ob.Id_KhachHnag = q.id.ToString();
                    }
                    else if (_ob1 != null)
                    {
                        _ob.Id_KhachHnag = _ob1.id.ToString();
                    }
                    db.SubmitChanges();



                    PlaceHolder8.Visible = false;
                    #region show bảng chi tiết sau khi khởi tạo báo giá
                    if (_giamgia_dacbiet != 0)
                    {
                        ViewState["giamgia_dacbiet"] = _giamgia_dacbiet;
                    }
                    else
                    {
                        ViewState["giamgia_dacbiet"] = "0";
                    }
                    if (_vat != 0)
                    {
                        ViewState["vat_chitiet"] = _vat;
                        ViewState["thanhtien_vat_chitiet"] = "0";//mới tạo thì bằng 0
                    }
                    else
                    {
                        ViewState["vat_chitiet"] = "0";
                        ViewState["thanhtien_vat_chitiet"] = "0";
                    }
                    PlaceHolder1.Visible = true;
                    but_add_edit.Text = "CẬP NHẬT BÁO GIÁ";//chuyển qua chế độ chỉnh sửa
                    ViewState["add_edit"] = "edit";
                    ViewState["id_edit"] = _ob.id.ToString();//gán id vừa tạo để giữ form, tiếp tục edit báo giá
                    ViewState["id_guide_chitiet"] = _ob.id_guide.ToString().ToLower();
                    load_edit(db, _ob.id.ToString());
                    #endregion
                    #region cập nhật dữ liệu và update hiển thị
                    show_main();
                    up_main.Update();
                    PlaceHolder1.Visible = true;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
                    #endregion

                }
                else//edit
                {
                    check_login_cl.check_login_admin("18", "19");//kiểm tra quyền
                    #region chuẩn bị dữ liệu
                    var q_edit = db.BaoGia_tbs.FirstOrDefault(p => p.id.ToString() == ViewState["id_edit"].ToString());
                    if (q_edit != null)
                    {
                        if (check_login_cl.CheckQuyen(db, ViewState["taikhoan"].ToString(), "18"))//quyền sửa toàn bộ
                        {

                        }
                        else//quyền sửa riêng tư
                        {
                            if (q_edit.nguoibaogia != ViewState["taikhoan"].ToString())
                            {
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không có quyền sửa báo giá của người khác.", "false", "false", "OK", "alert", ""), true);
                                return;
                            }
                        }


                        #region cập nhật
                        BaoGia_tb _ob = q_edit;
                        if (q_edit.trangthai == "Đã ký HĐ")
                        {
                            _ob.ghichu_chuagiao = txt_ghichu_giaohang.Text;
                            db.SubmitChanges();
                            update_baogia(db, _ob.id.ToString());
                            load_edit(db, _ob.id.ToString());
                            show_main();
                            up_main.Update();
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Báo giá này đã được xác nhận đã bán. Chỉ có nội dung Ghi chú giao hàng được lưu.", "false", "false", "OK", "alert", ""), true);
                            return;
                        }
                        string _old_sdt = q_edit.sdt_khachhang; // Lấy sđt cũ trước khi cập nhật
                        _ob.sdt_khachhang = _sdt;
                        _ob.ten_khachhang = _ten_kh;
                        _ob.diachi_khachhang = _diachi;
                        if (rd_loai_giamgia.SelectedValue == "phantram")
                        {
                            _ob.pt_giamgiadacbiet = (long)(_pt_GiamGia ?? 0);
                            _ob.giamgiadacbiet = 0; // Sẽ được tính lại trong update_baogia
                        }
                        else
                        {
                            _ob.pt_giamgiadacbiet = 0;
                            _ob.giamgiadacbiet = _giamgia_dacbiet;
                        }
                        _ob.vat = _vat;
                        _ob.ngayhethan = q_edit.ngaybaogia.Value.AddDays(_ngayhieuluc);
                        _ob.ThoiHan_BaoGia = txt_songayhieuluc.Text.Trim();
                        if (_ob.ngayhethan.Value.Date < DateTime.Now.Date)//nếu đã hết hạn
                            _ob.trangthai = "Hết hiệu lực";
                        else
                            _ob.trangthai = "Còn hiệu lực";

                        if (_giamgia_dacbiet != 0)
                        {
                            ViewState["giamgia_dacbiet"] = _giamgia_dacbiet;
                        }
                        else
                        {
                            ViewState["giamgia_dacbiet"] = "0";
                        }
                        if (_vat != 0)
                        {
                            ViewState["vat_chitiet"] = _vat;
                            ViewState["donhang_saugiamgia"] = "0";//cứ để đại cho khỏi lỗi, đoạn sau chạy hàm Load_chitiet sẽ nạp lai giá trí
                        }
                        else
                        {
                            ViewState["vat_chitiet"] = "0";
                            ViewState["donhang_saugiamgia"] = "0";//cứ để đại cho khỏi lỗi, đoạn sau chạy hàm Load_chitiet sẽ nạp lai giá trí
                        }

                        #endregion

                        #region cập nhật thông tin khách hàng
                        Data_KhachHang_tb q = null;
                        
                        if (!string.IsNullOrEmpty(_old_sdt)) {
                            q = db.Data_KhachHang_tbs.FirstOrDefault(p => p.sdt == _old_sdt);
                        }
                        
                        if (q == null && !string.IsNullOrEmpty(_sdt)) {
                            q = db.Data_KhachHang_tbs.FirstOrDefault(p => p.sdt == _sdt);
                        }

                        if (q == null)
                        {
                            Data_KhachHang_tb _ob1 = new Data_KhachHang_tb();
                            _ob1.sdt = _sdt; _ob1.ten = _ten_kh; _ob1.diachi = _diachi; _ob1.ngay_capnhat = _ngayhientai;
                            if (rd_loai_giamgia.SelectedValue == "phantram") {
                                _ob1.pt_GiamGia = _pt_GiamGia ?? 0;
                            }
                            db.Data_KhachHang_tbs.InsertOnSubmit(_ob1);
                        }
                        else//nếu có rồi mà đổi thông tin thì cập nhật mới
                        {
                            q.sdt = _sdt; // Cập nhật sđt mới (nếu có thay đổi)
                            q.ten = _ten_kh;
                            q.diachi = _diachi;
                            if (rd_loai_giamgia.SelectedValue == "phantram") {
                                q.pt_GiamGia = _pt_GiamGia;
                            }
                        }
                        #endregion


                        #region cập nhật số lượng tại bảng chi tiết
                        foreach (RepeaterItem item in Repeater2.Items)
                        {
                            // Tìm các điều khiển TextBox và Label từ RepeaterItem
                            TextBox txt_sl_chitiet = (TextBox)item.FindControl("txt_sl_chitiet");
                            TextBox txt_giamgia_phantram_chitiet = (TextBox)item.FindControl("txt_giamgia_phantram_chitiet");
                            Label lbID_chitiet = (Label)item.FindControl("lbID_chitiet");

                            // Kiểm tra nếu cả TextBox và Label không null
                            if (txt_sl_chitiet != null && lbID_chitiet != null)
                            {
                                // Lấy ID và rank từ các điều khiển
                                string _id_chitiet = lbID_chitiet.Text;
                                int _sl = Number_cl.Check_Int(txt_sl_chitiet.Text.Trim());
                                decimal _giamgia_phantram = Number_cl.Check_Decimal(txt_giamgia_phantram_chitiet.Text.Trim());
                                if (_sl >= 0)
                                {
                                    var q_chitiet = db.BaoGia_ChiTiet_tbs.FirstOrDefault(p => p.id.ToString() == _id_chitiet);
                                    if (q_chitiet != null)
                                    {
                                        q_chitiet.soluong = _sl;
                                        q_chitiet.thanhtien = _sl * q_chitiet.giaban_taithoidiemnay;

                                        q_chitiet.giamgia_phantram = _giamgia_phantram;

                                        decimal _giamgia_he_so = _giamgia_phantram / 100;
                                        decimal thanhtienDecimal = Convert.ToDecimal(q_chitiet.thanhtien);
                                        decimal _giamgia_thanhtienDecimal = thanhtienDecimal * _giamgia_he_so;

                                        q_chitiet.giamgia_thanhtien = (Int64)Math.Round(_giamgia_thanhtienDecimal, 0);
                                        q_chitiet.TongSauGiam = q_chitiet.thanhtien - q_chitiet.giamgia_thanhtien;
                                    }
                                }
                            }
                        }
                        #endregion

                        db.SubmitChanges();

                        update_baogia(db, _ob.id.ToString());


                        #region cập nhật dữ liệu và update hiển thị
                        load_edit(db, _ob.id.ToString());
                        show_main();
                        up_main.Update();
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công", "1000", "warning"), true);
                        #endregion

                    }
                    #endregion
                }
            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    #endregion



    #region Lọc dữ liệu
    #region chọn ngày nhanh
    private void ResetButtonCss()
    {
        but_homqua.CssClass = "small mt-1 light";
        but_homnay.CssClass = "small mt-1 light";
        but_tuantruoc.CssClass = "small mt-1 light";
        but_tuannay.CssClass = "small mt-1 light";
        but_thangtruoc.CssClass = "small mt-1 light";
        but_thangnay.CssClass = "small mt-1 light";
        but_quytruoc.CssClass = "small mt-1 light";
        but_quynay.CssClass = "small mt-1 light";
        but_namtruoc.CssClass = "small mt-1 light";
        but_namnay.CssClass = "small mt-1 light";
    }
    protected void but_homqua_Click(object sender, EventArgs e)
    {
        ResetButtonCss(); but_homqua.CssClass = "small mt-1 info";
        txt_tungay.Text = DateTime.Now.Date.AddDays(-1).ToShortDateString();
        txt_denngay.Text = DateTime.Now.Date.AddDays(-1).ToShortDateString();
    }
    protected void but_homnay_Click(object sender, EventArgs e)
    {
        ResetButtonCss(); but_homnay.CssClass = "small mt-1 info";
        txt_tungay.Text = DateTime.Now.Date.ToString();
        txt_denngay.Text = DateTime.Now.Date.ToString();
    }
    protected void but_tuantruoc_Click(object sender, EventArgs e)
    {
        ResetButtonCss(); but_tuantruoc.CssClass = "small mt-1 info";
        txt_tungay.Text = dt_cl.return_ngaydautuan().AddDays(-7).ToShortDateString();//lấy ngày đầu tuần
        txt_denngay.Text = dt_cl.return_ngaydautuan().AddDays(-1).ToShortDateString();
    }
    protected void but_tuannay_Click(object sender, EventArgs e)
    {
        ResetButtonCss(); but_tuannay.CssClass = "small mt-1 info";
        txt_tungay.Text = dt_cl.return_ngaydautuan().ToShortDateString();//lấy ngày đầu tuần
        txt_denngay.Text = dt_cl.return_ngaycuoituan().ToShortDateString();
    }
    protected void but_thangtruoc_Click(object sender, EventArgs e)
    {
        ResetButtonCss(); but_thangtruoc.CssClass = "small mt-1 info";
        txt_tungay.Text = dt_cl.return_ngaydauthangtruoc(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToShortDateString();
        txt_denngay.Text = dt_cl.return_ngaycuoithangtruoc(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToShortDateString();
    }
    protected void but_thangnay_Click(object sender, EventArgs e)
    {
        ResetButtonCss(); but_thangnay.CssClass = "small mt-1 info";
        txt_tungay.Text = dt_cl.return_ngaydauthang(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToShortDateString();
        txt_denngay.Text = dt_cl.return_ngaycuoithang(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToShortDateString();
    }
    protected void but_namtruoc_Click(object sender, EventArgs e)
    {
        ResetButtonCss(); but_namtruoc.CssClass = "small mt-1 info";
        txt_tungay.Text = dt_cl.return_ngaydaunamtruoc(DateTime.Now.Year.ToString()).ToShortDateString();
        txt_denngay.Text = dt_cl.return_ngaycuoinamtruoc(DateTime.Now.Year.ToString()).ToShortDateString();
    }
    protected void but_namnay_Click(object sender, EventArgs e)
    {
        ResetButtonCss(); but_namnay.CssClass = "small mt-1 info";
        txt_tungay.Text = dt_cl.return_ngaydaunam(DateTime.Now.Year.ToString()).ToShortDateString();
        txt_denngay.Text = dt_cl.return_ngaycuoinam(DateTime.Now.Year.ToString()).ToShortDateString();
    }
    protected void but_quytruoc_Click(object sender, EventArgs e)
    {
        ResetButtonCss(); but_quytruoc.CssClass = "small mt-1 info";
        txt_tungay.Text = dt_cl.return_ngaydauquytruoc(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToShortDateString();
        txt_denngay.Text = dt_cl.return_ngaycuoiquytruoc(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToShortDateString();
    }
    protected void but_quynay_Click(object sender, EventArgs e)
    {
        ResetButtonCss(); but_quynay.CssClass = "small mt-1 info";
        txt_tungay.Text = dt_cl.return_ngaydauquynay(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToShortDateString();
        txt_denngay.Text = dt_cl.return_ngaycuoiquynay(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToShortDateString();
    }
    #endregion
    protected void but_show_form_loc_Click(object sender, EventArgs e)
    {
        try
        {
            pn_loc.Visible = !pn_loc.Visible;
            up_loc.Update();
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    protected void but_loc_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            if (Request.Cookies["cookie_qlbaogia1"] != null)//nếu có ck r thì lưu giá trị mới
            {
                HttpCookie _ck = Request.Cookies["cookie_qlbaogia1"];
                _ck["show"] = txt_show.Text;
                _ck["trang_hientai"] = ViewState["current_page_qlbaogia1"].ToString();
                _ck["id_loctheothoigian"] = ddl_thoigian.SelectedValue;
                _ck["id_loc3"] = DropDownList3.SelectedValue;
                _ck["id_loc4"] = DropDownList4.SelectedValue;
                _ck["id_loc5"] = DropDownList5.SelectedValue;
                _ck["tungay"] = txt_tungay.Text;
                _ck["denngay"] = txt_denngay.Text;
                #region lưu giá trị người báo giá
                List<string> _chon_1 = new List<string>();
                foreach (ListItem item in ListBox2.Items)
                {
                    if (item.Selected)
                    {
                        _chon_1.Add(item.Value);
                    }
                }
                if (_chon_1.Contains(""))
                    _ck["nguoibaogia"] = "";
                else
                    _ck["nguoibaogia"] = string.Join(",", _chon_1);
                #endregion
                #region lưu giá trị Phân loại
                List<string> _chon_phanloai = new List<string>();
                foreach (ListItem item in ListBox1.Items)
                {
                    if (item.Selected)
                    {
                        _chon_phanloai.Add(item.Value);
                    }
                }
                // Kiểm tra nếu "Tất cả" được chọn
                if (_chon_phanloai.Contains(""))//Tất cả được chọn
                    _ck["phanloai"] = "";//Tất cả phân loại
                else
                    _ck["phanloai"] = string.Join(",", _chon_phanloai);//Ví dụ: Sản phẩm,Tin tức nếu chọn Sản phẩm và Tin tức
                #endregion
                _ck.Expires = DateTime.Now.AddDays(1);
                Response.Cookies.Set(_ck); // Cập nhật lại cookie
            }
            ViewState["tungay"] = txt_tungay.Text; ViewState["denngay"] = txt_denngay.Text;
            show_main();
            up_main.Update();
            pn_loc.Visible = false;
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    protected void but_huy_loc_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            if (Request.Cookies["cookie_qlbaogia1"] != null)
                Response.Cookies["cookie_qlbaogia1"].Expires = DateTime.Now.AddYears(-1);
            Response.Redirect(Request.Url.AbsoluteUri, false);
            Context.ApplicationInstance.CompleteRequest();
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    #endregion

    protected void lnk_xoadong_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("18", "19");
            LinkButton button = (LinkButton)sender;
            Int64 _id = Int64.Parse(button.CommandArgument);

            using (dbDataContext db = new dbDataContext())
            {
                var dm = db.BaoGia_tbs.FirstOrDefault(p => p.id == _id);
                if (dm != null)
                {
                    if (dm.trangthai != "Đã ký HĐ")
                    {
                        if (check_login_cl.CheckQuyen(db, ViewState["taikhoan"].ToString(), "18"))//quyền sửa toàn bộ
                        {
                        }
                        else//quyền sửa riêng tư
                        {
                            if (dm.nguoibaogia != ViewState["taikhoan"].ToString())
                            {
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không có quyền xóa báo giá của người khác.", "false", "false", "OK", "alert", ""), true);
                                return;
                            }
                        }

                        db.BaoGia_tbs.DeleteOnSubmit(dm);

                        string _idbg = dm.id.ToString();
                        var q = db.BaoGia_ChiTiet_tbs.Where(p => p.id_baogia == _idbg);
                        foreach (var t in q)
                        {
                            db.BaoGia_ChiTiet_tbs.DeleteOnSubmit(t);
                        }
                        var q1 = db.LichSu_ThanhToan_tbs.Where(p => p.idbg == _idbg);
                        foreach (var t1 in q1)
                        {
                            db.LichSu_ThanhToan_tbs.DeleteOnSubmit(t1);
                        }
                        var q2 = db.NhapXuatKho_tbs.Where(p => p.id_baogia == _idbg);
                        foreach (var t1 in q2)
                        {
                            db.NhapXuatKho_tbs.DeleteOnSubmit(t1);
                        }

                        db.SubmitChanges();
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa báo giá thành công", "2000", "success"), true);
                        show_main();
                        up_main.Update();
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Không thể xóa báo giá đã ký HĐ.", "false", "false", "OK", "alert", ""), true);
                    }
                }
            }
        }
        catch (Exception)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Lỗi", "Lỗi trong quá trình xóa dữ liệu.", "2000", "alert"), true);
        }
    }
    #region BIN - XÓA - KHÔI PHỤC - LƯU

    protected void but_xoa_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("18", "19");

            var selectedIds = new List<Int64>(); // Danh sách để lưu trữ ID của các mục đã được chọn

            // Thu thập tất cả ID của các mục đã được chọn trong Repeater1
            foreach (RepeaterItem item in Repeater1.Items)
            {
                CheckBox chkItem = (CheckBox)item.FindControl("checkID");
                Label lblData = (Label)item.FindControl("lbID");

                if (chkItem != null && lblData != null && chkItem.Checked)
                {
                    Int64 id = Int64.Parse(lblData.Text);
                    selectedIds.Add(id); // Thêm ID vào danh sách
                }
            }

            if (selectedIds.Count > 0)
            {
                // Sử dụng dbDataContext và thực hiện cập nhật hàng loạt
                using (dbDataContext db = new dbDataContext())
                {


                    var ListsToUpdate = db.BaoGia_tbs
                        .Where(d => selectedIds.Contains(d.id))
                        .ToList();

                    foreach (var dm in ListsToUpdate)
                    {
                        if (dm.trangthai != "Đã ký HĐ")
                        {
                            if (check_login_cl.CheckQuyen(db, ViewState["taikhoan"].ToString(), "18"))//quyền sửa toàn bộ
                            {

                            }
                            else//quyền sửa riêng tư
                            {
                                if (dm.nguoibaogia != ViewState["taikhoan"].ToString())
                                {
                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không có quyền xóa báo giá của người khác.", "false", "false", "OK", "alert", ""), true);
                                    return;
                                }
                            }

                            db.BaoGia_tbs.DeleteOnSubmit(dm);

                            string _idbg = dm.id.ToString();
                            var q = db.BaoGia_ChiTiet_tbs.Where(p => p.id_baogia == _idbg);
                            foreach (var t in q)
                            {
                                db.BaoGia_ChiTiet_tbs.DeleteOnSubmit(t);
                            }
                            var q1 = db.LichSu_ThanhToan_tbs.Where(p => p.idbg == _idbg);
                            foreach (var t1 in q1)
                            {
                                db.LichSu_ThanhToan_tbs.DeleteOnSubmit(t1);
                            }
                            var q2 = db.NhapXuatKho_tbs.Where(p => p.id_baogia == _idbg);
                            foreach (var t1 in q2)
                            {
                                db.NhapXuatKho_tbs.DeleteOnSubmit(t1);
                            }
                        }
                    }

                    // Lưu tất cả các thay đổi trong một lần
                    db.SubmitChanges();
                }

                // Hiển thị thông báo thành công
                show_main();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
            }
            else
            {
                // Hiển thị thông báo không có mục nào được chọn
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Không có mục nào được chọn.", "false", "false", "OK", "alert", ""), true);
            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    //protected void but_save_Click(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        check_login_cl.check_login_admin("none", "none");
    //        // Tạo một danh sách để lưu trữ các cập nhật cần thực hiện
    //        var updates = new List<(int id, bool noibat)>();
    //        // Lấy thông tin từ Repeater1
    //        foreach (RepeaterItem item in Repeater1.Items)
    //        {
    //            // Tìm các điều khiển TextBox và Label từ RepeaterItem
    //            CheckBox CheckBox1 = (CheckBox)item.FindControl("CheckBox1");
    //            Label lblData = (Label)item.FindControl("lbID");

    //            // Kiểm tra nếu cả TextBox và Label không null
    //            if (CheckBox1 != null && lblData != null)
    //            {
    //                // Lấy ID và rank từ các điều khiển
    //                string _id = lblData.Text;
    //                bool _check_noibat = CheckBox1.Checked;

    //                // Thêm thông tin vào danh sách cập nhật
    //                updates.Add((id: int.Parse(_id), noibat: _check_noibat));
    //            }
    //        }
    //        // Cập nhật cơ sở dữ liệu một cách hàng loạt
    //        using (dbDataContext db = new dbDataContext())
    //        {
    //            // Truy vấn và lấy tất cả các mục cần cập nhật trong một lần
    //            var itemsToUpdate = db.BaiViet_tbs
    //                .Where(d => updates.Select(u => u.id).Contains(d.id))
    //                .ToList();

    //            // Cập nhật giá trị rank cho tất cả các mục trong danh sách danhMucsToUpdate
    //            foreach (var dm in itemsToUpdate)
    //            {
    //                var update = updates.First(u => u.id == dm.id);
    //                dm.noibat = update.noibat;
    //            }

    //            // Lưu các thay đổi vào cơ sở dữ liệu một lần
    //            db.SubmitChanges();
    //        }
    //        show_main();
    //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
    //    }
    //    catch (Exception _ex)
    //    {
    //        string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
    //        if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
    //        {
    //            _tk = mahoa_cl.giaima_Bcorn(_tk);
    //        }
    //        else
    //            _tk = "";
    //        Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
    //    }
    //    //using (dbDataContext db = new dbDataContext())
    //    //{
    //    //    foreach (RepeaterItem item in Repeater1.Items)
    //    //    {
    //    //        TextBox txt_giaban_1 = (TextBox)item.FindControl("txt_giaban_1");//tìm txt_name
    //    //        Label lblData = (Label)item.FindControl("lbID");//tìm ID
    //    //        if (txt_giaban_1 != null && lblData != null)//đảm bảo có Control
    //    //        {
    //    //            string _id = lblData.Text;//lấy được ID
    //    //            string _giaban = txt_giaban_1.Text.Replace(".", "");
    //    //            if (!string.IsNullOrEmpty(_giaban))//có dữ liệu mới xử lý
    //    //            {
    //    //                // Thực hiện các thao tác với ID tại đây
    //    //                var q = db.DanhMuc_tbs.FirstOrDefault(p => p.id.ToString() == _id);
    //    //                if (q != null)
    //    //                {
    //    //                    int _r1 = Number_cl.Check_Int(_giaban);
    //    //                    if (_r1 > 0)
    //    //                    {
    //    //                        DanhMuc_tb _ob = q;
    //    //                        _ob.rank = _r1;
    //    //                    }
    //    //                }
    //    //            }
    //    //        }
    //    //        db.SubmitChanges();
    //    //    }
    //    //}
    //    //show_main();
    //    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
    //}
    //protected void but_xoa_vinh_vien_only_Click(object sender, EventArgs e)
    //{
    //    //demo CommandArgument
    //    //check_login_cl.check_login_admin("none", "none");
    //    //LinkButton button = (LinkButton)sender;
    //    //string _id = button.CommandArgument;
    //}
    #endregion


    protected void but_check_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            string _id_val = DropDownList2.SelectedValue;
            if (string.IsNullOrEmpty(_id_val)) return;

            long _id = Convert.ToInt64(_id_val);
            var q = db.Data_KhachHang_tbs.FirstOrDefault(p => p.id == _id);
            if (q != null)
            {
                txt_sdt.Text = q.sdt ?? "";
                txt_ten_kh.Text = q.ten;
                txt_diachi_kh.Text = q.diachi;
                
                if (q.pt_GiamGia != null && q.pt_GiamGia > 0)
                {
                    rd_loai_giamgia.SelectedValue = "phantram";
                    txt_giamgia_kh.Text = q.pt_GiamGia.Value.ToString("0.##").Replace(",", ".");
                }
                else
                {
                    rd_loai_giamgia.SelectedValue = "sotien";
                    txt_giamgia_kh.Text = "0";
                }
            }
            //else
            //{
            //    txt_ten_kh.Text = "";
            //    txt_diachi_kh.Text = "";
            //}
        }
        up_add.Update();
    }


    protected void but_add_sp_chitiet_Click(object sender, EventArgs e)//bonbap
    {

        check_login_cl.check_login_admin("18", "19");
        string _idbg = ViewState["id_edit"].ToString();
        string _idsp = DropDownList1.SelectedValue.ToString();
        int _soluong_xuat = Number_cl.Check_Int(txt_soluong.Text.Trim());
        decimal _giamgia_phantram = Number_cl.Check_Decimal(txt_giamgia_phantram.Text.Trim());
        if (_idsp == "")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng chọn sản phẩm.", "false", "false", "OK", "alert", ""), true);
            return;
        }
        if (_soluong_xuat <= 0)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Số lượng xuất không hợp lệ.", "false", "false", "OK", "alert", ""), true);
            return;
        }
        if (_giamgia_phantram < 0 || _giamgia_phantram > 100)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Giảm giá phần trăm không hợp lệ. (Từ 0-100)", "false", "false", "OK", "alert", ""), true);
            return;
        }
        using (dbDataContext db = new dbDataContext())
        {
            var q_edit = db.BaoGia_tbs.FirstOrDefault(p => p.id.ToString() == ViewState["id_edit"].ToString());
            if (q_edit != null)
            {
                if (check_login_cl.CheckQuyen(db, ViewState["taikhoan"].ToString(), "18"))//quyền sửa toàn bộ
                {

                }
                else//quyền sửa riêng tư
                {
                    if (q_edit.nguoibaogia != ViewState["taikhoan"].ToString())
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không có quyền sửa báo giá của người khác.", "false", "false", "OK", "alert", ""), true);
                        return;
                    }
                }
                if (q_edit.trangthai == "Đã ký HĐ")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Không thể thao tác các báo giá đã được ký Hợp đồng.", "false", "false", "OK", "alert", ""), true);
                    return;
                }
            }

            var q_sp = db.KhoSanPham_tbs.FirstOrDefault(p => p.id.ToString() == _idsp);
            if (q_sp != null)
            {

                //KHI NÀO XUẤT HÀNG MỚI CẦN BÁO
                //int _soluong_hientai = q_sp.soluong_hientai.Value;
                //if (_soluong_hientai <= 0)
                //{
                //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Sản phẩm này đã hết hàng.", "false", "false", "OK", "alert", ""), true);
                //    return;
                //}
                //if (_soluong_xuat > _soluong_hientai)
                //{
                //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Trong kho chỉ còn " + _soluong_hientai + " sản phẩm này.", "false", "false", "OK", "alert", ""), true);
                //    return;
                //}
                var q_chitiet = db.BaoGia_ChiTiet_tbs.FirstOrDefault(p => p.id_baogia == _idbg);
                if (q_chitiet != null)
                {
                    if (q_chitiet.id_sanpham == _idsp)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Sản phẩm này đã được thêm vào bào giá này. Bạn có thể xóa hoặc cập nhật số lượng nếu muốn.", "false", "false", "OK", "alert", ""), true);
                        return;
                    }
                }


                #region thêm sp vào báo giá
                BaoGia_ChiTiet_tb _ob = new BaoGia_ChiTiet_tb();
                _ob.id_baogia = _idbg;
                _ob.id_sanpham = _idsp;
                _ob.soluong = _soluong_xuat;
                _ob.giaban_taithoidiemnay = q_sp.giabanle;
                _ob.thanhtien = _soluong_xuat * q_sp.giabanle;
                _ob.giamgia_phantram = _giamgia_phantram;

                decimal _giamgia_he_so = _giamgia_phantram / 100;
                decimal thanhtienDecimal = Convert.ToDecimal(_ob.thanhtien);
                decimal _giamgia_thanhtienDecimal = thanhtienDecimal * _giamgia_he_so;

                _ob.giamgia_thanhtien = (Int64)Math.Round(_giamgia_thanhtienDecimal, 0);

                _ob.TongSauGiam = _ob.thanhtien - _ob.giamgia_thanhtien;
                
                // Lấy thông tin Seri, Bảo hành, Diễn giải
                _ob.Thang_BaoHanh = txt_baohanh_thang.Text.Trim();
                _ob.Mota = txt_diengiai.Text.Trim();

                // Chèn đối tượng vào cơ sở dữ liệu
                db.BaoGia_ChiTiet_tbs.InsertOnSubmit(_ob);
                db.SubmitChanges();
                
                // Xóa nội dung các ô nhập sau khi thêm thành công
                txt_so_seri.Text = "";
                txt_baohanh_thang.Text = "";
                txt_diengiai.Text = "";
                #endregion

                update_baogia(db, _idbg);

                #region cập nhật dữ liệu và update hiển thị
                load_edit(db, _idbg);
                show_main();
                up_main.Update();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công", "1000", "warning"), true);
                #endregion
            }
        }

    }

    protected void but_xoachitiet_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_admin("18", "19");

        Button button = (Button)sender;
        string _id = button.CommandArgument;//lấy id_chitiet_baogia
        using (dbDataContext db = new dbDataContext())
        {
            var q_chitiet = db.BaoGia_ChiTiet_tbs.FirstOrDefault(p => p.id.ToString() == _id);
            if (q_chitiet != null)
            {
                string _idbg = q_chitiet.id_baogia;
                var q_bg = db.BaoGia_tbs.FirstOrDefault(p => p.id.ToString() == _idbg);
                if (check_login_cl.CheckQuyen(db, ViewState["taikhoan"].ToString(), "18"))//quyền sửa toàn bộ
                {

                }
                else//quyền sửa riêng tư
                {
                    if (q_bg.nguoibaogia != ViewState["taikhoan"].ToString())
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không có quyền xóa báo giá của người khác.", "false", "false", "OK", "alert", ""), true);
                        return;
                    }
                }
                if (q_bg.trangthai == "Đã ký HĐ")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Không thể thao tác các báo giá đã được ký Hợp đồng.", "false", "false", "OK", "alert", ""), true);
                    return;
                }

                db.BaoGia_ChiTiet_tbs.DeleteOnSubmit(q_chitiet);
                db.SubmitChanges();

                update_baogia(db, _idbg);

                #region cập nhật dữ liệu và update hiển thị
                load_edit(db, _idbg);
                show_main();
                up_main.Update();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công", "1000", "warning"), true);
                #endregion


            }
        }
    }

    #region xác nhân đã bán, đã ký hđ --> up file hđ --> trừ sl kho
    protected void but_show_form_daban_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_admin("26", "27");
        LinkButton button = (LinkButton)sender;
        string _idbg = ""; Int64 tongSauThue = 0;

        // Tách hai tham số bằng dấu '|'
        string[] parameters = button.CommandArgument.Split('|');
        _idbg = parameters[0]; ViewState["idbg_chitiet"] = _idbg;
        tongSauThue = Int64.Parse(parameters[1].Replace(".", ""));
        ViewState["tongSauThue"] = tongSauThue;

        using (dbDataContext db = new dbDataContext())
        {
            var q = db.BaoGia_tbs.FirstOrDefault(p => p.id.ToString() == _idbg);
            if (q != null)
            {
                if (check_login_cl.CheckQuyen(db, ViewState["taikhoan"].ToString(), "26"))//quyền  toàn bộ
                {

                }
                else//quyền sửa riêng tư
                {
                    if (q.nguoibaogia != ViewState["taikhoan"].ToString())
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không có quyền bán hàng từ báo giá của người khác.", "false", "false", "OK", "alert", ""), true);
                        return;
                    }
                }
                if (q.trangthai == "Đã ký HĐ")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Báo giá này đã được bán trước đó.", "false", "false", "OK", "alert", ""), true);
                    return;
                }
                if (q.trangthai == "Hết hiệu lực")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Báo giá này đã hết hiệu lực.", "false", "false", "OK", "alert", ""), true);
                    return;
                }
                txt_dathanhtoan.Text = tongSauThue.ToString("#,##0");
            }
        }
        pn_daban.Visible = !pn_daban.Visible;
        up_daban.Update();

    }
    protected void but_close_form_daban_Click(object sender, EventArgs e)
    {
        ViewState["idbg_chitiet"] = ""; txt_dathanhtoan.Text = ""; ViewState["tongSauThue"] = 0;
        txt_dathanhtoan.Text = string.Empty;
        txt_ghichu_chuagiao.Text = string.Empty;
        txt_link_fileupload.Text = string.Empty;
        pn_daban.Visible = !pn_daban.Visible;
    }
    protected void but_daban_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_admin("26", "27");


        string _idbg = ViewState["idbg_chitiet"].ToString();
        Int64 _thanhtoan = Number_cl.Check_Int64(txt_dathanhtoan.Text.Replace(".", ""));
        Int64 _tongsauthue = Number_cl.Check_Int64(ViewState["tongSauThue"].ToString());
        Int64 _congno = _tongsauthue - _thanhtoan;
        string _file_hd = txt_link_fileupload.Text.Trim();
        string _ghichu_chuagiao = txt_ghichu_chuagiao.Text.Trim();
        //if (_file_hd == "")
        //{
        //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng tải file hợp đồng lên.", "false", "false", "OK", "alert", ""), true);
        //    return;
        //}
        if (_thanhtoan < 0 || _thanhtoan > _tongsauthue)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Số tiền thanh toán không hợp lệ.", "false", "false", "OK", "alert", ""), true);
            return;
        }
        using (dbDataContext db = new dbDataContext())
        {
            // Lấy thông tin báo giá dựa trên ID
            var baoGia = db.BaoGia_tbs.FirstOrDefault(p => p.id.ToString() == _idbg);
            if (baoGia != null)
            {
                if (check_login_cl.CheckQuyen(db, ViewState["taikhoan"].ToString(), "26"))//quyền  toàn bộ
                {

                }
                else//quyền sửa riêng tư
                {
                    if (baoGia.nguoibaogia != ViewState["taikhoan"].ToString())
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không có quyền bán hàng từ báo giá của người khác.", "false", "false", "OK", "alert", ""), true);
                        return;
                    }
                }

                // Cập nhật thông tin báo giá
                baoGia.trangthai = "Đã ký HĐ";
                baoGia.ngayban_kyhopdong = DateTime.Now;
                baoGia.congno = _congno;
                baoGia.file_hopdong = _file_hd;
                baoGia.ghichu_chuagiao = _ghichu_chuagiao;

                //tính doanh số bán hàng
                string _nguoiban = baoGia.nguoibaogia;
                var q_nguoibaogia = db.taikhoan_tbs.First(p => p.taikhoan == _nguoiban);
                int _phantram = q_nguoibaogia.phantram_doanhso_banhang.Value;
                baoGia.phantram_doanhso_now = _phantram;
                Int64 _tongtien = baoGia.giatri_thuc_donhang.Value;
                Int64 _thuongdoanhso = _tongtien * _phantram / 100;
                baoGia.thuongdoanhso = _thuongdoanhso;

                // Thêm lịch sử thanh toán
                var lichSuThanhToan = new LichSu_ThanhToan_tb
                {
                    idbg = _idbg,
                    sotien_thanhtoan = _thanhtoan,
                    ngay_thanhtoan = DateTime.Now,
                    nguoixacnhan = ViewState["taikhoan"]?.ToString()
                };
                db.LichSu_ThanhToan_tbs.InsertOnSubmit(lichSuThanhToan);

                #region Kiểm tra và trừ số lượng sản phẩm
                // Lấy danh sách chi tiết báo giá
                var chiTietBaoGia = db.BaoGia_ChiTiet_tbs.Where(p => p.id_baogia == _idbg).ToList();

                // Danh sách sản phẩm không đủ số lượng
                var sanPhamThieu = new List<string>();

                foreach (var chiTiet in chiTietBaoGia)
                {
                    // Lấy thông tin sản phẩm trong kho
                    var sanPhamKho = db.KhoSanPham_tbs.FirstOrDefault(k => k.id.ToString() == chiTiet.id_sanpham);
                    if (sanPhamKho != null)
                    {
                        // Kiểm tra số lượng tồn
                        if (sanPhamKho.soluong_hientai < chiTiet.soluong)
                        {
                            sanPhamThieu.Add($"Sản phẩm {sanPhamKho.ten} không đủ số lượng. Tồn: {sanPhamKho.soluong_hientai} Xuất: {chiTiet.soluong}");
                        }
                        else
                        {
                            #region lưu lịch sử xuất, giảm sl tồn trong kho
                            NhapXuatKho_tb _ob = new NhapXuatKho_tb();
                            _ob.nhap_hay_xuat = false;//xuất
                            _ob.id_sanpham = chiTiet.id_sanpham;
                            _ob.ten_sanpham = sanPhamKho.ten;
                            _ob.soluong_nhap = chiTiet.soluong;
                            _ob.gia_nhap = chiTiet.giaban_taithoidiemnay;
                            _ob.ngaynhap = DateTime.Now;
                            _ob.nguoinhap = ViewState["taikhoan"].ToString();
                            _ob.ton_hientai = sanPhamKho.soluong_hientai;//tồn trước khi xuất
                            _ob.id_baogia = _idbg;
                            sanPhamKho.soluong_hientai = sanPhamKho.soluong_hientai - chiTiet.soluong;//giảm tồn hiện tại
                            db.NhapXuatKho_tbs.InsertOnSubmit(_ob);
                            #endregion
                        }
                    }
                }

                // Nếu có sản phẩm không đủ số lượng, hiển thị thông báo và dừng xử lý
                if (sanPhamThieu.Any())
                {
                    string thongBao = string.Join("<br>", sanPhamThieu);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", thongBao, "false", "false", "OK", "alert", ""), true);
                    return;
                }
                #endregion

                // Lưu thay đổi vào cơ sở dữ liệu
                db.SubmitChanges();

                // Hiển thị giao diện và thông báo thành công
                show_main();
                up_main.Update();
                ViewState["idbg_chitiet"] = string.Empty;
                txt_dathanhtoan.Text = string.Empty;
                txt_ghichu_chuagiao.Text = string.Empty;
                txt_link_fileupload.Text = string.Empty;
                pn_daban.Visible = !pn_daban.Visible;

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                    thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
            }
            else
            {
                // Thông báo nếu không tìm thấy báo giá
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                    thongbao_class.metro_notifi("Lỗi", "Không tìm thấy báo giá.", "2000", "error"), true);
            }
        }


    }
    #endregion





    protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            //lấy ra từng item
            var dataItem = (dynamic)e.Item.DataItem;

            // Tìm CheckBox1 và thiết lập Checked nếu là nổi bật
            var checkBox = (CheckBox)e.Item.FindControl("checkID");
            if (checkBox != null)
            {
                if (dataItem.trangthai == "Đã ký HĐ")
                {
                    checkBox.Visible = false;
                }
                //hoặc
                // Lấy giá trị cần so sánh từ DataItem (sửa 'ten_field' thành tên trường dữ liệu phù hợp)
                //string valueToCompare = DataBinder.Eval(dataItem, "Tên Cột")?.ToString() ?? string.Empty;
            }
        }
    }

    public void update_baogia(dbDataContext db, string _idbg)
    {
        var q = db.BaoGia_tbs.FirstOrDefault(p => p.id.ToString() == _idbg);
        if (q != null)
        {
            int _vat = q.vat ?? 0; // Đảm bảo không null
            Int64 _tongsaugiam = 0; // Tổng sau giảm từng sản phẩm

            // Lấy danh sách chi tiết báo giá
            var q_chitiet = db.BaoGia_ChiTiet_tbs.Where(p => p.id_baogia == _idbg);
            if (q_chitiet.Any())
            {
                _tongsaugiam = q_chitiet.Sum(p => p.TongSauGiam ?? 0); // Đảm bảo không null
            }

            // Tính giảm giá đặc biệt dựa trên pt_giamgiadacbiet của báo giá
            Int64 _giamgiadacbiet = q.giamgiadacbiet ?? 0;
            if (q.pt_giamgiadacbiet.HasValue && q.pt_giamgiadacbiet.Value > 0)
            {
                _giamgiadacbiet = (Int64)(_tongsaugiam * q.pt_giamgiadacbiet.Value / 100);
                q.giamgiadacbiet = _giamgiadacbiet;
            }

            // Tính VAT (chỉ khi _vat > 0)
            Int64 _tongvat = (_vat > 0) ? (_tongsaugiam - _giamgiadacbiet) * _vat / 100 : 0;

            // Cập nhật vào bảng báo giá
            q.tongtien = _tongsaugiam;
            q.giatri_thuc_donhang = _tongsaugiam - _giamgiadacbiet + _tongvat;

            db.SubmitChanges();
        }
    }


    protected void but_thanhtoan_congno_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_admin("26", "27");
        string _idbg = ViewState["id_edit"].ToString();
        Int64 _thanhtoan = Number_cl.Check_Int64(txt_sotien_thanhtoan_congno.Text);

        using (dbDataContext db = new dbDataContext())
        {
            // Lấy thông tin báo giá dựa trên ID
            var baoGia = db.BaoGia_tbs.FirstOrDefault(p => p.id.ToString() == _idbg);
            if (baoGia != null)
            {
                if (check_login_cl.CheckQuyen(db, ViewState["taikhoan"].ToString(), "26"))//quyền  toàn bộ
                {

                }
                else//quyền riêng tư
                {
                    if (baoGia.nguoibaogia != ViewState["taikhoan"].ToString())
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không có quyền thao tác trên báo giá của người khác.", "false", "false", "OK", "alert", ""), true);
                        return;
                    }
                }
                if (baoGia.trangthai == "Hết hiệu lực")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Báo giá này đã hết hiệu lực.", "false", "false", "OK", "alert", ""), true);
                    return;
                }
                if (baoGia.congno.Value == 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Đã thanh toán đủ.", "false", "false", "OK", "alert", ""), true);
                    return;
                }
                if (_thanhtoan <= 0 || _thanhtoan > baoGia.congno.Value)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Số tiền thanh toán không hợp lệ.", "false", "false", "OK", "alert", ""), true);
                    return;
                }
                // lịch sử thanh toán
                var lichSuThanhToan = new LichSu_ThanhToan_tb
                {
                    idbg = _idbg,
                    sotien_thanhtoan = _thanhtoan,
                    ngay_thanhtoan = DateTime.Now,
                    nguoixacnhan = ViewState["taikhoan"]?.ToString()
                };
                db.LichSu_ThanhToan_tbs.InsertOnSubmit(lichSuThanhToan);
                baoGia.congno = baoGia.congno - _thanhtoan;
                db.SubmitChanges();
                load_congno(db, _idbg, baoGia.congno.Value, baoGia.trangthai);
                show_main();
                up_main.Update();
                txt_sotien_thanhtoan_congno.Text = baoGia.congno.Value.ToString("#,##0");
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                  thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
            }
        }
    }



    void XuatExcelbyID(string id_BG)
    {
        //LinkButton button = (LinkButton)sender;
        //string id_BG = button.CommandArgument;

        using (dbDataContext db = new dbDataContext())
        {
            // Lấy chi tiết báo giá
            var q_chitiet = load_main(db, id_BG).ToList();

            // Chuyển chi tiết sang list dictionary
            var list_dynamic = q_chitiet.Select(x =>
            {
                var dict = new Dictionary<string, object>();
                foreach (var prop in x.GetType().GetProperties())
                {
                    dict[prop.Name] = prop.GetValue(x, null);
                }
                return dict;
            }).ToList();

            // Lấy thông tin chung
            string tenkh = "", sdtkh = "", diachikh = "", ngaybg = "", hanbg = "", sobg = "", nhanvienbg = "", sdtnv = "";
            string giamgia = "0", vat = "0";

            var q = db.BaoGia_tbs.FirstOrDefault(p => p.id.ToString().ToLower() == id_BG.ToLower());
            if (q != null)
            {
                tenkh = q.ten_khachhang;
                sdtkh = q.sdt_khachhang;
                diachikh = q.diachi_khachhang;
                ngaybg = q.ngaybaogia?.ToString("dd/MM/yyyy");
                hanbg = q.ngayhethan?.ToString("dd/MM/yyyy");
                sobg = q.id.ToString();

                var q_nv = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == q.nguoibaogia);
                if (q_nv != null)
                {
                    nhanvienbg = q_nv.hoten;
                    sdtnv = q_nv.dienthoai;
                }

                giamgia = (q.giamgiadacbiet ?? 0).ToString("#,##0");
                vat = (q.vat ?? 0).ToString("#,##0");
                ViewState["giamgia_dacbiet"] = q.giamgiadacbiet ?? 0;
                ViewState["vat_chitiet"] = q.vat ?? 0;
            }

            // Ghi thông tin báo giá dạng cột (1 key mỗi cột, value dòng sau)
            var info = new Dictionary<string, string>
            {
                { "Khách hàng", tenkh },
                { "SĐT Khách hàng", sdtkh },
                { "Địa chỉ", diachikh },
                { "Ngày báo giá", ngaybg },
                { "Hạn báo giá", hanbg },
                { "Số BG", sobg },
                { "Nhân viên báo giá", nhanvienbg },
                { "SĐT nhân viên", sdtnv },
                { "Giảm giá đặc biệt", giamgia },
                { "VAT (%)", vat },
                { "Tổng tiền trước thuế", ViewState["TongSauGiam_ChiTiet"]?.ToString() ?? "0" },
                { "Tiền VAT", ViewState["thanhtien_vat_chitiet"]?.ToString() ?? "0" },
                { "Tổng thanh toán", ViewState["donhang_saugiamgia"]?.ToString() ?? "0" }
            };

            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet("Báo giá");

            // Style: font in đậm
            var boldFont = workbook.CreateFont();
            boldFont.IsBold = true;
            var boldStyle = workbook.CreateCellStyle();
            boldStyle.SetFont(boldFont);

            // Style định dạng tiền
            var currencyStyle = workbook.CreateCellStyle();
            currencyStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0");

            // Style định dạng ngày tháng
            var dateStyle = workbook.CreateCellStyle();
            dateStyle.DataFormat = workbook.CreateDataFormat().GetFormat("dd/MM/yyyy");

            // Ghi info vào dòng 0 và 1 (theo cột)
            var rowTitle = sheet.CreateRow(0);
            var rowValue = sheet.CreateRow(1);
            int infoCol = 0;
            foreach (var item in info)
            {
                var cellKey = rowTitle.CreateCell(infoCol);
                cellKey.SetCellValue(item.Key);
                cellKey.CellStyle = boldStyle;

                var cellVal = rowValue.CreateCell(infoCol);
                // Gán theo key, để xác định kiểu dữ liệu đặc biệt
                string key = item.Key;
                string val = item.Value;

                if (key.Contains("SĐT") || key.ToLower().Contains("sdt"))
                {
                    cellVal.SetCellValue(val); // giữ nguyên chuỗi, không định dạng số
                }
                else if (DateTime.TryParseExact(val, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dt))
                {
                    cellVal.SetCellValue(dt);
                    cellVal.CellStyle = dateStyle;
                }
                else if (decimal.TryParse(val, out decimal num))
                {
                    cellVal.SetCellValue((double)num);
                    cellVal.CellStyle = currencyStyle;
                }
                else
                {
                    cellVal.SetCellValue(val);
                }

                infoCol++;
            }

            // Header sản phẩm bắt đầu từ dòng 3
            int detailStartRow = 3;

            var headers = new Dictionary<string, string>
            {
                { "id_baogia", "ID Báo giá" },
                { "ten_sanpham", "Tên sản phẩm" },
                { "TenHang", "Tên hãng" },
                { "thongso_kythuat", "Thông số kỹ thuật" },
                { "giaban_taithoidiemnay", "Giá bán" },
                { "DVT", "Đơn vị tính" },
                { "soluong", "Số lượng" },
                { "thanhtien", "Thành tiền" },
                { "giamgia_phantram", "Giảm giá (%)" },
                { "giamgia_thanhtien", "Giảm giá (VNĐ)" },
            };

            // Ghi header chi tiết sản phẩm
            var headerRow = sheet.CreateRow(detailStartRow);
            int colHeader = 0;
            foreach (var header in headers.Values)
            {
                var cell = headerRow.CreateCell(colHeader++);
                cell.SetCellValue(header);
                cell.CellStyle = boldStyle;
            }

            // Ghi data chi tiết sản phẩm
            int dataRow = detailStartRow + 1;
            foreach (var item in list_dynamic)
            {
                var row = sheet.CreateRow(dataRow++);
                int col = 0;
                foreach (var key in headers.Keys)
                {
                    var value = item.ContainsKey(key) ? item[key] : null;
                    var cell = row.CreateCell(col++);

                    if (value == null)
                    {
                        cell.SetCellValue("");
                    }
                    else if (value is DateTime dt)
                    {
                        cell.SetCellValue(dt);
                        cell.CellStyle = dateStyle;
                    }
                    else if (decimal.TryParse(value.ToString(), out decimal num))
                    {
                        cell.SetCellValue((double)num);
                        cell.CellStyle = currencyStyle;
                    }
                    else
                    {
                        cell.SetCellValue(value.ToString());
                    }
                }
            }

            // Tự resize cột
            for (int i = 0; i < info.Count + headers.Count; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            // Lưu file
            string folderPath = Server.MapPath("~/uploads/Files/");
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            string fileName = Guid.NewGuid() + ".xlsx";
            string filePath = Path.Combine(folderPath, fileName);
            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                workbook.Write(fs);
            }

            Response.Redirect("/uploads/Files/" + fileName);

        }
    }


    IQueryable<object> load_main(dbDataContext db, string _idbg)
    {
        var q_chitiet = from chitiet in db.BaoGia_ChiTiet_tbs
                        join sanpham in db.KhoSanPham_tbs
                        on chitiet.id_sanpham equals sanpham.id.ToString() into sanphamGroup
                        from sanpham in sanphamGroup.DefaultIfEmpty()
                        join ob4 in db.DuLieuNguon_tbs.Where(p => p.kyhieu == "donvitinh")
                        on sanpham.donvitinh equals ob4.id.ToString() into DVTGroup
                        from ob4 in DVTGroup.DefaultIfEmpty()
                        join ob2 in db.DuLieuNguon_tbs.Where(p => p.kyhieu == "hangsanpham")
                        on sanpham.id_hang equals ob2.id.ToString() into HangGroup
                        from ob2 in HangGroup.DefaultIfEmpty()
                        where chitiet.id_baogia == _idbg
                        select new
                        {
                            chitiet.id,
                            chitiet.id_baogia,
                            chitiet.id_sanpham,
                            chitiet.soluong,
                            chitiet.thanhtien,
                            chitiet.giaban_taithoidiemnay,
                            chitiet.TongSauGiam,
                            chitiet.giamgia_phantram,
                            chitiet.giamgia_thanhtien,
                            ten_sanpham = sanpham != null ? sanpham.ten : "",
                            anh = sanpham != null ? sanpham.anh : "",
                            DVT = ob4 != null ? ob4.ten : "",
                            thongso_kythuat = sanpham != null ? sanpham.thongso_kythuat : "",
                            TenHang = ob2 != null ? ob2.ten : ""
                        };

        if (q_chitiet.Any())
        {
            long tongThanhTien = q_chitiet.Sum(p => p.thanhtien ?? 0);
            long tongGiam = q_chitiet.Sum(p => p.giamgia_thanhtien ?? 0);
            long tongSauGiam = q_chitiet.Sum(p => p.TongSauGiam ?? 0);

            ViewState["TongThanhTien_ChiTiet"] = tongThanhTien.ToString("#,##0");
            ViewState["TongGiam_ChiTiet"] = tongGiam.ToString("#,##0");
            ViewState["TongSauGiam_ChiTiet"] = tongSauGiam.ToString("#,##0");

            // Parse giảm giá đặc biệt và VAT từ ViewState
            long giamGiaDacBiet = 0;
            decimal vatRate = 0;

            if (ViewState["giamgia_dacbiet"] != null)
                long.TryParse(ViewState["giamgia_dacbiet"].ToString(), out giamGiaDacBiet);

            if (ViewState["vat_chitiet"] != null)
            {
                decimal vatPercent;
                if (decimal.TryParse(ViewState["vat_chitiet"].ToString(), out vatPercent))
                {
                    vatRate = vatPercent / 100;
                }
            }

            // Tính toán
            decimal tongTruocVAT = tongSauGiam - giamGiaDacBiet;
            decimal tienVAT = tongTruocVAT * vatRate;
            decimal tongSauGiamVaVAT = tongTruocVAT * (1 + vatRate);

            ViewState["thanhtien_vat_chitiet"] = Math.Round(tienVAT, 0).ToString("#,##0");
            ViewState["donhang_saugiamgia"] = Math.Round(tongSauGiamVaVAT, 0);
        }
        else
        {
            ViewState["TongThanhTien_ChiTiet"] = "0";
            ViewState["TongGiam_ChiTiet"] = "0";
            ViewState["TongSauGiam_ChiTiet"] = "0";
            ViewState["thanhtien_vat_chitiet"] = "0";
            ViewState["donhang_saugiamgia"] = "0";
        }

        return q_chitiet;
    }

    protected void but_show_form_import_Click(object sender, EventArgs e)
    {
        if (pn_import.Visible == false)
        {
            pn_import.Visible = true;
            up_import.Update();
        }
        else
        {
            pn_import.Visible = false;
            up_import.Update();
        }
    }

    protected void but_import_excel_Click(object sender, EventArgs e)
    {
        Server.ScriptTimeout = 3600; // Tăng timeout lên 1 tiếng để import file lớn
        if (file_import.HasFile)
        {
            try
            {
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (var package = new OfficeOpenXml.ExcelPackage(file_import.PostedFile.InputStream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet != null)
                    {
                        dbDataContext db = new dbDataContext();
                        db.CommandTimeout = 3600;

                        int rowCount = worksheet.Dimension.Rows;
                        int importedCount = 0;
                        int errorCount = 0;
                        
                        var dictKhachHang = db.Data_KhachHang_tbs.AsEnumerable()
                            .GroupBy(k => (k.ten ?? "").ToUpper())
                            .ToDictionary(g => g.Key, g => g.First());
                            
                        var dictSanPham = db.KhoSanPham_tbs.AsEnumerable()
                            .GroupBy(s => ((s.model ?? "").ToUpper() + "|" + (s.ten ?? "").ToUpper() + "|" + (s.so_seri ?? "").ToUpper()))
                            .ToDictionary(g => g.Key, g => g.First());
                        
                        Func<string, DateTime?> parseDate = (str) =>
                        {
                            if (string.IsNullOrEmpty(str)) return null;
                            if (DateTime.TryParse(str, out DateTime dt)) return dt;
                            if (double.TryParse(str, out double oaDate)) return DateTime.FromOADate(oaDate);
                            return null;
                        };

                        for (int row = 3; row <= rowCount; row++)
                        {
                            try 
                            {
                                // Reset context mỗi 500 dòng để tránh đầy bộ nhớ và lỗi transaction
                                if (row > 3 && row % 500 == 0)
                                {
                                    db.Dispose();
                                    db = new dbDataContext();
                                    db.CommandTimeout = 3600;
                                }

                                string strNgay = (worksheet.Cells[row, 1].Text ?? "").Trim();
                                if (string.IsNullOrEmpty(strNgay) && string.IsNullOrEmpty((worksheet.Cells[row, 3].Text ?? "").Trim())) continue;

                                string strTenKH = (worksheet.Cells[row, 3].Text ?? "").Trim();
                                string strModel = (worksheet.Cells[row, 4].Text ?? "").Trim();
                                string strDienGiai = (worksheet.Cells[row, 5].Text ?? "").Trim();
                                string strTenMatHang = (worksheet.Cells[row, 6].Text ?? "").Trim();
                                string strSeri = (worksheet.Cells[row, 7].Text ?? "").Trim();
                                string strMaKH = (worksheet.Cells[row, 8].Text ?? "").Trim();
                                string strTH = (worksheet.Cells[row, 10].Text ?? "").Trim(); // J2 -> ThoiHan_BaoGia
                                string strK2 = (worksheet.Cells[row, 11].Text ?? "").Trim(); // K2 -> NgayDo_L1
                                string strL2 = (worksheet.Cells[row, 12].Text ?? "").Trim(); // L2 -> Thang_BaoHanh
                                string strM2 = (worksheet.Cells[row, 13].Text ?? "").Trim(); // M2 -> Seri_Do_L1
                                string strN2 = (worksheet.Cells[row, 14].Text ?? "").Trim(); // N2 -> Id_khacHang_do_L1
                                string strO2 = (worksheet.Cells[row, 15].Text ?? "").Trim(); // O2 -> NgayDo_L2
                                string strP2 = (worksheet.Cells[row, 16].Text ?? "").Trim(); // P2 -> Seri_Do_L2

                                string idKhachHang = null;
                                if (!string.IsNullOrEmpty(strTenKH))
                                {
                                    string keyKH = strTenKH.ToUpper();
                                    if (dictKhachHang.ContainsKey(keyKH))
                                    {
                                        idKhachHang = dictKhachHang[keyKH].id.ToString();
                                    }
                                    else
                                    {
                                        Data_KhachHang_tb newKH = new Data_KhachHang_tb();
                                        newKH.ten = strTenKH;
                                        newKH.ngay_capnhat = DateTime.Now;
                                        db.Data_KhachHang_tbs.InsertOnSubmit(newKH);
                                        db.SubmitChanges();
                                        idKhachHang = newKH.id.ToString();
                                        dictKhachHang[keyKH] = newKH;
                                    }
                                }

                                string idSanPham = null;
                                if (!string.IsNullOrEmpty(strModel) || !string.IsNullOrEmpty(strTenMatHang))
                                {
                                    string keySP = strModel.ToUpper() + "|" + strTenMatHang.ToUpper() + "|" + strSeri.ToUpper();
                                    if (dictSanPham.ContainsKey(keySP))
                                    {
                                        idSanPham = dictSanPham[keySP].id.ToString();
                                    }
                                    else
                                    {
                                        KhoSanPham_tb newSP = new KhoSanPham_tb();
                                        newSP.model = strModel;
                                        newSP.ten = !string.IsNullOrEmpty(strTenMatHang) ? strTenMatHang : (strModel != "" ? strModel : "Không rõ");
                                        newSP.so_seri = strSeri; // G2 -> so_seri bảng KhoSanPham_tb
                                        newSP.donvitinh = "45";
                                        db.KhoSanPham_tbs.InsertOnSubmit(newSP);
                                        db.SubmitChanges();
                                        idSanPham = newSP.id.ToString();
                                        dictSanPham[keySP] = newSP;
                                    }
                                }

                                BaoGia_tb bg = new BaoGia_tb();
                                bg.id_guide = Guid.NewGuid();
                                bg.ngaybaogia = parseDate(strNgay) ?? DateTime.Now;
                                bg.ngayban_kyhopdong = parseDate(strNgay) ?? DateTime.Now;
                                bg.Id_KhachHnag = idKhachHang;
                                bg.ten_khachhang = strTenKH;
                                bg.ghichu_chuagiao = ""; // H2 bị bỏ qua
                                bg.ThoiHan_BaoGia = strTH;
                                
                                bg.nguoibaogia = ViewState["taikhoan"] != null ? ViewState["taikhoan"].ToString() : "";
                                bg.trangthai = "Đã ký HĐ";
                                bg.tongtien = 0;
                                bg.giatri_thuc_donhang = 0;
                                bg.congno = 0;
                                bg.thuongdoanhso = 0;
                                bg.phantram_doanhso_now = 0;
                                bg.vat = 0;
                                bg.giamgiadacbiet = 0;
                                bg.pt_giamgiadacbiet = 0;

                                db.BaoGia_tbs.InsertOnSubmit(bg);
                                db.SubmitChanges();

                                BaoGia_ChiTiet_tb bgct = new BaoGia_ChiTiet_tb();
                                bgct.id_baogia = bg.id.ToString();
                                bgct.id_sanpham = idSanPham;
                                bgct.soluong = 1;
                                bgct.thanhtien = 0;
                                bgct.giamgia_phantram = 0;
                                bgct.giamgia_thanhtien = 0;
                                bgct.TongSauGiam = 0;
                                
                                // Fields moved to BaoGia_ChiTiet_tb
                                bgct.Mota = strDienGiai; // E2
                                bgct.NgayDo_L1 = parseDate(strK2); // K2
                                bgct.Thang_BaoHanh = strL2; // L2
                                bgct.Seri_Do_L1 = strM2; // M2
                                bgct.Id_khacHang_do_L1 = strN2; // N2
                                bgct.NgayDo_L2 = parseDate(strO2); // O2
                                bgct.Seri_Do_L2 = strP2; // P2

                                db.BaoGia_ChiTiet_tbs.InsertOnSubmit(bgct);
                                db.SubmitChanges();
                                
                                importedCount++;
                            }
                            catch (Exception innerEx)
                            {
                                errorCount++;
                                try {
                                    System.IO.File.AppendAllText(Server.MapPath("~/admin/quan-ly-bao-gia/import_error_log.txt"), "Lỗi ở dòng Excel " + row + ": " + innerEx.Message + "\r\n");
                                 } catch {}

                                // Reset context để bỏ qua các thay đổi lỗi
                                db.Dispose();
                                db = new dbDataContext();
                                db.CommandTimeout = 3600;
                            }
                        }
                        
                        db.Dispose();
                        
                        pn_import.Visible = false;
                        up_import.Update();

                        string msg = "Import thành công " + importedCount + " báo giá.";
                        if (errorCount > 0) msg += " Có " + errorCount + " dòng bị lỗi (đã ghi vào file import_error_log.txt).";
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", msg, "false", "false", "OK", "success", ""), true);
                        
                        show_main();
                        up_main.Update();
                    }
                }
            }
            catch (Exception ex)
            {
                string safeMsg = ex.Message.Replace("'", "\\'").Replace("\r", " ").Replace("\n", " ");
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Lỗi", "Lỗi: " + safeMsg, "false", "false", "OK", "alert", ""), true);
            }
        }
        else
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Lỗi", "Vui lòng chọn file.", "false", "false", "OK", "alert", ""), true);
        }
    }

    protected void but_xem_chitiet_baogia_Click(object sender, EventArgs e)
    {
        LinkButton btn = (LinkButton)sender;
        string id_baogia = btn.CommandArgument;
        if (string.IsNullOrEmpty(id_baogia)) return;

        using (dbDataContext db = new dbDataContext())
        {
            var q_baogia = db.BaoGia_tbs.FirstOrDefault(p => p.id.ToString() == id_baogia);
            if (q_baogia != null)
            {
                lbl_xc_khachhang.Text = q_baogia.ten_khachhang;
                lbl_xc_sdt.Text = q_baogia.sdt_khachhang;
                lbl_xc_diachi.Text = q_baogia.diachi_khachhang;
                lbl_xc_ngaybaogia.Text = q_baogia.ngaybaogia.HasValue ? q_baogia.ngaybaogia.Value.ToString("dd/MM/yyyy") : "";
                
                string ngayhethan = q_baogia.ngayhethan.HasValue ? q_baogia.ngayhethan.Value.ToString("dd/MM/yyyy") : "";
                string thoihan = string.IsNullOrEmpty(q_baogia.ThoiHan_BaoGia) ? "chưa rõ" : q_baogia.ThoiHan_BaoGia;
                lbl_xc_ngayhethan.Text = thoihan + (string.IsNullOrEmpty(ngayhethan) ? "" : " (Đến " + ngayhethan + ")");
                
                lbl_xc_trangthai.Text = q_baogia.trangthai;
                
                lbl_xc_tongtien.Text = q_baogia.tongtien.HasValue ? q_baogia.tongtien.Value.ToString("#,##0") : "0";
                
                long tongGiam = 0;
                if (q_baogia.pt_giamgiadacbiet > 0 && q_baogia.tongtien.HasValue) {
                    tongGiam = (long)((q_baogia.pt_giamgiadacbiet.Value / 100.0) * q_baogia.tongtien.Value);
                    lbl_xc_tonggiam.Text = q_baogia.pt_giamgiadacbiet.Value.ToString() + "%" + " (" + tongGiam.ToString("#,##0") + ")";
                } else if (q_baogia.giamgiadacbiet > 0) {
                    tongGiam = q_baogia.giamgiadacbiet.Value;
                    lbl_xc_tonggiam.Text = tongGiam.ToString("#,##0");
                } else {
                    lbl_xc_tonggiam.Text = "0";
                }
                
                long sauGiam = q_baogia.tongtien.HasValue ? (q_baogia.tongtien.Value - tongGiam) : 0;
                lbl_xc_tongsaugiam.Text = sauGiam.ToString("#,##0");

                var query_chitiet = (from c in db.BaoGia_ChiTiet_tbs
                                    join s in db.KhoSanPham_tbs on c.id_sanpham equals s.id.ToString() into joined
                                    from s in joined.DefaultIfEmpty()
                                    where c.id_baogia == id_baogia
                                    select new
                                    {
                                        TenSanPham = s != null ? (s.ten + (s.so_seri != null && s.so_seri != "" ? " - " + s.so_seri : "")) : "Không rõ",
                                        c.soluong,
                                        c.giaban_taithoidiemnay,
                                        c.giamgia_phantram,
                                        c.giamgia_thanhtien,
                                        c.TongSauGiam,
                                        So_Seri = s != null ? s.so_seri : "",
                                        c.Thang_BaoHanh,
                                        c.Mota,
                                        c.Seri_Do_L1,
                                        c.Id_khacHang_do_L1,
                                        c.NgayDo_L1,
                                        c.Seri_Do_L2,
                                        c.Id_khacHang_do_L2,
                                        c.NgayDo_L2
                                    }).ToList().Select(x => new
                                    {
                                        x.TenSanPham,
                                        x.soluong,
                                        x.giaban_taithoidiemnay,
                                        GiamGiaHienThi = (x.giamgia_phantram != null && x.giamgia_phantram > 0) ? x.giamgia_phantram + "%" : (x.giamgia_thanhtien != null && x.giamgia_thanhtien > 0) ? string.Format("{0:#,##0}", x.giamgia_thanhtien) : "0",
                                        x.TongSauGiam,
                                        x.So_Seri,
                                        x.Thang_BaoHanh,
                                        x.Mota,
                                        x.Seri_Do_L1,
                                        x.Id_khacHang_do_L1,
                                        x.NgayDo_L1,
                                        x.Seri_Do_L2,
                                        x.Id_khacHang_do_L2,
                                        x.NgayDo_L2
                                    }).ToList();
                Repeater_ChiTietBaoGia.DataSource = query_chitiet;
                Repeater_ChiTietBaoGia.DataBind();

                pn_xem_chitiet.Visible = true;
                up_xem_chitiet.Update();
            }
        }
    }

    protected void but_close_xem_chitiet_Click(object sender, EventArgs e)
    {
        pn_xem_chitiet.Visible = false;
        Repeater_ChiTietBaoGia.DataSource = null;
        Repeater_ChiTietBaoGia.DataBind();
        up_xem_chitiet.Update();
    }

    protected void but_import_makh_excel_Click(object sender, EventArgs e)
    {
        Server.ScriptTimeout = 3600;
        if (file_import.HasFile)
        {
            try
            {
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (var package = new OfficeOpenXml.ExcelPackage(file_import.PostedFile.InputStream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet != null)
                    {
                        dbDataContext db = new dbDataContext();
                        db.CommandTimeout = 3600;

                        int rowCount = worksheet.Dimension.Rows;
                        int updatedCount = 0;
                        int errorCount = 0;

                        var dictSanPhamBySeri = db.KhoSanPham_tbs.AsEnumerable()
                            .Where(s => !string.IsNullOrEmpty(s.so_seri))
                            .GroupBy(s => s.so_seri.ToUpper())
                            .ToDictionary(g => g.Key, g => g.First());

                        for (int row = 3; row <= rowCount; row++)
                        {
                            try
                            {
                                if (row > 3 && row % 500 == 0)
                                {
                                    db.Dispose();
                                    db = new dbDataContext();
                                    db.CommandTimeout = 3600;

                                    dictSanPhamBySeri = db.KhoSanPham_tbs.AsEnumerable()
                                        .Where(s => !string.IsNullOrEmpty(s.so_seri))
                                        .GroupBy(s => s.so_seri.ToUpper())
                                        .ToDictionary(g => g.Key, g => g.First());
                                }

                                string strSeri = worksheet.Cells[row, 7].Text?.Trim() ?? ""; // G2
                                string strMaKH = worksheet.Cells[row, 8].Text?.Trim() ?? ""; // H2

                                if (!string.IsNullOrEmpty(strSeri) && !string.IsNullOrEmpty(strMaKH))
                                {
                                    string keySeri = strSeri.ToUpper();
                                    if (dictSanPhamBySeri.ContainsKey(keySeri))
                                    {
                                        string idSanPham = dictSanPhamBySeri[keySeri].id.ToString();

                                        var chiTietList = db.BaoGia_ChiTiet_tbs.Where(c => c.id_sanpham == idSanPham).ToList();
                                        bool updated = false;
                                        foreach (var ct in chiTietList)
                                        {
                                            ct.MaKichHoat = strMaKH;
                                            updated = true;
                                        }

                                        if (updated)
                                        {
                                            db.SubmitChanges();
                                            updatedCount++;
                                        }
                                    }
                                }
                            }
                            catch (Exception innerEx)
                            {
                                errorCount++;
                                try
                                {
                                    System.IO.File.AppendAllText(Server.MapPath("~/admin/quan-ly-bao-gia/import_error_log.txt"), "Lỗi Cập nhật KH dòng " + row + ": " + innerEx.Message + "\r\n");
                                }
                                catch { }

                                db.Dispose();
                                db = new dbDataContext();
                                db.CommandTimeout = 3600;
                                dictSanPhamBySeri = db.KhoSanPham_tbs.AsEnumerable()
                                    .Where(s => !string.IsNullOrEmpty(s.so_seri))
                                    .GroupBy(s => s.so_seri.ToUpper())
                                    .ToDictionary(g => g.Key, g => g.First());
                            }
                        }

                        db.Dispose();

                        pn_import.Visible = false;
                        up_import.Update();

                        string msg = "Cập nhật thành công Mã KH cho " + updatedCount + " sản phẩm.";
                        if (errorCount > 0) msg += " Có " + errorCount + " dòng bị lỗi (đã ghi vào file import_error_log.txt).";
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", msg, "false", "false", "OK", "success", ""), true);

                        show_main();
                        up_main.Update();
                    }
                }
            }
            catch (Exception ex)
            {
                string safeMsg = ex.Message.Replace("'", "\\'").Replace("\r", " ").Replace("\n", " ");
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Lỗi", "Lỗi: " + safeMsg, "false", "false", "OK", "alert", ""), true);
            }
        }
        else
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Lỗi", "Vui lòng chọn file.", "false", "false", "OK", "alert", ""), true);
        }
    }
}
