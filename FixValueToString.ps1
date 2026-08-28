$files = Get-ChildItem -Path . -Recurse -Filter *.cs
foreach ($f in $files) {
    $content = Get-Content $f.FullName -Raw
    $original = $content
    
    $content = $content.Replace('q.ngaybaogia.ToString("dd/MM/yyyy")', 'q.ngaybaogia.Value.ToString("dd/MM/yyyy")')
    $content = $content.Replace('q.ngayhethan.ToString("dd/MM/yyyy")', 'q.ngayhethan.Value.ToString("dd/MM/yyyy")')
    $content = $content.Replace('ct.soluong.ToString("#,##0")', 'ct.soluong.Value.ToString("#,##0")')
    $content = $content.Replace('bg.thuongdoanhso.ToString("#,##0")', 'bg.thuongdoanhso.Value.ToString("#,##0")')
    
    # Check if there are other .ToString( arguments in nullable types
    if ($content -ne $original) {
        Set-Content $f.FullName -Value $content -NoNewline
        Write-Host "Updated $($f.FullName)"
    }
}
