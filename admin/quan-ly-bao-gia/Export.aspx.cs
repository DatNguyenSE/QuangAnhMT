using NPOI.XSSF.UserModel;
using QRCoder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Export : System.Web.UI.Page
{
    

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string id = Request.QueryString["id"];
            if (!string.IsNullOrEmpty(id))
            {
                XuatExcelbyID(id); // Gọi lại logic của bạn ở đây
            }
        }
    }


    void XuatExcelbyID(string id_BG)
    {
        //LinkButton button = (LinkButton)sender;
        //string id_BG = button.CommandArgument;

        using (dbDataContext db = new dbDataContext())
        {
            // Lấy chi tiết báo giá
            var q_chitiet = load_main(db, id_BG).ToList();

            // Chuyển chi tiết sang list dictionary
            var list_dynamic = q_chitiet.Select(x =>
            {
                var dict = new Dictionary<string, object>();
                foreach (var prop in x.GetType().GetProperties())
                {
                    dict[prop.Name] = prop.GetValue(x, null);
                }
                return dict;
            }).ToList();

            // Lấy thông tin chung
            string tenkh = "", sdtkh = "", diachikh = "", ngaybg = "", hanbg = "", sobg = "", nhanvienbg = "", sdtnv = "";
            string giamgia = "0", vat = "0";

            var q = db.BaoGia_tbs.FirstOrDefault(p => p.id.ToString().ToLower() == id_BG.ToLower());
            if (q != null)
            {
                tenkh = q.ten_khachhang;
                sdtkh = q.sdt_khachhang;
                diachikh = q.diachi_khachhang;
                ngaybg = q.ngaybaogia?.ToString("dd/MM/yyyy");
                hanbg = q.ngayhethan?.ToString("dd/MM/yyyy");
                sobg = q.id.ToString();

                var q_nv = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == q.nguoibaogia);
                if (q_nv != null)
                {
                    nhanvienbg = q_nv.hoten;
                    sdtnv = q_nv.dienthoai;
                }

                giamgia = (q.giamgiadacbiet ?? 0).ToString("#,##0");
                vat = (q.vat ?? 0).ToString("#,##0");
                ViewState["giamgia_dacbiet"] = q.giamgiadacbiet ?? 0;
                ViewState["vat_chitiet"] = q.vat ?? 0;
            }

            // Ghi thông tin báo giá dạng cột (1 key mỗi cột, value dòng sau)
            var info = new Dictionary<string, string>
            {
                { "Khách hàng", tenkh },
                { "SĐT Khách hàng", sdtkh },
                { "Địa chỉ", diachikh },
                { "Ngày báo giá", ngaybg },
                { "Hạn báo giá", hanbg },
                { "Số BG", sobg },
                { "Nhân viên báo giá", nhanvienbg },
                { "SĐT nhân viên", sdtnv },
                { "Giảm giá đặc biệt", giamgia },
                { "VAT (%)", vat },
                { "Tổng tiền trước thuế", ViewState["TongSauGiam_ChiTiet"]?.ToString() ?? "0" },
                { "Tiền VAT", ViewState["thanhtien_vat_chitiet"]?.ToString() ?? "0" },
                { "Tổng thanh toán", ViewState["donhang_saugiamgia"]?.ToString() ?? "0" }
            };

            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet("Báo giá");

            // Style: font in đậm
            var boldFont = workbook.CreateFont();
            boldFont.IsBold = true;
            var boldStyle = workbook.CreateCellStyle();
            boldStyle.SetFont(boldFont);

            // Style định dạng tiền
            var currencyStyle = workbook.CreateCellStyle();
            currencyStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0");

            // Style định dạng ngày tháng
            var dateStyle = workbook.CreateCellStyle();
            dateStyle.DataFormat = workbook.CreateDataFormat().GetFormat("dd/MM/yyyy");

            // Ghi info vào dòng 0 và 1 (theo cột)
            var rowTitle = sheet.CreateRow(0);
            var rowValue = sheet.CreateRow(1);
            int infoCol = 0;
            foreach (var item in info)
            {
                var cellKey = rowTitle.CreateCell(infoCol);
                cellKey.SetCellValue(item.Key);
                cellKey.CellStyle = boldStyle;

                var cellVal = rowValue.CreateCell(infoCol);
                // Gán theo key, để xác định kiểu dữ liệu đặc biệt
                string key = item.Key;
                string val = item.Value;

                if (key.Contains("SĐT") || key.ToLower().Contains("sdt"))
                {
                    cellVal.SetCellValue(val); // giữ nguyên chuỗi, không định dạng số
                }
                else if (DateTime.TryParseExact(val, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dt))
                {
                    cellVal.SetCellValue(dt);
                    cellVal.CellStyle = dateStyle;
                }
                else if (decimal.TryParse(val, out decimal num))
                {
                    cellVal.SetCellValue((double)num);
                    cellVal.CellStyle = currencyStyle;
                }
                else
                {
                    cellVal.SetCellValue(val);
                }

                infoCol++;
            }

            // Header sản phẩm bắt đầu từ dòng 3
            int detailStartRow = 3;

            var headers = new Dictionary<string, string>
            {
                { "id_baogia", "ID Báo giá" },
                { "ten_sanpham", "Tên sản phẩm" },
                { "TenHang", "Tên hãng" },
                { "thongso_kythuat", "Thông số kỹ thuật" },
                { "giaban_taithoidiemnay", "Giá bán" },
                { "DVT", "Đơn vị tính" },
                { "soluong", "Số lượng" },
                { "thanhtien", "Thành tiền" },
                { "giamgia_phantram", "Giảm giá (%)" },
                { "giamgia_thanhtien", "Giảm giá (VNĐ)" },
            };

            // Ghi header chi tiết sản phẩm
            var headerRow = sheet.CreateRow(detailStartRow);
            int colHeader = 0;
            foreach (var header in headers.Values)
            {
                var cell = headerRow.CreateCell(colHeader++);
                cell.SetCellValue(header);
                cell.CellStyle = boldStyle;
            }

            // Ghi data chi tiết sản phẩm
            int dataRow = detailStartRow + 1;
            foreach (var item in list_dynamic)
            {
                var row = sheet.CreateRow(dataRow++);
                int col = 0;
                foreach (var key in headers.Keys)
                {
                    var value = item.ContainsKey(key) ? item[key] : null;
                    var cell = row.CreateCell(col++);

                    if (value == null)
                    {
                        cell.SetCellValue("");
                    }
                    else if (value is DateTime dt)
                    {
                        cell.SetCellValue(dt);
                        cell.CellStyle = dateStyle;
                    }
                    else if (decimal.TryParse(value.ToString(), out decimal num))
                    {
                        cell.SetCellValue((double)num);
                        cell.CellStyle = currencyStyle;
                    }
                    else
                    {
                        cell.SetCellValue(value.ToString());
                    }
                }
            }

            // Tự resize cột
            for (int i = 0; i < info.Count + headers.Count; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            // Lưu file
            string folderPath = Server.MapPath("~/uploads/Files/");
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            string fileName = Guid.NewGuid() + ".xlsx";
            string filePath = Path.Combine(folderPath, fileName);
            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                workbook.Write(fs);
            }

            Response.Redirect("/uploads/Files/" + fileName);

        }
    }


    IQueryable<object> load_main(dbDataContext db, string _idbg)
    {
        var q_chitiet = from chitiet in db.BaoGia_ChiTiet_tbs
                        join sanpham in db.KhoSanPham_tbs
                        on chitiet.id_sanpham equals sanpham.id.ToString() into sanphamGroup
                        from sanpham in sanphamGroup.DefaultIfEmpty()
                        join ob4 in db.DuLieuNguon_tbs.Where(p => p.kyhieu == "donvitinh")
                        on sanpham.donvitinh equals ob4.id.ToString() into DVTGroup
                        from ob4 in DVTGroup.DefaultIfEmpty()
                        join ob2 in db.DuLieuNguon_tbs.Where(p => p.kyhieu == "hangsanpham")
                        on sanpham.id_hang equals ob2.id.ToString() into HangGroup
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
                            ten_sanpham = sanpham != null ? sanpham.ten : "",
                            anh = sanpham != null ? sanpham.anh : "",
                            DVT = ob4 != null ? ob4.ten : "",
                            thongso_kythuat = sanpham != null ? sanpham.thongso_kythuat : "",
                            TenHang = ob2 != null ? ob2.ten : ""
                        };

        if (q_chitiet.Any())
        {
            long tongThanhTien = q_chitiet.Sum(p => p.thanhtien ?? 0);
            long tongGiam = q_chitiet.Sum(p => p.giamgia_thanhtien ?? 0);
            long tongSauGiam = q_chitiet.Sum(p => p.TongSauGiam ?? 0);

            ViewState["TongThanhTien_ChiTiet"] = tongThanhTien.ToString("#,##0");
            ViewState["TongGiam_ChiTiet"] = tongGiam.ToString("#,##0");
            ViewState["TongSauGiam_ChiTiet"] = tongSauGiam.ToString("#,##0");

            // Parse giảm giá đặc biệt và VAT từ ViewState
            long giamGiaDacBiet = 0;
            decimal vatRate = 0;

            if (ViewState["giamgia_dacbiet"] != null)
                long.TryParse(ViewState["giamgia_dacbiet"].ToString(), out giamGiaDacBiet);

            if (ViewState["vat_chitiet"] != null)
            {
                decimal vatPercent;
                if (decimal.TryParse(ViewState["vat_chitiet"].ToString(), out vatPercent))
                {
                    vatRate = vatPercent / 100;
                }
            }

            // Tính toán
            decimal tongTruocVAT = tongSauGiam - giamGiaDacBiet;
            decimal tienVAT = tongTruocVAT * vatRate;
            decimal tongSauGiamVaVAT = tongTruocVAT * (1 + vatRate);

            ViewState["thanhtien_vat_chitiet"] = Math.Round(tienVAT, 0).ToString("#,##0");
            ViewState["donhang_saugiamgia"] = Math.Round(tongSauGiamVaVAT, 0);
        }
        else
        {
            ViewState["TongThanhTien_ChiTiet"] = "0";
            ViewState["TongGiam_ChiTiet"] = "0";
            ViewState["TongSauGiam_ChiTiet"] = "0";
            ViewState["thanhtien_vat_chitiet"] = "0";
            ViewState["donhang_saugiamgia"] = "0";
        }

        return q_chitiet;
    }



}