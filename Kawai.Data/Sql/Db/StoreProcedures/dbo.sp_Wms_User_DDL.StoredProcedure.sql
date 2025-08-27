SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE OR ALTER PROCEDURE [sp_Wms_User_DDL]
	@Keyword varchar(max) = '' 
as
begin
	select 
		ma.UserID as UserGroupID, ma.FullName
	From SS_UserSetup ma
	where 1=1
	 	and ma.FullName like '%'+ @Keyword +'%'
end

GO
