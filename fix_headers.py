import re

with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\bang-cham-cong.aspx.cs', 'r', encoding='utf-8') as f:
    cs = f.read()

headers = '''        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Trách<br/>nhiệm</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>R&D</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Trực<br/>hotline</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Hỗ trợ<br/>D.A</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Doanh<br/>số</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Thưởng<br/>D.số</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Tổng GROSS<br/>dự kiến</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>NLĐ đóng BH<br/>(10,5%)</td>");
        htmlTable.Append("<td class='text-center bg-red fg-white' style='width:1px;min-width:1px'>Thực nhận<br/>trước PIT</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>DN đóng BH<br/>(21,5%)</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Kinh phí CĐ<br/>(2%)</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Tổng chi phí<br/>DN</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Ngân sách<br/>tối đa</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Chênh lệch</td>");'''

# Replace old headers block
old_headers_regex = r'htmlTable\.Append\("<td class=\'text-center bg-cobalt fg-white\' style=\'width:1px;min-width:1px\'>Trách<br/>nhiệm</td>"\);.*?htmlTable\.Append\("<td class=\'text-center bg-red fg-white\' style=\'width:1px;min-width:1px\'>Thực<br/>nhận</td>"\);'
cs = re.sub(old_headers_regex, headers, cs, flags=re.DOTALL)

with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\bang-cham-cong.aspx.cs', 'w', encoding='utf-8') as f:
    f.write(cs)
