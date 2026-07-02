using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_quan_ly_nhan_vien_bang_cham_cong : System.Web.UI.Page
{
    DateTime_cl dt_cl = new DateTime_cl();
    protected void Page_Load(object sender, EventArgs e)
    {
        Session["url_back"] = HttpContext.Current.Request.Url.AbsoluteUri;
        check_login_cl.check_login_admin("15", "28");

        string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
        if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
        {
            _tk = mahoa_cl.giaima_Bcorn(_tk);
        }
        else
            _tk = "";

        ViewState["taikhoan"] = _tk;

        using (dbDataContext db = new dbDataContext())
        {

            DateTime _dautuan = dt_cl.return_ngaydauthang(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString());
            TextBox3.Text = _dautuan.ToShortDateString();
            DateTime _cuoituan = dt_cl.return_ngaycuoithang(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString());
            Label24.Text = "Từ " + _dautuan.ToShortDateString() + " đến " + _cuoituan.ToShortDateString();
            main_bangdiemdanh(db, _dautuan, _cuoituan);
        }
    }
    protected void TextBox3_TextChanged(object sender, EventArgs e)//chọn ngày ngẫu nhiên sau đó tính ngày đầu tuần và ngày cuối tuần
    {
        DateTime _ngay = DateTime.Parse(TextBox3.Text);


        DateTime _dautuan = dt_cl.return_ngaydauthang(_ngay.Month.ToString(), _ngay.Year.ToString());
        TextBox3.Text = _dautuan.ToShortDateString();
        DateTime _cuoituan = dt_cl.return_ngaycuoithang(_ngay.Month.ToString(), _ngay.Year.ToString());
        using (dbDataContext db = new dbDataContext())
        {
            main_bangdiemdanh(db, _dautuan, _cuoituan);
        }
        Label24.Text = "Từ " + _dautuan.ToShortDateString() + " đến " + _cuoituan.ToShortDateString();

    }
    protected void LinkButton7_Click(object sender, EventArgs e)//lùi 1 tuần
    {
        using (dbDataContext db = new dbDataContext())
        {
            DateTime _ngay = DateTime.Parse(TextBox3.Text);

            DateTime _dautuan = dt_cl.return_ngaydauthangtruoc(_ngay.Month.ToString(), _ngay.Year.ToString());
            TextBox3.Text = _dautuan.ToShortDateString();
            DateTime _cuoituan = dt_cl.return_ngaycuoithangtruoc(_ngay.Month.ToString(), _ngay.Year.ToString());
            main_bangdiemdanh(db, _dautuan, _cuoituan);
            Label24.Text = "Từ " + _dautuan.ToShortDateString() + " đến " + _cuoituan.ToShortDateString();
        }
    }

    protected void LinkButton8_Click(object sender, EventArgs e)//tới 1 tuần
    {
        using (dbDataContext db = new dbDataContext())
        {
            DateTime _ngay = DateTime.Parse(TextBox3.Text);
            _ngay = _ngay.AddMonths(1);
            DateTime _dautuan = dt_cl.return_ngaydauthang(_ngay.Month.ToString(), _ngay.Year.ToString());
            TextBox3.Text = _dautuan.ToShortDateString();
            DateTime _cuoituan = dt_cl.return_ngaycuoithang(_ngay.Month.ToString(), _ngay.Year.ToString());
            main_bangdiemdanh(db, _dautuan, _cuoituan);
            Label24.Text = "Từ " + _dautuan.ToShortDateString() + " đến " + _cuoituan.ToShortDateString();

        }
    }
    protected void LinkButton9_Click(object sender, EventArgs e)//tuần này
    {
        using (dbDataContext db = new dbDataContext())
        {
            DateTime _ngay = DateTime.Now;

            DateTime _dautuan = dt_cl.return_ngaydauthang(_ngay.Month.ToString(), _ngay.Year.ToString());
            TextBox3.Text = _dautuan.ToShortDateString();
            DateTime _cuoituan = dt_cl.return_ngaycuoithang(_ngay.Month.ToString(), _ngay.Year.ToString());
            main_bangdiemdanh(db, _dautuan, _cuoituan);
            Label24.Text = "Từ " + _dautuan.ToShortDateString() + " đến " + _cuoituan.ToShortDateString();

        }
    }
    public void main_bangdiemdanh(dbDataContext db, DateTime _dautuan, DateTime _cuoituan)
    {
        // Sử dụng StringBuilder để tạo bảng HTML động
        StringBuilder htmlTable = new StringBuilder();

        // Mở thẻ table và tạo hàng đầu tiên
        htmlTable.Append("<table class='table row-hover table-border cell-border compact bg-white bcorn-fix-title-table'>");
        htmlTable.Append("<tbody>");
        htmlTable.Append("<tr style='background-color: #ecf0f5'>");

        // Cột số thứ tự và nhân viên
        htmlTable.Append("<td class='text-bold bg-cobalt fg-white text-center' style='width: 1px; min-width: 1px'>TT</td>");
        htmlTable.Append("<td class='text-bold bg-cobalt fg-white  text-left' style='width: 140px; min-width: 140px'>Nhân viên</td>");

        // Thêm cột cho mỗi ngày từ đầu tuần đến cuối tuần
        for (DateTime currentDay = _dautuan; currentDay <= _cuoituan; currentDay = currentDay.AddDays(1))
        {
            htmlTable.Append("<td class='bg-gray fw-600 text-center' style='width: 1px; min-width: 1px'>" + dt_cl.return_thuvietnam_viettat(currentDay) + "<br/>" + currentDay.ToString("dd/MM") + "</td>");
        }

        //tiêu đề cột tổng kết
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Ngày<br/>công</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>LCB</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Xăng<br/>xe</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Ăn<br/>trưa</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Điện<br/>thoại</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Trách<br/>nhiệm</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Doanh<br/>số</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Thưởng<br/>D.số</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Tổng<br/>cộng</td>");
        //htmlTable.Append("<td class='text-center bg-orange fg-white' style='width:1px;min-width:1px'>Phạt</td>");
        htmlTable.Append("<td class='text-center bg-red fg-white' style='width:1px;min-width:1px'>Thực<br/>nhận</td>");

        // Kết thúc hàng đầu tiên
        htmlTable.Append("</tr>");

        // Thực hiện join bảng chamcong_pg_chitiet_tb với taikhoan_table_2023 để lấy tên nhân viên
        var danhSachNhanVien = (from cc in db.ChamCong_tbs
                                join tk in db.taikhoan_tbs on cc.taikhoan equals tk.taikhoan
                                where
                                 cc.ngaychamcong.Value.Date >= _dautuan.Date
                                && cc.ngaychamcong.Value.Date <= _cuoituan.Date
                                select new
                                {
                                    cc.taikhoan,
                                    cc.ngaychamcong,
                                    tk.hoten,
                                    cc.baoraca,
                                    cc.LuongNgay_ChamCong,
                                    tk.ten,
                                }).OrderBy(x => x.ten).ToList();  // Sắp xếp theo tên từ A-Z

        if (check_login_cl.CheckQuyen(db, ViewState["taikhoan"].ToString(), "28"))
        { danhSachNhanVien = danhSachNhanVien.Where(p => p.taikhoan == ViewState["taikhoan"].ToString()).ToList(); }

        // Lấy danh sách các nhân viên duy nhất (dựa trên tài khoản)
        var nhanVienList = danhSachNhanVien
            .Select(x => new { x.taikhoan, x.hoten })
            .Distinct()
            .ToList();

        int counter = 1; // Đếm số thứ tự
        int TongKet_NgayCong = 0; Int64 TongKet_LCB = 0, TongKet_XangXe = 0, TongKet_AnUong = 0, TongKet_DienThoai = 0, TongKet_TrachNhiem = 0, TongKet_DoanhSo = 0, TongKet_ThuongDoanhSo = 0, TongKet_TongCong = 0, TongKet_Phat = 0, TongKet_ThucNhan = 0;
        // Tạo dòng dữ liệu cho mỗi nhân viên
        foreach (var nhanVien in nhanVienList)
        {
            // Khởi tạo các biến đếm cho từng loại trạng thái
            int tongNgayCong = 0;
            Int64 LuongCB = 0, _doanhso = 0, _doanhsoHangBaoHanh = 0, _thuongdoanhso = 0, _tongcong = 0, _phat = 0, _thucnhan = 0;
            htmlTable.Append("<tr>");
            htmlTable.Append("<td class='text-center  bg-cobalt fg-white'>" + counter + "</td>"); // Số thứ tự


            htmlTable.Append("<td class='text-left  bg-cobalt fg-white'>" + nhanVien.hoten + "</td>"); // Hiển thị tên bình thường

            // Hiển thị trạng thái chấm công theo từng ngày từ _dautuan đến _cuoituan
            for (DateTime currentDay = _dautuan.Date; currentDay <= _cuoituan.Date; currentDay = currentDay.AddDays(1))
            {
                var chamCong = danhSachNhanVien.FirstOrDefault(x =>
                    x.taikhoan == nhanVien.taikhoan &&
                    x.ngaychamcong.Value.Date == currentDay.Date);

                if (chamCong != null)
                {
                    //đếm số ngày công
                    tongNgayCong++;
                    //cộng dồn LCB
                    LuongCB = LuongCB + chamCong.LuongNgay_ChamCong.Value;
                    htmlTable.Append("<td class='text-center'>");

                    if (chamCong.baoraca != null)
                        htmlTable.Append("<div><span data-role='hint' data-hint-position='top' data-hint-text='Vào: " + chamCong.ngaychamcong.Value.ToString("HH:mm") + " - Ra: " + chamCong.baoraca.Value.ToString("HH:mm") + "' class='mif-checkmark fg-green'></span></div>");
                    else
                        htmlTable.Append("<div><span data-role='hint' data-hint-position='top' data-hint-text='Vào: " + chamCong.ngaychamcong.Value.ToString("HH:mm") + " - Ra: Không có' class='mif-checkmark fg-orange'></span></div>");
                    //string _trangthai_chamcong = "";
                    //switch (_trangthai_chamcong)
                    //{
                    //    case "1":
                    //        tongNgayCong++;
                    //        //htmlTable.Append("<div class='bg-green' data-role='hint' data-hint-position='top' data-hint-text='" + chamCong.ngaychamcong.Value.ToString("HH:mm") + "'>&nbsp;</div>");
                    //        break;
                    //    default:
                    //        htmlTable.Append("-"); // Hoặc trạng thái khác
                    //        break;
                    //}

                    htmlTable.Append("</td>");
                }
                else
                {
                    // Nếu không có dữ liệu, để trống hoặc hiển thị "-"
                    htmlTable.Append("<td class='text-center'></td>");
                }
            }
            // Thêm dòng thống kê ở cuối
            var q_nv = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == nhanVien.taikhoan);
            htmlTable.Append("<td class='text-center text-bold'>" + tongNgayCong + "</td>");

            htmlTable.Append("<td class='text-right text-bold'>" + LuongCB.ToString("#,##0") + "</td>");

            //htmlTable.Append("<td class='text-right '>" + q_nv.PhuCap_Xangxe.Value.ToString("#,##0") + "</td>");
            //htmlTable.Append("<td class='text-right '>" + q_nv.PhuCap_AnUong.Value.ToString("#,##0") + "</td>");
            //htmlTable.Append("<td class='text-right '>" + q_nv.PhuCap_DienThoai.Value.ToString("#,##0") + "</td>");
            //htmlTable.Append("<td class='text-right '>" + q_nv.PhuCap_TrachNhiem.Value.ToString("#,##0") + "</td>");

            #region tính các phụ cấp theo ngày đi làm, làm mới tính
            // Tính hệ số theo số ngày công / 26
            decimal heSoNgayCong = tongNgayCong / 26m;
            // Tính phụ cấp quy đổi theo tỷ lệ (làm tròn .5 lên)
            long pcXangXe = (long)Math.Round(q_nv.PhuCap_Xangxe.Value * heSoNgayCong, MidpointRounding.AwayFromZero);
            long pcAnUong = (long)Math.Round(q_nv.PhuCap_AnUong.Value * (decimal)tongNgayCong, MidpointRounding.AwayFromZero);
            long pcDienThoai = (long)Math.Round(q_nv.PhuCap_DienThoai.Value * heSoNgayCong, MidpointRounding.AwayFromZero);
            long pcTrachNhiem = (long)Math.Round(q_nv.PhuCap_TrachNhiem.Value * heSoNgayCong, MidpointRounding.AwayFromZero);
            // Hiển thị các cột phụ cấp đã quy đổi
            htmlTable.Append("<td class='text-right '>" + pcXangXe.ToString("#,##0") + "</td>");
            htmlTable.Append("<td class='text-right '>" + pcAnUong.ToString("#,##0") + "</td>");
            htmlTable.Append("<td class='text-right '>" + pcDienThoai.ToString("#,##0") + "</td>");
            htmlTable.Append("<td class='text-right '>" + pcTrachNhiem.ToString("#,##0") + "</td>");
            #endregion


            var q_ds = db.BaoGia_tbs
    .Where(p =>
        p.trangthai == "Đã ký HĐ" &&
        p.ngayban_kyhopdong.HasValue &&
        p.ngayban_kyhopdong.Value.Date >= _dautuan.Date &&
        p.ngayban_kyhopdong.Value.Date <= _cuoituan.Date &&
        p.nguoibaogia == nhanVien.taikhoan);
            if (q_ds.Any())
            {
                _doanhso = q_ds.Sum(p => p.giatri_thuc_donhang.Value);
                _thuongdoanhso = q_ds.Sum(p => p.thuongdoanhso.Value);
            }

            var q_baohanh = db.HangBaoHanh_tbs
    .Where(p =>
        p.trangthai == "Đã trả" &&
        p.NgayTra_ThucTe.HasValue &&
        p.NgayTra_ThucTe.Value.Date >= _dautuan.Date &&
        p.NgayTra_ThucTe.Value.Date <= _cuoituan.Date &&
        p.nguoitao == nhanVien.taikhoan);
            if (q_baohanh.Any())
            {
                _doanhso = _doanhso + q_baohanh.Sum(p => p.tongtien.Value);
                _thuongdoanhso = _thuongdoanhso + q_baohanh.Sum(p => p.thuongdoanhso.Value);
            }
            //_doanhsoHangBaoHanh

            htmlTable.Append("<td class='text-right '>" + _doanhso.ToString("#,##0") + "</td>");//doanh số
            htmlTable.Append("<td class='text-right '>" + _thuongdoanhso.ToString("#,##0") + "</td>");//thưởng doanh số

            //_tongcong = LuongCB + q_nv.PhuCap_Xangxe.Value + q_nv.PhuCap_AnUong.Value + (long)q_nv.PhuCap_DienThoai.Value + q_nv.PhuCap_TrachNhiem.Value + _thuongdoanhso;
            _tongcong = LuongCB + pcXangXe + pcAnUong + pcDienThoai + pcTrachNhiem + _thuongdoanhso;


            htmlTable.Append("<td class='text-right text-bold'>" + _tongcong.ToString("#,##0") + "</td>");
            //htmlTable.Append("<td class='text-right fg-orange'>" + _phat.ToString("#,##0") + "</td>");
            _thucnhan = _tongcong; // _thucnhan = _tongcong - _phat;
            htmlTable.Append("<td class='text-right text-bold fg-red'>" + _thucnhan.ToString("#,##0") + "</td>");

            htmlTable.Append("</tr>");
            counter++; // Tăng số thứ tự

            //TỔNG KẾT
            TongKet_NgayCong = TongKet_NgayCong + tongNgayCong;
            TongKet_LCB = TongKet_LCB + LuongCB;

            //TongKet_XangXe = TongKet_XangXe + q_nv.PhuCap_Xangxe.Value;
            //TongKet_AnUong = TongKet_AnUong + q_nv.PhuCap_AnUong.Value;
            //TongKet_DienThoai = TongKet_DienThoai + (long)q_nv.PhuCap_DienThoai.Value;
            //TongKet_TrachNhiem = TongKet_TrachNhiem + q_nv.PhuCap_TrachNhiem.Value;
            TongKet_XangXe = TongKet_XangXe + pcXangXe;
            TongKet_AnUong = TongKet_AnUong + pcAnUong;
            TongKet_DienThoai = TongKet_DienThoai + pcDienThoai;
            TongKet_TrachNhiem = TongKet_TrachNhiem + pcTrachNhiem;


            TongKet_DoanhSo = TongKet_DoanhSo + _doanhso;
            TongKet_ThuongDoanhSo = TongKet_ThuongDoanhSo + _thuongdoanhso;
            TongKet_TongCong = TongKet_TongCong + _tongcong;
            TongKet_Phat = TongKet_Phat + _phat;
            TongKet_ThucNhan = TongKet_ThucNhan + _thucnhan;
        }
        htmlTable.Append("<tr class='bg-gray'><td class='bg-gray'></td><td class='bg-gray'></td>");
        TimeSpan _songay = _cuoituan - _dautuan;

        htmlTable.Append("<td class='text-right text-bold' colspan='" + (_songay.Days + 1) + "'>TỔNG</td>");
        htmlTable.Append("<td class='text-center text-bold'>" + TongKet_NgayCong.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold'>" + TongKet_LCB.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold'>" + TongKet_XangXe.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold'>" + TongKet_AnUong.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold'>" + TongKet_DienThoai.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold'>" + TongKet_TrachNhiem.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold'>" + TongKet_DoanhSo.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold'>" + TongKet_ThuongDoanhSo.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold'>" + TongKet_TongCong.ToString("#,##0") + "</td>");
        //htmlTable.Append("<td class='text-right text-bold fg-orange'>" + TongKet_Phat.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold fg-red'>" + TongKet_ThucNhan.ToString("#,##0") + "</td>");
        htmlTable.Append("</tr>");
        // Đóng thẻ table
        htmlTable.Append("</tbody>");
        htmlTable.Append("</table>");

        // Hiển thị ra màn hình
        Literal1.Text = htmlTable.ToString();
    }
}