import re

with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\Default.aspx.cs', 'r', encoding='utf-8') as f:
    cs = f.read()

# I want to add LuongDongBH, NganSach_Max, PhuCap_RnD, PhuCap_TrucHotline, Thuong_DuAn_Max to the query
# Wait, I already added them today using ix2.py? Let's check if they are in the query!
# I will just write a regex to replace the ViewState calculation

# Replace ViewState calculation
old_viewstate = '''                  ViewState["tongLCB"] = (list_all.Sum(p => (long?)p.LuongCoBan) ?? 0).ToString("#,##0");
                  decimal _tong_phucap = list_all.Sum(p => (decimal?)((p.PhuCap_Xangxe ?? 0) + (p.PhuCap_AnUong ?? 0) + (p.PhuCap_DienThoai ?? 0) + (p.PhuCap_TrachNhiem ?? 0))) ?? 0;
                  ViewState["tongPhuCap"] = _tong_phucap.ToString("#,##0");
                  ViewState["tongThuNhap"] = (list_all.Sum(p => (decimal?)p.TongThuNhapThang) ?? 0).ToString("#,##0");'''

new_viewstate = '''                  ViewState["tongLCB"] = (list_all.Sum(p => (long?)p.LuongCoBan) ?? 0).ToString("#,##0");
                  decimal _tong_phucap = list_all.Sum(p => (decimal?)((p.PhuCap_Xangxe ?? 0) + (p.PhuCap_AnUong ?? 0) + (p.PhuCap_DienThoai ?? 0) + (p.PhuCap_TrachNhiem ?? 0) + (p.PhuCap_RnD ?? 0) + (p.PhuCap_TrucHotline ?? 0) + (p.Thuong_DuAn_Max ?? 0))) ?? 0;
                  ViewState["tongPhuCap"] = _tong_phucap.ToString("#,##0");
                  ViewState["tongBaoHiem"] = (list_all.Sum(p => (long?)p.LuongDongBH) ?? 0).ToString("#,##0");
                  ViewState["tongNganSach"] = (list_all.Sum(p => (long?)p.NganSach_Max) ?? 0).ToString("#,##0");'''

cs = cs.replace(old_viewstate, new_viewstate)
# try with regex if literal fails
cs = re.sub(r'ViewState\["tongLCB"\] = .*?ViewState\["tongThuNhap"\].*?;', new_viewstate, cs, flags=re.DOTALL)

with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\Default.aspx.cs', 'w', encoding='utf-8') as f:
    f.write(cs)
