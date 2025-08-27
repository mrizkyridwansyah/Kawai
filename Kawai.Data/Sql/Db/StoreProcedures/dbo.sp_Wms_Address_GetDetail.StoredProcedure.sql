SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [sp_Wms_Address_GetDetail]
	@AddressCode varchar(25)
as
begin
	select WarehouseCode, AreaCode, AddressCode, AddressName	From MS_Address
	where AddressCode = @AddressCode
end
GO
