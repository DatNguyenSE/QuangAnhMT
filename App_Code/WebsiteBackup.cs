using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

public static class WebsiteBackup
{
    public static bool Include(string relative)
    {
        string path = relative.Replace('\\', '/');
        string lower = path.ToLowerInvariant();
        string[] parts = lower.Split('/');
        foreach (string part in parts)
            if (part == ".git" || part == ".vs" || part == ".codex" || part == ".agents" ||
                part == "packages" || part == "node_modules" || part == "obj") return false;
        if (lower.StartsWith("app_data/publishprofiles/") ||
            lower.StartsWith("uploads/file-temp/") || lower.StartsWith("uploads/img-temp/")) return false;
        if (lower.StartsWith("app_data/databasebackup/")) return true;
        string ext = Path.GetExtension(lower);
        if (ext == ".zip" || ext == ".7z" || ext == ".rar" || ext == ".bak" ||
            ext == ".tmp" || ext == ".log" || ext == ".user" || ext == ".refresh" ||
            ext == ".pdb" || ext == ".py" || ext == ".ps1" || ext == ".sln" ||
            ext == ".slnx" || ext == ".publishproj") return false;
        if (parts.Length == 1 && (lower.StartsWith("checktrigger_") || lower.StartsWith("runsql") ||
            lower == "checkdb.cs" || lower.StartsWith("refactor") || lower == ".gitignore")) return false;
        return true;
    }

    public static void Create(string root, Stream output)
    {
        root = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        string database = Path.Combine(root, "App_Data", "DatabaseBackup");
        bool hasDatabase = false;
        if (Directory.Exists(database))
            foreach (string file in Directory.GetFiles(database))
            {
                string ext = Path.GetExtension(file).ToLowerInvariant();
                if ((ext == ".zip" || ext == ".bak" || ext == ".sql") && new FileInfo(file).Length > 0)
                    hasDatabase = true;
            }
        if (!hasDatabase) throw new InvalidOperationException("Chưa có file database trong App_Data/DatabaseBackup.");
        using (ZipOutputStream zip = new ZipOutputStream(output))
        {
            zip.IsStreamOwner = false;
            zip.SetLevel(6);
            zip.UseZip64 = UseZip64.Dynamic;
            AddDirectory(root, root, zip);
            zip.Finish();
        }
    }

    private static void AddDirectory(string root, string directory, ZipOutputStream zip)
    {
        foreach (string file in Directory.GetFiles(directory))
        {
            FileInfo info = new FileInfo(file);
            string relative = file.Substring(root.Length).Replace('\\', '/');
            if (!Include(relative) || (info.Attributes & FileAttributes.ReparsePoint) != 0) continue;
            ZipEntry entry = new ZipEntry(relative);
            entry.DateTime = info.LastWriteTime;
            entry.IsUnicodeText = true;
            zip.PutNextEntry(entry);
            using (FileStream input = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                input.CopyTo(zip);
            zip.CloseEntry();
        }
        foreach (string child in Directory.GetDirectories(directory))
        {
            if ((File.GetAttributes(child) & FileAttributes.ReparsePoint) != 0 ||
                !Include(child.Substring(root.Length) + "/")) continue;
            AddDirectory(root, child, zip);
        }
    }
}
