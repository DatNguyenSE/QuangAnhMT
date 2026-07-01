using QRCoder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;

public partial class bao_gia : System.Web.UI.Page
{
    public string id, notifi, tenkh, sdtkh, diachikh, ngaybg, hanbg, nhanvienbg, sdtnv, sobg;
    public void load_main(dbDataContext db, string _idbg)
    {

        // Lấy danh sách chi tiết bao giá cùng thông tin sản phẩm
        var q_chitiet = from chitiet in db.BaoGia_ChiTiet_tbs
                        join sanpham in db.KhoSanPham_tbs
                        on chitiet.id_sanpham equals sanpham.id.ToString() into sanphamGroup
                        from sanpham in sanphamGroup.DefaultIfEmpty()
                        join ob4 in db.DuLieuNguon_tbs.Where(p => p.kyhieu == "donvitinh") on sanpham.donvitinh equals ob4.id.ToString() into DVTGroup
                        from ob4 in DVTGroup.DefaultIfEmpty()
                        join ob2 in db.DuLieuNguon_tbs
                                    .Where(p => p.kyhieu == "hangsanpham") on sanpham.id_hang equals ob2.id.ToString() into HangGroup
                        from ob2 in HangGroup.DefaultIfEmpty()
                        where chitiet.id_baogia == _idbg
                        select new
                        {
                            chitiet.id,
                            chitiet.id_baogia,
                            chitiet.id_sanpham,
                            chitiet.soluong,
                            chitiet.thanhtien,
                            chitiet.giaban_taithoidiemnay,
                            chitiet.TongSauGiam,
                            chitiet.giamgia_phantram,
                            chitiet.giamgia_thanhtien,
                            ten_sanpham = sanpham == null ? "" : sanpham.ten,
                            anh = sanpham == null ? "" : sanpham.anh,
                            DVT = ob4.ten,
                            sanpham.thongso_kythuat,
                            TenHang = ob2.ten,
                            sanpham.hangthanhly,
                        };
        if (q_chitiet.Any())
        {


            ViewState["TongThanhTien_ChiTiet"] = q_chitiet.Sum(p => p.thanhtien.Value).ToString("#,##0");
            ViewState["TongGiam_ChiTiet"] = q_chitiet.Sum(p => p.giamgia_thanhtien.Value).ToString("#,##0");
            Int64 TongSauGiam_ChiTiet = q_chitiet.Sum(p => p.TongSauGiam.Value);
            ViewState["TongSauGiam_ChiTiet"] = TongSauGiam_ChiTiet.ToString("#,##0");
            //nếu có giảm đặc biệt
            if (ViewState["giamgia_dacbiet"].ToString() != "0")
            {
                if (ViewState["vat_chitiet"].ToString() == "0") // nếu không có VAT
                {
                    ViewState["donhang_saugiamgia"] = TongSauGiam_ChiTiet - Int64.Parse(ViewState["giamgia_dacbiet"].ToString());
                    ViewState["thanhtien_vat_chitiet"] = 0;
                }
                else // nếu có VAT
                {
                    decimal vatRate = decimal.Parse(ViewState["vat_chitiet"].ToString()) / 100; // Tính tỷ lệ VAT
                    long giamGiaDacBiet = Int64.Parse(ViewState["giamgia_dacbiet"].ToString());
                    // Tính giá trị VAT
                    decimal tongTruocVAT = TongSauGiam_ChiTiet - giamGiaDacBiet;
                    decimal tienVAT = tongTruocVAT * vatRate;
                    ViewState["thanhtien_vat_chitiet"] = (Math.Round(tienVAT, 0)).ToString("#,##0"); // Làm tròn giá trị VAT
                    // Tính tổng sau giảm giá và VAT
                    ViewState["donhang_saugiamgia"] = (TongSauGiam_ChiTiet - giamGiaDacBiet) * (1 + vatRate);
                }
            }
            else//nếu k có giảm giá đặc biệt
            {
                if (ViewState["vat_chitiet"].ToString() == "0") // nếu không có VAT
                {
                    ViewState["donhang_saugiamgia"] = TongSauGiam_ChiTiet - Int64.Parse(ViewState["giamgia_dacbiet"].ToString());
                    ViewState["thanhtien_vat_chitiet"] = 0;
                }
                else // nếu có VAT
                {
                    decimal vatRate = decimal.Parse(ViewState["vat_chitiet"].ToString()) / 100; // Tính tỷ lệ VAT
                    // Tính giá trị VAT
                    decimal tongTruocVAT = TongSauGiam_ChiTiet - 0;
                    decimal tienVAT = tongTruocVAT * vatRate;
                    ViewState["thanhtien_vat_chitiet"] = Math.Round(tienVAT, 0).ToString("#,##0"); // Làm tròn giá trị VAT

                    // Tính tổng sau giảm giá và VAT
                    ViewState["donhang_saugiamgia"] = (TongSauGiam_ChiTiet) * (1 + vatRate);
                }
            }
        }
        else
        {

            ViewState["TongThanhTien_ChiTiet"] = "0";
            ViewState["TongGiam_ChiTiet"] = "0";
            ViewState["TongSauGiam_ChiTiet"] = "0";
        }
        Repeater2.DataSource = q_chitiet.OrderBy(p => p.ten_sanpham);
        Repeater2.DataBind();

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (dbDataContext db = new dbDataContext())
            {
                if (!string.IsNullOrWhiteSpace(Request.QueryString["id"]))
                {
                    id = Request.QueryString["id"].ToString().Trim();
                    var q = db.BaoGia_tbs.FirstOrDefault(p => p.id_guide.ToString().ToLower() == id.ToLower());
                    if (q != null)
                    {
                        tenkh = q.ten_khachhang; 
                        sdtkh = q.sdt_khachhang; 
                        diachikh = q.diachi_khachhang;
                        ngaybg = q.ngaybaogia.Value.ToString("dd/MM/yyyy");
                        hanbg = q.ngayhethan.Value.ToString("dd/MM/yyyy");
                        var q_nhanvien = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == q.nguoibaogia);
                        sobg = q.id.ToString();
                        if (q_nhanvien != null)
                        {
                            nhanvienbg = q_nhanvien.hoten;
                            sdtnv = q_nhanvien.dienthoai;
                        }
                        if (q.giamgiadacbiet != 0)
                        {
                            ViewState["giamgia_dacbiet"] = q.giamgiadacbiet;
                        }
                        else
                        {
                            ViewState["giamgia_dacbiet"] = "0";
                        }
                        if (q.vat != 0)
                        {
                            ViewState["vat_chitiet"] = q.vat;
                        }
                        else
                        {
                            ViewState["vat_chitiet"] = "0";
                        }
                        load_main(db, sobg);



                        try
                        {
                            // --- Meta tags ---
                            var tenkhSafe = (tenkh ?? string.Empty).ToUpperInvariant();
                            string metaTags = $@"
    <!-- Title -->
    <title>BÁO GIÁ {tenkhSafe}</title>

    <!-- Open Graph Meta Tags -->
    <meta property='og:title' content='BÁO GIÁ {tenkhSafe}' />
";
                            literal_meta.Text = metaTags;

                            // --- Lấy và parse amount từ ViewState ---
                            long amount = 0;
                            var rawAmountObj = ViewState["donhang_saugiamgia"];
                            var rawAmount = rawAmountObj?.ToString()?.Trim() ?? string.Empty;

                            if (string.IsNullOrWhiteSpace(rawAmount))
                                throw new FormatException("Giá trị 'donhang_saugiamgia' rỗng hoặc không tồn tại trong ViewState.");

                            // Làm sạch các ký tự định dạng thường gặp: dấu phẩy, chấm, khoảng trắng, ký hiệu tiền
                            // Nếu bạn chắc chắn đây là SỐ NGUYÊN (đơn vị VND), cách làm sạch dưới đây là đủ:
                            rawAmount = rawAmount
                                .Replace("₫", "")
                                .Replace(",", "")
                                .Replace(".", "")
                                .Replace(" ", "")
                                .Trim();

                            if (!long.TryParse(rawAmount, NumberStyles.Integer, CultureInfo.InvariantCulture, out amount))
                                throw new FormatException($"Không thể chuyển '{rawAmountObj}' sang số nguyên hợp lệ.");

                            // --- Chuẩn bị các tham số gọi fload ---
                            string accountNo;
                            string description = $"{tenkh} TT {sobg}";

                            var vatFlag = (ViewState["vat_chitiet"] ?? string.Empty).ToString();
                            if (vatFlag == "0")
                            {
                                accountNo = "0421000502463";
                                fload(accountNo, amount, description, true);
                            }
                            else
                            {
                                accountNo = "1030308345";
                                fload(accountNo, amount, description, false);
                            }
                        }
                        catch (FormatException ex)
                        {
                            // Gợi ý: hiện thông báo thân thiện cho người dùng & log chi tiết để debug
                            // Ví dụ nếu bạn có Label thông báo:
                            // lblError.Text = "Số tiền không hợp lệ. Vui lòng kiểm tra lại.";
                            System.Diagnostics.Debug.WriteLine("FormatException khi parse donhang_saugiamgia: " + ex.Message);
                            throw; // hoặc bỏ 'throw' nếu bạn đã xử lý UI và muốn dừng ở đây
                        }
                        catch (Exception ex)
                        {
                            // Bắt các lỗi khác (null reference, v.v.)
                            System.Diagnostics.Debug.WriteLine("Lỗi không xác định: " + ex);
                            throw;
                        }



                    }
                    else
                    {
                        notifi = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                    }
                }
                else
                {
                    notifi = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                }
            }
        }
    }

     void fload(string accountNo, long amount, string description, bool isCheck)
    {
         string bankId = "vietcombank";
        string accountName = "THAI DINH AN";

        // 2. Tạo chuỗi URL VietQR Quick Link
        string encodedDescription = Uri.EscapeDataString(description);
        string encodedAccountName = Uri.EscapeDataString(accountName);

        string qrDataString = $"https://img.vietqr.io/image/{bankId}-{accountNo}-qr_only.png?amount={amount}&addInfo={encodedDescription}&accountName={encodedAccountName}";
        if(isCheck)
        {
            imgQRCode.ImageUrl = qrDataString;
            imgQRCode.Visible = true;
        }    
        else
        {
            imgQRCode1.ImageUrl = qrDataString;
            imgQRCode1.Visible = true;
        }    
       

    }
}