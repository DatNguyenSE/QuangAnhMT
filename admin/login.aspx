<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="admin_Default2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <%--Basic --%>
    <title>PHẦN MỀM QUẢN LÝ THÁI AN AUDIO</title>
    <meta charset="UTF-8" />
    <meta http-equiv="content-language" content="vi" />
    <meta name="viewport" content="width=device-width, initial-scale=1, user-scalable=yes" />
    <%--<meta name="robots" content="index, follow" />
    <meta name="revisit-after" content="1 days" />--%>

    <link rel='icon' href='/uploads/images/avt.png' sizes='16x16' type='image/x-icon' />
    <link rel='icon' href='/uploads/images/avt.png' sizes='32x32' type='image/x-icon' />
    <link rel='icon' href='/uploads/images/avt.png' sizes='48x48' type='image/x-icon' />

    <!-- Apple Touch Icon -->
    <link rel='apple-touch-icon' href='/uploads/images/avt.png' sizes='180x180' />
    <link rel='apple-touch-icon' href='/uploads/images/avt.png' sizes='167x167' />
    <link rel='apple-touch-icon' href='/uploads/images/avt.png' sizes='152x152' />
    <link rel='apple-touch-icon' href='/uploads/images/avt.png' sizes='120x120' />

    <!-- Android Icons -->
    <link rel='icon' href='/uploads/images/avt.png' sizes='192x192' />
    <link rel='icon' href='/uploads/images/avt.png' sizes='144x144' />

    <meta property='og:title' content='PHẦN MỀM QUẢN LÝ THÁI AN AUDIO' />
    <meta property='og:image' content='/uploads/images/home-open.jpg' />

    <%--Open Graph Meta Tags --%>
    <%--
<meta property="og:title" content="Mô tả ngắn và hấp dẫn về trang web của bạn" />
<meta property="og:url" content="URL_trang_web_cua_ban" />
<meta property="og:image" content="URL_hinh_anh_1200x630" />
<meta property="og:description" content="Mô tả ngắn và hấp dẫn về nội dung của trang web của bạn." />

<title></title>
<meta name="description" content="Mô tả ngắn và hấp dẫn về nội dung của trang web của bạn." />
<link rel="canonical" href="https://Hotasoft.com" />--%>


    <%--Favicon & icon Mobile & meta--%>
    <asp:Literal ID="literal_fav_icon" runat="server"></asp:Literal>

    <%--css nguồn--%>
    <link href="/Metro-UI-CSS-master/css/metro-all.min.css" rel="stylesheet" />
    <%--dành riêng cho trang login--%>
    <link href="/css/login.css?v=1.7" rel="stylesheet" />
    <%--viết thêm dựa vào metro--%>
    <link href="/css/bcorn-with-metro.css" rel="stylesheet" />
    <%--sửa lại css metro theo ý mình--%>
    <link href="/css/fix-metro.css?v=1.7" rel="stylesheet" />
    <!-- jquery nên để trước các js khác -->
    <%--<script src="/js/jquery-3.7.1.min.js"></script>--%>
</head>
<body class="body-login-bcorn">

    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True"></asp:ScriptManager>
        <div class="bg-login-bcorn">

            <div class="fg-white text-center" style="padding-top: 50px">
                <div class="text-bold" style="font-size: 17px">
                    LOGIN TO QUANGANHMT.COM
                </div>
                <%--<div>
                    Bạn chưa có tài khoản? 
                   <a href="tel:0842359155" class="fg-white fg-lightOrange" title="Nhấn để gọi"><span class="mif-phone pl-1"></span>Trợ giúp</a>
                </div>--%>
            </div>

            <div style="margin: 0 auto; max-width: 360px; z-index: 0;" class="pl-4 pr-4 pl-0-md pr-0-md">
                <div>
                    <div class="text-center" style="padding-top: 30px; padding-bottom: 40px;">
                        <img src="/uploads/images/logo.png" width="160" />
                    </div>

                    <div>
                        <asp:Panel ID="Panel1" runat="server" DefaultButton="but_login">
                            <div class="mt-0">
                                <asp:TextBox MaxLength="50" ID="txt_user" runat="server" data-role="input" data-prepend="<span class='mif-user'>" placeholder="Tài khoản" onkeypress="if (event.keyCode==13) return false;"></asp:TextBox><%--autocomplete="off" --%>
                            </div>
                            <div class="mt-5">
                                <asp:TextBox MaxLength="50" TextMode="Password" ID="txt_pass" runat="server" data-role="input" data-prepend="<span class='mif-key'>" placeholder="Mật khẩu"></asp:TextBox>
                            </div>
                            <div class="mt-5">
                                <%--<div style="float: left">
                                    <small><a href="/admin/quen-mat-khau/default.aspx" class="app-bar-item fg-white fg-lightOrange-hover">Quên mật khẩu?</a></small>
                                </div>--%>
                                <div style="float: right; width: 100%">
                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:Button ID="but_login" runat="server" Text="ĐĂNG NHẬP" CssClass="button primary " OnClick="but_login_Click" Width="100%" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                                        <ProgressTemplate>
                                            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                                                <div style="padding-top: 50vh;">
                                                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                                                </div>
                                            </div>
                                        </ProgressTemplate>
                                    </asp:UpdateProgress>
                                </div>
                                <div style="clear: both"></div>
                            </div>
                        </asp:Panel>
                    </div>
                </div>
                <%-- <div class="mb-20 pb-10"></div>--%>
                <%--<div class="border bd-default border-left-none border-right-none border-bottom-none mb-20 mt-10 text-center fg-white">
                <small class="mt-4">Hỗ trợ kỹ thuật: 0842 359 155</small>
            </div>--%>
            </div>
        </div>
    </form>
    <script src="/Metro-UI-CSS-master/js/metro.min.js"></script>
    <%=ViewState["thongbao"] %>
</body>
</html>
