using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_quan_ly_he_thong_cai_dat : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            Session["url_back"] = HttpContext.Current.Request.Url.AbsoluteUri;
            check_login_cl.check_login_admin("14", "14");
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";

            using (dbDataContext db = new dbDataContext())
            {
                load_data_all(db);
            }

        }
    }
    public void load_data_all(dbDataContext db)
    {
        var q = db.CaiDatChung_tbs.Where(p => p.phanloai_trang == "admin".ToString()).Select(p => new
        {
            p.vido,
            p.kinhdo,
        }).FirstOrDefault();
        if (q != null)
        {
            txt_vido.Text = q.vido;
            txt_kinhdo.Text = q.kinhdo;
        }
    }

    protected void but_update_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            string _vido = txt_vido.Text.Trim();
            string _kinhdo = txt_kinhdo.Text.Trim();
            var q = db.CaiDatChung_tbs.FirstOrDefault(p => p.phanloai_trang == "admin");
            if (q != null)
            {
                q.vido = _vido;
                q.kinhdo = _kinhdo;
                db.SubmitChanges();
                load_data_all(db);
                up_main.Update();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
            }
        }
    }
}