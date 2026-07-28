<%@ Page Language="C#" AutoEventWireup="true" CodeFile="chi-tiet-cong-no.aspx.cs" Inherits="chi_tiet_cong_no" %>

<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />
    <title>Chi tiết công nợ khách hàng - Quang Anh Audio</title>
    <link rel="stylesheet" href="https://cdn.metroui.org.ua/v4/css/metro-all.min.css" />
    <link rel='icon' href='/uploads/images/logo.png' sizes='32x32' type='image/png'>
    <style>
        body { background-color: #f0f2f5; font-family: -apple-system, system-ui, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif; }
        .container { max-width: 700px; margin: 30px auto; padding: 0 15px; }
        .card { background: #fff; box-shadow: 0 4px 12px rgba(0,0,0,0.08); border-radius: 8px; overflow: hidden; }
        .card-header { background: #ffffff; padding: 20px; text-align: center; border-bottom: 1px solid #eaeaea; }
        .card-header img.logo { max-height: 70px; margin-bottom: 10px; }
        .card-header h3 { margin: 0; font-size: 24px; color: #333; font-weight: 600; text-transform: uppercase; }
        .card-header p { margin: 5px 0 0; color: #666; font-size: 14px; }
        
        .card-body { padding: 25px; }
        
        .customer-info { background: #f8f9fa; border: 1px solid #e9ecef; border-radius: 6px; padding: 15px; margin-bottom: 25px; display: flex; flex-direction: column; gap: 8px; }
        .customer-info-row { display: flex; justify-content: space-between; align-items: center; border-bottom: 1px dashed #ddd; padding-bottom: 8px; }
        .customer-info-row:last-child { border-bottom: none; padding-bottom: 0; }
        .info-label { color: #555; }
        .info-value { font-weight: 600; color: #222; text-align: right; }
        .total-debt { font-size: 1.25rem; color: #d32f2f; font-weight: 700; }
        
        .section-title { font-size: 16px; font-weight: 600; color: #1976d2; margin-bottom: 15px; display: flex; align-items: center; gap: 8px; padding-bottom: 5px; border-bottom: 2px solid #1976d2; display: inline-block; }
        
        .table-wrap { overflow-x: auto; margin-bottom: 30px; border-radius: 6px; border: 1px solid #eee; }
        table { margin: 0 !important; width: 100%; border-collapse: collapse; }
        table th { background-color: #f8f9fa !important; color: #444; font-weight: 600; text-transform: uppercase; font-size: 12px; padding: 12px 10px !important; border-bottom: 2px solid #ddd !important; }
        table td { padding: 12px 10px !important; color: #333; vertical-align: middle; border-bottom: 1px solid #eee; }
        table tr:last-child td { border-bottom: none; }
        
        .text-bold { font-weight: bold; }
        .text-danger { color: #d32f2f; }
        
        .footer { text-align: center; padding: 20px; background: #fafafa; color: #777; font-size: 13px; border-top: 1px solid #eaeaea; }
        
        @media (max-width: 576px) {
            .container { margin: 15px auto; }
            .card-body { padding: 15px; }
            .info-box { padding: 12px; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="card">
                <div class="card-header">
                    <img src="/uploads/images/logo.png" alt="Quang Anh Audio Logo" class="logo" onerror="this.style.display='none';" />
                    <h3>BÁO CÁO CÔNG NỢ</h3>
                    <p>Kính gửi Quý khách hàng, dưới đây là chi tiết công nợ hiện tại của Quý khách.</p>
                </div>
                
                <div class="card-body">
                    <asp:Panel ID="pn_error" runat="server" Visible="false">
                        <div class="remark alert">
                            <asp:Label ID="lb_error" runat="server"></asp:Label>
                        </div>
                    </asp:Panel>

                    <asp:Panel ID="pn_content" runat="server">
                        <div class="customer-info">
                            <div class="customer-info-row">
                                <span class="info-label">Khách hàng:</span>
                                <span class="info-value"><asp:Literal ID="ltr_ten" runat="server"></asp:Literal></span>
                            </div>
                            <div class="customer-info-row">
                                <span class="info-label">Số điện thoại:</span>
                                <span class="info-value"><asp:Literal ID="ltr_sdt" runat="server"></asp:Literal></span>
                            </div>
                            <div class="customer-info-row mt-2">
                                <span class="info-label text-bold">TỔNG CÔNG NỢ:</span>
                                <span class="info-value total-debt"><asp:Literal ID="ltr_tongno" runat="server"></asp:Literal></span>
                            </div>
                        </div>

                        <div class="section-title"><span class="mif-shopping-basket"></span> Công nợ Mua Hàng</div>
                        <div class="table-wrap">
                            <asp:GridView ID="grv_banhang" runat="server" AutoGenerateColumns="false" CssClass="table striped hovered" GridLines="None" ShowHeaderWhenEmpty="true">
                                <Columns>
                                    <asp:BoundField DataField="Ngay" HeaderText="Ngày" DataFormatString="{0:dd/MM/yyyy}" ItemStyle-Width="25%" />
                                    <asp:BoundField DataField="MaDon" HeaderText="Mã đơn" ItemStyle-Width="20%" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                                    <asp:BoundField DataField="TongTienText" HeaderText="Tổng tiền" ItemStyle-CssClass="text-right" HeaderStyle-CssClass="text-right" />
                                    <asp:BoundField DataField="CongNoText" HeaderText="Còn nợ" ItemStyle-CssClass="text-right text-bold text-danger" HeaderStyle-CssClass="text-right" />
                                </Columns>
                                <EmptyDataTemplate>
                                    <div class="p-4 text-center text-muted">Không có công nợ mua hàng.</div>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </div>

                        <div class="section-title"><span class="mif-tools"></span> Công nợ Dịch Vụ / Bảo Hành</div>
                        <div class="table-wrap mb-0">
                            <asp:GridView ID="grv_baohanh" runat="server" AutoGenerateColumns="false" CssClass="table striped hovered" GridLines="None" ShowHeaderWhenEmpty="true">
                                <Columns>
                                    <asp:BoundField DataField="Ngay" HeaderText="Ngày" DataFormatString="{0:dd/MM/yyyy}" ItemStyle-Width="25%" />
                                    <asp:BoundField DataField="MaDon" HeaderText="Mã phiếu" ItemStyle-Width="20%" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                                    <asp:BoundField DataField="TongTienText" HeaderText="Tổng tiền" ItemStyle-CssClass="text-right" HeaderStyle-CssClass="text-right" />
                                    <asp:BoundField DataField="CongNoText" HeaderText="Còn nợ" ItemStyle-CssClass="text-right text-bold text-danger" HeaderStyle-CssClass="text-right" />
                                </Columns>
                                <EmptyDataTemplate>
                                    <div class="p-4 text-center text-muted">Không có công nợ bảo hành.</div>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </div>
                    </asp:Panel>
                </div>
                
                <div class="footer">
                    Mọi thắc mắc về công nợ, Quý khách vui lòng liên hệ trực tiếp với cửa hàng để được hỗ trợ.<br/>
                    <strong>Trân trọng cảm ơn Quý khách!</strong>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
