
create   procedure [dbo].[sp_Wms_ReceiptUnschedule_CheckIsDetailsUpdate]
	@Id				bigint,
	@SupplierCode	varchar(25),
	@Details		tvp_ReceiptUnscheduleDetail20260519 READONLY
as
begin
	if not exists (SELECT 1 fROM PartReceiptDetailBarcode WHERE ReceiptId = @Id)
	begin
		SELECT cast(0 as bit) IsUpdateDetails, 0 TypeConfirmation, 'Not Yet Print Label' TypeConfirmationDesc
		return
	end

	if exists 
	(
		select * From @Details dtl 
		FULL OUTER JOIN 
		(
			SELECT * fROM PartReceiptDetail WHERE ReceiptId = @Id
		) rdtl
		on dtl.ItemCode = rdtl.ItemCode and dtl.ReceiptQty = rdtl.ReceiptQty
		where dtl.ItemCode is null or rdtl.ItemCode is null
	) 
	begin
		SELECT cast(1 as bit) IsUpdateDetails, 1 TypeConfirmation, 'Detil Receipt' TypeConfirmationDesc
		return 
	end

	if exists 
	(
		select * From @Details dtl 
		INNER JOIN 
		(
			SELECT ItemCode, QtyPacking fROM PartReceiptDetail WHERE ReceiptId = @Id
		) rdtl on dtl.ItemCode = rdtl.ItemCode
		INNER JOIN Item_Master mi on dtl.ItemCode = mi.Item_Code
		LEFT JOIN 
		(
			select ItemCode, QtyPacking From ItemSupplierPacking where SupplierCode = @SupplierCode
		) mis on dtl.ItemCode = mis.ItemCode
		where isnull(mis.QtyPacking, mi.Number_Box) <> rdtl.QtyPacking
	) 
	begin
		SELECT cast(1 as bit) IsUpdateDetails, 2 TypeConfirmation, 'Qty Packing' TypeConfirmationDesc
		return 
	end

	SELECT cast(0 as bit) IsUpdateDetails, 0 TypeConfirmation, 'No Detail Changes' TypeConfirmationDesc
end
