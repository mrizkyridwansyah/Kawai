SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


create   procedure [sp_Wms_Item_Capture]
	@ItemCode varchar(25)
as
select * From Item_Master where Item_Code = @ItemCode
GO
