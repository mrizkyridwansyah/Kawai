SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [sp_Wms_BOMWorkStation_Delete]
	@ParentItem_Code 	 varchar(25) = '000006',
    @WorkStationCode varchar(25)=''
as 

delete from MS_BOMPerworkstation where ParentItem_Code = @ParentItem_Code and WorkStationCode = @WorkStationCode 

GO
