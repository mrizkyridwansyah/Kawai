SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [PartMaterialRequestItemDetail](
	[IDSeq] [bigint] IDENTITY(1,1) NOT NULL,
	[RequestDetailID] [bigint] NOT NULL,
	[ItemCode] [char](25) NOT NULL,
	[unit_Cls] [char](2) NOT NULL,
	[ChildRequirement_Qty] [numeric](9, 2) NOT NULL,
	[Remarks] [varchar](255) NULL,
	[RegisterDate] [datetime] NOT NULL,
	[RegisterUser] [varchar](25) NULL,
	[LastUpdate] [datetime] NULL,
	[LastUser] [varchar](25) NULL,
PRIMARY KEY CLUSTERED 
(
	[IDSeq] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [PartMaterialRequestItemDetail]  WITH CHECK ADD FOREIGN KEY([ItemCode])
REFERENCES [Item_Master] ([Item_Code])
GO
ALTER TABLE [PartMaterialRequestItemDetail]  WITH CHECK ADD FOREIGN KEY([unit_Cls])
REFERENCES [Unit_Cls] ([Unit_Cls])
GO
