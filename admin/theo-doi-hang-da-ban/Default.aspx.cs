using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_theo_doi_hang_da_ban_Default : System.Web.UI.Page
{
    private sealed class SoldItemRaw
    {
        public long detailId { get; set; }
        public string productId { get; set; }
        public string productName { get; set; }
        public string productModel { get; set; }
        public string productSerial { get; set; }
        public string serialReplacement1 { get; set; }
        public string serialReplacement2 { get; set; }
        public string productImage { get; set; }
        public int quantity { get; set; }
        public long price { get; set; }
        public long itemTongSauGiam { get; set; }
        public long baogiaId { get; set; }
        public DateTime? ngayban { get; set; }
        public string tenKhachHang { get; set; }
        public string sdtKhachHang { get; set; }
        public string diachiKhachHang { get; set; }
        public string maKH { get; set; }
        public string thangBaoHanh { get; set; }
        public int vat { get; set; }
        public long giamgiadacbiet { get; set; }
        public bool isDaban { get; set; }
    }

    private sealed class SoldItemRow
    {
        public string productId { get; set; }
        public string productName { get; set; }
        public string productModel { get; set; }
        public string productSerial { get; set; }
        public string productImage { get; set; }
        public int quantity { get; set; }
        public long price { get; set; }
        public long itemTongSauGiam { get; set; }
        public long totalPrice { get; set; }
        public long baogiaId { get; set; }
        public DateTime? ngayban { get; set; }
        public string tenKhachHang { get; set; }
        public string sdtKhachHang { get; set; }
        public string diachiKhachHang { get; set; }
        public string maKH { get; set; }
        public string thangBaoHanh { get; set; }
        public DateTime? warrantyExpiry { get; set; }
        public bool warrantyExpired { get; set; }
        public bool isDaban { get; set; }
    }

    private static SoldItemRow ProcessSoldItem(SoldItemRaw x, long totalSauGiamAll, DateTime now)
    {
        DateTime? warrantyExpiry = null;
        int warrantyMonths;
        if (x.ngayban.HasValue && int.TryParse(x.thangBaoHanh, out warrantyMonths))
            warrantyExpiry = x.ngayban.Value.AddMonths(warrantyMonths);

        long itemFinalPrice = 0;
        if (totalSauGiamAll > 0)
        {
            double ratio = (double)x.itemTongSauGiam / totalSauGiamAll;
            double itemShareDiscount = x.giamgiadacbiet * ratio;
            double itemAfterSpecialDiscount = x.itemTongSauGiam - itemShareDiscount;
            double itemWithVat = itemAfterSpecialDiscount * (1 + (double)x.vat / 100);
            itemFinalPrice = (long)Math.Round(itemWithVat);
        }

        return new SoldItemRow
        {
            productId = x.productId,
            productName = x.productName,
            productModel = x.productModel,
            productSerial = x.productSerial,
            productImage = x.productImage,
            quantity = x.quantity,
            price = x.price,
            itemTongSauGiam = x.itemTongSauGiam,
            totalPrice = itemFinalPrice,
            baogiaId = x.baogiaId,
            ngayban = x.ngayban,
            tenKhachHang = x.tenKhachHang,
            sdtKhachHang = x.sdtKhachHang,
            diachiKhachHang = x.diachiKhachHang,
            maKH = x.maKH,
            thangBaoHanh = x.thangBaoHanh,
            warrantyExpiry = warrantyExpiry,
            warrantyExpired = warrantyExpiry.HasValue && warrantyExpiry.Value < now,
            isDaban = x.isDaban
        };
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["url_back"] = HttpContext.Current.Request.Url.AbsoluteUri;
            check_login_cl.check_login_admin("41", "41"); // check view sold products tracking permission
            
            using (var db = new dbDataContext())
            {
                var customers = db.BaoGia_tbs
                    .Where(q => q.ngayban_kyhopdong != null && q.ten_khachhang != null && q.ten_khachhang != "")
                    .Select(q => q.ten_khachhang).Distinct().OrderBy(name => name).ToList();
                ddl_customer.DataSource = customers;
                ddl_customer.DataBind();
                ddl_customer.Items.Insert(0, new ListItem("Tất cả", ""));
            }
            ViewState["current_page"] = "1";
            show_main();
        }
    }

    protected void btn_export_excel_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_admin("41", "41");
        show_main(true);
    }

    public void show_main()
    {
        show_main(false);
    }

    private void show_main(bool exportExcel)
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                db.ObjectTrackingEnabled = false;
                db.DeferredLoadingEnabled = false;

                var quotes = db.BaoGia_tbs.Where(q => q.ngayban_kyhopdong != null);
                string customer = Convert.ToString(ViewState["filter_customer"]);
                if (!string.IsNullOrEmpty(customer))
                    quotes = quotes.Where(q => q.ten_khachhang == customer);
                bool byQuoteDate = Convert.ToString(ViewState["filter_date_type"]) == "1";
                if (ViewState["filter_from"] != null)
                {
                    DateTime from = (DateTime)ViewState["filter_from"];
                    quotes = byQuoteDate ? quotes.Where(q => q.ngaybaogia >= from) : quotes.Where(q => q.ngayban_kyhopdong >= from);
                }
                if (ViewState["filter_to"] != null)
                {
                    DateTime until = ((DateTime)ViewState["filter_to"]).AddDays(1);
                    quotes = byQuoteDate ? quotes.Where(q => q.ngaybaogia < until) : quotes.Where(q => q.ngayban_kyhopdong < until);
                }

                IQueryable<SoldItemRaw> rawQuery =
                    from ct in db.BaoGia_ChiTiet_tbs
                    join bg in quotes on ct.id_baogia equals bg.id.ToString()
                    join sp in db.KhoSanPham_tbs on ct.id_sanpham equals sp.id.ToString() into spGroup
                    from sp in spGroup.DefaultIfEmpty()
                    where bg.ngayban_kyhopdong != null
                    select new SoldItemRaw
                    {
                        detailId = ct.id,
                        productId = sp != null ? sp.id.ToString() : "",
                        productName = sp != null ? sp.ten : ct.id_sanpham,
                        productModel = sp != null ? sp.model : "",
                        productSerial = sp != null ? sp.so_seri : "",
                        serialReplacement1 = ct.Seri_Do_L1,
                        serialReplacement2 = ct.Seri_Do_L2,
                        productImage = sp != null ? sp.anh : "",
                        quantity = ct.soluong ?? 0,
                        price = ct.giaban_taithoidiemnay ?? 0,
                        itemTongSauGiam = ct.TongSauGiam ?? 0,
                        baogiaId = bg.id,
                        ngayban = bg.ngayban_kyhopdong,
                        tenKhachHang = bg.ten_khachhang,
                        sdtKhachHang = bg.sdt_khachhang,
                        diachiKhachHang = bg.diachi_khachhang,
                        maKH = ct.MaKichHoat,
                        thangBaoHanh = ct.Thang_BaoHanh,
                        vat = bg.vat ?? 0,
                        giamgiadacbiet = bg.giamgiadacbiet ?? 0,
                        isDaban = sp != null ? (sp.daban ?? false) : false
                    };

                string key = txt_timkiem.Text.Trim();
                if (string.IsNullOrEmpty(key))
                    key = txt_timkiem1.Text.Trim();

                if (!string.IsNullOrEmpty(key))
                {
                    long searchBaoGiaId;
                    bool hasBaoGiaId = long.TryParse(key, out searchBaoGiaId);
                    rawQuery = rawQuery.Where(p =>
                        p.productName.Contains(key) ||
                        p.productModel.Contains(key) ||
                        p.productSerial.Contains(key) ||
                        p.serialReplacement1.Contains(key) ||
                        p.serialReplacement2.Contains(key) ||
                        p.tenKhachHang.Contains(key) ||
                        p.sdtKhachHang.Contains(key) ||
                        (hasBaoGiaId && p.baogiaId == searchBaoGiaId));
                }

                if (!string.IsNullOrEmpty(txt_thangban.Text))
                {
                    string[] parts = txt_thangban.Text.Split('-');
                    if (parts.Length == 2)
                    {
                        int year, month;
                        if (int.TryParse(parts[0], out year) && int.TryParse(parts[1], out month))
                        {
                            rawQuery = rawQuery.Where(p => p.ngayban.HasValue && p.ngayban.Value.Year == year && p.ngayban.Value.Month == month);
                        }
                    }
                }

                int pageSize = Number_cl.Check_Int(Convert.ToString(ViewState["filter_size"] ?? "30"));
                if (pageSize <= 0) pageSize = 10;

                int currentPage;
                if (!int.TryParse(Convert.ToString(ViewState["current_page"]), out currentPage) || currentPage < 1)
                    currentPage = 1;

                // Export uses the same filters, ordering and calculations across every page.
                if (exportExcel)
                {
                    pageSize = int.MaxValue;
                    currentPage = 1;
                }

                DateTime now = DateTime.Now;
                bool validWarrantyOnly = Convert.ToString(ViewState["warranty_filter"]) == "valid";
                int totalRecords;
                List<SoldItemRow> pagedList;

                if (!validWarrantyOnly && !exportExcel)
                {
                    totalRecords = rawQuery.Count();
                    int totalPagesTemp = number_of_page_class.return_total_page(totalRecords, pageSize);
                    if (totalPagesTemp > 0 && currentPage > totalPagesTemp) currentPage = totalPagesTemp;

                    var pageRaw = rawQuery
                        .OrderByDescending(p => p.ngayban)
                        .ThenByDescending(p => p.baogiaId)
                        .ThenByDescending(p => p.detailId)
                        .Skip((currentPage - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    var pageBaoGiaIds = pageRaw.Select(p => p.baogiaId).Distinct().ToList();
                    var totalsByBaoGia = pageBaoGiaIds.Count == 0
                        ? new Dictionary<long, long>()
                        : rawQuery
                            .Where(p => pageBaoGiaIds.Contains(p.baogiaId))
                            .GroupBy(p => p.baogiaId)
                            .Select(g => new { BaoGiaId = g.Key, Total = g.Sum(x => x.itemTongSauGiam) })
                            .ToDictionary(x => x.BaoGiaId, x => x.Total);

                    pagedList = pageRaw.Select(x =>
                    {
                        long total;
                        totalsByBaoGia.TryGetValue(x.baogiaId, out total);
                        return ProcessSoldItem(x, total, now);
                    }).ToList();
                }
                else
                {
                    // Thang_BaoHanh đang là chuỗi nên SQL Server không thể AddMonths trực tiếp.
                    // Lấy toàn bộ kết quả khi lọc bảo hành hoặc xuất Excel.
                    var warrantyRaw = rawQuery
                        .OrderByDescending(p => p.ngayban)
                        .ThenByDescending(p => p.baogiaId)
                        .ThenByDescending(p => p.detailId)
                        .ToList();

                    var totalsByBaoGia = warrantyRaw
                        .GroupBy(x => x.baogiaId)
                        .ToDictionary(g => g.Key, g => g.Sum(x => x.itemTongSauGiam));

                    var validRows = warrantyRaw
                        .Select(x => ProcessSoldItem(x, totalsByBaoGia[x.baogiaId], now))
                        .Where(x => !validWarrantyOnly || (x.warrantyExpiry.HasValue && x.warrantyExpiry.Value >= now))
                        .ToList();

                    totalRecords = validRows.Count;
                    int totalPagesTemp = number_of_page_class.return_total_page(totalRecords, pageSize);
                    if (totalPagesTemp > 0 && currentPage > totalPagesTemp) currentPage = totalPagesTemp;
                    pagedList = validRows.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                }

                if (exportExcel)
                {
                    ExportSoldItems(pagedList);
                    return;
                }

                int totalPages = number_of_page_class.return_total_page(totalRecords, pageSize);
                ViewState["current_page"] = currentPage;
                ViewState["total_page"] = totalPages;

                btn_prev.Enabled = currentPage > 1;
                btn_next.Enabled = currentPage < totalPages;

                Repeater1.DataSource = pagedList;
                Repeater1.DataBind();
                tr_empty.Visible = totalRecords == 0;

                if (totalRecords > 0)
                {
                    int startRecord = (currentPage - 1) * pageSize + 1;
                    int endRecord = startRecord + pagedList.Count - 1;
                    lbl_page_info.Text = startRecord + "-" + endRecord + " trong số " + totalRecords.ToString("#,##0") + " sản phẩm đã bán";
                }
                else
                {
                    lbl_page_info.Text = "0-0 trong số 0 sản phẩm";
                }
            }
        }
        catch (Exception ex)
        {
            string account = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(account)) account = mahoa_cl.giaima_Bcorn(account);
            else account = "";
            Log_cl.Add_Log(ex.Message, account, ex.StackTrace);
            if (exportExcel)
                ScriptManager.RegisterStartupScript(this, GetType(), "export_error",
                    "alert('Không xuất được Excel. Vui lòng thử lại.');", true);
        }
    }

    private bool excelResponse;

    protected override void Render(HtmlTextWriter writer)
    {
        // CompleteRequest alone does not stop Web Forms from appending page HTML.
        if (!excelResponse) base.Render(writer);
    }

    private void ExportSoldItems(List<SoldItemRow> items)
    {
        var workbook = new NPOI.XSSF.UserModel.XSSFWorkbook();
        try
        {
            var sheet = workbook.CreateSheet("Hàng đã bán");
            string[] headers = { "Ngày bán", "Ảnh", "Khách hàng", "Sản phẩm", "Số Seri", "Mã KH", "SL Bán", "Giá bán", "Tổng tiền cuối", "Bảo hành", "ID Báo giá" };
            int[] widths = { 14, 22, 30, 42, 24, 22, 10, 18, 20, 30, 15 };
            var font = workbook.CreateFont();
            font.IsBold = true;
            var headerStyle = workbook.CreateCellStyle();
            headerStyle.SetFont(font);
            headerStyle.WrapText = true;
            headerStyle.FillForegroundColor = NPOI.SS.UserModel.IndexedColors.LightCornflowerBlue.Index;
            headerStyle.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
            var textStyle = workbook.CreateCellStyle();
            textStyle.WrapText = true;
            textStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            var numberStyle = workbook.CreateCellStyle();
            numberStyle.CloneStyleFrom(textStyle);
            numberStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0");
            var dateStyle = workbook.CreateCellStyle();
            dateStyle.CloneStyleFrom(textStyle);
            dateStyle.DataFormat = workbook.CreateDataFormat().GetFormat("dd/MM/yyyy");
            var header = sheet.CreateRow(0);
            header.HeightInPoints = 30;
            for (int col = 0; col < headers.Length; col++)
            {
                var cell = header.CreateCell(col);
                cell.SetCellValue(headers[col]);
                cell.CellStyle = headerStyle;
                sheet.SetColumnWidth(col, widths[col] * 256);
            }

            var drawing = sheet.CreateDrawingPatriarch();
            var pictures = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int index = 0; index < items.Count; index++)
            {
                var item = items[index];
                var row = sheet.CreateRow(index + 1);
                row.HeightInPoints = 65;
                for (int col = 0; col < headers.Length; col++)
                    row.CreateCell(col).CellStyle = textStyle;
                if (item.ngayban.HasValue) row.GetCell(0).SetCellValue(item.ngayban.Value);
                row.GetCell(0).CellStyle = dateStyle;
                string imageUrl = string.IsNullOrEmpty(item.productImage) ? "/uploads/images/no-image.png" : item.productImage;
                row.GetCell(1).SetCellValue(imageUrl);
                // Embed local upload images only; other image URLs remain readable in the cell.
                string virtualPath = imageUrl.StartsWith("~/") ? imageUrl.Substring(1) : imageUrl;
                if (virtualPath.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase) && !virtualPath.Contains(".."))
                {
                    string path = Server.MapPath("~" + virtualPath);
                    string extension = System.IO.Path.GetExtension(path).ToLowerInvariant();
                    if ((extension == ".png" || extension == ".jpg" || extension == ".jpeg") && System.IO.File.Exists(path))
                    {
                        int picture;
                        if (!pictures.TryGetValue(path, out picture))
                        {
                            picture = workbook.AddPicture(System.IO.File.ReadAllBytes(path),
                                extension == ".png" ? NPOI.SS.UserModel.PictureType.PNG : NPOI.SS.UserModel.PictureType.JPEG);
                            pictures[path] = picture;
                        }
                        var anchor = workbook.GetCreationHelper().CreateClientAnchor();
                        anchor.Col1 = 1;
                        anchor.Col2 = 2;
                        anchor.Row1 = index + 1;
                        anchor.Row2 = index + 2;
                        drawing.CreatePicture(anchor, picture);
                        row.GetCell(1).SetCellValue("");
                    }
                }
                row.GetCell(2).SetCellValue((item.tenKhachHang ?? "") + "\n" + (item.sdtKhachHang ?? ""));
                row.GetCell(3).SetCellValue((item.productName ?? "") + (item.isDaban ? "\nĐã đồng bộ" : ""));
                row.GetCell(4).SetCellValue(item.productSerial ?? "");
                row.GetCell(5).SetCellValue(item.maKH ?? "");
                row.GetCell(6).SetCellValue(item.quantity);
                row.GetCell(7).SetCellValue(item.price);
                row.GetCell(8).SetCellValue(item.totalPrice);
                for (int col = 6; col <= 8; col++) row.GetCell(col).CellStyle = numberStyle;
                string warranty = "Không rõ";
                if (!string.IsNullOrWhiteSpace(item.thangBaoHanh))
                    warranty = item.thangBaoHanh + " tháng\nHạn ngày: " +
                        (item.warrantyExpiry.HasValue ? item.warrantyExpiry.Value.ToString("dd/MM/yyyy") : "Không rõ") +
                        (item.warrantyExpired ? "\nHết hạn bảo hành" : "");
                row.GetCell(9).SetCellValue(warranty);
                row.GetCell(10).SetCellValue(item.baogiaId.ToString());
            }
            sheet.CreateFreezePane(0, 1);
            sheet.SetAutoFilter(new NPOI.SS.Util.CellRangeAddress(0, items.Count, 0, headers.Length - 1));
            using (var stream = new System.IO.MemoryStream())
            {
                workbook.Write(stream);
                byte[] data = stream.ToArray();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("Content-Disposition", "attachment; filename=HangDaBan_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();
                Response.BinaryWrite(data);
                excelResponse = true;
                Context.ApplicationInstance.CompleteRequest();
            }
        }
        finally
        {
            workbook.Close();
        }
    }
    protected void txt_timkiem_TextChanged(object sender, EventArgs e)
    {
        ViewState["current_page"] = "1";
        show_main();
    }

    protected void but_show_form_loc_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_admin("41", "41");
        pn_loc.Visible = !pn_loc.Visible;
        lb_filter_error.Text = "";
    }

    protected void but_loc_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_admin("41", "41");
        DateTime from, to;
        bool hasFrom = !string.IsNullOrWhiteSpace(txt_tungay.Text), hasTo = !string.IsNullOrWhiteSpace(txt_denngay.Text);
        bool fromValid = DateTime.TryParseExact(txt_tungay.Text.Trim(), "dd/MM/yyyy",
            System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out from);
        bool toValid = DateTime.TryParseExact(txt_denngay.Text.Trim(), "dd/MM/yyyy",
            System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out to);
        int size;
        if (!int.TryParse(txt_show.Text.Trim(), out size) || size < 1 || size > 10000)
        {
            lb_filter_error.Text = "Số lượng mỗi trang phải từ 1 đến 10.000.";
            return;
        }
        if ((hasFrom && !fromValid) || (hasTo && !toValid) ||
            (hasFrom && hasTo && from > to) || (hasTo && to == DateTime.MaxValue.Date))
        {
            lb_filter_error.Text = "Vui lòng nhập ngày dạng dd/MM/yyyy, từ ngày không lớn hơn đến ngày.";
            return;
        }
        ViewState["filter_customer"] = ddl_customer.SelectedValue;
        ViewState["filter_date_type"] = ddl_thoigian.SelectedValue;
        ViewState["filter_from"] = hasFrom ? (object)from : null;
        ViewState["filter_to"] = hasTo ? (object)to : null;
        ViewState["filter_size"] = size.ToString();
        ViewState["current_page"] = "1";
        pn_loc.Visible = false;
        show_main();
    }

    protected void but_huy_loc_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_admin("41", "41");
        foreach (string key in new[] { "filter_customer",
            "filter_date_type", "filter_from", "filter_to", "filter_size", "warranty_filter" })
            ViewState.Remove(key);
        ddl_customer.SelectedIndex = 0;
        ddl_thoigian.SelectedValue = "2";
        txt_tungay.Text = txt_denngay.Text = txt_thangban.Text = "";
        txt_show.Text = "30";
        lb_filter_error.Text = "";
        ViewState["current_page"] = "1";
        pn_loc.Visible = false;
        show_main();
    }

    protected void QuickDate_Click(object sender, EventArgs e)
    {
        DateTime today = DateTime.Today, from = today, to = today;
        string period = ((Button)sender).CommandArgument;
        if (period == "homqua") from = to = today.AddDays(-1);
        else if (period.StartsWith("tuan"))
        {
            from = today.AddDays(-(((int)today.DayOfWeek + 6) % 7));
            if (period == "tuantruoc") from = from.AddDays(-7);
            to = from.AddDays(6);
        }
        else if (period.StartsWith("thang"))
        {
            from = new DateTime(today.Year, today.Month, 1);
            if (period == "thangtruoc") from = from.AddMonths(-1);
            to = from.AddMonths(1).AddDays(-1);
        }
        else if (period.StartsWith("quy"))
        {
            from = new DateTime(today.Year, ((today.Month - 1) / 3) * 3 + 1, 1);
            if (period == "quytruoc") from = from.AddMonths(-3);
            to = from.AddMonths(3).AddDays(-1);
        }
        else if (period.StartsWith("nam"))
        {
            from = new DateTime(today.Year - (period == "namtruoc" ? 1 : 0), 1, 1);
            to = from.AddYears(1).AddDays(-1);
        }
        txt_tungay.Text = from.ToString("dd/MM/yyyy");
        txt_denngay.Text = to.ToString("dd/MM/yyyy");
        lb_filter_error.Text = "";
    }
    protected void txt_show_TextChanged(object sender, EventArgs e)
    {
        ViewState["current_page"] = "1";
        show_main();
    }

    protected void txt_thangban_TextChanged(object sender, EventArgs e)
    {
        ViewState["current_page"] = "1";
        show_main();
    }

    protected void btn_clear_thangban_Click(object sender, EventArgs e)
    {
        txt_thangban.Text = "";
        ViewState["current_page"] = "1";
        show_main();
    }

    protected void btn_warranty_Click(object sender, EventArgs e)
    {
        ViewState["warranty_filter"] = "valid";
        ViewState["current_page"] = "1";
        show_main();
    }

    protected void btn_all_Click(object sender, EventArgs e)
    {
        ViewState["warranty_filter"] = "";
        ViewState["current_page"] = "1";
        show_main();
    }

    protected void btn_prev_Click(object sender, EventArgs e)
    {
        int currentPage = int.Parse(ViewState["current_page"].ToString());
        if (currentPage > 1)
        {
            ViewState["current_page"] = (currentPage - 1).ToString();
            show_main();
        }
    }

    protected void btn_next_Click(object sender, EventArgs e)
    {
        int currentPage = int.Parse(ViewState["current_page"].ToString());
        int totalPages = int.Parse((ViewState["total_page"] != null ? ViewState["total_page"].ToString() : null) ?? "1");
        if (currentPage < totalPages)
        {
            ViewState["current_page"] = (currentPage + 1).ToString();
            show_main();
        }
    }

    protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
    }

    protected void btn_view_Click(object sender, EventArgs e)
    {
        LinkButton btn = (LinkButton)sender;
        string[] args = btn.CommandArgument.Split('|');
        if (args.Length == 2)
        {
            string baogiaId = args[0];
            string productId = args[1];

            using (dbDataContext db = new dbDataContext())
            {
                db.ObjectTrackingEnabled = false;
                db.DeferredLoadingEnabled = false;
                long baogiaIdValue;
                long.TryParse(baogiaId, out baogiaIdValue);
                var bg = db.BaoGia_tbs.FirstOrDefault(p => p.id == baogiaIdValue);
                var ct = db.BaoGia_ChiTiet_tbs.FirstOrDefault(p => p.id_baogia == baogiaId && p.id_sanpham == productId);
                long productIdValue;
                long.TryParse(productId, out productIdValue);
                var sp = db.KhoSanPham_tbs.FirstOrDefault(p => p.id == productIdValue);

                if (bg != null && ct != null)
                {
                    ViewState["detail_baogiaId"] = baogiaId;
                    ViewState["detail_productId"] = productId;
                    SetProductDetailEditMode(false);

                    // Populate Product Info
                    if (sp != null)
                    {
                        img_detail_sp.ImageUrl = string.IsNullOrEmpty(sp.anh) ? "/uploads/images/no-image.png" : sp.anh;
                        lbl_detail_tensp.Text = sp.ten;
                        lbl_detail_model.Text = sp.model;
                        lbl_detail_seri.Text = string.IsNullOrEmpty(sp.so_seri) ? "Không rõ" : sp.so_seri;
                        lbl_detail_thongso.Text = string.IsNullOrEmpty(sp.thongso_kythuat) ? "Không rõ" : sp.thongso_kythuat;
                    }
                    else
                    {
                        img_detail_sp.ImageUrl = "/uploads/images/no-image.png";
                        lbl_detail_tensp.Text = ct.id_sanpham;
                        lbl_detail_model.Text = "Không rõ";
                        lbl_detail_seri.Text = "Không rõ";
                        lbl_detail_thongso.Text = "Không rõ";
                    }

                     // Populate product inspection details
                     lbl_detail_makichhoat.Text = string.IsNullOrWhiteSpace(ct.MaKichHoat) ? "Không rõ" : ct.MaKichHoat;
                     lbl_detail_thangbaohanh.Text = string.IsNullOrWhiteSpace(ct.Thang_BaoHanh) ? "Không rõ" : ct.Thang_BaoHanh;
                     lbl_detail_seri_do_l1.Text = string.IsNullOrWhiteSpace(ct.Seri_Do_L1) ? "Không rõ" : ct.Seri_Do_L1;
                     lbl_detail_id_khachhang_do_l1.Text = string.IsNullOrWhiteSpace(ct.Id_khacHang_do_L1) ? "Không rõ" : ct.Id_khacHang_do_L1;
                     lbl_detail_ngaydo_l1.Text = ct.NgayDo_L1.HasValue ? ct.NgayDo_L1.Value.ToString("dd/MM/yyyy HH:mm") : "Không rõ";
                         lbl_detail_seri_do_l2.Text = string.IsNullOrWhiteSpace(ct.Seri_Do_L2) ? "Không rõ" : ct.Seri_Do_L2;
                         lbl_detail_id_khachhang_do_l2.Text = string.IsNullOrWhiteSpace(ct.Id_khacHang_do_L2) ? "Không rõ" : ct.Id_khacHang_do_L2;
                         lbl_detail_ngaydo_l2.Text = ct.NgayDo_L2.HasValue ? ct.NgayDo_L2.Value.ToString("dd/MM/yyyy HH:mm") : "Không rõ";
                         lbl_detail_mota.Text = string.IsNullOrWhiteSpace(ct.Mota) ? "Không rõ" : ct.Mota;

                         txt_edit_detail_model.Text = sp == null ? "" : sp.model;
                         txt_edit_detail_thongso.Text = sp == null ? "" : sp.thongso_kythuat;
                         txt_edit_detail_makichhoat.Text = ct.MaKichHoat;
                         txt_edit_detail_thangbaohanh.Text = ct.Thang_BaoHanh;
                         txt_edit_detail_seri_do_l1.Text = ct.Seri_Do_L1;
                         txt_edit_detail_id_khachhang_do_l1.Text = ct.Id_khacHang_do_L1;
                         txt_edit_detail_ngaydo_l1.Text = ct.NgayDo_L1.HasValue ? ct.NgayDo_L1.Value.ToString("yyyy-MM-ddTHH:mm") : "";
                         txt_edit_detail_seri_do_l2.Text = ct.Seri_Do_L2;
                         txt_edit_detail_id_khachhang_do_l2.Text = ct.Id_khacHang_do_L2;
                         txt_edit_detail_ngaydo_l2.Text = ct.NgayDo_L2.HasValue ? ct.NgayDo_L2.Value.ToString("yyyy-MM-ddTHH:mm") : "";
                         txt_edit_detail_mota.Text = ct.Mota;

                     // Populate Customer Details
                     lbl_detail_tenkh.Text = bg.ten_khachhang;
                    lbl_detail_sdtkh.Text = bg.sdt_khachhang;
                    lbl_detail_diachikh.Text = bg.diachi_khachhang;

                    // Populate Order Details
                    lbl_detail_mabg.Text = bg.id.ToString();
                    lbl_detail_ngayban.Text = bg.ngayban_kyhopdong.HasValue ? bg.ngayban_kyhopdong.Value.ToString("dd/MM/yyyy HH:mm") : "Không rõ";
                    lbl_detail_soluong.Text = (ct.soluong != null ? ct.soluong.Value.ToString("#,##0") : null) ?? "0";

                    // Proportional Calculations
                    long totalSauGiamAll = db.BaoGia_ChiTiet_tbs.Where(p => p.id_baogia == baogiaId).Sum(p => (long?)p.TongSauGiam) ?? 0;
                    long itemTongSauGiam = ct.TongSauGiam ?? 0;

                    long itemShareDiscount = 0;
                    long itemFinalPrice = 0;
                    long itemShareVat = 0;
                    long itemAfterSpecialDiscount = itemTongSauGiam;

                    long bg_giamgiadacbiet = bg.giamgiadacbiet ?? 0;
                    int bg_vat = bg.vat ?? 0;

                    // Calculate special discount if quotation level has it
                    if (totalSauGiamAll > 0)
                    {
                        double ratio = (double)itemTongSauGiam / totalSauGiamAll;
                        itemShareDiscount = (long)Math.Round(bg_giamgiadacbiet * ratio);
                        itemAfterSpecialDiscount = itemTongSauGiam - itemShareDiscount;
                        
                        double itemWithVat = itemAfterSpecialDiscount * (1 + (double)bg_vat / 100);
                        itemFinalPrice = (long)Math.Round(itemWithVat);
                        itemShareVat = itemFinalPrice - itemAfterSpecialDiscount;
                    }

                    // Populate Itemised Allocations
                    lbl_detail_thanhtiengoc_sp.Text = (ct.thanhtien ?? 0).ToString("#,##0");
                    lbl_detail_giamgia_sp.Text = (ct.giamgia_thanhtien ?? 0).ToString("#,##0");
                    lbl_detail_giamgia_phantram_sp.Text = (ct.giamgia_phantram ?? 0).ToString("0.#");
                    lbl_detail_saugiam_sp.Text = itemTongSauGiam.ToString("#,##0");
                    
                    double actualPtGiamGiaDacBiet = 0;
                    if (bg.pt_giamgiadacbiet.HasValue && bg.pt_giamgiadacbiet.Value > 0)
                    {
                        actualPtGiamGiaDacBiet = bg.pt_giamgiadacbiet.Value;
                    }
                    else if (totalSauGiamAll > 0 && bg_giamgiadacbiet > 0)
                    {
                        actualPtGiamGiaDacBiet = Math.Round((double)bg_giamgiadacbiet * 100 / totalSauGiamAll, 1);
                    }
                    lbl_detail_giamgiadacbiet_phantram_sp.Text = actualPtGiamGiaDacBiet.ToString("0.#");

                    lbl_detail_giamgiadacbiet_phanbo.Text = itemShareDiscount.ToString("#,##0");
                    lbl_detail_vat_phanbo.Text = itemShareVat.ToString("#,##0");
                    lbl_detail_vat_phantram.Text = bg_vat.ToString();
                    lbl_detail_tongtiencuoi_sp.Text = itemFinalPrice.ToString("#,##0");

                    // Populate Entire Order Info
                    long orderTongGiamSpecial = bg_giamgiadacbiet;
                    lbl_detail_tongdonhang_pt_giamgiadacbiet.Text = (bg.pt_giamgiadacbiet ?? 0).ToString();
lbl_detail_tongdonhang_saugiam.Text = totalSauGiamAll.ToString("#,##0");
                    lbl_detail_tongdonhang_giamgiadacbiet.Text = orderTongGiamSpecial.ToString("#,##0");
                    
                    long orderValueAfterSpecial = totalSauGiamAll - orderTongGiamSpecial;
                    long orderVatAmount = (long)Math.Round(orderValueAfterSpecial * ((double)bg_vat / 100));
                    
                    lbl_detail_tongdonhang_vat.Text = orderVatAmount.ToString("#,##0");
                    lbl_detail_tongdonhang_vat_phantram.Text = bg_vat.ToString();
                    lbl_detail_tongdonhang_giatrithuc.Text = (bg.giatri_thuc_donhang ?? 0).ToString("#,##0");

                    // Populate Order Items list with final prices
                    var allOrderItems = (from ctItem in db.BaoGia_ChiTiet_tbs
                                         join spItem in db.KhoSanPham_tbs on ctItem.id_sanpham equals spItem.id.ToString() into spGroup
                                         from spItem in spGroup.DefaultIfEmpty()
                                         where ctItem.id_baogia == baogiaId
                                         select new
                                         {
                                             productName = spItem != null ? spItem.ten : ctItem.id_sanpham,
                                             quantity = ctItem.soluong ?? 0,
                                             itemTongSauGiam = ctItem.TongSauGiam ?? 0
                                         }).ToList();

                    var itemsWithFinalPrice = allOrderItems.Select(x =>
                    {
                        long fPrice = 0;
                        if (totalSauGiamAll > 0)
                        {
                            double ratio = (double)x.itemTongSauGiam / totalSauGiamAll;
                            double fShareDiscount = bg_giamgiadacbiet * ratio;
                            double fAfterSpecialDiscount = x.itemTongSauGiam - fShareDiscount;
                            double fWithVat = fAfterSpecialDiscount * (1 + (double)bg_vat / 100);
                            fPrice = (long)Math.Round(fWithVat);
                        }
                        return new
                        {
                            x.productName,
                            x.quantity,
                            totalPrice = fPrice
                        };
                    }).ToList();

                    rpt_order_items.DataSource = itemsWithFinalPrice;
                    rpt_order_items.DataBind();

                    // Populate Sales Revenue & Employee Info
                    var tk = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == bg.nguoibaogia);
                    lbl_detail_nguoiban.Text = tk != null ? tk.hoten : bg.nguoibaogia;
                    lbl_detail_phantramdoanhso.Text = (bg.phantram_doanhso_now != null ? bg.phantram_doanhso_now.ToString() : null) ?? "0";
                    lbl_detail_thuongdoanhso.Text = (bg.thuongdoanhso != null ? bg.thuongdoanhso.Value.ToString("#,##0") : null) ?? "0";

                    // Show panel
                    pn_detail.Visible = true;
                }
            }
        }
    }

    protected void but_close_detail_Click(object sender, EventArgs e)
    {
        pn_detail.Visible = false;
        SetProductDetailEditMode(false);
    }

    private void SetProductDetailEditMode(bool editing)
    {
        pn_detail_edit_actions.Visible = editing;
        btn_edit_product_detail.Visible = !editing;
        pn_edit_detail_model.Visible = editing;
        pn_edit_detail_makichhoat.Visible = editing;
        pn_edit_detail_thangbaohanh.Visible = editing;
        pn_edit_detail_seri_do_l1.Visible = editing;
        pn_edit_detail_id_khachhang_do_l1.Visible = editing;
        pn_edit_detail_ngaydo_l1.Visible = editing;
        pn_edit_detail_seri_do_l2.Visible = editing;
        pn_edit_detail_id_khachhang_do_l2.Visible = editing;
        pn_edit_detail_ngaydo_l2.Visible = editing;
        pn_edit_detail_mota.Visible = editing;
        pn_edit_detail_thongso.Visible = editing;

        lbl_detail_model.Visible = !editing;
        lbl_detail_makichhoat.Visible = !editing;
        lbl_detail_thangbaohanh.Visible = !editing;
        lbl_detail_seri_do_l1.Visible = !editing;
        lbl_detail_id_khachhang_do_l1.Visible = !editing;
        lbl_detail_ngaydo_l1.Visible = !editing;
        lbl_detail_seri_do_l2.Visible = !editing;
        lbl_detail_id_khachhang_do_l2.Visible = !editing;
        lbl_detail_ngaydo_l2.Visible = !editing;
        lbl_detail_mota.Visible = !editing;
        lbl_detail_thongso.Visible = !editing;
    }

    protected void btn_edit_product_detail_Click(object sender, EventArgs e)
    {
        if (ViewState["detail_baogiaId"] != null && ViewState["detail_productId"] != null)
        {
            LoadEditableProductDetailValues(
                ViewState["detail_baogiaId"].ToString(),
                ViewState["detail_productId"].ToString());
            SetProductDetailEditMode(true);
        }
    }

    protected void btn_cancel_product_detail_Click(object sender, EventArgs e)
    {
        SetProductDetailEditMode(false);
    }

    protected void btn_save_product_detail_Click(object sender, EventArgs e)
    {
        if (ViewState["detail_baogiaId"] == null || ViewState["detail_productId"] == null)
            return;

        string baogiaId = ViewState["detail_baogiaId"].ToString();
        string productId = ViewState["detail_productId"].ToString();
        DateTime parsedDate;

        using (var db = new dbDataContext())
        {
            var ct = db.BaoGia_ChiTiet_tbs.FirstOrDefault(p => p.id_baogia == baogiaId && p.id_sanpham == productId);
            long productIdValue;
            long.TryParse(productId, out productIdValue);
            var sp = db.KhoSanPham_tbs.FirstOrDefault(p => p.id == productIdValue);

            if (ct == null)
                return;

            if (sp != null)
            {
                sp.model = txt_edit_detail_model.Text.Trim();
                sp.thongso_kythuat = txt_edit_detail_thongso.Text.Trim();
            }

            ct.MaKichHoat = txt_edit_detail_makichhoat.Text.Trim();
            ct.Thang_BaoHanh = txt_edit_detail_thangbaohanh.Text.Trim();
            ct.Seri_Do_L1 = txt_edit_detail_seri_do_l1.Text.Trim();
            ct.Id_khacHang_do_L1 = txt_edit_detail_id_khachhang_do_l1.Text.Trim();
            ct.NgayDo_L1 = DateTime.TryParse(txt_edit_detail_ngaydo_l1.Text, out parsedDate) ? (DateTime?)parsedDate : null;
            ct.Seri_Do_L2 = txt_edit_detail_seri_do_l2.Text.Trim();
            ct.Id_khacHang_do_L2 = txt_edit_detail_id_khachhang_do_l2.Text.Trim();
            ct.NgayDo_L2 = DateTime.TryParse(txt_edit_detail_ngaydo_l2.Text, out parsedDate) ? (DateTime?)parsedDate : null;
            ct.Mota = txt_edit_detail_mota.Text.Trim();

            db.SubmitChanges();
        }

        SetProductDetailEditMode(false);
        show_main();
        RefreshEditableProductDetailLabels(baogiaId, productId);
    }

    private void LoadEditableProductDetailValues(string baogiaId, string productId)
    {
        using (var db = new dbDataContext())
        {
            long productIdValue;
            long.TryParse(productId, out productIdValue);
            var ct = db.BaoGia_ChiTiet_tbs.FirstOrDefault(p => p.id_baogia == baogiaId && p.id_sanpham == productId);
            var sp = db.KhoSanPham_tbs.FirstOrDefault(p => p.id == productIdValue);
            if (ct == null) return;

            // Luôn lấy lại dữ liệu hiện tại trước khi mở form để ô không sửa vẫn giữ nguyên.
            txt_edit_detail_model.Text = sp == null ? "" : sp.model;
            txt_edit_detail_thongso.Text = sp == null ? "" : sp.thongso_kythuat;
            txt_edit_detail_makichhoat.Text = ct.MaKichHoat;
            txt_edit_detail_thangbaohanh.Text = ct.Thang_BaoHanh;
            txt_edit_detail_seri_do_l1.Text = ct.Seri_Do_L1;
            txt_edit_detail_id_khachhang_do_l1.Text = ct.Id_khacHang_do_L1;
            txt_edit_detail_ngaydo_l1.Text = ct.NgayDo_L1.HasValue ? ct.NgayDo_L1.Value.ToString("yyyy-MM-ddTHH:mm") : "";
            txt_edit_detail_seri_do_l2.Text = ct.Seri_Do_L2;
            txt_edit_detail_id_khachhang_do_l2.Text = ct.Id_khacHang_do_L2;
            txt_edit_detail_ngaydo_l2.Text = ct.NgayDo_L2.HasValue ? ct.NgayDo_L2.Value.ToString("yyyy-MM-ddTHH:mm") : "";
            txt_edit_detail_mota.Text = ct.Mota;
        }
    }

    private void RefreshEditableProductDetailLabels(string baogiaId, string productId)
    {
        using (var db = new dbDataContext())
        {
            long productIdValue;
            long.TryParse(productId, out productIdValue);
            var ct = db.BaoGia_ChiTiet_tbs.FirstOrDefault(p => p.id_baogia == baogiaId && p.id_sanpham == productId);
            var sp = db.KhoSanPham_tbs.FirstOrDefault(p => p.id == productIdValue);
            if (ct == null) return;

            lbl_detail_model.Text = sp == null || string.IsNullOrWhiteSpace(sp.model) ? "Không rõ" : sp.model;
            lbl_detail_thongso.Text = sp == null || string.IsNullOrWhiteSpace(sp.thongso_kythuat) ? "Không rõ" : sp.thongso_kythuat;
            lbl_detail_makichhoat.Text = string.IsNullOrWhiteSpace(ct.MaKichHoat) ? "Không rõ" : ct.MaKichHoat;
            lbl_detail_thangbaohanh.Text = string.IsNullOrWhiteSpace(ct.Thang_BaoHanh) ? "Không rõ" : ct.Thang_BaoHanh;
            lbl_detail_seri_do_l1.Text = string.IsNullOrWhiteSpace(ct.Seri_Do_L1) ? "Không rõ" : ct.Seri_Do_L1;
            lbl_detail_id_khachhang_do_l1.Text = string.IsNullOrWhiteSpace(ct.Id_khacHang_do_L1) ? "Không rõ" : ct.Id_khacHang_do_L1;
            lbl_detail_ngaydo_l1.Text = ct.NgayDo_L1.HasValue ? ct.NgayDo_L1.Value.ToString("dd/MM/yyyy HH:mm") : "Không rõ";
            lbl_detail_seri_do_l2.Text = string.IsNullOrWhiteSpace(ct.Seri_Do_L2) ? "Không rõ" : ct.Seri_Do_L2;
            lbl_detail_id_khachhang_do_l2.Text = string.IsNullOrWhiteSpace(ct.Id_khacHang_do_L2) ? "Không rõ" : ct.Id_khacHang_do_L2;
            lbl_detail_ngaydo_l2.Text = ct.NgayDo_L2.HasValue ? ct.NgayDo_L2.Value.ToString("dd/MM/yyyy HH:mm") : "Không rõ";
            lbl_detail_mota.Text = string.IsNullOrWhiteSpace(ct.Mota) ? "Không rõ" : ct.Mota;
        }
    }

    protected void btn_edit_warranty_Click(object sender, EventArgs e)
    {
        LinkButton btn = (LinkButton)sender;
        string[] args = btn.CommandArgument.Split('|');
        if (args.Length == 2)
        {
            string baogiaId = args[0];
            string productId = args[1];

            ViewState["edit_baogiaId"] = baogiaId;
            ViewState["edit_productId"] = productId;

            using (var db = new dbDataContext())
            {
                var ct = db.BaoGia_ChiTiet_tbs.FirstOrDefault(p => p.id_baogia == baogiaId && p.id_sanpham == productId);
                if (ct != null)
                {
                    txt_edit_thangbaohanh.Text = ct.Thang_BaoHanh;
                    pn_edit_warranty.Visible = true;
                }
            }
        }
    }

    protected void but_close_edit_warranty_Click(object sender, EventArgs e)
    {
        pn_edit_warranty.Visible = false;
    }

    protected void btn_save_warranty_Click(object sender, EventArgs e)
    {
        if (ViewState["edit_baogiaId"] != null && ViewState["edit_productId"] != null)
        {
            string baogiaId = ViewState["edit_baogiaId"].ToString();
            string productId = ViewState["edit_productId"].ToString();

            using (var db = new dbDataContext())
            {
                var ct = db.BaoGia_ChiTiet_tbs.FirstOrDefault(p => p.id_baogia == baogiaId && p.id_sanpham == productId);
                if (ct != null)
                {
                    ct.Thang_BaoHanh = txt_edit_thangbaohanh.Text;
                    db.SubmitChanges();
                    
                    pn_edit_warranty.Visible = false;
                    show_main();
                }
            }
        }
    }
}
