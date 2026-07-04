using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_data_khach_hang_Default : System.Web.UI.Page
{
    DateTime_cl dt_cl = new DateTime_cl();
    public void set_dulieu_macdinh()
    {
        try
        {
            ResetButtonCss();//button chọn ngày nhanh
            txt_show.Text = "30";
            ViewState["current_page_datakh"] = "1";
            //txt_tungay.Text = dt_cl.return_ngaydaunam(DateTime.Now.Year.ToString()).ToShortDateString();
            //txt_denngay.Text = dt_cl.return_ngaycuoinam(DateTime.Now.Year.ToString()).ToShortDateString();
            //ViewState["tungay"] = txt_tungay.Text;
            //ViewState["denngay"] = txt_denngay.Text;

            #region set_get_cookie
            // Lấy cookie "cookie_datakh" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_datakh"];
            if (cookie == null)
            {

                cookie = new HttpCookie("cookie_datakh");
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
                ViewState["current_page_datakh"] = cookie["trang_hientai"];
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
            check_login_cl.check_login_admin("20", "21");

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
                var nhanviens = db.taikhoan_tbs.Where(p => p.trangthai_lamviec == "Đang làm việc" && (p.phanloai == "Quản trị" || p.phanloai == "Nhân viên")).Select(p => new { p.taikhoan, p.hoten }).ToList();
                txt_nhanvien_chamsoc.DataSource = nhanviens;
                txt_nhanvien_chamsoc.DataValueField = "taikhoan";
                txt_nhanvien_chamsoc.DataTextField = "hoten";
                txt_nhanvien_chamsoc.DataBind();
                txt_nhanvien_chamsoc.Items.Insert(0, new ListItem("Chọn nhân viên chăm sóc", ""));
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

            #region lấy dữ liệu
            var list_all = (from ob1 in db.Data_KhachHang_tbs
                            join ob2 in db.taikhoan_tbs on ob1.nhanvien_chamsoc equals ob2.taikhoan into TaiKhoan
                            from ob2 in TaiKhoan.DefaultIfEmpty()
                            select new
                            {
                                ob1.id,
                                TenKH = ob1.ten,
                                SDT_KH = ob1.sdt,
                                DiaChi = ob1.diachi,
                                ob1.ngay_capnhat,
                                taikhoan_NV_ChamSoc = ob1.nhanvien_chamsoc,
                                HoTenNhanVien = ob2 == null ? "" : ob2.hoten,
                                SoLanBaoGia = db.BaoGia_tbs.Count(bg => bg.sdt_khachhang == ob1.sdt), // Đếm số lần báo giá
                                SoLanDaBan = db.BaoGia_tbs.Count(bg => bg.sdt_khachhang == ob1.sdt && bg.trangthai == "Đã ký HĐ")
                            }).AsQueryable();

            if (check_login_cl.CheckQuyen(db, ViewState["taikhoan"].ToString(), "21"))
            { list_all = list_all.Where(p => p.taikhoan_NV_ChamSoc == ViewState["taikhoan"].ToString()); }

            // Kiểm tra xem textbox có dữ liệu tìm kiếm không
            string _key = txt_timkiem.Text.Trim();
            if (!string.IsNullOrEmpty(_key))
                list_all = list_all.Where(p => p.TenKH.Contains(_key) || p.SDT_KH.Contains(_key) || p.DiaChi.Contains(_key) || p.id.ToString() == _key);
            else
            {
                string _key1 = txt_timkiem1.Text.Trim();
                if (!string.IsNullOrEmpty(_key1))
                    list_all = list_all.Where(p => p.TenKH.Contains(_key1) || p.SDT_KH.Contains(_key1) || p.DiaChi.Contains(_key1) || p.id.ToString() == _key1);
            }

            //xử lý theo thời gian
            if (txt_tungay.Text != "")
                list_all = list_all.Where(p => p.ngay_capnhat.Value.Date >= DateTime.Parse(txt_tungay.Text).Date);
            if (txt_denngay.Text != "")
                list_all = list_all.Where(p => p.ngay_capnhat.Value.Date <= DateTime.Parse(txt_denngay.Text).Date);


            //sắp xếp
            list_all = list_all.OrderByDescending(p => p.ngay_capnhat);
            int _Tong_Record = list_all.Count();


            #endregion

            #region phân trang OK, k sửa
            // Xử lý số record mỗi trang
            int show = Number_cl.Check_Int(txt_show.Text.Trim()); if (show <= 0) show = 30;
            //xử lý trang hiện tại. Đảm bảo current_page không nhỏ hơn 1 và không lớn hơn total_page
            int current_page = int.Parse(ViewState["current_page_datakh"].ToString()); int total_page = number_of_page_class.return_total_page(_Tong_Record, show); if (current_page < 1) current_page = 1; else if (current_page > total_page) current_page = total_page;
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
    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        try
        {
            ViewState["current_page_datakh"] = int.Parse(ViewState["current_page_datakh"].ToString()) - 1;
            #region LƯU TRANG HIỆN TẠI
            // Lấy cookie "cookie_datakh" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_datakh"];
            if (cookie != null)
            {
                cookie["trang_hientai"] = ViewState["current_page_datakh"].ToString();
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
            ViewState["current_page_datakh"] = int.Parse(ViewState["current_page_datakh"].ToString()) + 1;
            #region LƯU TRANG HIỆN TẠI
            // Lấy cookie "cookie_datakh" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_datakh"];
            if (cookie != null)
            {
                cookie["trang_hientai"] = ViewState["current_page_datakh"].ToString();
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
            ViewState["current_page_datakh"] = 1;
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
            if (Request.Cookies["cookie_datakh"] != null)//nếu có ck r thì lưu giá trị mới
            {
                HttpCookie _ck = Request.Cookies["cookie_datakh"];
                _ck["show"] = txt_show.Text;
                _ck["trang_hientai"] = ViewState["current_page_datakh"].ToString();
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
            if (Request.Cookies["cookie_datakh"] != null)
                Response.Cookies["cookie_datakh"].Expires = DateTime.Now.AddYears(-1);
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

        check_login_cl.check_login_admin("22", "23");

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
                var ListsToUpdate = db.Data_KhachHang_tbs
                    .Where(d => selectedIds.Contains(d.id))
                    .ToList();

                foreach (var dm in ListsToUpdate)
                {
                    db.Data_KhachHang_tbs.DeleteOnSubmit(dm);
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

    protected void but_xoa_item_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("22", "23");
            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;
            using (dbDataContext db = new dbDataContext())
            {
                var dm = db.Data_KhachHang_tbs.FirstOrDefault(p => p.id.ToString() == _id);
                if (dm != null)
                {
                    db.Data_KhachHang_tbs.DeleteOnSubmit(dm);
                    db.SubmitChanges();
                }
            }
            show_main();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa thành công.", "1000", "warning"), true);
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk)) _tk = mahoa_cl.giaima_Bcorn(_tk); else _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    #endregion

    #region Thêm khách hàng
    protected void but_show_form_add_Click(object sender, EventArgs e)
    {
        txt_sdt.Text = "";
        txt_tenkh.Text = "";
        txt_diachi.Text = "";
        txt_nhanvien_chamsoc.SelectedIndex = 0;
        ViewState["add_edit"] = "add";
        Label1.Text = "THÊM KHÁCH HÀNG";
        but_add_edit.Text = "THÊM MỚI";
        pn_add.Visible = true;
        up_add.Update();
    }
    
    protected void but_show_chinhsua_Click(object sender, EventArgs e)
    {
        try
        {
            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;
            ViewState["id_edit"] = _id;
            using (dbDataContext db = new dbDataContext())
            {
                var q = db.Data_KhachHang_tbs.FirstOrDefault(p => p.id.ToString() == _id);
                if (q != null)
                {
                    txt_sdt.Text = q.sdt;
                    txt_tenkh.Text = q.ten;
                    txt_diachi.Text = q.diachi;
                    if (q.nhanvien_chamsoc != null && txt_nhanvien_chamsoc.Items.FindByValue(q.nhanvien_chamsoc) != null)
                        txt_nhanvien_chamsoc.SelectedValue = q.nhanvien_chamsoc;
                    else
                        txt_nhanvien_chamsoc.SelectedIndex = 0;
                    
                    ViewState["add_edit"] = "edit";
                    Label1.Text = "CẬP NHẬT KHÁCH HÀNG";
                    but_add_edit.Text = "CẬP NHẬT";
                }
            }
            pn_add.Visible = true;
            up_add.Update();
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk)) _tk = mahoa_cl.giaima_Bcorn(_tk); else _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    protected void but_close_form_add_Click(object sender, EventArgs e)
    {
        pn_add.Visible = false;
        up_add.Update();
    }

    protected void but_add_edit_Click(object sender, EventArgs e)
    {
        string _sdt = txt_sdt.Text.Trim();
        string _ten = txt_tenkh.Text.Trim();
        string _diachi = txt_diachi.Text.Trim();
        string _nhanvien = txt_nhanvien_chamsoc.SelectedValue;
        if (string.IsNullOrEmpty(_nhanvien)) _nhanvien = null;

        if (string.IsNullOrEmpty(_sdt))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập số điện thoại.", "false", "false", "OK", "alert", ""), true);
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            if (ViewState["add_edit"] != null && ViewState["add_edit"].ToString() == "edit")
            {
                var ob = db.Data_KhachHang_tbs.FirstOrDefault(p => p.id.ToString() == ViewState["id_edit"].ToString());
                if (ob != null)
                {
                    var check = db.Data_KhachHang_tbs.FirstOrDefault(p => p.sdt == _sdt && p.id != ob.id);
                    if (check != null)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Số điện thoại đã tồn tại trong hệ thống.", "false", "false", "OK", "alert", ""), true);
                        return;
                    }
                    ob.sdt = _sdt;
                    ob.ten = _ten;
                    ob.diachi = _diachi;
                    ob.nhanvien_chamsoc = _nhanvien;
                    db.SubmitChanges();
                }
            }
            else
            {
                var check = db.Data_KhachHang_tbs.FirstOrDefault(p => p.sdt == _sdt);
                if (check != null)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Số điện thoại đã tồn tại trong hệ thống.", "false", "false", "OK", "alert", ""), true);
                    return;
                }

                Data_KhachHang_tb ob = new Data_KhachHang_tb();
                ob.sdt = _sdt;
                ob.ten = _ten;
                ob.diachi = _diachi;
                ob.ngay_capnhat = DateTime.Now;
                ob.nhanvien_chamsoc = _nhanvien;
                
                db.Data_KhachHang_tbs.InsertOnSubmit(ob);
                db.SubmitChanges();
            }
        }

        if (ViewState["add_edit"] != null && ViewState["add_edit"].ToString() == "edit")
        {
            pn_add.Visible = false;
        }
        up_add.Update();
        show_main();
        up_main.Update(); // Cập nhật lại grid dữ liệu bên ngoài
        
        // Reset lại form để tiện nhập tiếp
        txt_sdt.Text = "";
        txt_tenkh.Text = "";
        txt_diachi.Text = "";
        txt_nhanvien_chamsoc.SelectedIndex = 0;

        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "success"), true);
    }
    #endregion
}