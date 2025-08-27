SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [sp_Wms_Address_Delete]
	@AddressCode varchar(25)
as
begin
	if not exists (select 1 from MS_Address where AddressCode = @AddressCode)
	begin
		raiserror('Data Address didn''t Exists',16,1)
		return;
	end

	delete from MS_Address where AddressCode = @AddressCode
end
GO
