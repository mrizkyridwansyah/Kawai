SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create  PROCEDURE [sp_Wms_BOMWorkStation_DetailDelete]
		@LineCode Varchar(100),
		@ParentItem_Code Varchar(100),
		@WorkStationCode Varchar(100)
    
as 
Declare @SeqNo int
Select @SeqNo = Bomws_ID From MS_BOMPerworkstation_Header where Line_Code = @LineCode and ParentItemCode = @ParentItem_Code and WorkStationCode = @WorkStationCode
delete from MS_BOMPerworkstation_Detail where Bomws_ID = @SeqNo

GO
