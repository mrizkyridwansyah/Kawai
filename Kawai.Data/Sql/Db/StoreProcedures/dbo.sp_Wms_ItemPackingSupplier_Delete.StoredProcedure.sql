SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create   procedure [sp_Wms_ItemPackingSupplier_Delete]
	@SupplierCode varchar(15),
	@ItemCode varchar(25)
as
begin
	if not exists (select 1 from Trade_Master where Trade_Code = @SupplierCode)
	begin
		raiserror('Data Supplier didn''t exists!',16,1)
		return
	end
	
	if not exists (select 1 from item_master where Item_Code = @ItemCode)
	begin
		raiserror('Data Item didn''t exists!',16,1)
		return
	end

	if not exists (select 1 from ItemSupplierPacking where SupplierCode = @SupplierCode and ItemCode = @ItemCode)
	begin
		raiserror('Data Packing Item didn''t exists!',16,1)
		return
	end

	delete from ItemSupplierPacking where SupplierCode = @SupplierCode and ItemCode = @ItemCode
end
GO
