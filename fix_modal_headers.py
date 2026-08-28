with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\Default.aspx', 'r', encoding='utf-8') as f:
    html = f.read()

# Add header 1
target1 = '<div class="cell-lg-6 pr-4-lg">'
html = html.replace(target1, target1 + '\n                                    <h4 class="fw-600 fg-red mt-0 mb-4">THÔNG TIN TÀI KHOẢN</h4>')

# Add header 2
target2 = '<asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible="false">'
html = html.replace(target2, target2 + '\n                                        <h4 class="fw-600 fg-red mt-0 mb-4">THÔNG TIN LƯƠNG & PHỤ CẤP</h4>')

with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\Default.aspx', 'w', encoding='utf-8') as f:
    f.write(html)
