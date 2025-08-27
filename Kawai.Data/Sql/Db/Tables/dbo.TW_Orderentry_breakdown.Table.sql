SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [TW_Orderentry_breakdown](
	[Cust_Code] [varchar](7) NULL,
	[Po_no] [varchar](15) NULL,
	[Delivery_date] [datetime] NULL,
	[Item_code] [varchar](16) NULL,
	[Serial_no] [varchar](8) NULL
) ON [PRIMARY]
GO
