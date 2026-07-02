using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_quan_ly_kho_qr_sanpham : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Yêu cầu đăng nhập. Quyền 'qr' (hoặc quyền chung)
            check_login_cl.check_login_admin("qr", "qr");

            string so_seri = Request.QueryString["so_seri"];
            if (!string.IsNullOrEmpty(so_seri))
            {
                LoadProductDetails(so_seri);
            }
            else
            {
                pnContent.Visible = false;
                pnError.Visible = true;
            }
        }
    }

    private void LoadProductDetails(string so_seri)
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                var q = (from ob1 in db.KhoSanPham_tbs
                         join ob2 in db.DuLieuNguon_tbs
                             .Where(p => p.kyhieu == "hangsanpham") on ob1.id_hang equals ob2.id.ToString() into HangGroup
                         from ob2 in HangGroup.DefaultIfEmpty()
                         join ob3 in db.DuLieuNguon_tbs
                             .Where(p => p.kyhieu == "nhomsanpham") on ob1.id_nhom equals ob3.id.ToString() into NhomGroup
                         from ob3 in NhomGroup.DefaultIfEmpty()
                         join ob4 in db.DuLieuNguon_tbs
                             .Where(p => p.kyhieu == "donvitinh") on ob1.donvitinh equals ob4.id.ToString() into DVTGroup
                         from ob4 in DVTGroup.DefaultIfEmpty()
                         where ob1.so_seri == so_seri
                         select new
                         {
                             TenSP = ob1.ten,
                             Seri = ob1.so_seri,
                             Hang = ob2 == null ? "Chưa xác định" : ob2.ten,
                             Nhom = ob3 == null ? "Chưa xác định" : ob3.ten,
                             Model = string.IsNullOrEmpty(ob1.model) ? "Không có" : ob1.model,
                             DVT = ob4 == null ? "Chưa xác định" : ob4.ten,
                             Anh = string.IsNullOrEmpty(ob1.anh) ? "/uploads/images/no-image.png" : ob1.anh,
                             TonKho = ob1.soluong_hientai
                         }).FirstOrDefault();

                if (q != null)
                {
                    pnContent.Visible = true;
                    pnError.Visible = false;

                    litTenSP.Text = q.TenSP;
                    litSeri.Text = q.Seri;
                    litHang.Text = q.Hang;
                    litNhom.Text = q.Nhom;
                    litModel.Text = q.Model;
                    litDVT.Text = q.DVT;
                    litTonKho.Text = q.TonKho.HasValue ? q.TonKho.Value.ToString("#,##0") : "0";
                    imgProduct.ImageUrl = q.Anh;
                }
                else
                {
                    pnContent.Visible = false;
                    pnError.Visible = true;
                }
            }
        }
        catch (Exception ex)
        {
            pnContent.Visible = false;
            pnError.Visible = true;
            // Log error here if needed
        }
    }
}
