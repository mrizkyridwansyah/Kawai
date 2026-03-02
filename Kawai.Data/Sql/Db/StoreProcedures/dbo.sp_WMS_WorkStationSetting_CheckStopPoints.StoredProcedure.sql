SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--exec sp_WMS_WorkStationLineSetting_CheckStopPoints @StopPoints=N'ST001,ST002',@LineCode=N'091'
CREATE   PROCEDURE [sp_WMS_WorkStationSetting_CheckStopPoints]
(
    @StopPoints dbo.tvp_StopPointList READONLY,   
    @LineCode	NVARCHAR(50)
)
AS
BEGIN


    SET NOCOUNT ON;
		
    DECLARE @Message NVARCHAR(MAX);

    SELECT @Message = STRING_AGG(
        'StopPoint ' + RTRIM(SP.Description) +
        ' sudah digunakan di Line ' + RTRIM(L.Line_Name) +
        ' dan WorkStation ' + RTRIM(w.WorkStationCode),
        '; '
    )
    FROM WorkStationLineSetting w
	JOIN Manufacture_Line L ON w.LineCode=L.Line_Code
	JOIN MS_StopPoint SP ON SP.StopPointCode=W.StopPointCode
    INNER JOIN @StopPoints s
        ON w.StopPointCode = s.StopPointCode
    WHERE w.LineCode <> @LineCode;

    IF @Message IS NOT NULL
    BEGIN
		RAISERROR(@Message,16,1)
		RETURN
        --THROW 50001, @Message, 1;
    END
	
END
GO
