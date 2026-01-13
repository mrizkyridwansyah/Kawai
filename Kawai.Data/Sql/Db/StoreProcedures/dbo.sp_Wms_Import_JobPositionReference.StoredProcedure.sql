SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_Import_JobPositionReference]
as
begin
	select Code, Description, ParGroup from Cls_Parameter
end
GO
