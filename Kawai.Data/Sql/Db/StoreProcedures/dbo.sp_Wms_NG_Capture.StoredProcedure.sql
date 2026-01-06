SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



create   procedure [sp_Wms_NG_Capture]
	@NGCode varchar(25)
as
select * From MS_NG where NGCode = @NGCode
GO
