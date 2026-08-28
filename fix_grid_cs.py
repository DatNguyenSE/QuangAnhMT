with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\admin\quan-ly-nhan-vien\Default.aspx.cs', 'r', encoding='utf-8') as f:
    cs = f.read()

# Fix FormatPhucapItem
old_format_func = '''    protected string FormatPhucapItem(string title, object value)
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
    }'''

new_format_func = '''    protected string FormatPhucapItem(string title, object value)
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
    }'''
# Actually the function is already fine. 

# We need to change the grid columns in Repeater1_ItemDataBound or show_main()
# Let's check what's in the C# file using grep first.
