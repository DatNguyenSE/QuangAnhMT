import re

with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\bang-cham-cong.aspx.cs', 'r', encoding='utf-8') as f:
    cs = f.read()

# Fix items around line 625
items = '''            htmlTable.Append("<td class='text-right text-bold'>" + _tongcong.ToString("#,##0") + "</td>");
            htmlTable.Append("<td class='text-right '>" + nldDongBH.ToString("#,##0") + "</td>");
            htmlTable.Append("<td class='text-right text-bold fg-red'>" + _thucnhan.ToString("#,##0") + "</td>");
            htmlTable.Append("<td class='text-right '>" + dnDongBH.ToString("#,##0") + "</td>");
            htmlTable.Append("<td class='text-right '>" + kpCD.ToString("#,##0") + "</td>");
            htmlTable.Append("<td class='text-right text-bold'>" + tongChiPhiDN.ToString("#,##0") + "</td>");
            htmlTable.Append("<td class='text-right '>" + nganSachMax.ToString("#,##0") + "</td>");
            htmlTable.Append("<td class='text-right text-bold " + (chenhLech < 0 ? "fg-red" : "fg-green") + "'>" + chenhLech.ToString("#,##0") + "</td>");'''

old_items_regex = r'htmlTable\.Append\("<td class=\'text-right text-bold\'>" \+ _tongcong\.ToString\("#,##0"\) \+ "</td>"\);\s*htmlTable\.Append\("<td class=\'text-right text-bold fg-red\'>" \+ _thucnhan\.ToString\("#,##0"\) \+ "</td>"\);\s*htmlTable\.Append\("<td class=\'text-right \'>" \+ dnDongBH\.ToString\("#,##0"\) \+ "</td>"\);\s*htmlTable\.Append\("<td class=\'text-right \'>" \+ kpCD\.ToString\("#,##0"\) \+ "</td>"\);\s*htmlTable\.Append\("<td class=\'text-right text-bold\'>" \+ tongChiPhiDN\.ToString\("#,##0"\) \+ "</td>"\);\s*htmlTable\.Append\("<td class=\'text-right \'>" \+ nganSachMax\.ToString\("#,##0"\) \+ "</td>"\);\s*htmlTable\.Append\("<td class=\'text-right text-bold " \+ \(chenhLech < 0 \? "fg-red" : "fg-green"\) \+ "\'>" \+ chenhLech\.ToString\("#,##0"\) \+ "</td>"\);'

cs = re.sub(old_items_regex, items, cs)

# Fix footers
footers = '''        htmlTable.Append("<td class='text-right text-bold'>" + TongKet_HoTroDA.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold'>" + TongKet_DoanhSo.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold'>" + TongKet_ThuongDoanhSo.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold'>" + TongKet_TongCong.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold'>" + TongKet_BaoHiem.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold fg-red'>" + TongKet_ThucNhan.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold'>" + TongKet_DNBH.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold'>" + TongKet_KinhPhiCD.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold'>" + TongKet_ChiPhiDN.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold'>" + TongKet_NganSach.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold'>" + TongKet_ChenhLech.ToString("#,##0") + "</td>");'''

old_footers_regex = r'htmlTable\.Append\("<td class=\'text-right text-bold\'>" \+ TongKet_HoTroDA\.ToString\("#,##0"\) \+ "</td>"\);\s*htmlTable\.Append\("<td class=\'text-right text-bold\'>" \+ TongKet_BaoHiem\.ToString\("#,##0"\) \+ "</td>"\);\s*htmlTable\.Append\("<td class=\'text-right text-bold\'>" \+ TongKet_DoanhSo\.ToString\("#,##0"\) \+ "</td>"\);\s*htmlTable\.Append\("<td class=\'text-right text-bold\'>" \+ TongKet_ThuongDoanhSo\.ToString\("#,##0"\) \+ "</td>"\);\s*htmlTable\.Append\("<td class=\'text-right text-bold\'>" \+ TongKet_TongCong\.ToString\("#,##0"\) \+ "</td>"\);\s*htmlTable\.Append\("<td class=\'text-right text-bold fg-red\'>" \+ TongKet_ThucNhan\.ToString\("#,##0"\) \+ "</td>"\);\s*htmlTable\.Append\("<td class=\'text-right text-bold\'>" \+ TongKet_DNBH\.ToString\("#,##0"\) \+ "</td>"\);\s*htmlTable\.Append\("<td class=\'text-right text-bold\'>" \+ TongKet_KinhPhiCD\.ToString\("#,##0"\) \+ "</td>"\);\s*htmlTable\.Append\("<td class=\'text-right text-bold\'>" \+ TongKet_ChiPhiDN\.ToString\("#,##0"\) \+ "</td>"\);\s*htmlTable\.Append\("<td class=\'text-right text-bold\'>" \+ TongKet_NganSach\.ToString\("#,##0"\) \+ "</td>"\);\s*htmlTable\.Append\("<td class=\'text-right text-bold\'>" \+ TongKet_ChenhLech\.ToString\("#,##0"\) \+ "</td>"\);'

cs = re.sub(old_footers_regex, footers, cs)

with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\bang-cham-cong.aspx.cs', 'w', encoding='utf-8') as f:
    f.write(cs)
