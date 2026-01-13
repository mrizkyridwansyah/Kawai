SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [sp_Wms_BOMWorkStation_Upd]
	
	@ParentItem_Code 	 varchar(25) = '000006',
    @WorkStationCode varchar(25)='',
	@ChildItem_Code Varchar(25),
	@Qty   Numeric(9,5),
	@AllowSetting bit,
	@UserID varchar(25) 
as 
if @AllowSetting = 1
	begin
	     Declare @qtySisa Numeric(9,5), @Unit Varchar(100), @ItemName Varchar(75) , @ErrorMsg Varchar(Max)
		 Select @Unit = Unit_Cls , @ItemName = RTRIM(Item_Name) from BOM_Master where Parent_ItemCode = @ParentItem_Code and Item_Code = @ChildItem_Code
	     Select @qtySisa = ISNULL((Select Qty from BOM_Master where Parent_ItemCode = @ParentItem_Code and Item_Code = @ChildItem_Code),0) -  ISNULL((Select qty From MS_BOMPerworkstation where ParentItem_Code = @ParentItem_Code and ChildItem_Code = @ChildItem_Code and WorkStationCode <> @WorkStationCode),0)
		 if @qtySisa < @Qty
		 Begin
		 Set @ErrorMsg = 'The quantity of item ' + @ItemName+' cannot be greater than the quantity in the BOM master and the quantity already registered in the BOS workstation.' 
		 RAISERROR (@ErrorMsg,  
               16,  
               1  
               );
			   return
		 end
		 
		 insert into MS_BOMPerworkstation(
			ParentItem_Code,
			WorkStationCode,
			ChildItem_Code,
			Unit_Cls,
			Qty,
			RegisterDate,
			RegisterUser,
			LastUpdate,
			LastUser) values 
			(@ParentItem_Code , @WorkStationCode , @ChildItem_Code, @Unit,@Qty,   Getdate(),@UserID , NULL, NULL)
	 
	End
 
--delete from SS_UserPrivilege where UserID = @UserID
--delete from SS_UserWarehousePrivilege where UserID = @UserID


GO
