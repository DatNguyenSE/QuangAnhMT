using System;
using System.IO;
using System.Web;
using System.Web.UI;

public partial class admin_sao_luu_du_lieu : Page
{
    private bool downloading;

    protected override void OnInit(EventArgs e)
    {
        ViewStateUserKey = Session.SessionID;
        base.OnInit(e);
        check_login_cl.check_login_admin("6", "6");
    }

    protected void DownloadBackup(object sender, EventArgs e)
    {
        check_login_cl.check_login_admin("6", "6");
        Server.ScriptTimeout = 3600;
        // A unique temporary file is removed even if compression or download fails.
        string temporary = Path.Combine(Path.GetTempPath(), "website-backup-" + Guid.NewGuid().ToString("N") + ".zip");
        try
        {
            using (FileStream archive = new FileStream(temporary, FileMode.CreateNew, FileAccess.ReadWrite,
                FileShare.None, 65536, FileOptions.DeleteOnClose))
            {
                WebsiteBackup.Create(Server.MapPath("~/"), archive);
                archive.Position = 0;
                string name = "backup_" + DateTime.UtcNow.AddHours(7).ToString("dd-MM-yyyy") + ".zip";
                Response.Clear();
                Response.BufferOutput = false;
                Response.ContentType = "application/zip";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();
                Response.AddHeader("Content-Disposition", "attachment; filename=\"" + name + "\"");
                Response.AddHeader("Content-Length", archive.Length.ToString(System.Globalization.CultureInfo.InvariantCulture));
                string token = download_token.Value;
                if (System.Text.RegularExpressions.Regex.IsMatch(token ?? "", @"\A[0-9]{1,64}\z"))
                {
                    // The browser sees this cookie when download response headers arrive.
                    Response.Cookies.Add(new HttpCookie("website_backup_ready", token)
                    {
                        HttpOnly = false,
                        Secure = Request.IsSecureConnection,
                        Path = Request.Url.AbsolutePath,
                        Expires = DateTime.UtcNow.AddHours(2)
                    });
                }
                downloading = true;
                archive.CopyTo(Response.OutputStream);
                Response.Flush();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.TraceError("Website backup failed: {0}", ex);
            if (!downloading)
                lb_error.Text = "Không thể tạo bản sao lưu. Vui lòng kiểm tra file database trong App_Data/DatabaseBackup và quyền đọc file, ghi thư mục tạm trên hosting.";
        }
        finally
        {
            if (downloading) Context.ApplicationInstance.CompleteRequest();
        }
    }

    protected override void Render(HtmlTextWriter writer)
    {
        if (!downloading) base.Render(writer);
    }
}
