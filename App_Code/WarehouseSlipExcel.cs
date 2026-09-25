using System;
using System.Collections.Generic;
using OfficeOpenXml;
using OfficeOpenXml.Style;

public sealed class WarehouseSlipItem
{
    public string Name, Serial, Unit;
    public decimal Quantity;
}

public static class WarehouseSlipExcel
{
    public static byte[] Create(string number, string quoteDate, string printedDate,
        string customer, string phone, string address, string preparedBy, IList<WarehouseSlipItem> items)
    {
        // Follow the EPPlus license configuration already used by this application.
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (var package = new ExcelPackage())
        {
            var sheet = package.Workbook.Worksheets.Add("Phiếu xuất kho");
            sheet.View.ShowGridLines = false;
            sheet.Cells.Style.Font.Name = "Arial";
            sheet.Cells.Style.Font.Size = 11;
            sheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            double[] widths = { 7, 38, 27, 10, 13, 24 };
            for (int c = 1; c <= 6; c++) sheet.Column(c).Width = widths[c - 1];
            MergeText(sheet, 1, 1, 6, "Đơn vị: ................................................................................", true);
            MergeText(sheet, 3, 1, 6, "PHIẾU XUẤT KHO", true);
            sheet.Cells[3, 1].Style.Font.Size = 20;
            sheet.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Row(3).Height = 32;
            MergeText(sheet, 4, 1, 6, "Theo báo giá số " + number + " · Ngày báo giá: " + quoteDate, false);
            MergeText(sheet, 5, 1, 6, "Ngày lập phiếu: " + printedDate, false);
            sheet.Cells[4, 1, 5, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            MergeText(sheet, 7, 1, 6, "Khách hàng / Người nhận: " + customer, false);
            MergeText(sheet, 8, 1, 6, "Điện thoại: " + phone, false);
            MergeText(sheet, 9, 1, 6, "Địa chỉ: " + address, false);
            MergeText(sheet, 10, 1, 6, "Lý do xuất: Giao hàng theo báo giá số " + number + ".", false);
            MergeText(sheet, 11, 1, 6, "Xuất tại kho: ................................................................................", false);
            string[] headers = { "STT", "Tên hàng hóa", "Số seri", "ĐVT", "Số lượng", "Ghi chú" };
            for (int c = 1; c <= 6; c++) sheet.Cells[13, c].Value = headers[c - 1];
            sheet.Cells[13, 1, 13, 6].Style.Font.Bold = true;
            sheet.Cells[13, 1, 13, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Row(13).Height = 30;
            int row = 14;
            foreach (var item in items)
            {
                sheet.Cells[row, 1].Value = row - 13;
                sheet.Cells[row, 2].Value = item.Name;
                // Text preserves leading zeroes and never interprets user content as formulas.
                sheet.Cells[row, 3].Style.Numberformat.Format = "@";
                sheet.Cells[row, 3].Value = item.Serial;
                sheet.Cells[row, 4].Value = item.Unit;
                sheet.Cells[row, 5].Value = item.Quantity;
                sheet.Cells[row, 5].Style.Numberformat.Format = "#,##0.########";
                sheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[row, 1, row, 6].Style.WrapText = true;
                sheet.Row(row).Height = Math.Max(30, Math.Max(Lines(item.Name, 32), Lines(item.Serial, 23)) * 16 + 8);
                row++;
            }
            MergeText(sheet, row, 1, 4, "Tổng số lượng", true);
            sheet.Cells[row, 5].Formula = "SUM(E14:E" + (row - 1) + ")";
            sheet.Cells[row, 5].Style.Numberformat.Format = "#,##0.########";
            sheet.Cells[row, 5].Style.Font.Bold = true;
            for (int r = 13; r <= row; r++)
                for (int c = 1; c <= 6; c++)
                {
                    var border = sheet.Cells[r, c].Style.Border;
                    border.Top.Style = border.Bottom.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;
                }
            int signature = row + 3;
            MergeText(sheet, signature, 1, 2, "Người lập phiếu", true);
            MergeText(sheet, signature, 3, 4, "Người nhận hàng", true);
            MergeText(sheet, signature, 5, 6, "Thủ kho", true);
            for (int c = 1; c <= 5; c += 2)
            {
                MergeText(sheet, signature + 1, c, c + 1, "(Ký, họ tên)", false);
                sheet.Cells[signature + 1, c].Style.Font.Italic = true;
            }
            MergeText(sheet, signature + 5, 1, 2, preparedBy, false);
            sheet.Cells[signature, 1, signature + 5, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.View.FreezePanes(14, 1);
            sheet.PrinterSettings.PaperSize = ePaperSize.A4;
            sheet.PrinterSettings.Orientation = eOrientation.Portrait;
            sheet.PrinterSettings.FitToPage = true;
            sheet.PrinterSettings.FitToWidth = 1;
            sheet.PrinterSettings.FitToHeight = 0;
            sheet.PrinterSettings.RepeatRows = sheet.Cells["13:13"];
            sheet.PrinterSettings.PrintArea = sheet.Cells[1, 1, signature + 5, 6];
            sheet.PrinterSettings.LeftMargin = sheet.PrinterSettings.RightMargin = 0.3M;
            sheet.PrinterSettings.TopMargin = sheet.PrinterSettings.BottomMargin = 0.4M;
            sheet.Calculate();
            return package.GetAsByteArray();
        }
    }

    private static int Lines(string text, int width)
    {
        int lines = 0;
        foreach (string line in (text ?? "").Replace("\r", "").Split('\n'))
            lines += Math.Max(1, (int)Math.Ceiling(line.Length / (double)width));
        return lines;
    }

    private static void MergeText(ExcelWorksheet sheet, int row, int start, int end, string text, bool bold)
    {
        sheet.Cells[row, start, row, end].Merge = true;
        sheet.Cells[row, start].Value = text;
        sheet.Cells[row, start, row, end].Style.WrapText = true;
        sheet.Cells[row, start].Style.Font.Bold = bold;
        sheet.Row(row).Height = Math.Max(sheet.Row(row).Height, Lines(text, (end - start + 1) * 17) * 16 + 6);
    }
}
