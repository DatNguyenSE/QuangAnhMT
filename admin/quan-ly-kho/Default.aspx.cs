using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_quan_ly_kho_Default : System.Web.UI.Page
{

    // ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Tài khoản đã bị khóa.", "false", "false", "OK", "alert", ""), true);
    String_cl str_cl = new String_cl();
    DateTime_cl dt_cl = new DateTime_cl();

    public void set_dulieu_macdinh()
    {
        try
        {
            ResetButtonCss();//button chọn ngày nhanh
            txt_show.Text = "30";
            ViewState["current_page_qlkho"] = "1";


            #region set_get_cookie
            // Lấy cookie "cookie_qlkho" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_qlkho"];
            if (cookie == null)
            {
                ListBox1.SelectedIndex = 0;//mặc định chọn tất cả phân loại, nếu select=true ngoài html thì k lưu lịch sử đc, kệ mẹ nó cứ làm y vậy đi, đừng quan tâm tới đoạn này
                                           // Nếu cookie không tồn tại, tạo cookie mới
                cookie = new HttpCookie("cookie_qlkho");
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
                ViewState["current_page_qlkho"] = cookie["trang_hientai"];
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
            check_login_cl.check_login_admin("7", "7");

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
        // Lấy quyền người dùng từ một nơi nào đó, ví dụ: Session, ViewState hoặc từ cơ sở dữ liệu
        string userPermissions = ViewState["quyen"].ToString();
        var permissionsList = userPermissions.Split(',');
        // Kiểm tra nếu quyền của người dùng có quyền 3
        if (permissionsList.Contains("8"))
        {
            // Nếu người dùng có quyền 2, hiển thị giá trị 'LuongCoBan'
            PlaceHolder PlaceHolder5 = (PlaceHolder)e.Item.FindControl("PlaceHolder5");
            if (PlaceHolder5 != null)
            {
                PlaceHolder5.Visible = true;
            }
        }
        else
        {
            // Nếu không có quyền 2, ẩn giá trị 'LuongCoBan'
            PlaceHolder PlaceHolder5 = (PlaceHolder)e.Item.FindControl("PlaceHolder5");
            if (PlaceHolder5 != null)
            {
                PlaceHolder5.Visible = false;
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
                    PlaceHolder1.Visible = true;
                    PlaceHolder3.Visible = true;
                    PlaceHolder6.Visible = true;
                    PlaceHolder4.Visible = true;
                }
                else
                {
                    PlaceHolder1.Visible = false;
                    PlaceHolder3.Visible = false;
                    PlaceHolder6.Visible = false;
                    PlaceHolder4.Visible = false;
                }
                #endregion

                #region lấy dữ liệu
                var list_all = (from ob1 in db.KhoSanPham_tbs
                                join ob2 in db.DuLieuNguon_tbs
                                    .Where(p => p.kyhieu == "hangsanpham") on ob1.id_hang equals ob2.id.ToString() into HangGroup
                                from ob2 in HangGroup.DefaultIfEmpty()
                                join ob3 in db.DuLieuNguon_tbs
                                    .Where(p => p.kyhieu == "nhomsanpham") on ob1.id_nhom equals ob3.id.ToString() into NhomGroup
                                from ob3 in NhomGroup.DefaultIfEmpty()
                                join ob4 in db.DuLieuNguon_tbs
                                    .Where(p => p.kyhieu == "donvitinh") on ob1.donvitinh equals ob4.id.ToString() into DVTGroup
                                from ob4 in DVTGroup.DefaultIfEmpty()
                                select new
                                {
                                    ob1.id,
                                    TenSP = ob1.ten,
                                    Hang = ob2 == null ? "" : ob2.ten,
                                    Nhom = ob3 == null ? "" : ob3.ten,
                                    DVT = ob4 == null ? "" : ob4.ten,
                                    ob1.anh,
                                    ob1.model,
                                    ob1.thongso_kythuat,
                                    ob1.gianhap,
                                    TongGiaNhap = ob1.gianhap * ob1.soluong_hientai,
                                    ob1.giabanle,
                                    TongBanLe = ob1.giabanle * ob1.soluong_hientai,
                                    ob1.cohoadon,
                                    ob1.hangthanhly,
                                    ob1.soluong_hientai,
                                    ob1.ghichu,
                                    ob1.ngaytao,
                                    ob1.nguoitao,
                                }).AsQueryable();




                // Kiểm tra xem textbox có dữ liệu tìm kiếm không
                string _key = txt_timkiem.Text.Trim();
                if (!string.IsNullOrEmpty(_key))
                    list_all = list_all.Where(p => p.TenSP.Contains(_key) || p.id.ToString() == _key);
                else
                {
                    string _key1 = txt_timkiem1.Text.Trim();
                    if (!string.IsNullOrEmpty(_key1))
                        list_all = list_all.Where(p => p.TenSP.Contains(_key1) || p.Hang.Contains(_key1) || p.Nhom.Contains(_key1) || p.model == _key1 || p.id.ToString() == _key1);
                }

                ////xử lý theo thời gian
                //string _id_locthoigian = ddl_thoigian.SelectedValue;
                //if (_id_locthoigian == "1")//lọc theo ngày tạo
                //{
                //    if (txt_tungay.Text != "")
                //        list_all = list_all.Where(p => p.ngaytao.Value.Date >= DateTime.Parse(txt_tungay.Text).Date);
                //    if (txt_denngay.Text != "")
                //        list_all = list_all.Where(p => p.ngaytao.Value.Date <= DateTime.Parse(txt_denngay.Text).Date);
                //}

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
                list_all = list_all.OrderBy(p => p.Nhom).ThenBy(p => p.TenSP);
                int _Tong_Record = list_all.Count();

                Int64 _tongbanle = list_all.Sum(p => p.TongBanLe.Value), _tonggianhap = list_all.Sum(p => p.TongGiaNhap.Value);
                ViewState["tong_ton"] = list_all.Sum(p => p.soluong_hientai.Value).ToString("#,##0");
                ViewState["tong_giale"] = _tongbanle.ToString("#,##0");
                ViewState["tong_gianhap"] = _tonggianhap.ToString("#,##0");
                ViewState["tong_laigop"] = (_tongbanle - _tonggianhap).ToString("#,##0");
                #endregion

                #region phân trang OK, k sửa
                // Xử lý số record mỗi trang
                int show = Number_cl.Check_Int(txt_show.Text.Trim()); if (show <= 0) show = 30;
                //xử lý trang hiện tại. Đảm bảo current_page không nhỏ hơn 1 và không lớn hơn total_page
                int current_page = int.Parse(ViewState["current_page_qlkho"].ToString()); int total_page = number_of_page_class.return_total_page(_Tong_Record, show); if (current_page < 1) current_page = 1; else if (current_page > total_page) current_page = total_page;
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
            ViewState["current_page_qlkho"] = int.Parse(ViewState["current_page_qlkho"].ToString()) - 1;
            #region LƯU TRANG HIỆN TẠI
            // Lấy cookie "cookie_qlkho" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_qlkho"];
            if (cookie != null)
            {
                cookie["trang_hientai"] = ViewState["current_page_qlkho"].ToString();
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
            ViewState["current_page_qlkho"] = int.Parse(ViewState["current_page_qlkho"].ToString()) + 1;
            #region LƯU TRANG HIỆN TẠI
            // Lấy cookie "cookie_qlkho" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_qlkho"];
            if (cookie != null)
            {
                cookie["trang_hientai"] = ViewState["current_page_qlkho"].ToString();
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
            ViewState["current_page_qlkho"] = 1;
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
            txt_name.Text = ""; txt_model.Text = ""; txt_thongso.Text = ""; txt_ghichu.Text = "";
            txt_giaban.Text = "0";
            txt_gianhap.Text = "0";
            Label2.Text = ""; Button2.Visible = false; txt_link_fileupload.Text = "";
            DropDownList1.DataSource = null;
            DropDownList1.DataBind();
            DropDownList2.DataSource = null;
            DropDownList2.DataBind();
            DropDownList3.DataSource = null;
            DropDownList3.DataBind();
            ViewState["add_edit"] = null;
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
            check_login_cl.check_login_admin("9", "9");
            //reset control
            reset_control_add_edit();

            ViewState["add_edit"] = "add";
            Label1.Text = "THÊM SẢN PHẨM MỚI";
            but_add_edit.Text = "THÊM MỚI";

            using (dbDataContext db = new dbDataContext())
            {
                var data = db.DuLieuNguon_tbs
             .Where(p => p.kyhieu == "hangsanpham" || p.kyhieu == "nhomsanpham" || p.kyhieu == "donvitinh")
             .ToList();

                var hangSanPham = data.Where(p => p.kyhieu == "hangsanpham").OrderBy(p => p.ten).ToList();
                var nhomSanPham = data.Where(p => p.kyhieu == "nhomsanpham").OrderBy(p => p.ten).ToList();
                var donvitinh = data.Where(p => p.kyhieu == "donvitinh").OrderBy(p => p.ten).ToList();

                DropDownList1.DataSource = hangSanPham;
                DropDownList1.DataValueField = "id";
                DropDownList1.DataTextField = "ten";
                DropDownList1.DataBind();
                DropDownList1.Items.Insert(0, new ListItem("Chọn hãng", ""));
                DropDownList2.DataSource = nhomSanPham;
                DropDownList2.DataValueField = "id";
                DropDownList2.DataTextField = "ten";
                DropDownList2.DataBind();
                DropDownList2.Items.Insert(0, new ListItem("Chọn nhóm", ""));
                DropDownList3.DataSource = donvitinh;
                DropDownList3.DataValueField = "id";
                DropDownList3.DataTextField = "ten";
                DropDownList3.DataBind();
                DropDownList3.Items.Insert(0, new ListItem("Chọn đơn vị tính", ""));

            }
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
            var q = db.KhoSanPham_tbs.FirstOrDefault(p => p.id.ToString() == ViewState["id_edit"].ToString());
            if (q != null)
            {
                KhoSanPham_tb _ob = q;
                File_Folder_cl.del_file(_ob.anh);//xóa ảnh cũ nếu có
                _ob.anh = "";
                Button2.Visible = false;
                db.SubmitChanges();
                Label2.Text = ""; txt_link_fileupload.Text = "";
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa ảnh thành công.", "1000", "warning"), true);
            }
        }

    }
    protected void but_show_chinhsua_Click(object sender, EventArgs e)
    {

        check_login_cl.check_login_admin("10", "10");
        ViewState["add_edit"] = "edit";
        Label1.Text = "CHỈNH SỬA SẢN PHẨM";
        but_add_edit.Text = "CẬP NHẬT";
        using (dbDataContext db = new dbDataContext())
        {

            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;

            //truy vấn dữ liệu để sửa
            var q = db.KhoSanPham_tbs.FirstOrDefault(p => p.id.ToString() == _id);
            if (q != null)
            {
                ViewState["id_edit"] = _id;

                var data = db.DuLieuNguon_tbs
         .Where(p => p.kyhieu == "hangsanpham" || p.kyhieu == "nhomsanpham" || p.kyhieu == "donvitinh")
         .ToList();

                var hangSanPham = data.Where(p => p.kyhieu == "hangsanpham").OrderBy(p => p.ten).ToList();
                var nhomSanPham = data.Where(p => p.kyhieu == "nhomsanpham").OrderBy(p => p.ten).ToList();
                var donvitinh = data.Where(p => p.kyhieu == "donvitinh").OrderBy(p => p.ten).ToList();

                DropDownList1.DataSource = hangSanPham;
                DropDownList1.DataValueField = "id";
                DropDownList1.DataTextField = "ten";
                DropDownList1.DataBind();
                DropDownList1.Items.Insert(0, new ListItem("Chọn hãng", ""));
                // Kiểm tra nếu q.id_hang tồn tại trong danh sách
                if (hangSanPham.Any(p => p.id.ToString() == q.id_hang))
                    DropDownList1.SelectedValue = q.id_hang;
                else
                    DropDownList1.SelectedIndex = 0;

                DropDownList2.DataSource = nhomSanPham;
                DropDownList2.DataValueField = "id";
                DropDownList2.DataTextField = "ten";
                DropDownList2.DataBind();
                DropDownList2.Items.Insert(0, new ListItem("Chọn nhóm", ""));
                if (nhomSanPham.Any(p => p.id.ToString() == q.id_nhom))
                    DropDownList2.SelectedValue = q.id_nhom;
                else
                    DropDownList2.SelectedIndex = 0;


                DropDownList3.DataSource = donvitinh;
                DropDownList3.DataValueField = "id";
                DropDownList3.DataTextField = "ten";
                DropDownList3.DataBind();
                DropDownList3.Items.Insert(0, new ListItem("Chọn đơn vị tính", ""));
                if (donvitinh.Any(p => p.id.ToString() == q.donvitinh))
                    DropDownList3.SelectedValue = q.donvitinh;
                else
                    DropDownList3.SelectedIndex = 0;

                txt_name.Text = q.ten;
                txt_link_fileupload.Text = q.anh;
                txt_model.Text = q.model;
                txt_thongso.Text = q.thongso_kythuat;
                txt_gianhap.Text = q.gianhap.Value.ToString("#,##0");
                txt_giaban.Text = q.giabanle.Value.ToString("#,##0");

                if (q.anh != "")
                {
                    Button2.Visible = true;
                    Label2.Text = "<div><small>Ảnh cũ</small></div><img src='" + q.anh + "' style='max-width: 100px' />";
                }
                else
                {
                    Button2.Visible = false;
                    Label2.Text = "";
                }
                bool _cohoadon = q.cohoadon.Value;
                if (_cohoadon == true)
                {
                    rbCoHoaDon.Checked = true; rbKhongCoHoaDon.Checked = false;
                }
                else
                {
                    rbCoHoaDon.Checked = false; rbKhongCoHoaDon.Checked = true;
                }
                bool _hangthanhly = q.hangthanhly.Value;
                if (_hangthanhly == true)
                    check_hangthanhly.Checked = true;
                else
                    check_hangthanhly.Checked = false;

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
        try
        {
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

    protected void but_add_edit_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("9", "9");
            #region Chuẩn bị dữ liệu
            //đảm bảo luôn có thư mục chứa ảnh
            if (!Directory.Exists(Server.MapPath("~/uploads/img-handler/"))) Directory.CreateDirectory(Server.MapPath("~/uploads/img-handler/"));
            string _tensp = txt_name.Text.Trim();
            string _anh = txt_link_fileupload.Text;
            bool _cohoadon = true;
            if (rbKhongCoHoaDon.Checked == true)
                _cohoadon = false;
            bool _hangthanhly = false;
            if (check_hangthanhly.Checked)
                _hangthanhly = true;
            string _id_hang = DropDownList1.SelectedValue;
            string _id_nhom = DropDownList2.SelectedValue;
            string _id_donvitinh = DropDownList3.SelectedValue;
            string _model = txt_model.Text.Trim().ToUpper();
            string _thongso = txt_thongso.Text;
            string _ghichu = txt_ghichu.Text;
            DateTime _ngaytao = DateTime.Now;
            string _nguoitao = mahoa_cl.giaima_Bcorn(Session["taikhoan"].ToString());
            Int64 _giaban = Number_cl.Check_Int64(txt_giaban.Text.Trim());
            Int64 _gianhap = Number_cl.Check_Int64(txt_gianhap.Text.Trim());
            #endregion

            using (dbDataContext db = new dbDataContext())
            {
                #region Kiểm tra ngoại lệ.


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
                if (_id_hang == "")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng chọn hãng sản phẩm.", "false", "false", "OK", "alert", ""), true);
                    return;
                }
                if (_id_nhom == "")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng chọn nhóm sản phẩm.", "false", "false", "OK", "alert", ""), true);
                    return;
                }
                if (_id_donvitinh == "")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng chọn đơn vị tính.", "false", "false", "OK", "alert", ""), true);
                    return;
                }
                if (_model == "")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập Model sản phẩm.", "false", "false", "OK", "alert", ""), true);
                    return;
                }

                #endregion

                if (ViewState["add_edit"].ToString() == "add")
                {
                    var q = db.KhoSanPham_tbs.FirstOrDefault(p => p.ten.ToUpper() == _tensp.ToUpper());
                    if (q != null)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Sản phẩm này đã tồn tại.", "false", "false", "OK", "alert", ""), true);
                        return;
                    }
                    #region thêm mới
                    KhoSanPham_tb _ob = new KhoSanPham_tb();
                    _ob.sanpham_tuychon = false;//đánh dấu đây kp là sản phẩm có sẵn, sp này khi báo giá thì cứ thêm vào rồi báo, bán đc hàng thì tự nhập xuất và thêm vào kho --> đổi thành true khi bán đc hàng
                    _ob.ten = _tensp;
                    _ob.id_nhom = _id_nhom;
                    _ob.id_hang = _id_hang;
                    _ob.donvitinh = _id_donvitinh;
                    _ob.anh = _anh;
                    _ob.model = _model;
                    _ob.thongso_kythuat = _thongso;
                    _ob.giabanle = _giaban;
                    _ob.gianhap = _gianhap;
                    _ob.cohoadon = _cohoadon;
                    _ob.hangthanhly = _hangthanhly;
                    _ob.ghichu = _ghichu;
                    _ob.ngaytao = _ngaytao;
                    _ob.nguoitao = _nguoitao;
                    _ob.soluong_hientai = 0;
                    db.KhoSanPham_tbs.InsertOnSubmit(_ob);
                    db.SubmitChanges();
                    #endregion
                    #region cập nhật dữ liệu và update hiển thị
                    txt_name.Text = ""; txt_model.Text = ""; txt_thongso.Text = ""; txt_giaban.Text = "0"; txt_gianhap.Text = "0"; txt_ghichu.Text = ""; txt_link_fileupload.Text = "";
                    show_main();
                    up_main.Update();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
                    #endregion
                }
                else//edit
                {
                    #region chuẩn bị dữ liệu
                    var q_edit = db.KhoSanPham_tbs.FirstOrDefault(p => p.id.ToString() == ViewState["id_edit"].ToString());
                    if (q_edit != null)
                    {
                        var q = db.KhoSanPham_tbs.FirstOrDefault(p => p.ten.ToUpper() == _tensp.ToUpper() && p.id.ToString() != ViewState["id_edit"].ToString());
                        if (q != null)
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Sản phẩm này đã tồn tại.", "false", "false", "OK", "alert", ""), true);
                            return;
                        }

                        #region kiểm tra ngoại lệ. sau đó cập nhật
                        KhoSanPham_tb _ob = q_edit;
                        _ob.ten = _tensp;
                        _ob.id_nhom = _id_nhom;
                        _ob.id_hang = _id_hang;
                        _ob.donvitinh = _id_donvitinh;
                        _ob.anh = _anh;
                        _ob.model = _model;
                        _ob.thongso_kythuat = _thongso;
                        _ob.giabanle = _giaban;
                        _ob.gianhap = _gianhap;
                        _ob.cohoadon = _cohoadon;
                        _ob.hangthanhly = _hangthanhly;
                        _ob.ghichu = _ghichu;
                        db.SubmitChanges();


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
                var list_all = (from ob1 in db.KhoSanPham_tbs
                                join ob2 in db.DuLieuNguon_tbs
                                    .Where(p => p.kyhieu == "hangsanpham") on ob1.id_hang equals ob2.id.ToString() into HangGroup
                                from ob2 in HangGroup.DefaultIfEmpty()
                                join ob3 in db.DuLieuNguon_tbs
                                    .Where(p => p.kyhieu == "nhomsanpham") on ob1.id_nhom equals ob3.id.ToString() into NhomGroup
                                from ob3 in NhomGroup.DefaultIfEmpty()
                                join ob4 in db.DuLieuNguon_tbs
                                    .Where(p => p.kyhieu == "donvitinh") on ob1.donvitinh equals ob4.id.ToString() into DVTGroup
                                from ob4 in DVTGroup.DefaultIfEmpty()
                                select new
                                {
                                    ob1.id,
                                    TenSP = ob1.ten,
                                    Hang = ob2 == null ? "" : ob2.ten,
                                    Nhom = ob3 == null ? "" : ob3.ten,
                                    DVT = ob4 == null ? "" : ob4.ten,
                                    ob1.anh,
                                    ob1.model,
                                    ob1.thongso_kythuat,
                                    ob1.gianhap,
                                    TongGiaNhap = ob1.gianhap * ob1.soluong_hientai,
                                    ob1.giabanle,
                                    TongBanLe = ob1.giabanle * ob1.soluong_hientai,
                                    ob1.cohoadon,
                                    ob1.hangthanhly,
                                    ob1.soluong_hientai,
                                    ob1.ghichu,
                                    ob1.ngaytao,
                                    ob1.nguoitao,
                                }).AsQueryable();




                // Kiểm tra xem textbox có dữ liệu tìm kiếm không
                string _key = txt_timkiem.Text.Trim();
                if (!string.IsNullOrEmpty(_key))
                    list_all = list_all.Where(p => p.TenSP.Contains(_key) || p.id.ToString() == _key);
                else
                {
                    string _key1 = txt_timkiem1.Text.Trim();
                    if (!string.IsNullOrEmpty(_key1))
                        list_all = list_all.Where(p => p.TenSP.Contains(_key1) || p.Hang.Contains(_key1) || p.Nhom.Contains(_key1) || p.model == _key1 || p.id.ToString() == _key1);
                }

                ////xử lý theo thời gian
                //string _id_locthoigian = ddl_thoigian.SelectedValue;
                //if (_id_locthoigian == "1")//lọc theo ngày tạo
                //{
                //    if (txt_tungay.Text != "")
                //        list_all = list_all.Where(p => p.ngaytao.Value.Date >= DateTime.Parse(txt_tungay.Text).Date);
                //    if (txt_denngay.Text != "")
                //        list_all = list_all.Where(p => p.ngaytao.Value.Date <= DateTime.Parse(txt_denngay.Text).Date);
                //}

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
                list_all = list_all.OrderBy(p => p.Nhom).ThenBy(p => p.TenSP);
                int _Tong_Record = list_all.Count();

                //Int64 _tongbanle = list_all.Sum(p => p.TongBanLe.Value), _tonggianhap = list_all.Sum(p => p.TongGiaNhap.Value);
                //ViewState["tong_ton"] = list_all.Sum(p => p.soluong_hientai.Value).ToString("#,##0");
                //ViewState["tong_giale"] = _tongbanle.ToString("#,##0");
                //ViewState["tong_gianhap"] = _tonggianhap.ToString("#,##0");
                //ViewState["tong_laigop"] = (_tongbanle - _tonggianhap).ToString("#,##0");
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
                                case "id":
                                    worksheet.Cells[_row, _cot].Value = t.id; _cot = _cot + 1;
                                    break;
                                case "TenSP":
                                    worksheet.Cells[_row, _cot].Value = t.TenSP; _cot = _cot + 1;
                                    break;
                                case "hangthanhly":
                                    worksheet.Cells[_row, _cot].Value = t.hangthanhly; _cot = _cot + 1;
                                    break;
                                case "cohoadon":
                                    worksheet.Cells[_row, _cot].Value = t.cohoadon; _cot = _cot + 1;
                                    break;
                                case "DVT":
                                    worksheet.Cells[_row, _cot].Value = t.DVT; _cot = _cot + 1;
                                    break;
                                case "soluong_hientai":
                                    worksheet.Cells[_row, _cot].Value = t.soluong_hientai; _cot = _cot + 1;
                                    break;
                                case "giabanle":
                                    worksheet.Cells[_row, _cot].Value = t.giabanle; _cot = _cot + 1;
                                    break;
                                case "Hang":
                                    worksheet.Cells[_row, _cot].Value = t.Hang; _cot = _cot + 1;
                                    break;
                                case "model":
                                    worksheet.Cells[_row, _cot].Value = t.model; _cot = _cot + 1;
                                    break;
                                case "Nhom":
                                    worksheet.Cells[_row, _cot].Value = t.Nhom; _cot = _cot + 1;
                                    break;
                                case "ghichu":
                                    worksheet.Cells[_row, _cot].Value = t.ghichu; _cot = _cot + 1;
                                    break;
                                case "ngaytao":
                                    // Giả định t.ngaytao là thuộc tính DateTime hoặc DateTime?
                                    DateTime? ngayTao = t.ngaytao;

                                    if (ngayTao.HasValue)
                                    {
                                        // Chuyển đổi DateTime thành chỉ ngày (ngayTao.Value.Date)
                                        DateTime onlyDate = ngayTao.Value.Date;

                                        // Đặt giá trị ô là kiểu DateTime chỉ với ngày
                                        worksheet.Cells[_row, _cot].Value = onlyDate;

                                        // Định dạng số cho ô thành "dd/MM/yyyy"
                                        worksheet.Cells[_row, _cot].Style.Numberformat.Format = "dd/MM/yyyy";
                                    }
                                    else
                                    {
                                        // Nếu giá trị ngayTao là null, bạn có thể để trống ô đó hoặc xử lý theo cách khác
                                        worksheet.Cells[_row, _cot].Value = DBNull.Value; // Hoặc để trống, hoặc đặt giá trị mặc định
                                    }
                                    _cot = _cot + 1;
                                    break;
                                default: break;
                            }
                        }
                        _row++;
                    }
                    // Lưu tệp Excel vào đường dẫn đã chỉ định
                    string filePath = "/uploads/files/Kho-" + str_cl.taoma_theothoigian() + ".xlsx";
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
            if (Request.Cookies["cookie_qlkho"] != null)//nếu có ck r thì lưu giá trị mới
            {
                HttpCookie _ck = Request.Cookies["cookie_qlkho"];
                _ck["show"] = txt_show.Text;
                _ck["trang_hientai"] = ViewState["current_page_qlkho"].ToString();
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
            if (Request.Cookies["cookie_qlkho"] != null)
                Response.Cookies["cookie_qlkho"].Expires = DateTime.Now.AddYears(-1);
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
            check_login_cl.check_login_admin("11", "11");

            var selectedIds = new List<Int64>(); // Danh sách để lưu trữ ID của các mục đã được chọn

            // Thu thập tất cả ID của các mục đã được chọn trong Repeater1
            foreach (RepeaterItem item in Repeater1.Items)
            {
                CheckBox chkItem = (CheckBox)item.FindControl("checkID");
                Label lblData = (Label)item.FindControl("lbID");

                if (chkItem != null && lblData != null && chkItem.Checked)
                {
                    int id = int.Parse(lblData.Text);
                    selectedIds.Add(id); // Thêm ID vào danh sách
                }
            }

            if (selectedIds.Count > 0)
            {
                // Sử dụng dbDataContext và thực hiện cập nhật hàng loạt
                using (dbDataContext db = new dbDataContext())
                {
                    // Lấy tất cả các mục có ID trong danh sách và cập nhật thuộc tính `bin` của chúng
                    var ListsToUpdate = db.KhoSanPham_tbs
                        .Where(d => selectedIds.Contains(d.id))
                        .ToList();

                    foreach (var dm in ListsToUpdate)
                    {
                        db.KhoSanPham_tbs.DeleteOnSubmit(dm);
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


    #region xem chi tiết
    protected void but_show_chitiet_Click(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chức năng đang cập nhật", "1000", "warning"), true);
    }
    #endregion

    #region NHẬP HÀNG
    public void reset_control_nhaphang()
    {
        try
        {
            Label3.Text = null;
            Label4.Text = null; 
            ViewState["id_sanpham"] = null;
            txt_soluong_nhap.Text = "";
            //txt_gianhaphang.Text = "";
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
    protected void but_show_form_nhaphang_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_admin("12", "12");
        //reset_control_nhaphang();
        using (dbDataContext db = new dbDataContext())
        {

            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;
            //truy vấn dữ liệu để sửa
            var q = db.KhoSanPham_tbs.FirstOrDefault(p => p.id.ToString() == _id);
            if (q != null)
            {
                ViewState["id_edit"] = _id;
                Label3.Text = q.ten;
                Label4.Text = q.soluong_hientai.Value.ToString("#,##0");
                //txt_gianhaphang.Text = q.gianhap.Value.ToString("#,##0");
                //int _soluongnhap = Number_cl.Check_Int(txt_soluong_nhap.Text.Trim());
                //Int64 _gianhap = Number_cl.Check_Int64(txt_gianhaphang.Text.Trim());
                ViewState["gianhap_hientai"] = q.gianhap.ToString();
            }
            else
                ViewState["id_edit"] = null;
        }
        pn_nhaphang.Visible = !pn_nhaphang.Visible;
        up_nhaphang.Update();
    }
    protected void but_close_form_nhaphang_Click(object sender, EventArgs e)
    {
        reset_control_nhaphang();
        pn_nhaphang.Visible = !pn_nhaphang.Visible;
        up_nhaphang.Update();
    }


    protected void but_nhaphang_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("12", "12");
            DateTime _ngaynhap = DateTime.Now;
            string _nguoinhap = mahoa_cl.giaima_Bcorn(Session["taikhoan"].ToString());
            int _soluongnhap = Number_cl.Check_Int(txt_soluong_nhap.Text.Trim());
            Int64 _gianhap = Number_cl.Check_Int64(ViewState["gianhap_hientai"].ToString());
            using (dbDataContext db = new dbDataContext())
            {
                if (_soluongnhap == 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Số lượng nhập không hợp lệ.", "false", "false", "OK", "alert", ""), true);
                    return;
                }
                //if (_gianhap == 0)
                //{
                //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Giá nhập không hợp lệ.", "false", "false", "OK", "alert", ""), true);
                //    return;
                //}
                var q = db.KhoSanPham_tbs.FirstOrDefault(p => p.id.ToString() == ViewState["id_edit"].ToString());
                if (q != null)
                {
                    NhapXuatKho_tb _ob = new NhapXuatKho_tb();
                    _ob.nhap_hay_xuat = true;//nhập
                    _ob.id_sanpham = ViewState["id_edit"].ToString();
                    _ob.ten_sanpham = q.ten;
                    _ob.soluong_nhap = _soluongnhap;
                    _ob.gia_nhap = _gianhap;
                    if (_gianhap != q.gianhap)//nếu có thay đổi giá nhập
                        q.gianhap = _gianhap;//tự cập nhật giá nhập cho sản phẩm tại bảng Kho
                    _ob.ngaynhap = _ngaynhap;
                    _ob.nguoinhap = _nguoinhap;
                    _ob.ton_hientai = q.soluong_hientai;//tồn trước khi nhập
                    q.soluong_hientai = q.soluong_hientai + _soluongnhap;//tăng tồn hiện tại
                    db.NhapXuatKho_tbs.InsertOnSubmit(_ob);
                    db.SubmitChanges();
                    show_main();
                    up_main.Update();

                    //reset control
                    reset_control_nhaphang();
                    //ẩn form
                    pn_nhaphang.Visible = !pn_nhaphang.Visible;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
                }
            }
        }
        catch(Exception _ex)
        {
            Response.Redirect("/admin");
        }
    }
    //protected void txt_soluong_nhap_TextChanged(object sender, EventArgs e)
    //{
    //    int _soluongnhap = Number_cl.Check_Int(txt_soluong_nhap.Text.Trim());
    //    Int64 _gianhap = Number_cl.Check_Int64(txt_gianhaphang.Text.Trim());
    //    Label5.Text = (_soluongnhap * _gianhap).ToString("#,##0");
    //}
    //protected void txt_gianhaphang_TextChanged(object sender, EventArgs e)
    //{
    //    int _soluongnhap = Number_cl.Check_Int(txt_soluong_nhap.Text.Trim());
    //    Int64 _gianhap = Number_cl.Check_Int64(txt_gianhaphang.Text.Trim());
    //    Label5.Text = (_soluongnhap * _gianhap).ToString("#,##0");
    //}
    #endregion




}