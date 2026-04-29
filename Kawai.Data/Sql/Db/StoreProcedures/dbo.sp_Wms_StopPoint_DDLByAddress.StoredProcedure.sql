create procedure [dbo].[sp_Wms_StopPoint_DDLByAddress]
	@Keyword varchar(max) = '',
	@Line Varchar(100) =' ',
	@Workstation Varchar(100) = ' '
as
begin
   Select RTRIM(A.StopPointCode) StopPointCode, [Description], RTRIM(A.StopPointCode) + ' | ' + [Description] as [DDLDescription] from (
   select RTRIM(StopPointCode) StopPointCode from WorkStationLineSetting where LineCode = @Line and StopPointCode <>'' and WorkStationCode = @Workstation UNION
   select RTRIM(StopPointCode2) StopPointCode from WorkStationLineSetting where LineCode = @Line and StopPointCode2 <>''and WorkStationCode = @Workstation UNION
   select RTRIM(StopPointCode3) StopPointCode from WorkStationLineSetting where LineCode = @Line and StopPointCode3 <>''and WorkStationCode = @Workstation
    ) A LEFT JOIN  MS_StopPoint B ON A.StopPointCode = B.StopPointCode
	where  1=1  
	and (A.StopPointCode like '%'+ @Keyword +'%' or Description like '%'+ @Keyword +'%')  UNION
	select 
		RTRIM(StopPointCode) StopPointCode, [Description], RTRIM(StopPointCode) + ' | ' + [Description] as [DDLDescription]
	From MS_StopPoint 
	where 1=1 and StopPointCode not in 
	(
		Select StopPointCode2 From (
		select StopPointCode as StopPointCode2  from WorkStationLineSetting where iSNULL(StopPointCode,'') <> '' UNION
		select StopPointCode2 from WorkStationLineSetting  where iSNULL(StopPointCode2,'') <> ''UNION
		select StopPointCode3 from WorkStationLineSetting  where iSNULL(StopPointCode3,'') <> '') a)
	and (StopPointCode like '%'+ @Keyword +'%' or Description like '%'+ @Keyword +'%')
end
 