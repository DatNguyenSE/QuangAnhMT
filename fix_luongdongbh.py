import re

with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\bang-cham-cong.aspx.cs', 'r', encoding='utf-8') as f:
    cs = f.read()

target = 'htmlTable.Append("<td class=\'text-right \'>" + luongDongBH.ToString("#,##0") + "</td>");'
cs = cs.replace(target, '')

with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\bang-cham-cong.aspx.cs', 'w', encoding='utf-8') as f:
    f.write(cs)
