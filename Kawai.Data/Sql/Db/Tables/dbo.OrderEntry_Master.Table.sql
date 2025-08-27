SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [OrderEntry_Master](
	[Cust_Code] [char](15) NOT NULL,
	[PO_No] [char](35) NOT NULL,
	[Rev_No] [char](2) NULL,
	[PO_Date] [datetime] NULL,
	[Location_Code] [char](15) NULL,
	[Contact_Person] [char](25) NULL,
	[Fix_Cls] [char](1) NULL,
	[NoCommercial_Cls] [char](1) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_OrderEntry_Master] PRIMARY KEY CLUSTERED 
(
	[Cust_Code] ASC,
	[PO_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
