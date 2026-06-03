SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE view [vw_Area]
as
select AreaCode, AreaName from MS_Area
union all
select WorkStationCode, WorkStationName From MS_WorkStation
union all
select 'TMP', 'Temporary'
GO
