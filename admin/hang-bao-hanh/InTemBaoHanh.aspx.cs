using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_hang_bao_hanh_InTemBaoHanh : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadData();
        }
    }

    private void LoadData()
    {
        string id = Request.QueryString["id"];
        if (!string.IsNullOrEmpty(id))
        {
            try
            {
                using (dbDataContext db = new dbDataContext())
                {
                    var phieu = db.HangBaoHanh_tbs.FirstOrDefault(p => p.id.ToString() == id);
                    if (phieu != null)
                    {
                        lblMaPhieu.Text = "BH-" + phieu.id.ToString();
                        lblKhachHang.Text = phieu.ten_khachhang;
                        lblSDT.Text = phieu.sdt_khachhang;
                        lblNgayNhan.Text = phieu.ngaynhan.HasValue ? phieu.ngaynhan.Value.ToString("dd/MM/yyyy HH:mm") : "";

                        var chitiet = db.HangBaoHanh_ChiTiet_tbs.Where(p => p.id_PhieuBaoHanh == phieu.id.ToString()).ToList();
                        rptChiTiet.DataSource = chitiet;
                        rptChiTiet.DataBind();
                    }
                    else
                    {
                        Response.Write("Không tìm thấy phiếu bảo hành.");
                        Response.End();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("Lỗi: " + ex.Message);
                Response.End();
            }
        }
        else
        {
            Response.Write("Thiếu mã phiếu.");
            Response.End();
        }
    }
}
