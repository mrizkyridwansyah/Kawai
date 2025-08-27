SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [SupplyBom_Master](
	[Supply_No] [char](25) NOT NULL,
	[FromWH_Code] [char](25) NULL,
	[ToWH_Code] [char](25) NULL,
	[Supply_Date] [datetime] NULL,
	[register_date] [datetime] NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](10) NULL,
 CONSTRAINT [PK_SupplyBom_Master] PRIMARY KEY CLUSTERED 
(
	[Supply_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
