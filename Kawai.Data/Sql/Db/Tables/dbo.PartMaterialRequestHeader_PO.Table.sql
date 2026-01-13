SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [PartMaterialRequestHeader_PO](
	[RequestID] [bigint] IDENTITY(1,1) NOT NULL,
	[RequestNo] [varchar](50) NOT NULL,
	[RequestDate] [date] NOT NULL,
	[PO_NO] [char](25) NOT NULL,
	[Warehouse] [char](15) NOT NULL,
	[ProductionDate] [date] NOT NULL,
	[ParentItem_Code] [char](25) NOT NULL,
	[RequestSetQty] [numeric](9, 0) NOT NULL,
	[Status] [varchar](20) NOT NULL,
	[Remarks] [varchar](255) NULL,
	[RegisterDate] [datetime] NOT NULL,
	[RegisterUser] [varchar](25) NULL,
	[LastUpdate] [datetime] NULL,
	[LastUser] [varchar](25) NULL,
PRIMARY KEY CLUSTERED 
(
	[RequestID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[RequestNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [PartMaterialRequestHeader_PO]  WITH CHECK ADD FOREIGN KEY([ParentItem_Code])
REFERENCES [Item_Master] ([Item_Code])
GO
ALTER TABLE [PartMaterialRequestHeader_PO]  WITH CHECK ADD FOREIGN KEY([PO_NO])
REFERENCES [PurchaseOrder_Master] ([PO_No])
GO
