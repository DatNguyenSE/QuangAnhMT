using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_hang_bao_hanh_Default : System.Web.UI.Page
{ // ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Tài khoản đã bị khóa.", "false", "false", "OK", "alert", ""), true);
    String_cl str_cl = new String_cl();
    DateTime_cl dt_cl = new DateTime_cl();

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
    protected void Page_Load(object sender, EventArgs e)
    {
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

                #region tính toán Trễ hạn
                DateTime _ngayhientai = DateTime.Now;
                var q_check_all = db.CheckAll_tbs.FirstOrDefault(p => p.ngay.Value.Date == _ngayhientai.Date && p.hangmuc == "BaoHanh" && p.CheckAll == true);
                if (q_check_all == null)
                {
                    var q = db.HangBaoHanh_tbs.Where(p => p.trangthai != "Đã trả");
                    foreach (var t in q)
                    {
                        t.trehen = (t.NgayHenKhachTra != null && t.NgayHenKhachTra.Value.Date < _ngayhientai);
                    }

                    // Đánh dấu là đã check
                    CheckAll_tb _ob1 = new CheckAll_tb
                    {
                        ngay = _ngayhientai, // Lưu ngày giờ chính xác của lần cập nhật
                        hangmuc = "BaoHanh",
                        CheckAll = true
                    };
                    db.CheckAll_tbs.InsertOnSubmit(_ob1);
                    db.SubmitChanges();
                }

                #endregion
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

                #region lấy dữ liệu
                var list_all = (from ob1 in db.HangBaoHanh_tbs
                                join ob2 in db.HangBaoHanh_ChiTiet_tbs
                                on ob1.id.ToString() equals ob2.id_PhieuBaoHanh into chiTietGroup // Nhóm dữ liệu
                                join tk in db.taikhoan_tbs
                                on ob1.nguoitao equals tk.taikhoan into TaiKhoanGroup
                                from tk in TaiKhoanGroup.DefaultIfEmpty()
                                select new
                                {
                                    ob1.id,
                                    ob1.ngaytao,
                                    HoTenNhanVien = tk == null ? "" : tk.hoten,
                                    ob1.sdt_khachhang,
                                    ob1.ten_khachhang,
                                    ob1.diachi_khachhang,
                                    TongTien = chiTietGroup.Any() ? chiTietGroup.Sum(ct => (Int64?)ct.thanhtien) ?? 0 : 0,
                                    TongGiam = chiTietGroup.Any() ? chiTietGroup.Sum(ct => (Int64?)ct.giamgia_thanhtien) ?? 0 : 0,
                                    TongSauGiam = chiTietGroup.Any() ? chiTietGroup.Sum(ct => (Int64?)ct.TongSauGiam) ?? 0 : 0,
                                    congno = ob1.congno ?? 0,
                                    ob1.ghichu,
                                    ob1.NgayHenKhachTra,
                                    ob1.ngaynhan,
                                    ob1.NgayTra_ThucTe,
                                    ob1.trangthai,
                                    ob1.trehen
                                }).Distinct().AsQueryable();



                //if (check_login_cl.CheckQuyen(db, ViewState["taikhoan"].ToString(), "17"))
                //{ list_all = list_all.Where(p => p.nguoibaogia == ViewState["taikhoan"].ToString()); }

                // Kiểm tra xem textbox có dữ liệu tìm kiếm không
                string _key = txt_timkiem.Text.Trim();
                if (!string.IsNullOrEmpty(_key))
                    list_all = list_all.Where(p => p.ten_khachhang.Contains(_key) || p.diachi_khachhang.Contains(_key) || p.sdt_khachhang.Contains(_key) || p.id.ToString() == _key);
                else
                {
                    string _key1 = txt_timkiem1.Text.Trim();
                    if (!string.IsNullOrEmpty(_key1))
                        list_all = list_all.Where(p => p.ten_khachhang.Contains(_key1) || p.diachi_khachhang.Contains(_key1) || p.sdt_khachhang.Contains(_key1) || p.id.ToString() == _key1);
                }



                ////xử lý theo thời gian
                //string _id_locthoigian = ddl_thoigian.SelectedValue;
                //if (_id_locthoigian == "1")//lọc theo ngày BG
                //{
                //    if (txt_tungay.Text != "")
                //        list_all = list_all.Where(p => p.ngaybaogia.Value.Date >= DateTime.Parse(txt_tungay.Text).Date);
                //    if (txt_denngay.Text != "")
                //        list_all = list_all.Where(p => p.ngaybaogia.Value.Date <= DateTime.Parse(txt_denngay.Text).Date);
                //}
                //else if (_id_locthoigian == "2")//lọc theo ngày BG
                //{
                //    if (txt_tungay.Text != "")
                //        list_all = list_all.Where(p => p.ngayban_kyhopdong.HasValue && p.ngayban_kyhopdong.Value.Date >= DateTime.Parse(txt_tungay.Text).Date);
                //    if (txt_denngay.Text != "")
                //        list_all = list_all.Where(p => p.ngayban_kyhopdong.HasValue && p.ngayban_kyhopdong.Value.Date <= DateTime.Parse(txt_denngay.Text).Date);
                //}
                ////xử lý theo lọc khác: đã bán/chưa bán
                //string _id_loc3 = DropDownList3.SelectedValue;
                //if (_id_loc3 != "0")
                //{
                //    if (_id_loc3 == "1")//đã bán
                //        list_all = list_all.Where(p => p.ngayban_kyhopdong != null);
                //    else//chưa bán
                //        list_all = list_all.Where(p => p.ngayban_kyhopdong == null);
                //}
                ////xử lý theo lọc khác: Thanh toán/Công nợ
                //string _id_loc4 = DropDownList4.SelectedValue;
                //if (_id_loc4 != "0")
                //{
                //    if (_id_loc4 == "1")//đã thanh toán
                //        list_all = list_all.Where(p => p.ngayban_kyhopdong != null && p.congno == 0);
                //    else//công nợ
                //        list_all = list_all.Where(p => p.ngayban_kyhopdong == null && p.congno != 0);
                //}
                ////xử lý theo lọc khác: Giao hàng/Chưa giao
                //string _id_loc5 = DropDownList5.SelectedValue;
                //if (_id_loc5 != "0")
                //{
                //    if (_id_loc5 == "1")//đã fiao
                //        list_all = list_all.Where(p => p.ngayban_kyhopdong != null && p.ghichu_chuagiao == "");
                //    else//chưa giao
                //        list_all = list_all.Where(p => p.ngayban_kyhopdong == null && p.ghichu_chuagiao != "");
                //}
                ////lọc theo người bán
                //List<string> list_taikhoan = new List<string>();
                //foreach (ListItem item in ListBox2.Items)
                //{
                //    if (item.Selected)
                //        list_taikhoan.Add(item.Value);
                //}
                //if (!list_taikhoan.Contains(""))//nếu tồn tại "": tất cả thì k lọc
                //    list_all = list_all.Where(tk => list_taikhoan.Contains(tk.nguoibaogia));


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
                list_all = list_all.OrderByDescending(p => p.ngaytao);
                int _Tong_Record = list_all.Count();

                if (_Tong_Record != 0)
                {
                    ViewState["TongThanhTien"] = list_all.Sum(p => p.TongTien).ToString("#,##0");
                    ViewState["TongGiam"] = list_all.Sum(p => p.TongGiam).ToString("#,##0");
                    ViewState["TongSauGiam"] = list_all.Sum(p => p.TongSauGiam).ToString("#,##0");


                }
                else
                {
                    ViewState["TongThanhTien"] = "0";
                    ViewState["TongGiam"] = "0";
                    ViewState["TongSauGiam"] = "0";
                }
                #endregion

                #region phân trang OK, k sửa
                // Xử lý số record mỗi trang
                int show = Number_cl.Check_Int(txt_show.Text.Trim()); if (show <= 0) show = 30;
                //xử lý trang hiện tại. Đảm bảo current_page không nhỏ hơn 1 và không lớn hơn total_page
                int current_page = int.Parse(ViewState["current_page_hangbaohanh"].ToString()); int total_page = number_of_page_class.return_total_page(_Tong_Record, show); if (current_page < 1) current_page = 1; else if (current_page > total_page) current_page = total_page;
                ViewState["total_page"] = total_page;
                //xử lý nút bấm tới lui
                if (current_page >= total_page)
                {
                    but_xemtiep.Enabled = false;//máy tính
                    but_xemtiep1.Enabled = false;//điện thoại
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
                //PHÂN TRANG****PHÂN TRANG
                var list_split = list_all.Skip(current_page * show - show).Take(show);
                //xử lý thanh thông báo phân trang
                int stt = (show * current_page) - show + 1; int _s1 = stt + list_split.Count() - 1;
                if (_Tong_Record != 0) lb_show.Text = stt + "-" + _s1 + " trong số " + _Tong_Record.ToString("#,##0"); else lb_show.Text = "0-0/0"; lb_show_md.Text = stt + "-" + _s1 + " trong số " + _Tong_Record.ToString("#,##0");
                #endregion
                Repeater1.DataSource = list_split;
                Repeater1.DataBind();
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
       
        Label1.Text = null; txt_ngaynhan.Text = ""; txt_ngayhentra.Text = "";
        txt_sdt.Text = ""; txt_ten_kh.Text = ""; txt_diachi_kh.Text = "";
        PlaceHolder1.Visible = false;
        Repeater2.DataSource = null;
        Repeater2.DataBind();
        ViewState["add_edit"] = null;
        txt_name.Text = ""; txt_link_fileupload.Text = ""; txt_model.Text = ""; txt_thongso.Text = "";

        TextBox1.Text = "";
        TextBox2.Text = "";

        txt_soluong.Text = "1";
        txt_giamgia_phantram.Text = "0";
        txt_model.Text = "";
        txt_thongso.Text = "";

        DropDownList2.DataSource = null;
        DropDownList2.DataBind();
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
        //reset control
        reset_control_add_edit();

        ViewState["add_edit"] = "add";
        Label1.Text = "TẠO PHIẾU BẢO HÀNH";
        but_add_edit.Text = "TẠO PHIẾU";

        PlaceHolder8.Visible = true;
        using (dbDataContext db = new dbDataContext())
        {
            var q = (from u in db.Data_KhachHang_tbs
                     select new { u.sdt, u.ten, CustomField = u.sdt + " - " + u.ten });
            DropDownList2.DataSource = q;
            DropDownList2.DataValueField = "sdt";
            DropDownList2.DataTextField = "CustomField";
            DropDownList2.DataBind();
            DropDownList2.Items.Insert(0, new ListItem("Tìm thông tin khách hàng", ""));

            txt_ngaynhan.Text = DateTime.Now.ToShortDateString();
            txt_ngayhentra.Text = DateTime.Now.AddDays(7).ToShortDateString();

            var data = db.DuLieuNguon_tbs
            .Where(p => p.kyhieu == "hangsanpham" || p.kyhieu == "nhomsanpham" || p.kyhieu == "donvitinh")
            .ToList();

            var hangSanPham = data.Where(p => p.kyhieu == "hangsanpham").OrderBy(p => p.ten).ToList();
            var nhomSanPham = data.Where(p => p.kyhieu == "nhomsanpham").OrderBy(p => p.ten).ToList();
            var donvitinh = data.Where(p => p.kyhieu == "donvitinh").OrderBy(p => p.ten).ToList();



        }

        //hiện form add_edit trong updatePanel_add
        pn_add.Visible = !pn_add.Visible;
        up_add.Update();

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
            // Lấy danh sách chi tiết bao giá cùng thông tin sản phẩm
            var q_chitiet = from chitiet in db.HangBaoHanh_ChiTiet_tbs
                            join ob4 in db.DuLieuNguon_tbs.Where(p => p.kyhieu == "donvitinh") on chitiet.donvitinh equals ob4.id.ToString() into DVTGroup
                            from ob4 in DVTGroup.DefaultIfEmpty()
                            join ob2 in db.DuLieuNguon_tbs.Where(p => p.kyhieu == "hangsanpham") on chitiet.id_hang equals ob2.id.ToString() into HangGroup
                            from ob2 in HangGroup.DefaultIfEmpty()
                            join ob3 in db.DuLieuNguon_tbs.Where(p => p.kyhieu == "nhomsanpham") on chitiet.id_nhom equals ob3.id.ToString() into NhomGroup
                            from ob3 in NhomGroup.DefaultIfEmpty()
                            where chitiet.id_PhieuBaoHanh == _idbg
                            select new
                            {
                                chitiet.id,
                                DVT = ob4.ten,
                                TenHang = ob2.ten,
                                TenNhom = ob3.ten,
                                chitiet.soluong,
                                chitiet.sotien_baohanh,
                                chitiet.thanhtien,
                                chitiet.giamgia_phantram,
                                chitiet.giamgia_thanhtien,
                                chitiet.TongSauGiam,
                                chitiet.ten,
                                chitiet.anh,
                                chitiet.thongso_kythuat,
                                chitiet.model,
                            };
            if (q_chitiet.Any())
            {
                ViewState["TongThanhTien_ChiTiet"] = q_chitiet.Sum(p => p.thanhtien.Value).ToString("#,##0");
                ViewState["TongGiam_ChiTiet"] = q_chitiet.Sum(p => p.giamgia_thanhtien.Value).ToString("#,##0");
                Int64 TongSauGiam_ChiTiet = q_chitiet.Sum(p => p.TongSauGiam.Value);
                ViewState["TongSauGiam_ChiTiet"] = TongSauGiam_ChiTiet.ToString("#,##0");

                ViewState["donhang_saugiamgia"] = TongSauGiam_ChiTiet;
                ViewState["thanhtien_vat_chitiet"] = 0;
            }
            else
            {

                ViewState["TongThanhTien_ChiTiet"] = "0";
                ViewState["TongGiam_ChiTiet"] = "0";
                ViewState["TongSauGiam_ChiTiet"] = "0";
            }
            Repeater2.DataSource = q_chitiet.OrderBy(p => p.ten);
            Repeater2.DataBind();
        }
        catch (Exception _ex)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", _ex.StackTrace, "false", "false", "OK", "alert", ""), true);
            return;
        }

    }
    public void load_congno(dbDataContext db, string _idbg, Int64 _congno, string _trangthai)
    {

        PlaceHolder3.Visible = true;//hiện lịch sử thanh toán
        var q_thanhtoan = from thanhtoan in db.HangBaoHanh_LichSu_ThanhToan_tbs
                          join taiKhoan in db.taikhoan_tbs
                          on thanhtoan.nguoixacnhan equals taiKhoan.taikhoan
                          where thanhtoan.id_PhieuBaoHanh == _idbg
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
            PlaceHolder8.Visible = false;
            //truy vấn dữ liệu để sửa
            var q = db.HangBaoHanh_tbs.FirstOrDefault(p => p.id.ToString() == _id);
            if (q != null)
            {
                ViewState["id_edit"] = _id;
                txt_ghichu.Text = q.ghichu;
                txt_sdt.Text = q.sdt_khachhang;
                txt_ten_kh.Text = q.ten_khachhang;
                txt_diachi_kh.Text = q.diachi_khachhang;
                txt_ngaynhan.Text = q.ngaynhan.Value.ToShortDateString();
                txt_ngayhentra.Text = q.NgayHenKhachTra.Value.ToShortDateString();

                var data = db.DuLieuNguon_tbs
            .Where(p => p.kyhieu == "hangsanpham" || p.kyhieu == "nhomsanpham" || p.kyhieu == "donvitinh")
            .ToList();

                var hangSanPham = data.Where(p => p.kyhieu == "hangsanpham").OrderBy(p => p.ten).ToList();
                var nhomSanPham = data.Where(p => p.kyhieu == "nhomsanpham").OrderBy(p => p.ten).ToList();
                var donvitinh = data.Where(p => p.kyhieu == "donvitinh").OrderBy(p => p.ten).ToList();


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

        #region Chuẩn bị dữ liệu
        string _sdt = str_cl.XuLy_SDT_NhapVao(txt_sdt.Text);
        string _ten_kh = str_cl.VietHoa_ChuCai_DauTien(txt_ten_kh.Text.Trim());
        string _diachi = txt_diachi_kh.Text.Trim();
        DateTime _ngayhientai = DateTime.Now;
        DateTime _ngaynhan = DateTime.Parse(txt_ngaynhan.Text);
        DateTime _ngayhentra = DateTime.Parse(txt_ngayhentra.Text);
        string _ghichu = txt_ghichu.Text.Trim();
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
                _ob.nguoitao = ViewState["taikhoan"].ToString();
                _ob.ngaytao = _ngayhientai;
                _ob.trangthai = "Đã nhận";
                _ob.tongtien = 0;
                _ob.congno = 0;
                _ob.ghichu = _ghichu;
                _ob.trehen = false;
                _ob.phantram_doanhso_now = 0;
                _ob.thuongdoanhso = 0;
                db.HangBaoHanh_tbs.InsertOnSubmit(_ob);
                #endregion

                #region thêm mới khách hàng (nếu chưa thêm)
                var q = db.Data_KhachHang_tbs.FirstOrDefault(p => p.sdt == _sdt);
                if (q == null)
                {
                    Data_KhachHang_tb _ob1 = new Data_KhachHang_tb();
                    _ob1.sdt = _sdt; _ob1.ten = _ten_kh; _ob1.diachi = _diachi; _ob1.ngay_capnhat = _ngayhientai; _ob1.nhanvien_chamsoc = ViewState["taikhoan"].ToString();
                    db.Data_KhachHang_tbs.InsertOnSubmit(_ob1);
                }
                else//nếu có rồi mà đổi thông tin thì cập nhật mới
                {
                    string _ten_old = q.ten; string _diachi_old = q.diachi;
                    if (_ten_old.ToUpper() != _ten_kh.ToUpper())
                        q.ten = _ten_kh;
                    if (_diachi_old.ToUpper() != _diachi.ToUpper())
                        q.diachi = _diachi;
                    q.nhanvien_chamsoc = ViewState["taikhoan"].ToString();
                }
                #endregion

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

                    if (_ob.trangthai != "Đã trả")
                    {
                        if (_ob.NgayHenKhachTra.Value.Date < DateTime.Now.Date)//nếu đã hết hạn
                            _ob.trehen = true;
                        else
                            _ob.trehen = false;
                    }


                    #endregion

                    #region thêm mới khách hàng (nếu chưa thêm)
                    var q = db.Data_KhachHang_tbs.FirstOrDefault(p => p.sdt == _sdt);
                    if (q == null)
                    {
                        Data_KhachHang_tb _ob1 = new Data_KhachHang_tb();
                        _ob1.sdt = _sdt; _ob1.ten = _ten_kh; _ob1.diachi = _diachi; _ob1.ngay_capnhat = _ngayhientai;
                        db.Data_KhachHang_tbs.InsertOnSubmit(_ob1);
                    }
                    else//nếu có rồi mà đổi thông tin thì cập nhật mới
                    {
                        string _ten_old = q.ten; string _diachi_old = q.diachi;
                        if (_ten_old.ToUpper() != _ten_kh.ToUpper())
                            q.ten = _ten_kh;
                        if (_diachi_old.ToUpper() != _diachi.ToUpper())
                            q.diachi = _diachi;
                    }
                    #endregion


                    #region cập nhật số lượng tại bảng chi tiết
                    foreach (RepeaterItem item in Repeater2.Items)
                    {
                        // Tìm các điều khiển TextBox và Label từ RepeaterItem
                        TextBox txt_sl_chitiet = (TextBox)item.FindControl("txt_sl_chitiet");
                        TextBox txt_sotien_baohanh1 = (TextBox)item.FindControl("txt_sotien_baohanh1");
                        TextBox txt_giamgia_phantram_chitiet = (TextBox)item.FindControl("txt_giamgia_phantram_chitiet");
                        Label lbID_chitiet = (Label)item.FindControl("lbID_chitiet");

                        // Kiểm tra nếu cả TextBox và Label không null
                        if (txt_sl_chitiet != null && lbID_chitiet != null)
                        {
                            // Lấy ID và rank từ các điều khiển
                            string _id_chitiet = lbID_chitiet.Text;
                            int _sl = Number_cl.Check_Int(txt_sl_chitiet.Text.Trim());
                            Int64 _so_tien_bh = Number_cl.Check_Int64(txt_sotien_baohanh1.Text.Trim());
                            decimal _giamgia_phantram = Number_cl.Check_Decimal(txt_giamgia_phantram_chitiet.Text.Trim());
                            if (_sl >= 0)
                            {
                                var q_chitiet = db.HangBaoHanh_ChiTiet_tbs.FirstOrDefault(p => p.id.ToString() == _id_chitiet);
                                if (q_chitiet != null)
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
                                }
                            }
                        }
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
            string _sdt = DropDownList2.SelectedValue;
            var q = db.Data_KhachHang_tbs.FirstOrDefault(p => p.sdt == _sdt);
            if (q != null)
            {
                txt_sdt.Text = _sdt;
                txt_ten_kh.Text = q.ten;
                txt_diachi_kh.Text = q.diachi;
            }
            //else
            //{
            //    txt_ten_kh.Text = "";
            //    txt_diachi_kh.Text = "";
            //}
        }
    }


    protected void but_add_sp_chitiet_Click(object sender, EventArgs e)//bonbap
    {

        check_login_cl.check_login_admin("36", "36");
        string _idbg = ViewState["id_edit"].ToString();
        int _soluong_xuat = Number_cl.Check_Int(txt_soluong.Text.Trim());
        Int64 _sotien_baohanh = Number_cl.Check_Int64(txt_sotien_baohanhg.Text.Trim());
        decimal _giamgia_phantram = Number_cl.Check_Decimal(txt_giamgia_phantram.Text.Trim());

        string _tensp = txt_name.Text.Trim();
        string _anh = txt_link_fileupload.Text;
        string _ten_hang = TextBox1.Text.ToUpper();
        string _ten_donvitinh = TextBox2.Text.ToUpper();
        string _model = txt_model.Text.Trim().ToUpper();
        string _thongso = txt_thongso.Text;


        if (_soluong_xuat <= 0)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Số lượng không hợp lệ.", "false", "false", "OK", "alert", ""), true);
            return;
        }
        if (_giamgia_phantram < 0 || _giamgia_phantram > 100)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Giảm giá phần trăm không hợp lệ. (Từ 0-100)", "false", "false", "OK", "alert", ""), true);
            return;
        }
        if (_tensp == "")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập tên sản phẩm.", "false", "false", "OK", "alert", ""), true);
            return;
        }
        if (_anh == "")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng chọn ảnh sản phẩm.", "false", "false", "OK", "alert", ""), true);
            return;
        }
        if (_ten_hang == "")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập hãng sản phẩm.", "false", "false", "OK", "alert", ""), true);
            return;
        }

        if (_ten_donvitinh == "")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập đơn vị tính.", "false", "false", "OK", "alert", ""), true);
            return;
        }
        if (_model == "")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập Model sản phẩm.", "false", "false", "OK", "alert", ""), true);
            return;
        }
        using (dbDataContext db = new dbDataContext())
        {
            var q_edit = db.HangBaoHanh_tbs.FirstOrDefault(p => p.id.ToString() == ViewState["id_edit"].ToString());
            if (q_edit != null)
            {
                if (q_edit.trangthai == "Đã trả")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Không thể thao tác các phiếu bảo hành đã được trả hàng cho khách.", "false", "false", "OK", "alert", ""), true);
                    return;
                }
            }

            #region thêm sp vào phiếu
            HangBaoHanh_ChiTiet_tb _ob = new HangBaoHanh_ChiTiet_tb();
            _ob.id_PhieuBaoHanh = _idbg;
            _ob.soluong = _soluong_xuat;
            _ob.sotien_baohanh = _sotien_baohanh;
            _ob.thanhtien = _soluong_xuat * _sotien_baohanh;
            _ob.giamgia_phantram = _giamgia_phantram;
            decimal _giamgia_he_so = _giamgia_phantram / 100;
            decimal thanhtienDecimal = Convert.ToDecimal(_ob.thanhtien);
            decimal _giamgia_thanhtienDecimal = thanhtienDecimal * _giamgia_he_so;
            _ob.giamgia_thanhtien = (Int64)Math.Round(_giamgia_thanhtienDecimal, 0);
            _ob.TongSauGiam = _ob.thanhtien - _ob.giamgia_thanhtien;

            _ob.ten = _tensp;
            _ob.ten = _tensp;
            _ob.id_hang = _ten_hang;
            _ob.donvitinh = _ten_donvitinh;
            _ob.anh = _anh;
            _ob.model = _model;
            _ob.thongso_kythuat = _thongso;

            db.HangBaoHanh_ChiTiet_tbs.InsertOnSubmit(_ob);
            db.SubmitChanges();
            #endregion

            txt_name.Text = ""; txt_link_fileupload.Text = ""; txt_model.Text = ""; txt_thongso.Text = "";
            txt_soluong.Text = "0"; txt_soluong.Text = "1"; txt_giamgia_phantram.Text = "0";


            update_baogia(db, _idbg);

            #region cập nhật dữ liệu và update hiển thị
            load_edit(db, _idbg);
            load_congno(db, _idbg, q_edit.congno.Value, q_edit.trangthai);
            show_main();
            up_main.Update();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công", "1000", "warning"), true);
            #endregion

        }

    }

    protected void but_xoachitiet_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_admin("37", "37");

        Button button = (Button)sender;
        string _id = button.CommandArgument;//lấy id_chitiet_baogia
        using (dbDataContext db = new dbDataContext())
        {
            var q_chitiet = db.HangBaoHanh_ChiTiet_tbs.FirstOrDefault(p => p.id.ToString() == _id);
            if (q_chitiet != null)
            {
                string _idbg = q_chitiet.id_PhieuBaoHanh;
                var q_bg = db.HangBaoHanh_tbs.FirstOrDefault(p => p.id.ToString() == _idbg);

                if (q_bg.trangthai == "Đã trả")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Không thể thao tác các phiếu bảo hành đã được trả hàng cho khách.", "false", "false", "OK", "alert", ""), true);
                    return;
                }

                db.HangBaoHanh_ChiTiet_tbs.DeleteOnSubmit(q_chitiet);
                db.SubmitChanges();

                update_baogia(db, _idbg);

                #region cập nhật dữ liệu và update hiển thị
                load_edit(db, _idbg);
                load_congno(db, _idbg, q_bg.congno.Value, q_bg.trangthai);
                show_main();
                up_main.Update();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công", "1000", "warning"), true);
                #endregion


            }
        }
    }

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

                #region cập nhật số lượng tại bảng chi tiết
                foreach (RepeaterItem item in Repeater2.Items)
                {
                    // Tìm các điều khiển TextBox và Label từ RepeaterItem
                    TextBox txt_sl_chitiet = (TextBox)item.FindControl("txt_sl_chitiet");
                    TextBox txt_sotien_baohanh1 = (TextBox)item.FindControl("txt_sotien_baohanh1");
                    TextBox txt_giamgia_phantram_chitiet = (TextBox)item.FindControl("txt_giamgia_phantram_chitiet");
                    Label lbID_chitiet = (Label)item.FindControl("lbID_chitiet");

                    // Kiểm tra nếu cả TextBox và Label không null
                    if (txt_sl_chitiet != null && lbID_chitiet != null)
                    {
                        // Lấy ID và rank từ các điều khiển
                        string _id_chitiet = lbID_chitiet.Text;
                        int _sl = Number_cl.Check_Int(txt_sl_chitiet.Text.Trim());
                        Int64 _so_tien_bh = Number_cl.Check_Int64(txt_sotien_baohanh1.Text.Trim());
                        decimal _giamgia_phantram = Number_cl.Check_Decimal(txt_giamgia_phantram_chitiet.Text.Trim());
                        if (_sl >= 0)
                        {
                            var q_chitiet = db.HangBaoHanh_ChiTiet_tbs.FirstOrDefault(p => p.id.ToString() == _id_chitiet);
                            if (q_chitiet != null)
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
                            }
                        }
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
                //hoặc
                // Lấy giá trị cần so sánh từ DataItem (sửa 'ten_field' thành tên trường dữ liệu phù hợp)
                //string valueToCompare = DataBinder.Eval(dataItem, "Tên Cột")?.ToString() ?? string.Empty;
            }
        }
    }

    public void update_baogia(dbDataContext db, string _idbg)
    {
        var q = db.HangBaoHanh_tbs.FirstOrDefault(p => p.id.ToString() == _idbg);
        if (q != null)
        {
            Int64 _tongsaugiam = 0; // Tổng sau giảm từng sản phẩm

            // Lấy danh sách chi tiết báo giá
            var q_chitiet = db.HangBaoHanh_ChiTiet_tbs.Where(p => p.id_PhieuBaoHanh == _idbg);
            if (q_chitiet.Any())
            {
                _tongsaugiam = q_chitiet.Sum(p => p.TongSauGiam ?? 0); // Đảm bảo không null
                                                                       // Cập nhật vào bảng báo giá
                q.tongtien = _tongsaugiam;

                Int64 _dathanhtoan = 0;
                var q_lichsu_thanhtoan = db.HangBaoHanh_LichSu_ThanhToan_tbs.Where(p => p.id_PhieuBaoHanh == _idbg);
                if (q_lichsu_thanhtoan.Any())
                    _dathanhtoan = q_lichsu_thanhtoan.Sum(p => p.sotien_thanhtoan.Value);
                q.congno = _tongsaugiam - _dathanhtoan;


                db.SubmitChanges();
            }
            else//nếu bị xóa hết chi tiết thì xóa lịch sử thanh toán nếu có
            {
                foreach (var t in db.HangBaoHanh_LichSu_ThanhToan_tbs.Where(p => p.id_PhieuBaoHanh == _idbg))
                {
                    db.HangBaoHanh_LichSu_ThanhToan_tbs.DeleteOnSubmit(t);
                }
                q.tongtien = 0;
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
}