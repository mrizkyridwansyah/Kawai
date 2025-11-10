SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   procedure [sp_Wms_Address_Capture]
@AddressCode varchar(25)
as

select 
	WarehouseCode, AreaCode, AddressCode, AddressName, PictureName, PictureRealName, QRCode 
From MS_Address where AddressCode = @AddressCode
GO
