with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\Default.aspx', 'r', encoding='utf-8') as f:
    html = f.read()

cccd_block = '''                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Số CCCD</label>
                                        <div>
                                            <asp:TextBox ID="txt_so_cccd" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>'''

gioitinh_block = '''                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Giới tính</label>
                                        <div>
                                            <asp:DropDownList ID="ddl_gioitinh" runat="server" data-role="select">
                                                <asp:ListItem Value="Nam">Nam</asp:ListItem>
                                                <asp:ListItem Value="Nữ">Nữ</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>'''

chucdanh_block = '''                                    <div class="mt-3">
                                        <label class="fw-600">Chức danh</label>
                                        <div>
                                            <asp:TextBox ID="txt_chucdanh" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>'''

hoten_block = '''                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Họ và tên</label>
                                        <div>
                                            <asp:TextBox ID="txt_hoten" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>'''

# I will replace the sequence: cccd -> hoten -> gioitinh -> chucdanh
# with: hoten -> gioitinh -> cccd -> chucdanh

sequence_to_replace = cccd_block + '\n\n' + hoten_block + '\n                                    \n' + gioitinh_block + '\n\n' + chucdanh_block
new_sequence = hoten_block + '\n\n' + gioitinh_block + '\n\n' + cccd_block + '\n\n' + chucdanh_block

if sequence_to_replace in html:
    html = html.replace(sequence_to_replace, new_sequence)
    with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\Default.aspx', 'w', encoding='utf-8') as f:
        f.write(html)
    print("Replaced!")
else:
    print("Not found! Let's try with re")
    import re
    # Match any whitespace between the blocks
    regex = re.escape(cccd_block) + r'\s*' + re.escape(hoten_block) + r'\s*' + re.escape(gioitinh_block) + r'\s*' + re.escape(chucdanh_block)
    html = re.sub(regex, new_sequence, html)
    with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\Default.aspx', 'w', encoding='utf-8') as f:
        f.write(html)
    print("Replaced with regex!")

