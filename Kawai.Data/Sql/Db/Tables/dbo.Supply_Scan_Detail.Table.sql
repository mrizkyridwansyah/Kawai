SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Supply_Scan_Detail](
	[Seq_No] [numeric](18, 0) NOT NULL,
	[SJ_No] [char](25) NOT NULL,
	[Barcode_No] [varchar](100) NOT NULL,
	[Serial_No] [varchar](50) NULL,
	[Item_Code] [char](25) NULL,
	[Qty] [numeric](18, 5) NULL,
	[Scan_Cls] [char](1) NULL,
	[Register_User] [char](25) NULL,
	[Register_Date] [datetime] NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](25) NULL,
 CONSTRAINT [PK_Supply_Scan_Detail_1] PRIMARY KEY CLUSTERED 
(
	[Seq_No] ASC,
	[SJ_No] ASC,
	[Barcode_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
