<%@ Page Title="Cài đặt hệ thống" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="cai-dat.aspx.cs" Inherits="admin_quan_ly_he_thong_cai_dat" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="p-3">
                <div class="p-3 bg-white">
                    <asp:Panel ID="Panel1" runat="server" DefaultButton="but_update">
                        <div class="row">
                            <div class="cell-lg-5">
                                <div class="mt-3">
                                    <label class="fw-600">Vĩ độ công ty</label>
                                    <asp:TextBox ID="txt_vido" runat="server" data-role="input"></asp:TextBox>
                                </div>
                                <div class="mt-3">
                                    <label class="fw-600">Kinh độ công ty</label>
                                    <asp:TextBox ID="txt_kinhdo" runat="server" data-role="input"></asp:TextBox>
                                </div>
                                <div class="mt-5 mb-5 text-right">
                                    <asp:Button ID="but_update" runat="server" CssClass="success small" Text="CẬP NHẬT" OnClick="but_update_Click" />
                                </div>
                            </div>
                        </div>
                    </asp:Panel>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress5" runat="server" AssociatedUpdatePanelID="up_main">
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

