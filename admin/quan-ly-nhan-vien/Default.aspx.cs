using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Media.Animation;

public partial class admin_Default : System.Web.UI.Page
{

    // ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Tài khoản đã bị khóa.", "false", "false", "OK", "alert", ""), true);
    DanhMuc_cl dm_cl = new DanhMuc_cl();
    String_cl str_cl = new String_cl();
    DateTime_cl dt_cl = new DateTime_cl();

    public void set_dulieu_macdinh()
    {
        try
        {
            ResetButtonCss();//button chọn ngày nhanh
            txt_show.Text = "30";
            ViewState["current_page_qlnv"] = "1";



            #region set_get_cookie
            // Lấy cookie "cookie_qlnv" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_qlnv"];
            if (cookie == null)
            {
                ListBox1.SelectedIndex = 0;//mặc định chọn tất cả phân loại, nếu select=true ngoài html thì k lưu lịch sử đc, kệ mẹ nó cứ làm y vậy đi, đừng quan tâm tới đoạn này
                                           // Nếu cookie không tồn tại, tạo cookie mới
                cookie = new HttpCookie("cookie_qlnv");
                cookie["show"] = txt_show.Text;//lưu số dòng hiển thị mỗi trang
                cookie["trang_hientai"] = "1";//lưu trang hiện tại
                cookie["id_loctheothoigian"] = ddl_thoigian.SelectedValue;
                cookie["tungay"] = txt_tungay.Text;
                cookie["denngay"] = txt_denngay.Text;
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
                ViewState["current_page_qlnv"] = cookie["trang_hientai"];
                ddl_thoigian.SelectedValue = cookie["id_loctheothoigian"];
                txt_tungay.Text = cookie["tungay"];
                txt_denngay.Text = cookie["denngay"];
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
            check_login_cl.check_login_admin("1", "1");

            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";

            ViewState["taikhoan"] = _tk;

            //Nó k kịp lưu vì nó tải trang này trước khi load menu-left
            //if (Session["title"] != null)
            //    ViewState["title"] = Session["title"].ToString();

            set_dulieu_macdinh();
            show_main();

        }
    }
    #region main - phân trang - tìm kiếm
    //protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
    //{
    //    try
    //    {
    //        //lấy ra từng item
    //        var dataItem = (dynamic)e.Item.DataItem;


    //        // Tìm CheckBox1 và thiết lập Checked nếu là nổi bật
    //        var checkBox = (CheckBox)e.Item.FindControl("CheckBox1");
    //        if (checkBox != null)
    //        {
    //            checkBox.Checked = dataItem.noibat;
    //            //hoặc
    //            // Lấy giá trị cần so sánh từ DataItem (sửa 'ten_field' thành tên trường dữ liệu phù hợp)
    //            //string valueToCompare = DataBinder.Eval(dataItem, "Tên Cột")?.ToString() ?? string.Empty;
    //        }
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
    //}
    protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        // Lấy quyền người dùng từ một nơi nào đó, ví dụ: Session, ViewState hoặc từ cơ sở dữ liệu
        string userPermissions = ViewState["quyen"].ToString();
        var permissionsList = userPermissions.Split(',');
        // Kiểm tra nếu quyền của người dùng có quyền 3
        if (permissionsList.Contains("2"))
        {
            // Nếu người dùng có quyền 2, hiển thị giá trị 'LuongCoBan'
            PlaceHolder lblLuongCoBan = (PlaceHolder)e.Item.FindControl("lblLuongCoBan");
            if (lblLuongCoBan != null)
            {
                lblLuongCoBan.Visible = true;
            }
        }
        else
        {
            // Nếu không có quyền 2, ẩn giá trị 'LuongCoBan'
            PlaceHolder lblLuongCoBan = (PlaceHolder)e.Item.FindControl("lblLuongCoBan");
            if (lblLuongCoBan != null)
            {
                lblLuongCoBan.Visible = false;
            }
        }
    }

    public void show_main()
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                #region kiểm tra quyền - k cho xem lương cơ bản
                var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == ViewState["taikhoan"].ToString());
                ViewState["quyen"] = q.permission;
                var permissionsList = q.permission.Split(',');
                if (permissionsList.Contains("2"))
                {
                    PlaceHolder2.Visible = true;
                    PlaceHolder3.Visible = true;
                    PlaceHolder4.Visible = true;
                }
                else
                {
                    PlaceHolder2.Visible = false;
                    PlaceHolder3.Visible = false;
                    PlaceHolder4.Visible = false;
                }
                #endregion


                #region lấy dữ liệu
                var list_all = (from ob1 in db.taikhoan_tbs
                                    //join ob2 in db.DanhMuc_tbs on ob1.id_DanhMuc equals ob2.id.ToString() into danhMucGroup
                                    //from ob2 in danhMucGroup.DefaultIfEmpty()
                                select new
                                {
                                    ob1.taikhoan,
                                    ob1.anhdaidien,
                                    ob1.hoten,
                                    ob1.hoten_khongdau,
                                    ob1.ngaysinh,
                                    ob1.email,
                                    ob1.dienthoai,
                                    ob1.trangthai_lamviec,
                                    ob1.ngayvaolam,
                                    ob1.cccd_mattruoc,
                                    ob1.cccd_matsau,
                                    ob1.so_cccd,
                                    ob1.tennganhang,
                                    ob1.so_tknganhang,
                                    ob1.tenchu_tknganhang,
                                    ob1.loai_nhanvien,
                                    ob1.LuongCoBan,
                                    ob1.PhuCap_AnUong,
                                    ob1.PhuCap_DienThoai,
                                    ob1.PhuCap_TrachNhiem,
                                    ob1.PhuCap_Xangxe,
                                    TongThuNhapThang = (ob1.LuongCoBan ?? 0) + (ob1.PhuCap_AnUong ?? 0) + (ob1.PhuCap_DienThoai ?? 0) + (ob1.PhuCap_TrachNhiem ?? 0) + (ob1.PhuCap_Xangxe ?? 0),
                                    ob1.sdt_nguoithan,
                                    ob1.ten_nguoithan,
                                    ob1.phantram_doanhso_banhang
                                }).AsQueryable();




                // Kiểm tra xem textbox có dữ liệu tìm kiếm không
                string _key = txt_timkiem.Text.Trim();
                if (!string.IsNullOrEmpty(_key))
                    list_all = list_all.Where(p => p.hoten.Contains(_key) || p.hoten_khongdau.Contains(_key) || p.taikhoan == _key || p.dienthoai == _key);
                else
                {
                    string _key1 = txt_timkiem1.Text.Trim();
                    if (!string.IsNullOrEmpty(_key1))
                        list_all = list_all.Where(p => p.hoten.Contains(_key1) || p.hoten_khongdau.Contains(_key1) || p.taikhoan == _key1 || p.dienthoai == _key1);
                }

                //xử lý theo thời gian
                string _id_locthoigian = ddl_thoigian.SelectedValue;
                if (_id_locthoigian == "1")//lọc theo ngày vào làm
                {
                    if (txt_tungay.Text != "")
                        list_all = list_all.Where(p => p.ngayvaolam.Value.Date >= DateTime.Parse(txt_tungay.Text).Date);
                    if (txt_denngay.Text != "")
                        list_all = list_all.Where(p => p.ngayvaolam.Value.Date <= DateTime.Parse(txt_denngay.Text).Date);
                }


                //sắp xếp
                list_all = list_all.OrderByDescending(p => p.ngayvaolam);
                int _Tong_Record = list_all.Count();
                ViewState["tongLCB"] = (list_all.Sum(p => (long?)p.LuongCoBan) ?? 0).ToString("#,##0");
                decimal _tong_phucap = list_all.Sum(p => (decimal?)((p.PhuCap_Xangxe ?? 0) + (p.PhuCap_AnUong ?? 0) + (p.PhuCap_DienThoai ?? 0) + (p.PhuCap_TrachNhiem ?? 0))) ?? 0;
                ViewState["tongPhuCap"] = _tong_phucap.ToString("#,##0");
                ViewState["tongThuNhap"] = (list_all.Sum(p => (decimal?)p.TongThuNhapThang) ?? 0).ToString("#,##0");
                #endregion

                #region phân trang OK, k sửa
                // Xử lý số record mỗi trang
                int show = Number_cl.Check_Int(txt_show.Text.Trim()); if (show <= 0) show = 30;
                //xử lý trang hiện tại. Đảm bảo current_page không nhỏ hơn 1 và không lớn hơn total_page
                int current_page = int.Parse(ViewState["current_page_qlnv"].ToString()); int total_page = number_of_page_class.return_total_page(_Tong_Record, show); if (current_page < 1) current_page = 1; else if (current_page > total_page) current_page = total_page;
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
            ViewState["current_page_qlnv"] = int.Parse(ViewState["current_page_qlnv"].ToString()) - 1;
            #region LƯU TRANG HIỆN TẠI
            // Lấy cookie "cookie_qlnv" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_qlnv"];
            if (cookie != null)
            {
                cookie["trang_hientai"] = ViewState["current_page_qlnv"].ToString();
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
            ViewState["current_page_qlnv"] = int.Parse(ViewState["current_page_qlnv"].ToString()) + 1;
            #region LƯU TRANG HIỆN TẠI
            // Lấy cookie "cookie_qlnv" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_qlnv"];
            if (cookie != null)
            {
                cookie["trang_hientai"] = ViewState["current_page_qlnv"].ToString();
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
            ViewState["current_page_qlnv"] = 1;
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

    #region show thùng rác
    protected void but_show_thungrac_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");

            ViewState["current_page_qlnv"] = 1;
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
        try
        {
            Label1.Text = null;
            txt_taikhoan.Text = ""; txt_taikhoan.ReadOnly = false; txt_taikhoan.BackColor = System.Drawing.Color.White;
            txt_matkhau.Text = ""; txt_matkhau.Attributes.Remove("value");
            txt_link_fileupload.Text = ""; txt_link_fileupload1.Text = ""; txt_link_fileupload2.Text = "";
            txt_so_cccd.Text = ""; txt_tennganhang.Text = ""; txt_so_tknganhang.Text = ""; txt_tenchu_tknganhang.Text = "";
            txt_hoten.Text = "";
            txt_ngaysinh.Text = "";
            txt_dienthoai.Text = "";
            txt_ngayvaolam.Text = "";
            ViewState["add_edit"] = null;
            Button2.Visible = false; Button1.Visible = false; Button3.Visible = false;
            Label2.Text = ""; Label3.Text = ""; Label4.Text = "";
            txt_luongcoban.Text = ""; txt_phucap_xangxe.Text = ""; txt_phucap_anuong.Text = ""; txt_phucap_dienthoai.Text = ""; txt_phucap_trachniem.Text = "";
            txt_sdt_nguoithan.Text = "";
            txt_tennguoithan.Text = "";
            txt_phantram_doanhso.Text = "";
            rb_ChinhThuc.Checked = true;
            rb_HocViec.Checked = false;
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
            check_login_cl.check_login_admin("3", "3");
            reset_control_add_edit();
            PlaceHolder1.Visible = true;
            ViewState["add_edit"] = "add";
            Label1.Text = "THÊM NHÂN VIÊN";
            but_add_edit.Text = "THÊM MỚI";

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
    
    protected void but_show_doimatkhau_Click(object sender, EventArgs e)
    {
        LinkButton button = (LinkButton)sender;
        ViewState["taikhoan_doimatkhau"] = button.CommandArgument;
        txt_matkhau_moi.Text = "";
        txt_matkhau_moi.Attributes.Remove("value");
        txt_matkhau_xacnhan.Text = "";
        txt_matkhau_xacnhan.Attributes.Remove("value");
        pn_doimatkhau.Visible = true;
        up_doimatkhau.Update();
    }

    protected void but_close_doimatkhau_Click(object sender, EventArgs e)
    {
        pn_doimatkhau.Visible = false;
        up_doimatkhau.Update();
    }

    protected void but_doimatkhau_Click(object sender, EventArgs e)
    {
        string matkhaumoi = txt_matkhau_moi.Text.Trim();
        string xacnhan = txt_matkhau_xacnhan.Text.Trim();

        if (string.IsNullOrEmpty(matkhaumoi) || string.IsNullOrEmpty(xacnhan))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập đầy đủ thông tin.", "false", "false", "OK", "alert", ""), true);
            return;
        }

        if (matkhaumoi != xacnhan)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Mật khẩu xác nhận không khớp.", "false", "false", "OK", "alert", ""), true);
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            var user = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == ViewState["taikhoan_doimatkhau"].ToString());
            if (user != null)
            {
                user.matkhau = matkhaumoi;
                db.SubmitChanges();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Đổi mật khẩu thành công.", "1000", "success"), true);
                pn_doimatkhau.Visible = false;
                up_doimatkhau.Update();
            }
        }
    }
    protected void but_close_form_add_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
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
    protected void Button2_Click(object sender, EventArgs e)//xóa ảnh cũ
    {

        using (dbDataContext db = new dbDataContext())
        {
            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == ViewState["id_edit"].ToString());
            if (q != null)
            {
                taikhoan_tb _ob = q;
                File_Folder_cl.del_file(_ob.anhdaidien);//xóa ảnh cũ nếu có
                _ob.anhdaidien = "/uploads/images/macdinh.jpg";
                Button2.Visible = false;
                db.SubmitChanges();
                Label2.Text = "";
                txt_link_fileupload.Text = "";
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa ảnh thành công.", "1000", "warning"), true);
            }
        }

    }
    protected void but_show_chinhsua_Click(object sender, EventArgs e)
    {
        try
        {
            //check_login_cl.check_login_admin("4", "4");
            ViewState["add_edit"] = "edit";
            Label1.Text = "CHỈNH SỬA NHÂN VIÊN";
            but_add_edit.Text = "CẬP NHẬT";
            using (dbDataContext db = new dbDataContext())
            {
                LinkButton button = (LinkButton)sender;
                string _id = button.CommandArgument;
                //truy vấn dữ liệu để sửa
                var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _id);
                if (q != null)
                {
                    ViewState["id_edit"] = _id;

                    txt_taikhoan.Text = q.taikhoan;
                    txt_taikhoan.ReadOnly = true; txt_taikhoan.BackColor = System.Drawing.ColorTranslator.FromHtml("#e9ecef");
                    txt_matkhau.Attributes["value"] = q.matkhau;
                    txt_hoten.Text = q.hoten;
                    txt_dienthoai.Text = q.dienthoai;
                    txt_ngaysinh.Text = q.ngaysinh.HasValue ? q.ngaysinh.Value.ToString("dd/MM/yyyy") : "";
                    txt_ngayvaolam.Text = q.ngayvaolam.HasValue ? q.ngayvaolam.Value.ToString("dd/MM/yyyy") : "";
                    txt_link_fileupload.Text = q.anhdaidien;
                    txt_link_fileupload1.Text = q.cccd_mattruoc;
                    txt_link_fileupload2.Text = q.cccd_matsau;
                    txt_so_cccd.Text = q.so_cccd;
                    txt_tennganhang.Text = q.tennganhang;
                    txt_so_tknganhang.Text = q.so_tknganhang;
                    txt_tenchu_tknganhang.Text = q.tenchu_tknganhang;
                    rb_ChinhThuc.Checked = q.loai_nhanvien;
                    rb_HocViec.Checked = !q.loai_nhanvien;
                    PlaceHolder1.Visible = false;
                    if (q.LuongCoBan != null)
                        txt_luongcoban.Text = q.LuongCoBan.Value.ToString("#,##0");
                    if (q.PhuCap_Xangxe != null)
                        txt_phucap_xangxe.Text = q.PhuCap_Xangxe.Value.ToString("#,##0");
                    if (q.PhuCap_AnUong != null)
                        txt_phucap_anuong.Text = q.PhuCap_AnUong.Value.ToString("#,##0");
                    if (q.PhuCap_DienThoai != null)
                        txt_phucap_dienthoai.Text = q.PhuCap_DienThoai.Value.ToString("#,##0");
                    if (q.PhuCap_TrachNhiem != null)
                        txt_phucap_trachniem.Text = q.PhuCap_TrachNhiem.Value.ToString("#,##0");
                    if (q.phantram_doanhso_banhang != null)
                        txt_phantram_doanhso.Text = q.phantram_doanhso_banhang.Value.ToString("#,##0");


                    txt_sdt_nguoithan.Text = q.sdt_nguoithan;
                    txt_tennguoithan.Text = q.ten_nguoithan;

                    if (!string.IsNullOrEmpty(q.anhdaidien))
                    {
                        Button2.Visible = true;
                        Label2.Text = "<div><small>Ảnh cũ</small></div><img src='" + q.anhdaidien + "' style='max-width: 100px' />";
                    }
                    else
                    {
                        Button2.Visible = false;
                        Label2.Text = "";
                    }
                    if (!string.IsNullOrEmpty(q.cccd_mattruoc))
                    {
                        Button1.Visible = true;
                        Label3.Text = "<div><small>Ảnh cũ</small></div><img src='" + q.cccd_mattruoc + "' style='max-width: 100px' />";
                    }
                    else
                    {
                        Button1.Visible = false;
                        Label3.Text = "";
                    }
                    if (!string.IsNullOrEmpty(q.cccd_matsau))
                    {
                        Button3.Visible = true;
                        Label4.Text = "<div><small>Ảnh cũ</small></div><img src='" + q.cccd_matsau + "' style='max-width: 100px' />";
                    }
                    else
                    {
                        Button3.Visible = false;
                        Label4.Text = "";
                    }


                    //hiện form add_edit trong updatePanel_add
                    pn_add.Visible = !pn_add.Visible;
                    up_add.Update();
                }
                else
                    ViewState["id_edit"] = "";
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
    protected void but_close_chinhsua_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
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
    //code chung. add hoặc update

    protected void but_add_edit_Click(object sender, EventArgs e)
    {
        try
        {
            #region Chuẩn bị dữ liệu
            //đảm bảo luôn có thư mục chứa ảnh
            if (!Directory.Exists(Server.MapPath("~/uploads/img-handler/"))) Directory.CreateDirectory(Server.MapPath("~/uploads/img-handler/"));
            //xử lý dữ liệu đầu vào
            string _user = txt_taikhoan.Text.Trim().ToLower();
            string _pass = txt_matkhau.Text.Trim().ToLower();
            string _anhdaiien = txt_link_fileupload.Text;
            string _cccd_mattruoc = txt_link_fileupload1.Text;
            string _cccd_matsau = txt_link_fileupload2.Text;
            string _so_cccd = txt_so_cccd.Text.Trim();
            string _tennganhang = txt_tennganhang.Text.Trim();
            string _so_tknganhang = txt_so_tknganhang.Text.Trim();
            string _tenchu_tknganhang = txt_tenchu_tknganhang.Text.Trim();
            string _fullname = str_cl.VietHoa_ChuCai_DauTien(str_cl.Remove_Blank(txt_hoten.Text.Trim().ToLower()));
            string _ngaysinh = txt_ngaysinh.Text;
            string _sdt = txt_dienthoai.Text.Trim().Replace(" ", "");
            string _ngayvaolam = txt_ngayvaolam.Text;

            Int64 _luongcoban = Number_cl.Check_Int64(txt_luongcoban.Text.Trim());
            Int64 _phucap_xangxe = Number_cl.Check_Int64(txt_phucap_xangxe.Text.Trim());
            Int64 _phucap_anuong = Number_cl.Check_Int64(txt_phucap_anuong.Text.Trim());
            decimal _phucap_dienthoai = (decimal)Number_cl.Check_Int64(txt_phucap_dienthoai.Text.Trim());
            Int64 _phucap_trachnhiem = Number_cl.Check_Int64(txt_phucap_trachniem.Text.Trim());
            int _phantram_thuong_doanhso = Number_cl.Check_Int(txt_phantram_doanhso.Text.Trim());



            string _sdt_nguoithan = txt_sdt_nguoithan.Text.Trim().Replace(" ", "");
            string _ten_nguoithan = txt_tennguoithan.Text;

            bool _loai_nhanvien = true;
            if (rb_HocViec.Checked)
                _loai_nhanvien = false;


            #endregion
            using (dbDataContext db = new dbDataContext())
            {
                #region Kiểm tra ngoại lệ.

                if (_user == "")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập tài khoản.", "false", "false", "OK", "alert", ""), true);
                    return;
                }
                if (str_cl.check_taikhoan_hople(_user) == false)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Tài khoản phải có độ dài từ 5-30 ký tự không dấu hoặc chữ số và không chứa dấu cách. Vui lòng kiểm tra lại.", "false", "false", "OK", "alert", ""), true);
                    return;
                }

                if (_fullname == "")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập họ tên.", "false", "false", "OK", "alert", ""), true);
                    return;
                }
                if (_so_cccd == "")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập số CCCD.", "false", "false", "OK", "alert", ""), true);
                    return;
                }

                //if (_cccd_mattruoc == "")
                //{
                //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng chọn ảnh CCCD mặt trước.", "false", "false", "OK", "alert", ""), true);
                //    return;
                //}
                //if (_cccd_matsau == "")
                //{
                //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng chọn ảnh CCCD mặt sau.", "false", "false", "OK", "alert", ""), true);
                //    return;
                //}
                if (_phantram_thuong_doanhso < 0 || _phantram_thuong_doanhso > 100)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Phần trăm thưởng doanh số không hợp lệ.", "false", "false", "OK", "alert", ""), true);
                    return;
                }
                #endregion

                if (ViewState["add_edit"].ToString() == "add")
                {
                    if (_pass == "")
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập mật khẩu.", "false", "false", "OK", "alert", ""), true);
                        return;
                    }
                    var q_tk = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _user);
                    if (q_tk != null)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Tài khoản đã tồn tại. Vui lòng chọn tên khác.", "false", "false", "OK", "alert", ""), true);
                        return;
                    }
                    #region thêm mới


                    taikhoan_tb _ob = new taikhoan_tb();
                    _ob.taikhoan = _user; _ob.matkhau = _pass;
                    _ob.hoten = _fullname;
                    _ob.ngaysinh = string.IsNullOrEmpty(_ngaysinh) ? (DateTime?)null : DateTime.Parse(_ngaysinh);
                    _ob.ngayvaolam = string.IsNullOrEmpty(_ngayvaolam) ? (DateTime?)null : DateTime.Parse(_ngayvaolam);
                    _ob.ngaytao = DateTime.Now;
                    _ob.phanloai = "Quản trị";
                    _ob.ten = str_cl.tachten(_fullname);
                    _ob.hoten_khongdau = str_cl.remove_vietnamchar(_fullname);
                    _ob.dienthoai = _sdt;
                    if (_anhdaiien != "")
                        _ob.anhdaidien = _anhdaiien;
                    else
                        _ob.anhdaidien = "/uploads/images/macdinh.jpg";
                    _ob.permission = "";
                    _ob.makhoiphuc = "141191";
                    _ob.hsd_makhoiphuc = DateTime.Parse("01/01/1991");
                    _ob.block = false;
                    _ob.nguoitao = ViewState["taikhoan"].ToString();
                    _ob.trangthai_lamviec = "Đang làm việc";
                    _ob.cccd_mattruoc = null;
                    _ob.cccd_matsau = null;
                    _ob.so_cccd = _so_cccd;
                    _ob.tennganhang = _tennganhang;
                    _ob.so_tknganhang = _so_tknganhang;
                    _ob.tenchu_tknganhang = _tenchu_tknganhang;
                    _ob.loai_nhanvien = _loai_nhanvien;
                    _ob.LuongCoBan = _luongcoban;
                    _ob.PhuCap_AnUong = _phucap_anuong;
                    _ob.PhuCap_DienThoai = _phucap_dienthoai;
                    _ob.PhuCap_TrachNhiem = _phucap_trachnhiem;
                    _ob.PhuCap_Xangxe = _phucap_xangxe;
                    _ob.sdt_nguoithan = _sdt_nguoithan;
                    _ob.ten_nguoithan = _ten_nguoithan;
                    _ob.phantram_doanhso_banhang = _phantram_thuong_doanhso;

                    db.taikhoan_tbs.InsertOnSubmit(_ob);
                    db.SubmitChanges();
                    #endregion
                    #region cập nhật dữ liệu và update hiển thị

                    reset_control_add_edit();

                    show_main();
                    up_main.Update();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
                    #endregion
                }
                else//edit
                {
                    #region Chuẩn bị dữ liệu
                    var q_edit = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == ViewState["id_edit"].ToString());
                    if (q_edit != null)
                    {
                        #region Kiểm tra ngoại lệ. Sau đó cập nhật
                        taikhoan_tb _ob = q_edit;
                        _ob.hoten = _fullname;
                        _ob.ngaysinh = string.IsNullOrEmpty(_ngaysinh) ? (DateTime?)null : DateTime.Parse(_ngaysinh);
                        _ob.ngayvaolam = string.IsNullOrEmpty(_ngayvaolam) ? (DateTime?)null : DateTime.Parse(_ngayvaolam);
                        _ob.ten = str_cl.tachten(_fullname);
                        _ob.hoten_khongdau = str_cl.remove_vietnamchar(_fullname);
                        _ob.dienthoai = _sdt;
                        if (_anhdaiien != "")
                            _ob.anhdaidien = _anhdaiien;
                        else
                            _ob.anhdaidien = "/uploads/images/macdinh.jpg";
                        _ob.cccd_mattruoc = null;
                        _ob.cccd_matsau = null;
                        _ob.so_cccd = _so_cccd;
                        _ob.tennganhang = _tennganhang;
                        _ob.so_tknganhang = _so_tknganhang;
                        _ob.tenchu_tknganhang = _tenchu_tknganhang;
                        _ob.loai_nhanvien = _loai_nhanvien;
                        _ob.LuongCoBan = _luongcoban;
                        _ob.PhuCap_AnUong = _phucap_anuong;
                        _ob.PhuCap_DienThoai = _phucap_dienthoai;
                        _ob.PhuCap_TrachNhiem = _phucap_trachnhiem;
                        _ob.PhuCap_Xangxe = _phucap_xangxe;
                        _ob.sdt_nguoithan = _sdt_nguoithan;
                        _ob.ten_nguoithan = _ten_nguoithan;
                        _ob.phantram_doanhso_banhang = _phantram_thuong_doanhso;
                        db.SubmitChanges();
                        
                        if (_user != ViewState["id_edit"].ToString())
                        {
                            if (taikhoan_cl.exist_taikhoan(_user))
                            {
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Tên tài khoản mới đã tồn tại, các thông tin khác đã được lưu.", "5000", "warning"), true);
                            }
                            else
                            {
                                db.ExecuteCommand("UPDATE taikhoan_tb SET taikhoan={0} WHERE taikhoan={1}", _user, ViewState["id_edit"].ToString());
                            }
                        }
                        #region cập nhật dữ liệu và update hiển thị

                        show_main();
                        up_main.Update();

                        //reset control
                        reset_control_add_edit();
                        //ẩn form
                        pn_add.Visible = !pn_add.Visible;

                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
                        #endregion

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
                var list_all = (from ob1 in db.taikhoan_tbs
                                    //join ob2 in db.DanhMuc_tbs on ob1.id_DanhMuc equals ob2.id.ToString() into danhMucGroup
                                    //from ob2 in danhMucGroup.DefaultIfEmpty()
                                select new
                                {
                                    ob1.taikhoan,
                                    ob1.anhdaidien,
                                    ob1.hoten,
                                    ob1.hoten_khongdau,
                                    ob1.ngaysinh,
                                    ob1.email,
                                    ob1.dienthoai,
                                    ob1.trangthai_lamviec,
                                    ob1.ngayvaolam,
                                    ob1.cccd_mattruoc,
                                    ob1.cccd_matsau,
                                    ob1.LuongCoBan,
                                    ob1.sdt_nguoithan,
                                    ob1.ten_nguoithan,
                                }).AsQueryable();




                // Kiểm tra xem textbox có dữ liệu tìm kiếm không
                string _key = txt_timkiem.Text.Trim();
                if (!string.IsNullOrEmpty(_key))
                    list_all = list_all.Where(p => p.hoten.Contains(_key) || p.hoten_khongdau.Contains(_key) || p.taikhoan == _key || p.dienthoai == _key);
                else
                {
                    string _key1 = txt_timkiem1.Text.Trim();
                    if (!string.IsNullOrEmpty(_key1))
                        list_all = list_all.Where(p => p.hoten.Contains(_key) || p.hoten_khongdau.Contains(_key) || p.taikhoan == _key || p.dienthoai == _key);
                }

                //xử lý theo thời gian
                string _id_locthoigian = ddl_thoigian.SelectedValue;
                if (_id_locthoigian == "1")//lọc theo ngày vào làm
                {
                    if (txt_tungay.Text != "")
                        list_all = list_all.Where(p => p.ngayvaolam.Value.Date >= DateTime.Parse(txt_tungay.Text).Date);
                    if (txt_denngay.Text != "")
                        list_all = list_all.Where(p => p.ngayvaolam.Value.Date <= DateTime.Parse(txt_denngay.Text).Date);
                }


                //sắp xếp
                list_all = list_all.OrderByDescending(p => p.ngayvaolam);
                int _Tong_Record = list_all.Count();
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
                                case "taikhoan":
                                    worksheet.Cells[_row, _cot].Value = t.taikhoan; _cot = _cot + 1;
                                    break;
                                case "hoten":
                                    worksheet.Cells[_row, _cot].Value = t.hoten; _cot = _cot + 1;
                                    break;
                                //case "ngaytao":
                                //    // Giả định t.ngaytao là thuộc tính DateTime hoặc DateTime?
                                //    DateTime? ngayTao = t.ngaytao;

                                //    if (ngayTao.HasValue)
                                //    {
                                //        // Chuyển đổi DateTime thành chỉ ngày (ngayTao.Value.Date)
                                //        DateTime onlyDate = ngayTao.Value.Date;

                                //        // Đặt giá trị ô là kiểu DateTime chỉ với ngày
                                //        worksheet.Cells[_row, _cot].Value = onlyDate;

                                //        // Định dạng số cho ô thành "dd/MM/yyyy"
                                //        worksheet.Cells[_row, _cot].Style.Numberformat.Format = "dd/MM/yyyy";
                                //    }
                                //    else
                                //    {
                                //        // Nếu giá trị ngayTao là null, bạn có thể để trống ô đó hoặc xử lý theo cách khác
                                //        worksheet.Cells[_row, _cot].Value = DBNull.Value; // Hoặc để trống, hoặc đặt giá trị mặc định
                                //    }
                                //    _cot = _cot + 1;
                                //    break;
                                default: break;
                            }
                        }
                        _row++;
                    }
                    // Lưu tệp Excel vào đường dẫn đã chỉ định
                    string filePath = "/uploads/files/Bai-Viet-" + str_cl.taoma_theothoigian() + ".xlsx";
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
            if (Request.Cookies["cookie_qlnv"] != null)//nếu có ck r thì lưu giá trị mới
            {
                HttpCookie _ck = Request.Cookies["cookie_qlnv"];
                _ck["show"] = txt_show.Text;
                _ck["trang_hientai"] = ViewState["current_page_qlnv"].ToString();
                _ck["id_loctheothoigian"] = ddl_thoigian.SelectedValue;
                _ck["tungay"] = txt_tungay.Text;
                _ck["denngay"] = txt_denngay.Text;
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
            if (Request.Cookies["cookie_qlnv"] != null)
                Response.Cookies["cookie_qlnv"].Expires = DateTime.Now.AddYears(-1);
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

    #region xóa ảnh cccd
    protected void Button1_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == ViewState["id_edit"].ToString());
            if (q != null)
            {
                taikhoan_tb _ob = q;
                File_Folder_cl.del_file(_ob.cccd_mattruoc);//xóa ảnh cũ nếu có
                _ob.cccd_mattruoc = "";
                Button1.Visible = false;
                db.SubmitChanges();
                Label3.Text = "";
                txt_link_fileupload1.Text = "";
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
            }
        }
    }

    protected void Button3_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == ViewState["id_edit"].ToString());
            if (q != null)
            {
                taikhoan_tb _ob = q;
                File_Folder_cl.del_file(_ob.cccd_matsau);//xóa ảnh cũ nếu có
                _ob.cccd_matsau = "";
                Button3.Visible = false;
                db.SubmitChanges();
                Label4.Text = "";
                txt_link_fileupload2.Text = "";
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
            }
        }
    }
    #endregion

    #region phân quyền
    public void reset_control_phanquyen()
    {
        check_all_quyen_quanlynhanvien.Checked = false;
        check_list_quyen_quanlynhanvien.SelectedIndex = -1;
        check_all_quyen_quanlyhethong.Checked = false;
        check_list_quyen_quanlyhethong.SelectedIndex = -1;

        check_all_quyen_quanlykho.Checked = false;
        check_list_quyen_quanlykho.SelectedIndex = -1;
        check_all_quyen_quanlybaogia.Checked = false;
        check_list_quyen_quanlybaogia.SelectedIndex = -1;
        check_all_quyen_datakhachhang.Checked = false;
        check_list_quyen_datakhachhang.SelectedIndex = -1;
        check_all_quyen_quanlyhopdong.Checked = false;
        check_list_quyen_quanlyhopdong.SelectedIndex = -1;
        check_all_quyen_congviec.Checked = false;
        check_list_quyen_congviec.SelectedIndex = -1;
        check_all_quyen_baohanh.Checked = false;
        check_list_quyen_baohanh.SelectedIndex = -1;
        check_all_quyen_theodoihangdaban.Checked = false;
        check_list_quyen_theodoihangdaban.SelectedIndex = -1;

        ViewState["tk_phanquyen"] = null;
    }
    protected void but_close_form_phanquyen_Click(object sender, EventArgs e)
    {
        reset_control_phanquyen();
        pn_phanquyen.Visible = !pn_phanquyen.Visible;
        up_phanquyen.Update();
    }
    protected void but_show_form_phanquyen_Click(object sender, EventArgs e)
    {
        reset_control_phanquyen();
        check_login_cl.check_login_admin("5", "5");
        LinkButton button = (LinkButton)sender;
        string _tk = button.CommandArgument;
        ViewState["tk_phanquyen"] = _tk;
        using (dbDataContext db = new dbDataContext())
        {
            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _tk);
            string _quyen = q.permission; //có dạng 1,2,3,4,5
            if (!string.IsNullOrEmpty(_quyen))
            {
                // Tách chuỗi quyền thành mảng
                var quyenArray = _quyen.Split(',');

                #region quản lý nhân viên
                // Lặp qua các `ListItem` trong `CheckBoxList`
                foreach (ListItem item in check_list_quyen_quanlynhanvien.Items)
                {
                    // Nếu quyền tồn tại trong mảng, chọn ListItem đó
                    item.Selected = quyenArray.Contains(item.Value);
                }
                // Kiểm tra xem tất cả các quyền có được chọn không
                check_all_quyen_quanlynhanvien.Checked = check_list_quyen_quanlynhanvien.Items.Cast<ListItem>().All(i => i.Selected);
                #endregion
                #region quản lý hệ thống
                // Lặp qua các `ListItem` trong `CheckBoxList`
                foreach (ListItem item in check_list_quyen_quanlyhethong.Items)
                {
                    // Nếu quyền tồn tại trong mảng, chọn ListItem đó
                    item.Selected = quyenArray.Contains(item.Value);
                }
                // Kiểm tra xem tất cả các quyền có được chọn không
                check_all_quyen_quanlyhethong.Checked = check_list_quyen_quanlyhethong.Items.Cast<ListItem>().All(i => i.Selected);
                #endregion
                #region quản lý kho
                foreach (ListItem item in check_list_quyen_quanlykho.Items)
                {
                    // Nếu quyền tồn tại trong mảng, chọn ListItem đó
                    item.Selected = quyenArray.Contains(item.Value);
                }
                // Kiểm tra xem tất cả các quyền có được chọn không
                check_all_quyen_quanlykho.Checked = check_list_quyen_quanlykho.Items.Cast<ListItem>().All(i => i.Selected);
                #endregion
                #region quản lý báo giá
                foreach (ListItem item in check_list_quyen_quanlybaogia.Items)
                {
                    // Nếu quyền tồn tại trong mảng, chọn ListItem đó
                    item.Selected = quyenArray.Contains(item.Value);
                }
                // Kiểm tra xem tất cả các quyền có được chọn không
                check_all_quyen_quanlybaogia.Checked = check_list_quyen_quanlybaogia.Items.Cast<ListItem>().All(i => i.Selected);
                #endregion
                #region quản lý data khách hàng
                foreach (ListItem item in check_list_quyen_datakhachhang.Items)
                {
                    // Nếu quyền tồn tại trong mảng, chọn ListItem đó
                    item.Selected = quyenArray.Contains(item.Value);
                }
                // Kiểm tra xem tất cả các quyền có được chọn không
                check_all_quyen_datakhachhang.Checked = check_list_quyen_datakhachhang.Items.Cast<ListItem>().All(i => i.Selected);
                #endregion
                #region quản lý hợp đồng
                foreach (ListItem item in check_list_quyen_quanlyhopdong.Items)
                {
                    // Nếu quyền tồn tại trong mảng, chọn ListItem đó
                    item.Selected = quyenArray.Contains(item.Value);
                }
                // Kiểm tra xem tất cả các quyền có được chọn không
                check_all_quyen_quanlyhopdong.Checked = check_list_quyen_quanlyhopdong.Items.Cast<ListItem>().All(i => i.Selected);
                #endregion
                #region công việc
                foreach (ListItem item in check_list_quyen_congviec.Items)
                {
                    // Nếu quyền tồn tại trong mảng, chọn ListItem đó
                    item.Selected = quyenArray.Contains(item.Value);
                }
                // Kiểm tra xem tất cả các quyền có được chọn không
                check_all_quyen_congviec.Checked = check_list_quyen_congviec.Items.Cast<ListItem>().All(i => i.Selected);
                #endregion
                #region bảo hành
                foreach (ListItem item in check_list_quyen_baohanh.Items)
                {
                    // Nếu quyền tồn tại trong mảng, chọn ListItem đó
                    item.Selected = quyenArray.Contains(item.Value);
                }
                // Kiểm tra xem tất cả các quyền có được chọn không
                check_all_quyen_baohanh.Checked = check_list_quyen_baohanh.Items.Cast<ListItem>().All(i => i.Selected);
                #endregion
                #region theo dõi hàng đã bán
                foreach (ListItem item in check_list_quyen_theodoihangdaban.Items)
                {
                    item.Selected = quyenArray.Contains(item.Value);
                }
                check_all_quyen_theodoihangdaban.Checked = check_list_quyen_theodoihangdaban.Items.Cast<ListItem>().All(i => i.Selected);
                #endregion
            }
        }
        pn_phanquyen.Visible = !pn_phanquyen.Visible;
        up_phanquyen.Update();
    }
    protected void but_phanquyen_Click(object sender, EventArgs e)
    {
        string _tk = ViewState["tk_phanquyen"].ToString();
        using (dbDataContext db = new dbDataContext())
        {
            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _tk);
            if (q != null)
            {
                // Tạo chuỗi quyền QUẢN LÝ NHÂN VIÊN
                string _quyen_quanlynhanvien = string.Join(",",
                    check_list_quyen_quanlynhanvien.Items.Cast<ListItem>()
                    .Where(item => item.Selected)
                    .Select(item => item.Value)
                    .ToArray());

                // Tạo chuỗi quyền QUẢN LÝ HỆ THỐNG
                string _quyen_quanlyhethong = string.Join(",",
                    check_list_quyen_quanlyhethong.Items.Cast<ListItem>()
                    .Where(item => item.Selected)
                    .Select(item => item.Value)
                    .ToArray());
                // Tạo chuỗi quyền QUẢN LÝ KHO
                string _quyen_quanlykho = string.Join(",",
                    check_list_quyen_quanlykho.Items.Cast<ListItem>()
                    .Where(item => item.Selected)
                    .Select(item => item.Value)
                    .ToArray());
                // Tạo chuỗi quyền QUẢN LÝ BÁO GIÁ
                string _quyen_quanlybaogia = string.Join(",",
                    check_list_quyen_quanlybaogia.Items.Cast<ListItem>()
                    .Where(item => item.Selected)
                    .Select(item => item.Value)
                    .ToArray());
                // Tạo chuỗi quyền QUẢN LÝ DATA KHÁCH HÀNG
                string _quyen_quanlydatakhachhang = string.Join(",",
                    check_list_quyen_datakhachhang.Items.Cast<ListItem>()
                    .Where(item => item.Selected)
                    .Select(item => item.Value)
                    .ToArray());
                // Tạo chuỗi quyền QUẢN LÝ HỢP ĐỒNG
                string _quyen_quanlyhopdong = string.Join(",",
                    check_list_quyen_quanlyhopdong.Items.Cast<ListItem>()
                    .Where(item => item.Selected)
                    .Select(item => item.Value)
                    .ToArray());
                // Tạo chuỗi quyền CÔNG VIỆC
                string _quyen_congviec = string.Join(",",
                    check_list_quyen_congviec.Items.Cast<ListItem>()
                    .Where(item => item.Selected)
                    .Select(item => item.Value)
                    .ToArray());
                // Tạo chuỗi quyền CÔNG VIỆC
                string _quyen_baohanh = string.Join(",",
                    check_list_quyen_baohanh.Items.Cast<ListItem>()
                    .Where(item => item.Selected)
                    .Select(item => item.Value)
                    .ToArray());
                // Tạo chuỗi quyền THEO DÕI HÀNG ĐÃ BÁN
                string _quyen_theodoihangdaban = string.Join(",",
                    check_list_quyen_theodoihangdaban.Items.Cast<ListItem>()
                    .Where(item => item.Selected)
                    .Select(item => item.Value)
                    .ToArray());

                // Nối tất cả các quyền lại với nhau
                string all_quyen = string.Join(",", _quyen_quanlynhanvien, _quyen_quanlyhethong, _quyen_quanlykho, _quyen_quanlybaogia, _quyen_quanlydatakhachhang, _quyen_quanlyhopdong, _quyen_congviec, _quyen_baohanh, _quyen_theodoihangdaban);

                q.permission = all_quyen;

                db.SubmitChanges();

                reset_control_phanquyen();
                show_main();
                up_main.Update();
                pn_phanquyen.Visible = !pn_phanquyen.Visible;
                up_phanquyen.Update();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
            }
        }
    }

    #region quản lý nhân viên
    protected void check_all_quyen_quanlynhanvien_CheckedChanged(object sender, EventArgs e)
    {
        // Kiểm tra trạng thái của checkbox "Chọn tất cả"
        bool isChecked = check_all_quyen_quanlynhanvien.Checked;

        // Đặt trạng thái của tất cả các mục trong CheckBoxList theo trạng thái của "Chọn tất cả"
        foreach (ListItem item in check_list_quyen_quanlynhanvien.Items)
        {
            item.Selected = isChecked;
        }
    }

    protected void check_list_quyen_quanlynhanvien_SelectedIndexChanged(object sender, EventArgs e)
    {
        bool allSelected = true;

        foreach (ListItem item in check_list_quyen_quanlynhanvien.Items)
        {
            if (!item.Selected)
            {
                allSelected = false;
                break;
            }
        }

        // Cập nhật trạng thái của check_all_page theo kết quả kiểm tra
        check_all_quyen_quanlynhanvien.Checked = allSelected;
    }
    #endregion
    #region quản lý hệ thống
    protected void check_all_quyen_quanlyhethong_CheckedChanged(object sender, EventArgs e)
    {
        // Kiểm tra trạng thái của checkbox "Chọn tất cả"
        bool isChecked = check_all_quyen_quanlyhethong.Checked;

        // Đặt trạng thái của tất cả các mục trong CheckBoxList theo trạng thái của "Chọn tất cả"
        foreach (ListItem item in check_list_quyen_quanlyhethong.Items)
        {
            item.Selected = isChecked;
        }
    }

    protected void check_list_quyen_quanlyhethong_SelectedIndexChanged(object sender, EventArgs e)
    {
        bool allSelected = true;

        foreach (ListItem item in check_list_quyen_quanlyhethong.Items)
        {
            if (!item.Selected)
            {
                allSelected = false;
                break;
            }
        }

        // Cập nhật trạng thái của check_all_page theo kết quả kiểm tra
        check_all_quyen_quanlyhethong.Checked = allSelected;
    }
    #endregion
    #region quản lý kho
    protected void check_all_quyen_quanlykho_CheckedChanged(object sender, EventArgs e)
    {
        // Kiểm tra trạng thái của checkbox "Chọn tất cả"
        bool isChecked = check_all_quyen_quanlykho.Checked;

        // Đặt trạng thái của tất cả các mục trong CheckBoxList theo trạng thái của "Chọn tất cả"
        foreach (ListItem item in check_list_quyen_quanlykho.Items)
        {
            item.Selected = isChecked;
        }
    }

    protected void check_list_quyen_quanlykho_SelectedIndexChanged(object sender, EventArgs e)
    {
        bool allSelected = true;

        foreach (ListItem item in check_list_quyen_quanlykho.Items)
        {
            if (!item.Selected)
            {
                allSelected = false;
                break;
            }
        }

        // Cập nhật trạng thái của check_all_page theo kết quả kiểm tra
        check_all_quyen_quanlykho.Checked = allSelected;

    }
    #endregion
    #region quản lý báo giá
    protected void check_all_quyen_quanlybaogia_CheckedChanged(object sender, EventArgs e)
    {
        // Kiểm tra trạng thái của checkbox "Chọn tất cả"
        bool isChecked = check_all_quyen_quanlybaogia.Checked;

        // Đặt trạng thái của tất cả các mục trong CheckBoxList theo trạng thái của "Chọn tất cả"
        foreach (ListItem item in check_list_quyen_quanlybaogia.Items)
        {
            item.Selected = isChecked;
        }
    }

    protected void check_list_quyen_quanlybaogia_SelectedIndexChanged(object sender, EventArgs e)
    {
        bool allSelected = true;

        foreach (ListItem item in check_list_quyen_quanlybaogia.Items)
        {
            if (!item.Selected)
            {
                allSelected = false;
                break;
            }
        }

        // Cập nhật trạng thái của check_all_page theo kết quả kiểm tra
        check_all_quyen_quanlybaogia.Checked = allSelected;
    }
    #endregion
    #region quản lý data khách hàng
    protected void check_all_quyen_datakhachhang_CheckedChanged(object sender, EventArgs e)
    {
        // Kiểm tra trạng thái của checkbox "Chọn tất cả"
        bool isChecked = check_all_quyen_datakhachhang.Checked;

        // Đặt trạng thái của tất cả các mục trong CheckBoxList theo trạng thái của "Chọn tất cả"
        foreach (ListItem item in check_list_quyen_datakhachhang.Items)
        {
            item.Selected = isChecked;
        }
    }

    protected void check_list_quyen_datakhachhang_SelectedIndexChanged(object sender, EventArgs e)
    {
        bool allSelected = true;

        foreach (ListItem item in check_list_quyen_datakhachhang.Items)
        {
            if (!item.Selected)
            {
                allSelected = false;
                break;
            }
        }

        // Cập nhật trạng thái của check_all_page theo kết quả kiểm tra
        check_all_quyen_datakhachhang.Checked = allSelected;
    }
    #endregion
    #region quản lý hợp đồng
    protected void check_all_quyen_quanlyhopdong_CheckedChanged(object sender, EventArgs e)
    {
        // Kiểm tra trạng thái của checkbox "Chọn tất cả"
        bool isChecked = check_all_quyen_quanlyhopdong.Checked;

        // Đặt trạng thái của tất cả các mục trong CheckBoxList theo trạng thái của "Chọn tất cả"
        foreach (ListItem item in check_list_quyen_quanlyhopdong.Items)
        {
            item.Selected = isChecked;
        }
    }

    protected void check_list_quyen_quanlyhopdong_SelectedIndexChanged(object sender, EventArgs e)
    {
        bool allSelected = true;

        foreach (ListItem item in check_list_quyen_quanlyhopdong.Items)
        {
            if (!item.Selected)
            {
                allSelected = false;
                break;
            }
        }

        // Cập nhật trạng thái của check_all_page theo kết quả kiểm tra
        check_all_quyen_quanlyhopdong.Checked = allSelected;
    }
    #endregion
    #region quản lý công việc
    protected void check_all_quyen_congviec_CheckedChanged(object sender, EventArgs e)
    {
        // Kiểm tra trạng thái của checkbox "Chọn tất cả"
        bool isChecked = check_all_quyen_congviec.Checked;

        // Đặt trạng thái của tất cả các mục trong CheckBoxList theo trạng thái của "Chọn tất cả"
        foreach (ListItem item in check_list_quyen_congviec.Items)
        {
            item.Selected = isChecked;
        }
    }

    protected void check_list_quyen_congviec_SelectedIndexChanged(object sender, EventArgs e)
    {
        bool allSelected = true;

        foreach (ListItem item in check_list_quyen_congviec.Items)
        {
            if (!item.Selected)
            {
                allSelected = false;
                break;
            }
        }

        // Cập nhật trạng thái của check_all_page theo kết quả kiểm tra
        check_all_quyen_congviec.Checked = allSelected;
    }
    #endregion
    #region quản lý bảo hành
    protected void check_all_quyen_baohanh_CheckedChanged(object sender, EventArgs e)
    {
        // Kiểm tra trạng thái của checkbox "Chọn tất cả"
        bool isChecked = check_all_quyen_baohanh.Checked;

        // Đặt trạng thái của tất cả các mục trong CheckBoxList theo trạng thái của "Chọn tất cả"
        foreach (ListItem item in check_list_quyen_baohanh.Items)
        {
            item.Selected = isChecked;
        }
    }

    protected void check_list_quyen_baohanh_SelectedIndexChanged(object sender, EventArgs e)
    {
        bool allSelected = true;

        foreach (ListItem item in check_list_quyen_baohanh.Items)
        {
            if (!item.Selected)
            {
                allSelected = false;
                break;
            }
        }

        // Cập nhật trạng thái của check_all_page theo kết quả kiểm tra
        check_all_quyen_baohanh.Checked = allSelected;
    }
    #endregion
    #region quản lý theo dõi hàng đã bán
    protected void check_all_quyen_theodoihangdaban_CheckedChanged(object sender, EventArgs e)
    {
        bool isChecked = check_all_quyen_theodoihangdaban.Checked;
        foreach (ListItem item in check_list_quyen_theodoihangdaban.Items)
        {
            item.Selected = isChecked;
        }
    }

    protected void check_list_quyen_theodoihangdaban_SelectedIndexChanged(object sender, EventArgs e)
    {
        bool allSelected = true;
        foreach (ListItem item in check_list_quyen_theodoihangdaban.Items)
        {
            if (!item.Selected)
            {
                allSelected = false;
                break;
            }
        }
        check_all_quyen_theodoihangdaban.Checked = allSelected;
    }
    #endregion
    #endregion


    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_admin("4", "4");
        using (dbDataContext db = new dbDataContext())
        {
            LinkButton button = (LinkButton)sender;
            string _tk = button.CommandArgument;
            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan.ToString() == _tk);
            if(q!=null)
            {
                q.block = true;
                q.trangthai_lamviec = "Đã nghỉ việc";
                db.SubmitChanges();
                show_main();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
            }    
        }
    }

    protected void LinkButton2_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_admin("4", "4");
        using (dbDataContext db = new dbDataContext())
        {
            LinkButton button = (LinkButton)sender;
            string _tk = button.CommandArgument;
            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan.ToString() == _tk);
            if (q != null)
            {
                q.block = false;
                q.trangthai_lamviec = "Đang làm việc";
                db.SubmitChanges();
                show_main();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
            }
        }
    }
}