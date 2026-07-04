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
            check_login_cl.check_login_admin("7", "7");

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
                             Id = ob1.id,
                             Gianhap = ob1.gianhap,
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
                    ViewState["id_edit"] = q.Id.ToString();
                    ViewState["gianhap_hientai"] = q.Gianhap.ToString();
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

    protected void but_show_form_nhaphang_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_admin("qr", "qr");
        Label3.Text = litTenSP.Text;
        Label4.Text = litTonKho.Text;
        pn_nhaphang.Visible = true;
        up_nhaphang.Update();
    }

    protected void but_close_form_nhaphang_Click(object sender, EventArgs e)
    {
        reset_control_nhaphang();
        pn_nhaphang.Visible = false;
        up_nhaphang.Update();
    }

    private void reset_control_nhaphang()
    {
        txt_soluong_nhap.Text = "";
    }

    protected void but_nhaphang_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("qr", "qr");
            DateTime _ngaynhap = DateTime.Now;
            string _nguoinhap = mahoa_cl.giaima_Bcorn(Session["taikhoan"].ToString());
            int _soluongnhap = Number_cl.Check_Int(txt_soluong_nhap.Text.Trim());
            Int64 _gianhap = Number_cl.Check_Int64(ViewState["gianhap_hientai"].ToString());
            using (dbDataContext db = new dbDataContext())
            {
                if (_soluongnhap == 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Số lượng nhập không hợp lệ.", "false", "false", "OK", "alert", ""), true);
                    return;
                }
                var q = db.KhoSanPham_tbs.FirstOrDefault(p => p.id.ToString() == ViewState["id_edit"].ToString());
                if (q != null)
                {
                    NhapXuatKho_tb _ob = new NhapXuatKho_tb();
                    _ob.nhap_hay_xuat = true;//nhập
                    _ob.id_sanpham = ViewState["id_edit"].ToString();
                    _ob.ten_sanpham = q.ten;
                    _ob.soluong_nhap = _soluongnhap;
                    _ob.gia_nhap = _gianhap;
                    _ob.ngaynhap = _ngaynhap;
                    _ob.nguoinhap = _nguoinhap;
                    _ob.ton_hientai = q.soluong_hientai;//tồn trước khi nhập
                    q.soluong_hientai = q.soluong_hientai + _soluongnhap;//tăng tồn hiện tại
                    db.NhapXuatKho_tbs.InsertOnSubmit(_ob);
                    db.SubmitChanges();

                    // Load lại thông tin
                    LoadProductDetails(q.so_seri);
                    up_main.Update();

                    //reset control
                    reset_control_nhaphang();
                    //ẩn form
                    pn_nhaphang.Visible = false;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
                }
            }
        }
        catch(Exception _ex)
        {
            Response.Redirect("/admin");
        }
    }
}
