USE Kawaii
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Wms_Andon_WominRequest_GetList]
@Area VARCHAR(50)=null
AS
BEGIN

	--DECLARE 
	--@Area VARCHAR(50)='WH-001/001'

	SELECT * FROM (
	SELECT 'R.L1.WS1.20251120.1' RequestNo,'2025-11-20' ProductionDate, 'Line 01' Line,'Work Station 1' WorkStation, 'WH-001/001' PickingArea,
			'On Progress' PreparationStatus, 'TR0005' TrollyNumber, 'PCB' CurrentPosition ,'CLOTHS' NextLocation,TotalItem=1, Remaining=0
	UNION ALL
	SELECT 'R.L1.WS2.20251120.1' RequestNo,'2025-11-20' ProductionDate, 'Line 01' Line,'Work Station 2' WorkStation, 'WH-001/001' PickingArea,
			'Waiting' PreparationStatus, 'TR0011' TrollyNumber, 'METAL' CurrentPosition ,'BUFFER AREA' NextLocation,TotalItem=1, Remaining=0
	UNION ALL
	SELECT 'R.L3.WS2.20251120.1' RequestNo,'2025-11-20' ProductionDate, 'Line 03' Line,'Work Station 1' WorkStation, 'WH-001/001' PickingArea,
			'Waiting' PreparationStatus, 'TR0015' TrollyNumber, 'WOODEN' CurrentPosition ,'CLOTHS' NextLocation,TotalItem=1, Remaining=0
	) A
	WHERE PickingArea=@Area

END
GO