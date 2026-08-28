with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\bang-cham-cong.aspx.cs', 'r', encoding='utf-8') as f:
    cs = f.read()

# Headers
old_header = '''        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Trách<br/>nhiệm</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Doanh<br/>số</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Thưởng<br/>D.số</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Tổng GROSS<br/>dự kiến</td>");
        htmlTable.Append("<td class='text-center bg-red fg-white' style='width:1px;min-width:1px'>Thực nhận<br/>trước PIT</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>DN đóng BH<br/>(21,5%)</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Kinh phí CĐ<br/>(2%)</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Tổng chi phí<br/>DN</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Ngân sách<br/>tối đa</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Chênh lệch</td>");
        htmlTable.Append("<td class='text-center bg-red fg-white' style='width:1px;min-width:1px'>Thực<br/>nhận</td>");'''

new_header = '''        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Trách<br/>nhiệm</td>");
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

# Because Python multiline string matches might have wrong \r, let's normalize \r\n to \n in both
cs = cs.replace('\r\n', '\n')
old_header = old_header.replace('\r\n', '\n')
new_header = new_header.replace('\r\n', '\n')

cs = cs.replace(old_header, new_header)

with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\bang-cham-cong.aspx.cs', 'w', encoding='utf-8') as f:
    f.write(cs)
