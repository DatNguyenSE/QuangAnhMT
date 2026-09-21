SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRY
 BEGIN TRANSACTION;
 SELECT TOP (44) id,ten,soluong_hientai,gianhap,ngaytao,nguoitao
 INTO #SelectedProducts
 FROM [qua93172_quanganh_db].[let99665_thaianaudio].[KhoSanPham_tb] WITH (UPDLOCK,HOLDLOCK)
 ORDER BY ngaytao DESC,id DESC;
 IF (SELECT COUNT(*) FROM #SelectedProducts) <> 44
  THROW 50001, 'Expected 44 selected products.', 1;
 DECLARE @Inserted TABLE (history_id bigint,product_id nvarchar(10),quantity int);
 INSERT INTO [qua93172_quanganh_db].[let99665_thaianaudio].[NhapXuatKho_tb]
 (id_sanpham,ten_sanpham,soluong_nhap,gia_nhap,ngaynhap,nguoinhap,ton_hientai,nhap_hay_xuat)
 OUTPUT inserted.id,inserted.id_sanpham,inserted.soluong_nhap INTO @Inserted
 SELECT CONVERT(nvarchar(10),p.id),p.ten,p.soluong_hientai,p.gianhap,p.ngaytao,p.nguoitao,0,1
 FROM #SelectedProducts p
 WHERE p.soluong_hientai>0
 AND NOT EXISTS (
  SELECT 1 FROM [qua93172_quanganh_db].[let99665_thaianaudio].[NhapXuatKho_tb] n WITH (UPDLOCK,HOLDLOCK)
  WHERE n.id_sanpham=CONVERT(nvarchar(10),p.id) AND n.nhap_hay_xuat=1
 );
 COMMIT TRANSACTION;
 SELECT COUNT(*) inserted_count,SUM(quantity) total_quantity FROM @Inserted;
 SELECT history_id,RTRIM(product_id) product_id,quantity FROM @Inserted ORDER BY history_id;
 DROP TABLE #SelectedProducts;
END TRY
BEGIN CATCH
 IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
 THROW;
END CATCH;