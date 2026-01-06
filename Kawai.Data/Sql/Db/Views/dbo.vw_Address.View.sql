SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE view [vw_Address]
as
select 
	AddressCode, AddressName, AreaCode, WarehouseCode 
from MS_Address
union all
select 
	a.WorkStationCode, ws.WorkStationName, a.LineCode, b.Manufacture_Code ManufactureCode 
from WorkStationLineSetting a 
inner join Manufacture_Line b on a.LineCode = b.Line_Code 
inner join MS_WorkStation ws on a.WorkStationCode = ws.WorkStationCode
union all
select 'TMP', 'Temporary', null, null
GO
