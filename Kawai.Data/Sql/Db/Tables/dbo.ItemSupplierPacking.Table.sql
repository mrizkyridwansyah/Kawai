SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ItemSupplierPacking](
	[ItemCode] [varchar](25) NOT NULL,
	[SupplierCode] [varchar](15) NOT NULL,
	[QtyPacking] [numeric](18, 9) NULL,
	[UnitCls] [varchar](2) NULL,
	[RegisterDate] [datetime] NOT NULL,
	[RegisterUser] [varchar](25) NOT NULL,
	[LastUpdate] [datetime] NULL,
	[LastUser] [varchar](25) NULL,
 CONSTRAINT [PK_ItemSupplierPacking] PRIMARY KEY CLUSTERED 
(
	[ItemCode] ASC,
	[SupplierCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
