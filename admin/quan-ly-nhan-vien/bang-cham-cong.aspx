<%@ Page Title="Bảng chấm công" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="bang-cham-cong.aspx.cs" Inherits="admin_quan_ly_nhan_vien_bang_cham_cong" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .bcorn-fix-title-table {
            font-size: 15px !important;
        }

        .attendance-edit-panel {
            border: 1px solid #d8e0e8;
            border-radius: 4px;
            background: #f8fafc;
        }

            .bcorn-fix-title-table th:nth-child(1),
            .bcorn-fix-title-table td:nth-child(1),
            .bcorn-fix-title-table th:nth-child(2),
            .bcorn-fix-title-table td:nth-child(2) {
                position: sticky;
                left: 0;
                z-index: 3 !important;
            }
        .attendance-export-loading {
            display: flex; align-items: center; justify-content: center;
            position: fixed; inset: 0; z-index: 100000; background: rgba(15, 23, 42, .55);
        }
        .attendance-export-loading[hidden] { display: none; }
        .attendance-export-box {
            width: min(440px, 90vw); background: white; border-radius: 8px;
            padding: 28px; text-align: center; color: #172b4d;
        }
        .attendance-export-spinner {
            width: 40px; height: 40px; margin: 0 auto 18px;
            border: 4px solid #dbe5ee; border-top-color: #176b95; border-radius: 50%;
            animation: attendance-export-spin .8s linear infinite;
        }
        @keyframes attendance-export-spin { to { transform: rotate(360deg); } }
        @media (prefers-reduced-motion: reduce) { .attendance-export-spinner { animation: none; } }
    </style>
</asp:Content>
    <asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="p-3">
                <div class="mt-3 ">
                    <div class="row">
                        <div class="cell-lg-6  mb-3">
                            <label class="fw-600">
                                <asp:Label ID="Label24" runat="server" Text=""></asp:Label></label>
                            <div class="d-flex">
                                <asp:TextBox ID="TextBox3" AutoPostBack="true" OnTextChanged="TextBox3_TextChanged" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false"></asp:TextBox>
                                <asp:LinkButton ID="LinkButton7" runat="server" CssClass="button light" OnClick="LinkButton7_Click" ToolTip="Lùi"><</asp:LinkButton>
                                <asp:LinkButton ID="LinkButton9" runat="server" CssClass="button info" OnClick="LinkButton9_Click">Hiện tại</asp:LinkButton>
                                <asp:LinkButton ID="LinkButton8" runat="server" CssClass="button light" OnClick="LinkButton8_Click" ToolTip="Tới">></asp:LinkButton>
                            </div>
                        </div>
                    </div>

                    <div class="mt-2 mb-3">
                        <asp:LinkButton ID="btn_edit_attendance" runat="server" CssClass="button warning" OnClick="btn_edit_attendance_Click" Visible="false">
                            <span class="mif-pencil mr-1"></span> Chỉnh sửa chấm công
                        </asp:LinkButton>
                        <asp:LinkButton ID="btn_export_attendance" runat="server" CssClass="button info ml-1" OnClick="btn_export_attendance_Click" Visible="false">
                            <span class="mif-file-excel mr-1"></span> Xuất Excel chấm công
                        </asp:LinkButton>
                        <asp:LinkButton ID="btn_nhap_tangca" runat="server" CssClass="button secondary ml-1" OnClick="btn_nhap_tangca_Click" Visible="false">
                            <span class="mif-clock mr-1"></span> Nhập tiền tăng ca
                        </asp:LinkButton>
                        <asp:LinkButton ID="btn_chitiet_tangca" runat="server" CssClass="button success ml-1" OnClick="btn_chitiet_tangca_Click" Visible="false">
                            <span class="mif-list mr-1"></span> Chi tiết tăng ca ngày
                        </asp:LinkButton>
                        <asp:LinkButton ID="btn_nhap_tamung" runat="server" CssClass="button primary ml-1" OnClick="btn_nhap_tamung_Click" Visible="false">
                            <span class="mif-money mr-1"></span> Tạm ứng kỳ 1
                        </asp:LinkButton>
                    </div>

                    <asp:Panel ID="pn_nhap_tamung" runat="server" CssClass="attendance-edit-panel p-3 mb-3" Visible="false">
                        <div class="text-bold mb-2">Nhập tiền tạm ứng kỳ 1 theo tháng</div>
                        <div class="row flex-align-end">
                            <div class="cell-lg-4 cell-md-5 mb-2">
                                <label class="fw-600">Nhân viên</label>
                                <asp:DropDownList ID="ddl_tamung_account" runat="server" CssClass="w-100"></asp:DropDownList>
                            </div>
                            <div class="cell-lg-3 cell-md-4 mb-2">
                                <label class="fw-600">Tiền tạm ứng (VNĐ)</label>
                                <asp:TextBox ID="txt_tien_tamung_nhap" runat="server" data-role="input" MaxLength="14" oninput="format_sotien_new(this)" placeholder="0"></asp:TextBox>
                            </div>
                            <div class="cell-lg-2 cell-md-12 mb-2">
                                <asp:LinkButton ID="btn_save_tamung" runat="server" CssClass="button success" OnClick="btn_save_tamung_Click">
                                    <span class="mif-floppy-disk mr-1"></span> Lưu tạm ứng
                                </asp:LinkButton>
                            </div>
                        </div>
                        <asp:Label ID="lbl_tamung_message" runat="server" CssClass="d-block mt-1"></asp:Label>
                    </asp:Panel>

                    <asp:Panel ID="pn_nhap_tangca" runat="server" CssClass="attendance-edit-panel p-3 mb-3" Visible="false">
                        <div class="text-bold mb-2">Nhập tiền tăng ca theo tháng</div>
                        <div class="row flex-align-end">
                            <div class="cell-lg-4 cell-md-5 mb-2">
                                <label class="fw-600">Nhân viên</label>
                                <asp:DropDownList ID="ddl_tangca_account" runat="server" CssClass="w-100"></asp:DropDownList>
                            </div>
                            <div class="cell-lg-3 cell-md-4 mb-2">
                                <label class="fw-600">Tiền tăng ca (VNĐ)</label>
                                <asp:TextBox ID="txt_tien_tangca_nhap" runat="server" data-role="input" MaxLength="14" oninput="format_sotien_new(this)" placeholder="0"></asp:TextBox>
                            </div>
                            <div class="cell-lg-3 cell-md-4 mb-2">
                                <label class="fw-600">Ghi chú</label>
                                <asp:TextBox ID="txt_tangca_ghichu" runat="server" data-role="input" placeholder="Ghi chú (tùy chọn)"></asp:TextBox>
                            </div>
                            <div class="cell-lg-2 cell-md-12 mb-2">
                                <asp:LinkButton ID="btn_save_tangca" runat="server" CssClass="button success" OnClick="btn_save_tangca_Click">
                                    <span class="mif-floppy-disk mr-1"></span> Lưu
                                </asp:LinkButton>
                            </div>
                        </div>
                        <asp:Label ID="lbl_tangca_message" runat="server" CssClass="d-block mt-1"></asp:Label>
                    </asp:Panel>


                    <asp:Panel ID="pn_export_attendance" runat="server" CssClass="attendance-edit-panel p-3 mb-3" Visible="false">
                        <div class="text-bold mb-2">Xuất chấm công và chi tiết tăng ca theo tháng đang xem</div>
                        <div class="mb-2">Xuất toàn bộ nhân viên trong một sheet chung để kế toán kiểm tra. Chi tiết tăng ca của tất cả nhân viên nằm dưới bảng chấm công.</div>
                        <div class="d-flex flex-align-end">

                            <asp:LinkButton ID="btn_export_attendance_confirm" runat="server" CssClass="button info" OnClick="btn_export_attendance_confirm_Click" OnClientClick="return downloadAttendanceExcel(this);">
                                <span class="mif-download mr-1"></span> Xuất toàn bộ nhân viên
                            </asp:LinkButton>
                        </div>
                    </asp:Panel>

                    <asp:Panel ID="pn_edit_attendance" runat="server" CssClass="attendance-edit-panel p-3 mb-3" Visible="false">
                        <div class="text-bold mb-2">Chỉnh sửa ngày chấm công</div>
                        <div class="row flex-align-end">
                            <div class="cell-lg-4 cell-md-5 mb-2">
                                <label class="fw-600">Tài khoản</label>
                                <asp:DropDownList ID="ddl_edit_attendance_account" runat="server" CssClass="w-100"></asp:DropDownList>
                            </div>
                            <div class="cell-lg-3 cell-md-4 mb-2">
                                <label class="fw-600">Ngày chấm công</label>
                                <asp:TextBox ID="txt_edit_attendance_date" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false"></asp:TextBox>
                            </div>
                            <div class="cell-lg-2 cell-md-3 mb-2">
                                <label class="fw-600">Giờ vào ca</label>
                                <asp:TextBox ID="txt_edit_attendance_start_time" runat="server" TextMode="Time" CssClass="w-100"></asp:TextBox>
                            </div>
                            <div class="cell-lg-2 cell-md-3 mb-2">
                                <label class="fw-600">Giờ ra ca</label>
                                <asp:TextBox ID="txt_edit_attendance_end_time" runat="server" TextMode="Time" CssClass="w-100"></asp:TextBox>
                            </div>
                            <div class="cell-lg-3 cell-md-12 mb-2">
                                <asp:LinkButton ID="btn_add_attendance" runat="server" CssClass="button success mr-1" OnClick="btn_add_attendance_Click" OnClientClick="return confirm('Thêm ngày chấm công cho tài khoản này?');">
                                    <span class="mif-plus mr-1"></span> Thêm ngày công
                                </asp:LinkButton>
                                <asp:LinkButton ID="btn_delete_attendance" runat="server" CssClass="button alert" OnClick="btn_delete_attendance_Click" OnClientClick="return confirm('Xóa ngày chấm công của tài khoản này? Tiền sẽ được tính lại.');">
                                    <span class="mif-bin mr-1"></span> Xóa ngày công
                                </asp:LinkButton>
                            </div>
                        </div>
                        <asp:Label ID="lbl_edit_attendance_message" runat="server" CssClass="d-block mt-1"></asp:Label>
                    </asp:Panel>

                    <asp:Panel ID="pn_chitiet_tangca" runat="server" CssClass="attendance-edit-panel p-3 mb-3" Visible="false">
                        <div class="text-bold mb-2">Chi tiết tăng ca theo ngày (Tháng này)</div>
                        <div class="row flex-align-end mb-3">
                            <div class="cell-lg-4 cell-md-5 mb-2">
                                <label class="fw-600">Chọn nhân viên</label>
                                <asp:DropDownList ID="ddl_chitiet_tangca_account" runat="server" CssClass="w-100"></asp:DropDownList>
                            </div>
                            <div class="cell-lg-2 cell-md-3 mb-2">
                                <asp:LinkButton ID="btn_xem_chitiet_tangca" runat="server" CssClass="button info" OnClick="btn_xem_chitiet_tangca_Click">
                                    <span class="mif-search mr-1"></span> Xem
                                </asp:LinkButton>
                            </div>
                        </div>

                        <asp:Panel ID="pn_chitiet_table" runat="server" Visible="false">
                            <div class="text-bold mb-2">Nhân viên: <asp:Label ID="lbl_chitiet_tangca_hoten" runat="server" CssClass="fg-cobalt"></asp:Label></div>
                            <asp:Repeater ID="rpt_chitiet_tangca" runat="server" OnItemDataBound="rpt_chitiet_tangca_ItemDataBound">
                                <HeaderTemplate>
                                    <table class="table table-border cell-border">
                                        <thead>
                                            <tr class="bg-light">
                                                <th>Ngày</th>
                                                <th>Giờ tăng ca</th>
                                                <th>Hệ số</th>
                                                <th>Lương 1 giờ</th>
                                                <th>Thành tiền</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <asp:HiddenField ID="hdf_id_chamcong" runat="server" Value='<%# Eval("id") %>' />
                                            <%# Eval("ngaychamcong", "{0:dd/MM/yyyy}") %>
                                        </td>
                                        <td>
                                            <asp:Label ID="lbl_sogiodu" runat="server" Text='<%# Eval("SoGioDu") %>'></asp:Label>
                                            <asp:TextBox ID="txt_sogiodu" runat="server" Text='<%# Convert.ToDecimal(Eval("SoGioDu")).ToString(System.Globalization.CultureInfo.InvariantCulture) %>' style="width: 80px; padding: 4px; border: 1px solid #ccc; border-radius: 3px;" type="number" step="0.01" Visible="false"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="lbl_heso" runat="server" Text='<%# Eval("HeSoTangCa") %>'></asp:Label>
                                            <asp:TextBox ID="txt_heso" runat="server" Text='<%# Convert.ToDecimal(Eval("HeSoTangCa")).ToString(System.Globalization.CultureInfo.InvariantCulture) %>' style="width: 80px; padding: 4px; border: 1px solid #ccc; border-radius: 3px;" type="number" step="0.1" Visible="false"></asp:TextBox>
                                        </td>
                                        <td><%# Convert.ToDecimal(Eval("Luong1Gio")).ToString("#,##0") %></td>
                                        <td class="text-bold fg-green"><%# Convert.ToDecimal(Eval("ThanhTien")).ToString("#,##0") %></td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                        </tbody>
                                    </table>
                                </FooterTemplate>
                            </asp:Repeater>
                            <div class="mt-2">
                                <asp:LinkButton ID="btn_edit_chitiet_tangca" runat="server" CssClass="button warning" OnClick="btn_edit_chitiet_tangca_Click">
                                    <span class="mif-pencil mr-1"></span> Chỉnh sửa
                                </asp:LinkButton>
                                <asp:LinkButton ID="btn_save_chitiet_tangca" runat="server" CssClass="button success" OnClick="btn_save_chitiet_tangca_Click" Visible="false">
                                    <span class="mif-floppy-disk mr-1"></span> Lưu chi tiết tăng ca
                                </asp:LinkButton>
                            </div>
                        </asp:Panel>
                        <asp:Label ID="lbl_chitiet_tangca_message" runat="server" CssClass="d-block mt-1"></asp:Label>
                    </asp:Panel>
                </div>

                <div style="overflow: auto;" class="mt-3">
                    <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel2">
        <ProgressTemplate>
            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                <div style="padding-top: 45vh;">
                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <div id="attendance-export-loading" class="attendance-export-loading" hidden role="status" aria-live="polite" aria-busy="false">
        <div class="attendance-export-box">
            <div class="attendance-export-spinner" aria-hidden="true"></div>
            <strong>Đang xuất Excel toàn bộ nhân viên</strong>
            <p id="attendance-export-stage" class="mt-2">Đang tổng hợp chấm công và tăng ca. Vui lòng chờ…</p>
            <span id="attendance-export-elapsed" aria-hidden="true">0 giây</span>
        </div>
    </div>
    <div id="attendance-export-result" role="status" aria-live="polite" class="px-3 pb-3"></div>
    <script>
        (function () {
            var exporting = false;
            window.downloadAttendanceExcel = function (button) {
                if (exporting) return false;
                exporting = true;
                var overlay = document.getElementById('attendance-export-loading');
                var status = document.getElementById('attendance-export-result');
                var stage = document.getElementById('attendance-export-stage');
                var elapsed = document.getElementById('attendance-export-elapsed');
                var original = button.innerHTML;
                var started = Date.now();
                overlay.hidden = false;
                overlay.setAttribute('aria-busy', 'true');
                button.setAttribute('aria-disabled', 'true');
                button.innerHTML = 'Đang xuất Excel…';
                status.textContent = '';
                stage.textContent = 'Đang tổng hợp chấm công và tăng ca. Vui lòng chờ…';
                elapsed.textContent = '0 giây';
                var timer = window.setInterval(function () {
                    elapsed.textContent = Math.floor((Date.now() - started) / 1000) + ' giây';
                }, 1000);
                var controller = new AbortController();
                var timedOut = false;
                var timeout = window.setTimeout(function () {
                    timedOut = true;
                    controller.abort();
                }, 660000);
                (async function () {
                    try {
                        var form = button.closest('form');
                        var data = new FormData(form);
                        data.set('__EVENTTARGET', '<%= btn_export_attendance_confirm.UniqueID %>');
                        data.set('__EVENTARGUMENT', '');
                        data.delete('__ASYNCPOST');
                        // Send a normal Web Forms post so the response contains the binary file.
                        data.delete('<%= ScriptManager.GetCurrent(Page).UniqueID %>');
                        var response = await fetch(form.action, {
                            method: 'POST', body: data, credentials: 'same-origin', signal: controller.signal
                        });
                        if (!response.ok || (response.headers.get('Content-Type') || '').indexOf('application/vnd.ms-excel') < 0)
                            throw new Error('Không thể xuất file. Vui lòng kiểm tra phiên đăng nhập và thử lại.');
                        stage.textContent = 'Đang nhận file Excel…';
                        var blob = await response.blob();
                        if (!blob.size) throw new Error('File Excel rỗng. Vui lòng thử lại.');
                        var header = response.headers.get('Content-Disposition') || '';
                        var match = /filename="?([^";]+)"?/i.exec(header);
                        var url = URL.createObjectURL(blob);
                        var link = document.createElement('a');
                        link.href = url;
                        link.download = match ? match[1] : 'ChamCong_TatCa.xls';
                        document.body.appendChild(link);
                        link.click();
                        link.remove();
                        window.setTimeout(function () { URL.revokeObjectURL(url); }, 60000);
                        status.style.color = '#176539';
                        status.textContent = 'Đã tạo file Excel và chuyển cho trình duyệt tải xuống.';
                    } catch (error) {
                        status.style.color = '#b42318';
                        status.textContent = timedOut
                            ? 'Quá thời gian chờ xuất file. Vui lòng thử lại sau.'
                            : (error.message || 'Không tải được file. Vui lòng kiểm tra kết nối và thử lại.');
                    } finally {
                        window.clearInterval(timer);
                        window.clearTimeout(timeout);
                        overlay.hidden = true;
                        overlay.setAttribute('aria-busy', 'false');
                        button.removeAttribute('aria-disabled');
                        button.innerHTML = original;
                        exporting = false;
                    }
                })();
                return false;
            };
        })();
    </script>
</asp:Content>

