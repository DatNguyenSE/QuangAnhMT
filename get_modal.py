with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\Default.aspx', 'r', encoding='utf-8') as f:
    html = f.read()

start_idx = html.find('<asp:Panel ID="pn_add"')
end_idx = html.find('</asp:Panel>', start_idx) + 12

print(html[start_idx:end_idx])
