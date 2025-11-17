SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [InventoryCost_Group](
	[Group_Cls] [char](2) NOT NULL,
	[Cost_Cls] [char](2) NOT NULL,
	[Start_Date] [char](8) NOT NULL,
	[End_Date] [char](8) NULL,
	[Currency_Code] [char](2) NULL,
	[Amount] [numeric](18, 5) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_InventoryCost_Group] PRIMARY KEY CLUSTERED 
(
	[Group_Cls] ASC,
	[Cost_Cls] ASC,
	[Start_Date] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [InventoryCost_Group] ADD  CONSTRAINT [DF_InventoryCost_Group_Register_Date]  DEFAULT (getdate()) FOR [Register_Date]
GO
ALTER TABLE [InventoryCost_Group]  WITH CHECK ADD  CONSTRAINT [FK_InventoryCost_Group_Group_Cls] FOREIGN KEY([Group_Cls])
REFERENCES [Group_Cls] ([Group_Cls])
GO
ALTER TABLE [InventoryCost_Group] CHECK CONSTRAINT [FK_InventoryCost_Group_Group_Cls]
GO
ALTER TABLE [InventoryCost_Group]  WITH CHECK ADD  CONSTRAINT [FK_InventoryCost_Group_InventoryCost_Master] FOREIGN KEY([Cost_Cls])
REFERENCES [InventoryCost_Master] ([Cost_Cls])
GO
ALTER TABLE [InventoryCost_Group] CHECK CONSTRAINT [FK_InventoryCost_Group_InventoryCost_Master]
GO
