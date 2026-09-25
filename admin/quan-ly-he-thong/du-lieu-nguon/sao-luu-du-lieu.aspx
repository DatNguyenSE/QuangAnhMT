<%@ Page Title="Sao lưu dữ liệu" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="sao-luu-du-lieu.aspx.cs" Inherits="admin_sao_luu_du_lieu" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .backup-box { max-width: 720px; border: 1px solid #e5e5e5; padding: 24px; background: #fff; }
        .backup-box p { line-height: 1.65; }
        .backup-box .button { height: auto; min-height: 40px; white-space: normal; }
        @media (max-width: 576px) { .backup-box { padding: 18px; } }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="server">
    <div class="p-3">
        <div class="backup-box">
            <h5 class="text-upper text-bold mt-0">Sao lưu dữ liệu</h5>
            <hr />
            <p>Tải bản sao website gồm mã nguồn, hình ảnh, tài liệu và file database</p>

            <asp:Label ID="lb_error" runat="server" CssClass="fg-red d-block mb-3" role="alert" />
            <asp:HiddenField ID="download_token" runat="server" />
            <asp:LinkButton OnClientClick="if (!startBackup(this)) return false;" ID="but_backup" runat="server" CssClass="button success" OnClick="DownloadBackup">
                <span class="mif-download mr-2"></span>Sao lưu dữ liệu
            </asp:LinkButton>
            <p class="fg-gray mb-0"><small>File ZIP được đặt tên theo ngày hiện tại. Việc chuẩn bị có thể mất vài phút, vui lòng chờ tải xuống.</small></p>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="server">
    <script>
        var backupBusy = false;
        function startBackup(button) {
            if (backupBusy) return false;
            backupBusy = true;
            var token = Date.now().toString() + Math.random().toString().slice(2);
            document.getElementById('<%=download_token.ClientID%>').value = token;
            var error = document.getElementById('<%=lb_error.ClientID%>');
            error.textContent = '';
            button.setAttribute('aria-disabled', 'true');
            button.setAttribute('aria-busy', 'true');
            var activity = Metro.activity.open({ type: 'cycle', overlayClickClose: false });
            var started = Date.now();
            var timer = window.setInterval(function () {
                var ready = document.cookie.split(';').some(function (cookie) {
                    return cookie.trim() === 'website_backup_ready=' + token;
                });
                if (!ready && Date.now() - started < 3660000) return;
                window.clearInterval(timer);
                Metro.activity.close(activity);
                backupBusy = false;
                button.removeAttribute('aria-disabled');
                button.removeAttribute('aria-busy');
                if (ready) {
                    document.cookie = 'website_backup_ready=; Max-Age=0; path=' + window.location.pathname;
                } else {
                    error.textContent = 'Chưa nhận được phản hồi tải xuống. Vui lòng kiểm tra kết nối và tải lại trang.';
                }
            }, 500);
            return true;
        }
    </script>
</asp:Content>
