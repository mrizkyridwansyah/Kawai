SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--exec sp_WMS_WorkStationLineSetting_CheckStopPoints @StopPoints=N'ST001,ST002',@LineCode=N'091'
CREATE   PROCEDURE [sp_WMS_WorkStationSetting_CheckStopPoints]
(
    @StopPoints dbo.tvp_StopPointList READONLY,   
    @StopPoints2 dbo.tvp_StopPointList2 READONLY,   
    @StopPoints3 dbo.tvp_StopPointList3 READONLY,   
    @LineCode NVARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Message NVARCHAR(MAX);

-------------------------------------------------------
-- StopPoint tidak boleh sama antar input
-------------------------------------------------------

IF EXISTS (
    SELECT 1
    FROM @StopPoints s1
    JOIN @StopPoints2 s2 
        ON s1.StopPointCode = s2.StopPointCode2
)
BEGIN
    RAISERROR('StopPoint #1 tidak boleh sama dengan StopPoint #2',16,1)
    RETURN
END

IF EXISTS (
    SELECT 1
    FROM @StopPoints s1
    JOIN @StopPoints3 s3 
        ON s1.StopPointCode = s3.StopPointCode3
)
BEGIN
    RAISERROR('StopPoint #1 tidak boleh sama dengan StopPoint #3',16,1)
    RETURN
END

IF EXISTS (
    SELECT 1
    FROM @StopPoints2 s2
    JOIN @StopPoints3 s3 
        ON s2.StopPointCode2 = s3.StopPointCode3
)
BEGIN
    RAISERROR('StopPoint #2 tidak boleh sama dengan StopPoint #3',16,1)
    RETURN
END

-------------------------------------------------------
-- Gabungkan StopPoint existing di database
-------------------------------------------------------

;WITH UsedStopPoints AS
(
    SELECT 
        W.LineCode,
        W.WorkStationCode,
        W.StopPointCode AS StopPoint
    FROM WorkStationLineSetting W
    WHERE W.StopPointCode IS NOT NULL

    UNION ALL

    SELECT 
        W.LineCode,
        W.WorkStationCode,
        W.StopPointCode2
    FROM WorkStationLineSetting W
    WHERE W.StopPointCode2 IS NOT NULL
),
NewStopPoints AS
(
    SELECT StopPointCode AS StopPoint FROM @StopPoints
    UNION ALL
    SELECT StopPointCode2 FROM @StopPoints2
    UNION ALL
    SELECT StopPointCode3 FROM @StopPoints3
)

-------------------------------------------------------
-- Cek apakah sudah pernah dipakai
-------------------------------------------------------

SELECT @Message = STRING_AGG(
    'StopPoint ' + RTRIM(SP.Description) +
    ' sudah digunakan di Line ' + RTRIM(L.Line_Name) +
    ' dan WorkStation ' + RTRIM(U.WorkStationCode),
    '; '
)
FROM UsedStopPoints U
JOIN NewStopPoints N 
    ON U.StopPoint = N.StopPoint
JOIN Manufacture_Line L 
    ON U.LineCode = L.Line_Code
JOIN MS_StopPoint SP 
    ON SP.StopPointCode = U.StopPoint
WHERE U.LineCode <> @LineCode;

IF @Message IS NOT NULL
BEGIN
    RAISERROR(@Message,16,1)
    RETURN
END

END
GO
