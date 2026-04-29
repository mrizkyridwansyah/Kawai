CREATE   procedure [dbo].[sp_Wms_Production_Quality_Judgement_List]
	@PeriodFrom date			= '2026-04-01 00:00:00.000',
	@PeriodUntil date			= '2026-04-24 00:00:00.000',
	@FactoryCode varchar(25)	= '00000',
	@ProcessCode varchar(25)	= 'DP',
	@LineCode				varchar(25)		= 'DP-01',
	@CompleteCls bit			= 1
as
BEGIN
	Select a.ProductionID, a.ProdResultID,a.ProductionDate ScheduleDate, b.BarcodeNo, a.ItemCode,c.Item_Name ItemName, 
	c.Unit_Cls UnitCls,d.Description UnitClsDesc, b.LotNo, 
	 CAST(
		CASE 
		  WHEN b.ResultType = 'GOOD' THEN 1
		  ELSE 0 
		END 
	  AS BIT) AS Good,

	  CAST(
		CASE 
		  WHEN b.ResultType NOT IN ('GOOD','HOLD') THEN 1
		  ELSE 0 
		END 
	  AS BIT) AS NG,
	 a.LastUpdate, a.LastUser
	from ProductionResultHeader a
	Left Join ProductionResultDetail b on b.ProdResultID = a.ProdResultID
	Left Join Item_Master c on c.Item_Code = a.ItemCode
	Left Join Unit_Cls d on d.Unit_Cls = c.Unit_Cls
END
