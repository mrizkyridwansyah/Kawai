
 create   procedure [dbo].[SP_Scheduler_TransferDataRobot]
 as

Declare @TableMain table (RefNo Varchar(100)  , RequestID Varchar(100) , WorkStationCode Varchar(100) , SeqNo Varchar(100) , LineCode Varchar(100) , ProductionDate Date , ParentItem_Code Varchar(100), TrolleyCls Varchar(100))
Declare @TableMainLooping table ( RowNo int, RefNo Varchar(100)  , RequestID Varchar(100) , WorkStationCode Varchar(100) , SeqNo Varchar(100) , LineCode Varchar(100) , ProductionDate Date , ParentItem_Code Varchar(100), TrolleyCls Varchar(100))


Insert into @TableMain
Select distinct
A.RefNo,A.RequestID , WorkStationCode  , Seq , C.LineCode , C.ProductionDate , C.ParentItem_Code ,(select Troly_Cls from MS_BOMPerworkstation_Header ZZ where ZZ.ParentItemCode =  C.ParentItem_Code and ZZ.Line_Code = C.LineCode and ZZ.WorkStationCode = A.WorkStationCode)
FROM (Select RefNumber RefNo, RequestID , Trim(WorkStationCode) WorkStationCode, Seq,RequestStatusID from PartMaterialRequestDetail ) A 
LEFT JOIN PartMaterialRequestHeader C ON A.RequestID = C.RequestID 
WHERE RequestStatusID = '5' and Not EXISTS  ( Select * from ( Select 
																RefNumber RefNo,RequestID ,Trim(WorkStationCode) WorkStationCode , 
																Seq,RequestStatusID
															  from PartMaterialRequestDetail 
														    ) B Where B.RefNo = A.RefNo and RequestStatusID <>'5')


DECLARE @i int  
DECLARE @DataCount  INT 
DECLARE @RefNo Varchar(100)  
DECLARE @RequestID Varchar(100) 
DECLARE @WorkStationCode Varchar(100) 
DECLARE @SeqNo Varchar(100) 
DECLARE @LineCode Varchar(100) 
DECLARE @ProductionDate Date 
DECLARE @ParentItem_Code Varchar(100) 
DECLARE @TrolleyCls Varchar(100) 


insert into @TableMainLooping 
select  row_number() over(order by RequestID, WorkStationCode , SeqNo) as RowNo, * from @TableMain A Where Not EXISTS  (Select * from  [PartMaterialRequestSendRobotHeader] B where B.RequestSendID = A.RefNo)
select * from @TableMainLooping
SET @i  = 1
SET  @DataCount  = (select count(*) from @TableMainLooping)
		while @i <= @DataCount
		begin
			SET @RefNo				= (select RefNo  from @TableMainLooping where RowNo = @i)
			SET @RequestID 			= (select RequestID  from @TableMainLooping where RowNo = @i)
			SET @WorkStationCode	= (select WorkStationCode  from @TableMainLooping where RowNo = @i)
			SET @SeqNo 				= (select SeqNo  from @TableMainLooping where RowNo = @i)
			SET @LineCode			= (select LineCode  from @TableMainLooping where RowNo = @i)
			SET @ProductionDate		= (select Productiondate  from @TableMainLooping where RowNo = @i)
			SET @ParentItem_Code	= (select ParentItem_Code  from @TableMainLooping where RowNo = @i)
			SET @TrolleyCls	        = (select TrolleyCls  from @TableMainLooping where RowNo = @i)

			declare @isCurrentProcessManual bit = (select top 1 IsCurrentProcessManual from PartMaterialRequestDetail where RefNumber = @RefNo)
		 	 
 
	 
		  Insert into [PartMaterialRequestSendRobotHeader] (RequestSendID,RequestID,WorkStationCode,Seq,LineCode,ProductionDate,ParentItem_Code,Trolley_Cls,RegisterDate,RegisterUser,LastUpdate,LastUser)
	      values  (@RefNo,@RequestID,@WorkStationCode,@SeqNo,@LineCode,@ProductionDate,@ParentItem_Code,@TrolleyCls,Getdate(),'Scheduller',Getdate(),'Scheduller')

		  Insert into [PartMaterialRequestSendRobotDetail] (RequestSendID,Stop_Point,Pickup_Seq,RegisterDate,RegisterUser,LastUpdate,LastUser, [status], IsManual)
		  Select RequestSendID , StopPointCode , PickingSeq , Getdate(),'Scheduller',Getdate(),'Scheduller', 0, @isCurrentProcessManual from (
		  Select DISTINCT @RefNo as RequestSendID, B.StopPointCode , PickingSeq 
		  from PartMaterialRequestItemDetailScan A 
		  Left JOIN MS_Address B ON B.AddressCode = A.FromAddressCode 
		  LEFT JOIN MS_StopPoint C ON C.StopPointCode = B.StopPointCode
		  where B.StopPointCode is NOT NULL and IDSeq in (
 		  select IDSeq from PartMaterialRequestItemDetail where RequestDetailID in  (select RequestDetailID from PartMaterialRequestdetail where RequestID = @RequestID))
		  ) A Where NOT EXISTS (Select * From[PartMaterialRequestSendRobotDetail] CC where CC.RequestSendID = A.RequestSendID and CC.Stop_Point = A.StopPointCode)
		 
		 
		 set @i = @i + 1


		END


