using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Number_cl
/// </summary>
public class Number_cl
{
    public static int Check_Int(string _input)
    {
        if (string.IsNullOrEmpty(_input)) return 0;
        int _kq = 0;
        int.TryParse(_input.Replace(".","").Replace(",", "").Replace(" ", ""), out _kq);
        return _kq;
    }
    public static Int64 Check_Int64(string _input)
    {
        if (string.IsNullOrEmpty(_input)) return 0;
        Int64 _kq = 0;
        Int64.TryParse(_input.Replace(".", "").Replace(",", "").Replace(" ", ""), out _kq);
        return _kq;
    }
    public static decimal Check_Decimal(string _input)
    {
        decimal _kq = 0;
        decimal.TryParse(_input.Replace(".", ","), out _kq);
        return _kq;
    }

}