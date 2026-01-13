SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create   procedure [sp_Wms_Import_FactoryReference]
as
begin
	select Company_Code FactoryCode, Company_Name FactoryName from Company_Profile
end
GO
