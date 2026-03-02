SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [sp_Wms_Robot_EmptyTrolley_Capture]
	@TrolleyNo varchar(25)
AS
BEGIN
	print('empty trolley')
END
GO
