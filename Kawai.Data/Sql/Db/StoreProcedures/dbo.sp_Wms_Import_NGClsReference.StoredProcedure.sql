SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create   procedure [sp_Wms_Import_NGClsReference]
as
begin
	select ClsCode, Description from vw_Cls where TypeData = 'NGCls'
end
GO
