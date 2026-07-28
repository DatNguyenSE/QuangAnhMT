using System;
using System.Linq;
using System.Globalization;
using System.Web.UI.WebControls;

public partial class chi_tiet_cong_no : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadData();
        }
    }

    private void LoadData()
    {
        try
        {
            string token = Request.QueryString["token"];
            if (string.IsNullOrEmpty(token))
            {
                ShowError("Đường dẫn không hợp lệ hoặc đã hết hạn.");
                return;
            }

            // Giải mã token (format mong đợi: "sdt|ten_khach_hang")
            string decrypted = mahoa_cl.giaima_Bcorn(token);
            if (string.IsNullOrEmpty(decrypted) || !decrypted.Contains("|"))
            {
                ShowError("Đường dẫn bị lỗi hoặc không có quyền truy cập.");
                return;
            }

            string[] parts = decrypted.Split(new char[] { '|' }, 2);
            string sdt = parts[0];
            string ten = parts[1];

            ltr_sdt.Text = Server.HtmlEncode(string.IsNullOrEmpty(sdt) ? "Không rõ" : sdt);
            ltr_ten.Text = Server.HtmlEncode(string.IsNullOrEmpty(ten) ? "Không rõ" : ten);

            using (dbDataContext db = new dbDataContext())
            {
                // 1. Lấy công nợ Mua Hàng (Báo giá)
                var listBanHang = db.BaoGia_tbs
                    .Where(x => x.sdt_khachhang == sdt && x.ten_khachhang == ten && (x.congno ?? 0) > 0 && (x.ngayban_kyhopdong.HasValue || x.trangthai == "Đã ký HĐ"))
                    .Select(x => new
                    {
                        Ngay = x.ngayban_kyhopdong ?? x.ngaybaogia,
                        MaDon = x.id.ToString(),
                        TongTien = x.tongtien ?? 0,
                        CongNo = x.congno ?? 0
                    }).ToList()
                    .Select(x => new
                    {
                        x.Ngay,
                        x.MaDon,
                        TongTienText = Money(x.TongTien),
                        CongNoText = Money(x.CongNo)
                    }).ToList();

                grv_banhang.DataSource = listBanHang;
                grv_banhang.DataBind();

                // 2. Lấy công nợ Bảo Hành
                var listBaoHanh = db.HangBaoHanh_tbs
                    .Where(x => x.sdt_khachhang == sdt && x.ten_khachhang == ten && (x.congno ?? 0) > 0 && x.trangthai == "Đã trả")
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

                grv_baohanh.DataSource = listBaoHanh;
                grv_baohanh.DataBind();

                // Tính tổng nợ
                decimal tongNoBanHang = db.BaoGia_tbs
                    .Where(x => x.sdt_khachhang == sdt && x.ten_khachhang == ten && (x.congno ?? 0) > 0 && (x.ngayban_kyhopdong.HasValue || x.trangthai == "Đã ký HĐ"))
                    .Sum(x => (decimal?)x.congno) ?? 0;

                decimal tongNoBaoHanh = db.HangBaoHanh_tbs
                    .Where(x => x.sdt_khachhang == sdt && x.ten_khachhang == ten && (x.congno ?? 0) > 0 && x.trangthai == "Đã trả")
                    .Sum(x => (decimal?)x.congno) ?? 0;

                decimal tongCongNo = tongNoBanHang + tongNoBaoHanh;
                ltr_tongno.Text = Money(tongCongNo);

                if (tongCongNo == 0)
                {
                    ShowError("Khách hàng này hiện không có công nợ.");
                    pn_content.Visible = false;
                    pn_error.Visible = true;
                }
            }
        }
        catch (Exception ex)
        {
            ShowError("Đã xảy ra lỗi khi tải dữ liệu. Xin vui lòng thử lại sau.");
        }
    }

    private void ShowError(string msg)
    {
        pn_error.Visible = true;
        lb_error.Text = msg;
    }

    private string Money(decimal value)
    {
        return value.ToString("N0", new CultureInfo("vi-VN")) + " đ";
    }
}
