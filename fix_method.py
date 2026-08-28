import re

with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\Default.aspx.cs', 'r', encoding='utf-8') as f:
    cs = f.read()

# Add FormatPhucapItem before public void show_main()
method_code = '''
    protected string FormatPhucapItem(string title, object value)
    {
        if (value != null)
        {
            decimal val = 0;
            if (decimal.TryParse(value.ToString(), out val) && val > 0)
            {
                return "<div>- " + title + ": " + val.ToString("#,##0") + "</div>";
            }
        }
        return "";
    }
    
    public void show_main()'''

cs = cs.replace('public void show_main()', method_code)

with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\Default.aspx.cs', 'w', encoding='utf-8') as f:
    f.write(cs)
