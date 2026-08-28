import re

with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\Default.aspx', 'r', encoding='utf-8') as f:
    html = f.read()

new_repeater_cells = '''                                                <asp:PlaceHolder ID="lblLuongCoBan" runat="server" Visible="false">
                                                    <td class="text-right">
                                                        <%# Eval("LuongCoBan", "{0:#,##0}") %>
                                                    </td>
                                                    <td class="text-normal">
                                                        <%# FormatPhucapItem("Trách nhiệm", Eval("PhuCap_TrachNhiem")) %>
                                                        <%# FormatPhucapItem("R&D", Eval("PhuCap_RnD")) %>
                                                        <%# FormatPhucapItem("Trực Hotline", Eval("PhuCap_TrucHotline")) %>
                                                        <%# FormatPhucapItem("Hỗ trợ DA", Eval("Thuong_DuAn_Max")) %>
                                                        <%# FormatPhucapItem("Xăng xe", Eval("PhuCap_Xangxe")) %>
                                                        <%# FormatPhucapItem("Ăn trưa", Eval("PhuCap_AnUong")) %>
                                                        <%# FormatPhucapItem("Điện thoại", Eval("PhuCap_DienThoai")) %>
                                                    </td>
                                                    <td class="text-right">
                                                        <%# Eval("LuongDongBH", "{0:#,##0}") %>
                                                    </td>
                                                    <td class="text-right">
                                                        <%# Eval("NganSach_Max", "{0:#,##0}") %>
                                                    </td>
                                                </asp:PlaceHolder>'''

# I will find lblLuongCoBan and replace until its closing tag
start_idx = html.find('<asp:PlaceHolder ID="lblLuongCoBan"')
end_idx = html.find('</asp:PlaceHolder>', start_idx) + 18

if start_idx != -1 and end_idx != -1:
    html = html[:start_idx] + new_repeater_cells + html[end_idx:]
    with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\Default.aspx', 'w', encoding='utf-8') as f:
        f.write(html)
    print("Replaced lblLuongCoBan")
