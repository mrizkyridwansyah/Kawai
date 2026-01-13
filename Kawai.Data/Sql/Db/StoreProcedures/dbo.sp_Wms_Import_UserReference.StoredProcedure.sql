SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_Import_UserReference]
as
begin
	select UserID, FullName from vw_User
end
GO
