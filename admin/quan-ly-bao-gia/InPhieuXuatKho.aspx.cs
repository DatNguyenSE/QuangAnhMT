using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

public partial class admin_quan_ly_bao_gia_InPhieuXuatKho : Page
{
    protected string QuoteNumber = "", Customer = "", Phone = "", Address = "";
    protected string QuoteDate = "", PrintedDate = "", PreparedBy = "", RowsHtml = "";
    protected string TotalQuantity = "", ErrorMessage = "";
    protected bool CanPrint;

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
                ShowError(200, "Báo giá chưa có mặt hàng. Vui lòng thêm và lưu mặt hàng trước khi in.");
                return;
            }
            if (items.Any(p => p.Name == null))
            {
                ShowError(200, "Có mặt hàng trong báo giá không còn tồn tại trong kho. Vui lòng kiểm tra báo giá trước khi in.");
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
            var rows = new StringBuilder();
            int index = 0;
            foreach (var item in items)
            {
                rows.Append("<tr><td class='center'>").Append(++index)
                    .Append("</td><td>").Append(HttpUtility.HtmlEncode(item.Name))
                    .Append("</td><td>").Append(HttpUtility.HtmlEncode(item.Serial))
                    .Append("</td><td class='center'>").Append(HttpUtility.HtmlEncode(item.Unit))
                    .Append("</td><td class='number'>").Append(item.Quantity.ToString("N0"))
                    .Append("</td><td></td></tr>");
            }
            RowsHtml = rows.ToString();
            TotalQuantity = items.Sum(i => (long)i.Quantity).ToString("N0");
            CanPrint = true;
            Title = "Phiếu xuất kho - Báo giá " + QuoteNumber;
        }
    }

    private void ShowError(int statusCode, string message)
    {
        Response.StatusCode = statusCode;
        Response.TrySkipIisCustomErrors = true;
        ErrorMessage = message;
    }
}
