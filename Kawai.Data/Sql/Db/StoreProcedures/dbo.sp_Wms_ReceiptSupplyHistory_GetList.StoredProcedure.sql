SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_ReceiptSupplyHistory_GetList]
--DECLARE
	@WarehouseCode varchar(25) = 'WH-001',
	@AreaCode varchar(25) = 'TMP',
	@ItemCode varchar(25) = '888997',
	@LotNo varchar(100) = 'ALL',
	@Period date = null
as
begin
	
	if @Period is null
	begin
		set @Period = cast(format(getdate(), 'yyyy-MM') + '-01' as date)
	end
	
	select 
		res.*,
		isnull(sm.TMPreMonth,0) PreMonth,
		case when res.TransactionType = 'IN' then isnull(res.QtyTrans,0) else 0 end Receipt,
		case when res.TransactionType = 'OUT' then isnull(res.QtyTrans,0) else 0 end Supply,
		isnull(sm.TMLossReject,0) Reject,
		isnull(sm.TMCurrent,0) [Current],	
		mi.Item_Name ItemName, 
		case when res.TransactionType = 'OUT' THEN isnull(ma.AreaName, 'Temporary') else '' end FromAreaName, 
		case when res.TransactionType = 'IN' THEN isnull(ma.AreaName, 'Temporary') else '' end ToAreaName, 
		us.FullName LastUser 
	From 
	(
		select Status TransactionType, ProcessMenu, WarehouseCode, AreaCode, ItemCode, LotNo, Remarks, ReferenceNo [DocReference], format(LogDate, 'yyyy-MM-dd HH:mm') TransactionDate, UserID, SUM(QtyTrans) QtyTrans from ReceiptSupplyHistory
		where 1=1
		AND MONTH(LogDate) = MONTH(@Period) AND YEAR(LogDate) = YEAR(@Period)
		AND WarehouseCode <> isnull(RefWarehouseCode, '')
		AND AreaCode <> isnull(RefAreaCode, '')
		AND (@LotNo = 'ALL' or LotNo = @LotNo)
		group by Status, ProcessMenu, WarehouseCode, AreaCode, ItemCode, LotNo, Remarks, ReferenceNo, format(LogDate, 'yyyy-MM-dd HH:mm'), UserID
	) res
	LEFT JOIN 
	(
		SELECT * fROM StockMaster WHERE WarehouseCode = @WarehouseCode and ItemCode = @ItemCode
	) sm on res.AreaCode = sm.AreaCode and res.LotNo = sm.LotNo
	LEFT JOIN Item_Master mi on res.ItemCode = mi.Item_Code
	LEFT JOIN WareHouse_Master mw on res.WarehouseCode = mw.WH_Code
	LEFT JOIN MS_Area ma on res.AreaCode = ma.AreaCode
	LEFT join SS_UserSetup us on res.UserID = us.UserID
end
GO
