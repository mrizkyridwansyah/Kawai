SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [sp_Wms_Mobile_LoadingTrolley_CapturePicking]
	@PickingNo varchar(50)
as

select top 1 a.*
from PartMaterialRequestHeader a 
inner join PartMaterialRequestDetail b on a.RequestID = b.RequestID
where RefNumber = @PickingNo

select 
	RequestSendDetailID, 
	RequestSendID, 
	Stop_Point, 
	Pickup_Seq, 
	Status,
	StatusAMR, 
	LastUserRequestAMR,
	LastRequestDateAMR
from PartMaterialRequestSendRobotDetail a 
where RequestSendID = @PickingNo
GO
