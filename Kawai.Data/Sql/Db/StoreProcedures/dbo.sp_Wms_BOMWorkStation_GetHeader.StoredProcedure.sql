SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Proc [sp_Wms_BOMWorkStation_GetHeader]

 
@LineCode Varchar(100),
@ParentItem_Code Varchar(100),
@WorkStationCode Varchar(100) 

as
  Select 
 Line_Code
,ParentItemCode ParentItem_Code
,WorkStationCode
,MAX_Qty_Set as QtySet
,Troly_Cls as Trolley_Cls
From MS_BOMPerworkstation_Header where Line_Code = @LineCode and ParentItemCode = @ParentItem_Code and WorkStationCode = @WorkStationCode
    
GO
