using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;

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
            bool canEditAttendance = check_login_cl.CheckQuyen(db, _tk, "15");
            btn_edit_attendance.Visible = canEditAttendance;
            btn_export_attendance.Visible = canEditAttendance;
            btn_nhap_tangca.Visible = canEditAttendance;
            btn_chitiet_tangca.Visible = canEditAttendance;
            btn_nhap_tamung.Visible = canEditAttendance;
            if (canEditAttendance)
                ScriptManager.GetCurrent(Page).RegisterPostBackControl(btn_export_attendance_confirm);

            if (!IsPostBack)
            {
                DateTime _dautuan = dt_cl.return_ngaydauthang(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString());
                TextBox3.Text = _dautuan.ToShortDateString();
                txt_edit_attendance_date.Text = _dautuan.ToString("dd/MM/yyyy");
                SetDefaultAttendanceTimes();
                LoadAttendanceAccounts(db);
            }

            DateTime _ngayHienThi;
            if (!DateTime.TryParse(TextBox3.Text, out _ngayHienThi))
                _ngayHienThi = DateTime.Now;

            DateTime _dautuanHienThi = dt_cl.return_ngaydauthang(_ngayHienThi.Month.ToString(), _ngayHienThi.Year.ToString());
            DateTime _cuoituanHienThi = dt_cl.return_ngaycuoithang(_ngayHienThi.Month.ToString(), _ngayHienThi.Year.ToString());
            Label24.Text = "Từ " + _dautuanHienThi.ToShortDateString() + " đến " + _cuoituanHienThi.ToShortDateString();
            // Export builds its own report; avoid rendering/querying the on-screen table again.
            if (Request.Form["__EVENTTARGET"] != btn_export_attendance_confirm.UniqueID)
                main_bangdiemdanh(db, _dautuanHienThi, _cuoituanHienThi);
        }
    }

    private void LoadAttendanceAccounts(dbDataContext db)
    {
        var accounts = db.taikhoan_tbs
            .Where(p => p.phanloai == "Nhân viên" || p.phanloai == "Quản trị")
            .Select(p => new { p.taikhoan, p.hoten })
            .OrderBy(p => p.hoten)
            .ToList();
        ddl_edit_attendance_account.DataSource = accounts;
        ddl_edit_attendance_account.DataTextField = "hoten";
        ddl_edit_attendance_account.DataValueField = "taikhoan";
        ddl_edit_attendance_account.DataBind();
        ddl_tangca_account.DataSource = accounts;
        ddl_tangca_account.DataTextField = "hoten";
        ddl_tangca_account.DataValueField = "taikhoan";
        ddl_tangca_account.DataBind();
        ddl_chitiet_tangca_account.DataSource = accounts;
        ddl_chitiet_tangca_account.DataTextField = "hoten";
        ddl_chitiet_tangca_account.DataValueField = "taikhoan";
        ddl_chitiet_tangca_account.DataBind();
        ddl_tamung_account.DataSource = accounts;
        ddl_tamung_account.DataTextField = "hoten";
        ddl_tamung_account.DataValueField = "taikhoan";
        ddl_tamung_account.DataBind();
    }

    private bool TryGetAttendanceDate(out DateTime attendanceDate)
    {
        return DateTime.TryParseExact(
            txt_edit_attendance_date.Text.Trim(),
            "dd/MM/yyyy",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None,
            out attendanceDate);
    }

    private bool TryGetAttendanceTime(string value, out TimeSpan attendanceTime)
    {
        return TimeSpan.TryParseExact(
            value.Trim(),
            "hh\\:mm",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.TimeSpanStyles.None,
            out attendanceTime);
    }

    private void SetDefaultAttendanceTimes()
    {
        if (string.IsNullOrWhiteSpace(txt_edit_attendance_start_time.Text))
            txt_edit_attendance_start_time.Text = "08:00";
        if (string.IsNullOrWhiteSpace(txt_edit_attendance_end_time.Text))
            txt_edit_attendance_end_time.Text = "17:00";
    }

    private void EnsureAttendanceEditPermission()
    {
        check_login_cl.check_login_admin("15", "15");
    }

    protected void btn_edit_attendance_Click(object sender, EventArgs e)
    {
        EnsureAttendanceEditPermission();
        SetDefaultAttendanceTimes();
        pn_edit_attendance.Visible = !pn_edit_attendance.Visible;
        lbl_edit_attendance_message.Text = "";
    }

    protected void btn_add_attendance_Click(object sender, EventArgs e)
    {
        EnsureAttendanceEditPermission();
        DateTime attendanceDate;
        if (!TryGetAttendanceDate(out attendanceDate))
        {
            lbl_edit_attendance_message.Text = "Ngày chấm công không hợp lệ. Vui lòng nhập theo định dạng dd/MM/yyyy.";
            lbl_edit_attendance_message.CssClass = "d-block mt-1 fg-red";
            return;
        }

        TimeSpan startTime;
        TimeSpan endTime;
        if (!TryGetAttendanceTime(txt_edit_attendance_start_time.Text, out startTime)
            || !TryGetAttendanceTime(txt_edit_attendance_end_time.Text, out endTime))
        {
            SetAttendanceEditMessage("Giờ vào ca hoặc giờ ra ca không hợp lệ. Vui lòng nhập theo định dạng HH:mm.", false);
            return;
        }

        string account = ddl_edit_attendance_account.SelectedValue;
        using (dbDataContext db = new dbDataContext())
        {
            var employee = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == account);
            if (employee == null)
            {
                SetAttendanceEditMessage("Không tìm thấy tài khoản cần chấm công.", false);
                return;
            }

            bool alreadyExists = db.ChamCong_tbs.Any(p => p.taikhoan == account
                && p.ngaychamcong.HasValue
                && p.ngaychamcong.Value.Date == attendanceDate.Date);
            if (alreadyExists)
            {
                SetAttendanceEditMessage("Tài khoản này đã có ngày chấm công được chọn.", false);
                return;
            }

            long currentBasicSalary = employee.LuongCoBan ?? 0;
            DateTime vaoCa = attendanceDate.Date.Add(startTime);
            DateTime raCa = attendanceDate.Date.Add(endTime);
            double totalHours = (raCa - vaoCa).TotalHours;
            double overtime = totalHours - 9.0;

            ChamCong_tb attendance = new ChamCong_tb
            {
                taikhoan = account,
                ngaychamcong = vaoCa,
                baoraca = raCa,
                LCB_hientai = currentBasicSalary,
                LuongNgay_ChamCong = currentBasicSalary / 26,
                xacnhan_vaoca = true,
                SoGioDu = overtime > 0 ? (decimal)overtime : 0,
                HeSoTangCa = 1.5m
            };
            db.ChamCong_tbs.InsertOnSubmit(attendance);
            db.SubmitChanges();
            SetAttendanceEditMessage("Đã thêm ngày chấm công. Tiền đã được tính lại theo ngày công mới.", true);
        }

        RefreshAttendanceTable();
    }

    protected void btn_delete_attendance_Click(object sender, EventArgs e)
    {
        EnsureAttendanceEditPermission();
        DateTime attendanceDate;
        if (!TryGetAttendanceDate(out attendanceDate))
        {
            lbl_edit_attendance_message.Text = "Ngày chấm công không hợp lệ. Vui lòng nhập theo định dạng dd/MM/yyyy.";
            lbl_edit_attendance_message.CssClass = "d-block mt-1 fg-red";
            return;
        }

        string account = ddl_edit_attendance_account.SelectedValue;
        using (dbDataContext db = new dbDataContext())
        {
            var attendanceRecords = db.ChamCong_tbs
                .Where(p => p.taikhoan == account
                    && p.ngaychamcong.HasValue
                    && p.ngaychamcong.Value.Date == attendanceDate.Date)
                .ToList();
            if (attendanceRecords.Count == 0)
            {
                SetAttendanceEditMessage("Không có ngày chấm công nào để xóa.", false);
                return;
            }

            db.ChamCong_tbs.DeleteAllOnSubmit(attendanceRecords);
            db.SubmitChanges();
            SetAttendanceEditMessage("Đã xóa ngày chấm công. Tiền đã được tính lại theo ngày công còn lại.", true);
        }

        RefreshAttendanceTable();
    }

    protected void btn_export_attendance_Click(object sender, EventArgs e)
    {
        EnsureAttendanceEditPermission();
        pn_export_attendance.Visible = !pn_export_attendance.Visible;
    }

    protected void btn_nhap_tangca_Click(object sender, EventArgs e)
    {
        EnsureAttendanceEditPermission();
        pn_nhap_tangca.Visible = !pn_nhap_tangca.Visible;
        lbl_tangca_message.Text = "";
        if (pn_nhap_tangca.Visible)
        {
            // Load giá trị hiện tại của NV đang chọn cho tháng đang xem
            DateTime displayDate;
            if (!DateTime.TryParse(TextBox3.Text, out displayDate))
                displayDate = DateTime.Now;
            LoadTangCaValue(displayDate.Month, displayDate.Year);
        }
    }

    protected void btn_save_tangca_Click(object sender, EventArgs e)
    {
        EnsureAttendanceEditPermission();
        string account = ddl_tangca_account.SelectedValue;
        DateTime displayDate;
        if (!DateTime.TryParse(TextBox3.Text, out displayDate))
            displayDate = DateTime.Now;
        int thang = displayDate.Month;
        int nam = displayDate.Year;
        long tienTangCa = Number_cl.Check_Int64(txt_tien_tangca_nhap.Text.Trim());
        string ghichu = txt_tangca_ghichu.Text.Trim();
        string nguoiThucHien = ViewState["taikhoan"] != null ? ViewState["taikhoan"].ToString() : "";

        using (dbDataContext db = new dbDataContext())
        {
            var existing = db.TangCa_tbs.FirstOrDefault(p =>
                p.taikhoan == account && p.thang == thang && p.nam == nam);
            if (existing != null)
            {
                existing.tien_tang_ca = tienTangCa;
                existing.ghichu = ghichu;
                existing.ngaytao = DateTime.Now;
                existing.nguoitao = nguoiThucHien;
            }
            else
            {
                TangCa_tb newRecord = new TangCa_tb();
                newRecord.taikhoan = account;
                newRecord.thang = thang;
                newRecord.nam = nam;
                newRecord.tien_tang_ca = tienTangCa;
                newRecord.ghichu = ghichu;
                newRecord.ngaytao = DateTime.Now;
                newRecord.nguoitao = nguoiThucHien;
                db.TangCa_tbs.InsertOnSubmit(newRecord);
            }
            db.SubmitChanges();
        }
        lbl_tangca_message.CssClass = "d-block mt-1 fg-green";
        lbl_tangca_message.Text = "✔ Đã lưu tiền tăng ca tháng " + thang + "/" + nam + " cho nhân viên này.";
        RefreshAttendanceTable();
    }

    protected void btn_nhap_tamung_Click(object sender, EventArgs e)
    {
        EnsureAttendanceEditPermission();
        pn_nhap_tamung.Visible = !pn_nhap_tamung.Visible;
        lbl_tamung_message.Text = "";
        if (pn_nhap_tamung.Visible)
        {
            // Load giá trị hiện tại của NV đang chọn cho tháng đang xem
            DateTime displayDate;
            if (!DateTime.TryParse(TextBox3.Text, out displayDate))
                displayDate = DateTime.Now;
            LoadTamUngValue(displayDate.Month, displayDate.Year);
        }
    }

    private void LoadTamUngValue(int thang, int nam)
    {
        using (dbDataContext db = new dbDataContext())
        {
            var record = db.TangCa_tbs.FirstOrDefault(p => p.taikhoan == ddl_tamung_account.SelectedValue && p.thang == thang && p.nam == nam);
            if (record != null && record.tamung_ky1.HasValue)
                txt_tien_tamung_nhap.Text = record.tamung_ky1.Value.ToString("#,##0");
            else
                txt_tien_tamung_nhap.Text = "0";
        }
    }

    protected void btn_save_tamung_Click(object sender, EventArgs e)
    {
        EnsureAttendanceEditPermission();
        string account = ddl_tamung_account.SelectedValue;
        DateTime displayDate;
        if (!DateTime.TryParse(TextBox3.Text, out displayDate))
            displayDate = DateTime.Now;
        int thang = displayDate.Month;
        int nam = displayDate.Year;
        long tienTamUng = Number_cl.Check_Int64(txt_tien_tamung_nhap.Text.Trim());
        string nguoiThucHien = ViewState["taikhoan"] != null ? ViewState["taikhoan"].ToString() : "";

        using (dbDataContext db = new dbDataContext())
        {
            var existing = db.TangCa_tbs.FirstOrDefault(p =>
                p.taikhoan == account && p.thang == thang && p.nam == nam);
            if (existing != null)
            {
                existing.tamung_ky1 = tienTamUng;
                existing.ngaytao = DateTime.Now;
                existing.nguoitao = nguoiThucHien;
            }
            else
            {
                TangCa_tb newRecord = new TangCa_tb();
                newRecord.taikhoan = account;
                newRecord.thang = thang;
                newRecord.nam = nam;
                newRecord.tamung_ky1 = tienTamUng;
                newRecord.ngaytao = DateTime.Now;
                newRecord.nguoitao = nguoiThucHien;
                db.TangCa_tbs.InsertOnSubmit(newRecord);
            }
            db.SubmitChanges();
        }
        lbl_tamung_message.CssClass = "d-block mt-1 fg-green";
        lbl_tamung_message.Text = "✔ Đã lưu tiền tạm ứng tháng " + thang + "/" + nam + " cho nhân viên này.";
        RefreshAttendanceTable();
    }

    protected void btn_chitiet_tangca_Click(object sender, EventArgs e)
    {
        EnsureAttendanceEditPermission();
        pn_chitiet_tangca.Visible = !pn_chitiet_tangca.Visible;
        lbl_chitiet_tangca_message.Text = "";
        pn_chitiet_table.Visible = false; // Ẩn bảng cho đến khi bấm Xem
    }

    protected void btn_xem_chitiet_tangca_Click(object sender, EventArgs e)
    {
        EnsureAttendanceEditPermission();
        lbl_chitiet_tangca_message.Text = "";
        ViewState["EditMode_TangCa"] = false; // Mặc định là chế độ View
        btn_edit_chitiet_tangca.Visible = true;
        btn_save_chitiet_tangca.Visible = false;

        string selectedAccount = ddl_chitiet_tangca_account.SelectedValue;
        lbl_chitiet_tangca_hoten.Text = ddl_chitiet_tangca_account.SelectedItem.Text;
        
        LoadChiTietTangCa(selectedAccount);
        pn_chitiet_table.Visible = true;
    }

    protected void btn_edit_chitiet_tangca_Click(object sender, EventArgs e)
    {
        ViewState["EditMode_TangCa"] = true;
        btn_edit_chitiet_tangca.Visible = false;
        btn_save_chitiet_tangca.Visible = true;
        
        string selectedAccount = ddl_chitiet_tangca_account.SelectedValue;
        LoadChiTietTangCa(selectedAccount);
    }

    private void LoadChiTietTangCa(string account)
    {
        DateTime displayDate;
        if (!DateTime.TryParse(TextBox3.Text, out displayDate))
            displayDate = DateTime.Now;

        DateTime _dautuan = dt_cl.return_ngaydauthang(displayDate.Month.ToString(), displayDate.Year.ToString());
        DateTime _cuoituan = dt_cl.return_ngaycuoithang(displayDate.Month.ToString(), displayDate.Year.ToString());

        using (dbDataContext db = new dbDataContext())
        {
            var query = from cc in db.ChamCong_tbs
                        join tk in db.taikhoan_tbs on cc.taikhoan equals tk.taikhoan
                        where cc.ngaychamcong >= _dautuan.Date && cc.ngaychamcong <= _cuoituan.Date
                              && cc.SoGioDu > 0 && cc.taikhoan == account
                        orderby cc.ngaychamcong
                        select new
                        {
                            cc.id,
                            tk.hoten,
                            cc.ngaychamcong,
                            SoGioDu = cc.SoGioDu ?? 0,
                            HeSoTangCa = cc.HeSoTangCa ?? 1.5m,
                            Luong1Gio = tk.LuongCoBan > 0 ? (tk.LuongCoBan.Value / 26m / 8m) : 0,
                            ThanhTien = (cc.SoGioDu ?? 0) * (cc.HeSoTangCa ?? 1.5m) * (tk.LuongCoBan > 0 ? (tk.LuongCoBan.Value / 26m / 8m) : 0)
                        };

            rpt_chitiet_tangca.DataSource = query.ToList();
            rpt_chitiet_tangca.DataBind();
        }
    }

    protected void rpt_chitiet_tangca_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            bool isEditMode = ViewState["EditMode_TangCa"] != null && (bool)ViewState["EditMode_TangCa"];
            
            Label lbl_sogiodu = (Label)e.Item.FindControl("lbl_sogiodu");
            TextBox txt_sogiodu = (TextBox)e.Item.FindControl("txt_sogiodu");
            Label lbl_heso = (Label)e.Item.FindControl("lbl_heso");
            TextBox txt_heso = (TextBox)e.Item.FindControl("txt_heso");

            if (lbl_sogiodu != null && txt_sogiodu != null)
            {
                lbl_sogiodu.Visible = !isEditMode;
                txt_sogiodu.Visible = isEditMode;
            }
            if (lbl_heso != null && txt_heso != null)
            {
                lbl_heso.Visible = !isEditMode;
                txt_heso.Visible = isEditMode;
            }
        }
    }

    protected void btn_save_chitiet_tangca_Click(object sender, EventArgs e)
    {
        EnsureAttendanceEditPermission();
        using (dbDataContext db = new dbDataContext())
        {
            foreach (RepeaterItem item in rpt_chitiet_tangca.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    HiddenField hdf_id = (HiddenField)item.FindControl("hdf_id_chamcong");
                    TextBox txt_sogiodu = (TextBox)item.FindControl("txt_sogiodu");
                    TextBox txt_heso = (TextBox)item.FindControl("txt_heso");

                    long id = Convert.ToInt64(hdf_id.Value);
                    decimal soGioDu = 0, heSo = 0;
                    decimal.TryParse(txt_sogiodu.Text.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out soGioDu);
                    decimal.TryParse(txt_heso.Text.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out heSo);

                    var record = db.ChamCong_tbs.FirstOrDefault(p => p.id == id);
                    if (record != null)
                    {
                        record.SoGioDu = soGioDu;
                        record.HeSoTangCa = heSo;
                    }
                }
            }
            db.SubmitChanges();
        }
        
        ViewState["EditMode_TangCa"] = false;
        btn_edit_chitiet_tangca.Visible = true;
        btn_save_chitiet_tangca.Visible = false;

        lbl_chitiet_tangca_message.CssClass = "d-block mt-1 fg-green";
        lbl_chitiet_tangca_message.Text = "✔ Đã lưu chi tiết tăng ca ngày thành công.";
        
        string selectedAccount = ddl_chitiet_tangca_account.SelectedValue;
        LoadChiTietTangCa(selectedAccount);
        RefreshAttendanceTable();
    }

    private void LoadTangCaValue(int thang, int nam)
    {
        string account = ddl_tangca_account.SelectedValue;
        using (dbDataContext db = new dbDataContext())
        {
            var existing = db.TangCa_tbs.FirstOrDefault(p =>
                p.taikhoan == account && p.thang == thang && p.nam == nam);
            txt_tien_tangca_nhap.Text = existing != null && existing.tien_tang_ca.HasValue
                ? existing.tien_tang_ca.Value.ToString("#,##0") : "";
            txt_tangca_ghichu.Text = existing != null ? (existing.ghichu ?? "") : "";
        }
    }

    protected void btn_export_attendance_confirm_Click(object sender, EventArgs e)
    {
        EnsureAttendanceEditPermission();
        Server.ScriptTimeout = 600;


        DateTime displayDate;
        if (!DateTime.TryParse(TextBox3.Text, out displayDate))
            displayDate = DateTime.Now;

        DateTime startDate = dt_cl.return_ngaydauthang(displayDate.Month.ToString(), displayDate.Year.ToString()).Date;
        DateTime endDate = dt_cl.return_ngaycuoithang(displayDate.Month.ToString(), displayDate.Year.ToString()).Date;

        using (dbDataContext db = new dbDataContext())
        {
            var employees = db.taikhoan_tbs
                .Where(p => p.phanloai == "Nhân viên" || p.phanloai == "Quản trị")
                .OrderBy(p => p.hoten).ToList();
            if (employees.Count == 0)
            {
                SetAttendanceEditMessage("Không tìm thấy nhân viên cần xuất dữ liệu.", false);
                return;
            }
            // Load each monthly source once; lookups avoid queries for every employee.
            DateTime nextMonth = endDate.AddDays(1);
            var attendanceByAccount = db.ChamCong_tbs
                .Where(p => p.ngaychamcong >= startDate && p.ngaychamcong < nextMonth)
                .OrderBy(p => p.ngaychamcong).ToList().ToLookup(p => p.taikhoan);
            var monthlyByAccount = db.TangCa_tbs
                .Where(p => p.thang == startDate.Month && p.nam == startDate.Year)
                .ToList().ToLookup(p => p.taikhoan);
            var salesByAccount = db.BaoGia_tbs
                .Where(p => p.trangthai == "Đã ký HĐ" && p.ngayban_kyhopdong >= startDate && p.ngayban_kyhopdong < nextMonth)
                .Select(p => new { p.nguoibaogia, p.giatri_thuc_donhang, p.thuongdoanhso })
                .ToList().ToLookup(p => p.nguoibaogia);
            var warrantyByAccount = db.HangBaoHanh_tbs
                .Where(p => p.trangthai == "Đã trả" && p.NgayTra_ThucTe >= startDate && p.NgayTra_ThucTe < nextMonth)
                .Select(p => new { p.nguoitao, p.tongtien, p.thuongdoanhso })
                .ToList().ToLookup(p => p.nguoitao);
            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Chấm công tổng hợp");
            ICellStyle titleStyle = workbook.CreateCellStyle();
            IFont titleFont = workbook.CreateFont();
            titleFont.IsBold = true;
            titleStyle.SetFont(titleFont);
            titleStyle.WrapText = true;
            ICellStyle numberStyle = workbook.CreateCellStyle();
            numberStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0");
            ICellStyle decimalStyle = workbook.CreateCellStyle();
            decimalStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
            ICellStyle attendanceStyle = workbook.CreateCellStyle();
            attendanceStyle.WrapText = true;
            sheet.CreateRow(0).CreateCell(0).SetCellValue("BẢNG CHẤM CÔNG TOÀN BỘ NHÂN VIÊN");
            sheet.GetRow(0).GetCell(0).CellStyle = titleStyle;
            sheet.CreateRow(1).CreateCell(0).SetCellValue("Kỳ: " + startDate.ToString("dd/MM/yyyy") + " - " + endDate.ToString("dd/MM/yyyy"));
            string[] summaryHeaders = { "Ngày công", "LCB", "Xăng xe", "Ăn trưa", "Điện thoại", "Trách nhiệm", "R&D", "Trực hotline", "Hỗ trợ D.A", "Doanh số", "Thưởng D.số", "Tổng tiền tăng ca", "Tổng GROSS dự kiến", "NLĐ đóng BH (10,5%)", "Thực nhận trước PIT", "DN đóng BH (21,5%)", "Kinh phí CĐ (2%)", "Tổng chi phí DN", "Tạm ứng kỳ 1", "Nhận kỳ 2", "Ngân sách Max", "Chênh lệch chờ PL" };
            int daysInMonth = (endDate - startDate).Days + 1;
            int summaryOffset = 3;
            int calendarOffset = summaryOffset + summaryHeaders.Length;
            IRow header = sheet.CreateRow(3);
            header.HeightInPoints = 45;
            header.CreateCell(0).SetCellValue("STT");
            header.CreateCell(1).SetCellValue("Tài khoản");
            header.CreateCell(2).SetCellValue("Nhân viên");
            for (int day = 0; day < daysInMonth; day++)
                header.CreateCell(calendarOffset + day).SetCellValue(dt_cl.return_thuvietnam_viettat(startDate.AddDays(day)) + "\n" + startDate.AddDays(day).ToString("dd/MM"));
            for (int i = 0; i < summaryHeaders.Length; i++)
                header.CreateCell(summaryOffset + i).SetCellValue(summaryHeaders[i]);
            foreach (ICell cell in header.Cells)
                cell.CellStyle = titleStyle;
            int rowIndex = 4;
            long[] totals = new long[summaryHeaders.Length];
            int overtimeRowIndex = employees.Count + 8;
            sheet.CreateRow(overtimeRowIndex++).CreateCell(0).SetCellValue("CHI TIẾT TĂNG CA TẤT CẢ NHÂN VIÊN");
            sheet.GetRow(overtimeRowIndex - 1).GetCell(0).CellStyle = titleStyle;
            IRow overtimeHeader = sheet.CreateRow(overtimeRowIndex++);
            string[] overtimeHeaders = { "STT", "Tài khoản", "Nhân viên", "Ngày công", "Giờ vào", "Giờ ra", "Giờ tăng ca / ngày", "Hệ số", "Lương 1 giờ", "Tiền tăng ca / ngày", "Ghi chú", "Tổng giờ tăng ca", "Tổng tiền tăng ca" };
            for (int i = 0; i < overtimeHeaders.Length; i++)
            {
                overtimeHeader.CreateCell(i).SetCellValue(overtimeHeaders[i]);
                overtimeHeader.GetCell(i).CellStyle = titleStyle;
            }
            decimal totalOvertimeHours = 0;
            foreach (var employee in employees)
            {
                var attendanceRecords = attendanceByAccount[employee.taikhoan].ToList();

                long basicSalary = attendanceRecords.Sum(p => p.LuongNgay_ChamCong ?? 0);
                int workingDays = attendanceRecords
                    .Where(p => p.ngaychamcong.HasValue)
                    .Select(p => p.ngaychamcong.Value.Date)
                    .Distinct()
                    .Count();
                int mealEligibleDays = attendanceRecords
                    .Where(p => p.ngaychamcong.HasValue
                        && p.baoraca.HasValue
                        && p.baoraca.Value - p.ngaychamcong.Value >= TimeSpan.FromHours(7.5))
                    .Select(p => p.ngaychamcong.Value.Date)
                    .Distinct()
                    .Count();
                decimal allowanceRatio = workingDays / 26m;
                long travelAllowance = (long)Math.Round((employee.PhuCap_Xangxe ?? 0) * allowanceRatio, MidpointRounding.AwayFromZero);
                long mealAllowance = (long)Math.Round((employee.PhuCap_AnUong ?? 0) * mealEligibleDays / 26m, MidpointRounding.AwayFromZero);
                long phoneAllowance = (long)Math.Round((employee.PhuCap_DienThoai ?? 0) * allowanceRatio, MidpointRounding.AwayFromZero);
                long responsibilityAllowance = (long)Math.Round((employee.PhuCap_TrachNhiem ?? 0) * allowanceRatio, MidpointRounding.AwayFromZero);

                long research = (long)Math.Round((employee.PhuCap_RnD ?? 0) * allowanceRatio, MidpointRounding.AwayFromZero);
                long hotline = (long)Math.Round((employee.PhuCap_TrucHotline ?? 0) * allowanceRatio, MidpointRounding.AwayFromZero);
                long project = (long)Math.Round((employee.Thuong_DuAn_Max ?? 0) * allowanceRatio, MidpointRounding.AwayFromZero);
                var monthly = monthlyByAccount[employee.taikhoan].ToList();
                long manualOvertime = monthly.Sum(p => (long)(p.tien_tang_ca ?? 0));
                long advance = monthly.Sum(p => (long)(p.tamung_ky1 ?? 0));
                decimal hourlySalary = employee.LuongCoBan > 0 ? employee.LuongCoBan.Value / 26m / 8m : 0;
                var overtimeRecords = attendanceRecords.Where(p => p.SoGioDu > 0).ToList();
                long dailyOvertime = overtimeRecords.Sum(p => (long)Math.Round((p.SoGioDu ?? 0) * (p.HeSoTangCa ?? 1.5m) * hourlySalary, MidpointRounding.AwayFromZero));
                var sales = salesByAccount[employee.taikhoan];
                var warranty = warrantyByAccount[employee.taikhoan];
                long revenue = sales.Sum(p => p.giatri_thuc_donhang ?? 0) + warranty.Sum(p => p.tongtien ?? 0);
                long bonus = sales.Sum(p => p.thuongdoanhso ?? 0) + warranty.Sum(p => p.thuongdoanhso ?? 0);
                long gross = basicSalary + travelAllowance + mealAllowance + phoneAllowance + responsibilityAllowance
                    + research + hotline + project + bonus + manualOvertime + dailyOvertime;
                long insuranceBase = (long)(employee.LuongDongBH ?? 0);
                long employeeInsurance = (long)Math.Round(insuranceBase * 0.105m, MidpointRounding.AwayFromZero);
                long employerInsurance = (long)Math.Round(insuranceBase * 0.215m, MidpointRounding.AwayFromZero);
                long union = (long)Math.Round(insuranceBase * 0.02m, MidpointRounding.AwayFromZero);
                long net = gross - employeeInsurance;
                long employerCost = gross + employerInsurance + union;
                long budget = (long)(employee.NganSach_Max ?? 0);

                IRow summaryRow = sheet.CreateRow(rowIndex++);
                summaryRow.CreateCell(0).SetCellValue(rowIndex - 4);
                summaryRow.CreateCell(1).SetCellValue(employee.taikhoan);
                summaryRow.CreateCell(2).SetCellValue(employee.hoten ?? "");
                for (int day = 0; day < daysInMonth; day++)
                {
                    DateTime date = startDate.AddDays(day);
                    var records = attendanceRecords.Where(p => p.ngaychamcong.Value.Date == date).ToList();
                    string times = string.Join("\n", records.Select(p => p.ngaychamcong.Value.ToString("HH:mm") + " - "
                        + (p.baoraca.HasValue ? p.baoraca.Value.ToString("HH:mm") : "Chưa ra ca")).ToArray());
                    summaryRow.CreateCell(calendarOffset + day).SetCellValue(times);
                    summaryRow.GetCell(calendarOffset + day).CellStyle = attendanceStyle;
                    summaryRow.HeightInPoints = Math.Max(summaryRow.HeightInPoints, Math.Max(1, records.Count) * 16);
                }
                long[] summaryValues = { workingDays, basicSalary, travelAllowance, mealAllowance, phoneAllowance, responsibilityAllowance, research, hotline, project, revenue, bonus, manualOvertime + dailyOvertime, gross, employeeInsurance, net, employerInsurance, union, employerCost, advance, net - advance, budget, budget - employerCost };
                for (int i = 0; i < summaryValues.Length; i++)
                {
                    summaryRow.CreateCell(summaryOffset + i).SetCellValue(summaryValues[i]);
                    summaryRow.GetCell(summaryOffset + i).CellStyle = numberStyle;
                    totals[i] += summaryValues[i];
                }
                totalOvertimeHours += overtimeRecords.Sum(p => p.SoGioDu ?? 0);
                var manualRecords = monthly.Where(p => (p.tien_tang_ca ?? 0) != 0).ToList();
                if (overtimeRecords.Count > 0 || manualRecords.Count > 0)
                {
                    IRow detail = sheet.CreateRow(overtimeRowIndex++);
                    detail.CreateCell(0).SetCellValue(overtimeRowIndex - overtimeHeader.RowNum - 1);
                    detail.CreateCell(1).SetCellValue(employee.taikhoan);
                    detail.CreateCell(2).SetCellValue(employee.hoten ?? "");
                    var lines = Enumerable.Range(0, 8).Select(i => new List<string>()).ToArray();
                    var culture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
                    foreach (var overtime in overtimeRecords)
                    {
                        lines[0].Add(overtime.ngaychamcong.Value.ToString("dd'/'MM'/'yyyy"));
                        lines[1].Add(overtime.ngaychamcong.Value.ToString("HH:mm"));
                        lines[2].Add(overtime.baoraca.HasValue ? overtime.baoraca.Value.ToString("HH:mm") : "—");
                        lines[3].Add((overtime.SoGioDu ?? 0).ToString("0.00", culture));
                        lines[4].Add((overtime.HeSoTangCa ?? 1.5m).ToString("0.00", culture));
                        lines[5].Add(hourlySalary.ToString("#,##0.00", culture));
                        lines[6].Add(Math.Round((overtime.SoGioDu ?? 0) * (overtime.HeSoTangCa ?? 1.5m)
                            * hourlySalary, MidpointRounding.AwayFromZero).ToString("#,##0", culture));
                        lines[7].Add("—");
                    }
                    foreach (var manual in manualRecords)
                    {
                        lines[0].Add("Nhập tay " + startDate.ToString("MM'/'yyyy"));
                        for (int i = 1; i <= 5; i++) lines[i].Add("—");
                        lines[6].Add((manual.tien_tang_ca ?? 0).ToString("#,##0", culture));
                        lines[7].Add((manual.ghichu ?? "—").Replace("\r", " ").Replace("\n", " "));
                    }
                    for (int i = 0; i < lines.Length; i++)
                        detail.CreateCell(3 + i).SetCellValue(string.Join("\n", lines[i].ToArray()));
                    // Numeric employee totals remain directly summable by accounting.
                    detail.CreateCell(11).SetCellValue((double)overtimeRecords.Sum(p => p.SoGioDu ?? 0));
                    detail.CreateCell(12).SetCellValue(manualOvertime + dailyOvertime);
                }
            }
            IRow totalRow = sheet.CreateRow(rowIndex);
            totalRow.CreateCell(2).SetCellValue("TỔNG CỘNG");
            totalRow.GetCell(2).CellStyle = titleStyle;
            for (int i = 0; i < totals.Length; i++)
            {
                totalRow.CreateCell(summaryOffset + i).SetCellValue(totals[i]);
                totalRow.GetCell(summaryOffset + i).CellStyle = numberStyle;
            }
            if (overtimeRowIndex == overtimeHeader.RowNum + 1)
                sheet.CreateRow(overtimeRowIndex++).CreateCell(2).SetCellValue("Không phát sinh tăng ca trong tháng.");
            IRow overtimeTotal = sheet.CreateRow(overtimeRowIndex++);
            overtimeTotal.CreateCell(2).SetCellValue("TỔNG TĂNG CA");
            overtimeTotal.GetCell(2).CellStyle = titleStyle;
            overtimeTotal.CreateCell(11).SetCellValue((double)totalOvertimeHours);
            overtimeTotal.CreateCell(12).SetCellValue(totals[11]);
            for (int row = overtimeHeader.RowNum + 1; row < overtimeRowIndex; row++)
                for (int col = 6; col <= 9; col++)
                {
                    ICell cell = sheet.GetRow(row).GetCell(col);
                    if (cell != null)
                        cell.CellStyle = col == 9 ? numberStyle : decimalStyle;
                }
            FormatAttendanceExport(workbook, sheet, startDate, employees.Count, calendarOffset, daysInMonth,
                header.RowNum, totalRow.RowNum, overtimeHeader.RowNum, overtimeTotal.RowNum);
            using (MemoryStream stream = new MemoryStream())
            {
                workbook.Write(stream);
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", "attachment;filename=ChamCong_" + "TatCa" + "_" + startDate.ToString("yyyyMM") + ".xls");
                Response.BinaryWrite(stream.ToArray());
                Response.Flush();
                Response.SuppressContent = true;
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
        }
    }

    private static ICellStyle AttendanceExportStyle(IWorkbook workbook, short fill, bool bold, bool white, string format)
    {
        IFont font = workbook.CreateFont();
        font.FontName = "Arial";
        font.FontHeightInPoints = 10;
        font.IsBold = bold;
        font.Color = white ? IndexedColors.White.Index : IndexedColors.Black.Index;
        ICellStyle style = workbook.CreateCellStyle();
        style.SetFont(font);
        style.VerticalAlignment = VerticalAlignment.Center;
        style.WrapText = true;
        if (fill >= 0)
        {
            style.FillForegroundColor = fill;
            style.FillPattern = FillPattern.SolidForeground;
        }
        if (format != null)
        {
            style.DataFormat = workbook.CreateDataFormat().GetFormat(format);
            style.Alignment = HorizontalAlignment.Right;
        }
        return style;
    }

    private static void FormatAttendanceExport(IWorkbook workbook, ISheet sheet, DateTime startDate,
        int employeeCount, int calendarOffset, int daysInMonth, int headerRow, int totalRow,
        int overtimeHeaderRow, int overtimeTotalRow)
    {
        const string moneyFormat = "#,##0;[Red](#,##0);–";
        const string decimalFormat = "#,##0.00;[Red](#,##0.00);–";
        HSSFPalette palette = ((HSSFWorkbook)workbook).GetCustomPalette();
        palette.SetColorAtIndex(IndexedColors.DarkBlue.Index, 23, 43, 77);
        palette.SetColorAtIndex(IndexedColors.Grey25Percent.Index, 244, 247, 250);
        palette.SetColorAtIndex(IndexedColors.Grey40Percent.Index, 221, 228, 235);
        palette.SetColorAtIndex(IndexedColors.LightTurquoise.Index, 225, 241, 235);
        var heading = AttendanceExportStyle(workbook, IndexedColors.DarkBlue.Index, true, true, null);
        heading.Alignment = HorizontalAlignment.Center;
        var group = AttendanceExportStyle(workbook, IndexedColors.Grey40Percent.Index, true, false, null);
        var text = AttendanceExportStyle(workbook, -1, false, false, null);
        var alternate = AttendanceExportStyle(workbook, IndexedColors.Grey25Percent.Index, false, false, null);
        var money = AttendanceExportStyle(workbook, -1, false, false, moneyFormat);
        var alternateMoney = AttendanceExportStyle(workbook, IndexedColors.Grey25Percent.Index, false, false, moneyFormat);
        var decimalStyle = AttendanceExportStyle(workbook, -1, false, false, decimalFormat);
        var alternateDecimal = AttendanceExportStyle(workbook, IndexedColors.Grey25Percent.Index, false, false, decimalFormat);
        var keyMoney = AttendanceExportStyle(workbook, IndexedColors.LightTurquoise.Index, true, false, moneyFormat);
        var totalMoney = AttendanceExportStyle(workbook, IndexedColors.DarkBlue.Index, true, true, moneyFormat);
        var weekend = AttendanceExportStyle(workbook, IndexedColors.LightYellow.Index, false, false, null);
        weekend.Alignment = HorizontalAlignment.Center;
        var warning = AttendanceExportStyle(workbook, IndexedColors.LightOrange.Index, false, false, null);
        var title = AttendanceExportStyle(workbook, -1, true, false, null);
        IFont titleFont = workbook.CreateFont();
        titleFont.FontName = "Arial";
        titleFont.FontHeightInPoints = 15;
        titleFont.IsBold = true;
        title.SetFont(titleFont);
        sheet.DisplayGridlines = false;
        sheet.GetRow(0).HeightInPoints = 30;
        sheet.GetRow(0).GetCell(0).CellStyle = title;
        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, 10));
        sheet.GetRow(1).GetCell(0).SetCellValue("Tháng " + startDate.ToString("MM/yyyy") + "  |  "
            + employeeCount + " nhân viên  |  Đơn vị tiền: VNĐ");
        sheet.GetRow(1).GetCell(0).CellStyle = text;
        sheet.GetRow(1).HeightInPoints = 25;
        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, 0, 10));
        IRow groups = sheet.CreateRow(2);
        groups.HeightInPoints = 26;
        int[] starts = { 0, 3, 5, 12, 15, 18, 21, 23, calendarOffset };
        int[] ends = { 2, 4, 11, 14, 17, 20, 22, 24, calendarOffset + daysInMonth - 1 };
        string[] labels = { "NHÂN VIÊN", "NGÀY CÔNG / LƯƠNG", "PHỤ CẤP", "DOANH SỐ / TĂNG CA",
            "THU NHẬP", "CHI PHÍ DOANH NGHIỆP", "THANH TOÁN", "NGÂN SÁCH", "GIỜ VÀO – RA THEO NGÀY" };
        for (int i = 0; i < starts.Length; i++)
        {
            for (int col = starts[i]; col <= ends[i]; col++)
                groups.CreateCell(col).CellStyle = group;
            groups.GetCell(starts[i]).SetCellValue(labels[i]);
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(2, 2, starts[i], ends[i]));
        }
        sheet.GetRow(headerRow).HeightInPoints = 48;
        foreach (ICell cell in sheet.GetRow(headerRow).Cells)
            cell.CellStyle = heading;
        for (int row = headerRow + 1; row < totalRow; row++)
        {
            bool band = (row - headerRow) % 2 == 0;
            IRow data = sheet.GetRow(row);
            data.HeightInPoints = Math.Max(32, data.HeightInPoints);
            foreach (ICell cell in data.Cells)
            {
                cell.CellStyle = cell.ColumnIndex < 3 || cell.ColumnIndex >= calendarOffset
                    ? (band ? alternate : text) : (band ? alternateMoney : money);
                if (cell.ColumnIndex == 15 || cell.ColumnIndex == 17 || cell.ColumnIndex == 22)
                    cell.CellStyle = keyMoney;
                if (cell.ColumnIndex >= calendarOffset)
                {
                    DayOfWeek day = startDate.AddDays(cell.ColumnIndex - calendarOffset).DayOfWeek;
                    if (day == DayOfWeek.Saturday || day == DayOfWeek.Sunday)
                        cell.CellStyle = weekend;
                    if (cell.StringCellValue.Contains("Chưa ra ca"))
                        cell.CellStyle = warning;
                }
            }
        }
        sheet.GetRow(totalRow).HeightInPoints = 30;
        for (int col = 0; col < calendarOffset; col++)
        {
            ICell cell = sheet.GetRow(totalRow).GetCell(col) ?? sheet.GetRow(totalRow).CreateCell(col);
            cell.CellStyle = col < 3 ? heading : totalMoney;
        }
        IRow note = sheet.CreateRow(totalRow + 2);
        note.CreateCell(0).SetCellValue("Vàng nhạt: cuối tuần. Cam: chưa có giờ ra ca. Tăng ca gồm chi tiết ngày và khoản nhập tay.");
        note.GetCell(0).CellStyle = text;
        note.HeightInPoints = 28;
        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(note.RowNum, note.RowNum, 0, 10));
        sheet.GetRow(overtimeHeaderRow - 1).GetCell(0).CellStyle = title;
        sheet.GetRow(overtimeHeaderRow - 1).HeightInPoints = 30;
        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(overtimeHeaderRow - 1, overtimeHeaderRow - 1, 0, 10));
        sheet.GetRow(overtimeHeaderRow).HeightInPoints = 36;
        foreach (ICell cell in sheet.GetRow(overtimeHeaderRow).Cells)
            cell.CellStyle = heading;
        var detailStyles = new ICellStyle[2];
        for (int band = 0; band < 2; band++)
        {
            detailStyles[band] = AttendanceExportStyle(workbook,
                band == 0 ? (short)-1 : IndexedColors.Grey25Percent.Index, false, false, null);
            detailStyles[band].VerticalAlignment = VerticalAlignment.Top;
            // Explicit newlines align each date with its hours, rate and amount.
            detailStyles[band].WrapText = true;
            IFont detailFont = workbook.CreateFont();
            detailFont.FontName = "Arial";
            detailFont.FontHeightInPoints = 9;
            detailStyles[band].SetFont(detailFont);
        }
        for (int row = overtimeHeaderRow + 1; row < overtimeTotalRow; row++)
        {
            IRow data = sheet.GetRow(row);
            int band = (row - overtimeHeaderRow - 1) % 2;
            int lineCount = data.GetCell(3) == null ? 1 : data.GetCell(3).StringCellValue.Split('\n').Length;
            data.HeightInPoints = Math.Min(409, Math.Max(32, lineCount * 12 + 8));
            for (int col = 0; col <= 12; col++)
            {
                ICell cell = data.GetCell(col) ?? data.CreateCell(col);
                cell.CellStyle = col == 12 ? keyMoney : col == 11 ? (band == 0 ? decimalStyle : alternateDecimal)
                    : col >= 3 ? detailStyles[band] : (band == 0 ? text : alternate);
            }
        }
        sheet.GetRow(overtimeTotalRow).HeightInPoints = 30;
        for (int col = 0; col <= 12; col++)
        {
            ICell cell = sheet.GetRow(overtimeTotalRow).GetCell(col) ?? sheet.GetRow(overtimeTotalRow).CreateCell(col);
            cell.CellStyle = col == 12 ? totalMoney : heading;
        }
        for (int col = 0; col < calendarOffset + daysInMonth; col++)
            sheet.SetColumnWidth(col, (col == 0 ? 7 : col == 1 ? 20 : col == 2 ? 28 : col >= calendarOffset ? 21 : 18) * 256);
        sheet.CreateFreezePane(3, headerRow + 1);
        sheet.SetZoom(85);
        sheet.PrintSetup.Landscape = true;
        // Preserve readable print size instead of squeezing 50+ columns onto one page.
        sheet.PrintSetup.Scale = 85;
        sheet.FitToPage = false;
    }
    private void SetAttendanceEditMessage(string message, bool success)
    {
        lbl_edit_attendance_message.Text = message;
        lbl_edit_attendance_message.CssClass = success ? "d-block mt-1 fg-green" : "d-block mt-1 fg-red";
    }

    private void RefreshAttendanceTable()
    {
        using (dbDataContext db = new dbDataContext())
        {
            DateTime displayDate;
            if (!DateTime.TryParse(TextBox3.Text, out displayDate))
                displayDate = DateTime.Now;
            DateTime startDate = dt_cl.return_ngaydauthang(displayDate.Month.ToString(), displayDate.Year.ToString());
            DateTime endDate = dt_cl.return_ngaycuoithang(displayDate.Month.ToString(), displayDate.Year.ToString());
            Label24.Text = "Từ " + startDate.ToShortDateString() + " đến " + endDate.ToShortDateString();
            main_bangdiemdanh(db, startDate, endDate);
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
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>R&D</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Trực<br/>hotline</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Hỗ trợ<br/>D.A</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Doanh<br/>số</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Thưởng<br/>D.số</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Tổng tiền<br/>tăng ca</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Tổng GROSS<br/>dự kiến</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>NLĐ đóng BH<br/>(10,5%)</td>");
        htmlTable.Append("<td class='text-center bg-red fg-white' style='width:1px;min-width:1px'>Thực nhận<br/>trước PIT</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>DN đóng BH<br/>(21,5%)</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Kinh phí CĐ<br/>(2%)</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Tổng chi phí<br/>DN</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Tạm ứng<br/>kỳ 1</td>");
        htmlTable.Append("<td class='text-center bg-red fg-white' style='width:1px;min-width:1px'>Nhận<br/>kỳ 2</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Ngân sách<br/>Max</td>");
        htmlTable.Append("<td class='text-center bg-cobalt fg-white' style='width:1px;min-width:1px'>Chênh lệch<br/>chờ PL</td>");

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
                                    cc.SoGioDu,
                                    cc.HeSoTangCa
                                }).OrderBy(x => x.ten).ToList();  // Sắp xếp theo tên từ A-Z

        if (check_login_cl.CheckQuyen(db, ViewState["taikhoan"].ToString(), "28"))
        { danhSachNhanVien = danhSachNhanVien.Where(p => p.taikhoan == ViewState["taikhoan"].ToString()).ToList(); }

        // Lấy danh sách các nhân viên duy nhất (dựa trên tài khoản)
        var nhanVienList = danhSachNhanVien
            .Select(x => new { x.taikhoan, x.hoten })
            .Distinct()
            .ToList();

        int counter = 1; // Đếm số thứ tự
        int TongKet_NgayCong = 0; Int64 TongKet_LCB = 0, TongKet_XangXe = 0, TongKet_AnUong = 0, TongKet_DienThoai = 0, TongKet_TrachNhiem = 0, TongKet_RnD = 0, TongKet_TrucHotline = 0, TongKet_HoTroDA = 0, TongKet_BaoHiem = 0, TongKet_DoanhSo = 0, TongKet_ThuongDoanhSo = 0, TongKet_TongCong = 0, TongKet_Phat = 0, TongKet_ThucNhan = 0, TongKet_DNBH = 0, TongKet_KinhPhiCD = 0, TongKet_ChiPhiDN = 0, TongKet_NganSach = 0, TongKet_ChenhLech = 0, TongKet_TienTangCa = 0, TongKet_TamUng_Ky1 = 0, TongKet_NhanKy2 = 0;

        // Load tất cả tiền tăng ca của tháng đang xem (1 lần, dùng chung)
        var monthlyRecords = db.TangCa_tbs.Where(p => p.thang == _dautuan.Month && p.nam == _dautuan.Year).ToList();
        var tangCaThang = monthlyRecords.ToDictionary(p => p.taikhoan ?? "", p => (long)(p.tien_tang_ca ?? 0));
        var tamUngThang = monthlyRecords.ToDictionary(p => p.taikhoan ?? "", p => (long)(p.tamung_ky1 ?? 0));

        // Tạo dòng dữ liệu cho mỗi nhân viên
        foreach (var nhanVien in nhanVienList)
        {
            // Khởi tạo các biến đếm cho từng loại trạng thái
            int tongNgayCong = 0;
            int mealEligibleDays = 0;
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
                    if (chamCong.baoraca.HasValue
                        && chamCong.baoraca.Value - chamCong.ngaychamcong.Value >= TimeSpan.FromHours(7.5))
                        mealEligibleDays++;
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
            long pcAnUong = (long)Math.Round((q_nv.PhuCap_AnUong ?? 0) * mealEligibleDays / 26m, MidpointRounding.AwayFromZero);
            long pcDienThoai = (long)Math.Round(q_nv.PhuCap_DienThoai.Value * heSoNgayCong, MidpointRounding.AwayFromZero);
            long pcTrachNhiem = (long)Math.Round(q_nv.PhuCap_TrachNhiem.Value * heSoNgayCong, MidpointRounding.AwayFromZero);
            long pcRnD = (long)Math.Round((q_nv.PhuCap_RnD ?? 0) * heSoNgayCong, MidpointRounding.AwayFromZero);
            long pcTrucHotline = (long)Math.Round((q_nv.PhuCap_TrucHotline ?? 0) * heSoNgayCong, MidpointRounding.AwayFromZero);
            long pcHoTroDA = (long)Math.Round((q_nv.Thuong_DuAn_Max ?? 0) * heSoNgayCong, MidpointRounding.AwayFromZero);
            long luongDongBH = (long)(q_nv.LuongDongBH ?? 0);

            // Hiển thị các cột phụ cấp đã quy đổi
            htmlTable.Append("<td class='text-right '>" + pcXangXe.ToString("#,##0") + "</td>");
            htmlTable.Append("<td class='text-right '>" + pcAnUong.ToString("#,##0") + "</td>");
            htmlTable.Append("<td class='text-right '>" + pcDienThoai.ToString("#,##0") + "</td>");
            htmlTable.Append("<td class='text-right '>" + pcTrachNhiem.ToString("#,##0") + "</td>");
            htmlTable.Append("<td class='text-right '>" + pcRnD.ToString("#,##0") + "</td>");
            htmlTable.Append("<td class='text-right '>" + pcTrucHotline.ToString("#,##0") + "</td>");
            htmlTable.Append("<td class='text-right '>" + pcHoTroDA.ToString("#,##0") + "</td>");
            
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
                _doanhso = q_ds.Sum(p => p.giatri_thuc_donhang) ?? 0;
                _thuongdoanhso = q_ds.Sum(p => p.thuongdoanhso) ?? 0;
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
                _doanhso = _doanhso + (q_baohanh.Sum(p => p.tongtien) ?? 0);
                _thuongdoanhso = _thuongdoanhso + (q_baohanh.Sum(p => p.thuongdoanhso) ?? 0);
            }
            
            htmlTable.Append("<td class='text-right '>" + _doanhso.ToString("#,##0") + "</td>");//doanh số
            htmlTable.Append("<td class='text-right '>" + _thuongdoanhso.ToString("#,##0") + "</td>");//thưởng doanh số

            // Tiền tăng ca: đọc từ bảng TangCa_tb theo tháng (nhập tay) cộng với tổng tiền tăng ca theo ngày
            long tienTangCaThuCong = tangCaThang.ContainsKey(nhanVien.taikhoan) ? tangCaThang[nhanVien.taikhoan] : 0;
            
            // Tính tổng tiền tăng ca tự động từ chấm công ngày
            long tienTangCaNgay = 0;
            decimal luong1GioDecimal = (q_nv != null && q_nv.LuongCoBan > 0) ? (q_nv.LuongCoBan.Value / 26m / 8m) : 0;
            var overtimeDays = danhSachNhanVien.Where(p => p.taikhoan == nhanVien.taikhoan).ToList();
            foreach (var cc in overtimeDays)
            {
                if (cc.SoGioDu > 0)
                {
                    decimal thanhTienThapPhan = (cc.SoGioDu ?? 0) * (cc.HeSoTangCa ?? 1.5m) * luong1GioDecimal;
                    tienTangCaNgay += (long)Math.Round(thanhTienThapPhan, MidpointRounding.AwayFromZero);
                }
            }

            long tienTangCa = tienTangCaThuCong + tienTangCaNgay;
            htmlTable.Append("<td class='text-right '>" + tienTangCa.ToString("#,##0") + "</td>");//tăng ca

            _tongcong = LuongCB + pcXangXe + pcAnUong + pcDienThoai + pcTrachNhiem + pcRnD + pcTrucHotline + pcHoTroDA + _thuongdoanhso + tienTangCa;
            long nldDongBH = (long)Math.Round(luongDongBH * 0.105m, MidpointRounding.AwayFromZero);
            _thucnhan = _tongcong - _phat - nldDongBH;

            // Tạm ứng kỳ 1 và nhận kỳ 2
            long tamUngKy1 = tamUngThang.ContainsKey(nhanVien.taikhoan) ? tamUngThang[nhanVien.taikhoan] : 0;
            long nhanKy2 = _thucnhan - tamUngKy1;
            
            long dnDongBH = (long)Math.Round(luongDongBH * 0.215m, MidpointRounding.AwayFromZero);
            long kpCD = (long)Math.Round(luongDongBH * 0.02m, MidpointRounding.AwayFromZero);
            long tongChiPhiDN = _tongcong + dnDongBH + kpCD;
            long nganSachMax = (long)(q_nv.NganSach_Max ?? 0);
            long chenhLech = nganSachMax - tongChiPhiDN;

                        htmlTable.Append("<td class='text-right text-bold'>" + _tongcong.ToString("#,##0") + "</td>");
            htmlTable.Append("<td class='text-right '>" + nldDongBH.ToString("#,##0") + "</td>");
            htmlTable.Append("<td class='text-right text-bold fg-red'>" + _thucnhan.ToString("#,##0") + "</td>");
            htmlTable.Append("<td class='text-right '>" + dnDongBH.ToString("#,##0") + "</td>");
            htmlTable.Append("<td class='text-right '>" + kpCD.ToString("#,##0") + "</td>");
            htmlTable.Append("<td class='text-right text-bold'>" + tongChiPhiDN.ToString("#,##0") + "</td>");
            htmlTable.Append("<td class='text-right '>" + tamUngKy1.ToString("#,##0") + "</td>");//tạm ứng kỳ 1
            htmlTable.Append("<td class='text-right text-bold " + (nhanKy2 < 0 ? "fg-red" : "fg-green") + "'>" + nhanKy2.ToString("#,##0") + "</td>");//nhận kỳ 2
            htmlTable.Append("<td class='text-right '>" + nganSachMax.ToString("#,##0") + "</td>");
            htmlTable.Append("<td class='text-right text-bold " + (chenhLech < 0 ? "fg-red" : "fg-green") + "'>" + chenhLech.ToString("#,##0") + "</td>");

            htmlTable.Append("</tr>");
            counter++; // Tăng số thứ tự

            //TỔNG KẾT
            TongKet_NgayCong = TongKet_NgayCong + tongNgayCong;
            TongKet_LCB = TongKet_LCB + LuongCB;
            TongKet_XangXe = TongKet_XangXe + pcXangXe;
            TongKet_AnUong = TongKet_AnUong + pcAnUong;
            TongKet_DienThoai = TongKet_DienThoai + pcDienThoai;
            TongKet_TrachNhiem = TongKet_TrachNhiem + pcTrachNhiem;
            TongKet_RnD = TongKet_RnD + pcRnD;
            TongKet_TrucHotline = TongKet_TrucHotline + pcTrucHotline;
            TongKet_HoTroDA = TongKet_HoTroDA + pcHoTroDA;
            TongKet_BaoHiem = TongKet_BaoHiem + nldDongBH;
            TongKet_DoanhSo = TongKet_DoanhSo + _doanhso;
            TongKet_ThuongDoanhSo = TongKet_ThuongDoanhSo + _thuongdoanhso;
            TongKet_TienTangCa = TongKet_TienTangCa + tienTangCa;
            TongKet_TongCong = TongKet_TongCong + _tongcong;
            TongKet_ThucNhan = TongKet_ThucNhan + _thucnhan;
            TongKet_TamUng_Ky1 = TongKet_TamUng_Ky1 + tamUngKy1;
            TongKet_NhanKy2 = TongKet_NhanKy2 + nhanKy2;
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
        htmlTable.Append("<td class='text-right text-bold'>" + TongKet_RnD.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold'>" + TongKet_TrucHotline.ToString("#,##0") + "</td>");
                htmlTable.Append("<td class='text-right text-bold'>" + TongKet_HoTroDA.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold'>" + TongKet_DoanhSo.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold'>" + TongKet_ThuongDoanhSo.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold'>" + TongKet_TienTangCa.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold'>" + TongKet_TongCong.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold'>" + TongKet_BaoHiem.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold fg-red'>" + TongKet_ThucNhan.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold'>" + TongKet_DNBH.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold'>" + TongKet_KinhPhiCD.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold'>" + TongKet_ChiPhiDN.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold'>" + TongKet_TamUng_Ky1.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold fg-red'>" + TongKet_NhanKy2.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold'>" + TongKet_NganSach.ToString("#,##0") + "</td>");
        htmlTable.Append("<td class='text-right text-bold'>" + TongKet_ChenhLech.ToString("#,##0") + "</td>");
        htmlTable.Append("</tr>");
        // Đóng thẻ table
        htmlTable.Append("</tbody>");
        htmlTable.Append("</table>");

        // Hiển thị ra màn hình
        Literal1.Text = htmlTable.ToString();
    }
}
