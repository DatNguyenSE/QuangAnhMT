import re

with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\Default.aspx', 'r', encoding='utf-8') as f:
    html = f.read()

# Update PlaceHolder2 (Headers)
old_ph2 = '''                                        <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible="false">
                                            <th class="text-center" style="width: 60px; min-width: 60px;">Lương CB</th>
                                            <th class="text-center" style="width: 150px; min-width: 150px;">Phụ cấp</th>
                                            <th class="text-center" style="width: 120px; min-width: 120px;">TN tháng</th>
                                        </asp:PlaceHolder>'''
new_ph2 = '''                                        <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible="false">
                                            <th class="text-center" style="width: 80px; min-width: 80px;">Lương CB</th>
                                            <th class="text-center" style="width: 200px; min-width: 200px;">Phụ cấp</th>
                                            <th class="text-center" style="width: 100px; min-width: 100px;">Bảo hiểm</th>
                                            <th class="text-center" style="width: 100px; min-width: 100px;">Ngân sách tối đa</th>
                                        </asp:PlaceHolder>'''
html = html.replace(old_ph2, new_ph2)

# Update PlaceHolder3 (Footer)
# In PlaceHolder3, the last columns were: TỔNG, tongLCB, tongPhuCap, tongThuNhap, empty, empty
old_ph3 = '''                                    <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible="false">
                                        <tr>
                                            <td class=" bg-white"></td>
                                            <td colspan="7" class="text-right text-bold bg-white">TỔNG</td>
                                            <td class="text-right text-bold"><%=ViewState["tongLCB"] %></td>
                                            <td class="text-right text-bold"><%=ViewState["tongPhuCap"] %></td>
                                            <td class="text-right text-bold"><%=ViewState["tongThuNhap"] %></td>
                                            <td style="display:none;"></td>
                                            <td></td>
                                        </tr>
                                    </asp:PlaceHolder>'''
# Note: I changed colspan="6" to colspan="7" earlier today. 
# tongThuNhap is gone. We have tongBaoHiem and tongNganSach.
new_ph3 = '''                                    <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible="false">
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

# I will use Regex because formatting might mismatch
html = re.sub(r'<asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible="false">\s*<tr>\s*<td class=" bg-white"></td>\s*<td colspan="\d+" class="text-right text-bold bg-white">TỔNG</td>\s*<td class="text-right text-bold"><%=ViewState\["tongLCB"\] %></td>\s*<td class="text-right text-bold"><%=ViewState\["tongPhuCap"\] %></td>\s*<td class="text-right text-bold"><%=ViewState\["tongThuNhap"\] %></td>\s*<td style="display:none;"></td>\s*<td></td>\s*</tr>\s*</asp:PlaceHolder>', new_ph3, html)

with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\Default.aspx', 'w', encoding='utf-8') as f:
    f.write(html)
