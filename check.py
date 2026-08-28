import sys

with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\App_Code\db.designer.cs', 'r', encoding='utf-8') as f:
    cs = f.read()

if 'PhuCap_RnD' not in cs:
    print('Missing PhuCap_RnD in designer!')
else:
    print('PhuCap_RnD exists in designer!')
