using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_quan_ly_kho_Default : System.Web.UI.Page
{
    String_cl str_cl = new String_cl();
    DateTime_cl dt_cl = new DateTime_cl();

    private long? TryGetId(string value)
    {
        long id;
        if (long.TryParse(value, out id))
            return id;
        return null;
    }

    private void BindProductSourceDropDowns(dbDataContext db, KhoSanPham_tb currentProduct)
    {
        var data = db.DuLieuNguon_tbs
            .Where(p => p.kyhieu == "hangsanpham" || p.kyhieu == "nhomsanpham" || p.kyhieu == "donvitinh")
            .ToList();

        var hangSanPham = data.Where(p => p.kyhieu == "hangsanpham").OrderBy(p => p.ten).ToList();
        var nhomSanPham = data.Where(p => p.kyhieu == "nhomsanpham").OrderBy(p => p.ten).ToList();
        var donViTinh = data.Where(p => p.kyhieu == "donvitinh").OrderBy(p => p.ten).ToList();

        DropDownList1.DataSource = hangSanPham;
        DropDownList1.DataTextField = "ten";
        DropDownList1.DataValueField = "id";
        DropDownList1.DataBind();
        DropDownList1.Items.Insert(0, new ListItem("Chọn hãng", ""));

        DropDownList2.DataSource = nhomSanPham;
        DropDownList2.DataTextField = "ten";
        DropDownList2.DataValueField = "id";
        DropDownList2.DataBind();
        DropDownList2.Items.Insert(0, new ListItem("Chọn nhóm", ""));

        DropDownList3.DataSource = donViTinh;
        DropDownList3.DataTextField = "ten";
        DropDownList3.DataValueField = "id";
        DropDownList3.DataBind();
        DropDownList3.Items.Insert(0, new ListItem("Chọn đơn vị tính", ""));

        if (currentProduct != null)
        {
            if (DropDownList1.Items.FindByValue(currentProduct.id_hang) != null)
                DropDownList1.SelectedValue = currentProduct.id_hang;
            if (DropDownList2.Items.FindByValue(currentProduct.id_nhom) != null)
                DropDownList2.SelectedValue = currentProduct.id_nhom;
            if (DropDownList3.Items.FindByValue(currentProduct.donvitinh) != null)
                DropDownList3.SelectedValue = currentProduct.donvitinh;
        }
    }

    public void set_dulieu_macdinh()
    {
        try
        {
            ResetButtonCss();
            txt_show.Text = "30";
            ViewState["current_page_qlkho"] = "1";

            #region set_get_cookie
            HttpCookie cookie = Request.Cookies["cookie_qlkho"];
            if (cookie == null)
            {
                ListBox1.SelectedIndex = 0;
                cookie = new HttpCookie("cookie_qlkho");
                cookie["show"] = txt_show.Text;
                cookie["trang_hientai"] = "1";
                cookie["id_loctheothoigian"] = ddl_thoigian.SelectedValue;
                cookie["tungay"] = txt_tungay.Text;
                cookie["denngay"] = txt_denngay.Text;
                cookie["phanloai"] = "";
                cookie.Expires = DateTime.Now.AddDays(1);
                cookie.HttpOnly = true;
                cookie.Secure = true;
                Response.Cookies.Add(cookie);
            }
            else
            {
                txt_show.Text = cookie["show"];
                ViewState["current_page_qlkho"] = cookie["trang_hientai"];
                ddl_thoigian.SelectedValue = cookie["id_loctheothoigian"];
                txt_tungay.Text = cookie["tungay"];
                txt_denngay.Text = cookie["denngay"];
                if (cookie["phanloai"] == "")
                    ListBox1.SelectedIndex = 0;
                else
                {
                    string[] _chon_phanloai = cookie["phanloai"].Split(',');
                    foreach (ListItem item in ListBox1.Items)
                    {
                        if (_chon_phanloai.Contains(item.Value))
                            item.Selected = true;
                    }
                }
                cookie.Expires = DateTime.Now.AddDays(1);
                Response.Cookies.Set(cookie);
            }
            #endregion
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk))
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

            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk))
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

    public string GetHanBaoHanhWarning(object hanBaoHanhObj)
    {
        if (hanBaoHanhObj == null || hanBaoHanhObj == DBNull.Value)
            return "";

        DateTime dt;
        if (hanBaoHanhObj is DateTime)
        {
            dt = (DateTime)hanBaoHanhObj;
        }
        else if (!DateTime.TryParse(Convert.ToString(hanBaoHanhObj), out dt))
        {
            return "";
        }

        int daysLeft = (dt.Date - DateTime.Today).Days;

        if (daysLeft < 0)
        {
            return string.Format("<span class='text-bold' style='background-color: #653819; color: #ffffff; font-size: 10px; padding: 1px 6px; border-radius: 4px; display: inline-block;' title='Hạn bảo hành: {0:dd/MM/yyyy}'>Hết hạn bảo hành</span>", dt);
        }
        else if (daysLeft <= 10)
        {
            return string.Format("<span class='text-bold' style='background-color: #d97706; color: #ffffff; font-size: 10px; padding: 1px 6px; border-radius: 4px; display: inline-block;' title='Hạn bảo hành: {0:dd/MM/yyyy}'>Bảo hành (còn {1} ngày)</span>", dt, daysLeft);
        }

        return "";
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
                bool showSold = ViewState["show_sold"] != null && (bool)ViewState["show_sold"];
                if (lbl_toggle_sold_products != null)
                {
                    if (showSold)
                    {
                        lbl_toggle_sold_products.Text = "Quay lại sản phẩm tồn";
                        if (icon_toggle != null) icon_toggle.Attributes["class"] = "mif-undo fg-red";
                    }
                    else
                    {
                        lbl_toggle_sold_products.Text = "Xem sản phẩm đã bán";
                        if (icon_toggle != null) icon_toggle.Attributes["class"] = "mif-checkmark";
                    }
                }

                string _key = ViewState["search_key"] as string;
                if (_key == null)
                {
                    _key = !string.IsNullOrWhiteSpace(txt_timkiem1.Text)
                        ? txt_timkiem1.Text.Trim()
                        : txt_timkiem.Text.Trim();
                }

                var products = db.KhoSanPham_tbs.AsQueryable();

                if (showSold)
                {
                    products = products.Where(p => p.daban == true);
                }
                else
                {
                    products = products.Where(p => p.daban == null || p.daban == false);
                }

                if (!string.IsNullOrEmpty(_key))
                {
                    long searchId;
                    bool hasSearchId = long.TryParse(_key, out searchId);
                    var categoryIds = db.DuLieuNguon_tbs
                        .Where(p => (p.kyhieu == "hangsanpham" || p.kyhieu == "nhomsanpham") && p.ten.Contains(_key))
                        .Select(p => p.id.ToString());

                    products = products.Where(p =>
                        p.ten.Contains(_key) ||
                        p.model.Contains(_key) ||
                        p.so_seri.Contains(_key) ||
                        (hasSearchId && p.id == searchId) ||
                        categoryIds.Contains(p.id_hang) ||
                        categoryIds.Contains(p.id_nhom));
                }

                var list_all = (from ob1 in products
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
                                    ob1.so_seri,
                                    ob1.model,
                                    ob1.thongso_kythuat,
                                    ob1.gianhap,
                                    TongGiaNhap = ob1.gianhap * ob1.soluong_hientai,
                                    ob1.giabanle,
                                    TongBanLe = ob1.giabanle * ob1.soluong_hientai,
                                    ob1.cohoadon,
                                    ob1.hangthanhly,
                                    ob1.phantram_thanhly,
                                    ob1.han_baohanh,
                                    ob1.soluong_hientai,
                                    ob1.ghichu,
                                    ob1.ngaytao,
                                    ob1.nguoitao,
                                }).AsQueryable();

                var stats = products.GroupBy(p => 1).Select(g => new
                {
                    Count = g.Count(),
                    TongBanLe = g.Sum(p => (p.giabanle ?? 0) * (p.soluong_hientai ?? 0)),
                    TongGiaNhap = g.Sum(p => (p.gianhap ?? 0) * (p.soluong_hientai ?? 0)),
                    TongTon = g.Sum(p => p.soluong_hientai ?? 0)
                }).FirstOrDefault();

                int _Tong_Record = stats?.Count ?? 0;
                Int64 _tongbanle = stats?.TongBanLe ?? 0;
                Int64 _tonggianhap = stats?.TongGiaNhap ?? 0;
                Int64 _tong_ton = stats?.TongTon ?? 0;
                ViewState["tong_ton"] = _tong_ton.ToString("#,##0");
                ViewState["tong_giale"] = _tongbanle.ToString("#,##0");
                ViewState["tong_gianhap"] = _tonggianhap.ToString("#,##0");
                ViewState["tong_laigop"] = (_tongbanle - _tonggianhap).ToString("#,##0");
                #endregion

                #region phân trang OK, k sửa
                list_all = list_all
                    .OrderByDescending(p => p.ngaytao)
                    .ThenBy(p => p.Nhom)
                    .ThenBy(p => p.TenSP);
                int show = Number_cl.Check_Int(txt_show.Text.Trim()); if (show <= 0) show = 30;
                int current_page = int.Parse(ViewState["current_page_qlkho"].ToString()); int total_page = number_of_page_class.return_total_page(_Tong_Record, show); if (total_page == 0) total_page = 1; if (current_page > total_page) current_page = total_page; if (current_page < 1) current_page = 1;
                ViewState["total_page"] = total_page;
                if (current_page >= total_page)
                {
                    but_xemtiep.Enabled = false;
                    but_xemtiep1.Enabled = false;
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
                var list_split = list_all.Skip(current_page * show - show).Take(show).ToList();
                int stt = (show * current_page) - show + 1; int _s1 = stt + list_split.Count - 1;
                if (_Tong_Record != 0) lb_show.Text = stt + "-" + _s1 + " trong số " + _Tong_Record.ToString("#,##0"); else lb_show.Text = "0-0/0"; lb_show_md.Text = stt + "-" + _s1 + " trong số " + _Tong_Record.ToString("#,##0");
                #endregion
                Repeater1.DataSource = list_split;
                Repeater1.DataBind();
            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk))
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
            HttpCookie cookie = Request.Cookies["cookie_qlkho"];
            if (cookie != null)
            {
                cookie["trang_hientai"] = ViewState["current_page_qlkho"].ToString();
                cookie.Expires = DateTime.Now.AddDays(1);
                Response.Cookies.Set(cookie);
            }
            #endregion
            show_main();
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk))
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
            HttpCookie cookie = Request.Cookies["cookie_qlkho"];
            if (cookie != null)
            {
                cookie["trang_hientai"] = ViewState["current_page_qlkho"].ToString();
                cookie.Expires = DateTime.Now.AddDays(1);
                Response.Cookies.Set(cookie);
            }
            #endregion
            show_main();
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk))
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
            TextBox searchBox = sender as TextBox;
            ViewState["search_key"] = searchBox == null ? "" : searchBox.Text.Trim();
            show_main();
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk))
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
    private T FindControlRecursive<T>(Control root, string id) where T : Control
    {
        if (root == null) return null;
        if (root.ID == id) return root as T;

        foreach (Control child in root.Controls)
        {
            T result = FindControlRecursive<T>(child, id);
            if (result != null) return result;
        }
        return null;
    }

    private TextBox GetQuickTextBox(string id)
    {
        return FindControlRecursive<TextBox>(this, id);
    }

    private PlaceHolder GetQuickPlaceHolder(string id)
    {
        return FindControlRecursive<PlaceHolder>(this, id);
    }

    public void reset_control_add_edit()
    {
        try
        {
            Label1.Text = null;
            txt_so_seri.Text = ""; txt_name.Text = ""; txt_model.Text = ""; txt_thongso.Text = ""; txt_ghichu.Text = "";
            txt_giaban.Text = "0";
            txt_gianhap.Text = "0";
            txt_ngaytao.Text = DateTime.Today.ToString("yyyy-MM-dd");
            check_hangthanhly.Checked = false;
            txt_phantram_thanhly.Text = "100";
            txt_thang_baohanh.Text = "";
            txt_han_baohanh.Text = "";
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
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk))
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
            reset_control_add_edit();

            ViewState["add_edit"] = "add";
            ViewState["quick_entry"] = false;
            pn_add.CssClass = "";
            GetQuickPlaceHolder("ph_quick_entry_info").Visible = false;
            GetQuickPlaceHolder("ph_quick_entry_quantity").Visible = false;
            GetQuickPlaceHolder("ph_quick_entry_common_start").Visible = false;
            Label1.Text = "THÊM SẢN PHẨM MỚI";
            but_add_edit.Text = "THÊM MỚI";

            using (dbDataContext db = new dbDataContext())
            {
                BindProductSourceDropDowns(db, null);
            }
            pn_add.Visible = !pn_add.Visible;
            up_add.Update();
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk))
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    protected void but_open_quick_entry_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("9", "9");
            TextBox quickBarcode = GetQuickTextBox("txt_quick_barcode");
            string barcode = quickBarcode == null ? "" : quickBarcode.Text.Trim();
            if (string.IsNullOrEmpty(barcode)) return;

            using (dbDataContext db = new dbDataContext())
            {
                reset_control_add_edit();
                BindProductSourceDropDowns(db, null);
            }

            ViewState["add_edit"] = "add";
            ViewState["quick_entry"] = true;
            pn_add.CssClass = "quick-entry-panel";
            GetQuickPlaceHolder("ph_quick_entry_info").Visible = true;
            GetQuickPlaceHolder("ph_quick_entry_quantity").Visible = true;
            GetQuickPlaceHolder("ph_quick_entry_common_start").Visible = true;
            Label1.Text = "NHẬP NHANH SẢN PHẨM BẰNG BARCODE";
            but_add_edit.Text = "XÁC NHẬN TẠO SẢN PHẨM";
            txt_so_seri.Text = barcode;
            GetQuickTextBox("txt_quick_quantity").Text = "1";
            GetQuickTextBox("txt_quick_date").Text = DateTime.Today.ToString("yyyy-MM-dd");
            pn_add.Visible = true;
            up_add.Update();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "quick_serial_preview", "window.setTimeout(function(){ if(window.updateQuickSerialPreview) updateQuickSerialPreview(); }, 80);", true);
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            Log_cl.Add_Log(_ex.Message, string.IsNullOrEmpty(_tk) ? "" : mahoa_cl.giaima_Bcorn(_tk), _ex.StackTrace);
        }
    }

    protected void but_close_form_add_Click(object sender, EventArgs e)
    {
        try
        {
            reset_control_add_edit();
            pn_add.CssClass = "";
            GetQuickPlaceHolder("ph_quick_entry_info").Visible = false;
            GetQuickPlaceHolder("ph_quick_entry_quantity").Visible = false;
            GetQuickPlaceHolder("ph_quick_entry_common_start").Visible = false;
            pn_add.Visible = !pn_add.Visible;
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk))
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            long? editId = TryGetId(Convert.ToString(ViewState["id_edit"]));
            var q = db.KhoSanPham_tbs.FirstOrDefault(p => p.id == editId);
            if (q != null)
            {
                KhoSanPham_tb _ob = q;
                File_Folder_cl.del_file(_ob.anh);
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

            long? productId = TryGetId(_id);
            var q = db.KhoSanPham_tbs.FirstOrDefault(p => p.id == productId);
            if (q != null)
            {
                ViewState["id_edit"] = _id;

                BindProductSourceDropDowns(db, q);

                txt_so_seri.Text = q.so_seri;
                txt_name.Text = q.ten;
                txt_link_fileupload.Text = q.anh;
                txt_model.Text = q.model;
                txt_thongso.Text = q.thongso_kythuat;
                txt_gianhap.Text = (q.gianhap ?? 0).ToString("#,##0").Replace(",", ".");
                txt_giaban.Text = (q.giabanle ?? 0).ToString("#,##0").Replace(",", ".");
                txt_ngaytao.Text = q.ngaytao.HasValue ? q.ngaytao.Value.ToString("yyyy-MM-dd") : DateTime.Today.ToString("yyyy-MM-dd");

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
                bool _cohoadon = q.cohoadon ?? false;
                if (_cohoadon == true)
                {
                    rbCoHoaDon.Checked = true; rbKhongCoHoaDon.Checked = false;
                }
                else
                {
                    rbCoHoaDon.Checked = false; rbKhongCoHoaDon.Checked = true;
                }
                bool _hangthanhly = q.hangthanhly ?? false;
                check_hangthanhly.Checked = _hangthanhly;
                txt_phantram_thanhly.Text = (q.phantram_thanhly ?? 100).ToString();

                if (q.han_baohanh.HasValue)
                {
                    txt_han_baohanh.Text = q.han_baohanh.Value.ToString("yyyy-MM-dd");
                    if (q.ngaytao.HasValue)
                    {
                        int diffMonths = ((q.han_baohanh.Value.Year - q.ngaytao.Value.Year) * 12) + q.han_baohanh.Value.Month - q.ngaytao.Value.Month;
                        if (diffMonths > 0)
                            txt_thang_baohanh.Text = diffMonths.ToString();
                        else
                            txt_thang_baohanh.Text = "";
                    }
                    else
                    {
                        txt_thang_baohanh.Text = "";
                    }
                }
                else
                {
                    txt_han_baohanh.Text = "";
                    txt_thang_baohanh.Text = "";
                }

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
            reset_control_add_edit();
            pn_add.Visible = !pn_add.Visible;
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk))
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
            if (!Directory.Exists(Server.MapPath("~/uploads/img-handler/"))) Directory.CreateDirectory(Server.MapPath("~/uploads/img-handler/"));
            string _so_seri = txt_so_seri.Text.Trim();
            string _tensp = txt_name.Text.Trim();
            string _anh = txt_link_fileupload.Text;
            bool _cohoadon = true;
            if (rbKhongCoHoaDon.Checked == true)
                _cohoadon = false;
            bool _hangthanhly = false;
            if (check_hangthanhly.Checked)
                _hangthanhly = true;
            long _phantram_thanhly = Number_cl.Check_Int64(txt_phantram_thanhly.Text.Trim());
            if (_phantram_thanhly <= 0) _phantram_thanhly = 100;

            string _id_hang = DropDownList1.SelectedValue;
            string _id_nhom = DropDownList2.SelectedValue;
            string _id_donvitinh = DropDownList3.SelectedValue;
            string _model = txt_model.Text.Trim().ToUpper();
            string _thongso = txt_thongso.Text;
            string _ghichu = txt_ghichu.Text;

            DateTime _ngaytao = DateTime.Now;
            DateTime dtNgayTao;
            if (DateTime.TryParseExact(txt_ngaytao.Text.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtNgayTao))
                _ngaytao = dtNgayTao;
            else if (DateTime.TryParse(txt_ngaytao.Text.Trim(), out dtNgayTao))
                _ngaytao = dtNgayTao;

            DateTime? _han_baohanh = null;
            DateTime dtHan;
            int _thang_bh = Number_cl.Check_Int(txt_thang_baohanh.Text.Trim());
            if (_thang_bh > 0)
            {
                _han_baohanh = _ngaytao.AddMonths(_thang_bh);
            }
            else if (DateTime.TryParseExact(txt_han_baohanh.Text.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtHan))
            {
                _han_baohanh = dtHan;
            }
            else if (DateTime.TryParse(txt_han_baohanh.Text.Trim(), out dtHan))
            {
                _han_baohanh = dtHan;
            }

            string _nguoitao = mahoa_cl.giaima_Bcorn(Session["taikhoan"].ToString());
            Int64 _giaban = Number_cl.Check_Int64(txt_giaban.Text.Trim());
            Int64 _gianhap = Number_cl.Check_Int64(txt_gianhap.Text.Trim());
            #endregion

            using (dbDataContext db = new dbDataContext())
            {
                if (Convert.ToBoolean(ViewState["quick_entry"]))
                {
                    SaveQuickProducts(db, _so_seri, _tensp, _anh, _cohoadon, _hangthanhly, _phantram_thanhly, _han_baohanh, _id_hang, _id_nhom, _id_donvitinh, _model, _thongso, _ghichu, _giaban, _gianhap, _nguoitao);
                    return;
                }

                #region Kiểm tra ngoại lệ.
                if (_tensp == "")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập tên sản phẩm.", "false", "false", "OK", "alert", ""), true);
                    return;
                }
                #endregion

                if (ViewState["add_edit"].ToString() == "add")
                {
                    if (!string.IsNullOrEmpty(_so_seri))
                    {
                        var q_seri = db.KhoSanPham_tbs.FirstOrDefault(p => p.so_seri == _so_seri);
                        if (q_seri != null)
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Số seri này đã tồn tại.", "false", "false", "OK", "alert", ""), true);
                            return;
                        }
                    }
                    #region thêm mới
                    KhoSanPham_tb _ob = new KhoSanPham_tb();
                    _ob.sanpham_tuychon = false;
                    _ob.so_seri = _so_seri;
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
                    _ob.phantram_thanhly = _phantram_thanhly;
                    _ob.han_baohanh = _han_baohanh;
                    _ob.ghichu = _ghichu;
                    _ob.ngaytao = _ngaytao;
                    _ob.nguoitao = _nguoitao;
                    _ob.soluong_hientai = string.IsNullOrEmpty(_so_seri) ? 0 : 1;
                    _ob.daban = false;
                    db.KhoSanPham_tbs.InsertOnSubmit(_ob);
                    db.SubmitChanges();
                    #endregion
                    #region cập nhật dữ liệu và update hiển thị
                    txt_so_seri.Text = ""; txt_name.Text = ""; txt_model.Text = ""; txt_thongso.Text = ""; txt_giaban.Text = "0"; txt_gianhap.Text = "0"; txt_ghichu.Text = ""; txt_link_fileupload.Text = "";
                    txt_ngaytao.Text = DateTime.Today.ToString("yyyy-MM-dd");
                    check_hangthanhly.Checked = false;
                    txt_phantram_thanhly.Text = "100";
                    txt_thang_baohanh.Text = "";
                    txt_han_baohanh.Text = "";
                    DropDownList1.SelectedIndex = 0; DropDownList2.SelectedIndex = 0; DropDownList3.SelectedIndex = 0;
                    show_main();
                    up_main.Update();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
                    #endregion
                }
                else//edit
                {
                    #region chuẩn bị dữ liệu
                    long? editId = TryGetId(Convert.ToString(ViewState["id_edit"]));
                    var q_edit = db.KhoSanPham_tbs.FirstOrDefault(p => p.id == editId);
                    if (q_edit != null)
                    {
                        if (!string.IsNullOrEmpty(_so_seri))
                        {
                            var q_seri = db.KhoSanPham_tbs.FirstOrDefault(p => p.so_seri == _so_seri && p.id != editId);
                            if (q_seri != null)
                            {
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Số seri này đã tồn tại.", "false", "false", "OK", "alert", ""), true);
                                return;
                            }
                        }

                        #region kiểm tra ngoại lệ. sau đó cập nhật
                        KhoSanPham_tb _ob = q_edit;
                        _ob.so_seri = _so_seri;
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
                        _ob.phantram_thanhly = _phantram_thanhly;
                        _ob.han_baohanh = _han_baohanh;
                        _ob.ghichu = _ghichu;
                        _ob.ngaytao = _ngaytao;
                        db.SubmitChanges();

                        #region cập nhật dữ liệu và update hiển thị
                        show_main();
                        up_main.Update();
                        reset_control_add_edit();
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
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk))
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    private void SaveQuickProducts(dbDataContext db, string baseSerial, string name, string image, bool coHoaDon, bool hangThanhLy,
        long phanTramThanhLy, DateTime? hanBaoHanh, string idHang, string idNhom, string idDonViTinh, string model, string thongSo, string ghiChu,
        long giaBan, long giaNhap, string nguoiTao)
    {
        int quantity;
        if (string.IsNullOrWhiteSpace(baseSerial) || string.IsNullOrWhiteSpace(name))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                thongbao_class.metro_dialog("Thiếu thông tin", "Vui lòng nhập số seri và tên sản phẩm.", "false", "false", "OK", "alert", ""), true);
            return;
        }

        TextBox quickQuantity = GetQuickTextBox("txt_quick_quantity");
        TextBox quickDate = GetQuickTextBox("txt_quick_date");
        if (quickQuantity == null || quickDate == null || !int.TryParse(quickQuantity.Text.Trim(), out quantity) || quantity < 1 || quantity > 1000)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                thongbao_class.metro_dialog("Số lượng không hợp lệ", "Số lượng sản phẩm muốn tạo phải từ 1 đến 1.000.", "false", "false", "OK", "alert", ""), true);
            return;
        }

        long baseNumber;
        if (!long.TryParse(baseSerial, NumberStyles.None, CultureInfo.InvariantCulture, out baseNumber))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                thongbao_class.metro_dialog("Barcode không hợp lệ", "Mã seri phải là chuỗi số để có thể tăng tuần tự.", "false", "false", "OK", "alert", ""), true);
            return;
        }

        DateTime ngayNhap;
        if (!DateTime.TryParseExact(quickDate.Text.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out ngayNhap))
            ngayNhap = DateTime.Today;

        int serialWidth = baseSerial.Length;
        var serials = Enumerable.Range(1, quantity)
            .Select(i =>
            {
                long serialNumber;
                try
                {
                    serialNumber = checked(baseNumber + i - 1L);
                }
                catch (OverflowException)
                {
                    return null;
                }

                string serial = serialNumber.ToString(CultureInfo.InvariantCulture);
                return serial.Length < serialWidth ? serial.PadLeft(serialWidth, '0') : serial;
            })
            .ToList();

        if (serials.Any(p => p == null))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                thongbao_class.metro_dialog("Số seri không hợp lệ", "Dãy seri vượt quá giới hạn số cho phép.", "false", "false", "OK", "alert", ""), true);
            return;
        }

        var duplicateSerials = db.KhoSanPham_tbs
            .Where(p => serials.Contains(p.so_seri))
            .Select(p => p.so_seri)
            .Distinct()
            .ToList();
        var duplicateSet = new HashSet<string>(duplicateSerials, StringComparer.Ordinal);
        var newSerials = serials.Where(p => !duplicateSet.Contains(p)).ToList();

        if (newSerials.Count == 0)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                thongbao_class.metro_dialog("Không thể tạo sản phẩm", "Tất cả " + quantity.ToString("#,##0") + " số seri đã có trong kho: " + string.Join(", ", duplicateSerials), "false", "false", "OK", "alert", ""), true);
            return;
        }

        foreach (string serial in newSerials)
        {
            KhoSanPham_tb product = new KhoSanPham_tb();
            product.sanpham_tuychon = false;
            product.so_seri = serial;
            product.ten = name;
            product.id_nhom = idNhom;
            product.id_hang = idHang;
            product.donvitinh = idDonViTinh;
            product.anh = image;
            product.model = model;
            product.thongso_kythuat = thongSo;
            product.giabanle = giaBan;
            product.gianhap = giaNhap;
            product.cohoadon = coHoaDon;
            product.hangthanhly = hangThanhLy;
            product.phantram_thanhly = phanTramThanhLy;
            product.han_baohanh = hanBaoHanh;
            product.ghichu = ghiChu;
            product.ngaytao = ngayNhap;
            product.nguoitao = nguoiTao;
            product.soluong_hientai = 1;
            product.daban = false;
            db.KhoSanPham_tbs.InsertOnSubmit(product);
        }
        db.SubmitChanges();

        reset_control_add_edit();
        ViewState["quick_entry"] = false;
        pn_add.CssClass = "";
        pn_add.Visible = false;
        GetQuickPlaceHolder("ph_quick_entry_info").Visible = false;
        GetQuickPlaceHolder("ph_quick_entry_quantity").Visible = false;
        GetQuickPlaceHolder("ph_quick_entry_common_start").Visible = false;
        show_main();
        up_main.Update();
        up_add.Update();
        string resultMessage = "Đã tạo " + newSerials.Count.ToString("#,##0") + " sản phẩm.";
        if (duplicateSerials.Count > 0)
        {
            resultMessage += "<br/><br/>Bỏ qua " + duplicateSerials.Count.ToString("#,##0") + " seri đã có trong kho: " + string.Join(", ", duplicateSerials.Take(20));
            if (duplicateSerials.Count > 20)
                resultMessage += "...";
        }
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
            thongbao_class.metro_dialog("Nhập kho thành công", resultMessage, "false", "false", "OK", "alert", ""), true);
    }
    #endregion

    #region Xuất excel
    protected void but_show_form_xuat_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            pn_xuat.Visible = !pn_xuat.Visible;

            check_list_page.Items.Clear();
            for (int i = 1; i <= int.Parse(ViewState["total_page"].ToString()); i++)
            {
                ListItem item = new ListItem($"Trang {i}", i.ToString());
                check_list_page.Items.Add(item);
                item.Selected = true;
            }

            up_xuat.Update();
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk))
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
                    break;
                }
            }
            if (!_chonmuc)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Không có mục nào được chọn.", "false", "false", "OK", "alert", ""), true);
                return;
            }

            List<ListItem> selectedPage = new List<ListItem>();
            foreach (ListItem item in check_list_page.Items)
            {
                if (item.Selected)
                {
                    selectedPage.Add(item);
                    _chonPage = true;
                }
            }
            if (!_chonPage)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Không có trang nào được chọn.", "false", "false", "OK", "alert", ""), true);
                return;
            }

            if (!Directory.Exists(Server.MapPath("~/uploads/files/")))
                Directory.CreateDirectory(Server.MapPath("~/uploads/files/"));
            using (dbDataContext db = new dbDataContext())
            {
                db.ObjectTrackingEnabled = false;
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
                                    ob1.so_seri,
                                    ob1.model,
                                    ob1.thongso_kythuat,
                                    ob1.gianhap,
                                    TongGiaNhap = ob1.gianhap * ob1.soluong_hientai,
                                    ob1.giabanle,
                                    TongBanLe = ob1.giabanle * ob1.soluong_hientai,
                                    ob1.cohoadon,
                                    ob1.hangthanhly,
                                    ob1.phantram_thanhly,
                                    ob1.han_baohanh,
                                    ob1.soluong_hientai,
                                    ob1.ghichu,
                                    ob1.ngaytao,
                                    ob1.nguoitao,
                                }).AsQueryable();

                string _key = txt_timkiem.Text.Trim();
                if (!string.IsNullOrEmpty(_key))
                {
                    long searchId;
                    bool hasSearchId = long.TryParse(_key, out searchId);
                    list_all = list_all.Where(p => p.TenSP.Contains(_key) || (hasSearchId && p.id == searchId));
                }
                else
                {
                    string _key1 = txt_timkiem1.Text.Trim();
                    if (!string.IsNullOrEmpty(_key1))
                    {
                        long searchId1;
                        bool hasSearchId1 = long.TryParse(_key1, out searchId1);
                        list_all = list_all.Where(p =>
                            p.TenSP.Contains(_key1) ||
                            p.Hang.Contains(_key1) ||
                            p.Nhom.Contains(_key1) ||
                            p.model == _key1 ||
                            (hasSearchId1 && p.id == searchId1));
                    }
                }

                list_all = list_all.OrderBy(p => p.Nhom).ThenBy(p => p.TenSP);
                var exportData = list_all.ToList();
                int _Tong_Record = exportData.Count;
                #endregion

                #region xuất vào excel
                using (ExcelPackage package = new ExcelPackage())
                {
                    int _cot = 1;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet 1");
                    foreach (ListItem item in check_list_excel.Items)
                    {
                        if (item.Selected)
                        {
                            worksheet.Cells[1, _cot].Value = item.Text;
                            _cot = _cot + 1;
                        }
                    }
                    _cot = 1;
                    int _row = 2;

                    #region xác định dữ liệu chuẩn bị xuất
                    IEnumerable<dynamic> list_xuat;
                    if (check_all_page.Checked == true)
                        list_xuat = exportData;
                    else
                    {
                        List<dynamic> list_split = new List<dynamic>();
                        foreach (ListItem selectedItem in selectedPage)
                        {
                            int pageNumber = int.Parse(selectedItem.Value);
                            int itemsPerPage = Number_cl.Check_Int(txt_show.Text.Trim());
                            int startIndex = (pageNumber - 1) * itemsPerPage;
                            int endIndex = startIndex + itemsPerPage;
                            var pageData = exportData.Skip(startIndex).Take(itemsPerPage);
                            list_split.AddRange(pageData);
                        }
                        list_xuat = list_split;
                    }
                    #endregion

                    foreach (var t in list_xuat)
                    {
                        _cot = 1;
                        foreach (ListItem item in check_list_excel.Items.Cast<ListItem>().Where(item => item.Selected))
                        {
                            string _tencot = item.Value;
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
                                    DateTime? ngayTao = t.ngaytao;
                                    if (ngayTao.HasValue)
                                    {
                                        DateTime onlyDate = ngayTao.Value.Date;
                                        worksheet.Cells[_row, _cot].Value = onlyDate;
                                        worksheet.Cells[_row, _cot].Style.Numberformat.Format = "dd/MM/yyyy";
                                    }
                                    else
                                    {
                                        worksheet.Cells[_row, _cot].Value = DBNull.Value;
                                    }
                                    _cot = _cot + 1;
                                    break;
                                case "nguoitao":
                                    worksheet.Cells[_row, _cot].Value = t.nguoitao; _cot = _cot + 1;
                                    break;
                                case "thongso_kythuat":
                                    worksheet.Cells[_row, _cot].Value = t.thongso_kythuat; _cot = _cot + 1;
                                    break;
                                case "gianhap":
                                    worksheet.Cells[_row, _cot].Value = t.gianhap; _cot = _cot + 1;
                                    break;
                            }
                        }
                        _row++;
                    }

                    string fileName = "KhoSanPham_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    string filePath = Server.MapPath("~/uploads/files/" + fileName);
                    FileInfo file = new FileInfo(filePath);
                    package.SaveAs(file);

                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
                    Response.TransmitFile(filePath);
                    Response.Flush();
                    Response.End();
                }
                #endregion
            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk))
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
            bool isChecked = check_all_excel.Checked;
            foreach (ListItem item in check_list_excel.Items)
            {
                item.Selected = isChecked;
            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk))
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
            bool allSelected = true;
            foreach (ListItem item in check_list_excel.Items)
            {
                if (!item.Selected)
                {
                    allSelected = false;
                    break;
                }
            }
            check_all_excel.Checked = allSelected;
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk))
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
            bool isChecked = check_all_page.Checked;
            foreach (ListItem item in check_list_page.Items)
            {
                item.Selected = isChecked;
            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk))
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
            bool allSelected = true;
            foreach (ListItem item in check_list_page.Items)
            {
                if (!item.Selected)
                {
                    allSelected = false;
                    break;
                }
            }
            check_all_page.Checked = allSelected;
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk))
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
        txt_tungay.Text = dt_cl.return_ngaydautuan().AddDays(-7).ToShortDateString();
        txt_denngay.Text = dt_cl.return_ngaydautuan().AddDays(-1).ToShortDateString();
    }
    protected void but_tuannay_Click(object sender, EventArgs e)
    {
        ResetButtonCss(); but_tuannay.CssClass = "small mt-1 info";
        txt_tungay.Text = dt_cl.return_ngaydautuan().ToShortDateString();
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
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk))
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
            if (Request.Cookies["cookie_qlkho"] != null)
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
                if (_chon_phanloai.Contains(""))
                    _ck["phanloai"] = "";
                else
                    _ck["phanloai"] = string.Join(",", _chon_phanloai);
                #endregion
                _ck.Expires = DateTime.Now.AddDays(1);
                Response.Cookies.Set(_ck);
            }
            show_main();
            up_main.Update();
            pn_loc.Visible = false;
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk))
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
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk))
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
            var selectedIds = new List<Int64>();

            foreach (RepeaterItem item in Repeater1.Items)
            {
                CheckBox chkItem = (CheckBox)item.FindControl("checkID");
                Label lblData = (Label)item.FindControl("lbID");

                if (chkItem != null && lblData != null && chkItem.Checked)
                {
                    int id = int.Parse(lblData.Text);
                    selectedIds.Add(id);
                }
            }

            if (selectedIds.Count > 0)
            {
                using (dbDataContext db = new dbDataContext())
                {
                    var ListsToUpdate = db.KhoSanPham_tbs
                        .Where(d => selectedIds.Contains(d.id))
                        .ToList();

                    foreach (var dm in ListsToUpdate)
                    {
                        db.KhoSanPham_tbs.DeleteOnSubmit(dm);
                    }
                    db.SubmitChanges();
                }

                show_main();
                up_main.Update();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Không có mục nào được chọn.", "false", "false", "OK", "alert", ""), true);
            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk))
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    protected void but_sao_chep_Click(object sender, EventArgs e)
    {
        try
        {
            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;
            using (dbDataContext db = new dbDataContext())
            {
                var q = db.KhoSanPham_tbs.FirstOrDefault(p => p.id.ToString() == _id);
                if (q != null)
                {
                    KhoSanPham_tb _ob = new KhoSanPham_tb();
                    _ob.sanpham_tuychon = q.sanpham_tuychon;
                    _ob.so_seri = q.so_seri + " (copy)";
                    _ob.ten = q.ten;
                    _ob.id_nhom = q.id_nhom;
                    _ob.id_hang = q.id_hang;
                    _ob.donvitinh = q.donvitinh;
                    _ob.anh = q.anh;
                    _ob.model = q.model;
                    _ob.thongso_kythuat = q.thongso_kythuat;
                    _ob.giabanle = q.giabanle;
                    _ob.gianhap = q.gianhap;
                    _ob.cohoadon = q.cohoadon;
                    _ob.hangthanhly = q.hangthanhly;
                    _ob.phantram_thanhly = q.phantram_thanhly;
                    _ob.han_baohanh = q.han_baohanh;
                    _ob.ghichu = q.ghichu;
                    _ob.ngaytao = q.ngaytao;
                    _ob.nguoitao = q.nguoitao;
                    _ob.soluong_hientai = q.soluong_hientai;

                    db.KhoSanPham_tbs.InsertOnSubmit(_ob);
                    db.SubmitChanges();

                    show_main();
                    up_main.Update();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Sao chép thành công.", "1000", "success"), true);
                }
            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk))
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    protected void but_xoa_item_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("11", "11");
            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;
            using (dbDataContext db = new dbDataContext())
            {
                var q = db.KhoSanPham_tbs.FirstOrDefault(p => p.id.ToString() == _id);
                if (q != null)
                {
                    db.KhoSanPham_tbs.DeleteOnSubmit(q);
                    db.SubmitChanges();
                    show_main();
                    up_main.Update();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
                }
            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk))
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
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
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk))
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
        using (dbDataContext db = new dbDataContext())
        {
            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;
            long? productId = TryGetId(_id);
            var q = db.KhoSanPham_tbs.FirstOrDefault(p => p.id == productId);
            if (q != null)
            {
                ViewState["id_edit"] = _id;
                Label3.Text = q.ten;
                Label4.Text = q.soluong_hientai.Value.ToString("#,##0");
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

    protected void but_show_form_chinhsuasoluong_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_admin("12", "12");
        using (dbDataContext db = new dbDataContext())
        {
            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;
            long? productId = TryGetId(_id);
            var q = db.KhoSanPham_tbs.FirstOrDefault(p => p.id == productId);
            if (q != null)
            {
                ViewState["id_edit_soluong"] = _id;
                Label_ten_chinhsuasoluong.Text = q.ten;
                txt_chinhsuasoluong.Text = q.soluong_hientai.HasValue ? q.soluong_hientai.Value.ToString("#,##0") : "0";
            }
            else
                ViewState["id_edit_soluong"] = null;
        }
        pn_chinhsuasoluong.Visible = true;
        up_chinhsuasoluong.Update();
    }

    protected void but_close_form_chinhsuasoluong_Click(object sender, EventArgs e)
    {
        pn_chinhsuasoluong.Visible = false;
        up_chinhsuasoluong.Update();
    }

    protected void but_luu_chinhsuasoluong_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_admin("12", "12");
        if (ViewState["id_edit_soluong"] != null)
        {
            try
            {
                using (dbDataContext db = new dbDataContext())
                {
                    long? productId = TryGetId(ViewState["id_edit_soluong"].ToString());
                    var q = db.KhoSanPham_tbs.FirstOrDefault(p => p.id == productId);
                    if (q != null)
                    {
                        int _new_soluong = Number_cl.Check_Int(txt_chinhsuasoluong.Text.Trim());
                        q.soluong_hientai = _new_soluong;
                        q.daban = _new_soluong <= 0;
                        db.SubmitChanges();
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Cập nhật số lượng thành công.", "1000", "success"), true);
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Lỗi", "Có lỗi xảy ra: " + ex.Message, "false", "false", "OK", "alert", ""), true);
            }
        }
        pn_chinhsuasoluong.Visible = false;
        up_chinhsuasoluong.Update();
        show_main();
        up_main.Update();
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
                long? editId = TryGetId(Convert.ToString(ViewState["id_edit"]));
                var q = db.KhoSanPham_tbs.FirstOrDefault(p => p.id == editId);
                if (q != null)
                {
                    NhapXuatKho_tb _ob = new NhapXuatKho_tb();
                    _ob.nhap_hay_xuat = true;
                    _ob.id_sanpham = ViewState["id_edit"].ToString();
                    _ob.ten_sanpham = q.ten;
                    _ob.soluong_nhap = _soluongnhap;
                    _ob.gia_nhap = _gianhap;
                    if (_gianhap != q.gianhap)
                        q.gianhap = _gianhap;
                    _ob.ngaynhap = _ngaynhap;
                    _ob.nguoinhap = _nguoinhap;
                    _ob.ton_hientai = q.soluong_hientai;
                    q.soluong_hientai = q.soluong_hientai + _soluongnhap;
                    q.daban = false;
                    db.NhapXuatKho_tbs.InsertOnSubmit(_ob);
                    db.SubmitChanges();
                    show_main();
                    up_main.Update();

                    reset_control_nhaphang();
                    pn_nhaphang.Visible = !pn_nhaphang.Visible;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
                }
            }
        }
        catch (Exception _ex)
        {
            Response.Redirect("/admin");
        }
    }
    #endregion

    protected void txt_so_seri_TextChanged(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("12", "12");
            TextBox txt = (TextBox)sender;
            RepeaterItem item = (RepeaterItem)txt.NamingContainer;
            Label lbID = (Label)item.FindControl("lbID");
            long id = long.Parse(lbID.Text);
            string new_seri = txt.Text.Trim();

            using (dbDataContext db = new dbDataContext())
            {
                if (!string.IsNullOrEmpty(new_seri))
                {
                    var q_seri = db.KhoSanPham_tbs.FirstOrDefault(p => p.so_seri == new_seri && p.id != id);
                    if (q_seri != null)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Số seri này đã tồn tại ở sản phẩm khác.", "false", "false", "OK", "alert", ""), true);
                        show_main();
                        up_main.Update();
                        return;
                    }
                }

                var q = db.KhoSanPham_tbs.FirstOrDefault(p => p.id == id);
                if (q != null)
                {
                    q.so_seri = new_seri;
                    db.SubmitChanges();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Cập nhật số seri thành công.", "1000", "warning"), true);
                    show_main();
                    up_main.Update();
                }
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Lỗi", "Có lỗi xảy ra: " + ex.Message, "false", "false", "OK", "alert", ""), true);
        }
    }

    protected void but_save_Click(object sender, EventArgs e)
    {
    }

    protected void but_luu_Click(object sender, EventArgs e)
    {
    }

    protected void but_toggle_sold_products_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            bool showSold = ViewState["show_sold"] != null && (bool)ViewState["show_sold"];
            ViewState["show_sold"] = !showSold;
            ViewState["current_page_qlkho"] = 1;
            show_main();
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk))
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    protected void but_show_import_excel_Click(object sender, EventArgs e)
    {
        pn_import_excel.Visible = true;
        up_import_excel.Update();
    }

    protected void but_close_import_excel_Click(object sender, EventArgs e)
    {
        pn_import_excel.Visible = false;
        up_import_excel.Update();
    }

    protected void but_confirm_import_excel_Click(object sender, EventArgs e)
    {
    }
}
