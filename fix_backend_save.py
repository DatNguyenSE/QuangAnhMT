import re

with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\Default.aspx.cs', 'r', encoding='utf-8') as f:
    cs = f.read()

# 1. Fetch values
old_fetch = '''            Int64 _luongcoban = Number_cl.Check_Int64(txt_luongcoban.Text.Trim());'''
new_fetch = '''            Int64 _luongcoban = Number_cl.Check_Int64(txt_luongcoban.Text.Trim());
            Int64 _phucap_rnd = Number_cl.Check_Int64(txt_phucap_rnd.Text.Trim());
            Int64 _phucap_hotline = Number_cl.Check_Int64(txt_phucap_hotline.Text.Trim());
            Int64 _hotro_da_max = Number_cl.Check_Int64(txt_hotro_da_max.Text.Trim());
            Int64 _luong_dong_bh = Number_cl.Check_Int64(txt_luong_dong_bh.Text.Trim());
            Int64 _ngansach_max = Number_cl.Check_Int64(txt_ngansach_max.Text.Trim());
            string _chucdanh = txt_chucdanh.Text.Trim();'''
cs = cs.replace(old_fetch, new_fetch)

# 2. Save into _ob (Insert and Update)
old_save = '''                    _ob.LuongCoBan = _luongcoban;'''
new_save = '''                    _ob.LuongCoBan = _luongcoban;
                    _ob.PhuCap_RnD = _phucap_rnd;
                    _ob.PhuCap_TrucHotline = _phucap_hotline;
                    _ob.Thuong_DuAn_Max = _hotro_da_max;
                    _ob.LuongDongBH = _luong_dong_bh;
                    _ob.NganSach_Max = _ngansach_max;
                    _ob.chucdanh = _chucdanh;'''
cs = cs.replace(old_save, new_save) # will replace both occurrences

# 3. Bind values in Edit mode
old_bind = '''                    if (q.LuongCoBan != null)
                        txt_luongcoban.Text = q.LuongCoBan.Value.ToString("#,##0");'''
new_bind = '''                    if (q.LuongCoBan != null)
                        txt_luongcoban.Text = q.LuongCoBan.Value.ToString("#,##0");
                    if (q.PhuCap_RnD != null)
                        txt_phucap_rnd.Text = q.PhuCap_RnD.Value.ToString("#,##0");
                    if (q.PhuCap_TrucHotline != null)
                        txt_phucap_hotline.Text = q.PhuCap_TrucHotline.Value.ToString("#,##0");
                    if (q.Thuong_DuAn_Max != null)
                        txt_hotro_da_max.Text = q.Thuong_DuAn_Max.Value.ToString("#,##0");
                    if (q.LuongDongBH != null)
                        txt_luong_dong_bh.Text = q.LuongDongBH.Value.ToString("#,##0");
                    if (q.NganSach_Max != null)
                        txt_ngansach_max.Text = q.NganSach_Max.Value.ToString("#,##0");
                    txt_chucdanh.Text = q.chucdanh;'''
cs = cs.replace(old_bind, new_bind)

with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\Default.aspx.cs', 'w', encoding='utf-8') as f:
    f.write(cs)
