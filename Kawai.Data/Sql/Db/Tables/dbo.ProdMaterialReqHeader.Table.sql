SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ProdMaterialReqHeader](
	[ParamKey] [varchar](100) NOT NULL,
	[ChildItemCode] [varchar](25) NOT NULL,
	[UnitCls] [varchar](25) NULL,
	[TotalReqQty] [numeric](18, 9) NOT NULL,
	[CurrentStock] [numeric](18, 9) NOT NULL,
	[Shortage] [numeric](18, 9) NOT NULL
) ON [PRIMARY]
GO
