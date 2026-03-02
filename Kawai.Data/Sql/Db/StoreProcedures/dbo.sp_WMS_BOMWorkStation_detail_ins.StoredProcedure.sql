SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [sp_WMS_BOMWorkStation_detail_ins]
		@LineCode Varchar(100),
		@ParentItem_Code Varchar(100),
		@WorkStationCode Varchar(100),
		@ChildItem_Code  Varchar(100),
		@Qty   Numeric(9,5),
	    @UserID varchar(25) 
    
as 
Declare @SeqNo int
Select @SeqNo = Bomws_ID From MS_BOMPerworkstation_Header where Line_Code = @LineCode and ParentItemCode = @ParentItem_Code and WorkStationCode = @WorkStationCode

Declare @Unit Varchar(10)
select @Unit = Unit_Cls from Item_Master where Item_Code = @ChildItem_Code
Insert into MS_BOMPerworkstation_Detail (Bomws_ID,ChildItem_Code,Unit_Cls,Qty,RegisterDate,RegisterUser,LastUpdate) values (@SeqNo ,@ChildItem_Code, @Unit , @Qty, Getdate() , @UserID, Getdate() )
GO
