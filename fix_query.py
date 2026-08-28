with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\Default.aspx.cs', 'r', encoding='utf-8') as f:
    cs = f.read()

target = 'ob1.PhuCap_Xangxe,'
replacement = '''ob1.PhuCap_Xangxe,
                                      ob1.PhuCap_RnD,
                                      ob1.PhuCap_TrucHotline,
                                      ob1.Thuong_DuAn_Max,
                                      ob1.LuongDongBH,
                                      ob1.NganSach_Max,
                                      ob1.chucdanh,'''

cs = cs.replace(target, replacement)

with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\Default.aspx.cs', 'w', encoding='utf-8') as f:
    f.write(cs)
