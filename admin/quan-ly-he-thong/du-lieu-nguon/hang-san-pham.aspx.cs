using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_quan_ly_he_thong_du_lieu_nguon_Default : System.Web.UI.Page
{

    // ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Tài khoản đã bị khóa.", "false", "false", "OK", "alert", ""), true);
    String_cl str_cl = new String_cl();
    public void set_dulieu_macdinh()
    {
        try
        {
  
            ViewState["current_page_hangsp"] = "1";

            #region set_get_cookie
            // Lấy cookie "cookie_hangsp" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_hangsp"];
            if (cookie == null)
            {
                // Nếu cookie không tồn tại, tạo cookie mới
                cookie = new HttpCookie("cookie_hangsp");
                cookie["trang_hientai"] = "1";//lưu trang hiện tại
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
                ViewState["current_page_hangsp"] = cookie["trang_hientai"];
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
            try
            {
                Session["url_back"] = HttpContext.Current.Request.Url.AbsoluteUri;
                check_login_cl.check_login_admin("6", "6");


                //Nó k kịp lưu vì nó tải trang này trước khi load menu-left
                //if (Session["title"] != null)
                //    ViewState["title"] = Session["title"].ToString();

                set_dulieu_macdinh();
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
    }
    #region main - phân trang - tìm kiếm

    public void show_main()
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                #region lấy dữ liệu
                var list_all = (from ob1 in db.DuLieuNguon_tbs.Where(p => p.kyhieu == "hangsanpham")
                                select new
                                {
                                    ob1.id,
                                    ob1.ten,
                                }).AsQueryable();

                // Kiểm tra xem textbox có dữ liệu tìm kiếm không
                string _key = txt_timkiem.Text.Trim();
                if (!string.IsNullOrEmpty(_key))
                    list_all = list_all.Where(p => p.ten.Contains(_key) || p.id.ToString() == _key);
                else
                {
                    string _key1 = txt_timkiem1.Text.Trim();
                    if (!string.IsNullOrEmpty(_key1))
                        list_all = list_all.Where(p => p.ten.Contains(_key1) || p.id.ToString() == _key1);
                }

                //sắp xếp
                list_all = list_all.OrderBy(p => p.ten);
                int _Tong_Record = list_all.Count();
                #endregion

                #region phân trang OK, k sửa
                // Xử lý số record mỗi trang
                int show = 100; if (show <= 0) show = 100;
                //xử lý trang hiện tại. Đảm bảo current_page không nhỏ hơn 1 và không lớn hơn total_page
                int current_page = int.Parse(ViewState["current_page_hangsp"].ToString()); int total_page = number_of_page_class.return_total_page(_Tong_Record, show); if (current_page < 1) current_page = 1; else if (current_page > total_page) current_page = total_page;
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
            check_login_cl.check_login_admin("6", "6");
            ViewState["current_page_hangsp"] = int.Parse(ViewState["current_page_hangsp"].ToString()) - 1;

            #region LƯU TRANG HIỆN TẠI
            // Lấy cookie "cookie_hangsp" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_hangsp"];
            if (cookie != null)
            {
                cookie["trang_hientai"] = ViewState["current_page_hangsp"].ToString();
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
            check_login_cl.check_login_admin("6", "6");
            ViewState["current_page_hangsp"] = int.Parse(ViewState["current_page_hangsp"].ToString()) + 1;

            #region LƯU TRANG HIỆN TẠI
            // Lấy cookie "cookie_hangsp" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_hangsp"];
            if (cookie != null)
            {
                cookie["trang_hientai"] = ViewState["current_page_hangsp"].ToString();
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
            check_login_cl.check_login_admin("6", "6");
            ViewState["current_page_hangsp"] = 1;
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
    //thêm
    protected void but_show_form_add_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("6", "6"); reset_control_add_edit();
            Label1.Text = "THÊM HÃNG";
            but_add_edit.Text = "THÊM MỚI";
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
            check_login_cl.check_login_admin("6", "6");
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


    public void reset_control_add_edit()
    {
        try
        {
            txt_name.Text = "";

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
            check_login_cl.check_login_admin("6", "6");
            using (dbDataContext db = new dbDataContext())
            {

                #region Chuẩn bị dữ liệu
                string _name = str_cl.Remove_Blank(txt_name.Text.Trim());
                #endregion

                #region Kiểm tra ngoại lệ. Sau đó thêm mới
                if (_name == "")
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập tên hãng.", "false", "false", "OK", "alert", ""), true);
                else
                {
                    DuLieuNguon_tb _ob = new DuLieuNguon_tb();
                    _ob.ten = _name;
                    _ob.kyhieu = "hangsanpham";
                    db.DuLieuNguon_tbs.InsertOnSubmit(_ob);
                    db.SubmitChanges();
                    #region cập nhật dữ liệu và update hiển thị
                    txt_name.Text = "";
                    show_main();
                    up_main.Update();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
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
    #endregion


    #region BIN - XÓA - LƯU

    protected void but_remove_bin_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("6", "6");

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
                    // Lấy tất cả các mục có ID trong danh sách và cập nhật thuộc tính `bin` của chúng
                    var danhMucsToUpdate = db.DuLieuNguon_tbs
                        .Where(d => selectedIds.Contains(d.id))
                        .ToList();

                    foreach (var dm in danhMucsToUpdate)
                    {
                        db.DuLieuNguon_tbs.DeleteOnSubmit(dm);
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

    protected void but_save_Click(object sender, EventArgs e)
    {

            check_login_cl.check_login_admin("6", "6");
            // Tạo một danh sách để lưu trữ các cập nhật cần thực hiện
            var updates = new List<(Int64 id, string ten)>();
            // Lấy thông tin từ Repeater1
            foreach (RepeaterItem item in Repeater1.Items)
            {
                // Tìm các điều khiển TextBox và Label từ RepeaterItem
                TextBox txt_ten = (TextBox)item.FindControl("txt_ten");
                Label lblData = (Label)item.FindControl("lbID");

                // Kiểm tra nếu cả TextBox và Label không null
                if (txt_ten != null && lblData != null)
                {
                    // Lấy ID và rank từ các điều khiển
                    string _id = lblData.Text;
                    string _ten = txt_ten.Text.Trim();

                    // Kiểm tra nếu rank không rỗng
                    if (!string.IsNullOrEmpty(_ten))
                    {
                        updates.Add((id: Int64.Parse(_id), ten: _ten));
                    }
                }
            }
            // Cập nhật cơ sở dữ liệu một cách hàng loạt
            using (dbDataContext db = new dbDataContext())
            {
                // Truy vấn và lấy tất cả các mục cần cập nhật trong một lần
                var danhMucsToUpdate = db.DuLieuNguon_tbs
                    .Where(d => updates.Select(u => u.id).Contains(d.id))
                    .ToList();

                // Cập nhật giá trị rank cho tất cả các mục trong danh sách danhMucsToUpdate
                foreach (var dm in danhMucsToUpdate)
                {
                    var update = updates.First(u => u.id == dm.id);
                    dm.ten = update.ten;
                }

                // Lưu các thay đổi vào cơ sở dữ liệu một lần
                db.SubmitChanges();
            }
            show_main();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);

    }

    #endregion

}