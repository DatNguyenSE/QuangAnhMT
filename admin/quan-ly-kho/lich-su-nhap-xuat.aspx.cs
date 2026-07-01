using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_quan_ly_kho_lich_su_nhap_xuat : System.Web.UI.Page
{
    DateTime_cl dt_cl = new DateTime_cl();
    public void set_dulieu_macdinh()
    {
        try
        {
            ResetButtonCss();//button chọn ngày nhanh
            txt_show.Text = "30";
            ViewState["current_page_lsnhapxuat"] = "1";
            txt_tungay.Text = dt_cl.return_ngaydaunam(DateTime.Now.Year.ToString()).ToShortDateString();
            txt_denngay.Text = dt_cl.return_ngaycuoinam(DateTime.Now.Year.ToString()).ToShortDateString();
            ViewState["tungay"] = txt_tungay.Text;
            ViewState["denngay"] = txt_denngay.Text;

            #region set_get_cookie
            // Lấy cookie "cookie_lsnhapxuat" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_lsnhapxuat"];
            if (cookie == null)
            {

                cookie = new HttpCookie("cookie_lsnhapxuat");
                cookie["show"] = txt_show.Text;//lưu số dòng hiển thị mỗi trang
                cookie["trang_hientai"] = "1";//lưu trang hiện tại
                cookie["tungay"] = txt_tungay.Text;
                cookie["denngay"] = txt_denngay.Text;

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
                ViewState["current_page_lsnhapxuat"] = cookie["trang_hientai"];
                txt_tungay.Text = cookie["tungay"];
                txt_denngay.Text = cookie["denngay"];

                // Thiết lập lại thời gian hết hạn của cookie là 1 ngày từ thời điểm hiện tại
                cookie.Expires = DateTime.Now.AddDays(1);
                // Cập nhật cookie trong Response.Cookies
                Response.Cookies.Set(cookie);
            }
            #endregion

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
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            Session["url_back"] = HttpContext.Current.Request.Url.AbsoluteUri;
            check_login_cl.check_login_admin("13", "13");

            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";

            ViewState["taikhoan"] = _tk;



            set_dulieu_macdinh();
            show_main();

        }
    }
    #region main - phân trang - tìm kiếm
    protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        string userPermissions = ViewState["quyen"].ToString();
        var permissionsList = userPermissions.Split(',');
        if (permissionsList.Contains("8"))
        {
            PlaceHolder PlaceHolder5 = (PlaceHolder)e.Item.FindControl("PlaceHolder5");
            if (PlaceHolder5 != null)
            {
                PlaceHolder5.Visible = true;
            }
        }
        else
        {
            PlaceHolder PlaceHolder5 = (PlaceHolder)e.Item.FindControl("PlaceHolder5");
            if (PlaceHolder5 != null)
            {
                PlaceHolder5.Visible = false;
            }
        }

        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            //lấy ra từng item
            var dataItem = (dynamic)e.Item.DataItem;


            // Tìm CheckBox1 và thiết lập Checked nếu là nổi bật
            var checkBox = (CheckBox)e.Item.FindControl("checkID");
            if (checkBox != null)
            {
                if (dataItem.nhap_hay_xuat == false)//nếu là xuất thì k cho xóa
                {
                    checkBox.Visible = false;
                }
            }
        }
    }
    public void show_main()
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                #region kiểm tra quyền - k cho xem giá nhập
                var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == ViewState["taikhoan"].ToString());
                ViewState["quyen"] = q.permission;
                var permissionsList = q.permission.Split(',');
                if (permissionsList.Contains("8"))
                {
                    PlaceHolder4.Visible = true;
                    PlaceHolder1.Visible = true;
                }
                else
                {
                    PlaceHolder4.Visible = false;
                    PlaceHolder1.Visible = false;
                }
                #endregion



                #region lấy dữ liệu
                var list_all = (from nxk in db.NhapXuatKho_tbs
                                join ob5 in db.BaoGia_tbs on nxk.id_baogia equals ob5.id.ToString() into BaoGiaGroup
                                from ob5 in BaoGiaGroup.DefaultIfEmpty()
                                join ob1 in db.KhoSanPham_tbs on nxk.id_sanpham equals ob1.id.ToString() into SanPhamGroup
                                from ob1 in SanPhamGroup.DefaultIfEmpty()
                                join ob2 in db.DuLieuNguon_tbs
                                    .Where(p => p.kyhieu == "hangsanpham") on ob1.id_hang equals ob2.id.ToString() into HangGroup
                                from ob2 in HangGroup.DefaultIfEmpty()
                                join ob3 in db.DuLieuNguon_tbs
                                    .Where(p => p.kyhieu == "nhomsanpham") on ob1.id_nhom equals ob3.id.ToString() into NhomGroup
                                from ob3 in NhomGroup.DefaultIfEmpty()
                                join ob4 in db.DuLieuNguon_tbs
                                    .Where(p => p.kyhieu == "donvitinh") on ob1.donvitinh equals ob4.id.ToString() into DVTGroup
                                from ob4 in DVTGroup.DefaultIfEmpty()
                                join tk in db.taikhoan_tbs on nxk.nguoinhap equals tk.taikhoan into TaiKhoanGroup
                                from tk in TaiKhoanGroup.DefaultIfEmpty() // Thêm join với bảng TaiKhoan_tbs
                                select new
                                {
                                    TenKhach=ob5.ten_khachhang,
                                    SDT_Khach=ob5.sdt_khachhang,
                                    nxk.id,
                                    nxk.id_baogia,
                                    TenSP = ob1 == null ? "" : ob1.ten,
                                    Hang = ob2 == null ? "" : ob2.ten,
                                    Nhom = ob3 == null ? "" : ob3.ten,
                                    DVT = ob4 == null ? "" : ob4.ten,
                                    ob1.anh,
                                    ob1.model,
                                    ob1.thongso_kythuat,
                                    ob1.gianhap,
                                    TongGia = ob1 == null ? 0 : ob1.gianhap * ob1.soluong_hientai,
                                    GiaNhapXuat = nxk.gia_nhap,
                                    TongBanLe = ob1 == null ? 0 : ob1.giabanle * ob1.soluong_hientai,
                                    ob1.cohoadon,
                                    ob1.hangthanhly,
                                    nxk.ton_hientai,
                                    nxk.soluong_nhap,
                                    TonCuoi_SauNhap = nxk.ton_hientai + nxk.soluong_nhap,
                                    TonCuoi_SauXuat = nxk.ton_hientai - nxk.soluong_nhap,
                                    ob1.ghichu,
                                    nxk.ngaynhap,
                                    nxk.nguoinhap,
                                    HoTenNhanVien = tk == null ? "" : tk.hoten, // Lấy họ tên nhân viên từ bảng TaiKhoan_tbs
                                    nxk.nhap_hay_xuat,
                                }).AsQueryable();


                // Kiểm tra xem textbox có dữ liệu tìm kiếm không
                string _key = txt_timkiem.Text.Trim();
                if (!string.IsNullOrEmpty(_key))
                    list_all = list_all.Where(p => p.TenSP.Contains(_key) || p.Hang.Contains(_key) || p.Nhom.Contains(_key) || p.model == _key || p.id.ToString() == _key);
                else
                {
                    string _key1 = txt_timkiem1.Text.Trim();
                    if (!string.IsNullOrEmpty(_key1))
                        list_all = list_all.Where(p => p.TenSP.Contains(_key1) || p.Hang.Contains(_key1) || p.Nhom.Contains(_key1) || p.model == _key1 || p.id.ToString() == _key1);
                }

                //xử lý theo thời gian
                if (txt_tungay.Text != "")
                    list_all = list_all.Where(p => p.ngaynhap.Value.Date >= DateTime.Parse(txt_tungay.Text).Date);
                if (txt_denngay.Text != "")
                    list_all = list_all.Where(p => p.ngaynhap.Value.Date <= DateTime.Parse(txt_denngay.Text).Date);


                //sắp xếp
                list_all = list_all.OrderByDescending(p => p.ngaynhap);
                int _Tong_Record = list_all.Count();

                if (list_all.Any(p => p.nhap_hay_xuat == true))
                {
                    ViewState["TongSLNhap"] = list_all.Where(p => p.nhap_hay_xuat == true).Sum(p => p.soluong_nhap);
                    ViewState["TongTienNhap"] = list_all.Where(p => p.nhap_hay_xuat == true).Sum(p => p.TongGia);
                }
                else
                { ViewState["TongSLNhap"] = 0; ViewState["TongTienNhap"] = 0; }
                if (list_all.Any(p => p.nhap_hay_xuat == false))
                {
                    ViewState["TongSLBan"] = list_all.Where(p => p.nhap_hay_xuat == false).Sum(p => p.soluong_nhap);
                    ViewState["TongTienBan"] = list_all.Where(p => p.nhap_hay_xuat == false).Sum(p => p.TongGia);
                }
                else
                { ViewState["TongSLBan"] = 0; ViewState["TongTienBan"] = 0; }
                // Tính chênh lệch (Chuyển sang kiểu Int64 để tính toán chính xác)
                Int64 TongTienNhapLong = Int64.Parse(ViewState["TongTienNhap"].ToString());
                Int64 TongTienBanLong = Int64.Parse(ViewState["TongTienBan"].ToString());

                ViewState["ChenhLech"] = (TongTienBanLong - TongTienNhapLong).ToString("#,##0");
                #endregion

                #region phân trang OK, k sửa
                // Xử lý số record mỗi trang
                int show = Number_cl.Check_Int(txt_show.Text.Trim()); if (show <= 0) show = 30;
                //xử lý trang hiện tại. Đảm bảo current_page không nhỏ hơn 1 và không lớn hơn total_page
                int current_page = int.Parse(ViewState["current_page_lsnhapxuat"].ToString()); int total_page = number_of_page_class.return_total_page(_Tong_Record, show); if (current_page < 1) current_page = 1; else if (current_page > total_page) current_page = total_page;
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
            ViewState["current_page_lsnhapxuat"] = int.Parse(ViewState["current_page_lsnhapxuat"].ToString()) - 1;
            #region LƯU TRANG HIỆN TẠI
            // Lấy cookie "cookie_lsnhapxuat" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_lsnhapxuat"];
            if (cookie != null)
            {
                cookie["trang_hientai"] = ViewState["current_page_lsnhapxuat"].ToString();
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
            ViewState["current_page_lsnhapxuat"] = int.Parse(ViewState["current_page_lsnhapxuat"].ToString()) + 1;
            #region LƯU TRANG HIỆN TẠI
            // Lấy cookie "cookie_lsnhapxuat" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_lsnhapxuat"];
            if (cookie != null)
            {
                cookie["trang_hientai"] = ViewState["current_page_lsnhapxuat"].ToString();
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
            ViewState["current_page_lsnhapxuat"] = 1;
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
            if (Request.Cookies["cookie_lsnhapxuat"] != null)//nếu có ck r thì lưu giá trị mới
            {
                HttpCookie _ck = Request.Cookies["cookie_lsnhapxuat"];
                _ck["show"] = txt_show.Text;
                _ck["trang_hientai"] = ViewState["current_page_lsnhapxuat"].ToString();
                _ck["tungay"] = txt_tungay.Text;
                _ck["denngay"] = txt_denngay.Text;
                _ck.Expires = DateTime.Now.AddDays(1);
                Response.Cookies.Set(_ck); // Cập nhật lại cookie
            }
            ViewState["tungay"] = txt_tungay.Text;
            ViewState["denngay"] = txt_denngay.Text;
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
            if (Request.Cookies["cookie_lsnhapxuat"] != null)
                Response.Cookies["cookie_lsnhapxuat"].Expires = DateTime.Now.AddYears(-1);
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

        check_login_cl.check_login_admin("13", "13");

        var selectedIds = new List<Int64>(); // Danh sách để lưu trữ ID của các mục đã được chọn

        // Thu thập tất cả ID của các mục đã được chọn trong Repeater1
        foreach (RepeaterItem item in Repeater1.Items)
        {
            CheckBox chkItem = (CheckBox)item.FindControl("checkID");
            // Label lblData = (Label)item.FindControl("lbID");
            var lblData = item.FindControl("lbID") as System.Web.UI.WebControls.Label;


            if (chkItem != null && lblData != null && chkItem.Checked)
            {
                int id = int.Parse(lblData.Text);
                selectedIds.Add(id); // Thêm ID vào danh sách
            }
        }

        if (selectedIds.Count > 0)
        {
            using (dbDataContext db = new dbDataContext())
            {
                var ListsToUpdate = db.NhapXuatKho_tbs
                    .Where(d => selectedIds.Contains(d.id))
                    .ToList();

                foreach (var dm in ListsToUpdate)
                {
                    #region CẬP NHẬT SỐ LƯỢNG BÊN KHO
                    string _idsp = dm.id_sanpham;
                    var q_sp = db.KhoSanPham_tbs.FirstOrDefault(p => p.id.ToString() == _idsp);
                    if (q_sp != null)
                    {
                        if (dm.nhap_hay_xuat == true)//nếu là nhập, khi xóa thì trừ lại số lượng
                        {
                            q_sp.soluong_hientai = q_sp.soluong_hientai - dm.soluong_nhap;
                        }
                        else//nếu là xuất, khi xóa thì cộng lại số lượng
                        {
                            q_sp.soluong_hientai = q_sp.soluong_hientai + dm.soluong_nhap;
                        }
                    }
                    #endregion
                    db.NhapXuatKho_tbs.DeleteOnSubmit(dm);

                }

                db.SubmitChanges();
            }
            show_main();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
        }
        else
        {
            // Hiển thị thông báo không có mục nào được chọn
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Không có mục nào được chọn.", "false", "false", "OK", "alert", ""), true);
        }

    }


    #endregion

 

}