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
        ddl_export_attendance_account.DataSource = accounts;
        ddl_export_attendance_account.DataTextField = "hoten";
        ddl_export_attendance_account.DataValueField = "taikhoan";
        ddl_export_attendance_account.DataBind();
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
            ChamCong_tb attendance = new ChamCong_tb
            {
                taikhoan = account,
                ngaychamcong = attendanceDate.Date.Add(startTime),
                baoraca = attendanceDate.Date.Add(endTime),
                LCB_hientai = currentBasicSalary,
                LuongNgay_ChamCong = currentBasicSalary / 26,
                xacnhan_vaoca = true
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

    protected void btn_export_attendance_confirm_Click(object sender, EventArgs e)
    {
        EnsureAttendanceEditPermission();

        string account = ddl_export_attendance_account.SelectedValue;
        DateTime displayDate;
        if (!DateTime.TryParse(TextBox3.Text, out displayDate))
            displayDate = DateTime.Now;

        DateTime startDate = dt_cl.return_ngaydauthang(displayDate.Month.ToString(), displayDate.Year.ToString()).Date;
        DateTime endDate = dt_cl.return_ngaycuoithang(displayDate.Month.ToString(), displayDate.Year.ToString()).Date;

        using (dbDataContext db = new dbDataContext())
        {
            var employee = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == account);
            if (employee == null)
            {
                SetAttendanceEditMessage("Không tìm thấy tài khoản cần xuất dữ liệu.", false);
                return;
            }

            var attendanceRecords = db.ChamCong_tbs
                .Where(p => p.taikhoan == account
                    && p.ngaychamcong.HasValue
                    && p.ngaychamcong.Value.Date >= startDate
                    && p.ngaychamcong.Value.Date <= endDate)
                .OrderBy(p => p.ngaychamcong)
                .ToList();

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

            // Dùng định dạng Excel 97-2003 để các máy/Excel đời cũ vẫn mở được.
            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Chấm công");
            ICellStyle titleStyle = workbook.CreateCellStyle();
            IFont titleFont = workbook.CreateFont();
            titleFont.IsBold = true;
            titleStyle.SetFont(titleFont);

            int rowIndex = 0;
            IRow employeeRow = sheet.CreateRow(rowIndex++);
            employeeRow.CreateCell(0).SetCellValue("Nhân viên");
            employeeRow.CreateCell(1).SetCellValue((employee.hoten ?? "") + " (" + employee.taikhoan + ")");
            employeeRow.GetCell(0).CellStyle = titleStyle;
            employeeRow.GetCell(1).CellStyle = titleStyle;

            IRow periodRow = sheet.CreateRow(rowIndex++);
            periodRow.CreateCell(0).SetCellValue("Kỳ chấm công");
            periodRow.CreateCell(1).SetCellValue(startDate.ToString("dd/MM/yyyy") + " - " + endDate.ToString("dd/MM/yyyy"));
            periodRow.GetCell(0).CellStyle = titleStyle;
            periodRow.GetCell(1).CellStyle = titleStyle;
            rowIndex++;

            IRow summaryTitle = sheet.CreateRow(rowIndex++);
            summaryTitle.CreateCell(0).SetCellValue("TỔNG QUÁT THÁNG");
            summaryTitle.GetCell(0).CellStyle = titleStyle;
            IRow summaryHeader = sheet.CreateRow(rowIndex++);
            string[] summaryHeaders = { "Ngày công", "Lương ngày công", "Xăng xe", "Ăn trưa", "Điện thoại", "Trách nhiệm", "Tổng thu nhập cố định" };
            for (int i = 0; i < summaryHeaders.Length; i++)
            {
                summaryHeader.CreateCell(i).SetCellValue(summaryHeaders[i]);
                summaryHeader.GetCell(i).CellStyle = titleStyle;
            }
            long fixedIncome = basicSalary + travelAllowance + mealAllowance + phoneAllowance + responsibilityAllowance;
            IRow summaryRow = sheet.CreateRow(rowIndex++);
            long[] summaryValues = { workingDays, basicSalary, travelAllowance, mealAllowance, phoneAllowance, responsibilityAllowance, fixedIncome };
            for (int i = 0; i < summaryValues.Length; i++)
                summaryRow.CreateCell(i).SetCellValue(summaryValues[i]);
            rowIndex++;

            IRow calendarTitle = sheet.CreateRow(rowIndex++);
            calendarTitle.CreateCell(0).SetCellValue("THEO DÕI NGÀY LÀM VIỆC");
            calendarTitle.GetCell(0).CellStyle = titleStyle;
            IRow calendarHeader = sheet.CreateRow(rowIndex++);
            calendarHeader.CreateCell(0).SetCellValue("Ngày trong tháng");
            calendarHeader.GetCell(0).CellStyle = titleStyle;
            int calendarColumn = 1;
            for (DateTime currentDate = startDate; currentDate <= endDate; currentDate = currentDate.AddDays(1))
            {
                calendarHeader.CreateCell(calendarColumn).SetCellValue(currentDate.ToString("dd/MM"));
                calendarHeader.GetCell(calendarColumn).CellStyle = titleStyle;
                calendarColumn++;
            }

            IRow calendarRow = sheet.CreateRow(rowIndex++);
            calendarRow.CreateCell(0).SetCellValue("Chấm công");
            calendarRow.GetCell(0).CellStyle = titleStyle;
            calendarColumn = 1;
            for (DateTime currentDate = startDate; currentDate <= endDate; currentDate = currentDate.AddDays(1))
            {
                var attendance = attendanceRecords.FirstOrDefault(p => p.ngaychamcong.HasValue && p.ngaychamcong.Value.Date == currentDate.Date);
                string attendanceText = "";
                if (attendance != null)
                {
                    string startTime = attendance.ngaychamcong.Value.ToString("HH'h'mm");
                    string endTime = attendance.baoraca.HasValue ? attendance.baoraca.Value.ToString("HH'h'mm") : "";
                    attendanceText = "Có [" + startTime + "]" + (string.IsNullOrEmpty(endTime) ? "" : "-[" + endTime + "]");
                }
                calendarRow.CreateCell(calendarColumn++).SetCellValue(attendanceText);
            }
            rowIndex++;

            for (int i = 0; i < calendarColumn; i++)
                sheet.AutoSizeColumn(i);

            using (MemoryStream stream = new MemoryStream())
            {
                workbook.Write(stream);
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", "attachment;filename=ChamCong_" + account + "_" + startDate.ToString("yyyyMM") + ".xls");
                Response.BinaryWrite(stream.ToArray());
                Response.Flush();
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
        }
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
