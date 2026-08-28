$files = Get-ChildItem -Path . -Recurse -Filter *.cs
foreach ($f in $files) {
    $content = Get-Content $f.FullName -Raw
    $original = $content
    
    $replacements = @(
        @{ find = 'Session["url_back"]?.ToString()'; replace = '(Session["url_back"] != null ? Session["url_back"].ToString() : null)' },
        @{ find = 'ViewState["taikhoan"]?.ToString()'; replace = '(ViewState["taikhoan"] != null ? ViewState["taikhoan"].ToString() : null)' },
        @{ find = 'q.ngaybaogia?.ToString("dd/MM/yyyy")'; replace = '(q.ngaybaogia != null ? q.ngaybaogia.ToString("dd/MM/yyyy") : null)' },
        @{ find = 'q.ngayhethan?.ToString("dd/MM/yyyy")'; replace = '(q.ngayhethan != null ? q.ngayhethan.ToString("dd/MM/yyyy") : null)' },
        @{ find = 'ViewState["TongSauGiam_ChiTiet"]?.ToString()'; replace = '(ViewState["TongSauGiam_ChiTiet"] != null ? ViewState["TongSauGiam_ChiTiet"].ToString() : null)' },
        @{ find = 'ViewState["thanhtien_vat_chitiet"]?.ToString()'; replace = '(ViewState["thanhtien_vat_chitiet"] != null ? ViewState["thanhtien_vat_chitiet"].ToString() : null)' },
        @{ find = 'ViewState["donhang_saugiamgia"]?.ToString()'; replace = '(ViewState["donhang_saugiamgia"] != null ? ViewState["donhang_saugiamgia"].ToString() : null)' },
        @{ find = 'ViewState["add_edit"]?.ToString()'; replace = '(ViewState["add_edit"] != null ? ViewState["add_edit"].ToString() : null)' },
        @{ find = 'ViewState["id_edit"]?.ToString()'; replace = '(ViewState["id_edit"] != null ? ViewState["id_edit"].ToString() : null)' },
        @{ find = 'worksheet.Cells[row, 7].Text?.Trim()'; replace = '(worksheet.Cells[row, 7].Text != null ? worksheet.Cells[row, 7].Text.Trim() : null)' },
        @{ find = 'worksheet.Cells[row, 8].Text?.Trim()'; replace = '(worksheet.Cells[row, 8].Text != null ? worksheet.Cells[row, 8].Text.Trim() : null)' },
        @{ find = 'stats?.Count'; replace = '(stats != null ? (int?)stats.Count : null)' },
        @{ find = 'stats?.TongBanLe'; replace = '(stats != null ? (long?)stats.TongBanLe : null)' },
        @{ find = 'stats?.TongGiaNhap'; replace = '(stats != null ? (long?)stats.TongGiaNhap : null)' },
        @{ find = 'stats?.TongTon'; replace = '(stats != null ? (long?)stats.TongTon : null)' },
        @{ find = 'detailPhieu?.SoLuongTra'; replace = '(detailPhieu != null ? (int?)detailPhieu.SoLuongTra : null)' },
        @{ find = 'ViewState["total_page"]?.ToString()'; replace = '(ViewState["total_page"] != null ? ViewState["total_page"].ToString() : null)' },
        @{ find = 'ct.soluong?.ToString("#,##0")'; replace = '(ct.soluong != null ? ct.soluong.ToString("#,##0") : null)' },
        @{ find = 'bg.phantram_doanhso_now?.ToString()'; replace = '(bg.phantram_doanhso_now != null ? bg.phantram_doanhso_now.ToString() : null)' },
        @{ find = 'bg.thuongdoanhso?.ToString("#,##0")'; replace = '(bg.thuongdoanhso != null ? bg.thuongdoanhso.ToString("#,##0") : null)' },
        @{ find = 'Session["taikhoan"]?.ToString()'; replace = '(Session["taikhoan"] != null ? Session["taikhoan"].ToString() : null)' },
        @{ find = 'rawAmountObj?.ToString()?.Trim()'; replace = '(rawAmountObj != null && rawAmountObj.ToString() != null ? rawAmountObj.ToString().Trim() : null)' },
        @{ find = 'Path.GetExtension(fileName)?.ToLower()'; replace = '(Path.GetExtension(fileName) != null ? Path.GetExtension(fileName).ToLower() : null)' }
    )

    foreach ($r in $replacements) {
        $content = $content.Replace($r.find, $r.replace)
    }
    
    if ($content -ne $original) {
        Set-Content $f.FullName -Value $content -NoNewline
        Write-Host "Updated $($f.FullName)"
    }
}
