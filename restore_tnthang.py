import re

with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\Default.aspx', 'r', encoding='utf-8') as f:
    html = f.read()

# Fix PlaceHolder2 (Headers)
old_ph2 = '''                                        <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible="false">
                                            <th class="text-center" style="width: 80px; min-width: 80px;">Lương CB</th>
                                            <th class="text-center" style="width: 200px; min-width: 200px;">Phụ cấp</th>
                                            <th class="text-center" style="width: 100px; min-width: 100px;">Bảo hiểm</th>
                                            <th class="text-center" style="width: 100px; min-width: 100px;">Ngân sách tối đa</th>
                                        </asp:PlaceHolder>'''
new_ph2 = '''                                        <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible="false">
                                            <th class="text-center" style="width: 80px; min-width: 80px;">Lương CB</th>
                                            <th class="text-center" style="width: 180px; min-width: 180px;">Phụ cấp</th>
                                            <th class="text-center" style="width: 100px; min-width: 100px;">Bảo hiểm</th>
                                            <th class="text-center" style="width: 120px; min-width: 120px;">TN tháng</th>
                                            <th class="text-center" style="width: 100px; min-width: 100px;">Ngân sách Max</th>
                                        </asp:PlaceHolder>'''
html = html.replace(old_ph2, new_ph2)

# Fix repeater (Rows)
old_repeater = '''                                                    <td class="text-right">
                                                        <%# Eval("LuongDongBH", "{0:#,##0}") %>
                                                    </td>
                                                    <td class="text-right">
                                                        <%# Eval("NganSach_Max", "{0:#,##0}") %>
                                                    </td>
                                                </asp:PlaceHolder>'''
new_repeater = '''                                                    <td class="text-right">
                                                        <%# Eval("LuongDongBH", "{0:#,##0}") %>
                                                    </td>
                                                    <td class="text-right">
                                                        <b><%# Eval("TongThuNhapThang", "{0:#,##0}") %></b>
                                                        <div><small>+ <%# Eval("phantram_doanhso_banhang") %>% DS bán hàng</small></div>
                                                    </td>
                                                    <td class="text-right">
                                                        <%# Eval("NganSach_Max", "{0:#,##0}") %>
                                                    </td>
                                                </asp:PlaceHolder>'''
html = html.replace(old_repeater, new_repeater)

# Fix PlaceHolder3 (Footer)
# Need to insert tongThuNhap between tongBaoHiem and tongNganSach. 
# Also increase colspan for TỔNG? Wait. Visible columns in THead: 7 (before PlaceHolder2) + 5 (PlaceHolder2) + 1 = 13.
# Footer: empty(1) + TỔNG(7) + LCB(1) + PhuCap(1) + BaoHiem(1) + ThuNhap(1) + NganSach(1) + empty(1) = 13.
# So TỔNG colspan is still 7!
old_ph3 = '''                                    <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible="false">
                                        <tr>
                                            <td class=" bg-white"></td>
                                            <td colspan="7" class="text-right text-bold bg-white">TỔNG</td>
                                            <td class="text-right text-bold"><%=ViewState["tongLCB"] %></td>
                                            <td class="text-right text-bold"><%=ViewState["tongPhuCap"] %></td>
                                            <td class="text-right text-bold"><%=ViewState["tongBaoHiem"] %></td>
                                            <td class="text-right text-bold"><%=ViewState["tongNganSach"] %></td>
                                            <td style="display:none;"></td>
                                            <td></td>
                                        </tr>
                                    </asp:PlaceHolder>'''
new_ph3 = '''                                    <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible="false">
                                        <tr>
                                            <td class=" bg-white"></td>
                                            <td colspan="7" class="text-right text-bold bg-white">TỔNG</td>
                                            <td class="text-right text-bold"><%=ViewState["tongLCB"] %></td>
                                            <td class="text-right text-bold"><%=ViewState["tongPhuCap"] %></td>
                                            <td class="text-right text-bold"><%=ViewState["tongBaoHiem"] %></td>
                                            <td class="text-right text-bold"><%=ViewState["tongThuNhap"] %></td>
                                            <td class="text-right text-bold"><%=ViewState["tongNganSach"] %></td>
                                            <td style="display:none;"></td>
                                            <td></td>
                                        </tr>
                                    </asp:PlaceHolder>'''
html = html.replace(old_ph3, new_ph3)

with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\Default.aspx', 'w', encoding='utf-8') as f:
    f.write(html)
