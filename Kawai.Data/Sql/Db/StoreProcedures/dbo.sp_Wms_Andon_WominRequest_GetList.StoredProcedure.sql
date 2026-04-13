SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [sp_Wms_Andon_WominRequest_GetList]
--declare
@Area VARCHAR(50)='02'
AS
BEGIN


 
 
	Declare @CountProdID int
	Declare @CountData int
	Declare @CountDataScan int
	 Select @CountProdID = COUNT( Distinct ProductionID) from PartMaterialRequestHeader where RequestID in (
	 SELECT RequestID FROM PartMaterialRequestDetail where AreaCode = @Area)
	
	select @CountData = SUM(qty)  from (
	select A.RefNumber , Count( Distinct B.ItemCode)qty from   
	(SELECT RequestDetailID , RefNumber FROM PartMaterialRequestDetail where AreaCode = @Area) A 
	Left Join PartMaterialRequestItemDetail B ON A.RequestDetailID = B.RequestDetailID
	Group by A.RefNumber ) A

	 select @CountDataScan = SUM(qty)  From (
	Select RefNumber,Count(distinct QtyScan) Qty from (
	select RefNumber,  B.ItemCode,
     b.ChildRequirement_Qty - ISNULL((select SUM(Qty) from PartMaterialRequestItemDetailScan cc where cc.ItemCode = b.ItemCode and cc.IDSeq = B.IDSeq),0) QtyScan
	From PartMaterialRequestDetail A 
	LEFT JOIN PartMaterialRequestItemDetail B ON A.RequestDetailID = B.RequestDetailID
	Left Join Item_Master c on b.ItemCode = c.Item_Code
	Left JOIN Unit_Cls d on d.Unit_Cls = b.unit_Cls
	where a.RequestDetailID in (SELECT RequestDetailID   FROM PartMaterialRequestDetail where AreaCode = @Area)  
	) a where QtyScan = 0
	Group by RefNumber) A 
	SELECT A.RequestDetailNo RequestNo, B.ProductionDate , B.LineCode 
	,D.Line_Name as Line , A.WorkStationCode , C.WorkStationName as WorkStation,
	E.StatusDescription PreparationStatus, 
	--(Select  top 1 AreaName from MS_Area dd where DD.ItemType = @Area) PickingArea,
	LTRIM(RTRIM(F.Item_Name)) PickingArea,
	G.Description as Model,
	A.Trolley_No as TrollyNumber ,
	(Select Top 1 AreaName from MS_Address ss Left JOIN MS_Area cc ON ss.AreaCode = cc.AreaCode where StopPointCode = (select LastPosition from MS_Trolley where TrolleyCode = 	A.Trolley_No))  CurrentPosition ,
	'' NextLocation, TotalItem=1, Remaining=0 ,
	A.RefNumber,
	(select Top 1 Pickup_Seq from PartMaterialRequestSendRobotDetail ddd where  ddd.RequestSendID =A.RefNumber and Stop_Point in (select LastPosition from MS_Trolley where TrolleyCode = 	A.Trolley_No)) Picking_Seq
	into #Tblmain
	FROM PartMaterialRequestDetail A 
	left JOIN PartMaterialRequestHeader B ON A.RequestID = B.RequestID
	Left Join MS_WorkStation C ON C.WorkStationCode = A.WorkStationCode
	Left Join Manufacture_Line D on D.Line_Code = B.LineCode
	left join RequestStatusCls E ON E.RequestStatusID = A.RequestStatusID
	left join Item_Master F ON F.Item_Code = B.ParentItem_Code
	LEFT JOIN Model_Cls G ON G.Model_Cls = F.Model_Cls
	 
	where AreaCode = @Area

	 
	 select 
	 A.RefNumber RequestNo	
	 ,ProductionDate	
	 ,LineCode	
	 ,Line	
	 ,WorkStationCode	
	 ,WorkStation	
	 ,PreparationStatus	
	 ,PickingArea	 
	 ,Model
	 ,TrollyNumber	
	 ,CurrentPosition	
	  	, (Select Top 1 AreaName from MS_Address ss Left JOIN MS_Area cc ON ss.AreaCode = cc.AreaCode where StopPointCode =(select Top 1 Stop_Point from PartMaterialRequestSendRobotDetail ddd where  ddd.RequestSendID =A.RefNumber and ddd.Pickup_Seq > a.Picking_Seq )) NextLocation	
	 ,TotalItem	
	 ,@CountData - @CountDataScan  Remaining	 ,
	 @CountProdID Womin,
	 Cast(@CountDataScan as varchar) + '/' + Cast( @CountData as varchar) PickingProgress

	  
 from #Tblmain A
	 Drop table #Tblmain
	 

END


 
GO
