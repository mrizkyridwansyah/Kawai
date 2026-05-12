
create   procedure [dbo].[sp_Wms_Robot_GetDataRequest]
	@RequestSendID varchar(50) = 'ALL'
AS
BEGIN
 	SELECT
		hd.RequestSendID,
		hd.LineCode,
		hd.WorkStationCode,
		hd.ProductionDate ProductionDate ,
		hd.Model,
		hd.TrolleyCls,
		hd.TrolleyNo,
		hd.PickingDatetime PickingTime,
		dtl.Stop_Point StopPoint,
		hd.IsCurrentProcessManual,
		DENSE_RANK() OVER (PARTITION BY hd.RequestSendID ORDER BY dtl.Pickup_Seq) as PickupSequence,
		dtl.[Status]
	FROM 
	(
		SELECT 		
			a.RequestSendID,
			a.LineCode,
			a.WorkStationCode,
			a.ProductionDate ProductionDate ,
			a.ParentItem_Code as Model,
			a.Trolley_Cls TrolleyCls,
			b.Trolley_No TrolleyNo,
			a.RegisterDate PickingDatetime,
			b.IsCurrentProcessManual
		fROM PartMaterialRequestSendRobotHeader a
		INNER JOIN PartMaterialRequestDetail b on a.RequestSendID = b.RefNumber
		WHERE 1 = case when @RequestSendID = 'ALL' THEN 1 WHEN RequestSendID = @RequestSendID THEN 1 ELSE 0 END
	) hd
	inner join PartMaterialRequestSendRobotDetail dtl on dtl.RequestSendID = hd.RequestSendID
	ORDER BY dtl.Pickup_Seq
END
 


 
