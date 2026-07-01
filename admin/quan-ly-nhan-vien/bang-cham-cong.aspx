<%@ Page Title="Bảng chấm công" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="bang-cham-cong.aspx.cs" Inherits="admin_quan_ly_nhan_vien_bang_cham_cong" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .bcorn-fix-title-table {
            font-size: 15px !important;
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

