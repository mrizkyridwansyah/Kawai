SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [PartSupplyRequest_Master](
	[SupplyRec_No] [char](25) NOT NULL,
	[FromWarehouse_Code] [char](15) NULL,
	[Machine_No] [char](10) NULL,
	[ToWarehouse_Code] [char](15) NULL,
	[ChildSupply_Date] [datetime] NULL,
	[Supply_Cls] [char](2) NULL,
	[Auto_Cls] [char](1) NULL,
	[Request_Cls] [numeric](18, 0) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
	[womIn_No] [char](100) NULL,
	[WomIn_Status] [char](1) NULL,
 CONSTRAINT [PK_PartSupplyRequest_Master] PRIMARY KEY CLUSTERED 
(
	[SupplyRec_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
