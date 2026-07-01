using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_quan_ly_cong_viec_Default : System.Web.UI.Page
{
    DateTime_cl dt_cl = new DateTime_cl();
    public void set_dulieu_macdinh()
    {
        try
        {
            ResetButtonCss();//button chọn ngày nhanh
            txt_show.Text = "30";
            ViewState["current_page_congviec"] = "1";
            //txt_tungay.Text = dt_cl.return_ngaydaunam(DateTime.Now.Year.ToString()).ToShortDateString();
            //txt_denngay.Text = dt_cl.return_ngaycuoinam(DateTime.Now.Year.ToString()).ToShortDateString();
            //ViewState["tungay"] = txt_tungay.Text;
            //ViewState["denngay"] = txt_denngay.Text;

            #region set_get_cookie
            // Lấy cookie "cookie_congviec" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_congviec"];
            if (cookie == null)
            {

                cookie = new HttpCookie("cookie_congviec");
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
                ViewState["current_page_congviec"] = cookie["trang_hientai"];
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
            check_login_cl.check_login_admin("30", "31");

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
                #region tính toán Trễ hạn
                DateTime _ngayhientai = DateTime.Now;
                var q_check_all = db.CheckAll_tbs.FirstOrDefault(p => p.ngay.Value.Date == _ngayhientai.Date && p.hangmuc == "CongViec" && p.CheckAll == true);
                if (q_check_all == null)
                {
                    var q = db.CongViec_tbs.Where(p => p.trangthai != "Hoàn thành");
                    foreach (var t in q)
                    {
                        t.trehan = (t.ThoiHan != null && t.ThoiHan.Value.Date < _ngayhientai);
                    }

                    // Đánh dấu là đã check
                    CheckAll_tb _ob1 = new CheckAll_tb
                    {
                        ngay = _ngayhientai, // Lưu ngày giờ chính xác của lần cập nhật
                        hangmuc = "CongViec",
                        CheckAll = true
                    };
                    db.CheckAll_tbs.InsertOnSubmit(_ob1);
                    db.SubmitChanges();
                }

                #endregion
            }

            set_dulieu_macdinh();
            show_main();

        }
    }
    #region main - phân trang - tìm kiếm
    public string trave_hoten_nguoinhan(dbDataContext db, string _list_vn)
    {
        string _kq = "";
        string[] _arr = _list_vn.Split(','); // Tách thành mảng
        foreach (string _tk in _arr)
        {
            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _tk);
            if (q != null)
            {
                _kq = _kq + "<div>" + q.hoten + "</div>";
            }
        }
        return _kq;
    }

    public void show_main()
    {

        using (dbDataContext db = new dbDataContext())
        {

            #region lấy dữ liệu
            var list_all = (from ob1 in db.CongViec_tbs
                            join ob2 in db.taikhoan_tbs on ob1.nguoigiao equals ob2.taikhoan into TaiKhoan
                            from ob2 in TaiKhoan.DefaultIfEmpty()
                            select new
                            {
                                ob1.id,
                                ob1.TenCongViec,
                                ob1.Gap_KhongGap,
                                ob1.nguoinhan_list,
                                HoTen_NguoiNhan = trave_hoten_nguoinhan(db, ob1.nguoinhan_list),
                                ob1.ThoiHan,
                                ob1.nguoigiao,
                                ob1.ngaygiao,
                                ob1.trangthai,
                                ob1.NguoiBaoHoanThanh,
                                ob1.thoigian_BaoHoanThanh,
                                ob1.GhiChu_KhiBao_HoanThanh,
                                ob1.AnhDinhKem_HoanThanh,
                                ob1.tunhan_chidinh,
                                ob1.trehan,
                                TenNguoiGiao = ob2 == null ? "" : ob2.hoten,
                            }).AsQueryable();

            if (check_login_cl.CheckQuyen(db, ViewState["taikhoan"].ToString(), "31"))
            {
                list_all = list_all.Where(p => p.nguoinhan_list.Contains(ViewState["taikhoan"].ToString()) || p.nguoigiao == ViewState["taikhoan"].ToString() || p.nguoinhan_list == "");
            }

            // Kiểm tra xem textbox có dữ liệu tìm kiếm không
            string _key = txt_timkiem.Text.Trim();
            if (!string.IsNullOrEmpty(_key))
                list_all = list_all.Where(p => p.TenCongViec.Contains(_key) || p.id.ToString() == _key);
            else
            {
                string _key1 = txt_timkiem1.Text.Trim();
                if (!string.IsNullOrEmpty(_key1))
                    list_all = list_all.Where(p => p.TenCongViec.Contains(_key1) || p.id.ToString() == _key1);
            }

            //xử lý theo thời gian
            if (txt_tungay.Text != "")
                list_all = list_all.Where(p => p.ngaygiao.Value.Date >= DateTime.Parse(txt_tungay.Text).Date);
            if (txt_denngay.Text != "")
                list_all = list_all.Where(p => p.ngaygiao.Value.Date <= DateTime.Parse(txt_denngay.Text).Date);


            //sắp xếp
            list_all = list_all.OrderByDescending(p => p.ngaygiao);
            int _Tong_Record = list_all.Count();


            #endregion

            #region phân trang OK, k sửa
            // Xử lý số record mỗi trang
            int show = Number_cl.Check_Int(txt_show.Text.Trim()); if (show <= 0) show = 30;
            //xử lý trang hiện tại. Đảm bảo current_page không nhỏ hơn 1 và không lớn hơn total_page
            int current_page = int.Parse(ViewState["current_page_congviec"].ToString()); int total_page = number_of_page_class.return_total_page(_Tong_Record, show); if (current_page < 1) current_page = 1; else if (current_page > total_page) current_page = total_page;
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
            ViewState["current_page_congviec"] = int.Parse(ViewState["current_page_congviec"].ToString()) - 1;
            #region LƯU TRANG HIỆN TẠI
            // Lấy cookie "cookie_congviec" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_congviec"];
            if (cookie != null)
            {
                cookie["trang_hientai"] = ViewState["current_page_congviec"].ToString();
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
            ViewState["current_page_congviec"] = int.Parse(ViewState["current_page_congviec"].ToString()) + 1;
            #region LƯU TRANG HIỆN TẠI
            // Lấy cookie "cookie_congviec" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_congviec"];
            if (cookie != null)
            {
                cookie["trang_hientai"] = ViewState["current_page_congviec"].ToString();
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
            ViewState["current_page_congviec"] = 1;
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
            if (Request.Cookies["cookie_congviec"] != null)//nếu có ck r thì lưu giá trị mới
            {
                HttpCookie _ck = Request.Cookies["cookie_congviec"];
                _ck["show"] = txt_show.Text;
                _ck["trang_hientai"] = ViewState["current_page_congviec"].ToString();
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
            if (Request.Cookies["cookie_congviec"] != null)
                Response.Cookies["cookie_congviec"].Expires = DateTime.Now.AddYears(-1);
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

        check_login_cl.check_login_admin("34", "34");

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
                var ListsToUpdate = db.CongViec_tbs
                    .Where(d => selectedIds.Contains(d.id))
                    .ToList();

                foreach (var dm in ListsToUpdate)
                {
                    db.CongViec_tbs.DeleteOnSubmit(dm);
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

    #region ADD - EDIT - CHI TIẾT
    protected void RadioButton3_CheckedChanged(object sender, EventArgs e)
    {
        if (RadioButton3.Checked)
            PlaceHolder1.Visible = false;
        else
            PlaceHolder1.Visible = true;
    }

    protected void RadioButton4_CheckedChanged(object sender, EventArgs e)
    {
        if (RadioButton3.Checked)
            PlaceHolder1.Visible = false;
        else
            PlaceHolder1.Visible = true;
    }
    public void reset_control_add_edit()
    {
        Label1.Text = null;
        ViewState["add_edit"] = null;
        ListBox1.DataSource = null;
        ListBox1.DataBind();
        TextBox2.Text = ""; TextBox1.Text = "";
        RadioButton1.Checked = true; RadioButton3.Checked = true;
    }

    protected void but_show_form_add_Click(object sender, EventArgs e)
    {

        check_login_cl.check_login_admin("32", "32");
        reset_control_add_edit();
        ViewState["add_edit"] = "add";
        Label1.Text = "GIAO VIỆC";
        but_add_edit.Text = "GIAO VIỆC";

        using (dbDataContext db = new dbDataContext())
        {
            var q = db.taikhoan_tbs.Where(p => p.taikhoan != ViewState["taikhoan"].ToString()).Select(c => new { c.taikhoan, c.hoten }).AsQueryable();
            ListBox1.DataSource = q;
            ListBox1.DataValueField = "taikhoan";
            ListBox1.DataTextField = "hoten";
            ListBox1.DataBind();
            ListBox1.Items.Insert(0, new ListItem("Chọn", ""));
            ListBox1.SelectedIndex = 0;

            //TextBox2.Text = DateTime.Now.ToShortDateString();
        }

        pn_add.Visible = !pn_add.Visible;
        up_add.Update();

    }
    protected void but_close_form_add_Click(object sender, EventArgs e)
    {

        //reset control
        reset_control_add_edit();
        //ẩn form
        pn_add.Visible = !pn_add.Visible;

    }




    protected void but_show_chinhsua_Click(object sender, EventArgs e)
    {

        check_login_cl.check_login_admin("33", "33");
        ViewState["add_edit"] = "edit";
        Label1.Text = "SỬA CÔNG VIỆC";
        but_add_edit.Text = "CẬP NHẬT";
        using (dbDataContext db = new dbDataContext())
        {
            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;
            //truy vấn dữ liệu để sửa
            var q = db.CongViec_tbs.FirstOrDefault(p => p.id.ToString() == _id);
            if (q != null)
            {
                if(q.trangthai=="Hoàn thành")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Không thể chỉnh sửa công việc đã hoàn thành.", "false", "false", "OK", "alert", ""), true);
                    return;
                }    
                ViewState["id_edit"] = _id;
                TextBox1.Text = q.TenCongViec;
                if(q.Gap_KhongGap==true)
                {
                    RadioButton2.Checked = true;
                    RadioButton1.Checked = false;
                }    
                else
                {
                    RadioButton2.Checked = false ;
                    RadioButton1.Checked = true;
                }
                if (q.tunhan_chidinh == true)
                {
                    RadioButton3.Checked = true;
                    RadioButton4.Checked = false;
                    PlaceHolder1.Visible = false;
                }
                else
                {
                    PlaceHolder1.Visible = true;
                    RadioButton3.Checked = false;
                    RadioButton4.Checked = true;
                    var q123 = db.taikhoan_tbs.Where(p => p.taikhoan != ViewState["taikhoan"].ToString()).Select(c => new { c.taikhoan, c.hoten }).AsQueryable();
                    ListBox1.DataSource = q123;
                    ListBox1.DataValueField = "taikhoan";
                    ListBox1.DataTextField = "hoten";
                    ListBox1.DataBind();
                    ListBox1.Items.Insert(0, new ListItem("Chọn", ""));
                    string _list_nguoinhan = q.nguoinhan_list;
                    string[] selectedAccounts = _list_nguoinhan.Split(',');
                    foreach (ListItem item in ListBox1.Items)
                    {
                        if (selectedAccounts.Contains(item.Value))
                        {
                            item.Selected = true;
                        }
                    }
                }
                if (q.ThoiHan != null)
                    TextBox2.Text = q.ThoiHan.Value.ToShortDateString();

                

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

        reset_control_add_edit();
        //ẩn form
        pn_add.Visible = !pn_add.Visible;

    }
    //code chung. add hoặc update

    protected void but_add_edit_Click(object sender, EventArgs e)
    {

        #region Chuẩn bị dữ liệu
        string _t1 = TextBox1.Text.Trim();//tên công việc
        List<string> list_nv = new List<string>();
        foreach (ListItem item in ListBox1.Items)
        {
            if (item.Selected && !string.IsNullOrWhiteSpace(item.Value))
            {
                list_nv.Add(item.Value);
            }
        }
        string _listnv = string.Join(",", list_nv);
        bool _tunhan_chidinh = RadioButton3.Checked;
        bool _gap_khonggap = RadioButton2.Checked;
        string _thoihan = TextBox2.Text.Trim();
        string _nguoigiao = ViewState["taikhoan"].ToString();
        DateTime _ngaygiao = DateTime.Now;
        #endregion


        using (dbDataContext db = new dbDataContext())
        {
            #region Kiểm tra ngoại lệ.
            if (_t1 == "")
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập mô tả công việc.", "false", "false", "OK", "alert", ""), true);
                return;
            }
            if (_listnv == "" && _tunhan_chidinh == false)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng chọn nhân viên nhận việc.", "false", "false", "OK", "alert", ""), true);
                return;
            }
            #endregion

            if (ViewState["add_edit"].ToString() == "add")
            {
                #region thêm mới
                if (_tunhan_chidinh == false)//nếu là chỉ định
                {
                    CongViec_tb _ob = new CongViec_tb();
                    _ob.ngaygiao = _ngaygiao; _ob.nguoigiao = _nguoigiao; _ob.trangthai = "Đã giao"; _ob.TenCongViec = _t1; _ob.Gap_KhongGap = _gap_khonggap;
                    _ob.trehan = false;
                    if (_thoihan != "")
                    {
                        _ob.ThoiHan = DateTime.Parse(_thoihan);
                        if (_ob.ThoiHan.Value.Date < DateTime.Now.Date)
                            _ob.trehan = true;
                    }
                    else
                    {
                        _ob.ThoiHan = null; _ob.trehan = false;
                    }
                    _ob.tunhan_chidinh = _tunhan_chidinh;
                    _ob.nguoinhan_list = _listnv; 
                    db.CongViec_tbs.InsertOnSubmit(_ob);
                    foreach (var t in list_nv)//giao việc cho nhiều nhân viên trong list được chọn
                    {
                        if (!string.IsNullOrWhiteSpace(t))
                        {
                            //thông báo cho người nhận việc
                            ThongBao_tb _ob4 = new ThongBao_tb();
                            _ob4.id = Guid.NewGuid();
                            _ob4.daxem = false;//chưa xem
                            _ob4.nguoithongbao = ViewState["taikhoan"].ToString();
                            _ob4.nguoinhan = t;
                            _ob4.link = "/admin/quan-ly-cong-viec/default.aspx";
                            _ob4.noidung = db.taikhoan_tbs.First(p => p.taikhoan == ViewState["taikhoan"].ToString()).hoten + " vừa giao việc cho bạn.";
                            _ob4.thoigian = DateTime.Now;
                            _ob4.bin = false;
                            db.ThongBao_tbs.InsertOnSubmit(_ob4);
                        }
                    }
                }
                else//nếu là tự nhận
                {
                    CongViec_tb _ob = new CongViec_tb();
                    _ob.ngaygiao = _ngaygiao; _ob.nguoigiao = _nguoigiao; _ob.trangthai = "Đã giao"; _ob.TenCongViec = _t1; _ob.Gap_KhongGap = _gap_khonggap;
                    _ob.trehan = false;
                    if (_thoihan != "")
                    {
                        _ob.ThoiHan = DateTime.Parse(_thoihan);
                        if (_ob.ThoiHan.Value.Date < DateTime.Now.Date)
                            _ob.trehan = true;
                    }
                    else
                    {
                        _ob.ThoiHan = null; _ob.trehan = true;
                    }
                    _ob.tunhan_chidinh = _tunhan_chidinh;
                    _ob.nguoinhan_list = ""; 
                    db.CongViec_tbs.InsertOnSubmit(_ob);
                }
                db.SubmitChanges();
                #endregion

                #region cập nhật dữ liệu và update hiển thị
                reset_control_add_edit();
                pn_add.Visible = !pn_add.Visible;
                show_main();
                up_main.Update();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
                #endregion
            }
            else//edit
            {
                #region Chuẩn bị dữ liệu
                var q_edit = db.CongViec_tbs.FirstOrDefault(p => p.id.ToString() == ViewState["id_edit"].ToString());
                if (q_edit != null)
                {
                    #region Kiểm tra ngoại lệ. Sau đó cập nhật
                    CongViec_tb _ob = q_edit;
                    if (_tunhan_chidinh == false)//nếu là chỉ định
                    {
                        _ob.TenCongViec = _t1; _ob.Gap_KhongGap = _gap_khonggap;
                        if (_thoihan != "")
                        {
                            _ob.ThoiHan = DateTime.Parse(_thoihan);
                            if (_ob.ThoiHan.Value.Date < DateTime.Now.Date)
                                _ob.trehan = true;
                        }
                        else
                        {
                            _ob.ThoiHan = null; _ob.trehan = true;
                        }
                        _ob.tunhan_chidinh = _tunhan_chidinh;
                        _ob.nguoinhan_list = _listnv;
                        foreach (var t in list_nv)//giao việc cho nhiều nhân viên trong list được chọn
                        {
                            if (!string.IsNullOrWhiteSpace(t))
                            {
                                //thông báo cho người nhận việc
                                ThongBao_tb _ob4 = new ThongBao_tb();
                                _ob4.id = Guid.NewGuid();
                                _ob4.daxem = false;//chưa xem
                                _ob4.nguoithongbao = ViewState["taikhoan"].ToString();
                                _ob4.nguoinhan = t;
                                _ob4.link = "/admin/quan-ly-cong-viec/default.aspx";
                                _ob4.noidung = db.taikhoan_tbs.First(p => p.taikhoan == ViewState["taikhoan"].ToString()).hoten + " vừa cập nhật công việc có mặt bạn.";
                                _ob4.thoigian = DateTime.Now;
                                _ob4.bin = false;
                                db.ThongBao_tbs.InsertOnSubmit(_ob4);
                            }
                        }
                    }
                    else
                    {
                         _ob.TenCongViec = _t1; _ob.Gap_KhongGap = _gap_khonggap;
                        if (_thoihan != "")
                        {
                            _ob.ThoiHan = DateTime.Parse(_thoihan);
                            if (_ob.ThoiHan.Value.Date < DateTime.Now.Date)
                                _ob.trehan = true;
                        }
                        else
                        {
                            _ob.ThoiHan = null; _ob.trehan = true;
                        }
                        _ob.tunhan_chidinh = _tunhan_chidinh;
                        _ob.nguoinhan_list = "";
                    }
                    

                    db.SubmitChanges();
                    #region cập nhật dữ liệu và update hiển thị
                    reset_control_add_edit();
                    pn_add.Visible = !pn_add.Visible;
                    show_main();
                    up_main.Update();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
                    #endregion

                    #endregion
                }
                #endregion
            }
        }

    }
    #endregion




    protected void but_nhanviecngay_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;//id công việc
            string _tk = ViewState["taikhoan"].ToString();

            var q = db.CongViec_tbs.FirstOrDefault(p => p.id.ToString() == _id);
            if (q != null)
            {
                if (q.trangthai == "Đã nhận")
                {
                    show_main();
                    up_main.Update();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Việc này đã được nhận.", "false", "false", "OK", "alert", ""), true);
                    return;
                }

                if (q.tunhan_chidinh == true)//nếu đây là việc tự nhận
                {
                    q.nguoinhan_list = _tk;
                }
                else//nếu đây là việc chỉ định
                {
                    bool check_nguoinhan = q.nguoinhan_list.Split(',').Any(x => x.Trim() == _tk);
                    if (check_nguoinhan == false)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không có danh sách nhận việc.", "false", "false", "OK", "alert", ""), true);
                        return;
                    }
                }
                q.trangthai = "Đã nhận";
                q.NguoiBamNhanViec = _tk;
                q.thoigian_BamNhanViec = DateTime.Now;

                //thông báo cho người giao việc
                ThongBao_tb _ob4 = new ThongBao_tb();
                _ob4.id = Guid.NewGuid();
                _ob4.daxem = false;//chưa xem
                _ob4.nguoithongbao = _tk;
                _ob4.nguoinhan = q.nguoigiao;
                _ob4.link = "/admin/quan-ly-cong-viec/default.aspx";
                _ob4.noidung = db.taikhoan_tbs.First(p => p.taikhoan == _tk).hoten + " đã nhận việc bạn giao. ID công việc: " + _id;
                _ob4.thoigian = DateTime.Now;
                _ob4.bin = false;
                db.ThongBao_tbs.InsertOnSubmit(_ob4);

                db.SubmitChanges();

                show_main();
                up_main.Update();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);

            }
        }
    }

    #region báo cáo hoàn thành
    public void reset_control_baohoanthanh()
    {
        ViewState["idcv"] = null;
        txt_link_fileupload.Text = "";
    }
    protected void but_close_form_baohoanthanh_Click(object sender, EventArgs e)
    {
        reset_control_baohoanthanh();
        pn_baohoanthanh.Visible = !pn_baohoanthanh.Visible;
    }
    protected void but_show_form_baohoanthanh_Click(object sender, EventArgs e)
    {
        reset_control_baohoanthanh();
        using (dbDataContext db = new dbDataContext())
        {
            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;//id công việc
            string _tk = ViewState["taikhoan"].ToString();
            var q = db.CongViec_tbs.FirstOrDefault(p => p.id.ToString() == _id);
            if (q != null)
            {
                if (q.trangthai == "Hoàn thành")
                {
                    show_main();
                    up_main.Update();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Việc này đã hoàn thành.", "false", "false", "OK", "alert", ""), true);
                    return;
                }
                bool check_nguoinhan = q.nguoinhan_list.Split(',').Any(x => x.Trim() == _tk);
                if (check_nguoinhan == false)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không có danh sách nhận việc.", "false", "false", "OK", "alert", ""), true);
                    return;
                }
                ViewState["idcv"] = _id;
                pn_baohoanthanh.Visible = !pn_baohoanthanh.Visible;
                up_baohoanthanh.Update();
            }
        }
    }

    protected void but_hoanthanh_Click(object sender, EventArgs e)
    {
        string _ghichu = TextBox3.Text.Trim();
        string _anh = txt_link_fileupload.Text.Trim();
        if (_ghichu == "")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập ghi chú báo cáo.", "false", "false", "OK", "alert", ""), true);
            return;
        }
        using (dbDataContext db = new dbDataContext())
        {
            string _id = ViewState["idcv"].ToString();
            string _tk = ViewState["taikhoan"].ToString();
            var q = db.CongViec_tbs.FirstOrDefault(p => p.id.ToString() == _id);
            if (q != null)
            {
                if (q.trangthai == "Hoàn thành")
                {
                    show_main();
                    up_main.Update();
                    reset_control_baohoanthanh();
                    pn_baohoanthanh.Visible = !pn_baohoanthanh.Visible;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Việc này đã hoàn thành.", "false", "false", "OK", "alert", ""), true);
                    return;
                }

                q.GhiChu_KhiBao_HoanThanh = _ghichu;
                q.AnhDinhKem_HoanThanh = _anh;
                q.NguoiBaoHoanThanh = _tk;
                q.thoigian_BaoHoanThanh = DateTime.Now;
                q.trangthai = "Hoàn thành";

                //thông báo cho người giao việc
                ThongBao_tb _ob4 = new ThongBao_tb();
                _ob4.id = Guid.NewGuid();
                _ob4.daxem = false;//chưa xem
                _ob4.nguoithongbao = _tk;
                _ob4.nguoinhan = q.nguoigiao;
                _ob4.link = "/admin/quan-ly-cong-viec/default.aspx";
                _ob4.noidung = db.taikhoan_tbs.First(p => p.taikhoan == _tk).hoten + " đã hoàn thành công việc. ID công việc: " + _id;
                _ob4.thoigian = DateTime.Now;
                _ob4.bin = false;
                db.ThongBao_tbs.InsertOnSubmit(_ob4);

                db.SubmitChanges();

                show_main();
                up_main.Update();
                reset_control_baohoanthanh();
                pn_baohoanthanh.Visible = !pn_baohoanthanh.Visible;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
            }
        }
    }
    #endregion

}