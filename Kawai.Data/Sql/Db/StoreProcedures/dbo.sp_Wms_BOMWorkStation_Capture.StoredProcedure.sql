SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [sp_Wms_BOMWorkStation_Capture]
	@ParentItem_Code 	 varchar(25) = '000006',
    @WorkStationCode varchar(25)=''
as 

select ChildItem_Code, Qty From MS_BOMPerworkstation where ParentItem_Code = @ParentItem_Code and WorkStationCode = @WorkStationCode
GO
