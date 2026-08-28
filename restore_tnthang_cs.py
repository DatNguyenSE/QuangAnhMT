with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\Default.aspx.cs', 'r', encoding='utf-8') as f:
    cs = f.read()

old_vs = '''                  ViewState["tongBaoHiem"] = (list_all.Sum(p => (long?)p.LuongDongBH) ?? 0).ToString("#,##0");
                  ViewState["tongNganSach"] = (list_all.Sum(p => (long?)p.NganSach_Max) ?? 0).ToString("#,##0");'''
new_vs = '''                  ViewState["tongBaoHiem"] = (list_all.Sum(p => (long?)p.LuongDongBH) ?? 0).ToString("#,##0");
                  ViewState["tongThuNhap"] = (list_all.Sum(p => (decimal?)p.TongThuNhapThang) ?? 0).ToString("#,##0");
                  ViewState["tongNganSach"] = (list_all.Sum(p => (long?)p.NganSach_Max) ?? 0).ToString("#,##0");'''

cs = cs.replace(old_vs, new_vs)

with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\Default.aspx.cs', 'w', encoding='utf-8') as f:
    f.write(cs)
