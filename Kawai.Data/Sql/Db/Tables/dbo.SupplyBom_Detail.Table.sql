SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [SupplyBom_Detail](
	[Supply_No] [char](25) NOT NULL,
	[Item_Code] [char](25) NOT NULL,
	[PO_NO] [char](25) NOT NULL,
	[PO_Date] [datetime] NULL,
	[Register_Date] [datetime] NULL,
	[Qty] [numeric](18, 5) NULL,
	[Unit_Cls] [char](2) NULL,
	[Supplier_Code] [char](25) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
 CONSTRAINT [PK_SupplyBom_Detail] PRIMARY KEY CLUSTERED 
(
	[Supply_No] ASC,
	[Item_Code] ASC,
	[PO_NO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
