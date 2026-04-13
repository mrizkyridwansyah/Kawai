SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   procedure [sp_Wms_Robot_SetTrolley]
	@RequestDetailId bigint,
	@TrolleyCode varchar(25),
	@RobotCode varchar(50)=''
as
begin
	if not exists (select * From PartMaterialRequestDetail where RequestDetailID = @RequestDetailId)	
	begin
		raiserror('Data Request tidak ditemukan!',16,1)
		return
	end

	if exists (select * From PartMaterialRequestDetail where RequestDetailID = @RequestDetailId and isnull(Trolley_No, @TrolleyCode) <> @TrolleyCode)	
	begin
		raiserror('Data Request sudah di set dengan Trolley berbeda!',16,1)
		return
	end

	update PartMaterialRequestDetail set Trolley_No = @RequestDetailId, LastUpdate = GETDATE(), LastUser = @RobotCode 
	where RequestDetailID = @RequestDetailId
end
GO
