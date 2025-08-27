SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Quality_Check](
	[DN_No] [varchar](25) NOT NULL,
	[Item_Code] [varchar](25) NOT NULL,
	[Supplier_Code] [varchar](15) NOT NULL,
	[Receipt_Date] [datetime] NOT NULL,
	[Inspection_Date] [datetime] NULL,
	[Inspection_User] [varchar](15) NULL,
	[QC_Status] [varchar](2) NULL,
	[QC_Photo] [varchar](max) NULL,
	[Remarks] [varchar](6) NULL,
	[Status] [varchar](2) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [varchar](15) NULL,
	[Register_Date] [datetime] NULL,
	[Qty] [numeric](5, 0) NULL,
 CONSTRAINT [PK_Quality_Check] PRIMARY KEY CLUSTERED 
(
	[DN_No] ASC,
	[Item_Code] ASC,
	[Supplier_Code] ASC,
	[Receipt_Date] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
