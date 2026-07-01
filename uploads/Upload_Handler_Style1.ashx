<%@ WebHandler Language="C#" Class="Upload_Handler_Style1" %>
using System;
using System.Web;
using System.IO;
using System.Drawing;

public class Upload_Handler_Style1 : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        string logPath = context.Server.MapPath("~/logs/upload_error.log");
        string tempDir = context.Server.MapPath("~/uploads/img-temp/");
        string outputDir = context.Server.MapPath("~/uploads/img-handler/");

        try
        {
            if (context.Request.Files.Count > 0)
            {
                HttpPostedFile file = context.Request.Files[0];

                // Kiểm tra loại tệp
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".heic" };
                string fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!Array.Exists(allowedExtensions, ext => ext == fileExtension))
                {
                    context.Response.StatusCode = 400;
                    context.Response.Write("Định dạng ảnh không hợp lệ.");
                    return;
                }

                // Kiểm tra kích thước tệp
                int maxFileSize = 10 * 1024 * 1024; // 10 MB
                if (file.ContentLength > maxFileSize)
                {
                    context.Response.StatusCode = 400;
                    context.Response.Write("Vui lòng chọn file có kích thước nhỏ hơn 10 MB.");
                    return;
                }

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(tempDir)) Directory.CreateDirectory(tempDir);
                if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);

                string tempFileName = Guid.NewGuid() + fileExtension;
                string tempFilePath = Path.Combine(tempDir, tempFileName);

                string newFileName = Guid.NewGuid() + fileExtension;
                string newFilePath = Path.Combine(outputDir, newFileName);

                // Lưu file tạm
                file.SaveAs(tempFilePath);

                // Xử lý ảnh
                using (Image image = Image.FromFile(tempFilePath))
                {
                    if (image.Width >= 600)
                    {
                        xulyanh_cl.ProcessAndSaveImage(tempFilePath, newFilePath, 600, 100);
                    }
                    else
                    {
                        // Sao chép nếu không cần xử lý
                        File.Copy(tempFilePath, newFilePath);
                    }
                }

                // Xóa file tạm
                File.Delete(tempFilePath);

                // Trả về đường dẫn file mới
                context.Response.StatusCode = 200;
                context.Response.Write("/uploads/img-handler/" + newFileName);
            }
            else
            {
                context.Response.StatusCode = 400;
                context.Response.Write("Vui lòng chọn file.");
            }
        }
        catch (Exception ex)
        {
            // Ghi log lỗi
            File.AppendAllText(logPath, DateTime.Now + " - " + ex.Message + Environment.NewLine);

            context.Response.StatusCode = 500;
            context.Response.Write("Lỗi server: " + ex.Message);
        }
    }

    public bool IsReusable
    {
        get { return false; }
    }
}

