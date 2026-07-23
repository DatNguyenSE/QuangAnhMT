using System;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

public partial class admin_thong_ke_ban_hang : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["url_back"] = HttpContext.Current.Request.Url.AbsoluteUri;
            check_login_cl.check_login_admin("40", "40");

            string tk = Session["taikhoan"] as string;
            ViewState["taikhoan"] = string.IsNullOrEmpty(tk) ? "" : mahoa_cl.giaima_Bcorn(tk);

            LoadNhanVien();
            SetDefaultDate();
            LoadThongKe();
        }
    }

    private void SetDefaultDate()
    {
        DateTime now = DateTime.Now;
        txt_tungay.Text = new DateTime(now.Year, now.Month, 1).ToString("dd/MM/yyyy");
        txt_denngay.Text = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month)).ToString("dd/MM/yyyy");
    }

    private void LoadNhanVien()
    {
        using (dbDataContext db = new dbDataContext())
        {
            var q = db.taikhoan_tbs
                .Select(x => new { x.taikhoan, x.hoten })
                .ToList()
                .Select(x => new { x.taikhoan, HoTen = string.IsNullOrEmpty(x.hoten) ? x.taikhoan : x.hoten })
                .OrderBy(x => x.HoTen)
                .ToList();

            ddl_nhanvien.DataSource = q;
            ddl_nhanvien.DataValueField = "taikhoan";
            ddl_nhanvien.DataTextField = "HoTen";
            ddl_nhanvien.DataBind();
            ddl_nhanvien.Items.Insert(0, new ListItem("Tất cả", ""));
        }
    }

    protected void but_thongke_Click(object sender, EventArgs e)
    {
        LoadThongKe();
    }

    protected void but_reset_Click(object sender, EventArgs e)
    {
        SetDefaultDate();
        ddl_loai_ngay.SelectedValue = "ban";
        ddl_trangthai.SelectedValue = "daban";
        ddl_thanhtoan.SelectedValue = "all";
        ddl_nhanvien.SelectedValue = "";
        txt_timkiem.Text = "";
        txt_sanpham.Text = "";
        LoadThongKe();
    }

    private void LoadThongKe()
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                DateTime? tuNgay = ParseDate(txt_tungay.Text);
                DateTime? denNgay = ParseDate(txt_denngay.Text);
                DateTime? tuNgayLoc = tuNgay.HasValue ? (DateTime?)tuNgay.Value.Date : null;
                DateTime? denNgayLoc = denNgay.HasValue ? (DateTime?)denNgay.Value.Date.AddDays(1) : null;
                string currentUser = ViewState["taikhoan"] == null ? "" : ViewState["taikhoan"].ToString();

                var list = (from bg in db.BaoGia_tbs
                            join tk in db.taikhoan_tbs on bg.nguoibaogia equals tk.taikhoan into gTk
                            from tk in gTk.DefaultIfEmpty()
                            join ct in db.BaoGia_ChiTiet_tbs on bg.id.ToString() equals ct.id_baogia into chiTietGroup
                            let tongTien = chiTietGroup.Sum(x => (long?)x.thanhtien) ?? 0
                            let tongSauGiamCT = chiTietGroup.Sum(x => (long?)x.TongSauGiam) ?? 0
                            let giamGiaDacBiet = (long?)bg.giamgiadacbiet ?? 0
                            let tongSauGiam = tongSauGiamCT - giamGiaDacBiet
                            let vat = (int?)bg.vat ?? 0
                            let tongSauThue = vat != 0 ? tongSauGiam * (1 + (decimal)vat / 100) : tongSauGiam
                            select new
                            {
                                bg.id,
                                bg.ngaybaogia,
                                bg.ngayban_kyhopdong,
                                bg.nguoibaogia,
                                HoTenNhanVien = tk != null ? tk.hoten : bg.nguoibaogia,
                                bg.ten_khachhang,
                                bg.sdt_khachhang,
                                bg.diachi_khachhang,
                                bg.trangthai,
                                CongNo = bg.congno ?? 0,
                                Thuong = bg.thuongdoanhso ?? 0,
                                TongTien = tongTien,
                                TongSauThue = tongSauThue
                            }).AsQueryable();

                if (check_login_cl.CheckQuyen(db, currentUser, "17"))
                    list = list.Where(x => x.nguoibaogia == currentUser);

                if (tuNgay.HasValue)
                {
                    if (ddl_loai_ngay.SelectedValue == "baogia")
                        list = list.Where(x => x.ngaybaogia.HasValue && x.ngaybaogia.Value >= tuNgayLoc.Value);
                    else
                        list = list.Where(x => x.ngayban_kyhopdong.HasValue && x.ngayban_kyhopdong.Value >= tuNgayLoc.Value);
                }

                if (denNgay.HasValue)
                {
                    if (ddl_loai_ngay.SelectedValue == "baogia")
                        list = list.Where(x => x.ngaybaogia.HasValue && x.ngaybaogia.Value < denNgayLoc.Value);
                    else
                        list = list.Where(x => x.ngayban_kyhopdong.HasValue && x.ngayban_kyhopdong.Value < denNgayLoc.Value);
                }

                if (ddl_trangthai.SelectedValue == "daban")
                    list = list.Where(x => x.ngayban_kyhopdong.HasValue || x.trangthai == "Đã ký HĐ");
                else if (ddl_trangthai.SelectedValue == "chuaban")
                    list = list.Where(x => !x.ngayban_kyhopdong.HasValue && x.trangthai != "Đã ký HĐ");

                if (ddl_thanhtoan.SelectedValue == "done")
                    list = list.Where(x => (x.ngayban_kyhopdong.HasValue || x.trangthai == "Đã ký HĐ") && x.CongNo == 0);
                else if (ddl_thanhtoan.SelectedValue == "debt")
                    list = list.Where(x => (x.ngayban_kyhopdong.HasValue || x.trangthai == "Đã ký HĐ") && x.CongNo != 0);

                if (!string.IsNullOrWhiteSpace(ddl_nhanvien.SelectedValue))
                    list = list.Where(x => x.nguoibaogia == ddl_nhanvien.SelectedValue);

                string key = txt_timkiem.Text.Trim();
                if (!string.IsNullOrWhiteSpace(key))
                {
                    list = list.Where(x =>
                        x.id.ToString() == key ||
                        (x.ten_khachhang != null && x.ten_khachhang.Contains(key)) ||
                        (x.sdt_khachhang != null && x.sdt_khachhang.Contains(key)) ||
                        (x.diachi_khachhang != null && x.diachi_khachhang.Contains(key)));
                }

                var data = list.ToList();
                var ids = data.Select(x => x.id.ToString()).ToList();

                if (!string.IsNullOrWhiteSpace(txt_sanpham.Text))
                {
                    string spKey = txt_sanpham.Text.Trim();

                    var hangIds = db.DuLieuNguon_tbs
                        .Where(h => h.kyhieu == "hangsanpham" && h.ten != null && h.ten.Contains(spKey))
                        .Select(h => h.id.ToString())
                        .ToList();

                    var idSanPhams = db.KhoSanPham_tbs
                        .Where(sp =>
                            (sp.ten != null && sp.ten.Contains(spKey)) ||
                            (sp.model != null && sp.model.Contains(spKey)) ||
                            hangIds.Contains(sp.id_hang))
                        .Select(sp => sp.id.ToString())
                        .ToList();

                    var idsCoSp = db.BaoGia_ChiTiet_tbs
                        .Where(ct => ids.Contains(ct.id_baogia) && idSanPhams.Contains(ct.id_sanpham))
                        .Select(ct => ct.id_baogia)
                        .Distinct()
                        .ToList();

                    data = data.Where(x => idsCoSp.Contains(x.id.ToString())).ToList();
                    ids = data.Select(x => x.id.ToString()).ToList();
                }

                var dataBan = data.Where(x => x.ngayban_kyhopdong.HasValue || x.trangthai == "Đã ký HĐ").ToList();
                var idsBan = dataBan.Select(x => x.id.ToString()).ToList();

                // --- BẮT ĐẦU TRUY VẤN HÀNG BẢO HÀNH ---
                var listBH = (from bh in db.HangBaoHanh_tbs
                              select new
                              {
                                  bh.id,
                                  bh.ngaytao,
                                  bh.NgayTra_ThucTe,
                                  bh.nguoitao,
                                  bh.ten_khachhang,
                                  bh.sdt_khachhang,
                                  bh.diachi_khachhang,
                                  bh.trangthai,
                                  CongNo = bh.congno ?? 0,
                                  TongSauThue = bh.giatri_thuc_donhang ?? 0
                              }).AsQueryable();

                if (check_login_cl.CheckQuyen(db, currentUser, "17"))
                    listBH = listBH.Where(x => x.nguoitao == currentUser);

                if (tuNgay.HasValue)
                {
                    if (ddl_loai_ngay.SelectedValue == "baogia")
                        listBH = listBH.Where(x => x.ngaytao.HasValue && x.ngaytao.Value >= tuNgayLoc.Value);
                    else
                        listBH = listBH.Where(x => x.NgayTra_ThucTe.HasValue && x.NgayTra_ThucTe.Value >= tuNgayLoc.Value);
                }

                if (denNgay.HasValue)
                {
                    if (ddl_loai_ngay.SelectedValue == "baogia")
                        listBH = listBH.Where(x => x.ngaytao.HasValue && x.ngaytao.Value < denNgayLoc.Value);
                    else
                        listBH = listBH.Where(x => x.NgayTra_ThucTe.HasValue && x.NgayTra_ThucTe.Value < denNgayLoc.Value);
                }

                if (ddl_trangthai.SelectedValue == "daban")
                    listBH = listBH.Where(x => x.trangthai == "Đã trả");
                else if (ddl_trangthai.SelectedValue == "chuaban")
                    listBH = listBH.Where(x => x.trangthai != "Đã trả");

                if (ddl_thanhtoan.SelectedValue == "done")
                    listBH = listBH.Where(x => x.trangthai == "Đã trả" && x.CongNo == 0);
                else if (ddl_thanhtoan.SelectedValue == "debt")
                    listBH = listBH.Where(x => x.trangthai == "Đã trả" && x.CongNo != 0);

                if (!string.IsNullOrWhiteSpace(ddl_nhanvien.SelectedValue))
                    listBH = listBH.Where(x => x.nguoitao == ddl_nhanvien.SelectedValue);

                if (!string.IsNullOrWhiteSpace(key))
                {
                    listBH = listBH.Where(x =>
                        x.id.ToString() == key ||
                        (x.ten_khachhang != null && x.ten_khachhang.Contains(key)) ||
                        (x.sdt_khachhang != null && x.sdt_khachhang.Contains(key)) ||
                        (x.diachi_khachhang != null && x.diachi_khachhang.Contains(key)));
                }

                var dataBH = listBH.ToList();
                var idsBH = dataBH.Select(x => x.id.ToString()).ToList();

                if (!string.IsNullOrWhiteSpace(txt_sanpham.Text))
                {
                    string spKey = txt_sanpham.Text.Trim();
                    var hangIds = db.DuLieuNguon_tbs
                        .Where(h => h.kyhieu == "hangsanpham" && h.ten != null && h.ten.Contains(spKey))
                        .Select(h => h.id.ToString())
                        .ToList();

                    var idsCoSpBH = db.HangBaoHanh_ChiTiet_tbs
                        .Where(ct => idsBH.Contains(ct.id_PhieuBaoHanh) &&
                            (
                                (ct.ten != null && ct.ten.Contains(spKey)) ||
                                (ct.model != null && ct.model.Contains(spKey)) ||
                                (ct.id_hang != null && hangIds.Contains(ct.id_hang))
                            ))
                        .Select(ct => ct.id_PhieuBaoHanh)
                        .Distinct()
                        .ToList();

                    dataBH = dataBH.Where(x => idsCoSpBH.Contains(x.id.ToString())).ToList();
                    idsBH = dataBH.Select(x => x.id.ToString()).ToList();
                }

                var dataBanBH = dataBH.Where(x => x.trangthai == "Đã trả").ToList();
                decimal doanhThuBaoHanh = dataBanBH.Sum(x => (decimal)x.TongSauThue);
                decimal congNoBaoHanh = dataBanBH.Sum(x => (decimal)x.CongNo);
                // --- KẾT THÚC TRUY VẤN HÀNG BẢO HÀNH ---

                int tongBaoGia = data.Count;
                int donBan = dataBan.Count + dataBanBH.Count;
                decimal doanhThuBanHang = dataBan.Sum(x => (decimal)x.TongSauThue);
                decimal congNoBanHang = dataBan.Sum(x => (decimal)x.CongNo);
                
                decimal doanhThu = doanhThuBanHang + doanhThuBaoHanh;
                decimal congNo = congNoBanHang + congNoBaoHanh;
                decimal daThanhToan = doanhThu - congNo;

                var chiTietBan = (from ct in db.BaoGia_ChiTiet_tbs
                                  join kho in db.KhoSanPham_tbs on ct.id_sanpham equals kho.id.ToString() into gKho
                                  from kho in gKho.DefaultIfEmpty()
                                  join hang in db.DuLieuNguon_tbs.Where(h => h.kyhieu == "hangsanpham") on kho.id_hang equals hang.id.ToString() into gHang
                                  from hang in gHang.DefaultIfEmpty()
                                  where idsBan.Contains(ct.id_baogia)
                                  select new
                                  {
                                      ct.id_baogia,
                                      ct.id_sanpham,
                                      TenSanPham = kho != null ? (kho.ten ?? ct.id_sanpham) : ct.id_sanpham,
                                      Model = kho != null ? (kho.model ?? "") : "",
                                      TenHang = hang != null ? (hang.ten ?? "Không rõ") : "Không rõ",
                                      SoLuong = ((int?)ct.soluong ?? 0),
                                      DoanhThu = ((long?)ct.TongSauGiam ?? 0),
                                      GiaVon = kho != null ? (((int?)ct.soluong ?? 0) * ((long?)kho.gianhap ?? 0)) : 0
                                  }).ToList();

                decimal giaVon = chiTietBan.Sum(x => (decimal)x.GiaVon);
                decimal loiNhuan = doanhThuBanHang - giaVon; // Note: giaVon only from BaoGia

                ltr_don_ban.Text = donBan.ToString("N0");
                ltr_doanhthu.Text = Money(doanhThu);
                ltr_doanhthubaohanh.Text = Money(doanhThuBaoHanh);
                ltr_congno.Text = Money(congNo);
                ltr_dathanhtoan.Text = Money(daThanhToan);

                grv_nhanvien.DataSource = dataBan
                    .GroupBy(x => new { x.nguoibaogia, x.HoTenNhanVien })
                    .Select(g => new
                    {
                        NhanVien = string.IsNullOrEmpty(g.Key.HoTenNhanVien) ? g.Key.nguoibaogia : g.Key.HoTenNhanVien,
                        SoDon = g.Count(),
                        DoanhThu = g.Sum(x => (decimal)x.TongSauThue),
                        CongNo = g.Sum(x => (decimal)x.CongNo),
                        Thuong = g.Sum(x => (decimal)x.Thuong)
                    })
                    .OrderByDescending(x => x.DoanhThu)
                    .Select(x => new
                    {
                        x.NhanVien,
                        x.SoDon,
                        DoanhThuText = Money(x.DoanhThu),
                        CongNoText = Money(x.CongNo),
                        ThuongText = Money(x.Thuong)
                    })
                    .ToList();
                grv_nhanvien.DataBind();

                grv_hang.DataSource = chiTietBan
                    .Where(x => !string.IsNullOrWhiteSpace(x.TenHang) && x.TenHang != "Không rõ")
                    .GroupBy(x => x.TenHang)
                    .Select(g => new
                    {
                        TenHang = g.Key,
                        SoLuong = g.Sum(x => x.SoLuong),
                        DoanhThu = g.Sum(x => (decimal)x.DoanhThu)
                    })
                    .OrderByDescending(x => x.DoanhThu)
                    .Select(x => new
                    {
                        x.TenHang,
                        x.SoLuong,
                        DoanhThuText = Money(x.DoanhThu)
                    })
                    .ToList();
                grv_hang.DataBind();

                grv_sanpham.DataSource = chiTietBan
                    .GroupBy(x => new { x.id_sanpham, x.TenSanPham, x.Model })
                    .Select(g => new
                    {
                        TenSanPham = string.IsNullOrEmpty(g.Key.Model) ? g.Key.TenSanPham : g.Key.TenSanPham + " - " + g.Key.Model,
                        SoLuong = g.Sum(x => x.SoLuong),
                        DoanhThu = g.Sum(x => (decimal)x.DoanhThu),
                        GiaVon = g.Sum(x => (decimal)x.GiaVon)
                    })
                    .OrderByDescending(x => x.SoLuong)
                    .Take(50)
                    .Select(x => new
                    {
                        x.TenSanPham,
                        x.SoLuong,
                        DoanhThuText = Money(x.DoanhThu),
                        GiaVonText = Money(x.GiaVon),
                        LoiNhuanText = Money(x.DoanhThu - x.GiaVon)
                    })
                    .ToList();
                grv_sanpham.DataBind();

                grv_khachhang.DataSource = dataBan
                    .GroupBy(x => new { x.ten_khachhang, x.sdt_khachhang })
                    .Select(g => new
                    {
                        KhachHang = (string.IsNullOrEmpty(g.Key.ten_khachhang) ? "Không rõ" : g.Key.ten_khachhang) + (string.IsNullOrWhiteSpace(g.Key.sdt_khachhang) ? "" : " - " + g.Key.sdt_khachhang),
                        SoDon = g.Count(),
                        DoanhThu = g.Sum(x => (decimal)x.TongSauThue),
                        CongNo = g.Sum(x => (decimal)x.CongNo)
                    })
                    .OrderByDescending(x => x.DoanhThu)
                    .Take(50)
                    .Select(x => new
                    {
                        x.KhachHang,
                        x.SoDon,
                        DoanhThuText = Money(x.DoanhThu),
                        CongNoText = Money(x.CongNo)
                    })
                    .ToList();
                grv_khachhang.DataBind();

                pn_thongbao.Visible = false;
            }
        }
        catch (Exception ex)
        {
            pn_thongbao.Visible = true;
            lb_thongbao.Text = "Không tải được thống kê: " + Server.HtmlEncode(ex.Message);
            ClearSummaryAndTables();
        }
    }

    private void ClearSummaryAndTables()
    {
        ltr_don_ban.Text = "0";
        ltr_doanhthu.Text = "0 đ";
        ltr_congno.Text = "0 đ";
        ltr_dathanhtoan.Text = "0 đ";

        grv_nhanvien.DataSource = null; grv_nhanvien.DataBind();
        grv_hang.DataSource = null; grv_hang.DataBind();
        grv_sanpham.DataSource = null; grv_sanpham.DataBind();
        grv_khachhang.DataSource = null; grv_khachhang.DataBind();
    }

    private DateTime? ParseDate(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        DateTime d;
        string[] formats = { "dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd", "M/d/yyyy", "MM/dd/yyyy" };
        if (DateTime.TryParseExact(value.Trim(), formats, new CultureInfo("vi-VN"), DateTimeStyles.None, out d)) return d;
        if (DateTime.TryParse(value, new CultureInfo("vi-VN"), DateTimeStyles.None, out d)) return d;
        if (DateTime.TryParse(value, out d)) return d;
        return null;
    }

    private static string Money(decimal value)
    {
        return value.ToString("N0", new CultureInfo("vi-VN")) + " đ";
    }
}
