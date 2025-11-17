SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [BOM_Master_Production](
	[Parent_ItemCode] [varchar](25) NOT NULL,
	[Description] [varchar](75) NULL,
	[Item_Code] [varchar](25) NOT NULL,
	[Item_Name] [varchar](75) NULL,
	[Qty] [numeric](18, 5) NOT NULL,
	[R_Qty] [numeric](18, 5) NULL,
	[Unit_Cls] [varchar](2) NULL,
	[Currency_Code] [varchar](2) NULL,
	[Accounting_Code] [varchar](7) NULL,
	[Start_Date] [varchar](8) NOT NULL,
	[End_date] [varchar](8) NULL,
	[Doc_No] [varchar](50) NULL,
	[DailySeq_No] [numeric](18, 0) NOT NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [varchar](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_BOM_Master_Production_1] PRIMARY KEY CLUSTERED 
(
	[Parent_ItemCode] ASC,
	[Item_Code] ASC,
	[DailySeq_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [BOM_Master_Production] ADD  CONSTRAINT [DF__BOM_Master___Qty__408A9835]  DEFAULT ((0)) FOR [Qty]
GO
ALTER TABLE [BOM_Master_Production] ADD  CONSTRAINT [DF__BOM_Maste__R_Qty__417EBC6E]  DEFAULT ((0)) FOR [R_Qty]
GO
ALTER TABLE [BOM_Master_Production] ADD  CONSTRAINT [DF_BOM_Master_Production_Register_Date]  DEFAULT (getdate()) FOR [Register_Date]
GO
ALTER TABLE [BOM_Master_Production]  WITH CHECK ADD  CONSTRAINT [FK_BOM_DailyProduction] FOREIGN KEY([DailySeq_No])
REFERENCES [Daily_Production] ([Seq_No])
GO
ALTER TABLE [BOM_Master_Production] CHECK CONSTRAINT [FK_BOM_DailyProduction]
GO
