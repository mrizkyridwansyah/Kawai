SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [sp_Wms_ItemPackingSupplier_Create]
	@SupplierCode varchar(15),
	@ItemCode varchar(25),
	@QtyPacking numeric(18,9),
	@RegisterBy varchar(25)
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

	if exists (select 1 from ItemSupplierPacking where SupplierCode = @SupplierCode and ItemCode = @ItemCode)
	begin
		raiserror('Data Packing Item already exists!',16,1)
		return
	end

	declare @UnitCls varchar(3) = (select Unit_Cls from Item_Master where Item_Code = @ItemCode)

	insert into ItemSupplierPacking (SupplierCode, ItemCode, QtyPacking, UnitCls, RegisterDate, RegisterUser)
	values (@SupplierCode, @ItemCode, @QtyPacking, @UnitCls, getdate(), @RegisterBy)
end
GO
