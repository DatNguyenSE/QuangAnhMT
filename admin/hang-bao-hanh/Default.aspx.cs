using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OfficeOpenXml;
using System.IO;

public partial class admin_hang_bao_hanh_Default : System.Web.UI.Page
{ // ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Tài khoản đã bị khóa.", "false", "false", "OK", "alert", ""), true);
    String_cl str_cl = new String_cl();
    DateTime_cl dt_cl = new DateTime_cl();

    private sealed class DropdownOption
    {
        public string Id { get; set; }
        public string Text { get; set; }
    }

    private sealed class WarrantyListRow
    {
        public long id { get; set; }
        public DateTime? ngaytao { get; set; }
        public string HoTenNhanVien { get; set; }
        public string sdt_khachhang { get; set; }
        public string ten_khachhang { get; set; }
        public string diachi_khachhang { get; set; }
        public long TongTien { get; set; }
        public long TongGiam { get; set; }
        public long TongSauGiam { get; set; }
        public int vat { get; set; }
        public decimal TongTien_VAT { get; set; }
        public decimal TongSauThue { get; set; }
        public long congno { get; set; }
        public string ghichu { get; set; }
        public DateTime? NgayHenKhachTra { get; set; }
        public DateTime? ngaynhan { get; set; }
        public DateTime? NgayTra_ThucTe { get; set; }
        public string trangthai { get; set; }
        public bool trehen { get; set; }
        public string SoPhieuTra { get; set; }
        public string ChiTietSanPhamHtml { get; set; }
    }

    private sealed class DetailAggregate
    {
        public string IdPhieu { get; set; }
        public long TongTien { get; set; }
        public long TongGiamChiTiet { get; set; }
        public long TongSauGiamChiTiet { get; set; }
    }

    private static string BuildProductHtml(IEnumerable<HangBaoHanh_ChiTiet_tb> details)
    {
        if (details == null) return "";

        var list = details.ToList();
        if (list.Count == 0) return "";

        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < list.Count; i++)
        {
            var item = list[i];
            sb.Append("<div class='fw-600 text-blue'>")
              .Append(HttpUtility.HtmlEncode(item.ten ?? ""))
              .Append("</div>");
            sb.Append("<div class='text-small'>Seri: <b>")
              .Append(HttpUtility.HtmlEncode(item.seri ?? ""))
              .Append("</b> | SL: <b>")
              .Append(item.soluong ?? 0)
              .Append("</b></div>");

            if (i < list.Count - 1)
                sb.Append("<hr style='margin:4px 0;'/>");
        }
        return sb.ToString();
    }

    private static List<DropdownOption> GetCustomers(dbDataContext db)
    {
        var customers = db.Data_KhachHang_tbs
            .OrderBy(x => x.ten)
            .Select(x => new { x.id, x.sdt, x.ten })
            .ToList()
            .Select(x => new DropdownOption
            {
                Id = x.id.ToString(),
                Text = (string.IsNullOrEmpty(x.sdt) ? "(Không có SĐT)" : x.sdt) + " - " + x.ten
            })
            .ToList();
        return customers;
    }

    private static List<DropdownOption> GetProducts(dbDataContext db)
    {
        var products = db.KhoSanPham_tbs
            .OrderByDescending(p => p.id)
            .Select(p => new { p.id, p.ten, p.so_seri })
            .Take(1000)
            .ToList()
            .Select(p => new DropdownOption { Id = p.id.ToString(), Text = p.ten + " - " + p.so_seri })
            .ToList();
        return products;
    }

    public void set_dulieu_macdinh()
    {

        ResetButtonCss();//button chọn ngày nhanh
        txt_show.Text = "30";
        ViewState["current_page_hangbaohanh"] = "1";
        txt_tungay.Text = dt_cl.return_ngaydaunam(DateTime.Now.Year.ToString()).ToShortDateString();
        txt_denngay.Text = dt_cl.return_ngaycuoinam(DateTime.Now.Year.ToString()).ToShortDateString();
        ViewState["tungay"] = txt_tungay.Text;
        ViewState["denngay"] = txt_denngay.Text;




        #region set_get_cookie
        // Lấy cookie "cookie_hangbaohanh" từ Request.Cookies
        HttpCookie cookie = Request.Cookies["cookie_hangbaohanh"];
        if (cookie == null)
        {
            ListBox1.SelectedIndex = 0;//mặc định chọn tất cả phân loại, nếu select=true ngoài html thì k lưu lịch sử đc, kệ mẹ nó cứ làm y vậy đi, đừng quan tâm tới đoạn này
            ListBox2.SelectedIndex = 0;
            // Nếu cookie không tồn tại, tạo cookie mới
            cookie = new HttpCookie("cookie_hangbaohanh");
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
            ViewState["current_page_hangbaohanh"] = cookie["trang_hientai"];
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
    public string _id_phongban = "";
    public List<DuLieuNguon_tb> _listHang = null;
    public List<DuLieuNguon_tb> _listDVT = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["action"] == "importExcel")
        {
            HandleAjaxImport();
            return;
        }

        if (!IsPostBack)
        {

            Session["url_back"] = HttpContext.Current.Request.Url.AbsoluteUri;
            check_login_cl.check_login_admin("35", "35");

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

                var dataNguon = db.DuLieuNguon_tbs.Where(p => p.kyhieu == "hangsanpham" || p.kyhieu == "donvitinh").ToList();
                
                ddl_hang_add.DataSource = dataNguon.Where(p => p.kyhieu == "hangsanpham").OrderBy(p => p.ten).ToList();
                ddl_hang_add.DataValueField = "id";
                ddl_hang_add.DataTextField = "ten";
                ddl_hang_add.DataBind();
                ddl_hang_add.Items.Insert(0, new ListItem("Chọn hãng", ""));

                ddl_dvt_add.DataSource = dataNguon.Where(p => p.kyhieu == "donvitinh").OrderBy(p => p.ten).ToList();
                ddl_dvt_add.DataValueField = "id";
                ddl_dvt_add.DataTextField = "ten";
                ddl_dvt_add.DataBind();
                ddl_dvt_add.Items.Insert(0, new ListItem("Chọn ĐVT", ""));

                // Trạng thái trễ hẹn được tính khi đọc dữ liệu, tránh UPDATE toàn bảng lúc mở trang.
            }
            set_dulieu_macdinh();
            show_main();

            if (!string.IsNullOrWhiteSpace(Request.QueryString["id"]))
            {
                ViewState["id_to_home"] = Request.QueryString["id"];
                but_show_chinhsua_Click(sender, e);
            }
            else
                ViewState["id_to_home"] = null;
        }
    }
    #region main - phân trang - tìm kiếm

    public void show_main()
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                // Danh sách chỉ đọc: tắt tracking và deferred loading để giảm RAM/CPU.
                db.ObjectTrackingEnabled = false;
                db.DeferredLoadingEnabled = false;

                var phieuQuery = db.HangBaoHanh_tbs.AsQueryable();

                string key = txt_timkiem.Text.Trim();
                if (string.IsNullOrEmpty(key))
                    key = txt_timkiem1.Text.Trim();

                if (!string.IsNullOrEmpty(key))
                {
                    long searchId;
                    bool hasSearchId = long.TryParse(key, out searchId);

                    // Giữ nguyên phạm vi tìm kiếm cũ nhưng để SQL xử lý dưới dạng EXISTS,
                    // không kéo danh sách ID chi tiết lên RAM trước.
                    phieuQuery = phieuQuery.Where(p =>
                        p.ten_khachhang.Contains(key) ||
                        p.diachi_khachhang.Contains(key) ||
                        p.sdt_khachhang.Contains(key) ||
                        (hasSearchId && p.id == searchId) ||
                        db.HangBaoHanh_ChiTiet_tbs.Any(c =>
                            c.id_PhieuBaoHanh == p.id.ToString() &&
                            (c.seri.Contains(key) || c.so_phieu_tra.Contains(key)))
                    );
                }

                int totalRecords = phieuQuery.Count();
                int pageSize = Number_cl.Check_Int(txt_show.Text.Trim());
                if (pageSize <= 0) pageSize = 30;

                int currentPage;
                if (!int.TryParse(Convert.ToString(ViewState["current_page_hangbaohanh"]), out currentPage))
                    currentPage = 1;

                int totalPages = number_of_page_class.return_total_page(totalRecords, pageSize);
                if (totalPages <= 0) totalPages = 1;
                if (currentPage < 1) currentPage = 1;
                if (currentPage > totalPages) currentPage = totalPages;

                ViewState["current_page_hangbaohanh"] = currentPage.ToString();
                ViewState["total_page"] = totalPages;

                but_xemtiep.Enabled = currentPage < totalPages;
                but_xemtiep1.Enabled = currentPage < totalPages;
                but_quaylai.Enabled = currentPage > 1;
                but_quaylai1.Enabled = currentPage > 1;

                // Tổng hợp chi tiết ngay tại SQL: mỗi phiếu chỉ còn một dòng tổng,
                // thay vì tải toàn bộ chi tiết về RAM.
                if (totalRecords > 0)
                {
                    var detailAggregates = db.HangBaoHanh_ChiTiet_tbs
                        .GroupBy(c => c.id_PhieuBaoHanh)
                        .Select(g => new DetailAggregate
                        {
                            IdPhieu = g.Key,
                            TongTien = g.Sum(x => (long?)x.thanhtien) ?? 0,
                            TongGiamChiTiet = g.Sum(x => (long?)x.giamgia_thanhtien) ?? 0,
                            TongSauGiamChiTiet = g.Sum(x => (long?)x.TongSauGiam) ?? 0
                        })
                        .ToList()
                        .ToDictionary(x => x.IdPhieu ?? "", x => x);

                    var masterTotals = phieuQuery
                        .Select(p => new
                        {
                            p.id,
                            GiamGiaDacBiet = p.giamgiadacbiet ?? 0,
                            Vat = p.vat ?? 0
                        })
                        .ToList();

                    long tongThanhTien = 0;
                    long tongGiam = 0;
                    long tongSauGiam = 0;
                    decimal tongTienVat = 0;
                    decimal tongSauThue = 0;

                    foreach (var p in masterTotals)
                    {
                        DetailAggregate detail;
                        if (!detailAggregates.TryGetValue(p.id.ToString(), out detail))
                        {
                            detail = new DetailAggregate();
                        }

                        long sauGiam = detail.TongSauGiamChiTiet - p.GiamGiaDacBiet;
                        decimal tienVat = p.Vat != 0 ? sauGiam * ((decimal)p.Vat / 100m) : 0m;
                        decimal sauThue = p.Vat != 0 ? sauGiam * (1m + (decimal)p.Vat / 100m) : sauGiam;

                        tongThanhTien += detail.TongTien;
                        tongGiam += detail.TongGiamChiTiet + p.GiamGiaDacBiet;
                        tongSauGiam += sauGiam;
                        tongTienVat += tienVat;
                        tongSauThue += sauThue;
                    }

                    ViewState["TongThanhTien"] = tongThanhTien.ToString("#,##0");
                    ViewState["TongGiam"] = tongGiam.ToString("#,##0");
                    ViewState["TongSauGiam"] = tongSauGiam.ToString("#,##0");
                    ViewState["TongTien_VAT"] = Convert.ToInt64(tongTienVat).ToString("#,##0");
                    ViewState["TongSauThue"] = Convert.ToInt64(tongSauThue).ToString("#,##0");
                }
                else
                {
                    ViewState["TongThanhTien"] = "0";
                    ViewState["TongGiam"] = "0";
                    ViewState["TongSauGiam"] = "0";
                    ViewState["TongTien_VAT"] = "0";
                    ViewState["TongSauThue"] = "0";
                }

                // Chỉ lấy đúng dữ liệu master của trang hiện tại.
                var pagedPhieu = phieuQuery
                    .OrderByDescending(p => p.ngaytao)
                    .ThenByDescending(p => p.id)
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new
                    {
                        p.id,
                        p.ngaytao,
                        p.nguoitao,
                        p.sdt_khachhang,
                        p.ten_khachhang,
                        p.diachi_khachhang,
                        p.giamgiadacbiet,
                        p.vat,
                        p.congno,
                        p.ghichu,
                        p.NgayHenKhachTra,
                        p.ngaynhan,
                        p.NgayTra_ThucTe,
                        p.trangthai,
                        p.trehen
                    })
                    .ToList();

                var pageIdStrings = pagedPhieu.Select(p => p.id.ToString()).ToList();

                // Một truy vấn duy nhất lấy toàn bộ chi tiết của trang, loại bỏ N+1 query
                // từ GetChiTietSanPham trong ItemTemplate.
                var pageDetails = pageIdStrings.Count == 0
                    ? new List<HangBaoHanh_ChiTiet_tb>()
                    : db.HangBaoHanh_ChiTiet_tbs
                        .Where(c => pageIdStrings.Contains(c.id_PhieuBaoHanh))
                        .ToList();

                var detailLookup = pageDetails.ToLookup(c => c.id_PhieuBaoHanh ?? "");

                var accountNames = pagedPhieu
                    .Where(p => !string.IsNullOrEmpty(p.nguoitao))
                    .Select(p => p.nguoitao)
                    .Distinct()
                    .ToList();

                var accountLookup = accountNames.Count == 0
                    ? new Dictionary<string, string>()
                    : db.taikhoan_tbs
                        .Where(t => accountNames.Contains(t.taikhoan))
                        .Select(t => new { t.taikhoan, t.hoten })
                        .ToList()
                        .GroupBy(t => t.taikhoan)
                        .ToDictionary(g => g.Key, g => g.Select(x => x.hoten).FirstOrDefault() ?? "");

                var listSplit = new List<WarrantyListRow>(pagedPhieu.Count);
                foreach (var p in pagedPhieu)
                {
                    string idString = p.id.ToString();
                    var details = detailLookup[idString].ToList();

                    long tongTien = details.Sum(x => x.thanhtien ?? 0);
                    long tongGiamChiTiet = details.Sum(x => x.giamgia_thanhtien ?? 0);
                    long tongSauGiamChiTiet = details.Sum(x => x.TongSauGiam ?? 0);
                    long giamGiaDacBiet = p.giamgiadacbiet ?? 0;
                    long tongGiam = tongGiamChiTiet + giamGiaDacBiet;
                    long tongSauGiam = tongSauGiamChiTiet - giamGiaDacBiet;
                    int vat = p.vat ?? 0;
                    decimal tienVat = vat != 0 ? tongSauGiam * ((decimal)vat / 100m) : 0m;
                    decimal sauThue = vat != 0 ? tongSauGiam * (1m + (decimal)vat / 100m) : tongSauGiam;

                    string employeeName = "";
                    if (!string.IsNullOrEmpty(p.nguoitao))
                        accountLookup.TryGetValue(p.nguoitao, out employeeName);

                    listSplit.Add(new WarrantyListRow
                    {
                        id = p.id,
                        ngaytao = p.ngaytao,
                        HoTenNhanVien = employeeName ?? "",
                        sdt_khachhang = p.sdt_khachhang,
                        ten_khachhang = p.ten_khachhang,
                        diachi_khachhang = p.diachi_khachhang,
                        TongTien = tongTien,
                        TongGiam = tongGiam,
                        TongSauGiam = tongSauGiam,
                        vat = vat,
                        TongTien_VAT = tienVat,
                        TongSauThue = sauThue,
                        congno = p.congno ?? 0,
                        ghichu = p.ghichu,
                        NgayHenKhachTra = p.NgayHenKhachTra,
                        ngaynhan = p.ngaynhan,
                        NgayTra_ThucTe = p.NgayTra_ThucTe,
                        trangthai = p.trangthai,
                        trehen = p.trehen ?? false,
                        SoPhieuTra = details.Select(x => x.so_phieu_tra).FirstOrDefault(x => !string.IsNullOrEmpty(x)) ?? "",
                        ChiTietSanPhamHtml = BuildProductHtml(details)
                    });
                }

                int startRecord = totalRecords == 0 ? 0 : ((currentPage - 1) * pageSize) + 1;
                int endRecord = totalRecords == 0 ? 0 : startRecord + listSplit.Count - 1;

                lb_show.Text = totalRecords == 0
                    ? "0-0/0"
                    : startRecord + "-" + endRecord + " trong số " + totalRecords.ToString("#,##0");
                lb_show_md.Text = startRecord + "-" + endRecord + " trong số " + totalRecords.ToString("#,##0");

                Repeater1.DataSource = listSplit;
                Repeater1.DataBind();
            }
        }
        catch (Exception ex)
        {
            string account = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(account))
                account = mahoa_cl.giaima_Bcorn(account);
            else
                account = "";

            Log_cl.Add_Log(ex.Message, account, ex.StackTrace);
        }
    }

    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            ViewState["current_page_hangbaohanh"] = int.Parse(ViewState["current_page_hangbaohanh"].ToString()) - 1;
            #region LƯU TRANG HIỆN TẠI
            // Lấy cookie "cookie_hangbaohanh" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_hangbaohanh"];
            if (cookie != null)
            {
                cookie["trang_hientai"] = ViewState["current_page_hangbaohanh"].ToString();
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
            ViewState["current_page_hangbaohanh"] = int.Parse(ViewState["current_page_hangbaohanh"].ToString()) + 1;
            #region LƯU TRANG HIỆN TẠI
            // Lấy cookie "cookie_hangbaohanh" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_hangbaohanh"];
            if (cookie != null)
            {
                cookie["trang_hientai"] = ViewState["current_page_hangbaohanh"].ToString();
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
            ViewState["current_page_hangbaohanh"] = 1;
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



    #region ADD - EDIT - CHI TIẾT
    public void reset_control_add_edit()
    {
       
        Label1.Text = null; txt_maphieu_header.Text = "";
        txt_ngaynhan.Text = DateTime.Now.ToString("dd/MM/yyyy");
        txt_ngayhentra.Text = DateTime.Now.AddDays(3).ToString("dd/MM/yyyy");
        txt_sdt.Text = ""; txt_ten_kh.Text = ""; txt_diachi_kh.Text = "";
        
        // Repeater2 is removed
        // Repeater2.DataSource = null;
        // Repeater2.DataBind();
        ViewState["add_edit"] = null;
        txt_name.Text = ""; txt_anh1.Text = ""; txt_anh2.Text = ""; txt_anh3.Text = ""; img_anh1.ImageUrl = "/uploads/images/no-image.png"; img_anh2.ImageUrl = "/uploads/images/no-image.png"; img_anh3.ImageUrl = "/uploads/images/no-image.png";
        
        // Clear Single Product fields
        txt_sl_chitiet.Text = "1";
        txt_sotien_baohanh1.Text = "0";
        txt_seri.Text = "";
        txt_thoihan_baohanh.Text = "";
        txt_ghichu_sanpham.Text = "";
        ddl_huongxuly.SelectedIndex = 0;
        txt_noisua.Text = "";
        txt_madoitac.Text = "";
        txt_ngaymangsua.Text = "";
        txt_slmangsua.Text = "";
        txt_ngaysuave.Text = "";
        txt_slsuave.Text = "";
        txt_congnodoitac.Text = "0";
        txt_sophieutra.Text = "";
        txt_sltrakhach.Text = "";
        txt_congnotrakhach.Text = "0";
        ddl_trangthai_chitiet.SelectedIndex = 0;
        rd_trangthai_thanhtoan.SelectedIndex = 0;
        txt_ghichutrakhach.Text = "";
        txt_ngaytrathucte.Text = "";

        DropDownList2.DataSource = null;
        DropDownList2.DataBind();
        
        using (dbDataContext db = new dbDataContext())
        {
            DropDownList1.DataSource = GetProducts(db);
            DropDownList1.DataTextField = "Text";
            DropDownList1.DataValueField = "Id";
            DropDownList1.DataBind();
        }
        DropDownList1.Items.Insert(0, new ListItem("Tìm theo tên SP, số seri", ""));
        
        Repeater5.DataSource = null;
        Repeater5.DataBind();
        txt_sotien_thanhtoan_congno.Text = "0";
        Label2.Text = null;
        PlaceHolder3.Visible = false;
        txt_ghichu.Text = "";

        ViewState["TongThanhTien_ChiTiet"] = "0";
        ViewState["TongGiam_ChiTiet"] = "0";
        ViewState["TongSauGiam_ChiTiet"] = "0";
        ViewState["id_guide_chitiet"] = "";

        ViewState["thanhtien_vat_chitiet"] = "0";
        ViewState["Tong_ThanhToan"] = 0;

    }
    protected void but_show_form_add_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_admin("36", "36");
        
        using (dbDataContext db = new dbDataContext())
        {
            // Tự động tạo phiếu nháp ngay khi bấm thêm mới
            HangBaoHanh_tb _ob = new HangBaoHanh_tb();
            _ob.sdt_khachhang = "";
            _ob.ten_khachhang = "";
            _ob.diachi_khachhang = "";
            _ob.ngaynhan = DateTime.Now;
            _ob.NgayHenKhachTra = DateTime.Now.AddDays(7);
            
            _ob.pt_giamgiadacbiet = 0;
            _ob.giamgiadacbiet = 0;
            _ob.vat = 0;
            
            _ob.trangthai = "Đang xử lý";
            _ob.congno = 0;
            
            _ob.nguoitao = ViewState["taikhoan"] != null ? ViewState["taikhoan"].ToString() : "";
            _ob.ngaytao = DateTime.Now;
            _ob.tongtien = 0;
            _ob.giatri_thuc_donhang = 0;
            _ob.ghichu = "";
            _ob.trehen = false;
            _ob.phantram_doanhso_now = 0;
            
            db.HangBaoHanh_tbs.InsertOnSubmit(_ob);
            db.SubmitChanges();
            
            // Gọi but_show_chinhsua_Click bằng cách truyền CommandArgument qua sender
            LinkButton dummyBtn = new LinkButton();
            dummyBtn.CommandArgument = _ob.id.ToString();
            
            // Tạm thời vô hiệu hóa id_to_home (nếu có) để ép nó dùng CommandArgument
            object old_id_to_home = ViewState["id_to_home"];
            ViewState["id_to_home"] = null;
            
            but_show_chinhsua_Click(dummyBtn, e);
            
            ViewState["id_to_home"] = old_id_to_home;
        }
        
        // Đổi lại tiêu đề cho đúng ngữ cảnh thêm mới
        Label1.Text = "TẠO PHIẾU BẢO HÀNH";
    }
    protected void but_close_form_add_Click(object sender, EventArgs e)
    {
        ViewState["id_to_home"] = null;
        //reset control
        reset_control_add_edit();
        //ẩn form
        pn_add.Visible = !pn_add.Visible;

    }

    //chỉnh sửa
    public void load_edit(dbDataContext db, string _idbg)
    {
        try
        {
            // Removed KhoSanPham_tbs loading to optimize lag

            // Lấy 1 sản phẩm duy nhất của phiếu này
            var chitiet = db.HangBaoHanh_ChiTiet_tbs.FirstOrDefault(p => p.id_PhieuBaoHanh == _idbg);
            if (chitiet != null)
            {
                txt_name.Text = chitiet.ten;
                txt_sl_chitiet.Text = chitiet.soluong != null ? chitiet.soluong.Value.ToString() : "1";
                txt_sotien_baohanh1.Text = chitiet.sotien_baohanh != null ? chitiet.sotien_baohanh.Value.ToString("#,##0") : "0";
                txt_seri.Text = chitiet.seri;
                txt_thoihan_baohanh.Text = chitiet.thoi_han_baohanh;
                txt_ghichu_sanpham.Text = chitiet.ghichu_sanpham;
                
                string[] anhs = !string.IsNullOrEmpty(chitiet.anh) ? chitiet.anh.Split(',') : new string[0];
                txt_anh1.Text = anhs.Length > 0 ? anhs[0] : "";
                img_anh1.ImageUrl = anhs.Length > 0 && !string.IsNullOrEmpty(anhs[0]) ? anhs[0] : "/uploads/images/no-image.png";
                txt_anh2.Text = anhs.Length > 1 ? anhs[1] : "";
                img_anh2.ImageUrl = anhs.Length > 1 && !string.IsNullOrEmpty(anhs[1]) ? anhs[1] : "/uploads/images/no-image.png";
                txt_anh3.Text = anhs.Length > 2 ? anhs[2] : "";
                img_anh3.ImageUrl = anhs.Length > 2 && !string.IsNullOrEmpty(anhs[2]) ? anhs[2] : "/uploads/images/no-image.png";

                if (!string.IsNullOrEmpty(chitiet.huong_xuly) && ddl_huongxuly.Items.FindByValue(chitiet.huong_xuly) != null)
                    ddl_huongxuly.SelectedValue = chitiet.huong_xuly;
                else
                    ddl_huongxuly.SelectedIndex = 0;

                txt_noisua.Text = chitiet.noi_sua;
                txt_madoitac.Text = chitiet.ma_doitac_sua;
                txt_ngaymangsua.Text = chitiet.ngay_mang_sua != null ? chitiet.ngay_mang_sua.Value.ToShortDateString() : "";
                txt_slmangsua.Text = chitiet.sl_mang_sua != null ? chitiet.sl_mang_sua.Value.ToString() : "";
                txt_ngaysuave.Text = chitiet.ngay_sua_ve != null ? chitiet.ngay_sua_ve.Value.ToShortDateString() : "";
                txt_slsuave.Text = chitiet.sl_sua_ve != null ? chitiet.sl_sua_ve.Value.ToString() : "";
                txt_congnodoitac.Text = chitiet.congno_doitac != null ? chitiet.congno_doitac.Value.ToString("#,##0") : "0";

                txt_sophieutra.Text = chitiet.so_phieu_tra;
                txt_sltrakhach.Text = chitiet.sl_tra_khach != null ? chitiet.sl_tra_khach.Value.ToString() : "";
                txt_congnotrakhach.Text = chitiet.congno_trakhach != null ? chitiet.congno_trakhach.Value.ToString("#,##0") : "0";
                
                if (!string.IsNullOrEmpty(chitiet.trangthai_chitiet) && ddl_trangthai_chitiet.Items.FindByValue(chitiet.trangthai_chitiet) != null)
                    ddl_trangthai_chitiet.SelectedValue = chitiet.trangthai_chitiet;
                else
                    ddl_trangthai_chitiet.SelectedIndex = 0;
                    
                if (!string.IsNullOrEmpty(chitiet.trangthai_thanhtoan) && rd_trangthai_thanhtoan.Items.FindByValue(chitiet.trangthai_thanhtoan) != null)
                    rd_trangthai_thanhtoan.SelectedValue = chitiet.trangthai_thanhtoan;
                else
                    rd_trangthai_thanhtoan.SelectedIndex = 0;
                    
                txt_ghichutrakhach.Text = chitiet.ghichu_trakhach;

                // Tính TỔNG CỘNG cho ViewState (bảng tổng cộng dùng chung)
                ViewState["TongThanhTien_ChiTiet"] = chitiet.thanhtien != null ? chitiet.thanhtien.Value.ToString("#,##0") : "0";
                ViewState["TongGiam_ChiTiet"] = chitiet.giamgia_thanhtien != null ? chitiet.giamgia_thanhtien.Value.ToString("#,##0") : "0";
                Int64 TongSauGiam_ChiTiet = chitiet.TongSauGiam ?? 0;
                ViewState["TongSauGiam_ChiTiet"] = TongSauGiam_ChiTiet.ToString("#,##0");

                Int64 TongChiPhiSuaChua = (Int64)(chitiet.congno_doitac ?? 0);
                ViewState["TongChiPhiSuaChua_ChiTiet"] = TongChiPhiSuaChua.ToString("#,##0");

                ViewState["donhang_saugiamgia"] = TongSauGiam_ChiTiet + TongChiPhiSuaChua;
                ViewState["thanhtien_vat_chitiet"] = 0;

                var q = db.HangBaoHanh_tbs.FirstOrDefault(p => p.id.ToString() == _idbg);
                long giamGiaDacBiet = 0;
                decimal vatRate = 0;

                if (q != null)
                {
                    giamGiaDacBiet = q.giamgiadacbiet ?? 0;
                    vatRate = q.vat ?? 0;

                    ViewState["pt_giamgiadacbiet"] = q.pt_giamgiadacbiet ?? 0;
                    ViewState["giamgia_dacbiet"] = giamGiaDacBiet;
                    ViewState["vat_chitiet"] = vatRate;
                    
                    txt_ngaytrathucte.Text = q.NgayTra_ThucTe != null ? q.NgayTra_ThucTe.Value.ToShortDateString() : "";
                }

                long tongVAT = (long)((TongSauGiam_ChiTiet + TongChiPhiSuaChua - giamGiaDacBiet) * vatRate / 100);
                ViewState["thanhtien_vat_chitiet"] = tongVAT.ToString("#,##0");

                ViewState["donhang_saugiamgia"] = TongSauGiam_ChiTiet + TongChiPhiSuaChua - giamGiaDacBiet + tongVAT;
            }
            else
            {
                // Reset fields
                txt_name.Text = ""; txt_sl_chitiet.Text = "1"; txt_sotien_baohanh1.Text = "0"; txt_seri.Text = ""; txt_thoihan_baohanh.Text = ""; txt_ghichu_sanpham.Text = "";
                txt_anh1.Text = ""; txt_anh2.Text = ""; txt_anh3.Text = ""; img_anh1.ImageUrl = "/uploads/images/no-image.png"; img_anh2.ImageUrl = "/uploads/images/no-image.png"; img_anh3.ImageUrl = "/uploads/images/no-image.png"; ddl_huongxuly.SelectedIndex = 0;
                txt_noisua.Text = ""; txt_madoitac.Text = ""; txt_ngaymangsua.Text = ""; txt_slmangsua.Text = ""; txt_ngaysuave.Text = ""; txt_slsuave.Text = ""; txt_congnodoitac.Text = "0";
                txt_sophieutra.Text = ""; txt_sltrakhach.Text = ""; txt_congnotrakhach.Text = "0"; ddl_trangthai_chitiet.SelectedIndex = 0; rd_trangthai_thanhtoan.SelectedIndex = 0; txt_ghichutrakhach.Text = ""; txt_ngaytrathucte.Text = "";

                ViewState["TongThanhTien_ChiTiet"] = "0";
                ViewState["TongGiam_ChiTiet"] = "0";
                ViewState["TongSauGiam_ChiTiet"] = "0";
                ViewState["TongChiPhiSuaChua_ChiTiet"] = "0";
                ViewState["pt_giamgiadacbiet"] = "0";
                ViewState["giamgia_dacbiet"] = "0";
                ViewState["vat_chitiet"] = "0";
                ViewState["donhang_saugiamgia"] = "0";
            }
        }
        catch (Exception _ex)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", _ex.StackTrace, "false", "false", "OK", "alert", ""), true);
            return;
        }

    }
    public void load_congno(dbDataContext db, string _idbg, Int64 _congno, string _trangthai)
    {
        PlaceHolder3.Visible = true;

        var thanhToanList = (from thanhtoan in db.HangBaoHanh_LichSu_ThanhToan_tbs
                             join taiKhoan in db.taikhoan_tbs
                                 on thanhtoan.nguoixacnhan equals taiKhoan.taikhoan
                             where thanhtoan.id_PhieuBaoHanh == _idbg
                             orderby thanhtoan.ngay_thanhtoan descending
                             select new
                             {
                                 thanhtoan.id,
                                 thanhtoan.ngay_thanhtoan,
                                 thanhtoan.nguoixacnhan,
                                 thanhtoan.sotien_thanhtoan,
                                 taiKhoan.hoten
                             }).ToList();

        Repeater5.DataSource = thanhToanList;
        Repeater5.DataBind();

        txt_sotien_thanhtoan_congno.Text = _congno.ToString("#,##0");
        ViewState["Tong_ThanhToan"] = thanhToanList.Sum(p => p.sotien_thanhtoan ?? 0);

        Label2.Text = _congno == 0
            ? "Đã thanh toán đủ"
            : "Còn thiếu " + _congno.ToString("#,##0");
    }

    protected void but_show_chinhsua_Click(object sender, EventArgs e)
    {
        reset_control_add_edit();
        check_login_cl.check_login_admin("37", "37");
        ViewState["add_edit"] = "edit";
        Label1.Text = "CHỈNH SỬA PHIẾU BẢO HÀNH";
        but_add_edit.Text = "CẬP NHẬT PHIẾU BẢO HÀNH";
        PlaceHolder1.Visible = true;
        using (dbDataContext db = new dbDataContext())
        {
            string _id = "";
            if (ViewState["id_to_home"] != null)
            {
                _id = ViewState["id_to_home"].ToString();
            }
            else
            {
                LinkButton button = (LinkButton)sender;
                _id = button.CommandArgument;
            }
            //truy vấn dữ liệu để sửa
            var q = db.HangBaoHanh_tbs.FirstOrDefault(p => p.id.ToString() == _id);
            if (q != null)
            {
                ViewState["id_edit"] = _id;
                txt_ghichu.Text = q.ghichu;
                txt_sdt.Text = q.sdt_khachhang;
                txt_ten_kh.Text = q.ten_khachhang;
                txt_diachi_kh.Text = q.diachi_khachhang;
                txt_maphieu_header.Text = _id;
                txt_ngaynhan.Text = q.ngaynhan != null ? q.ngaynhan.Value.ToString("dd/MM/yyyy") : "";
                txt_ngayhentra.Text = q.NgayHenKhachTra != null ? q.NgayHenKhachTra.Value.ToString("dd/MM/yyyy") : "";
                
                // Bind new master fields
                if (!string.IsNullOrEmpty(q.trangthai))
                {
                    if (ddl_trangthai.Items.FindByValue(q.trangthai.Trim()) != null)
                        ddl_trangthai.SelectedValue = q.trangthai.Trim();
                }
                //txt_ngaytrathucte.Text = q.NgayTra_ThucTe != null ? q.NgayTra_ThucTe.Value.ToShortDateString() : "";
                txt_congno.Text = q.congno != null ? q.congno.Value.ToString("#,##0") : "0";
                //chk_trehen.Checked = q.trehen ?? false;

                if (q.pt_giamgiadacbiet != null && q.pt_giamgiadacbiet > 0)
                {
                    rd_loai_giamgia.SelectedValue = "phantram";
                    txt_giamgia_kh.Text = q.pt_giamgiadacbiet.Value.ToString();
                }
                else
                {
                    rd_loai_giamgia.SelectedValue = "sotien";
                    txt_giamgia_kh.Text = q.giamgiadacbiet != null ? q.giamgiadacbiet.Value.ToString("#,##0") : "0";
                }
                txt_vat.Text = q.vat != null ? q.vat.Value.ToString() : "0";

                 DropDownList2.DataSource = GetCustomers(db);
                 DropDownList2.DataValueField = "Id";
                 DropDownList2.DataTextField = "Text";
                DropDownList2.DataBind();
                DropDownList2.Items.Insert(0, new ListItem("Tìm thông tin khách hàng", ""));

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


    protected void but_add_edit_Click(object sender, EventArgs e)
    {
        try {
        #region Chuẩn bị dữ liệu
        string _sdt = str_cl.XuLy_SDT_NhapVao(txt_sdt.Text);
        string _ten_kh = str_cl.VietHoa_ChuCai_DauTien(txt_ten_kh.Text.Trim());
        string _diachi = txt_diachi_kh.Text.Trim();
        DateTime _ngayhientai = DateTime.Now;
        DateTime _ngaynhan = DateTime.Now;
        if (!string.IsNullOrEmpty(txt_ngaynhan.Text))
        {
            DateTime.TryParseExact(txt_ngaynhan.Text.Trim(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out _ngaynhan);
        }
        
        DateTime _ngayhentra = DateTime.Now;
        if (!string.IsNullOrEmpty(txt_ngayhentra.Text))
        {
            DateTime.TryParseExact(txt_ngayhentra.Text.Trim(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out _ngayhentra);
        }
        string _ghichu = txt_ghichu.Text.Trim();
        
        long _giamgia_dacbiet = 0;
        decimal? _pt_GiamGia = null;
        if (rd_loai_giamgia.SelectedValue == "phantram")
        {
            _pt_GiamGia = Number_cl.Check_Decimal(txt_giamgia_kh.Text);
        }
        else
        {
            _giamgia_dacbiet = Number_cl.Check_Int64(txt_giamgia_kh.Text);
        }
        int _vat = Number_cl.Check_Int(txt_vat.Text);

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
            #endregion

            if (ViewState["add_edit"].ToString() == "add")
            {
                check_login_cl.check_login_admin("36", "36");//kiểm tra quyền
                #region thêm mới báo giá
                HangBaoHanh_tb _ob = new HangBaoHanh_tb();
                _ob.sdt_khachhang = _sdt;
                _ob.ten_khachhang = _ten_kh;
                _ob.diachi_khachhang = _diachi;
                _ob.ngaynhan = _ngaynhan;
                _ob.NgayHenKhachTra = _ngayhentra;
                
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
                
                // Lưu master fields mới
                _ob.trangthai = ddl_trangthai.SelectedValue;
                _ob.congno = Number_cl.Check_Int64(txt_congno.Text.Trim());
                //_ob.trehen = chk_trehen.Checked;
                //if (!string.IsNullOrEmpty(txt_ngaytrathucte.Text.Trim())) {
                //    _ob.NgayTra_ThucTe = DateTime.ParseExact(txt_ngaytrathucte.Text.Trim(), "dd/MM/yyyy", null);
                //} else {
                //    _ob.NgayTra_ThucTe = null;
                //}
                _ob.vat = _vat;
                
                _ob.nguoitao = ViewState["taikhoan"].ToString();
                _ob.ngaytao = _ngayhientai;
                _ob.trangthai = "Đã nhận";
                _ob.tongtien = 0;
                _ob.giatri_thuc_donhang = 0;
                _ob.congno = 0;
                _ob.ghichu = _ghichu;
                _ob.trehen = false;
                _ob.phantram_doanhso_now = 0;
                _ob.thuongdoanhso = 0;
                db.HangBaoHanh_tbs.InsertOnSubmit(_ob);
                #endregion

                // (Đã bỏ chức năng tự động thêm khách hàng mới theo yêu cầu)
                db.SubmitChanges();
 
                PlaceHolder8.Visible = false;


                #region show bảng chi tiết sau khi khởi tạo phiếu bán hàng
                ViewState["thanhtien_vat_chitiet"] = "0";//bonbap
                PlaceHolder1.Visible = true;
                but_add_edit.Text = "CẬP NHẬT PHIẾU BẢO HÀNH";//chuyển qua chế độ chỉnh sửa
                ViewState["add_edit"] = "edit";
                ViewState["id_edit"] = _ob.id.ToString();//gán id vừa tạo để giữ form, tiếp tục edit báo giá
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
                check_login_cl.check_login_admin("37", "37");//kiểm tra quyền
                #region chuẩn bị dữ liệu
                var q_edit = db.HangBaoHanh_tbs.FirstOrDefault(p => p.id.ToString() == ViewState["id_edit"].ToString());
                if (q_edit != null)
                {

                    #region cập nhật
                    HangBaoHanh_tb _ob = q_edit;
                    if (q_edit.trangthai == "Đã trả")
                    {
                        _ob.ghichu = txt_ghichu.Text;
                        db.SubmitChanges();
                        update_baogia(db, _ob.id.ToString());
                        load_edit(db, _ob.id.ToString());
                        show_main();
                        up_main.Update();
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Phiếu bảo hành này đã trả hàng cho khách. Chỉ có nội dung Ghi chú được lưu.", "false", "false", "OK", "alert", ""), true);
                        return;
                    }


                    _ob.sdt_khachhang = _sdt;
                    _ob.ten_khachhang = _ten_kh;
                    _ob.diachi_khachhang = _diachi;
                    _ob.ngaynhan = _ngaynhan;
                    _ob.NgayHenKhachTra = _ngayhentra;
                    _ob.ghichu = _ghichu;

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
                    
                    // Cập nhật master fields mới
                    _ob.trangthai = ddl_trangthai.SelectedValue;
                    _ob.congno = Number_cl.Check_Int64(txt_congno.Text.Trim());
                    //_ob.trehen = chk_trehen.Checked;
                    if (!string.IsNullOrEmpty(txt_ngaytrathucte.Text.Trim())) {
                        _ob.NgayTra_ThucTe = DateTime.ParseExact(txt_ngaytrathucte.Text.Trim(), "dd/MM/yyyy", null);
                    } else {
                        _ob.NgayTra_ThucTe = null;
                    }

                    if (_ob.trangthai != "Đã trả")
                    {
                        if (_ob.NgayHenKhachTra.Value.Date < DateTime.Now.Date)//nếu đã hết hạn
                            _ob.trehen = true;
                        // Nếu chưa hết hạn thì giữ nguyên giá trị do người dùng check ở giao diện (vì khách có thể phàn nàn sớm)
                    }


                    #endregion

                    // (Đã bỏ chức năng tự động cập nhật khách hàng mới)

                    #region cập nhật chi tiết bảo hành (single product)
                    string _id_phieu = _ob.id.ToString();
                    var q_chitiet = db.HangBaoHanh_ChiTiet_tbs.FirstOrDefault(p => p.id_PhieuBaoHanh == _id_phieu);
                    bool isNewDetail = false;
                    if (q_chitiet == null)
                    {
                        q_chitiet = new HangBaoHanh_ChiTiet_tb();
                        q_chitiet.id_PhieuBaoHanh = _id_phieu;
                        isNewDetail = true;
                    }

                    int _sl = Number_cl.Check_Int(txt_sl_chitiet.Text.Trim());
                    Int64 _so_tien_bh = Number_cl.Check_Int64(txt_sotien_baohanh1.Text.Trim());
                    decimal _giamgia_phantram = 0; // Bỏ field này ở form mới
                    
                    q_chitiet.ten = txt_name.Text.Trim();
                    q_chitiet.soluong = _sl;
                    q_chitiet.sotien_baohanh = _so_tien_bh;
                    q_chitiet.thanhtien = _sl * _so_tien_bh;
                    q_chitiet.giamgia_phantram = _giamgia_phantram;
                    
                    decimal _giamgia_he_so = _giamgia_phantram / 100;
                    decimal thanhtienDecimal = Convert.ToDecimal(q_chitiet.thanhtien);
                    decimal _giamgia_thanhtienDecimal = thanhtienDecimal * _giamgia_he_so;
                    q_chitiet.giamgia_thanhtien = (Int64)Math.Round(_giamgia_thanhtienDecimal, 0);
                    q_chitiet.TongSauGiam = q_chitiet.thanhtien - q_chitiet.giamgia_thanhtien;
                    
                    q_chitiet.anh = string.Join(",", new[] { txt_anh1.Text.Trim(), txt_anh2.Text.Trim(), txt_anh3.Text.Trim() }.Where(s => !string.IsNullOrEmpty(s)));
                    q_chitiet.seri = txt_seri.Text.Trim();
                    q_chitiet.thoi_han_baohanh = txt_thoihan_baohanh.Text.Trim();
                    q_chitiet.ghichu_sanpham = txt_ghichu_sanpham.Text.Trim();
                    q_chitiet.huong_xuly = ddl_huongxuly.SelectedValue;
                    q_chitiet.noi_sua = txt_noisua.Text.Trim();
                    q_chitiet.ma_doitac_sua = txt_madoitac.Text.Trim();
                    
                    DateTime tempDate;
                    if(DateTime.TryParseExact(txt_ngaymangsua.Text.Trim(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out tempDate))
                        q_chitiet.ngay_mang_sua = tempDate;
                    else
                        q_chitiet.ngay_mang_sua = null;
                        
                    q_chitiet.sl_mang_sua = Number_cl.Check_Int(txt_slmangsua.Text.Trim());
                    
                    if(DateTime.TryParseExact(txt_ngaysuave.Text.Trim(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out tempDate))
                        q_chitiet.ngay_sua_ve = tempDate;
                    else
                        q_chitiet.ngay_sua_ve = null;
                        
                    q_chitiet.sl_sua_ve = Number_cl.Check_Int(txt_slsuave.Text.Trim());
                    q_chitiet.congno_doitac = Number_cl.Check_Int64(txt_congnodoitac.Text.Trim());
                    
                    q_chitiet.so_phieu_tra = txt_sophieutra.Text.Trim();
                    q_chitiet.sl_tra_khach = Number_cl.Check_Int(txt_sltrakhach.Text.Trim());
                    q_chitiet.congno_trakhach = Number_cl.Check_Int64(txt_congnotrakhach.Text.Trim());
                    
                    q_chitiet.trangthai_chitiet = ddl_trangthai_chitiet.SelectedValue;
                    q_chitiet.trangthai_thanhtoan = rd_trangthai_thanhtoan.SelectedValue;
                    q_chitiet.ghichu_trakhach = txt_ghichutrakhach.Text.Trim();

                    if (isNewDetail)
                    {
                        db.HangBaoHanh_ChiTiet_tbs.InsertOnSubmit(q_chitiet);
                    }
                    #endregion

                    db.SubmitChanges();
 
                    if (ViewState["id_to_home"] != null)
                        ViewState["id_to_home"] = null;


                    update_baogia(db, _ob.id.ToString());


                    #region cập nhật dữ liệu và update hiển thị
                    load_edit(db, _ob.id.ToString());
                    load_congno(db, _ob.id.ToString(), _ob.congno.Value, _ob.trangthai);
                    show_main();
                    up_main.Update();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công", "1000", "warning"), true);
                    #endregion

                }
                #endregion
            }
        }
        } // Đóng khối try
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Lỗi hệ thống", ex.Message + "\\n" + ex.StackTrace.Replace("\\n", " "), "false", "false", "OK", "alert", ""), true);
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
            if (Request.Cookies["cookie_hangbaohanh"] != null)//nếu có ck r thì lưu giá trị mới
            {
                HttpCookie _ck = Request.Cookies["cookie_hangbaohanh"];
                _ck["show"] = txt_show.Text;
                _ck["trang_hientai"] = ViewState["current_page_hangbaohanh"].ToString();
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
            if (Request.Cookies["cookie_hangbaohanh"] != null)
                Response.Cookies["cookie_hangbaohanh"].Expires = DateTime.Now.AddYears(-1);
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

    #region BIN - XÓA - KHÔI PHỤC - LƯU

    protected void but_xoa_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("38", "38");

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


                    var ListsToUpdate = db.HangBaoHanh_tbs
                        .Where(d => selectedIds.Contains(d.id))
                        .ToList();

                    foreach (var dm in ListsToUpdate)
                    {
                        if (dm.trangthai != "Đã trả")
                        {


                            db.HangBaoHanh_tbs.DeleteOnSubmit(dm);
 
                            string _idbg = dm.id.ToString();
                            var q = db.HangBaoHanh_ChiTiet_tbs.Where(p => p.id_PhieuBaoHanh == _idbg);
                            foreach (var t in q)
                            {
                                db.HangBaoHanh_ChiTiet_tbs.DeleteOnSubmit(t);
                            }

                            var q1 = db.HangBaoHanh_LichSu_ThanhToan_tbs.Where(p => p.id_PhieuBaoHanh == _idbg);
                            foreach (var t1 in q1)
                            {
                                db.HangBaoHanh_LichSu_ThanhToan_tbs.DeleteOnSubmit(t1);
                            }
                        }
                    }
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

    protected void lnk_xoadong_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("38", "38");
            LinkButton button = (LinkButton)sender;
            string _idbg = button.CommandArgument;

            using (dbDataContext db = new dbDataContext())
            {
                var dm = db.HangBaoHanh_tbs.FirstOrDefault(d => d.id.ToString() == _idbg);
                if (dm != null)
                {
                    if (dm.trangthai != "Đã trả")
                    {
                        db.HangBaoHanh_tbs.DeleteOnSubmit(dm);

                        var q = db.HangBaoHanh_ChiTiet_tbs.Where(p => p.id_PhieuBaoHanh == _idbg);
                        foreach (var t in q)
                        {
                            db.HangBaoHanh_ChiTiet_tbs.DeleteOnSubmit(t);
                        }

                        var q1 = db.HangBaoHanh_LichSu_ThanhToan_tbs.Where(p => p.id_PhieuBaoHanh == _idbg);
                        foreach (var t1 in q1)
                        {
                            db.HangBaoHanh_LichSu_ThanhToan_tbs.DeleteOnSubmit(t1);
                        }

                        db.SubmitChanges();
                        show_main();
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Phiếu bảo hành đã trả hàng không thể xóa.", "false", "false", "OK", "alert", ""), true);
                    }
                }
            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk))
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
    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            if (!string.IsNullOrEmpty(DropDownList1.SelectedValue))
            {
                string id_sp = DropDownList1.SelectedValue;
                var sp = db.KhoSanPham_tbs.FirstOrDefault(p => p.id.ToString() == id_sp);
                if (sp != null)
                {
                    txt_name.Text = sp.ten;
                    txt_seri.Text = sp.so_seri;
                    
                    string donvitinh_name = "";
                    if (!string.IsNullOrEmpty(sp.donvitinh)) 
                    {
                        var dvt = db.DuLieuNguon_tbs.FirstOrDefault(d => d.id.ToString() == sp.donvitinh);
                        if (dvt != null) donvitinh_name = dvt.ten;
                    }
                    txt_dvt.Text = donvitinh_name;
                }
            }
        }
    }
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
             long customerId;
             if (!long.TryParse(DropDownList2.SelectedValue, out customerId))
                 return;
             var q = db.Data_KhachHang_tbs.FirstOrDefault(p => p.id == customerId);
             if (q != null)
             {
                 txt_sdt.Text = q.sdt;
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
    }


    // Removed but_add_sp_chitiet_Click and but_xoachitiet_Click

    #region xác nhân đã bán, đã ký hđ --> up file hđ --> trừ sl kho
    //protected void but_show_form_daban_Click(object sender, EventArgs e)
    //{
    //    check_login_cl.check_login_admin("37", "37");
    //    LinkButton button = (LinkButton)sender;
    //    string _idbg = button.CommandArgument;

    //    ViewState["idbg_chitiet"] = _idbg;

    //    using (dbDataContext db = new dbDataContext())
    //    {
    //        var q = db.HangBaoHanh_tbs.FirstOrDefault(p => p.id.ToString() == _idbg);
    //        if (q != null)
    //        {
    //            if (q.trangthai == "Đã trả")
    //            {
    //                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Phiếu bảo hành này đã trả hàng cho khách.", "false", "false", "OK", "alert", ""), true);
    //                return;
    //            }
    //        }
    //    }
    //    pn_daban.Visible = !pn_daban.Visible;
    //    up_daban.Update();

    //}
    //protected void but_close_form_daban_Click(object sender, EventArgs e)
    //{
    //    ViewState["idbg_chitiet"] = ""; 
    //    txt_ghichu_chuagiao.Text = string.Empty;
    //    txt_link_fileupload.Text = string.Empty;
    //    pn_daban.Visible = !pn_daban.Visible;
    //}
    protected void but_daban_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_admin("37", "37");
        string _idbg = ViewState["id_edit"].ToString();
        string _ghichu = txt_ghichu.Text.Trim();

        using (dbDataContext db = new dbDataContext())
        {
            // Lấy thông tin báo giá dựa trên ID
            var baoGia = db.HangBaoHanh_tbs.FirstOrDefault(p => p.id.ToString() == _idbg);
            if (baoGia != null)
            {
                var baoGia_chitiet = db.HangBaoHanh_ChiTiet_tbs.Where(p => p.id_PhieuBaoHanh == _idbg);
                if (!baoGia_chitiet.Any())
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Phiếu bảo hành này không có hàng nào để trả.", "false", "false", "OK", "alert", ""), true);
                    return;
                }

                // Cập nhật thông tin báo giá
                baoGia.trangthai = "Đã trả";
                baoGia.NgayTra_ThucTe = DateTime.Now;
                baoGia.ghichu = _ghichu;

                #region cập nhật chi tiết bảo hành
                var q_chitiet = db.HangBaoHanh_ChiTiet_tbs.FirstOrDefault(p => p.id_PhieuBaoHanh == _idbg);
                if (q_chitiet != null)
                {
                    int _sl = Number_cl.Check_Int(txt_sl_chitiet.Text.Trim());
                    Int64 _so_tien_bh = Number_cl.Check_Int64(txt_sotien_baohanh1.Text.Trim());
                    decimal _giamgia_phantram = 0; // Removed from UI
                    if (_sl >= 0)
                    {
                        q_chitiet.soluong = _sl;
                        q_chitiet.sotien_baohanh = _so_tien_bh;
                        q_chitiet.thanhtien = _sl * _so_tien_bh;

                        q_chitiet.giamgia_phantram = _giamgia_phantram;

                        decimal _giamgia_he_so = _giamgia_phantram / 100;
                        decimal thanhtienDecimal = Convert.ToDecimal(q_chitiet.thanhtien);
                        decimal _giamgia_thanhtienDecimal = thanhtienDecimal * _giamgia_he_so;

                        q_chitiet.giamgia_thanhtien = (Int64)Math.Round(_giamgia_thanhtienDecimal, 0);
                        q_chitiet.TongSauGiam = q_chitiet.thanhtien - q_chitiet.giamgia_thanhtien;
                        
                        // Cập nhật trạng thái chi tiết
                        q_chitiet.trangthai_chitiet = "Đã bàn giao";
                    }
                }
                #endregion

                db.SubmitChanges();

                if (ViewState["id_to_home"] != null)
                    ViewState["id_to_home"] = null;
                Button1.Visible = false;

                update_baogia(db, _idbg);
                load_edit(db, _idbg);
                load_congno(db, _idbg, baoGia.congno.Value, baoGia.trangthai);
                show_main();
                up_main.Update();

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                    thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
            }
            else
            {
                // Thông báo nếu không tìm thấy báo giá
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                    thongbao_class.metro_notifi("Lỗi", "Không tìm thấy phiếu bảo hành.", "2000", "error"), true);
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
                if (dataItem.trangthai == "Đã trả")
                {
                    checkBox.Visible = false;
                }
            }
            var ph_excel_list = (PlaceHolder)e.Item.FindControl("ph_excel_list");
            var lnk_excel_list = (HyperLink)e.Item.FindControl("lnk_excel_list");
            if (ph_excel_list != null && lnk_excel_list != null)
            {
                string _id = dataItem.id.ToString();
                string relativePath = "/uploads/excel-baohanh/" + _id + ".xlsx";
                string absolutePath = Server.MapPath("~" + relativePath);
                if (System.IO.File.Exists(absolutePath))
                {
                    ph_excel_list.Visible = true;
                    lnk_excel_list.NavigateUrl = relativePath;
                }
                else
                {
                    string relativePathXls = "/uploads/excel-baohanh/" + _id + ".xls";
                    string absolutePathXls = Server.MapPath("~" + relativePathXls);
                    if (System.IO.File.Exists(absolutePathXls))
                    {
                        ph_excel_list.Visible = true;
                        lnk_excel_list.NavigateUrl = relativePathXls;
                    }
                    else
                    {
                        ph_excel_list.Visible = false;
                    }
                }
            }
        }
    }

    public void update_baogia(dbDataContext db, string _idbg)
    {
        var q = db.HangBaoHanh_tbs.FirstOrDefault(p => p.id.ToString() == _idbg);
        if (q != null)
        {
            Int64 _tongsaugiam = 0; // Tổng sau giảm từng sản phẩm
            int _vat = q.vat ?? 0;

            // Lấy danh sách chi tiết báo giá
            var q_chitiet = db.HangBaoHanh_ChiTiet_tbs.Where(p => p.id_PhieuBaoHanh == _idbg);
            if (q_chitiet.Any())
            {
                var lstChitiet = q_chitiet.ToList();
                _tongsaugiam = lstChitiet.Sum(p => p.TongSauGiam ?? 0) + (Int64)lstChitiet.Sum(p => p.congno_doitac ?? 0); // Đảm bảo không null và cộng thêm chi phí sửa chữa
                
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

                Int64 _dathanhtoan = 0;
                var q_lichsu_thanhtoan = db.HangBaoHanh_LichSu_ThanhToan_tbs.Where(p => p.id_PhieuBaoHanh == _idbg);
                if (q_lichsu_thanhtoan.Any())
                    _dathanhtoan = q_lichsu_thanhtoan.Sum(p => p.sotien_thanhtoan.Value);
                q.congno = q.giatri_thuc_donhang - _dathanhtoan;


                db.SubmitChanges();
            }
            else//nếu bị xóa hết chi tiết thì xóa lịch sử thanh toán nếu có
            {
                foreach (var t in db.HangBaoHanh_LichSu_ThanhToan_tbs.Where(p => p.id_PhieuBaoHanh == _idbg))
                {
                    db.HangBaoHanh_LichSu_ThanhToan_tbs.DeleteOnSubmit(t);
                }
                q.tongtien = 0;
                q.giatri_thuc_donhang = 0;
                q.congno = 0;
                db.SubmitChanges();
            }

        }
    }


    protected void but_thanhtoan_congno_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_admin("37", "37");
        string _idbg = ViewState["id_edit"].ToString();
        Int64 _thanhtoan = Number_cl.Check_Int64(txt_sotien_thanhtoan_congno.Text);

        using (dbDataContext db = new dbDataContext())
        {
            // Lấy thông tin báo giá dựa trên ID
            var baoGia = db.HangBaoHanh_tbs.FirstOrDefault(p => p.id.ToString() == _idbg);
            if (baoGia != null)
            {
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
                var lichSuThanhToan = new HangBaoHanh_LichSu_ThanhToan_tb
                {
                    id_PhieuBaoHanh = _idbg,
                    sotien_thanhtoan = _thanhtoan,
                    ngay_thanhtoan = DateTime.Now,
                    nguoixacnhan = ViewState["taikhoan"]?.ToString()
                };
                db.HangBaoHanh_LichSu_ThanhToan_tbs.InsertOnSubmit(lichSuThanhToan);
                baoGia.congno = baoGia.congno - _thanhtoan;
                
                // Tự động gán Ngày trả thực tế khi thanh toán công nợ thành công
                baoGia.NgayTra_ThucTe = DateTime.Now;
                
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

    public string GetChiTietSanPham(object id_phieu)
    {
        // Danh sách chính đã nạp chi tiết theo lô trong show_main().
        // Giữ phương thức để tương thích với các trang cũ, không truy vấn DB theo từng dòng.
        return "";
    }

    public string GetOrInsertDuLieuNguon(dbDataContext db, string ten, string kyhieu)
    {
        if (string.IsNullOrEmpty(ten)) return "";
        ten = ten.Trim().ToUpper();
        var exists = db.DuLieuNguon_tbs.FirstOrDefault(p => p.kyhieu == kyhieu && p.ten.ToUpper() == ten);
        if (exists != null)
        {
            return exists.id.ToString();
        }
        else
        {
            DuLieuNguon_tb ob = new DuLieuNguon_tb();
            ob.kyhieu = kyhieu;
            ob.ten = ten;
            db.DuLieuNguon_tbs.InsertOnSubmit(ob);
            db.SubmitChanges(); 
            return ob.id.ToString();
        }
    }

    protected void btnRefreshGrid_Click(object sender, EventArgs e)
    {
        show_main();
        up_main.Update();
    }

    private void HandleAjaxImport()
    {
        Response.Clear();
        Response.ContentType = "application/json";
        
        string _tk = Session["taikhoan"] as string;
        if (string.IsNullOrEmpty(_tk))
        {
            Response.Write("{\"success\": false, \"message\": \"Phiên làm việc đã hết hạn. Vui lòng tải lại trang.\"}");
            Response.End();
            return;
        }
        _tk = mahoa_cl.giaima_Bcorn(_tk);

        if (Request.Files.Count > 0)
        {
            HttpPostedFile file = Request.Files[0];
            try
            {
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (var package = new OfficeOpenXml.ExcelPackage(file.InputStream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet != null)
                    {
                        using (dbDataContext db = new dbDataContext())
                        {
                            int rowCount = worksheet.Dimension.Rows;
                            int importedCount = 0;
                            int colCount = worksheet.Dimension.Columns;
                            
                            // ----- DYNAMIC COLUMN MAPPING -----
                            string[] r1 = new string[colCount + 1];
                            string[] r2 = new string[colCount + 1];
                            string lastR1 = "";
                            for (int i = 1; i <= colCount; i++) {
                                string val1 = (worksheet.Cells[1, i].Text ?? "").Trim().ToUpper();
                                string val2 = (worksheet.Cells[2, i].Text ?? "").Trim().ToUpper();
                                if (!string.IsNullOrEmpty(val1)) lastR1 = val1;
                                r1[i] = lastR1;
                                r2[i] = val2;
                                if (string.IsNullOrEmpty(r1[i]) && !string.IsNullOrEmpty(val2)) r1[i] = val2; 
                            }

                            Func<string, string, int, int> findCol = (top, bottom, skip) => {
                                int found = 0;
                                for (int i = 1; i <= colCount; i++) {
                                    bool tMatch = string.IsNullOrEmpty(top) || (r1[i] != null && r1[i].Contains(top));
                                    bool bMatch = string.IsNullOrEmpty(bottom) || (r2[i] != null && (r2[i] == bottom || r2[i].Contains(bottom))) || (r1[i] != null && r1[i].Contains(bottom));
                                    if (tMatch && bMatch) {
                                        if (found == skip) return i;
                                        found++;
                                    }
                                }
                                return -1;
                            };

                            int c_ngayNhan = findCol("", "NGÀY NHẬN", 0);
                            int c_tenKH = findCol("", "TÊN KHÁCH HÀNG", 0);
                            int c_tenSP = findCol("", "TÊN SẢN PHẨM", 0);
                            int c_dvt = findCol("", "ĐVT", 0);
                            int c_seri = findCol("", "SERI", 0);
                            int c_maKichHoat = -1;
                            for (int i = 1; i <= colCount; i++) { if (r1[i] == "MÃ KÍCH HOẠT" || r2[i] == "MÃ KÍCH HOẠT") { c_maKichHoat = i; break; } }
                            int c_thoiHan = findCol("", "THỜI", 0);
                            int c_slNhan = -1;
                            for(int i=1; i<=colCount; i++) { if (r1[i] == "SL" || r2[i] == "SL") { c_slNhan = i; break; } }
                            int c_ghiChu1 = -1;
                            for(int i=1; i<=colCount; i++) { if ((r1[i] != null && r1[i].Contains("GHI CHÚ")) || (r2[i] != null && r2[i].Contains("GHI CHÚ"))) { c_ghiChu1 = i; break; } }
                            int c_maDt = findCol("MANG ĐI SỬA", "MÃ ĐT", 0);
                            if (c_maDt == -1) c_maDt = findCol("", "MÃ ĐT", 0);
                            int c_noiSua = findCol("MANG ĐI SỬA", "NƠI SỬA", 0);
                            if (c_noiSua == -1) c_noiSua = findCol("", "NƠI SỬA", 0);
                            int c_ngayMang = findCol("MANG ĐI SỬA", "NGÀY", 0);
                            int c_slMang = findCol("MANG ĐI SỬA", "SL", 0);
                            int c_ngayVe = findCol("MANG ĐI SỬA", "NGÀY VỀ", 0);
                            if (c_ngayVe == -1) c_ngayVe = findCol("", "NGÀY VỀ", 0);
                            int c_slVe = findCol("MANG ĐI SỬA", "SL", 1);
                            int c_cnMang = findCol("MANG ĐI SỬA", "CN", 0);
                            int c_soPhieu = findCol("", "SỐ PHIẾU", 0);
                            int c_ngayTra = findCol("TRẢ KHÁCH", "NGÀY", 0);
                            int c_slTra = findCol("TRẢ KHÁCH", "SL", 0);
                            int c_cnTra = findCol("TRẢ KHÁCH", "CN", 0);
                            int c_ghiChu2 = findCol("", "GHI CHÚ", 1);
                            int c_doiL1 = findCol("", "L.1", 0);
                            if (c_doiL1==-1) c_doiL1 = findCol("", "L 1", 0);
                            if (c_doiL1==-1) c_doiL1 = findCol("", "L1", 0);
                            int c_doiL2 = findCol("", "L.2", 0);
                            if (c_doiL2==-1) c_doiL2 = findCol("", "L 2", 0);
                            if (c_doiL2==-1) c_doiL2 = findCol("", "L2", 0);
                            int c_doiL3 = findCol("", "L.3", 0);
                            if (c_doiL3==-1) c_doiL3 = findCol("", "L 3", 0);
                            if (c_doiL3==-1) c_doiL3 = findCol("", "L3", 0);

                            Func<int, int, string> getVal = (r, c) => c != -1 && c <= colCount ? (worksheet.Cells[r, c].Text ?? "").Trim() : "";

                            for (int row = 3; row <= rowCount; row++)
                            {
                                string ngayNhanStr = getVal(row, c_ngayNhan);
                                string tenKhachHang = getVal(row, c_tenKH);
                                if (string.IsNullOrWhiteSpace(ngayNhanStr) && string.IsNullOrWhiteSpace(tenKhachHang)) continue;

                                string tenSanPham = getVal(row, c_tenSP);
                                string donViTinh = getVal(row, c_dvt);
                                string seri = getVal(row, c_seri);
                                string maKichHoat = getVal(row, c_maKichHoat);
                                string thoiHan = getVal(row, c_thoiHan);
                                string slNhanStr = getVal(row, c_slNhan);
                                string ghiChu1 = getVal(row, c_ghiChu1);
                                string maDt = getVal(row, c_maDt);
                                string noiSua = getVal(row, c_noiSua);
                                string ngayMangSuaStr = getVal(row, c_ngayMang);
                                string slMangSuaStr = getVal(row, c_slMang);
                                string ngaySuaVeStr = getVal(row, c_ngayVe);
                                string slSuaVeStr = getVal(row, c_slVe);
                                string cnMangDiStr = getVal(row, c_cnMang);
                                string soPhieuTra = getVal(row, c_soPhieu);
                                string ngayTraStr = getVal(row, c_ngayTra);
                                string slTraKhachStr = getVal(row, c_slTra);
                                string cnTraKhachStr = getVal(row, c_cnTra);
                                string ghiChu2 = getVal(row, c_ghiChu2);
                                string doiL1 = getVal(row, c_doiL1);
                                string doiL2 = getVal(row, c_doiL2);
                                string doiL3 = getVal(row, c_doiL3);

                                HangBaoHanh_tb master = new HangBaoHanh_tb();
                                master.ngaytao = DateTime.Now;
                                master.nguoitao = _tk;
                                master.trangthai = "Đang xử lý";
                                master.ten_khachhang = tenKhachHang;
                                master.trehen = false;
                                
                                DateTime ngayNhan;
                                if (DateTime.TryParse(ngayNhanStr, out ngayNhan) && ngayNhan.Year > 1900) master.ngaynhan = ngayNhan;
                                else master.ngaynhan = DateTime.Now;
                                
                                DateTime ngayTra;
                                if (DateTime.TryParse(ngayTraStr, out ngayTra) && ngayTra.Year > 1900) master.NgayTra_ThucTe = ngayTra;

                                long cnTraKhach;
                                if (long.TryParse(cnTraKhachStr, out cnTraKhach)) master.congno = cnTraKhach;

                                db.HangBaoHanh_tbs.InsertOnSubmit(master);
                                db.SubmitChanges();

                                HangBaoHanh_ChiTiet_tb detail = new HangBaoHanh_ChiTiet_tb();
                                detail.id_PhieuBaoHanh = master.id.ToString();
                                detail.ten = tenSanPham;
                                detail.donvitinh = donViTinh;
                                detail.seri = seri;
                                detail.ma_kich_hoat = maKichHoat;
                                detail.thoi_han_baohanh = thoiHan;
                                detail.ghichu_sanpham = ghiChu1;
                                detail.ma_doitac_sua = maDt;
                                detail.noi_sua = noiSua;
                                
                                int slNhan;
                                if (int.TryParse(slNhanStr, out slNhan)) detail.soluong = slNhan;

                                DateTime ngayMangSua;
                                if (DateTime.TryParse(ngayMangSuaStr, out ngayMangSua) && ngayMangSua.Year > 1900) detail.ngay_mang_sua = ngayMangSua;

                                int slMangSua;
                                if (int.TryParse(slMangSuaStr, out slMangSua)) detail.sl_mang_sua = slMangSua;

                                DateTime ngaySuaVe;
                                if (DateTime.TryParse(ngaySuaVeStr, out ngaySuaVe) && ngaySuaVe.Year > 1900) detail.ngay_sua_ve = ngaySuaVe;

                                int slSuaVe;
                                if (int.TryParse(slSuaVeStr, out slSuaVe)) detail.sl_sua_ve = slSuaVe;

                                long cnMangDi;
                                if (long.TryParse(cnMangDiStr, out cnMangDi)) detail.congno_doitac = cnMangDi;

                                detail.so_phieu_tra = soPhieuTra;

                                int slTraKhach;
                                if (int.TryParse(slTraKhachStr, out slTraKhach)) detail.sl_tra_khach = slTraKhach;
                                
                                long cnTraKhachDetail;
                                if (long.TryParse(cnTraKhachStr, out cnTraKhachDetail)) detail.congno_trakhach = cnTraKhachDetail;

                                detail.ghichu_trakhach = ghiChu2;
                                detail.ma_kichhoat_doi_l1 = doiL1;
                                detail.ma_kichhoat_doi_l2 = doiL2;
                                detail.ma_kichhoat_doi_l3 = doiL3;
                                
                                detail.sotien_baohanh = 0;
                                detail.thanhtien = 0;
                                detail.giamgia_phantram = 0;
                                detail.giamgia_thanhtien = 0;
                                detail.TongSauGiam = 0;

                                db.HangBaoHanh_ChiTiet_tbs.InsertOnSubmit(detail);
                                db.SubmitChanges();
                                
                                importedCount++;
                            }
                            
                            Response.Write("{\"success\": true, \"message\": \"Đã import thành công " + importedCount + " bản ghi!\"}");
                        }
                    }
                    else
                    {
                        Response.Write("{\"success\": false, \"message\": \"File Excel không có sheet nào.\"}");
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("{\"success\": false, \"message\": \"Lỗi: " + ex.Message.Replace("\"", "\\\"").Replace("\n", " ") + "\"}");
            }
        }
        else
        {
            Response.Write("{\"success\": false, \"message\": \"Vui lòng chọn file Excel.\"}");
        }
        
        Response.End();
    }

    // Removed Repeater2_ItemDataBound
}
