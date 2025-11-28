SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [MS_BOMPerworkstation](
	[ParentItem_Code] [char](25) NOT NULL,
	[WorkStationCode] [char](15) NOT NULL,
	[ChildItem_Code] [char](25) NOT NULL,
	[Unit_Cls] [char](2) NOT NULL,
	[Qty] [numeric](9, 5) NOT NULL,
	[RegisterDate] [datetime] NOT NULL,
	[RegisterUser] [varchar](25) NULL,
	[LastUpdate] [datetime] NULL,
	[LastUser] [varchar](25) NULL
) ON [PRIMARY]
GO
ALTER TABLE [MS_BOMPerworkstation]  WITH CHECK ADD FOREIGN KEY([ChildItem_Code])
REFERENCES [Item_Master] ([Item_Code])
GO
ALTER TABLE [MS_BOMPerworkstation]  WITH CHECK ADD FOREIGN KEY([ParentItem_Code])
REFERENCES [Item_Master] ([Item_Code])
GO
ALTER TABLE [MS_BOMPerworkstation]  WITH CHECK ADD FOREIGN KEY([Unit_Cls])
REFERENCES [Unit_Cls] ([Unit_Cls])
GO
ALTER TABLE [MS_BOMPerworkstation]  WITH CHECK ADD FOREIGN KEY([WorkStationCode])
REFERENCES [MS_WorkStation] ([WorkStationCode])
GO
