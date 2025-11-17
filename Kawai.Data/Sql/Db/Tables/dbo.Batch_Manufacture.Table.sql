SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Batch_Manufacture](
	[Item_Code] [char](25) NOT NULL,
	[IDRMat] [numeric](18, 5) NULL,
	[JPYMat] [numeric](18, 5) NULL,
	[USDMat] [numeric](18, 5) NULL,
	[EURMat] [numeric](18, 5) NULL,
	[IDRCon] [numeric](18, 5) NULL,
	[USDCon] [numeric](18, 5) NULL,
	[AMPLI] [numeric](18, 5) NULL,
	[SPK1] [numeric](18, 5) NULL,
	[SPK2] [numeric](18, 5) NULL,
	[PCAT] [numeric](18, 5) NULL,
	[PDIA] [numeric](18, 5) NULL,
	[PINJ] [numeric](18, 5) NULL,
	[PMTM] [numeric](18, 5) NULL,
	[IDRDuty] [numeric](18, 5) NULL,
	[JPYDuty] [numeric](18, 5) NULL,
	[USDDuty] [numeric](18, 5) NULL,
	[Times] [numeric](10, 5) NULL,
	[Last_Batch] [datetime] NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_Batch_Manufacture] PRIMARY KEY CLUSTERED 
(
	[Item_Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [Batch_Manufacture] ADD  CONSTRAINT [DF_Batch_Manufacture_Register_Date]  DEFAULT (getdate()) FOR [Register_Date]
GO
