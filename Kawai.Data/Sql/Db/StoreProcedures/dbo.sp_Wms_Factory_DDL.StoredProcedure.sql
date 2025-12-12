SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_Factory_DDL]
	@Keyword varchar(max)
as
	select Company_Code CompanyCode, Company_Name CompanyName, Company_Code + ' | ' + Company_Name DDLDescription
	From Company_Profile
	where 1=1
	and (Company_Code like '%'+ @Keyword +'%' or Company_Name like '%'+ @Keyword +'%')


GO
