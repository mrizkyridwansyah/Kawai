SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [sp_Wms_Area_Capture]
	@AreaCode varchar(25)
as
begin
	select * From MS_Area where AreaCode = @AreaCode
end





GO
