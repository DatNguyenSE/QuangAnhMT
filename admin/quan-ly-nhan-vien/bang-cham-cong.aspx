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
                    </div>

                    <asp:Panel ID="pn_export_attendance" runat="server" CssClass="attendance-edit-panel p-3 mb-3" Visible="false">
                        <div class="text-bold mb-2">Chọn nhân viên cần xuất file</div>
                        <div class="d-flex flex-align-end">
                            <div style="min-width: 280px; max-width: 420px; width: 100%;" class="mr-2">
                                <label class="fw-600">Nhân viên</label>
                                <asp:DropDownList ID="ddl_export_attendance_account" runat="server" CssClass="w-100"></asp:DropDownList>
                            </div>
                            <asp:LinkButton ID="btn_export_attendance_confirm" runat="server" CssClass="button info" OnClick="btn_export_attendance_confirm_Click">
                                <span class="mif-download mr-1"></span> Tải file Excel
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
</asp:Content>

