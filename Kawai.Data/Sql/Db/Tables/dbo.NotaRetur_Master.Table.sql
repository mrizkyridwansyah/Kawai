SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NotaRetur_Master](
	[NotaRetur_No] [char](15) NOT NULL,
	[NotaRetur_Type] [char](1) NULL,
	[Cust_Code] [char](15) NULL,
	[NotaRetur_Date] [datetime] NULL,
	[Remarks] [char](75) NULL,
	[Total_Qty] [numeric](18, 5) NULL,
	[Amount] [numeric](18, 5) NULL,
	[PPN] [numeric](18, 2) NULL,
	[Total_Amount] [numeric](22, 5) NULL,
	[Amount_IDR] [numeric](18, 2) NULL,
	[PPN_IDR] [numeric](18, 2) NULL,
	[User_Entry] [char](15) NULL,
	[Date_Entry] [datetime] NULL,
	[User_Update] [char](5) NULL,
	[Date_Update] [datetime] NULL,
 CONSTRAINT [PK_NotaRetur_Master] PRIMARY KEY CLUSTERED 
(
	[NotaRetur_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
