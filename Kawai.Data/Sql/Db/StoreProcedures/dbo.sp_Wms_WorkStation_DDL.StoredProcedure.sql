SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE   PROCEDURE [sp_Wms_WorkStation_DDL]
	@Keyword varchar(max) = ''
as
begin
	select 
		RTRIM(WorkStationCode) WorkStationCode, WorkStationName,
	    WorkStationCode +' | '+ RTRIM(WorkStationName) DDLDescription
	From MS_WorkStation
	where 1=1
	and WorkStationName like '%'+ @Keyword +'%'
end

GO
