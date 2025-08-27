SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [InventoryCost_Item](
	[Item_Code] [char](25) NOT NULL,
	[Cost_Cls] [char](2) NOT NULL,
	[Start_Date] [char](8) NOT NULL,
	[End_Date] [char](8) NULL,
	[Currency_Code] [char](2) NULL,
	[Amount] [numeric](18, 5) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_InventoryCost_Item] PRIMARY KEY CLUSTERED 
(
	[Item_Code] ASC,
	[Cost_Cls] ASC,
	[Start_Date] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [InventoryCost_Item]  WITH CHECK ADD  CONSTRAINT [FK_InventoryCost_Item_InventoryCost_Master] FOREIGN KEY([Cost_Cls])
REFERENCES [InventoryCost_Master] ([Cost_Cls])
GO
ALTER TABLE [InventoryCost_Item] CHECK CONSTRAINT [FK_InventoryCost_Item_InventoryCost_Master]
GO
