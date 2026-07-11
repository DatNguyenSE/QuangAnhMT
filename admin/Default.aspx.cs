using System;
using System.Collections.Generic;
using System.Linq;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;



public partial class admin_Default : System.Web.UI.Page
{
    public string notifi = "";
    public void Ham_DiemDanh(string lat, string lon)
    {
        try
        {
            if (!string.IsNullOrEmpty(lat) && !string.IsNullOrEmpty(lon))
            {
                using (dbDataContext db = new dbDataContext())
                {
                    DateTime _dt = DateTime.Now;

                    #region chấm công
                    string _tk = ViewState["taikhoan"].ToString();
                    var q_check = db.ChamCong_tbs.FirstOrDefault(p => p.taikhoan == _tk && p.ngaychamcong.Value.Date == _dt.Date);
                    if (q_check != null)
                    {
                        if (q_check.baoraca == null)//đã báo vào nhưng chưa báo ra
                        {
                            ViewState["check_diemdanh"] = "1"; // Đã báo vào nhưng CHƯA BÁO RA
                        }
                        else//đã báo ra ca
                        {
                            ViewState["check_diemdanh"] = "2"; // Đã báo vào và đã báo ra
                            Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Hôm nay bạn đã chấm công rồi.", "1000", "warning");
                            Response.Redirect(Request.Url.AbsoluteUri);
                        }

                    }
                    else
                    {
                        ViewState["check_diemdanh"] = "0"; // Chưa chấm công
                    }

                    int _khoangcach = 0;
                    if (ViewState["check_diemdanh"].ToString() == "0")//chưa chấm công thì thêm mới và báo vào ca
                    {
                        ChamCong_tb _ob = new ChamCong_tb();
                        _ob.ngaychamcong = _dt;
                        _ob.taikhoan = _tk;
                        _ob.vido = lat;
                        _ob.kinhdo = lon;

                        string _vido_congty = "", _kinhdo_congty = "";
                        var q = db.CaiDatChung_tbs.FirstOrDefault(p => p.phanloai_trang == "admin");
                        if (q != null)
                        {
                            _vido_congty = q.vido;
                            _kinhdo_congty = q.kinhdo;
                        }
                        _khoangcach = ViTri_cl.TinhKhoanCach(lat, lon, _vido_congty, _kinhdo_congty);
                        _ob.khoangcach = _khoangcach;

                        Int64 _LCB_hientai = 0, _LuongNgay = 0;
                        var q_tk = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == ViewState["taikhoan"].ToString());
                        if (q_tk != null)
                        {
                            _LCB_hientai = q_tk.LuongCoBan ?? 0;
                            _LuongNgay = _LCB_hientai / 26;
                        }
                        _ob.LCB_hientai = _LCB_hientai;
                        _ob.LuongNgay_ChamCong = _LuongNgay;

                        db.ChamCong_tbs.InsertOnSubmit(_ob);
                        db.SubmitChanges();
                        Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Báo vào ca thành công.", "1000", "warning");
                        Response.Redirect(Request.Url.AbsoluteUri);
                    }
                    else if (ViewState["check_diemdanh"].ToString() == "1")//chưa chấm công thì thêm mới và báo vào ca
                    {
                        q_check.baoraca = _dt;
                        q_check.vido_raca = lat;
                        q_check.kinhdo_raca = lon;

                        string _vido_congty = "", _kinhdo_congty = "";
                        var q = db.CaiDatChung_tbs.FirstOrDefault(p => p.phanloai_trang == "admin");
                        if (q != null)
                        {
                            _vido_congty = q.vido;
                            _kinhdo_congty = q.kinhdo;
                        }
                        _khoangcach = ViTri_cl.TinhKhoanCach(lat, lon, _vido_congty, _kinhdo_congty);
                        q_check.khoangcach_raca = _khoangcach;

                        db.SubmitChanges();
                        Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Báo ra ca thành công.", "1000", "warning");
                        Response.Redirect(Request.Url.AbsoluteUri);
                    }
                    else
                    {
                        Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Hôm nay bạn đã chấm công rồi.", "1000", "warning");
                        Response.Redirect(Request.Url.AbsoluteUri);
                    }
                    #endregion

                }
            }
            else
            {
                Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Vui lòng cấp quyền truy cập vị trí từ trình duyệt của bạn.", "1000", "warning");
                Response.Redirect(Request.Url.AbsoluteUri);
            }
        }
        catch (Exception _ex)
        {
            notifi = thongbao_class.metro_dialog_onload("Thông báo", _ex.Message, "false", "false", "OK", "alert", "");
        }

    }
    protected void but_diemdanh_Click(object sender, EventArgs e)
    {

    }
    protected void Page_Load(object sender, EventArgs e)
    {
        //Response.Write(Session["a"]);
        if (!IsPostBack)
        {

            Session["url_back"] = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
            check_login_cl.check_login_admin("none", "none");
            if (Session["title"] != null)
                ViewState["title"] = Session["title"].ToString();
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                ViewState["taikhoan"] = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                ViewState["taikhoan"] = "";

            using (dbDataContext db = new dbDataContext())
            {
                #region liên quan tới chấm công
                var q_check = db.ChamCong_tbs.FirstOrDefault(p => p.taikhoan == ViewState["taikhoan"].ToString() && p.ngaychamcong.Value.Date == DateTime.Now.Date);
                if (q_check != null)
                {
                    if (q_check.baoraca == null)//đã báo vào nhưng chưa báo ra
                    {
                        ViewState["check_diemdanh"] = "1"; // Đã báo vào nhưng CHƯA BÁO RA
                        but_diemdanh.Text = "Báo ra ca"; but_diemdanh.CssClass = "small warning rounded ml-20";
                        Label1.Text = "<div class='fg-green'><small>Vào ca: " + q_check.ngaychamcong.Value.ToString("HH:mm") + "'.</small></div>";
                    }
                    else//đã báo ra ca
                    {
                        ViewState["check_diemdanh"] = "2"; // Đã báo vào và đã báo ra
                        but_diemdanh.Visible = false;
                        Label1.Text = "<div class='fg-green'><small>Vào ca: <b>" + q_check.ngaychamcong.Value.ToString("HH:mm") + "</b> <span class='fg-orange pl-10'>Ra ca: <b>" + q_check.baoraca.Value.ToString("HH:mm") + "</b></span></small></div>";
                    }
                }
                else
                {
                    ViewState["check_diemdanh"] = "0"; // Chưa chấm công
                    Label1.Text = "<div class='fg-red'><small>Hôm nay bạn chưa chấm công.</small></div>";
                }

                //hiển thị ds hôm nay
                var q_homnay = from cc in db.ChamCong_tbs
                               join tk in db.taikhoan_tbs on cc.taikhoan equals tk.taikhoan
                               where cc.ngaychamcong.Value.Date == DateTime.Now.Date
                               select new
                               {
                                   cc.taikhoan,
                                   cc.ngaychamcong,
                                   tk.hoten,
                                   tk.anhdaidien,
                                   cc.baoraca,
                                   cc.vido,
                                   cc.kinhdo,
                                   cc.khoangcach,
                                   cc.vido_raca,
                                   cc.kinhdo_raca,
                                   cc.khoangcach_raca,
                               };

                Repeater1.DataSource = q_homnay.OrderBy(p => p.ngaychamcong).ToList();
                Repeater1.DataBind();
                #endregion
                load_congviec_chuahoanthanh(db, ViewState["taikhoan"].ToString());
                load_congviec_chuaainhan(db, ViewState["taikhoan"].ToString());
                load_baohanh_chuatra(db);
            }
        }
        else
        {
            if (ViewState["check_diemdanh"].ToString() == "0" || ViewState["check_diemdanh"].ToString() == "1")
            {
                string lat = ((HiddenField)Page.Master.FindControl("latitude")).Value;
                string lon = ((HiddenField)Page.Master.FindControl("longitude")).Value;

                if (!string.IsNullOrEmpty(lat) && !string.IsNullOrEmpty(lon))
                {
                    Ham_DiemDanh(lat, lon);
                }
            }

        }
    }

    #region CÔNG VIỆC
    public void load_congviec_chuahoanthanh(dbDataContext db, string _tk)
    {
        var list_all = (from ob1 in db.CongViec_tbs.Where(p => (p.nguoinhan_list.Contains(_tk)||p.nguoigiao== _tk) && p.trangthai == "Đã nhận")
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
        if (list_all.Any())
        {
            list_all = list_all.OrderByDescending(p => p.ngaygiao);
            Repeater2.DataSource = list_all;
            Repeater2.DataBind();
            PlaceHolder1.Visible = true;
        }
        else
        {
            PlaceHolder1.Visible = false;
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
                    load_congviec_chuahoanthanh(db,_tk);
                    UpdatePanel1.Update();
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
                    load_congviec_chuahoanthanh(db, _tk);
                    UpdatePanel1.Update();
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

                load_congviec_chuahoanthanh(db, _tk);
                UpdatePanel1.Update();

                reset_control_baohoanthanh();
                pn_baohoanthanh.Visible = !pn_baohoanthanh.Visible;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
            }
        }
    }
    #endregion
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
    public void load_congviec_chuaainhan(dbDataContext db, string _tk)
    {
        var list_all = (from ob1 in db.CongViec_tbs.Where(p =>  p.trangthai == "Đã giao")
                        join ob2 in db.taikhoan_tbs on ob1.nguoigiao equals ob2.taikhoan into TaiKhoan
                        from ob2 in TaiKhoan.DefaultIfEmpty()
                        select new
                        {
                            ob1.id,
                            ob1.TenCongViec,
                            ob1.Gap_KhongGap,
                            ob1.nguoinhan_list,

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
        if (list_all.Any())
        {
            list_all = list_all.OrderByDescending(p => p.ngaygiao);
            Repeater3.DataSource = list_all;
            Repeater3.DataBind();
            PlaceHolder6.Visible = true;
        }
        else
        {
            PlaceHolder6.Visible = false;
        }
    }
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
                    load_congviec_chuahoanthanh(db, _tk);
                    load_congviec_chuaainhan(db, _tk);
                    UpdatePanel1.Update();
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

                load_congviec_chuahoanthanh(db, _tk);
                load_congviec_chuaainhan(db, _tk);
                UpdatePanel1.Update();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);

            }
        }
    }
    #endregion

    #region BẢO HÀNH
    public void load_baohanh_chuatra(dbDataContext db)
    {
   
       
                #region lấy dữ liệu
                var base_phieu = db.HangBaoHanh_tbs
                                   .Where(p => p.trangthai != "Đã trả")
                                   .OrderBy(p => p.NgayHenKhachTra)
                                   .Take(50)
                                   .ToList();

                var phieu_ids = base_phieu.Select(p => p.id.ToString()).ToList();
                var chitiet = new List<HangBaoHanh_ChiTiet_tb>();
                if (phieu_ids.Count > 0)
                {
                    chitiet = db.HangBaoHanh_ChiTiet_tbs
                                    .Where(c => phieu_ids.Contains(c.id_PhieuBaoHanh))
                                    .ToList();
                }
                var taikhoans = db.taikhoan_tbs.ToList();

                var list_all = (from ob1 in base_phieu
                                join tk in taikhoans on ob1.nguoitao equals tk.taikhoan into TaiKhoanGroup
                                from tk in TaiKhoanGroup.DefaultIfEmpty()
                                let chiTietGroup = chitiet.Where(c => c.id_PhieuBaoHanh == ob1.id.ToString())
                                select new
                                {
                                    ob1.id,
                                    ob1.ngaytao,
                                    HoTenNhanVien = tk == null ? "" : tk.hoten,
                                    ob1.sdt_khachhang,
                                    ob1.ten_khachhang,
                                    ob1.diachi_khachhang,
                                    TongTien = chiTietGroup.Any() ? chiTietGroup.Sum(ct => (Int64?)ct.thanhtien) ?? 0 : 0,
                                    TongGiam = chiTietGroup.Any() ? chiTietGroup.Sum(ct => (Int64?)ct.giamgia_thanhtien) ?? 0 : 0,
                                    TongSauGiam = chiTietGroup.Any() ? chiTietGroup.Sum(ct => (Int64?)ct.TongSauGiam) ?? 0 : 0,
                                    congno = ob1.congno ?? 0,
                                    ob1.ghichu,
                                    ob1.NgayHenKhachTra,
                                    ob1.ngaynhan,
                                    ob1.NgayTra_ThucTe,
                                    ob1.trangthai,
                                    ob1.trehen
                                }).ToList();

                #endregion

                Repeater4.DataSource = list_all;
                Repeater4.DataBind();
            

    }
    #endregion



}