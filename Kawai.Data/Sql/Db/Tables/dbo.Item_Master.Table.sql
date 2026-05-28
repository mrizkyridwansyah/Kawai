 
CREATE TABLE [dbo].[Item_Master](
	[Item_Code] [char](25) NOT NULL,
	[Item_Name] [char](75) NOT NULL,
	[FinishGoodPart_Cls] [char](2) NOT NULL,
	[Drawing_Number] [char](15) NULL,
	[WH_Code] [char](15) NOT NULL,
	[Address] [char](15) NULL,
	[Supplier_Code] [char](15) NOT NULL,
	[Delivery_Place] [char](15) NULL,
	[Manufacture_Code] [char](15) NULL,
	[Line_Code] [char](15) NULL,
	[Part_Cls] [char](2) NOT NULL,
	[MakerItem_Code] [char](30) NOT NULL,
	[Reserve_Cls] [char](2) NOT NULL,
	[Suply_Cls] [char](2) NOT NULL,
	[Provision_Cls] [char](2) NOT NULL,
	[Material_Cls] [char](2) NULL,
	[Thickness] [numeric](18, 5) NULL,
	[Width] [numeric](18, 5) NULL,
	[Length] [numeric](18, 5) NULL,
	[Weight] [numeric](18, 5) NULL,
	[GrossWeight] [numeric](18, 5) NULL,
	[SheetCoil_Cls] [char](2) NULL,
	[Pitch] [numeric](9, 2) NULL,
	[Number_Producible] [numeric](18, 4) NULL,
	[Scrap_Weight] [numeric](9, 2) NULL,
	[DrawingMaterial_Cls] [char](2) NULL,
	[SurfaceTreatment_Cls] [char](2) NULL,
	[Surface_OrderPointQty] [numeric](7, 0) NULL,
	[HeatTreatment_Cls] [char](2) NULL,
	[Heat_OrderPointQty] [numeric](7, 0) NULL,
	[Sample] [numeric](9, 2) NULL,
	[SW_Qty] [numeric](9, 2) NULL,
	[EW_Qty] [numeric](9, 2) NULL,
	[Number_Process] [numeric](2, 0) NULL,
	[Material_Coefficient] [numeric](9, 2) NULL,
	[Process_Coefficient] [numeric](9, 2) NULL,
	[Min_Lot] [numeric](7, 0) NULL,
	[Lot_Qty] [numeric](7, 0) NULL,
	[Lot_Coefficience] [numeric](7, 0) NULL,
	[Product_ReadTime] [numeric](2, 0) NULL,
	[Yield_Percentage] [numeric](5, 2) NULL,
	[Number_Entering] [numeric](7, 2) NULL,
	[PackingStyle_Cls] [char](2) NULL,
	[Group_Cls] [char](2) NULL,
	[Production_Cls] [char](2) NULL,
	[Standard_Stock] [numeric](10, 0) NULL,
	[Safety_Stock] [numeric](10, 0) NULL,
	[Max_Stock] [numeric](10, 0) NULL,
	[Min_Stock] [numeric](10, 0) NULL,
	[Alowance_Day] [numeric](2, 0) NULL,
	[Delivery_ReadTime] [numeric](2, 0) NULL,
	[MakeBuy_Cls] [char](2) NOT NULL,
	[Control_Cls] [char](2) NOT NULL,
	[OrderPoint_Qty] [numeric](10, 0) NULL,
	[Unit_Cls] [char](2) NULL,
	[Number_Box] [numeric](10, 0) NULL,
	[PackingStyleMaterial_Cls] [char](2) NULL,
	[Accounting_Code] [char](7) NULL,
	[Explosion_Cls] [char](2) NOT NULL,
	[PersonInCharge_Cls] [char](2) NULL,
	[StockControl_Cls] [char](2) NOT NULL,
	[SupplyIssue_Cls] [char](2) NULL,
	[Use_EndDay] [char](8) NULL,
	[HS_Code] [char](15) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
	[MinOrder] [numeric](10, 0) NULL,
	[Safety_Stock_Percentage] [numeric](5, 2) NULL,
	[SAP_Item_Code] [char](18) NULL,
	[TypeAccs] [char](2) NULL,
	[Model_Cls] [char](10) NULL,
	[POType_Cls] [varchar](2) NULL,
	[ClasificationPart_Cls] [varchar](2) NULL,
	[Destination_cls] [varchar](2) NULL,
	[Color_cls] [varchar](2) NULL,
	[Grouping_Class_Part_Code] [varchar](4) NULL,
 CONSTRAINT [PK_Item_Master] PRIMARY KEY CLUSTERED 
(
	[Item_Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Item_Master] ADD  CONSTRAINT [DF_Item_Master_SupplyIssue_Cls_1]  DEFAULT ('01') FOR [SupplyIssue_Cls]
GO

ALTER TABLE [dbo].[Item_Master] ADD  CONSTRAINT [DF_Item_Master_Register_Date]  DEFAULT (getdate()) FOR [Register_Date]
GO

ALTER TABLE [dbo].[Item_Master]  WITH NOCHECK ADD  CONSTRAINT [FK_Item_Master_Control_Cls] FOREIGN KEY([Control_Cls])
REFERENCES [dbo].[Control_Cls] ([Control_Cls])
ON UPDATE CASCADE
GO

ALTER TABLE [dbo].[Item_Master] CHECK CONSTRAINT [FK_Item_Master_Control_Cls]
GO

ALTER TABLE [dbo].[Item_Master]  WITH NOCHECK ADD  CONSTRAINT [FK_Item_Master_Trade_Master] FOREIGN KEY([Supplier_Code])
REFERENCES [dbo].[Trade_Master] ([Trade_Code])
ON UPDATE CASCADE
GO

ALTER TABLE [dbo].[Item_Master] CHECK CONSTRAINT [FK_Item_Master_Trade_Master]
GO

 
