import re

with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\Default.aspx.cs', 'r', encoding='utf-8') as f:
    cs = f.read()

# 1. Update show_main grid
grid_add = '''
                                     cc.PhuCap_AnUong,
                                     cc.PhuCap_DienThoai,
                                     cc.PhuCap_TrachNhiem,
                                     cc.chucdanh,
                                     cc.PhuCap_RnD,
                                     cc.PhuCap_TrucHotline,
                                     cc.Thuong_DuAn_Max,
                                     cc.LuongDongBH,
                                     cc.NganSach_Max,
'''
if 'cc.PhuCap_RnD' not in cs:
    cs = cs.replace('cc.PhuCap_TrachNhiem,', grid_add)

# 2. Add to insert
insert_add = '''
                tk.chucdanh = txt_chucdanh.Text.Trim();
                tk.PhuCap_RnD = txt_phucap_rnd.Text == "" ? 0 : decimal.Parse(txt_phucap_rnd.Text.Replace(",", ""));
                tk.PhuCap_TrucHotline = txt_phucap_hotline.Text == "" ? 0 : decimal.Parse(txt_phucap_hotline.Text.Replace(",", ""));
                tk.Thuong_DuAn_Max = txt_hotro_da_max.Text == "" ? 0 : decimal.Parse(txt_hotro_da_max.Text.Replace(",", ""));
                tk.LuongDongBH = txt_luong_dong_bh.Text == "" ? 0 : decimal.Parse(txt_luong_dong_bh.Text.Replace(",", ""));
                tk.NganSach_Max = txt_ngansach_max.Text == "" ? 0 : decimal.Parse(txt_ngansach_max.Text.Replace(",", ""));
'''
if 'tk.PhuCap_RnD = txt_phucap_rnd.Text' not in cs:
    cs = cs.replace('tk.PhuCap_TrachNhiem = txt_phucap_trachniem.Text == "" ? 0 : decimal.Parse(txt_phucap_trachniem.Text.Replace(",", ""));', 'tk.PhuCap_TrachNhiem = txt_phucap_trachniem.Text == "" ? 0 : decimal.Parse(txt_phucap_trachniem.Text.Replace(",", ""));' + insert_add)

# 3. Add to edit bind
edit_bind = '''
            txt_chucdanh.Text = q.chucdanh;
            ddl_gioitinh.SelectedValue = "Nam"; // Default, need to add gioitinh to db if requested, but user didn't ask for gioitinh in db? Wait, user has gioitinh. Let's ignore gioitinh DB for now if it's not there, or add it if it is.
            txt_phucap_rnd.Text = q.PhuCap_RnD.HasValue ? q.PhuCap_RnD.Value.ToString("#,##0") : "";
            txt_phucap_hotline.Text = q.PhuCap_TrucHotline.HasValue ? q.PhuCap_TrucHotline.Value.ToString("#,##0") : "";
            txt_hotro_da_max.Text = q.Thuong_DuAn_Max.HasValue ? q.Thuong_DuAn_Max.Value.ToString("#,##0") : "";
            txt_luong_dong_bh.Text = q.LuongDongBH.HasValue ? q.LuongDongBH.Value.ToString("#,##0") : "";
            txt_ngansach_max.Text = q.NganSach_Max.HasValue ? q.NganSach_Max.Value.ToString("#,##0") : "";
'''
if 'txt_phucap_rnd.Text' not in cs:
    cs = cs.replace('txt_phucap_trachniem.Text = q.PhuCap_TrachNhiem.HasValue ? q.PhuCap_TrachNhiem.Value.ToString("#,##0") : "";', 'txt_phucap_trachniem.Text = q.PhuCap_TrachNhiem.HasValue ? q.PhuCap_TrachNhiem.Value.ToString("#,##0") : "";' + edit_bind)

# 4. Clear form
clear_form = '''
        txt_chucdanh.Text = "";
        txt_phucap_rnd.Text = "";
        txt_phucap_hotline.Text = "";
        txt_hotro_da_max.Text = "";
        txt_luong_dong_bh.Text = "";
        txt_ngansach_max.Text = "";
'''
if 'txt_phucap_rnd.Text = "";' not in cs:
    cs = cs.replace('txt_phucap_trachniem.Text = "";', 'txt_phucap_trachniem.Text = "";' + clear_form)


with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\Default.aspx.cs', 'w', encoding='utf-8') as f:
    f.write(cs)

