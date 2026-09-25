using System;
using System.Linq;
using System.Web;
using System.Web.UI;

public partial class admin_quan_ly_bao_gia_InPhieuXuatKho : Page
{
    protected string QuoteNumber = "", Customer = "", Phone = "", Address = "";
    protected string QuoteDate = "", PrintedDate = "", PreparedBy = "";
    protected string ErrorMessage = "";
    protected bool IsExcelDownload;

    protected void Page_Load(object sender, EventArgs e)
    {
        check_login_cl.check_login_admin("16", "17");
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetNoStore();
        long id;
        if (!long.TryParse(Request.QueryString["id"], out id) || id <= 0)
        {
            ShowError(400, "Mã báo giá không hợp lệ.");
            return;
        }
        using (dbDataContext db = new dbDataContext())
        {
            db.ObjectTrackingEnabled = false;
            var quote = db.BaoGia_tbs.FirstOrDefault(p => p.id == id);
            if (quote == null)
            {
                ShowError(404, "Không tìm thấy báo giá.");
                return;
            }
            string quoteId = quote.id.ToString();
            var items = (from detail in db.BaoGia_ChiTiet_tbs
                         join product in db.KhoSanPham_tbs on detail.id_sanpham equals product.id.ToString() into products
                         from product in products.DefaultIfEmpty()
                         join unit in db.DuLieuNguon_tbs.Where(u => u.kyhieu == "donvitinh")
                             on product.donvitinh equals unit.id.ToString() into units
                         from unit in units.DefaultIfEmpty()
                         where detail.id_baogia == quoteId
                         orderby detail.id
                         select new {
                             ProductId = detail.id_sanpham,
                             Name = product == null ? null : product.ten,
                             Serial = product == null ? "" : product.so_seri,
                             Unit = unit == null ? "" : unit.ten,
                             Quantity = detail.soluong ?? 0
                         }).ToList();
            if (items.Count == 0)
            {
                ShowError(200, "Báo giá chưa có mặt hàng. Vui lòng thêm và lưu mặt hàng trước khi xuất Excel.");
                return;
            }
            if (items.Any(p => p.Name == null))
            {
                ShowError(200, "Có mặt hàng trong báo giá không còn tồn tại trong kho. Vui lòng kiểm tra báo giá trước khi xuất Excel.");
                return;
            }
            QuoteNumber = quoteId;
            Customer = quote.ten_khachhang ?? "";
            Phone = quote.sdt_khachhang ?? "";
            Address = quote.diachi_khachhang ?? "";
            QuoteDate = quote.ngaybaogia.HasValue ? quote.ngaybaogia.Value.ToString("dd/MM/yyyy") : "";
            PrintedDate = DateTime.Now.ToString("dd/MM/yyyy");
            string account = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(account))
            {
                account = mahoa_cl.giaima_Bcorn(account);
                PreparedBy = db.taikhoan_tbs.Where(t => t.taikhoan == account).Select(t => t.hoten).FirstOrDefault() ?? account;
            }
            var excelItems = items.Select(item => new WarehouseSlipItem {
                Name = item.Name, Serial = item.Serial, Unit = item.Unit, Quantity = item.Quantity
            }).ToList();
            byte[] file = WarehouseSlipExcel.Create(QuoteNumber, QuoteDate, PrintedDate,
                Customer, Phone, Address, PreparedBy, excelItems);
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", "attachment; filename=\"Phieu_xuat_kho_" + quoteId + ".xlsx\"");
            Response.AddHeader("Content-Length", file.Length.ToString(System.Globalization.CultureInfo.InvariantCulture));
            IsExcelDownload = true;
            Response.BinaryWrite(file);
            Context.ApplicationInstance.CompleteRequest();
        }
    }

    protected override void Render(HtmlTextWriter writer)
    {
        if (!IsExcelDownload) base.Render(writer);
    }

    private void ShowError(int statusCode, string message)
    {
        Response.StatusCode = statusCode;
        Response.TrySkipIisCustomErrors = true;
        ErrorMessage = message;
    }
}
