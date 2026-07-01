using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;


/// <summary>
/// Summary description for ExportExcel
/// </summary>
public static class ExportExcel
{

    public static void ExportToExcel(List<IDictionary<string, object>> listData, XSSFWorkbook workbook)
    {
        //XSSFWorkbook workbook = new XSSFWorkbook();
        ISheet sheet = workbook.CreateSheet("Danh sách");

        // Ghi dòng tiêu đề
        IRow headerRow = sheet.CreateRow(0);
        var firstItem = listData.FirstOrDefault();
        if (firstItem == null) return;

        int colIndex = 0;
        foreach (var prop in ((IDictionary<string, object>)firstItem))
        {
            headerRow.CreateCell(colIndex++).SetCellValue(prop.Key);
        }

        // Ghi dữ liệu
        for (int i = 0; i < listData.Count; i++)
        {
            var row = sheet.CreateRow(i + 1);
            var item = (IDictionary<string, object>)listData[i];

            int j = 0;
            foreach (var val in item.Values)
            {
                if (val != null)
                    row.CreateCell(j).SetCellValue(val.ToString());
                j++;
            }
        }

    }

    public static void ExportToExcelFormat(List<IDictionary<string, object>> listData, Dictionary<string, string> headers, XSSFWorkbook workbook)
    {
        ISheet sheet = workbook.CreateSheet("Danh sách");

        // Tạo font đậm cho header
        IFont boldFont = workbook.CreateFont();
        boldFont.IsBold = true;

        // Style cho header: in đậm, căn giữa
        ICellStyle headerStyle = workbook.CreateCellStyle();
        headerStyle.SetFont(boldFont);
        headerStyle.Alignment = HorizontalAlignment.Center;
        headerStyle.VerticalAlignment = VerticalAlignment.Center;

        // Style ngày tháng (dd/MM/yyyy)
        ICellStyle dateStyle = workbook.CreateCellStyle();
        short dateFormat = workbook.CreateDataFormat().GetFormat("dd/MM/yyyy");
        dateStyle.DataFormat = dateFormat;

        // Style số (2 chữ số sau dấu chấm)
        ICellStyle numberStyle = workbook.CreateCellStyle();
        short numberFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
        numberStyle.DataFormat = numberFormat;

        ICellStyle timeStyle = workbook.CreateCellStyle();
        short timeFormat = workbook.CreateDataFormat().GetFormat("HH:mm");
        timeStyle.DataFormat = timeFormat;

        // Ghi dòng tiêu đề
        IRow headerRow = sheet.CreateRow(0);
        int colIndex = 0;
        foreach (var kv in headers)
        {
            var cell = headerRow.CreateCell(colIndex++);
            cell.SetCellValue(kv.Value); // Tiêu đề hiển thị
            cell.CellStyle = headerStyle;
        }

        // Ghi dữ liệu
        for (int i = 0; i < listData.Count; i++)
        {
            var row = sheet.CreateRow(i + 1);
            var item = listData[i];
            int j = 0;

            foreach (var key in headers.Keys)
            {
                if (item.ContainsKey(key) && item[key] != null)
                {
                    var cell = row.CreateCell(j);
                    var value = item[key];

                    if (value is DateTime dt)
                    {

                        // Nếu cột là giờ-phút
                        if (key == "giobatdau" || key == "gioketthuc")
                        {
                            cell.SetCellValue(dt);
                            cell.CellStyle = timeStyle;
                        }
                        // Nếu cột là ngày-tháng
                        else if (key == "gionhan")
                        {
                            cell.SetCellValue(dt);
                            cell.CellStyle = dateStyle;
                        }
                        else
                        {
                            // Nếu không rõ thì để kiểu mặc định toàn bộ datetime
                            cell.SetCellValue(dt.ToString("dd/MM/yyyy HH:mm"));
                        }
                        //cell.SetCellValue(dt);
                        //cell.CellStyle = dateStyle;
                    }
                    else if (value is double || value is float || value is decimal)
                    {
                        cell.SetCellValue(Convert.ToDouble(value));
                        cell.CellStyle = numberStyle;
                    }
                    else
                    {
                        cell.SetCellValue(value.ToString());
                    }
                }
                j++;
            }
        }

        // Auto-size cột
        for (int i = 0; i < headers.Count; i++)
        {
            sheet.AutoSizeColumn(i);
        }


    }

}