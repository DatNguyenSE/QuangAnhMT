using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.UI;


public class check_login_cl
{
    //HttpCookie _ck = HttpContext.Current.Request.Cookies["cookie_userinfo_admin_bcorn"];
    //HttpContext.Current.Response.Write(_ck["taikhoan"] + "<br>");
    //HttpContext.Current.Response.Cookies["cookie_userinfo_admin_bcorn"].Expires = DateTime.Now.AddDays(-1);
    //HttpContext.Current.Response.Write(_tk + "1<br>");
    //HttpContext.Current.Response.Redirect("/123");

    //thông báo từ class ra trang chủ
    //Page page = HttpContext.Current.Handler as Page;
    //ScriptManager.RegisterStartupScript(page, page.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Tài khoản đã bị khóa.", "false", "false", "OK", "alert", ""), true);

    public static void del_all_cookie_session_admin()
    {
        try
        {
            //xóa Cookie cookie_userinfo_admin_bcorn
            HttpCookie myCookie = new HttpCookie("cookie_userinfo_admin_bcorn");
            myCookie.Expires = DateTime.Now.AddYears(-1);
            HttpContext.Current.Response.Cookies.Add(myCookie);
            //xóa tất cả Session
            // HttpContext.Current.Session.Clear();
            HttpContext.Current.Session["taikhoan"] = "";
            HttpContext.Current.Session["matkhau"] = "";
        }
        catch (Exception _ex)
        {
            string _tk = HttpContext.Current.Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    public static void check_login_admin(string _quyen1, string _quyen2)
    {
        using (dbDataContext db = new dbDataContext())
        {
            #region XỬ LÝ TÀI KHOẢN. LẤY TÀI KHOẢN VÀ MẬT KHẨU ĐÃ ĐƯỢC MÃ HÓA --> Giải mã
            string _tk = "", _mk = "", _tk_mahoa = "", _mk_mahoa = "";
            // Lấy giá trị từ cookie
            HttpCookie _ck = HttpContext.Current.Request.Cookies["cookie_userinfo_admin_bcorn"];
            if (_ck != null && !string.IsNullOrEmpty(_ck["taikhoan"]) && !string.IsNullOrEmpty(_ck["matkhau"]))
            {
                // Nếu có cookie, thì lấy giá trị từ cookie và giải mã chúng
                _tk_mahoa = _ck["taikhoan"];
                _mk_mahoa = _ck["matkhau"];
                _tk = mahoa_cl.giaima_Bcorn(_tk_mahoa);
                _mk = mahoa_cl.giaima_Bcorn(_mk_mahoa);
            }
            else
            {
                // Nếu không có cookie, thì kiểm tra session. Nếu có session, thì lấy giá trị từ session
                if (HttpContext.Current.Session["taikhoan"] != null && HttpContext.Current.Session["matkhau"] != null)
                {
                    _tk_mahoa = HttpContext.Current.Session["taikhoan"].ToString();
                    _mk_mahoa = HttpContext.Current.Session["matkhau"].ToString();
                    _tk = mahoa_cl.giaima_Bcorn(_tk_mahoa);
                    _mk = mahoa_cl.giaima_Bcorn(_mk_mahoa);
                }
            }
            #endregion

            #region KIỂM TRA TÍNH HỢP LỆ & QUYỀN CỦA TÀI KHOẢN
            if (!taikhoan_cl.exist_taikhoan(_tk)) // nếu tài khoản không tồn tại
            {
                del_all_cookie_session_admin(); // xóa toàn bộ Cookie và Session
                                                // lưu nội dung thông báo
                HttpContext.Current.Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Vui lòng đăng nhập để tiếp tục.", "2000", "warning");
                HttpContext.Current.Response.Redirect("/admin/login.aspx"); // chuyển trang và nhận thông báo
            }
            else // nếu tài khoản tồn tại
            {
                // lấy thông tin tài khoản và xử lý tiếp
                var _ob = (from tk in db.taikhoan_tbs
                           where tk.taikhoan == _tk
                           select new
                           { tk.matkhau, tk.block, tk.hansudung, tk.permission, tk.phanloai }).FirstOrDefault();
                if (_mk != _ob.matkhau) // so sánh với mật khẩu được giải mã từ Cookie, nếu khác nhau
                {
                    del_all_cookie_session_admin(); // xóa toàn bộ Cookie và Session
                    HttpContext.Current.Session["thongbao"] = thongbao_class.metro_dialog_onload("Thông báo", "Mật khẩu đã được thay đổi. <br/>Vui lòng đăng nhập lại.", "false", "false", "OK", "alert", "");
                    HttpContext.Current.Response.Redirect("/admin/login.aspx"); // chuyển trang và nhận thông báo

                }
                else // tiếp tục xử lý
                {
                    if (_ob.block == true) // nếu tài khoản này bị khóa
                    {
                        del_all_cookie_session_admin(); // xóa toàn bộ Cookie và Session
                        HttpContext.Current.Session["thongbao"] = thongbao_class.metro_dialog_onload("Thông báo", "Tài khoản đã bị khóa.", "false", "false", "OK", "alert", "");
                        HttpContext.Current.Response.Redirect("/admin/login.aspx"); // chuyển trang và nhận thông báo

                    }
                    else
                    {
                        if (_ob.hansudung != null && DateTime.Now > _ob.hansudung.Value) // nếu có hạn sử dụng và hết hạn
                        {
                            del_all_cookie_session_admin(); // xóa toàn bộ Cookie và Session
                            HttpContext.Current.Session["thongbao"] = thongbao_class.metro_dialog_onload("Thông báo", "Tài khoản của bạn đã hết hạn sử dụng.", "false", "false", "OK", "alert", "");
                            HttpContext.Current.Response.Redirect("/admin/login.aspx"); // chuyển trang và nhận thông báo

                        }
                        else // kiểm tra loại tài khoản
                        {
                            if (_ob.phanloai != "Quản trị") // nếu Phân loại tk không phải là Quản trị
                            {
                                del_all_cookie_session_admin(); // xóa toàn bộ Cookie và Session
                                HttpContext.Current.Session["thongbao"] = thongbao_class.metro_dialog_onload("Thông báo", "Loại tài khoản không hợp lệ.", "false", "false", "OK", "alert", "");
                                HttpContext.Current.Response.Redirect("/admin/login.aspx"); // chuyển trang và nhận thông báo

                            }
                            else // kiểm tra quyền
                            {
                                string _quyen = _ob.permission;
                                if (_quyen1 == "none" || _quyen2 == "none" || _quyen.IndexOf(_quyen1) != -1 || _quyen.IndexOf(_quyen2) != -1) // có quyền
                                {
                                    // Gia hạn cookie. Tôi sợ lâu ngày họ quên mật khẩu, với lại sợ chậm nên chưa dùng đoạn sau
                                    //_ck["taikhoan"] = _tk_mahoa;
                                    //_ck["matkhau"] = _mk_mahoa;
                                    //_ck.Expires = DateTime.Now.AddDays(7);
                                    //HttpContext.Current.Response.Cookies.Set(_ck);
                                    // gia hạn session. Đảm bảo khi nào cũng có Session
                                    HttpContext.Current.Session["taikhoan"] = _tk_mahoa;
                                    HttpContext.Current.Session["matkhau"] = _mk_mahoa;
                                }
                                else // nếu k có quyền
                                {
                                    HttpContext.Current.Session["thongbao"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
                                    HttpContext.Current.Response.Redirect("/admin/default.aspx"); // chuyển trang và nhận thông báo

                                }
                            }
                        }
                    }
                }
            }
            #endregion
        }
    }






    public static bool CheckQuyen(dbDataContext db, string _tk, string _quyen)
    {
        // Chỉ lấy trường permission từ database
        var permissionOfUser = db.taikhoan_tbs
            .Where(p => p.taikhoan == _tk)
            .Select(p => p.permission)
            .FirstOrDefault();

        if (permissionOfUser != null)
        {
            // So khớp chính xác từng quyền
            string[] permissions = permissionOfUser.Split(',');
            return permissions.Any(permission => permission == _quyen);
        }

        return false;
    }

}