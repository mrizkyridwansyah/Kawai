SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





Create PROCEDURE [sp_Wms_StopPoint_AddressSetting]
	@StopPointCode varchar(25),
	@AddressCode varchar(25)
as
begin
Update MS_Address Set StopPointCode = @StopPointCode where AddressCode = @AddressCode
end
GO
