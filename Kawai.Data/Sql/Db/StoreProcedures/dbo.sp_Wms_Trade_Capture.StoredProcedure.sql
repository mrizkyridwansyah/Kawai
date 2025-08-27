SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE OR ALTER PROCEDURE [sp_Wms_Trade_Capture]
	@Trade_Code varchar(25)
as
select * From Trade_Master where Trade_Code = @Trade_Code
GO
