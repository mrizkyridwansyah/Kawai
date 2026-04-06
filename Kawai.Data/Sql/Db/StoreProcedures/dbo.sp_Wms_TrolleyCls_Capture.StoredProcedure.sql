SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE PROCEDURE [sp_Wms_TrolleyCls_Capture]
	@Trolley_Cls varchar(25)
as
select 
Trolley_Cls 
,[Description]
,Qty
,RegisterDate
,RegisterUser
,LastUpdate
,LastUser
From Trolley_Cls where Trolley_Cls = @Trolley_Cls
GO
