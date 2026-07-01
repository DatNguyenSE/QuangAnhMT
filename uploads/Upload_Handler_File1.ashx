<%@ WebHandler Language="C#" Class="Upload_Handler_File1" %>
using System;
using System.Web;
using System.IO;
using System.Drawing;

public class Upload_Handler_File1 : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        string logPath = context.Server.MapPath("~/logs/upload_error.log");
        string tempDir = context.Server.MapPath("~/uploads/file-temp/");
        string outputDir = context.Server.MapPath("~/uploads/file-handler/");

        try
        {
            if (context.Request.Files.Count > 0)
            {
                HttpPostedFile file = context.Request.Files[0];

                // Kiểm tra loại tệp
                string[] allowedExtensions = { ".docx", ".doc", ".pdf",".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".heic"  };
                string fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!Array.Exists(allowedExtensions, ext => ext == fileExtension))
                {
                    context.Response.StatusCode = 400;
                    context.Response.Write("Định dạng file không hợp lệ.");
                    return;
                }

                // Kiểm tra kích thước tệp
                int maxFileSize = 100 * 1024 * 1024; // 100 MB
                if (file.ContentLength > maxFileSize)
                {
                    context.Response.StatusCode = 400;
                    context.Response.Write("Vui lòng chọn file có kích thước nhỏ hơn 100 MB.");
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

                // Xử lý file (ở đây chỉ sao chép file từ thư mục tạm sang thư mục đích)
                File.Move(tempFilePath, newFilePath);

                // Trả về đường dẫn file mới
                context.Response.StatusCode = 200;
                context.Response.Write("/uploads/file-handler/" + newFileName);
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