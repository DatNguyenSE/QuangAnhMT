using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_quan_ly_kho_muon_hang : System.Web.UI.Page
{
    String_cl str_cl = new String_cl();
    DateTime_cl dt_cl = new DateTime_cl();

    public void set_dulieu_macdinh()
    {
        try
        {
            ResetButtonCss();//button chọn ngày nhanh
            txt_show.Text = "30";
            ViewState["current_page_muonhang"] = "1";


            #region set_get_cookie
            // Lấy cookie "cookie_muonhang" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_muonhang"];
            if (cookie == null)
            {
                ListBox1.SelectedIndex = 0;//mặc định chọn tất cả phân loại, nếu select=true ngoài html thì k lưu lịch sử đc, kệ mẹ nó cứ làm y vậy đi, đừng quan tâm tới đoạn này
                                           // Nếu cookie không tồn tại, tạo cookie mới
                cookie = new HttpCookie("cookie_muonhang");
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
                ViewState["current_page_muonhang"] = cookie["trang_hientai"];
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
                    //PlaceHolder1.Visible = true;
                    // PlaceHolder3.Visible = true;
                    // PlaceHolder6.Visible = true;
                    //PlaceHolder4.Visible = true;
                }
                else
                {
                    // PlaceHolder1.Visible = false;
                    // PlaceHolder3.Visible = false;
                    // PlaceHolder6.Visible = false;
                    //PlaceHolder4.Visible = false;
                }
                #endregion

                #region lấy dữ liệu
                var list_all = (from ob1 in db.PhieuMuonHang_tbs
                                join ob2 in db.PhieuMuonHang_ChiTiet_tbs
                                     on ob1.id.ToString() equals ob2.id_PhieuMuon
                                join ob3 in db.KhoSanPham_tbs
                                     on ob2.id_sanpham equals ob3.id.ToString() into PhieuMuons
                                from ob3 in PhieuMuons.DefaultIfEmpty()
                                group new { ob1, ob2, ob3 } by new
                                {
                                    ob1.id,
                                    ob1.ngaytao,
                                    ob1.tenchuongtrinh,
                                    ob1.nguoitao,
                                } into g

                                select new
                                {
                                    id = g.Key.id,
                                    TenSP = string.Join(", ", g.Select(x => x.ob3.ten).Distinct()),
                                    TenChuongTrinh = g.Key.tenchuongtrinh ?? "",
                                    ngaytao = g.Key.ngaytao,
                                    nguoitao = g.Key.nguoitao,
                                    soLuongMuon =  g.Sum(p=>p.ob2.SoLuongMuon) == null ? 0 : g.Sum(p => p.ob2.SoLuongMuon),
                                    soLuongTra = g.Sum(p => p.ob2.SoLuongTra) == null ? 0 : g.Sum(p => p.ob2.SoLuongTra),
                                    TinhTrang = (g.Sum(p => p.ob2.SoLuongMuon) == null ? 0 : g.Sum(p => p.ob2.SoLuongMuon)) - (g.Sum(p => p.ob2.SoLuongTra) == null ? 0 : g.Sum(p => p.ob2.SoLuongTra)) == 0 ? "Đã trả xong" : "Chưa trả hết",
                                }).AsQueryable();

                list_all = list_all.OrderByDescending(p => p.id);

                // Kiểm tra xem textbox có dữ liệu tìm kiếm không
                string _key = txt_timkiem.Text.Trim();
                if (!string.IsNullOrEmpty(_key))
                    list_all = list_all.Where(p => p.nguoitao.Contains(_key) || p.id.ToString() == _key);
                else
                {
                    string _key1 = txt_timkiem1.Text.Trim();
                    if (!string.IsNullOrEmpty(_key1))
                        list_all = list_all.Where(p => p.nguoitao.Contains(_key1) || p.id.ToString() == _key1);
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
                list_all = list_all.OrderByDescending(p => p.id);
                int _Tong_Record = list_all.Count();

                #endregion

                #region phân trang OK, k sửa
                // Xử lý số record mỗi trang
                int show = Number_cl.Check_Int(txt_show.Text.Trim()); if (show <= 0) show = 30;
                //xử lý trang hiện tại. Đảm bảo current_page không nhỏ hơn 1 và không lớn hơn total_page
                int current_page = int.Parse(ViewState["current_page_muonhang"].ToString()); int total_page = number_of_page_class.return_total_page(_Tong_Record, show); if (current_page < 1) current_page = 1; else if (current_page > total_page) current_page = total_page;
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
            ViewState["current_page_muonhang"] = int.Parse(ViewState["current_page_muonhang"].ToString()) - 1;
            #region LƯU TRANG HIỆN TẠI
            // Lấy cookie "cookie_muonhang" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_muonhang"];
            if (cookie != null)
            {
                cookie["trang_hientai"] = ViewState["current_page_muonhang"].ToString();
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
            ViewState["current_page_muonhang"] = int.Parse(ViewState["current_page_muonhang"].ToString()) + 1;
            #region LƯU TRANG HIỆN TẠI
            // Lấy cookie "cookie_muonhang" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_muonhang"];
            if (cookie != null)
            {
                cookie["trang_hientai"] = ViewState["current_page_muonhang"].ToString();
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
            ViewState["current_page_muonhang"] = 1;
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
            if (Request.Cookies["cookie_muonhang"] != null)//nếu có ck r thì lưu giá trị mới
            {
                HttpCookie _ck = Request.Cookies["cookie_muonhang"];
                _ck["show"] = txt_show.Text;
                _ck["trang_hientai"] = ViewState["current_page_muonhang"].ToString();
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
            if (Request.Cookies["cookie_muonhang"] != null)
                Response.Cookies["cookie_muonhang"].Expires = DateTime.Now.AddYears(-1);
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

    #region ADD - EDIT - CHI TIẾT

    // Load View Chi Tiết
    public void load_edit(dbDataContext db, string _idbg)
    {
        try
        {
            // Lấy danh sách chi tiết bao giá cùng thông tin sản phẩm
            var list_all = (from ob1 in db.PhieuMuonHang_tbs
                            join ob2 in db.PhieuMuonHang_ChiTiet_tbs
                                 on ob1.id.ToString() equals ob2.id_PhieuMuon
                            join ob3 in db.KhoSanPham_tbs
                                 on ob2.id_sanpham equals ob3.id.ToString() into PhieuMuons
                            from ob3 in PhieuMuons.DefaultIfEmpty()
                            group new { ob1, ob2, ob3 } by new
                            {
                                ob2.id,
                                ob3.ten,
                                ob3.anh,
                                ob2.SoLuongMuon,
                                ob2.SoLuongTra,
                                ob2.NgayMuon,
                                ob2.NgayTra,
                                ob2.NguoiMuon,
                                ob2.id_PhieuMuon

                            } into g

                            select new
                            {
                                idPhieuMuon = g.Key.id_PhieuMuon,
                                id = g.Key.id,
                                TenSP = g.Key.ten,
                                anh = g.Key.anh,
                                SoLuongMuon = g.Key.SoLuongMuon,
                                SoLuongTra = g.Key.SoLuongTra,
                                NgayMuon = g.Key.NgayMuon,
                                NgayTra = g.Key.NgayTra,
                                NguoiMuon = g.Key.NguoiMuon,
                            }).Where(p => p.idPhieuMuon.ToString() == _idbg).AsQueryable();

            list_all = list_all.OrderByDescending(p => p.id);

            Repeater2.DataSource = list_all;
            Repeater2.DataBind();

            Repeater3.DataSource = list_all;
            Repeater3.DataBind();
        }
        catch (Exception _ex)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", _ex.StackTrace, "false", "false", "OK", "alert", ""), true);
            return;
        }

    }

    // Đóng Form Nhập Trả
    protected void but_close_form_nhaptra_Click(object sender, EventArgs e)
    {
        reset_control_nhaphang();
        pn_nhaphang.Visible = !pn_nhaphang.Visible;
        up_nhaphang.Update();
    }

    // Reset Form Tạo Mới
    public void reset_control_add_edit()
    {
        try
        {
            Label1.Text = null;
            // txt_name.Text = ""; txt_model.Text = ""; txt_thongso.Text = ""; txt_ghichu.Text = "";
            txt_soluongmuon.Text = "0";
            DropDownList1.DataSource = null;
            DropDownList1.DataBind();

            Repeater2.DataSource = null;
            Repeater2.DataBind();
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

    // Mở Form Tạo Mới
    protected void but_show_form_add_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("9", "9");
            //reset control
            reset_control_add_edit();

            ViewState["add_edit"] = "add";
            Label1.Text = "Tạo phiếu mượn";
            but_add_edit.Text = "Tạo Mới";

            using (dbDataContext db = new dbDataContext())
            {
                var data = db.KhoSanPham_tbs
             .ToList();
                DropDownList1.DataSource = data;
                DropDownList1.DataValueField = "id";
                DropDownList1.DataTextField = "ten";
                DropDownList1.DataBind();
                DropDownList1.Items.Insert(0, new ListItem("Chọn sản phẩm", ""));

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

    // Đóng From Tạo Mới
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


    // Hiển thị Form Chỉnh sửa
    protected void but_show_chinhsua_Click(object sender, EventArgs e)
    {

        check_login_cl.check_login_admin("10", "10");
        ViewState["add_edit"] = "edit";
        Label1.Text = "CHỈNH SỬA PHIẾU MƯỢN";
        but_add_edit.Text = "CẬP NHẬT";
        using (dbDataContext db = new dbDataContext())
        {

            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;

            //truy vấn dữ liệu để sửa
            var q = db.PhieuMuonHang_tbs.FirstOrDefault(p => p.id.ToString() == _id);
            if (q != null)
            {
                ViewState["id_edit"] = _id;

                var data = db.KhoSanPham_tbs
             .ToList();
                DropDownList1.DataSource = data;
                DropDownList1.DataValueField = "id";
                DropDownList1.DataTextField = "ten";
                DropDownList1.DataBind();
                DropDownList1.Items.Insert(0, new ListItem("Chọn sản phẩm", ""));

                TxtTenChuongTrinhf.Text = q.tenchuongtrinh;

                load_edit(db, _id);

                //hiện form add_edit trong updatePanel_add
                pn_add.Visible = !pn_add.Visible;
                up_add.Update();
            }
            else
                ViewState["id_edit"] = "";

        }

    }
    // Đóng Chỉnh Sửa
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
    // Thêm mới và Chỉnh sửa Phiếu mượn
    protected void but_add_edit_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("9", "9");
            #region Chuẩn bị dữ liệu
            string _id_hang = DropDownList1.SelectedValue;
            DateTime _ngaytao = DateTime.Now;
            string _nguoitao = mahoa_cl.giaima_Bcorn(Session["taikhoan"].ToString());
            string tenChuongTrinh = TxtTenChuongTrinhf.Text;

            

            Int64 _soLuongMuon = Number_cl.Check_Int64(txt_soluongmuon.Text.Trim());
            #endregion

            using (dbDataContext db = new dbDataContext())
            {
                var hoTen = db.taikhoan_tbs.Where(p=>p.taikhoan.Contains(_nguoitao)).FirstOrDefault();

                #region Kiểm tra ngoại lệ.
                if (_id_hang == "")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng chọn sản phẩm.", "false", "false", "OK", "alert", ""), true);
                    return;
                }
                if (_soLuongMuon == 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập số lượng mượn.", "false", "false", "OK", "alert", ""), true);
                    return;
                }

                #endregion
                #region Kiểm tra tồn

                // 2. Lấy số lượng tồn kho từ bảng kho
                var soLuongTon = db.KhoSanPham_tbs
                    .Where(x => x.id.ToString() == _id_hang)
                    .Select(x => x.soluong_hientai)
                    .FirstOrDefault();

                // 3. Tính tồn kho thực tế

                int? soLuongConLaiThucTe = soLuongTon - Convert.ToInt32(_soLuongMuon);

                if (soLuongConLaiThucTe < 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Tồn đã hết hoặc tồn thấp hơn số lượng mượn nên không thể mượn được.", "false", "false", "OK", "alert", ""), true);
                    return;
                }

                #endregion

                string id_phieuMuon = "";
                if (ViewState["add_edit"].ToString() == "add")
                {
                    #region thêm mới
                    var phieuMuonKho = db.PhieuMuonHang_tbs.Where(p => p.id.ToString() == id_phieuMuon).ToList();
                    if (phieuMuonKho.Count <= 0)
                    {
                        PhieuMuonHang_tb _ob = new PhieuMuonHang_tb();
                        _ob.ngaytao = _ngaytao;
                        _ob.nguoitao = hoTen.hoten ?? "";
                        _ob.tenchuongtrinh = tenChuongTrinh ?? "";
                        db.PhieuMuonHang_tbs.InsertOnSubmit(_ob);
                        db.SubmitChanges();
                        id_phieuMuon = _ob.id.ToString();
                        ViewState["id_edit"] = _ob.id.ToString();
                    }

                }

                #endregion

                ViewState["add_edit"] = "edit";

                if (ViewState["id_edit"] != null)
                {
                    id_phieuMuon = ViewState["id_edit"].ToString();
                }

                var phieuMuon = db.PhieuMuonHang_tbs.Where(p => p.id.ToString() == id_phieuMuon).ToList();
                if (phieuMuon.Any())
                {
                    var detailPhieuMuon = db.PhieuMuonHang_ChiTiet_tbs.Where(p => p.id_PhieuMuon.ToString() == id_phieuMuon && p.id_sanpham == _id_hang).ToList();
                    if(detailPhieuMuon.Any())
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Đã tồn tại sản phẩm này, không thể thêm nữa trong phiếu mượn này.", "false", "false", "OK", "alert", ""), true);
                        return;
                    }    


                    PhieuMuonHang_ChiTiet_tb addDetailPhieu = new PhieuMuonHang_ChiTiet_tb();
                    addDetailPhieu.id_PhieuMuon = id_phieuMuon;
                    addDetailPhieu.id_sanpham = _id_hang;
                    addDetailPhieu.NgayMuon = DateTime.Now;
                    addDetailPhieu.SoLuongMuon = Convert.ToInt32(_soLuongMuon);
                    addDetailPhieu.NguoiMuon = hoTen.hoten ?? ""; ;

                    db.PhieuMuonHang_ChiTiet_tbs.InsertOnSubmit(addDetailPhieu);
                    db.SubmitChanges();

                    //ẩn form
                    //pn_nhaphang.Visible = !pn_nhaphang.Visible;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Thêm thành công.", "1000", "warning"), true);
                    txt_soluongmuon.Text = "0";
                    load_edit(db, id_phieuMuon);
                    show_main();
                    up_main.Update();
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
    
    // Reset Form Nhập Trả
    public void reset_control_nhaphang()
    {
        try
        {
            //Label3.Text = null;
           // Label4.Text = null;
            ViewState["id_sanpham"] = null;
            txt_soluong_tra.Text = "";
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

    // Xóa tất cả Phiếu đã chọn
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
                    var ListsToUpdate = db.PhieuMuonHang_tbs
                        .Where(d => selectedIds.Contains(d.id))
                        .ToList();

                    foreach (var dm in ListsToUpdate)
                    {
                        var listDetail = db.PhieuMuonHang_ChiTiet_tbs.Where(p => p.id_PhieuMuon == dm.id.ToString()).ToList();

                        foreach(var item in listDetail)
                        {
                            db.PhieuMuonHang_ChiTiet_tbs.DeleteOnSubmit(item);
                        }

                        db.PhieuMuonHang_tbs.DeleteOnSubmit(dm);
                    }

                    // Lưu tất cả các thay đổi trong một lần
                    db.SubmitChanges();
                }

                // Hiển thị thông báo thành công
                show_main();
                up_main.Update();
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


    // Load Form Nhập Trả
    protected void but_show_form_nhaphang_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_admin("12", "12");
        reset_control_nhaphang();
        using (dbDataContext db = new dbDataContext())
        {

            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;
            //truy vấn dữ liệu để sửa

            var list_all = (from ob1 in db.PhieuMuonHang_tbs
                            join ob2 in db.PhieuMuonHang_ChiTiet_tbs
                                 on ob1.id.ToString() equals ob2.id_PhieuMuon
                            join ob3 in db.KhoSanPham_tbs
                                 on ob2.id_sanpham equals ob3.id.ToString() into PhieuMuons
                            from ob3 in PhieuMuons.DefaultIfEmpty()
                            group new { ob1, ob2, ob3 } by new
                            {
                                ob3.ten,
                                ob2.id_PhieuMuon,
                                ob2.id_sanpham,
                                ob2.SoLuongMuon,
                                ob2.SoLuongTra
                            } into g

                            select new
                            {
                                idPhieuMuon = g.Key.id_PhieuMuon,
                                ten = g.Key.ten,
                                id = g.Key.id_sanpham,
                                soLuongMuon = g.Key.SoLuongMuon,
                                soLuongTra = g.Key.SoLuongTra
                            }).ToList()
                            .Where(p => p.idPhieuMuon.ToString() == _id && (p.soLuongMuon ?? 0) != (p.soLuongTra ?? 0)).AsQueryable();

                DropDownList2.DataSource = list_all;
                DropDownList2.DataValueField = "id";
                DropDownList2.DataTextField = "ten";
                DropDownList2.DataBind();
                DropDownList2.Items.Insert(0, new ListItem("Chọn sản phẩm", ""));
            ViewState["id_NhapTra"] = _id;

            load_edit(db, _id);
        }

        
        pn_nhaphang.Visible = !pn_nhaphang.Visible;
        up_nhaphang.Update();
    }
    // Nhập Trả
    protected void but_nhapTra_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            string id = ViewState["id_NhapTra"].ToString();
            string _id_hang = DropDownList2.SelectedValue;
            int soLuongTra = Convert.ToInt32(txt_soluong_tra.Text);
            if(soLuongTra <= 0)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập số lượng trả.", "false", "false", "OK", "alert", ""), true);
                return;
            } 
            

            var idPhieu = db.PhieuMuonHang_tbs.Where(p => p.id.ToString() == id).ToList();
            if(idPhieu.Any())
            {
                var detailPhieu = db.PhieuMuonHang_ChiTiet_tbs.FirstOrDefault(p => p.id_PhieuMuon == id && p.id_sanpham == _id_hang);

                if ((detailPhieu?.SoLuongTra ?? 0) > 0)
                {
                    if (soLuongTra + detailPhieu.SoLuongTra > detailPhieu.SoLuongMuon)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Số lượng trả không được nhiều hơn số lượng mượn", "false", "false", "OK", "alert", ""), true);
                        return;
                    }

                    detailPhieu.SoLuongTra = soLuongTra + detailPhieu.SoLuongTra;
                    detailPhieu.NgayTra = DateTime.Now;
                }
                else
                {
                    if (soLuongTra > detailPhieu.SoLuongMuon)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Số lượng trả không được nhiều hơn số lượng mượn", "false", "false", "OK", "alert", ""), true);
                        return;
                    }

                    detailPhieu.SoLuongTra = soLuongTra;
                    detailPhieu.NgayTra = DateTime.Now;
                }
                
            }

            db.SubmitChanges();

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Trả thành công.", "1000", "warning"), true);
            txt_soluong_tra.Text = "0";
            load_edit(db, id);
            show_main();
            up_nhaphang.Update();
            up_main.Update();
        }
    }

    // Xóa Chi Tiết Phiếu
    protected void but_xoachitiet_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            Button button = (Button)sender;
            string _id = button.CommandArgument;

            var detailPhieuMuon = db.PhieuMuonHang_ChiTiet_tbs.Where(p => p.id.ToString() == _id).FirstOrDefault();

            string idPhieu = "";
            if(detailPhieuMuon != null)
            {
                idPhieu = detailPhieuMuon.id_PhieuMuon;
                db.PhieuMuonHang_ChiTiet_tbs.DeleteOnSubmit(detailPhieuMuon);
                db.SubmitChanges();
            }

            load_edit(db, idPhieu);
            show_main();
            up_add.Update();
        }

        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa thành công.", "1000", "warning"), true);
    }


    protected void but_show_form_xuat_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            var list_all = db.spLoad_ExportPhieuMuonHang().AsQueryable();    
            //sắp xếp
            list_all = list_all.OrderByDescending(p => p.NgayMuon);

            var list_all_result = list_all.ToList();
            var list_dynamic = list_all_result.Select(x =>
                (IDictionary<string, object>)x.GetType().GetProperties().ToDictionary(
                    p => p.Name,
                    p => p.GetValue(x, null)
                )
            ).ToList();

            var headers = new Dictionary<string, string>
                {
                    { "NguoiMuon", "Người mượn" },
                    { "TenSanPham", "Tên sản phẩm" },
                    { "tenchuongtrinh", "Tên chương trình" },
                    { "NgayMuon", "Ngày mượn" },
                    { "NgayTra", "Ngày trả" },
                    { "SoLuongMuon", "Số lượng mượn" },
                    { "SoLuongTra", "Số lượng trả" },
                };


            XSSFWorkbook workbook = new XSSFWorkbook();

            ExportExcel.ExportToExcelFormat(list_dynamic, headers, workbook);

            // xong hết thì save file lại

            string fileName = Guid.NewGuid().ToString() + ".xlsx";
            string filePath = Server.MapPath("~/uploads/Files/" + fileName);
            using (FileStream fs = new FileStream(filePath, FileMode.CreateNew))
            {
                workbook.Write(fs);
            }

            // Tải file
            Response.Redirect("/uploads/Files/" + fileName);
        }
    }

    #endregion
}