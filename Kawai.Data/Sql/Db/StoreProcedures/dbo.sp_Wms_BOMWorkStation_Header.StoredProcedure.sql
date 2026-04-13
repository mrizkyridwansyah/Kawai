SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Proc [sp_Wms_BOMWorkStation_Header]

@FactoryCode Varchar(100),
@LineCode Varchar(100),
@ModelCls Varchar(100),
@ParentItem_Code Varchar(100),
@ProcessCode Varchar(100),
@QtySet Numeric(10,0),
@Trolley_Cls Varchar(100),
@WorkStationCode Varchar(100),
@UserID varchar(25)

as
   IF NOT EXISTS (Select Top 1 1 From MS_BOMPerworkstation_Header where Line_Code = @LineCode and ParentItemCode = @ParentItem_Code and WorkStationCode = @WorkStationCode)
   Begin
   Insert into MS_BOMPerworkstation_Header (Line_Code,ParentItemCode,WorkStationCode,MAX_Qty_Set,Troly_Cls,RegisterDate,RegisterUser,LastUpdate,LastUser) values(@LineCode,@ParentItem_Code,@WorkStationCode,@QtySet,@Trolley_Cls,Getdate(),@UserID,Getdate(),@UserID)
   end
   Else
   Begin
   Update MS_BOMPerworkstation_Header 
   set MAX_Qty_Set = @QtySet,
       Troly_Cls = @Trolley_Cls,
	   LastUpdate= getdate(),
	   LastUser = @UserID
	   where Line_Code = @LineCode and ParentItemCode = @ParentItem_Code and WorkStationCode = @WorkStationCode
   End
GO
