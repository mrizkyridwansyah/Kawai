SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER VIEW [vw_Cls]
as
select rtrim('BCType_Cls') TypeData, BCType_Cls ClsCode, BCType_Cls Description from BCType_Cls			
union all
select rtrim('ClasificationPart_Cls') TypeData, ClasificationPart_Cls ClsCode, Description from ClasificationPart_Cls	
union all
select rtrim('Cls_Parameter') TypeData, Code ClsCode, Description from Cls_Parameter			
union all
select rtrim('Color_Cls') TypeData, Color_Cls ClsCode, Description from Color_Cls				
union all
select rtrim('Control_Cls') TypeData, Control_Cls ClsCode, Description from Control_Cls			
union all
select rtrim('Curr_Cls') TypeData, Curr_Cls ClsCode, Description from Curr_Cls				
union all
select rtrim('Department_Cls') TypeData, Department_Cls ClsCode, Description from Department_Cls		
union all
select rtrim('Destination_Cls') TypeData, Destination_Cls ClsCode, Description from Destination_Cls		
union all
select rtrim('DrawingMaterial_Cls') TypeData, DrawingMaterial_Cls ClsCode, Description from DrawingMaterial_Cls	
union all
select rtrim('Group_Cls') TypeData, Group_Cls ClsCode, Description from Group_Cls				
union all
select rtrim('HeatTreatment_Cls') TypeData, HeatTreatment_Cls ClsCode, Description from HeatTreatment_Cls		
union all
select rtrim('Insurance_Cls') TypeData, Insurance_Cls ClsCode, Description from Insurance_Cls			
--union all
--select rtrim('Masterlist_Part_cls') TypeData, '' ClsCode, nam Description from Masterlist_Part_cls	
union all
select rtrim('Material_Cls') TypeData, Material_Cls ClsCode, Description from Material_Cls			
union all
select rtrim('MaterialConsump_Cls') TypeData, MaterialConsump_Cls ClsCode, Description from MaterialConsump_Cls	
union all
select rtrim('Model_Cls') TypeData, Model_Cls ClsCode, Description from Model_Cls				
union all
select rtrim('Package_Cls') TypeData, Package_Cls ClsCode, Description from Package_Cls			
union all
select rtrim('PackingStyle_Cls') TypeData, PackingStyle_Cls ClsCode, Description from PackingStyle_Cls		
union all
select rtrim('PaymentCode_Cls') TypeData, PaymentCode_Cls ClsCode, Description from PaymentCode_Cls		
union all
select rtrim('PaymentTerm_Cls') TypeData, PaymentTerm_Cls ClsCode, Description from PaymentTerm_Cls		
union all
select rtrim('PersonInCharge_Cls') TypeData, PersonInCharge_Cls ClsCode, Description from PersonInCharge_Cls	
union all
select rtrim('PO_Cls') TypeData, PO_Cls ClsCode, Description from PO_Cls				
union all
select rtrim('POPacking_Cls') TypeData, POPacking_Cls ClsCode, Description from POPacking_Cls			
union all
select rtrim('POType_Cls') TypeData, POType_Cls ClsCode, Description from POType_Cls			
union all
select rtrim('PriceCondition_Cls') TypeData, PriceCondition_Cls ClsCode, Description from PriceCondition_Cls	
union all
select rtrim('Process_Cls') TypeData, Process_Cls ClsCode, Description from Process_Cls			
union all
select rtrim('Reason_Cls') TypeData, Reason_Cls ClsCode, Description from Reason_Cls			
union all
select rtrim('Region_Cls') TypeData, Region_Cls ClsCode, Description from Region_Cls			
union all
select rtrim('Remarks_Cls') TypeData, Remarks_Cls ClsCode, Description from Remarks_Cls			
union all
select rtrim('Section_Cls') TypeData, Section_Cls ClsCode, Description from Section_Cls			
union all
select rtrim('SheetCoil_Cls') TypeData, SheetCoil_Cls ClsCode, Description from SheetCoil_Cls			
union all
select rtrim('Status_Cls') TypeData, Status_Cls ClsCode, Description from Status_Cls			
union all
select rtrim('StopTime_Cls') TypeData, StopTime_Cls ClsCode, Description from StopTime_Cls			
union all
select rtrim('SurfaceTreatment_Cls') TypeData, SurfaceTreatment_Cls ClsCode, Description from SurfaceTreatment_Cls	
union all
select rtrim('Tax_Cls') TypeData, Tax_Code ClsCode, Tax_Name Description from Tax_Cls				
--union all
--select rtrim('TR_Cls') TypeData, '' ClsCode, Description from TR_Cls				
union all
select rtrim('Transport_Cls') TypeData, Transport_Cls ClsCode, Description from Transport_Cls			
union all
select rtrim('Transportation_Cls') TypeData, Transportation_Cls ClsCode, Description from Transportation_Cls	
union all
select rtrim('Unit_Cls') TypeData, Unit_Cls ClsCode, Description from Unit_Cls				
union all
select rtrim('WorkingLossTime_Cls') TypeData, WorkingLossTime_Cls ClsCode, Description from WorkingLossTime_Cls
union all
select  TypeData,   ClsCode, Description from NonCls
union all
select 'ItemFinishGoodCls' TypeData, Code, Description from vw_FinishGoodCls
union all
select 'ItemPartCls' TypeData, Code, Description from vw_PartCls
union all
select 'ItemReserveCls' TypeData, Code, Description from vw_ReserveCls 
union all
select 'ItemSupplyCls' TypeData, Code, Description from vw_SupplyCls 
union all
select 'ItemProvisionCls' TypeData, Code, Description from vw_ProvisionCls
union all
select 'ItemProductionCls' TypeData, Code, Description from vw_ProductionCls
union all
select 'ItemStockControlCls' TypeData, Code, Description from vw_StockControlCls 
union all
select 'ItemMakeOrBuyCls' TypeData, Code, Description from vw_MakeOrBuyCls 
union all
select 'ItemExplosionCls' TypeData, Code, Description from vw_ExplosionCls 
union all
select 'ItemTypeAccs' TypeData, Code, Description from vw_TypeAccs 
union all
select 'IQCResult' TypeData, Code, Description from vw_IQCResult
GO
