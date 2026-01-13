SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [sp_Wms_PeriodSetting_ListDetail] 
--declare
	@Year int = 2026
AS
BEGIN
Declare @TablePeriod Table ([Month] Int ,[MonthPeriod] Varchar(100), [MonthName] Varchar(100))

        Insert into @TablePeriod
        SELECT [Month]= 1, [MonthPeriod]= '01', [MonthName]= 'January'    UNION
        SELECT [Month]= 2, [MonthPeriod]= '02', [MonthName]= 'February'	  UNION
        SELECT [Month]= 3, [MonthPeriod]= '03', [MonthName]= 'March'	  UNION
        SELECT [Month]= 4, [MonthPeriod]= '04', [MonthName]= 'April'	  UNION
        SELECT [Month]= 5, [MonthPeriod]= '05', [MonthName]= 'May'		  UNION
        SELECT [Month]= 6, [MonthPeriod]= '06', [MonthName]= 'June'		  UNION
        SELECT [Month]= 7, [MonthPeriod]= '07', [MonthName]= 'July'		  UNION
        SELECT [Month]= 8, [MonthPeriod]= '08', [MonthName]= 'August'	  UNION
        SELECT [Month]= 9, [MonthPeriod]= '09', [MonthName]= 'September'  UNION
        SELECT [Month]= 10,[MonthPeriod]= '10', [MonthName]= 'October'	  UNION
        SELECT [Month]= 11,[MonthPeriod]= '11', [MonthName]= 'November'	  UNION
        SELECT [Month]= 12,[MonthPeriod]= '12', [MonthName]= 'December'

Select Cast(@Year as varchar) + A.MonthPeriod as [Period], @Year as [Year] , A.[Month], A.[MonthName],
	    FORMAT(StartPeriod, 'yyyy-MM-dd') StartDate,
		FORMAT(StartPeriod, 'HH:mm') StartTime,
		FORMAT(EndPeriod, 'yyyy-MM-dd') EndDate,
		FORMAT(EndPeriod, 'HH:mm') EndTime,
		FORMAT(ISNULL(StartSO,EndPeriod), 'yyyy-MM-dd')  as StartSODate,
		CASE WHEN StartPeriod IS NOT NULL THEN ISNULL(FORMAT(StartSO, 'HH:mm'),'07:00') ELSE NULL END  as StartSOTime,
		FORMAT(ISNULL(FinishSO,EndPeriod) , 'yyyy-MM-dd')  as EndSODate,
		CASE WHEN StartPeriod IS NOT NULL THEN FORMAT(ISNULL(FinishSO,EndPeriod), 'HH:mm') ELSE NULL END  as  EndSOTime 
  from  @TablePeriod a Left JOIN MS_PeriodSetting b ON a.[Month] = b.[Month] and b.[Year] = @Year   
	 
END
GO
