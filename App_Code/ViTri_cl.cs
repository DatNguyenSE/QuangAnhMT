using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

public class ViTri_cl
{
    public static int TinhKhoanCach(string lat1Str, string lon1Str, string lat2Str, string lon2Str)
    {
        // Chuyển đổi từ chuỗi sang decimal với văn hóa không phụ thuộc
        decimal lat1 = decimal.Parse(lat1Str, CultureInfo.InvariantCulture);
        decimal lon1 = decimal.Parse(lon1Str, CultureInfo.InvariantCulture);
        decimal lat2 = decimal.Parse(lat2Str, CultureInfo.InvariantCulture);
        decimal lon2 = decimal.Parse(lon2Str, CultureInfo.InvariantCulture);

        // Chuyển đổi từ decimal sang double
        double lat1Double = (double)lat1;
        double lon1Double = (double)lon1;
        double lat2Double = (double)lat2;
        double lon2Double = (double)lon2;

        // Bán kính trái đất (mét)
        double R = 6371000;

        // Chuyển đổi từ độ sang radian
        double dLat = ToRadians(lat2Double - lat1Double);
        double dLon = ToRadians(lon2Double - lon1Double);

        // Chuyển đổi tọa độ từ độ sang radian
        lat1Double = ToRadians(lat1Double);
        lat2Double = ToRadians(lat2Double);

        // Công thức Haversine
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(lat1Double) * Math.Cos(lat2Double) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        // Tính khoảng cách
        double distanceInMeters = R * c;

        // Trả về kết quả làm tròn và ép kiểu về int
        return (int)Math.Round(distanceInMeters);
    }

    // Chuyển đổi từ độ sang radian
    private static double ToRadians(double angleInDegrees)
    {
        return angleInDegrees * Math.PI / 180.0;
    }
}