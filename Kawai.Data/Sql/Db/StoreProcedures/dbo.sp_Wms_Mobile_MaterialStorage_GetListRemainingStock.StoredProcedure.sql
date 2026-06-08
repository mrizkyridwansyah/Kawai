CREATE   procedure [dbo].[sp_Wms_Mobile_MaterialStorage_GetListRemainingStock]
	@UserId varchar(25)
as
begin
	SELECT 
		sd.RefNo, sd.WarehouseCode, sd.BarcodeNo, sd.ItemCode, mi.Item_Name ItemName, sd.LotNo, sd.SublotNo, sd.Qty
	FROM StockDetail sd
	inner join Item_Master mi on sd.ItemCode = mi.Item_Code
	inner join 
	(
		select GroupingClassPartCode from SS_UserGroupingClassPartPrivilege 
		where UserID = @UserId and AllowAccess = 1
	) gr on isnull(mi.Grouping_Class_Part_Code, 'OT') = gr.GroupingClassPartCode
	WHERE AreaCode = 'TMP' 
	and AddressCode = 'TMP' 
	and Qty > 0		
end