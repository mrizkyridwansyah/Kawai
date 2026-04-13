SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--USE [EZRunnerV3_KawaiLive]
--GO
--/****** Object:  StoredProcedure [dbo].[sp_Wms_SendRobot]    Script Date: 2026-02-26 13:48:23 ******/
--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO

CREATE PROCEDURE [sp_Wms_SendRobot]
 
as
 

SELECT
  H.RequestSendID,
   H.LineCode,
   H.WorkStationCode,
   H.ProductionDate ProductionDate ,
   H.ParentItem_Code as Model,
   H.Trolley_Cls TrolleyCls,
   H.RegisterDate PickingDatetime ,
 

 JSON_QUERY(
 (
  SELECT
   D.RequestSendID,
   D.Stop_Point StopPoint,
   D.Pickup_Seq as PickupSequence,
   1 [Status]
  FROM PartMaterialRequestSendRobotDetail D
  WHERE D.RequestSendID = H.RequestSendID
  ORDER BY D.Pickup_Seq
  FOR JSON PATH
 )) AS Details

FROM PartMaterialRequestSendRobotHeader H
 
  

FOR JSON PATH;

 


 
GO
