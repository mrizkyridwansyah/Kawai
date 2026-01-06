SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create   procedure [sp_Wms_ItemPackingSupplier_Capture]
	@SupplierCode varchar(15),
	@ItemCode varchar(25)
as
begin
	select * from ItemSupplierPacking where SupplierCode = @SupplierCode and ItemCode = @ItemCode
end
GO
