

create   procedure [dbo].[SP_Scheduler_TransferDataRobot2]
AS

BEGIN

DECLARE @TableMain TABLE
(
    RefNo VARCHAR(100),
    RequestID VARCHAR(100),
    WorkStationCode VARCHAR(100),
    SeqNo VARCHAR(100),
    LineCode VARCHAR(100),
    ProductionDate DATE,
    ParentItemCode VARCHAR(100),
    TrolleyCls VARCHAR(100)
);

INSERT INTO @TableMain
SELECT 
	dtl.RefNo,
    dtl.RequestID,
    dtl.WorkStationCode,
    dtl.Seq,
    hd.LineCode,
    hd.ProductionDate,
    hd.ParentItem_Code,
	bomws.Troly_Cls
FROM
(
    SELECT
		DISTINCT
        RefNumber AS RefNo,
        RequestID,
        TRIM(WorkStationCode) AS WorkStationCode,
        Seq,
        RequestStatusID
    FROM PartMaterialRequestDetail
	WHERE RequestStatusID = 5
) dtl
INNER JOIN PartMaterialRequestHeader hd 
	ON dtl.RequestID = hd.RequestID
INNER JOIN MS_BOMPerworkstation_Header bomws 
	ON bomws.ParentItemCode = hd.ParentItem_Code AND bomws.Line_Code = hd.LineCode AND bomws.WorkStationCode = dtl.WorkStationCode
LEFT JOIN PartMaterialRequestSendRobotHeader hdr ON hdr.RequestSendID = dtl.RefNo
WHERE hdr.RequestSendID IS NULL

INSERT INTO PartMaterialRequestSendRobotHeader
(
    RequestSendID,
    RequestID,
    WorkStationCode,
    Seq,
    LineCode,
    ProductionDate,
    ParentItem_Code,
    Trolley_Cls,
    RegisterDate,
    RegisterUser,
    LastUpdate,
    LastUser
)
SELECT 
	RefNo, 
	RequestID, 
	WorkStationCode, 
	SeqNo, 
	LineCode, 
	ProductionDate, 
	ParentItemCode, 
	TrolleyCls,
    GETDATE(),
    'Scheduller',
    GETDATE(),
    'Scheduller'
fROM @TableMain ORDER BY RefNo

INSERT INTO PartMaterialRequestSendRobotDetail
(
    RequestSendID,
    Stop_Point,
    Pickup_Seq,
    RegisterDate,
    RegisterUser,
    LastUpdate,
    LastUser,
    [status],
	IsManual
)
SELECT
    RefNumber,
    StopPointCode,
    PickingSeq,
    GETDATE(),
    'Scheduller',
    GETDATE(),
    'Scheduller',
    0,
	IsCurrentProcessManual
FROM
(
    SELECT DISTINCT
		dtl.RefNumber,
        addr.StopPointCode,
        sp.PickingSeq,
		dtl.IsCurrentProcessManual
    FROM 
	(
		select x.* From PartMaterialRequestDetail x
		inner join @TableMain y on x.RefNumber = y.RefNo
	) dtl 
	INNER JOIN PartMaterialRequestItemDetail dtlI on dtli.RequestDetailID = dtl.RequestDetailID
	INNER JOIN PartMaterialRequestItemDetailScan scan on scan.IDSeq = dtli.IDSeq
    INNER JOIN MS_Address addr ON addr.AddressCode = scan.FromAddressCode
    INNER JOIN MS_StopPoint sp ON sp.StopPointCode = addr.StopPointCode
	LEFT JOIN PartMaterialRequestSendRobotDetail dtlr on dtlr.RequestSendID = dtl.RefNumber and dtlr.Stop_Point = sp.StopPointCode
	WHERE dtlr.RequestSendDetailID IS NULL
) A

END;

