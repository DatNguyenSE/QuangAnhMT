using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_theo_doi_hang_da_ban_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["url_back"] = HttpContext.Current.Request.Url.AbsoluteUri;
            check_login_cl.check_login_admin("41", "41"); // check view sold products tracking permission
            
            ViewState["current_page"] = "1";
            show_main();
        }
    }

    public void show_main()
    {
        using (dbDataContext db = new dbDataContext())
        {
            // 1. Fetch raw query data
            var rawQuery = from ct in db.BaoGia_ChiTiet_tbs
                           join bg in db.BaoGia_tbs on ct.id_baogia equals bg.id.ToString()
                           join sp in db.KhoSanPham_tbs on ct.id_sanpham equals sp.id.ToString() into spGroup
                           from sp in spGroup.DefaultIfEmpty()
                           where bg.ngayban_kyhopdong != null
                           select new
                           {
                               productId = sp != null ? sp.id.ToString() : "",
                               productName = sp != null ? sp.ten : "Sản phẩm tự chọn",
                               productModel = sp != null ? sp.model : "",
                               productSerial = sp != null ? sp.so_seri : "",
                               productImage = sp != null ? sp.anh : "",
                               quantity = ct.soluong ?? 0,
                               price = ct.giaban_taithoidiemnay ?? 0,
                               itemTongSauGiam = ct.TongSauGiam ?? 0,
                               baogiaId = bg.id,
                               ngayban = bg.ngayban_kyhopdong,
                               tenKhachHang = bg.ten_khachhang,
                               sdtKhachHang = bg.sdt_khachhang,
                               diachiKhachHang = bg.diachi_khachhang,
                               nguoiBan = bg.nguoibaogia,
                               vat = bg.vat ?? 0,
                               giamgiadacbiet = bg.giamgiadacbiet ?? 0
                           };

            // Search Filter
            string key = txt_timkiem.Text.Trim();
            if (string.IsNullOrEmpty(key))
            {
                key = txt_timkiem1.Text.Trim();
            }
            if (!string.IsNullOrEmpty(key))
            {
                rawQuery = rawQuery.Where(p => p.productName.Contains(key) || 
                                               p.productModel.Contains(key) || 
                                               p.productSerial.Contains(key) || 
                                               p.tenKhachHang.Contains(key) || 
                                               p.sdtKhachHang.Contains(key) ||
                                               p.baogiaId.ToString() == key);
            }

            var rawList = rawQuery.OrderByDescending(p => p.ngayban).ToList();

            // 2. Group in memory to calculate total itemTongSauGiam per quotation
            var totalsByBaoGia = rawList
                .GroupBy(x => x.baogiaId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.itemTongSauGiam));

            // 3. Project in memory to calculate proportional discount and VAT for each item
            var processedList = rawList.Select(x =>
            {
                long totalSauGiamAll = totalsByBaoGia[x.baogiaId];
                long itemFinalPrice = 0;

                if (totalSauGiamAll > 0)
                {
                    double ratio = (double)x.itemTongSauGiam / totalSauGiamAll;
                    double itemShareDiscount = x.giamgiadacbiet * ratio;
                    double itemAfterSpecialDiscount = x.itemTongSauGiam - itemShareDiscount;
                    double itemWithVat = itemAfterSpecialDiscount * (1 + (double)x.vat / 100);
                    itemFinalPrice = (long)Math.Round(itemWithVat);
                }
                else
                {
                    itemFinalPrice = 0;
                }

                return new
                {
                    x.productId,
                    x.productName,
                    x.productModel,
                    x.productSerial,
                    x.productImage,
                    x.quantity,
                    x.price,
                    x.itemTongSauGiam,
                    totalPrice = itemFinalPrice, // final total price after discount and tax!
                    x.baogiaId,
                    x.ngayban,
                    x.tenKhachHang,
                    x.sdtKhachHang,
                    x.diachiKhachHang,
                    x.nguoiBan
                };
            }).ToList();

            int totalRecords = processedList.Count();
            int pageSize = Number_cl.Check_Int(txt_show.Text.Trim());
            if (pageSize <= 0) pageSize = 10;

            int currentPage = int.Parse(ViewState["current_page"].ToString());
            int totalPages = number_of_page_class.return_total_page(totalRecords, pageSize);
            if (currentPage < 1) currentPage = 1;
            else if (currentPage > totalPages && totalPages > 0) currentPage = totalPages;
            
            ViewState["current_page"] = currentPage;
            ViewState["total_page"] = totalPages;

            // Enable/disable page buttons
            btn_prev.Enabled = (currentPage > 1);
            btn_next.Enabled = (currentPage < totalPages);

            // Pagination
            var pagedList = processedList.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            Repeater1.DataSource = pagedList;
            Repeater1.DataBind();

            tr_empty.Visible = (totalRecords == 0);

            // Display page info
            int startRecord = (currentPage - 1) * pageSize + 1;
            int endRecord = startRecord + pagedList.Count - 1;
            if (totalRecords > 0)
            {
                lbl_page_info.Text = startRecord + "-" + endRecord + " trong số " + totalRecords.ToString("#,##0") + " sản phẩm đã bán";
            }
            else
            {
                lbl_page_info.Text = "0-0 trong số 0 sản phẩm";
            }
        }
    }

    protected void txt_timkiem_TextChanged(object sender, EventArgs e)
    {
        ViewState["current_page"] = "1";
        show_main();
    }

    protected void txt_show_TextChanged(object sender, EventArgs e)
    {
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
        int totalPages = int.Parse(ViewState["total_page"]?.ToString() ?? "1");
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
                var bg = db.BaoGia_tbs.FirstOrDefault(p => p.id.ToString() == baogiaId);
                var ct = db.BaoGia_ChiTiet_tbs.FirstOrDefault(p => p.id_baogia == baogiaId && p.id_sanpham == productId);
                var sp = db.KhoSanPham_tbs.FirstOrDefault(p => p.id.ToString() == productId);

                if (bg != null && ct != null)
                {
                    // Populate Product Info
                    if (sp != null)
                    {
                        img_detail_sp.ImageUrl = string.IsNullOrEmpty(sp.anh) ? "/uploads/images/no-image.png" : sp.anh;
                        lbl_detail_tensp.Text = sp.ten;
                        lbl_detail_model.Text = sp.model;
                        lbl_detail_seri.Text = string.IsNullOrEmpty(sp.so_seri) ? "N/A" : sp.so_seri;
                        lbl_detail_thongso.Text = string.IsNullOrEmpty(sp.thongso_kythuat) ? "N/A" : sp.thongso_kythuat;
                    }
                    else
                    {
                        img_detail_sp.ImageUrl = "/uploads/images/no-image.png";
                        lbl_detail_tensp.Text = "Sản phẩm tự chọn";
                        lbl_detail_model.Text = "N/A";
                        lbl_detail_seri.Text = "N/A";
                        lbl_detail_thongso.Text = "N/A";
                    }

                    // Populate Customer Details
                    lbl_detail_tenkh.Text = bg.ten_khachhang;
                    lbl_detail_sdtkh.Text = bg.sdt_khachhang;
                    lbl_detail_diachikh.Text = bg.diachi_khachhang;

                    // Populate Order Details
                    lbl_detail_mabg.Text = bg.id.ToString();
                    lbl_detail_ngayban.Text = bg.ngayban_kyhopdong.HasValue ? bg.ngayban_kyhopdong.Value.ToString("dd/MM/yyyy HH:mm") : "N/A";
                    lbl_detail_soluong.Text = ct.soluong?.ToString("#,##0") ?? "0";

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
                                             productName = spItem != null ? spItem.ten : "Sản phẩm tự chọn",
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
                    lbl_detail_nguoiban.Text = bg.nguoibaogia;
                    lbl_detail_phantramdoanhso.Text = bg.phantram_doanhso_now?.ToString() ?? "0";
                    lbl_detail_thuongdoanhso.Text = bg.thuongdoanhso?.ToString("#,##0") ?? "0";

                    // Show panel
                    pn_detail.Visible = true;
                }
            }
        }
    }

    protected void but_close_detail_Click(object sender, EventArgs e)
    {
        pn_detail.Visible = false;
    }
}
