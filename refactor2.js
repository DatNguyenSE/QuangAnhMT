const fs = require('fs');

function replaceAll(filePath, replacements) {
    let content = fs.readFileSync(filePath, 'utf8');
    let newContent = content;
    for (let r of replacements) {
        newContent = newContent.split(r.find).join(r.replace);
    }
    if (newContent !== content) {
        fs.writeFileSync(filePath, newContent, 'utf8');
        console.log("Updated " + filePath);
    }
}

// 1. guiEmail_cl.cs
replaceAll("App_Code/guiEmail_cl.cs", [
    {find: "using static OfficeOpenXml.ExcelErrorValue;", replace: "// using static OfficeOpenXml.ExcelErrorValue;"}
]);

// 2. DateTime_cl.cs
replaceAll("App_Code/DateTime_cl.cs", [
    {find: "return DateTime.TryParse(_date, out _);", replace: "DateTime dummy;\n        return DateTime.TryParse(_date, out dummy);"}
]);

// 3. Default.aspx.cs discards & pattern matching
replaceAll("admin/quan-ly-bao-gia/Default.aspx.cs", [
    {find: "var productIdsLong = productIdsStr.Where(s => long.TryParse(s, out _)).Select(s => long.Parse(s)).ToList();", 
     replace: "var productIdsLong = productIdsStr.Where(s => { long dummy; return long.TryParse(s, out dummy); }).Select(s => long.Parse(s)).ToList();"},
     
    {find: `string productId = ViewState["quick_quote_product"]?.ToString();
                        if (long.TryParse(productId, out long quickProductId))`,
     replace: `string productId = ViewState["quick_quote_product"] != null ? ViewState["quick_quote_product"].ToString() : null;
                        long quickProductId;
                        if (long.TryParse(productId, out quickProductId))`},

    {find: `if (key.Contains("SĐT") || key.ToLower().Contains("sdt"))
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
                }`,
     replace: `DateTime dt;
                decimal num;
                if (key.Contains("SĐT") || key.ToLower().Contains("sdt"))
                {
                    cellVal.SetCellValue(val); // giữ nguyên chuỗi, không định dạng số
                }
                else if (DateTime.TryParseExact(val, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dt))
                {
                    cellVal.SetCellValue(dt);
                    cellVal.CellStyle = dateStyle;
                }
                else if (decimal.TryParse(val, out num))
                {
                    cellVal.SetCellValue((double)num);
                    cellVal.CellStyle = currencyStyle;
                }`},

    {find: `if (value == null)
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
                    }`,
     replace: `decimal num;
                    if (value == null)
                    {
                        cell.SetCellValue("");
                    }
                    else if (value is DateTime)
                    {
                        DateTime dt = (DateTime)value;
                        cell.SetCellValue(dt);
                        cell.CellStyle = dateStyle;
                    }
                    else if (decimal.TryParse(value.ToString(), out num))
                    {
                        cell.SetCellValue((double)num);
                        cell.CellStyle = currencyStyle;
                    }`},

    {find: `if (string.IsNullOrEmpty(str)) return null;
                            if (DateTime.TryParse(str, out DateTime dt)) return dt;
                            if (double.TryParse(str, out double oaDate)) return DateTime.FromOADate(oaDate);`,
     replace: `if (string.IsNullOrEmpty(str)) return null;
                            DateTime dt;
                            if (DateTime.TryParse(str, out dt)) return dt;
                            double oaDate;
                            if (double.TryParse(str, out oaDate)) return DateTime.FromOADate(oaDate);`}
]);

// 4. Default - Copy.aspx.cs pattern matching
replaceAll("admin/quan-ly-bao-gia/Default - Copy.aspx.cs", [
    {find: `if (key.Contains("SĐT") || key.ToLower().Contains("sdt"))
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
                    }`,
     replace: `DateTime dt;
                    decimal num;
                    if (key.Contains("SĐT") || key.ToLower().Contains("sdt"))
                    {
                        cellVal.SetCellValue(val); // giữ nguyên chuỗi, không định dạng số
                    }
                    else if (DateTime.TryParseExact(val, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dt))
                    {
                        cellVal.SetCellValue(dt);
                        cellVal.CellStyle = dateStyle;
                    }
                    else if (decimal.TryParse(val, out num))
                    {
                        cellVal.SetCellValue((double)num);
                        cellVal.CellStyle = currencyStyle;
                    }`},

    {find: `if (value == null)
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
                        }`,
     replace: `decimal num;
                        if (value == null)
                        {
                            cell.SetCellValue("");
                        }
                        else if (value is DateTime)
                        {
                            DateTime dt = (DateTime)value;
                            cell.SetCellValue(dt);
                            cell.CellStyle = dateStyle;
                        }
                        else if (decimal.TryParse(value.ToString(), out num))
                        {
                            cell.SetCellValue((double)num);
                            cell.CellStyle = currencyStyle;
                        }`}
]);

// 5. Export.aspx.cs pattern matching
replaceAll("admin/quan-ly-bao-gia/Export.aspx.cs", [
    {find: `if (key.Contains("SĐT") || key.ToLower().Contains("sdt"))
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
                }`,
     replace: `DateTime dt;
                decimal num;
                if (key.Contains("SĐT") || key.ToLower().Contains("sdt"))
                {
                    cellVal.SetCellValue(val); // giữ nguyên chuỗi, không định dạng số
                }
                else if (DateTime.TryParseExact(val, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dt))
                {
                    cellVal.SetCellValue(dt);
                    cellVal.CellStyle = dateStyle;
                }
                else if (decimal.TryParse(val, out num))
                {
                    cellVal.SetCellValue((double)num);
                    cellVal.CellStyle = currencyStyle;
                }`},

    {find: `if (value == null)
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
                    }`,
     replace: `decimal num;
                    if (value == null)
                    {
                        cell.SetCellValue("");
                    }
                    else if (value is DateTime)
                    {
                        DateTime dt = (DateTime)value;
                        cell.SetCellValue(dt);
                        cell.CellStyle = dateStyle;
                    }
                    else if (decimal.TryParse(value.ToString(), out num))
                    {
                        cell.SetCellValue((double)num);
                        cell.CellStyle = currencyStyle;
                    }`}
]);

// 6. ExportExcel.cs
replaceAll("App_Code/ExportExcel.cs", [
    {find: `if (value is DateTime dt)
                    {`,
     replace: `if (value is DateTime)
                    {
                        DateTime dt = (DateTime)value;`}
]);
console.log("Done phase 1");
