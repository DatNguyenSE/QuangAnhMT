using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_uc_menu_top_uc : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                #region vô hiệu hóa timer trên một số trang có CKEditor
                string _url = Request.Url.AbsolutePath.ToLower();
                switch (_url)
                {
                    case "/admin/quan-ly-kho/default.aspx":
                    case "/admin/hang-bao-hanh/default.aspx":
                        //case "/admin/quan-ly-tai-lieu/default.aspx":
                        Timer1.Enabled = false;
                        break;
                    default:
                        Timer1.Enabled = true; // Bật lại Timer nếu không phải là các trang trên
                        break;
                }
                #endregion


                if (Session["title"] != null)
                    ViewState["title"] = Session["title"].ToString();

                ViewState["sapxep_thongbao"] = "1";//mặc định sx thông báo theo mới nhất lên đầu
                but_sapxep_moinhat.CssClass = "info small rounded";

                using (dbDataContext db = new dbDataContext())
                {
                    show_soluong_thongbao(db);
                    lay_thongtin_nguoidung(db);
                }


            }
            catch (Exception _ex)
            {
                string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
                if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
                    _tk = mahoa_cl.giaima_Bcorn(_tk);
                else
                    _tk = "";
                Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
            }
        }
    }

    public void lay_thongtin_nguoidung(dbDataContext db)
    {
        string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
        if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
        {
            _tk = mahoa_cl.giaima_Bcorn(_tk);
            try
            {
                var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _tk);
                ViewState["hoten"] = q.hoten;
                ViewState["anhdaidien"] = q.anhdaidien;
                ViewState["sdt"] = q.dienthoai;
                ViewState["taikhoan"] = _tk;
            }
            catch (Exception _ex)
            {
                Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
            }
        }
    }

    #region thông báo
    protected void Timer1_Tick(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            show_soluong_thongbao(db);
        }
    }

    public void show_soluong_thongbao(dbDataContext db)
    {
        try
        {
            // Lấy giá trị tài khoản từ ViewState một lần
            string taiKhoan = mahoa_cl.giaima_Bcorn(Session["taikhoan"].ToString());

            // Đếm số lượng thông báo chưa đọc
            int soLuongThongBaoChuaDoc = db.ThongBao_tbs.Count(p => p.nguoinhan == taiKhoan && p.daxem == false && p.bin == false);

            // Cập nhật nhãn hiển thị số lượng thông báo
            if (soLuongThongBaoChuaDoc < 100)
                lb_sl_thongbao.Text = soLuongThongBaoChuaDoc.ToString();
            else
                lb_sl_thongbao.Text = "99+";
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    public void show_noidung_thongbao(dbDataContext db)
    {
        try
        {
            // Lấy giá trị tài khoản từ ViewState một lần
            string taiKhoan = mahoa_cl.giaima_Bcorn(Session["taikhoan"].ToString());

            var list_all = (from ob1 in db.ThongBao_tbs
                            join ob2 in db.taikhoan_tbs on ob1.nguoithongbao equals ob2.taikhoan
                            where ob1.nguoinhan == taiKhoan && ob1.bin == false
                            select new
                            {
                                ob1.id, // id thông báo
                                avt_nguoithongbao = ob2.anhdaidien,
                                daxem = ob1.daxem,
                                noidung = ob1.noidung,
                                thoigian = ob1.thoigian,
                                link = ob1.link.Contains("?") ? ob1.link + "&" : ob1.link + "?"
                            }).AsQueryable();

            if (ViewState["sapxep_thongbao"].ToString() == "2")//lọc ra chưa đọc, mới nhất lên đầu
                list_all = list_all.Where(p => p.daxem == false).OrderByDescending(p => p.thoigian).Take(20);
            else//sx theo mới nhất lên đầu
                list_all = list_all.OrderByDescending(p => p.thoigian).Take(20);


            // Gán dữ liệu cho Repeater
            Repeater1.DataSource = list_all;
            Repeater1.DataBind();
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    protected void but_sapxep_moinhat_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            ViewState["sapxep_thongbao"] = "1";
            but_sapxep_moinhat.CssClass = "info small rounded";
            but_sapxep_chuadoc.CssClass = "light small rounded";
            using (dbDataContext db = new dbDataContext())
            {
                show_noidung_thongbao(db);
            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    protected void but_sapxep_chuadoc_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            ViewState["sapxep_thongbao"] = "2";
            but_sapxep_moinhat.CssClass = "light small rounded";
            but_sapxep_chuadoc.CssClass = "info small rounded";
            using (dbDataContext db = new dbDataContext())
            {
                show_noidung_thongbao(db);
            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    protected void but_show_form_thongbao_Click(object sender, EventArgs e)
    {

        check_login_cl.check_login_admin("none", "none");

        string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
        _tk = mahoa_cl.giaima_Bcorn(_tk);
        using (dbDataContext db = new dbDataContext())
        {
            show_noidung_thongbao(db);
            var q = db.ThongBao_tbs.Where(p => p.nguoinhan == _tk && p.daxem == false);
            foreach (var t in q)//nhấn vô nút thông báo là đánh dấu đã xem hết
            {
                t.daxem = true;
            }
            db.SubmitChanges();
        }
        UpdatePanel2.Update();


        // ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", mahoa_cl.giaima_Bcorn(Session["taikhoan"].ToString()), "1000", "warning"), true);

    }
    protected void but_chuadoc_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;
            using (dbDataContext db = new dbDataContext())
            {
                ThongBao_tb q = db.ThongBao_tbs.FirstOrDefault(p => p.id.ToString() == _id);
                q.daxem = false;
                db.SubmitChanges();
                show_noidung_thongbao(db);
                show_soluong_thongbao(db);
                UpdatePanel1.Update();
            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    protected void but_dadoc_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;
            using (dbDataContext db = new dbDataContext())
            {
                ThongBao_tb q = db.ThongBao_tbs.FirstOrDefault(p => p.id.ToString() == _id);
                q.daxem = true;
                db.SubmitChanges();
                show_noidung_thongbao(db);
                show_soluong_thongbao(db);
                UpdatePanel1.Update();
            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    protected void but_xoathongbao_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;
            using (dbDataContext db = new dbDataContext())
            {
                ThongBao_tb q = db.ThongBao_tbs.FirstOrDefault(p => p.id.ToString() == _id);
                q.bin = true;
                db.SubmitChanges();
                show_noidung_thongbao(db);
                show_soluong_thongbao(db);
                UpdatePanel1.Update();
            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    #endregion

    protected void but_dangxuat_Click(object sender, EventArgs e)
    {
        Session["taikhoan"] = "";
        Session["matkhau"] = "";
        if (Request.Cookies["cookie_userinfo_admin_bcorn"] != null)
            Response.Cookies["cookie_userinfo_admin_bcorn"].Expires = DateTime.Now.AddDays(-1);
        Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Đăng xuất thành công.", "2000", "warning");
        Response.Redirect("/admin/login.aspx");
    }
    #region ĐỔI MẬT KHẨU
    protected void but_doimatkhau_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            string _pass_old = TextBox1.Text.Trim();
            string _pass_1 = TextBox2.Text.Trim();
            string _pass_2 = TextBox3.Text.Trim();
            if (_pass_old == "" || _pass_1 == "" || _pass_2 == "")
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng nhập đầy đủ thông tin.", "2000", "warning"), true);
                return;
            }
            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == ViewState["taikhoan"].ToString());
            if (q != null)
            {
                if (_pass_old != q.matkhau)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Mật khẩu hiện tại không đúng.", "2000", "warning"), true);
                    return;
                }
                if (_pass_1 != _pass_2)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Mật khẩu mới không trùng nhau.", "2000", "warning"), true);
                    return;
                }
                taikhoan_tb _ob = q;
                _ob.matkhau = _pass_1;
                db.SubmitChanges();
                Session["taikhoan"] = "";
                Session["matkhau"] = "";
                if (Request.Cookies["cookie_userinfo_admin_bcorn"] != null)
                    Response.Cookies["cookie_userinfo_admin_bcorn"].Expires = DateTime.Now.AddDays(-1);
                Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Đổi mật khẩu thành công. Vui lòng đăng nhập lại.", "2000", "warning");
                Response.Redirect("/admin/login.aspx");
            }
            else
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng tải lại trang.", "2000", "warning"), true);
        }
    }
    protected void but_show_form_doimatkhau_Click(object sender, EventArgs e)
    {
        if (ViewState["taikhoan"].ToString() != "")
        {
            pn_doimatkhau.Visible = !pn_doimatkhau.Visible;
            up_doimatkhau.Update();
        }
    }
    protected void but_close_doimatkhau_Click(object sender, EventArgs e)
    {
        TextBox1.Text = ""; TextBox2.Text = ""; TextBox3.Text = "";
        pn_doimatkhau.Visible = !pn_doimatkhau.Visible;
    }
    #endregion 

}