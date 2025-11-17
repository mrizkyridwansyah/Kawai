SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [WareHouse_Master](
	[WH_Code] [char](15) NOT NULL,
	[WH_Name] [char](50) NULL,
	[Adm_Group] [char](15) NULL,
	[StockControl_Cls] [char](2) NULL,
	[Use_EndDay] [char](8) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
	[NG_Cls] [char](2) NULL,
	[Company_Code] [char](25) NULL,
 CONSTRAINT [PK_WareHouse_Master] PRIMARY KEY CLUSTERED 
(
	[WH_Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [WareHouse_Master] ADD  CONSTRAINT [DF_WareHouse_Master_Register_Date]  DEFAULT (getdate()) FOR [Register_Date]
GO
ALTER TABLE [WareHouse_Master]  WITH NOCHECK ADD  CONSTRAINT [FK_WareHouse_Master_Trade_Master] FOREIGN KEY([Adm_Group])
REFERENCES [Trade_Master] ([Trade_Code])
GO
ALTER TABLE [WareHouse_Master] CHECK CONSTRAINT [FK_WareHouse_Master_Trade_Master]
GO
