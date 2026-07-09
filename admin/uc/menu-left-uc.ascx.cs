using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_uc_menu_left_uc : System.Web.UI.UserControl
{
    public string a0, a1, a1_1, a1_2, a1_3, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, muon_hang;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                string _url = HttpContext.Current.Request.Url.AbsolutePath.ToLower().Trim();
                switch (_url)
                {
                    case ("/admin/quan-ly-nhan-vien/default.aspx"): a2 = "active"; Session["title"] = "Quản lý nhân viên"; break;
                    case ("/admin/hang-bao-hanh/default.aspx"): a10 = "active"; Session["title"] = "Hàng bảo hành"; break;
                    case ("/admin/quan-ly-cong-viec/default.aspx"): a9 = "active"; Session["title"] = "Công việc"; break;
                    case ("/admin/data-khach-hang/default.aspx"): a8 = "active"; Session["title"] = "Data khách hàng"; break;
                    case ("/admin/quan-ly-bao-gia/default.aspx"): a7 = "active"; Session["title"] = "Quản lý báo giá"; break;
                    case ("/admin/theo-doi-hang-da-ban/default.aspx"): a11 = "active"; Session["title"] = "Theo dõi hàng đã bán"; break;
                    case ("/admin/quan-ly-he-thong/cai-dat.aspx"): a5 = "active"; Session["title"] = "Cài đặt hệ thống"; break;
                    case ("/admin/quan-ly-nhan-vien/bang-cham-cong.aspx"): a6 = "active"; Session["title"] = "Bảng chấm công"; break;
                    case ("/admin/quan-ly-kho/default.aspx"): a3 = "active"; Session["title"] = "Quản lý kho"; break;
                    case ("/admin/quan-ly-kho/lich-su-nhap-xuat.aspx"): a4 = "active"; Session["title"] = "Lịch sử nhập xuất"; break;
                    case ("/admin/quan-ly-kho/muon-hang.aspx"): muon_hang = "active"; Session["title"] = "Mượn hàng"; break;
                    case ("/admin/quan-ly-he-thong/du-lieu-nguon/hang-san-pham.aspx"): a1 = "active"; a1_1 = "active"; Session["title"] = "Hãng sản phẩm"; break;
                    case ("/admin/quan-ly-he-thong/du-lieu-nguon/nhom-san-pham.aspx"): a1 = "active"; a1_2 = "active"; Session["title"] = "Nhóm sản phẩm"; break;
                    case ("/admin/quan-ly-he-thong/du-lieu-nguon/don-vi-tinh.aspx"): a1 = "active"; a1_3 = "active"; Session["title"] = "Đơn vị tính"; break;
                    default: a0 = "active"; Session["title"] = "Trang chủ"; break;
                }

                //using (dbDataContext db = new dbDataContext())
                //{

                //}
            }
            catch (Exception _ex)
            {
                string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
                if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
                    _tk = mahoa_cl.giaima_Bcorn(_tk);
                else
                    _tk = "";
                Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
            }
        }
    }
}