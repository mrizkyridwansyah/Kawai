SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [sp_Wms_ItemPackingSupplier_Update]
	@SupplierCode varchar(15),
	@ItemCode varchar(25),
	@QtyPacking numeric(18,9),
	@UpdateBy varchar(25)
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

	update ItemSupplierPacking 
	set QtyPacking = @QtyPacking, LastUpdate = getdate(), LastUser = @UpdateBy
	where SupplierCode = @SupplierCode and ItemCode = @ItemCode
end
GO
