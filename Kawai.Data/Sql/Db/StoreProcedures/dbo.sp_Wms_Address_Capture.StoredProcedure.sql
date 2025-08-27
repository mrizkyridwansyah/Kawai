SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [sp_Wms_Address_Capture]
@AddressCode varchar(25)
as

select * From MS_Address where AddressCode = @AddressCode
GO
