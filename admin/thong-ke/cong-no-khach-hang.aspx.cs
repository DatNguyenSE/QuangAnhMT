using System;
using System.Linq;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

public partial class admin_thong_ke_cong_no_khach_hang : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["url_back"] = HttpContext.Current.Request.Url.AbsoluteUri;
            check_login_cl.check_login_admin("40", "40"); // Quyền thống kê bán hàng
            txt_tungay.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("dd/MM/yyyy");
            txt_denngay.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)).ToString("dd/MM/yyyy");
            LoadThongKe();
        }
    }

    protected void but_thongke_Click(object sender, EventArgs e)
    {
        LoadThongKe();
    }

    protected void but_reset_Click(object sender, EventArgs e)
    {
        txt_timkiem.Text = "";
        txt_tungay.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("dd/MM/yyyy");
        txt_denngay.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)).ToString("dd/MM/yyyy");
        LoadThongKe();
    }

    private void LoadThongKe()
    {
        try
        {
            DateTime tuNgay = DateTime.MinValue;
            DateTime denNgay = DateTime.MaxValue;
            if (!string.IsNullOrEmpty(txt_tungay.Text))
            {
                DateTime.TryParseExact(txt_tungay.Text, "dd/MM/yyyy", null, DateTimeStyles.None, out tuNgay);
            }
            if (!string.IsNullOrEmpty(txt_denngay.Text))
            {
                DateTime.TryParseExact(txt_denngay.Text, "dd/MM/yyyy", null, DateTimeStyles.None, out denNgay);
                denNgay = denNgay.AddDays(1).AddSeconds(-1);
            }

            using (dbDataContext db = new dbDataContext())
            {
                var thresholdDate = DateTime.Now.AddDays(-30);
                
                // Lấy các đơn bán hàng có nợ (bao gồm cả điều chỉnh công nợ)
                var listBanHang = db.BaoGia_tbs
                    .Where(x => (x.congno ?? 0) > 0 && (x.ngayban_kyhopdong.HasValue || x.trangthai == "Đã ký HĐ" || x.trangthai == "Điều chỉnh công nợ"))
                    .Where(x => (x.ngayban_kyhopdong ?? x.ngaybaogia) >= tuNgay && (x.ngayban_kyhopdong ?? x.ngaybaogia) <= denNgay)
                    .Select(x => new
                    {
                        TenKhachHang = x.ten_khachhang ?? "Không rõ",
                        SoDienThoai = x.sdt_khachhang ?? "Không rõ",
                        NoBanHang = x.congno ?? 0,
                        NoBaoHanh = 0L,
                        NgayTao = x.ngayban_kyhopdong ?? x.ngaybaogia,
                        QuaHan = ((x.ngayban_kyhopdong ?? x.ngaybaogia) < thresholdDate) ? (x.congno ?? 0) : 0L
                    }).ToList();

                // Lấy các đơn bảo hành có nợ
                var listBaoHanh = db.HangBaoHanh_tbs
                    .Where(x => (x.congno ?? 0) > 0 && x.trangthai == "Đã trả")
                    .Where(x => (x.NgayTra_ThucTe ?? x.ngaytao) >= tuNgay && (x.NgayTra_ThucTe ?? x.ngaytao) <= denNgay)
                    .Select(x => new
                    {
                        TenKhachHang = x.ten_khachhang ?? "Không rõ",
                        SoDienThoai = x.sdt_khachhang ?? "Không rõ",
                        NoBanHang = 0L,
                        NoBaoHanh = x.congno ?? 0,
                        NgayTao = x.NgayTra_ThucTe ?? x.ngaytao,
                        QuaHan = ((x.NgayTra_ThucTe ?? x.ngaytao) < thresholdDate) ? (x.congno ?? 0) : 0L
                    }).ToList();

                // Gộp chung
                var allData = listBanHang.Concat(listBaoHanh).ToList();

                // Gom nhóm theo Khách hàng và SĐT
                var grouped = allData
                    .GroupBy(x => new { x.TenKhachHang, x.SoDienThoai })
                    .Select(g => new
                    {
                        TenKhachHang = g.Key.TenKhachHang,
                        SoDienThoai = g.Key.SoDienThoai,
                        NoBanHang = g.Sum(x => x.NoBanHang),
                        NoBaoHanh = g.Sum(x => x.NoBaoHanh),
                        TongNo = g.Sum(x => x.NoBanHang) + g.Sum(x => x.NoBaoHanh),
                        QuaHan = g.Sum(x => x.QuaHan),
                        NgayQuaHanList = g.Where(x => x.QuaHan > 0).Select(x => x.NgayTao).OrderBy(x => x).ToList()
                    })
                    .Where(x => x.TongNo > 0)
                    .OrderByDescending(x => x.TongNo)
                    .ToList();

                // Lọc theo tìm kiếm
                string key = txt_timkiem.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(key))
                {
                    grouped = grouped.Where(x => 
                        x.TenKhachHang.ToLower().Contains(key) || 
                        x.SoDienThoai.ToLower().Contains(key)).ToList();
                }

                // Tính tổng kết
                decimal sumTongNo = grouped.Sum(x => (decimal)x.TongNo);
                decimal sumNoBanHang = grouped.Sum(x => (decimal)x.NoBanHang);
                decimal sumNoBaoHanh = grouped.Sum(x => (decimal)x.NoBaoHanh);

                ltr_tongno.Text = Money(sumTongNo);
                ltr_nobanhang.Text = Money(sumNoBanHang);
                ltr_nobaohanh.Text = Money(sumNoBaoHanh);
                ltr_sokhach.Text = grouped.Count.ToString("N0");

                // Tạo Token và Link Public cho mỗi người
                string domain = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                
                var displayData = grouped.Select(x => {
                    string rawToken = x.SoDienThoai + "|" + x.TenKhachHang;
                    string encryptedToken = mahoa_cl.mahoa_Bcorn(rawToken);
                    string encodedToken = HttpUtility.UrlEncode(encryptedToken);
                    string publicLink = domain + "/chi-tiet-cong-no.aspx?token=" + encodedToken;
                    
                    string strNgayQuaHan = "";
                    if (x.NgayQuaHanList.Any())
                    {
                        var dates = x.NgayQuaHanList.Where(d => d.HasValue).Select(d => d.Value.ToString("dd/MM/yy")).Distinct();
                        strNgayQuaHan = string.Join("<br/>", dates);
                    }

                    return new {
                        x.TenKhachHang,
                        x.SoDienThoai,
                        NoBanHangText = Money(x.NoBanHang),
                        NoBaoHanhText = Money(x.NoBaoHanh),
                        TongNoText = Money(x.TongNo),
                        QuaHanText = x.QuaHan > 0 ? "<span class='fg-red fw-bold'><span class='mif-warning'></span> Báo động</span>" : "",
                        NgayQuaHan = strNgayQuaHan,
                        Token = encryptedToken,
                        PublicLink = publicLink
                    };
                }).ToList();

                grv_khachhang.DataSource = displayData;
                grv_khachhang.DataBind();

                pn_thongbao.Visible = false;
                up_grid.Update();
            }
        }
        catch (Exception ex)
        {
            pn_thongbao.Visible = true;
            lb_thongbao.Text = "Không tải được dữ liệu: " + Server.HtmlEncode(ex.Message);
        }
    }

    protected void grv_khachhang_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "ViewDetail")
        {
            string token = e.CommandArgument.ToString();
            LoadDetailModal(token);
        }
    }

    private void LoadDetailModal(string token)
    {
        try
        {
            string decrypted = mahoa_cl.giaima_Bcorn(token);
            if (string.IsNullOrEmpty(decrypted) || !decrypted.Contains("|")) return;

            string[] parts = decrypted.Split(new char[] { '|' }, 2);
            string sdt = parts[0];
            string ten = parts[1];

            ltr_modal_ten.Text = Server.HtmlEncode(ten);
            ltr_modal_sdt.Text = Server.HtmlEncode(sdt);

            DateTime tuNgay = DateTime.MinValue;
            DateTime denNgay = DateTime.MaxValue;
            if (!string.IsNullOrEmpty(txt_tungay.Text))
            {
                DateTime.TryParseExact(txt_tungay.Text, "dd/MM/yyyy", null, DateTimeStyles.None, out tuNgay);
            }
            if (!string.IsNullOrEmpty(txt_denngay.Text))
            {
                DateTime.TryParseExact(txt_denngay.Text, "dd/MM/yyyy", null, DateTimeStyles.None, out denNgay);
                denNgay = denNgay.AddDays(1).AddSeconds(-1);
            }

            using (dbDataContext db = new dbDataContext())
            {
                // 1. Mua hàng
                var listBanHang = db.BaoGia_tbs
                    .Where(x => x.sdt_khachhang == sdt && x.ten_khachhang == ten && (x.congno ?? 0) > 0 && (x.ngayban_kyhopdong.HasValue || x.trangthai == "Đã ký HĐ" || x.trangthai == "Điều chỉnh công nợ"))
                    .Where(x => (x.ngayban_kyhopdong ?? x.ngaybaogia) >= tuNgay && (x.ngayban_kyhopdong ?? x.ngaybaogia) <= denNgay)
                    .Select(x => new
                    {
                        Ngay = x.ngayban_kyhopdong ?? x.ngaybaogia,
                        MaDon = x.id.ToString(),
                        TrangThai = x.trangthai == "Điều chỉnh công nợ" ? "Điều chỉnh nợ cũ" : "Bán hàng",
                        TongTien = x.tongtien ?? 0,
                        CongNo = x.congno ?? 0
                    }).ToList()
                    .Select(x => new
                    {
                        x.Ngay,
                        x.MaDon,
                        x.TrangThai,
                        TongTienText = Money(x.TongTien),
                        CongNoText = Money(x.CongNo)
                    }).ToList();

                grv_modal_banhang.DataSource = listBanHang;
                grv_modal_banhang.DataBind();

                // 2. Bảo hành
                var listBaoHanh = db.HangBaoHanh_tbs
                    .Where(x => x.sdt_khachhang == sdt && x.ten_khachhang == ten && (x.congno ?? 0) > 0 && x.trangthai == "Đã trả")
                    .Where(x => (x.NgayTra_ThucTe ?? x.ngaytao) >= tuNgay && (x.NgayTra_ThucTe ?? x.ngaytao) <= denNgay)
                    .Select(x => new
                    {
                        Ngay = x.NgayTra_ThucTe ?? x.ngaytao,
                        MaDon = x.id.ToString(),
                        TongTien = x.giatri_thuc_donhang ?? 0,
                        CongNo = x.congno ?? 0
                    }).ToList()
                    .Select(x => new
                    {
                        x.Ngay,
                        x.MaDon,
                        TongTienText = Money(x.TongTien),
                        CongNoText = Money(x.CongNo)
                    }).ToList();

                grv_modal_baohanh.DataSource = listBaoHanh;
                grv_modal_baohanh.DataBind();

                ScriptManager.RegisterStartupScript(up_modal, up_modal.GetType(), "open_modal", "Metro.dialog.open('#modal_chitiet');", true);
                up_modal.Update();
            }
        }
        catch (Exception ex)
        {
            pn_thongbao.Visible = true;
            lb_thongbao.Text = "Lỗi khi tải chi tiết: " + Server.HtmlEncode(ex.Message);
        }
    }

    protected void but_export_Click(object sender, EventArgs e)
    {
        try
        {
            DateTime tuNgay = DateTime.MinValue;
            DateTime denNgay = DateTime.MaxValue;
            if (!string.IsNullOrEmpty(txt_tungay.Text))
            {
                DateTime.TryParseExact(txt_tungay.Text, "dd/MM/yyyy", null, DateTimeStyles.None, out tuNgay);
            }
            if (!string.IsNullOrEmpty(txt_denngay.Text))
            {
                DateTime.TryParseExact(txt_denngay.Text, "dd/MM/yyyy", null, DateTimeStyles.None, out denNgay);
                denNgay = denNgay.AddDays(1).AddSeconds(-1);
            }

            using (dbDataContext db = new dbDataContext())
            {
                var listBanHang = db.BaoGia_tbs
                    .Where(x => (x.congno ?? 0) > 0 && (x.ngayban_kyhopdong.HasValue || x.trangthai == "Đã ký HĐ"))
                    .Where(x => (x.ngayban_kyhopdong ?? x.ngaybaogia) >= tuNgay && (x.ngayban_kyhopdong ?? x.ngaybaogia) <= denNgay)
                    .Select(x => new { TenKhachHang = x.ten_khachhang ?? "Không rõ", SoDienThoai = x.sdt_khachhang ?? "Không rõ", NoBanHang = x.congno ?? 0, NoBaoHanh = 0L }).ToList();

                var listBaoHanh = db.HangBaoHanh_tbs
                    .Where(x => (x.congno ?? 0) > 0 && x.trangthai == "Đã trả")
                    .Where(x => (x.NgayTra_ThucTe ?? x.ngaytao) >= tuNgay && (x.NgayTra_ThucTe ?? x.ngaytao) <= denNgay)
                    .Select(x => new { TenKhachHang = x.ten_khachhang ?? "Không rõ", SoDienThoai = x.sdt_khachhang ?? "Không rõ", NoBanHang = 0L, NoBaoHanh = x.congno ?? 0 }).ToList();

                var allData = listBanHang.Concat(listBaoHanh).ToList();
                var grouped = allData
                    .GroupBy(x => new { x.TenKhachHang, x.SoDienThoai })
                    .Select(g => new {
                        TenKhachHang = g.Key.TenKhachHang,
                        SoDienThoai = g.Key.SoDienThoai,
                        NoBanHang = g.Sum(x => x.NoBanHang),
                        NoBaoHanh = g.Sum(x => x.NoBaoHanh),
                        TongNo = g.Sum(x => x.NoBanHang) + g.Sum(x => x.NoBaoHanh)
                    })
                    .Where(x => x.TongNo > 0)
                    .OrderByDescending(x => x.TongNo)
                    .ToList();

                string key = txt_timkiem.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(key))
                {
                    grouped = grouped.Where(x => x.TenKhachHang.ToLower().Contains(key) || x.SoDienThoai.ToLower().Contains(key)).ToList();
                }

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("<table border='1' style='border-collapse:collapse; text-align:center;'>");
                sb.Append("<tr>");
                sb.Append("<th style='background-color:#f89406;color:white;'>STT</th>");
                sb.Append("<th style='background-color:#f89406;color:white;'>KHÁCH HÀNG</th>");
                sb.Append("<th style='background-color:#f89406;color:white;'>SỐ ĐIỆN THOẠI</th>");
                sb.Append("<th style='background-color:#f89406;color:white;'>NỢ MUA HÀNG</th>");
                sb.Append("<th style='background-color:#f89406;color:white;'>NỢ BẢO HÀNH</th>");
                sb.Append("<th style='background-color:#f89406;color:white;'>TỔNG NỢ</th>");
                sb.Append("</tr>");
                
                int stt = 1;
                foreach (var item in grouped)
                {
                    sb.Append("<tr>");
                    sb.Append("<td>" + stt++ + "</td>");
                    sb.Append("<td style='text-align:left;'>" + item.TenKhachHang + "</td>");
                    sb.Append("<td style=\"mso-number-format:'\\@'\">" + item.SoDienThoai + "</td>");
                    sb.Append("<td>" + item.NoBanHang + "</td>");
                    sb.Append("<td>" + item.NoBaoHanh + "</td>");
                    sb.Append("<td style='color:red; font-weight:bold;'>" + item.TongNo + "</td>");
                    sb.Append("</tr>");
                }
                sb.Append("</table>");

                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=CongNoKhachHang_" + DateTime.Now.ToString("ddMMyyyy") + ".xls");
                Response.Charset = "utf-8";
                Response.ContentType = "application/vnd.ms-excel";
                Response.ContentEncoding = System.Text.Encoding.Unicode;
                Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());
                Response.Write(sb.ToString());
                Response.Flush();
                Response.End();
            }
        }
        catch (System.Threading.ThreadAbortException) { } 
        catch (Exception ex)
        {
            pn_thongbao.Visible = true;
            lb_thongbao.Text = "Lỗi khi xuất file: " + Server.HtmlEncode(ex.Message);
        }
    }

    protected void but_dieuchinh_save_Click(object sender, EventArgs e)
    {
        try
        {
            string sdt = !string.IsNullOrEmpty(txt_dc_sdt.Text) ? txt_dc_sdt.Text.Trim() : (Request.Form[txt_dc_sdt.UniqueID] ?? "").Trim();
            string ten = !string.IsNullOrEmpty(txt_dc_ten.Text) ? txt_dc_ten.Text.Trim() : (Request.Form[txt_dc_ten.UniqueID] ?? "").Trim();
            string tienStr = !string.IsNullOrEmpty(txt_dc_sotien.Text) ? txt_dc_sotien.Text.Trim() : (Request.Form[txt_dc_sotien.UniqueID] ?? "").Trim();
            string ghichu = !string.IsNullOrEmpty(txt_dc_ghichu.Text) ? txt_dc_ghichu.Text.Trim() : (Request.Form[txt_dc_ghichu.UniqueID] ?? "").Trim();
            string ngayStr = !string.IsNullOrEmpty(txt_dc_ngay.Text) ? txt_dc_ngay.Text.Trim() : (Request.Form[txt_dc_ngay.UniqueID] ?? "").Trim();

            if (string.IsNullOrEmpty(sdt) || string.IsNullOrEmpty(ten) || string.IsNullOrEmpty(tienStr))
            {
                ScriptManager.RegisterStartupScript(up_dieuchinh, up_dieuchinh.GetType(), "alert", "Metro.notify.create('Vui lòng nhập đủ thông tin bắt buộc!', 'Lỗi', { cls: 'alert' });", true);
                return;
            }

            long tien = 0;
            if (!long.TryParse(tienStr, out tien))
            {
                ScriptManager.RegisterStartupScript(up_dieuchinh, up_dieuchinh.GetType(), "alert", "Metro.notify.create('Số tiền không hợp lệ!', 'Lỗi', { cls: 'alert' });", true);
                return;
            }

            DateTime ngayGhiNo = DateTime.Now;
            if (!string.IsNullOrEmpty(ngayStr))
            {
                try {
                    ngayGhiNo = DateTime.ParseExact(ngayStr, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                } catch {
                    ngayGhiNo = DateTime.Now;
                }
            }

            using (dbDataContext db = new dbDataContext())
            {
                BaoGia_tb bg = new BaoGia_tb();
                bg.sdt_khachhang = sdt;
                bg.ten_khachhang = ten;
                bg.ngaybaogia = ngayGhiNo;
                bg.ngayban_kyhopdong = ngayGhiNo;
                bg.nguoibaogia = Session["taikhoan"]?.ToString() ?? "";
                bg.trangthai = "Điều chỉnh công nợ";
                bg.tongtien = tien;
                bg.congno = tien;
                bg.ghichu_chuagiao = ghichu;

                db.BaoGia_tbs.InsertOnSubmit(bg);
                db.SubmitChanges();
            }

            txt_dc_sdt.Text = "";
            txt_dc_ten.Text = "";
            txt_dc_sotien.Text = "";
            txt_dc_ngay.Text = "";
            txt_dc_ghichu.Text = "";

            LoadThongKe();
            ScriptManager.RegisterStartupScript(up_dieuchinh, up_dieuchinh.GetType(), "alert_ok", "Metro.notify.create('Đã thêm công nợ thành công!', 'Thành công', { cls: 'success' }); Metro.dialog.close('#modal_dieuchinh');", true);
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(up_dieuchinh, up_dieuchinh.GetType(), "alert_err", "Metro.notify.create('Lỗi: " + Server.HtmlEncode(ex.Message) + "', 'Lỗi', { cls: 'alert' });", true);
        }
    }

    protected void grv_modal_banhang_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "ViewHistory")
        {
            LoadHistory(e.CommandArgument.ToString(), "banhang");
        }
    }

    protected void grv_modal_baohanh_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "ViewHistory")
        {
            LoadHistory(e.CommandArgument.ToString(), "baohanh");
        }
    }

    private void LoadHistory(string maDon, string type)
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                if (type == "banhang")
                {
                    var list = db.LichSu_ThanhToan_tbs
                        .Where(x => x.idbg == maDon)
                        .Select(x => new
                        {
                            Ngay = x.ngay_thanhtoan,
                            SoTienText = Money(x.sotien_thanhtoan ?? 0)
                        }).OrderByDescending(x => x.Ngay).ToList();
                    
                    grv_modal_lichsu.DataSource = list;
                    grv_modal_lichsu.DataBind();
                }
                else
                {
                    var list = db.HangBaoHanh_LichSu_ThanhToan_tbs
                        .Where(x => x.id_PhieuBaoHanh == maDon)
                        .Select(x => new
                        {
                            Ngay = x.ngay_thanhtoan,
                            SoTienText = Money(x.sotien_thanhtoan ?? 0)
                        }).OrderByDescending(x => x.Ngay).ToList();
                    
                    grv_modal_lichsu.DataSource = list;
                    grv_modal_lichsu.DataBind();
                }

                ScriptManager.RegisterStartupScript(up_lichsu, up_lichsu.GetType(), "open_history", "Metro.dialog.open('#modal_lichsu');", true);
                up_lichsu.Update();
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(up_lichsu, up_lichsu.GetType(), "alert_err", "Metro.notify.create('Lỗi tải lịch sử: " + Server.HtmlEncode(ex.Message) + "', 'Lỗi', { cls: 'alert' });", true);
        }
    }

    private static string Money(decimal value)
    {
        return value.ToString("N0", new CultureInfo("vi-VN")) + " đ";
    }
}
