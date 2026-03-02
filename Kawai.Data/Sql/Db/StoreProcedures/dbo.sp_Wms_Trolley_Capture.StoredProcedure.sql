SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE   PROCEDURE [sp_Wms_Trolley_Capture]
	@TrolleyCode varchar(25)
as
select * From MS_Trolley where TrolleyCode = @TrolleyCode
GO
