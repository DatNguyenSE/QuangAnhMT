$files = Get-ChildItem -Path . -Recurse -Filter *.cs
foreach ($f in $files) {
    $content = Get-Content $f.FullName -Raw
    $original = $content
    
    $replacements = @(
        @{ find = 'new ListItem($"Trang {i}", i.ToString())'; replace = 'new ListItem(string.Format("Trang {0}", i), i.ToString())' },
        @{ find = 'string script = $"window.open(''{url}'', ''_blank'');";'; replace = 'string script = string.Format("window.open(''{0}'', ''_blank'');", url);' },
        @{ find = 'ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), $"alert(''Lỗi xuất Excel: {errMsg}'');", true);'; replace = 'ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), string.Format("alert(''Lỗi xuất Excel: {0}'');", errMsg), true);' },
        @{ find = 'sanPhamThieu.Add($"Sản phẩm {sanPhamKho.ten} không đủ số lượng. Tồn: {sanPhamKho.soluong_hientai} Xuất: {chiTiet.soluong}");'; replace = 'sanPhamThieu.Add(string.Format("Sản phẩm {0} không đủ số lượng. Tồn: {1} Xuất: {2}", sanPhamKho.ten, sanPhamKho.soluong_hientai, chiTiet.soluong));' },
        @{ find = 'return $"<span class=''fg-green''>↑ 100% (+{current.ToString("N0")} {unit})</span>";'; replace = 'return string.Format("<span class=''fg-green''>↑ 100% (+{0} {1})</span>", current.ToString("N0"), unit);' },
        @{ find = 'return $"<span class=''fg-green''>↑ {pct:0.#}% (+{diff.ToString("N0")} {unit})</span>";'; replace = 'return string.Format("<span class=''fg-green''>↑ {0:0.#}% (+{1} {2})</span>", pct, diff.ToString("N0"), unit);' },
        @{ find = 'return $"<span class=''fg-red''>↓ {Math.Abs(pct):0.#}% ({diff.ToString("N0")} {unit})</span>";'; replace = 'return string.Format("<span class=''fg-red''>↓ {0:0.#}% ({1} {2})</span>", Math.Abs(pct), diff.ToString("N0"), unit);' }
    )

    foreach ($r in $replacements) {
        $content = $content.Replace($r.find, $r.replace)
    }
    
    if ($content -ne $original) {
        Set-Content $f.FullName -Value $content -NoNewline
        Write-Host "Updated $($f.FullName)"
    }
}
