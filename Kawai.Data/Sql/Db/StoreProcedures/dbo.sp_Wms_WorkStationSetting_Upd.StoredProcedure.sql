SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [sp_Wms_WorkStationSetting_Upd]
	
	@LineCode varchar(100),
	@WorkStationCode varchar(100),
	@AllowSetting bit,
	@UserID varchar(25) 
as 
if @AllowSetting = 1
	begin
		 insert into WorkStationLineSetting (
			LineCode,
			WorkStationCode,
			Barcode,
			RegisterDate,
			RegisterUser,
			LastUpdate,
			LastUser) values 
			(@LineCode , @WorkStationCode , RTRIM(@LineCode) + RTRIM(@WorkStationCode), Getdate(),@UserID , NULL, NULL)
	 
	End
   
--delete from SS_UserPrivilege where UserID = @UserID
--delete from SS_UserWarehousePrivilege where UserID = @UserID


GO
