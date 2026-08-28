$connString = "Data Source=112.78.2.146;Initial Catalog=qua93172_quanganh_db;User ID=qua93172_quanganh_db;Password=2Zftv32_PVif~vpz;Encrypt=False"
$conn = New-Object System.Data.SqlClient.SqlConnection($connString)
$conn.Open()
$cmd = $conn.CreateCommand()
$backupName = "[app56734_test_gpt].[taikhoan_tb_backup_" + (Get-Date).ToString("yyyyMMdd_HHmmss") + "]"
$cmd.CommandText = "SELECT * INTO " + $backupName + " FROM [app56734_test_gpt].[taikhoan_tb];"
$cmd.ExecuteNonQuery()
$cmd.CommandText = "IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[app56734_test_gpt].[taikhoan_tb]') AND name = 'chucdanh') BEGIN ALTER TABLE [app56734_test_gpt].[taikhoan_tb] ADD chucdanh nvarchar(200) NULL END;"
$cmd.ExecuteNonQuery()
$cmd.CommandText = "IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[app56734_test_gpt].[taikhoan_tb]') AND name = 'PhuCap_RnD') BEGIN ALTER TABLE [app56734_test_gpt].[taikhoan_tb] ADD PhuCap_RnD decimal(18,0) NULL END;"
$cmd.ExecuteNonQuery()
$cmd.CommandText = "IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[app56734_test_gpt].[taikhoan_tb]') AND name = 'PhuCap_TrucHotline') BEGIN ALTER TABLE [app56734_test_gpt].[taikhoan_tb] ADD PhuCap_TrucHotline decimal(18,0) NULL END;"
$cmd.ExecuteNonQuery()
$cmd.CommandText = "IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[app56734_test_gpt].[taikhoan_tb]') AND name = 'Thuong_DuAn_Max') BEGIN ALTER TABLE [app56734_test_gpt].[taikhoan_tb] ADD Thuong_DuAn_Max decimal(18,0) NULL END;"
$cmd.ExecuteNonQuery()
$cmd.CommandText = "IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[app56734_test_gpt].[taikhoan_tb]') AND name = 'LuongDongBH') BEGIN ALTER TABLE [app56734_test_gpt].[taikhoan_tb] ADD LuongDongBH decimal(18,0) NULL END;"
$cmd.ExecuteNonQuery()
$cmd.CommandText = "IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[app56734_test_gpt].[taikhoan_tb]') AND name = 'NganSach_Max') BEGIN ALTER TABLE [app56734_test_gpt].[taikhoan_tb] ADD NganSach_Max decimal(18,0) NULL END;"
$cmd.ExecuteNonQuery()
Write-Host "Backup and schema update successful. Backup table: $backupName"
